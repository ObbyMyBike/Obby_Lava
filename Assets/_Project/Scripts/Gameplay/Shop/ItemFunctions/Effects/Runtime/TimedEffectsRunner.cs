using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TimedEffectsRunner
{
    public event OnEffectLaunch OnEffectLaunched;
    
    private readonly GlobalCoroutineRunner runner;
    private readonly Dictionary<ItemType, Coroutine> activeCoroutines = new Dictionary<ItemType, Coroutine>();

    [Inject]
    public TimedEffectsRunner(GlobalCoroutineRunner runner)
    {
        this.runner = runner;
    }
    
    public void StartTimed(ItemType type, Sprite icon, float duration, Action onStart, Action onEnd)
    {
        if (activeCoroutines.TryGetValue(type, out Coroutine old))
        {
            runner.StopCoroutine(old);
            activeCoroutines.Remove(type);
        }

        onStart?.Invoke();
        OnEffectLaunched?.Invoke(type, icon, duration);
        
        if (runner != null && runner.isActiveAndEnabled)
        {
            Coroutine coroutine = runner.StartCoroutine(RunEffect(type, duration, onEnd));
            activeCoroutines[type] = coroutine;
        }
        else
        {
            onEnd?.Invoke();
        }
    }

    IEnumerator RunEffect(ItemType type, float duration, Action onEnd)
    {
        yield return new WaitForSeconds(duration);
        
        onEnd?.Invoke();
        
        activeCoroutines.Remove(type);
    }
}