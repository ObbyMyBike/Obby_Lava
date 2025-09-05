using System;
using Zenject;

public class PlayerDeathSubscription : IInitializable, IDisposable
{
    private readonly PlayerHealth _playerHealth;
    private readonly PlayerRespawn _respawnService;

    [Inject]
    public PlayerDeathSubscription(PlayerHealth playerHealth, PlayerRespawn respawnService)
    {
        _playerHealth = playerHealth;
        _respawnService = respawnService;
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
        _respawnService.RequestRespawn();
    }
}