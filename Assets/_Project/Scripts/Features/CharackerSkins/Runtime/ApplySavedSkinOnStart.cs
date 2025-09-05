using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class ApplySavedSkinOnStart : IInitializable, IDisposable
{
    private const string LOG_PREFIX = "[ApplySavedSkinOnStart] ";
    
    private readonly YGSkinsSaveRepository repository;
    private readonly DiContainer container;
    private readonly PlayerPrefabSpawn spawner;
    private readonly GlobalCoroutineRunner coroutineRunner;
    
    private bool _subscribed;

    public ApplySavedSkinOnStart(YGSkinsSaveRepository repository, DiContainer container, [InjectOptional] PlayerPrefabSpawn spawner, GlobalCoroutineRunner coroutineRunner)
    {
        this.repository = repository;
        this.container = container;
        this.spawner = spawner;
        this.coroutineRunner = coroutineRunner;
    }

    void IInitializable.Initialize()
    {
       
        if (spawner != null)
        {
            spawner.OnPlayerSpawned += OnPlayerSpawned;
            _subscribed = true;
            Debug.Log(LOG_PREFIX + "Subscribed to PlayerPrefabSpawn.OnPlayerSpawned");
        }
        
        SafeApplyWithDelay();
    }

    void IDisposable.Dispose()
    {
        if (_subscribed && spawner != null)
            spawner.OnPlayerSpawned -= OnPlayerSpawned;
        
        _subscribed = false;
    }

    private void OnPlayerSpawned(Transform _)
    {
        SafeApplyWithDelay();

        if (_subscribed && spawner != null)
            spawner.OnPlayerSpawned -= OnPlayerSpawned;
        
        _subscribed = false;
    }

    private void SafeApplyWithDelay()
    {
        if (coroutineRunner == null)
        {
            ApplyNow();
            return;
        }

        coroutineRunner.StartCoroutine(WaitAndApply());
    }

    private IEnumerator WaitAndApply()
    {
        const float TIMEOUT = 2f;       // можно увеличить, если нужно
        float t = 0f;

        PlayerSkinApplier applier = null;

        // Пере-Resolve безопасен — Zenject отдаст тот же singleton
        while (t < TIMEOUT)
        {
            if (applier == null)
                applier = container.Resolve<PlayerSkinApplier>();

            if (applier != null && applier.IsReady)
                break;

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (applier == null || !applier.IsReady)
        {
            Debug.LogWarning(LOG_PREFIX + "Apply skipped: SkinApplier not ready after timeout");
            yield break;
        }

        var selected = repository.SelectedSkin;
        Debug.Log(LOG_PREFIX + $"ApplyNow -> SelectedSkin={selected}");
        applier.ApplySkin(selected);
    }
    
    private void ApplyNow()
    {
        PlayerSkinApplier applier = container.Resolve<PlayerSkinApplier>();
        SkinIdType selected = repository.SelectedSkin;
        Debug.Log(LOG_PREFIX + $"ApplyNow -> SelectedSkin={selected}");
        applier.ApplySkin(selected);
    }
}