using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EffectTimersPanel : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private EffectTimerUI _timerPrefab;

    private readonly Dictionary<ItemType, EffectTimerUI> ui = new();

    [Inject]
    public void Construct(TimedEffectsRunner timedEffectsRunner)
    {
        timedEffectsRunner.OnEffectLaunched += OnEffectsOnLaunched;
    }

    private void OnEffectsOnLaunched(ItemType type, Sprite icon, float duration)
    {
        EffectTimerUI timerUI = Instantiate(_timerPrefab, _container);
        
        timerUI.Setup(icon, duration);
        ui[type] = timerUI;
    }
}