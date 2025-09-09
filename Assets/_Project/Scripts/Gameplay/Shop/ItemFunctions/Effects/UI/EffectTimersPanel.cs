using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EffectTimersPanel : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private EffectTimerUI _timerPrefab;

    private readonly Dictionary<ItemType, EffectTimerUI> timersByType = new Dictionary<ItemType, EffectTimerUI>();

    private TimedEffectsRunner _timedEffectsRunner;
    
    [Inject]
    public void Construct(TimedEffectsRunner timedEffectsRunner)
    {
        _timedEffectsRunner = timedEffectsRunner;
    }
    
    private void OnEnable()
    {
        if (_timedEffectsRunner != null)
            _timedEffectsRunner.OnEffectLaunched += OnEffectLaunched;
    }

    private void OnDisable()
    {
        if (_timedEffectsRunner != null)
            _timedEffectsRunner.OnEffectLaunched -= OnEffectLaunched;
        
        foreach (KeyValuePair<ItemType, EffectTimerUI> pair in timersByType)
        {
            if (pair.Value != null)
                pair.Value.OnExpired -= OnTimerExpired;
        }
    }

    private void OnEffectLaunched(ItemType type, Sprite icon, float durationSeconds)
    {
        if (timersByType.TryGetValue(type, out EffectTimerUI existing))
        {
            if (existing != null)
            {
                existing.SetIcon(icon);
                existing.RestartCountdown(durationSeconds);
                
                return;
            }
        }
        
        EffectTimerUI timer = Instantiate(_timerPrefab, _container);
        timer.SetType(type);
        timer.TrySetupIcon(icon);
        
        timer.OnExpired += OnTimerExpired;
        
        timer.StartCountdown(durationSeconds);

        timersByType[type] = timer;
    }

    private void OnTimerExpired(ItemType type)
    {
        if (timersByType.TryGetValue(type, out EffectTimerUI ui))
        {
            if (ui != null)
                ui.OnExpired -= OnTimerExpired;

            timersByType.Remove(type);
        }
    }
}