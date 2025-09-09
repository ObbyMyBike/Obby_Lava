using UnityEngine;

public class LadderPlaneCoordinator
{
    private readonly CharacterController controller;
    private readonly LadderSettingsConfig config;

    public LadderPlaneCoordinator(CharacterController controller, LadderSettingsConfig config)
    {
        this.controller = controller;
        this.config = config;
    }

    public void AlignRotationToLadder(Transform ladderFacing)
    {
        Vector3 faceDirection = -ladderFacing.forward;
        faceDirection.y = 0f;

        if (faceDirection.sqrMagnitude > 1e-4f)
            controller.transform.rotation = Quaternion.LookRotation(faceDirection);
    }

    public void SnapToDesiredPlaneOffset(Transform ladderFacing)
    {
        Vector3 origin = ladderFacing.position;
        Vector3 normal = ladderFacing.forward;

        float signed = Vector3.Dot(controller.transform.position - origin, normal);
        float targetSigned = -config.ClimbPlaneOffset;
        float delta = targetSigned - signed;

        if (Mathf.Abs(delta) > 1e-4f)
            controller.Move(normal * delta);
    }

    public void MaintainDesiredPlaneOffset(Transform ladderFacing)
    {
        Vector3 origin = ladderFacing.position;
        Vector3 normal = ladderFacing.forward;

        float signed = Vector3.Dot(controller.transform.position - origin, normal);
        float targetSigned = -config.ClimbPlaneOffset;
        float delta = targetSigned - signed;

        if (Mathf.Abs(delta) > config.ClimbPlaneOffsetEps)
        {
            float maxStep = Mathf.Max(config.ForwardClearEps, 0.005f);
            float step = Mathf.Clamp(delta, -maxStep, maxStep);
            
            controller.Move(normal * step);
        }
    }
}