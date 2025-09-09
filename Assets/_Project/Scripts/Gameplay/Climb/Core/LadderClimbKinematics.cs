using UnityEngine;

public class LadderClimbKinematics
{
    private readonly CharacterController controller;
    private readonly LadderSettingsConfig config;

    public LadderClimbKinematics(CharacterController controller, LadderSettingsConfig config)
    {
        this.controller = controller;
        this.config = config;
    }

    public void ApplyVertical(float climbSigned, float deltaTime)
    {
        if (Mathf.Abs(climbSigned) <= 0.001f)
            return;

        controller.Move(Vector3.up * (climbSigned * config.ClimbSpeed * deltaTime));
    }

    public bool TryApplyLateral(Transform ladderFacing, Collider lateralBounds, float lateral, float deltaTime)
    {
        if (Mathf.Abs(lateral) <= 0.10f || config.ClimbSideSpeed <= 0f)
            return true;

        Vector3 step = ladderFacing.right * (lateral * config.ClimbSideSpeed * deltaTime);
        Vector3 next = controller.transform.position + step;
        bool inside = lateralBounds == null || lateralBounds.bounds.Contains(next);

        if (!inside)
            return false;

        controller.Move(step);
        
        return true;
    }
}