using System;
using UnityEngine;
using TMPro;
using UniRx;

public abstract class CountdownPanelBase : MonoBehaviour
{
    private const float MIN_DENOMINATOR = 0.0001f;

    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _timeText;

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private IPanelAnimator _animator;
    
    public CanvasGroup Group => _group;
    
    protected virtual void Awake()
    {
        _animator = CreateAnimator();

        if (_animator != null)
            _animator.InitializeLayout();
        else
            InitializeGroupHidden();
    }

    protected virtual void OnDestroy()
    {
        disposables.Dispose();
        (_animator as IDisposable)?.Dispose();
    }
    
    protected void BeginCountdown(float seconds)
    {
        disposables.Clear();

        Show();

        float remaining = seconds;

        Observable.EveryUpdate().TakeWhile(_ => remaining > 0f).Subscribe(_ =>
            {
                remaining -= Time.deltaTime;
                
                float clamped = Mathf.Max(0f, remaining);

                if (_timeText != null)
                    _timeText.text = Format(clamped);

                float normalized = 1f - (clamped / Mathf.Max(seconds, MIN_DENOMINATOR));
                
                OnTick(clamped, normalized, seconds);
            },
            Hide).AddTo(disposables);
    }
    
    protected void Hide()
    {
        if (_animator != null)
            _animator.PlayHide();
        else
            HideGroup();

        disposables.Clear();
        
        OnHidden();
    }
    
    protected abstract IPanelAnimator CreateAnimator();
    
    protected virtual void OnTick(float secondsLeft, float normalized, float totalSeconds) { }

    protected virtual void OnShown() { }
    
    protected virtual void OnHidden() { }
    
    private string Format(float value)
    {
        int minutes = Mathf.FloorToInt(value / 60f);
        int seconds = Mathf.FloorToInt(value % 60f);
        
        return $"{minutes:00}:{seconds:00}";
    }
    
    private void InitializeGroupHidden()
    {
        if (_group == null)
            return;
        
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;
    }

    private void ShowGroup()
    {
        if (_group == null)
            return;
        
        _group.alpha = 1f;
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }
    
    private void Show()
    {
        if (_animator != null)
            _animator.PlayShow();
        else
            ShowGroup();

        OnShown();
    }

    private void HideGroup()
    {
        if (_group == null)
            return;
        
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;
    }
}