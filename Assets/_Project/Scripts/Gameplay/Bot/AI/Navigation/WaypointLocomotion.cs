using UnityEngine;

public class WaypointLocomotion
{
    private readonly BotAiContext context;

    public WaypointLocomotion(BotAiContext context) => this.context = context;

    public void MoveTowards(Transform waypointTransform, float deltaTime)
    {
        if (waypointTransform == null)
        {
            context.Motion.IdlePose(deltaTime);
            
            return;
        }

        Vector3 toTarget = waypointTransform.position - context.Agent.Controller.transform.position;
        Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z);
        Vector3 direction = (flat.sqrMagnitude > BotAiConstants.ZERO_TOLERANCE) ? flat.normalized : Vector3.zero;

        Vector3 separation = context.Steering.ComputeSeparation(context.Agent, context.Registry.Active, context.Config.MinDistanceBetweenBots);
        
        if (separation.sqrMagnitude > 0f)
        {
            Vector3 blend = direction + separation * context.Config.SeparationWeight;
            
            if (blend.sqrMagnitude > BotAiConstants.ZERO_TOLERANCE)
                direction = blend.normalized;
        }

        context.Agent.FaceTowards(direction, context.Config.RotationSpeed);
        context.Jump.TickCooldown(deltaTime);
        context.Motion.MoveAndAnimate(direction, deltaTime);
    }
}