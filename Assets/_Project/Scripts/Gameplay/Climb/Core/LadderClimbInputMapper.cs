using UnityEngine;

public class LadderClimbInputMapper
{
    private const float INPUT_DEADZONE = 0.05f;

    public LadderClimbInput MapToLadderLocal(Vector3 moveDirectionWorld, Transform ladderFacing)
    {
        Vector3 planar = Vector3.ProjectOnPlane(moveDirectionWorld, Vector3.up);

        if (planar.sqrMagnitude < INPUT_DEADZONE * INPUT_DEADZONE)
            return new LadderClimbInput { ClimbSigned = 0f, Lateral = 0f };

        Vector3 local = ladderFacing.InverseTransformDirection(planar).normalized;
        float into = Mathf.Clamp01(-local.z);
        float away = Mathf.Clamp01(+local.z);

        return new LadderClimbInput
        {
            ClimbSigned = Mathf.Clamp(into - away, -1f, 1f),
            Lateral = Mathf.Clamp(local.x, -1f, 1f)
        };
    }
}