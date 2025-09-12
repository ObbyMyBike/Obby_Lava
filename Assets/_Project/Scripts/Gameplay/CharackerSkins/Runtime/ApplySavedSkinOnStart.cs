using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class ApplySavedSkinOnStart : IInitializable, IDisposable
{
    private const float TIMEOUT = 2f;
    
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

        float time = 0f;

        PlayerSkinApplier applier = null;
        
        while (time < TIMEOUT)
        {
            if (applier == null)
                applier = container.Resolve<PlayerSkinApplier>();

            if (applier != null && applier.IsReady)
                break;

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        if (applier == null || !applier.IsReady)
            yield break;

        SkinIdType selected = repository.SelectedSkin;


        applier.ApplySkin(selected);
    }
    
    private void ApplyNow()
    {
        PlayerSkinApplier applier = container.Resolve<PlayerSkinApplier>();
        SkinIdType selected = repository.SelectedSkin;
        applier.ApplySkin(selected);
    }
}