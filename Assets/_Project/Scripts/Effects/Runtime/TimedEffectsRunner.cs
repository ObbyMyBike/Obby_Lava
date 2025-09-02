using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TimedEffectsRunner
{
    public delegate void OnEffectPlayed(ItemType type, Sprite icon, float duration);
    
    public event OnEffectPlayed OnEffectLaunched;
    
    private readonly MonoBehaviour runner;
    private readonly Dictionary<ItemType, Coroutine> active = new();

    [Inject]
    public TimedEffectsRunner([Inject(Id = "CoroutineRunner")] MonoBehaviour runner)
    {
        this.runner = runner;
    }
    
    public void StartTimed(ItemType type, Sprite icon, float duration, Action onStart, Action onEnd)
    {
        if (active.TryGetValue(type, out Coroutine old))
        {
            runner.StopCoroutine(old);
            active.Remove(type);
        }

        onStart?.Invoke();
        OnEffectLaunched?.Invoke(type, icon, duration);
        
        Coroutine startCoroutine = runner.StartCoroutine(RunEffect(type, duration, onEnd));
        active[type] = startCoroutine;
    }

    IEnumerator RunEffect(ItemType type, float duration, Action onEnd)
    {
        yield return new WaitForSeconds(duration);
        
        onEnd?.Invoke();
        
        active.Remove(type);
    }
}