using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class CheckpointTrigger : MonoBehaviour
{
    private const bool REQUIRE_TRIGGER = true;

    private readonly float progressOverride = float.NaN;
    private readonly bool singleUseForPlayer = true;
    private readonly bool singleUseForBots = false;
    
    private PlayerRespawn _playerRespawn;
    private BotRespawnDirectory _botRespawnDirectory; 
    private Collider _collider;
    private bool _activatedByPlayer;
    
    [Inject]
    public void Construct(PlayerRespawn respawn, BotRespawnDirectory respawnDirectory)
    {
        _playerRespawn = respawn;
        _botRespawnDirectory = respawnDirectory;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        
        if (REQUIRE_TRIGGER)
            _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        float progress = float.IsNaN(progressOverride) ? transform.position.y : progressOverride;
        
        if (other.TryGetComponent(out Player _))
        {
            if (singleUseForPlayer && _activatedByPlayer)
                return;

            bool acceptedByPlayer = _playerRespawn != null && _playerRespawn.TrySetCheckpoint(transform, progress, false);

            if (acceptedByPlayer && singleUseForPlayer)
                _activatedByPlayer = true;
            
            return;
        }
        
        BotAgent botAgent = other.GetComponentInParent<BotAgent>();
        
        if (botAgent != null && _botRespawnDirectory != null && _botRespawnDirectory.TryGet(botAgent, out BotRespawn botRespawn))
        {
            if (singleUseForBots && _activatedByPlayer)
                return;

            bool wasAcceptedByBot = botRespawn.TrySetCheckpoint(transform, progress);
            
            if (wasAcceptedByBot)
            {
                if (singleUseForBots)
                    _activatedByPlayer = true;
            }
        }
    }
    
    public void MarkActivated() => _activatedByPlayer = true;
}