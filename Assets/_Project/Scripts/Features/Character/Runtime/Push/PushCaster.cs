using UnityEngine;

public class PushCaster
{
    private const float RAYCAST_ORIGIN_UP_OFFSET = 0.5f;

    public event OnUsed OnPushUsed;

    private readonly CharacterController controller;
    private readonly PushInteraction pushService;

    private readonly float pushRange;
    private readonly float pushRadius;
    private readonly float pushCooldown;

    private float _lastPushTime = -Mathf.Infinity;
    
    public PushCaster(CharacterController controller, PushInteraction pushService, float pushRange, float pushRadius, float pushCooldown)
    {
        this.controller = controller;
        this.pushService = pushService;
        this.pushRange = pushRange;
        this.pushRadius = pushRadius;
        this.pushCooldown = pushCooldown;
    }
    
    public void TryPush()
    {
        if (Time.time < _lastPushTime + pushCooldown)
            return;

        _lastPushTime = Time.time;
        
        OnPushUsed?.Invoke();

        Vector3 origin = controller.transform.position + Vector3.up * RAYCAST_ORIGIN_UP_OFFSET;
        pushService.PushNearest(origin, pushRadius, pushRange);
    }
}