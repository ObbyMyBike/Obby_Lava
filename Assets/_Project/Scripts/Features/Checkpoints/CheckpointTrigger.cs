using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class CheckpointTrigger : MonoBehaviour
{
    private const bool REQUIRE_TRIGGER = true;

    private readonly float progressOverride = float.NaN;
    private readonly bool singleUse = true;
    
    private PlayerRespawn _respawn;
    private Collider _collider;
    private bool _activated;
    
    [Inject]
    public void Construct(PlayerRespawn respawn)
    {
        _respawn = respawn;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        
        if (REQUIRE_TRIGGER)
            _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_activated && singleUse)
            return;
        
        if (!other.TryGetComponent(out Player _))
            return;

        float progress = float.IsNaN(progressOverride) ? transform.position.y : progressOverride;
        bool accepted = _respawn.TrySetCheckpoint(transform, progress, false);
        
        if (accepted && singleUse)
            _activated = true;
    }
    
    public void MarkActivated() => _activated = true;
}