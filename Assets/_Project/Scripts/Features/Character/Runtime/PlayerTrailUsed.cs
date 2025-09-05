using UnityEngine;

public class PlayerTrailUsed
{
    private const float DEFAULT_TRAIL_TIME_SECONDS = 0.5f;
    
    private readonly TrailRenderer trailRenderer;
    
    public PlayerTrailUsed(TrailRenderer trailRenderer)
    {
        this.trailRenderer = trailRenderer;

        if (this.trailRenderer != null)
        {
            if (float.IsInfinity(this.trailRenderer.time) || this.trailRenderer.time <= 0f)
                this.trailRenderer.time = DEFAULT_TRAIL_TIME_SECONDS;

            this.trailRenderer.emitting = false;
            this.trailRenderer.Clear();
        }
    }

    public void Enable()
    {
        if (trailRenderer != null)
            trailRenderer.emitting = true;
    }

    public void Disable()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
        }
    }

    public void TryApplyLifetimeOverride(float lifetimeSeconds)
    {
        if (trailRenderer == null)
            return;

        if (lifetimeSeconds > 0f && !float.IsInfinity(lifetimeSeconds))
            trailRenderer.time = lifetimeSeconds;
    }
}