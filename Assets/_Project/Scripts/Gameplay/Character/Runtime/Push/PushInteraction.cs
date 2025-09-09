using UnityEngine;
using Zenject;

public class PushInteraction
{
    private const float MIN_SQR_DISTANCE = 0.0001f;
    private const int MAX_COLLIDERS = 32;
    
    private readonly Collider[] colliderBuffer = new Collider[MAX_COLLIDERS];
    private readonly SpawnedPlayerAccessor accessor;
    private readonly LayerMask pushableMask;
    private readonly float pushForce;
    
    public PushInteraction(SpawnedPlayerAccessor accessor, LayerMask pushableMask, float pushForce)
    {
        this.accessor = accessor;
        this.pushableMask = pushableMask;
        this.pushForce = pushForce;
    }
    
    public bool PushNearest(Vector3 origin, float innerRadius, float outerRadius)
    {
        Transform playerTransform = accessor.Transform;
        if (playerTransform == null)
            return false;

        PushableRigidbodyAdapter target = FindNearest(origin, innerRadius, playerTransform.position);
        
        if (target == null)
            target = FindNearest(origin, outerRadius, playerTransform.position);

        if (target == null)
            return false;

        ApplyHorizontalImpulse(target, playerTransform.position);
        return true;
    }

    public int TryPush(Vector3 origin, float radius, int maxCount)
    {
        Transform playerTransform = accessor.Transform;
        
        if (playerTransform == null)
            return 0;

        int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, colliderBuffer, pushableMask, QueryTriggerInteraction.Collide);
        
        if (hitCount == 0)
            return 0;
        
        Vector3 playerPos = playerTransform.position;
        int pushed = 0;
        
        for (int i = 0; i < hitCount; i++)
        {
            Collider collider = colliderBuffer[i];
            
            if (collider == null)
                continue;

            if (!collider.TryGetComponent(out PushableRigidbodyAdapter pushable))
                continue;

            Vector3 delta = collider.transform.position - playerPos;
            delta.y = 0f;
            
            if (delta.sqrMagnitude <= MIN_SQR_DISTANCE)
                continue;

            Vector3 impulse = delta.normalized * pushForce;
            pushable.TryPush(impulse);

            pushed++;
            
            if (pushed >= maxCount)
                break;
        }

        return pushed;
    }
    
    private PushableRigidbodyAdapter FindNearest(Vector3 origin, float radius, Vector3 playerPosition)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(origin, radius, colliderBuffer, pushableMask, QueryTriggerInteraction.Collide);
        
        if (hitCount == 0)
            return null;

        PushableRigidbodyAdapter nearest = null;
        float nearestSqr = float.PositiveInfinity;

        for (int i = 0; i < hitCount; i++)
        {
            Collider collider = colliderBuffer[i];
            
            if (collider == null)
                continue;

            if (!collider.TryGetComponent(out PushableRigidbodyAdapter pushable))
                continue;

            Vector3 delta = collider.transform.position - playerPosition;
            delta.y = 0f;
            float sqr = delta.sqrMagnitude;
            
            if (sqr <= MIN_SQR_DISTANCE)
                continue;

            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearest = pushable;
            }
        }

        return nearest;
    }

    private void ApplyHorizontalImpulse(PushableRigidbodyAdapter target, Vector3 playerPosition)
    {
        Vector3 delta = target.transform.position - playerPosition;
        delta.y = 0f;
        
        if (delta.sqrMagnitude <= MIN_SQR_DISTANCE)
            return;

        Vector3 impulse = delta.normalized * pushForce;
        target.TryPush(impulse);
    }
}