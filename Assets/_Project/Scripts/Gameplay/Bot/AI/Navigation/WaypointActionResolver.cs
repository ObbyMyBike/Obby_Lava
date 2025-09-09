using UnityEngine;

public class WaypointActionResolver
{
    private readonly BotAiContext context;

    public WaypointActionResolver(BotAiContext context) => this.context = context;

    public WaypointActionResult HandleAtWaypoint(float deltaTime, Transform waypointTransform)
    {
        waypointTransform.TryGetComponent(out Waypoint waypoint);
        
        if (waypoint != null && waypoint.RequireClimb && context.Climb.TryBegin(waypoint.ClimbFacing, waypoint.ClimbLateralBounds))
        {
            context.Motion.IdlePose(deltaTime);
            
            return WaypointActionResult.BeganClimb;
        }

        context.Jump.TickCooldown(deltaTime);
        
        if (context.Failure.HasPendingSabotage)
        {
            if (waypoint != null && waypoint.RequireJump)
            {
                bool jumped = context.Jump.TryJumpNow(context.Agent.Animation);
                
                if (!jumped)
                    context.Jump.ForceJumpIfGrounded(context.Agent.Animation);
            }

            context.FailureDice.ApplyFatalKick(context.Agent);
            context.Failure.MarkSabotageApplied();
            context.Motion.IdlePose(deltaTime);
            
            return WaypointActionResult.Sabotaged;
        }
        
        if (waypoint != null && waypoint.RequireJump)
        {
            if (context.Jump.TryJumpNow(context.Agent.Animation))
            {
                context.Motion.IdlePose(deltaTime);
                
                return WaypointActionResult.DidJumpAndAdvance;
            }

            context.Motion.IdlePose(deltaTime);
            
            return WaypointActionResult.WaitingForJump;
        }

        return WaypointActionResult.None;
    }

    public void TickClimb(float deltaTime, out bool finished)
    {
        finished = false;

        context.Climb.Tick(deltaTime);
        context.Climb.TryExitIfGroundClose();

        bool grounded = context.Agent.Controller != null && context.Agent.Controller.isGrounded;
        context.Agent.Animation.UpdateAnimation(Vector3.zero, grounded);

        if (!context.Climb.IsClimbing)
            finished = true;
    }
}