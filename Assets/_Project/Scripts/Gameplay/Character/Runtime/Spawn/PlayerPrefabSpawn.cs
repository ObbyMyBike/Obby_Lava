using UnityEngine;
using Zenject;

public class PlayerPrefabSpawn : IInitializable
{
    public event OnPlayerSpawn OnPlayerSpawned;

    private readonly DiContainer container;
    private readonly SpawnedPlayerAccessor accessor;
    private readonly Player playerPrefab;
    private readonly Transform startSpawnPoint;
    private readonly Transform parentForPlayer;

    public PlayerPrefabSpawn(DiContainer container, SpawnedPlayerAccessor accessor, Player playerPrefab, Transform startSpawnPoint, [InjectOptional] Transform parentForPlayer)
    {
        this.container = container;
        this.accessor = accessor;
        this.playerPrefab = playerPrefab;
        this.startSpawnPoint = startSpawnPoint;
        this.parentForPlayer = parentForPlayer;
    }

    void IInitializable.Initialize()
    {
        if (playerPrefab == null || startSpawnPoint == null)
            return;
        
        Player spawnedPlayer  = container.InstantiatePrefabForComponent<Player>(playerPrefab, startSpawnPoint.position, startSpawnPoint.rotation, parentForPlayer);

        if (spawnedPlayer  == null)
            return;

        bool hadCharacterController = spawnedPlayer.TryGetComponent(out CharacterController characterController);
        bool wasEnabled = hadCharacterController && characterController.enabled;

        if (hadCharacterController && wasEnabled)
            characterController.enabled = false;

        spawnedPlayer.transform.SetPositionAndRotation(startSpawnPoint.position, startSpawnPoint.rotation);

        if (hadCharacterController)
            characterController.enabled = wasEnabled;

        accessor.Set(spawnedPlayer);

        OnPlayerSpawned?.Invoke(spawnedPlayer.transform);
    }
}