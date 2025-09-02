using UnityEngine;

public class ForwardPushCaster
{
    private static readonly float RAYCAST_ORIGIN_UP_OFFSET = 0.5f;
    
    public event OnUsed OnPushUsed;

    private readonly CharacterController controller;
    
    private readonly float pushForce;
    private readonly float pushRange;
    private readonly float pushRadius;
    private readonly float pushCooldown;

    private float _lastPushTime = -Mathf.Infinity;

    public ForwardPushCaster(CharacterController controller, float pushForce, float pushRange, float pushRadius, float pushCooldown) 
    {
        this.controller = controller;
        this.pushForce = pushForce;
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

        Vector3 origin = controller.transform.position + Vector3.up * 0.5f;
        Vector3 direction = controller.transform.forward;

        if (Physics.SphereCast(origin, pushRadius, direction, out RaycastHit hit, pushRange))
        {
            if (hit.collider.TryGetComponent(out IPushable pushable))
                pushable.TryPush(direction * pushForce);
        }
    }
    
    public void DrawGizmos()
    {
        if (controller == null)
            return;

        Vector3 origin = controller.transform.position + Vector3.up * RAYCAST_ORIGIN_UP_OFFSET;
        Vector3 forward = controller.transform.forward;
        Vector3 endPoint = origin + forward * pushRange;
        
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawWireSphere(origin, pushRadius);
        Gizmos.DrawWireSphere(endPoint, pushRadius);
        Gizmos.DrawLine(origin, endPoint);
    }
}