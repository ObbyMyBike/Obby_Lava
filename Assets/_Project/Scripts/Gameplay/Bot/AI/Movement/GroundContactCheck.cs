using UnityEngine;

public class GroundContactCheck
{
    private readonly CharacterController controller;
    private readonly BotDataConfig config;

    public GroundContactCheck(CharacterController controller, BotDataConfig config)
    {
        this.controller = controller;
        this.config = config;
    }

    public bool Probe(out RaycastHit hit)
    {
        Bounds bounds = controller.bounds;
        Vector3 center = bounds.center;
        float radius = Mathf.Max(config.GroundProbeRadiusMin, Mathf.Min(config.GroundProbeRadius, controller.radius * 0.95f));
        float distance = bounds.extents.y + Mathf.Max(0.01f, config.GroundProbeDistance);
        
        return Physics.SphereCast(center, radius, Vector3.down, out hit, distance, config.GroundMask, QueryTriggerInteraction.Ignore);
    }
}