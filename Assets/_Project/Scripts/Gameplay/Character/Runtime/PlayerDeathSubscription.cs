using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Zenject;

public class PlayerDeathSubscription : IInitializable, IDisposable
{
    private readonly PlayerHealth _playerHealth;
    private readonly PlayerRespawn _respawnService;
    private readonly SpawnedPlayerAccessor _playerAccessor;
    private readonly YGSkinsSaveRepository _skinsRepo;
    private readonly GlobalCoroutineRunner _coroutineRunner;

    [Inject]
    public PlayerDeathSubscription(
        PlayerHealth playerHealth, 
        PlayerRespawn respawnService, 
        SpawnedPlayerAccessor playerAccessor,
        YGSkinsSaveRepository skinRepo,
        GlobalCoroutineRunner coroutineRunnter)
    {
        _playerHealth = playerHealth;
        _respawnService = respawnService;
        _playerAccessor = playerAccessor;
        _skinsRepo = skinRepo;
        _coroutineRunner = coroutineRunnter;
    }

    void IInitializable.Initialize()
    {
        _playerHealth.OnDied += OnPlayerDied;
    }

    void IDisposable.Dispose()
    {
        _playerHealth.OnDied -= OnPlayerDied;
    }
    
    private void OnPlayerDied()
    {
        var destructor = _playerAccessor.Player.Destructor;
        destructor.Destruct(_skinsRepo.SelectedSkin);
        _playerAccessor.Player.gameObject.SetActive(false);
        _coroutineRunner.StartCoroutine(WaitDestruction(destructor.DestructionTime));

    }

    private IEnumerator WaitDestruction(float time)
    {
        yield return new WaitForSeconds(time);
        _playerAccessor.Player.gameObject.SetActive(true);
        _respawnService.RequestRespawn();
    }
}