using UnityEngine;

public class LadderClimbExitEvaluator
{
    private readonly CharacterController controller;
    private readonly LadderSettingsConfig config;
    private readonly LayerMask groundMask;
    
    public LadderClimbExitEvaluator(CharacterController controller, LayerMask groundMask, LadderSettingsConfig config)
    {
        this.controller = controller;
        this.groundMask = groundMask;
        this.config = config;
    }

    public bool IsGroundClose()
    {
        if (controller.isGrounded)
            return true;

        Bounds bounds = controller.bounds;
        float skin = controller.skinWidth;
        Vector3 feet = new Vector3(bounds.center.x, bounds.min.y + skin + 0.005f, bounds.center.z);
        float maxTouchDist = skin + 0.01f;

        bool hit = Physics.Raycast(feet, Vector3.down, out _, maxTouchDist, groundMask, QueryTriggerInteraction.Ignore);
        
        return hit;
    }

    public bool IsReadyForTopExit(float enterCenterY, float timeSinceEnter)
    {
        float deltaY = controller.bounds.center.y - enterCenterY;
        bool enoughY = deltaY >= config.MinDeltaYBeforeTopExit;
        bool enoughTime = timeSinceEnter >= config.MinTimeBeforeTopExit;
        
        return enoughY && enoughTime;
    }

    public bool TrySoftTopExit(Transform ladderFacing)
    {
        Vector3 start = controller.bounds.center + Vector3.up * config.TopUpOffset;
        Vector3 forwardToPlatform = -ladderFacing.forward;
        Vector3 probe = start + forwardToPlatform * config.TopForwardProbeDistance;

        if (!Physics.Raycast(probe, Vector3.down, out var downHit, config.TopDowncastDistance, groundMask, QueryTriggerInteraction.Ignore))
            return false;

        float upDot = Vector3.Dot(downHit.normal, Vector3.up);
        
        if (upDot < config.TopMinUpDot)
            return false;

        float skin = controller.skinWidth;
        Bounds bounds = controller.bounds;
        float currentFeetY = bounds.min.y;
        float desiredFeetY = downHit.point.y + skin + 0.02f;
        float upDelta = Mathf.Max(desiredFeetY - currentFeetY, config.TopExtraUpNudge);

        controller.Move(Vector3.up * upDelta);

        float forwardClearMin = controller.radius + config.ForwardClearEps;
        controller.Move(forwardToPlatform * forwardClearMin);

        return true;
    }
}