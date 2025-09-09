using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawnExecutor : MonoBehaviour
{
    private SpawnedPlayerAccessor _accessor;
    private PlayerHealth _playerHealth;
    private PlayerRespawn _playerRespawn;
    private CharacterController _characterController;

    [Inject]
    public void Construct(SpawnedPlayerAccessor accessor, PlayerHealth playerHealth, PlayerRespawn playerRespawn)
    {
        _accessor = accessor;
        _playerHealth = playerHealth;
        _playerRespawn = playerRespawn;
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (_playerRespawn != null)
            _playerRespawn.OnRespawnRequested += OnRespawnRequested;
    }
    
    private void OnDisable()
    {
        if (_playerRespawn != null)
            _playerRespawn.OnRespawnRequested -= OnRespawnRequested;
    }

    private void OnRespawnRequested()
    {
        Transform spawnPoint = _playerRespawn.GetCurrentSpawnPoint();
        Player player = _accessor.Player;
        CharacterController controller = _accessor.CharacterController ?? _characterController;

        if (spawnPoint == null || player == null || controller == null)
            return;

        bool wasEnabled = controller.enabled;
        
        if (wasEnabled)
            controller.enabled = false;

        Transform playerTransform = player.transform;
        playerTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        Physics.SyncTransforms();

        if (wasEnabled)
            controller.enabled = true;

        _playerHealth?.RestoreToMax();
    }
}