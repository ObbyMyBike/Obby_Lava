using System;
using UniRx;
using UnityEngine;
using Zenject;

public class LavaProgressionFlow : IInitializable, IDisposable
{
    private const float EPSILON = 0.0001f;
    
    public event OnLavaPreCountdownDisplay OnPreCountdownDisplayed;
    public event OnLavaCountdownDisplay OnCountdownDisplayed;
    public event OnLavaRiseStart OnRiseStarted;
    public event OnLavaHeightChange OnHeightChanged;
    public event OnLavaRiseComplete OnRiseCompleted;
    public event OnLavaPhaseChange OnPhaseChanged;
    
    private readonly LavaProgressionConfig config;
    private readonly ILavaSurface surface;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private LavaPhaseType _phaseType = LavaPhaseType.Idle;
    private float _currentHeight;
    private int _currentStepIndex = -1;

    public LavaProgressionFlow(LavaProgressionConfig config, ILavaSurface surface, float initialHeight = 0f)
    {
        this.config = config;
        this.surface = surface;
        _currentHeight = initialHeight;
    }
    
    void IInitializable.Initialize()
    {
        SetPhase(LavaPhaseType.Idle);
        
        if (surface is LavaSurfaceView view)
            view.SetBaseY(config.BaseSurfaceY);
    }

    void IDisposable.Dispose()
    {
        disposables.Clear();
    }

    public void StartSequence()
    {
        if (config.Steps.Count == 0)
        {
            SetPhase(LavaPhaseType.Completed);
            
            return;
        }

        _currentStepIndex = -1;
        
        ProceedToNextStep();
    }

    private void ProceedToNextStep()
    {
        _currentStepIndex++;

        if (_currentStepIndex >= config.Steps.Count)
        {
            SetPhase(LavaPhaseType.Completed);
            
            return;
        }
        
        LavaStepConfig step = config.Steps[_currentStepIndex];
        float waitBefore = step.WaitBeforeCountdownSeconds > 0f ? step.WaitBeforeCountdownSeconds : config.DefaultWaitBeforeCountdownSeconds;
        float countdown = step.CountdownDurationSeconds > 0f ? step.CountdownDurationSeconds : config.DefaultCountdownDurationSeconds;
        
        SetPhase(LavaPhaseType.Waiting);
        
        OnPreCountdownDisplayed?.Invoke(waitBefore);
        
        Observable.Timer(TimeSpan.FromSeconds(waitBefore)).ObserveOnMainThread().Subscribe(_ =>
            {
                SetPhase(LavaPhaseType.Countdown);
                
                OnCountdownDisplayed?.Invoke(countdown);
                
                Observable.Timer(TimeSpan.FromSeconds(countdown)).ObserveOnMainThread().Subscribe(__ => StartRise(step)).AddTo(disposables);
            }).AddTo(disposables);
    }

    private void StartRise(LavaStepConfig step)
    {
        float target = step.TargetHeight;
        float duration = step.RiseDurationSeconds > 0f ? step.RiseDurationSeconds : config.DefaultRiseDurationSeconds;
        AnimationCurve curve = step.RiseCurve != null && step.RiseCurve.length > 0 ? step.RiseCurve : config.DefaultRiseCurve;
        
        SetPhase(LavaPhaseType.Rising);
        
        OnRiseStarted?.Invoke();
        
        float startHeight = _currentHeight;
        float elapsed = 0f;
        
        Observable.EveryUpdate().TakeWhile(_ => elapsed < duration - EPSILON).DoOnSubscribe(() => { }).DoOnCancel(() => { }).Subscribe(_ =>
                {
                    elapsed += Time.deltaTime;
                    float time = Mathf.Clamp01(elapsed / Mathf.Max(duration, EPSILON));
                    float eased = curve.Evaluate(time);
                    _currentHeight = Mathf.Lerp(startHeight, target, eased);

                    surface.ApplyHeight(_currentHeight);
                    
                    OnHeightChanged?.Invoke(_currentHeight);
                }, 
                () =>
                {
                    _currentHeight = target;
                    surface.ApplyHeight(_currentHeight);
                    
                    OnHeightChanged?.Invoke(_currentHeight);
                    OnRiseCompleted?.Invoke(_currentHeight);

                    StartHold(step);
                }).AddTo(disposables);
    }

    private void StartHold(LavaStepConfig step)
    {
        float hold = step.HoldDurationSeconds > 0f ? step.HoldDurationSeconds : config.DefaultHoldDurationSeconds;
        
        SetPhase(LavaPhaseType.Holding);
        
        Observable.Timer(TimeSpan.FromSeconds(hold)).ObserveOnMainThread().Subscribe(_ => ProceedToNextStep()).AddTo(disposables);
    }

    private void SetPhase(LavaPhaseType phase)
    {
        _phaseType = phase;
        
        OnPhaseChanged?.Invoke(_phaseType, _currentStepIndex);
    }
}