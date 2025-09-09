using System;
using UnityEngine;

public class CheckpointProgressSync : IDisposable
{
    private readonly BotAiContext context;
    
    private bool _synced;
    private float _resyncCooldown;

    public CheckpointProgressSync(BotAiContext context)
    {
        this.context = context;
        
        if (this.context.Respawn != null)
            this.context.Respawn.OnRespawned += OnRespawned;
    }

    void IDisposable.Dispose()
    {
        if (context.Respawn != null)
            context.Respawn.OnRespawned -= OnRespawned;
    }

    public void TickCooldown(float dt)
    {
        if (_resyncCooldown > 0f)
            _resyncCooldown -= dt;
    }

    public void EnsureStartSync()
    {
        if (_synced)
            return;

        context.Track.SyncToProgress(context.Respawn.LastCheckpointProgress);

        Vector3 position = context.Agent.Controller.transform.position;
        context.Track.SyncToNearestInYBand(position, BotAiConstants.START_MAX_ABOVE, BotAiConstants.START_MAX_BELOW);

        _synced = true;
    }

    public bool TryResyncIfNeeded(float deltaTime, out Transform newWaypoint)
    {
        newWaypoint = context.Track.Current;

        if (_resyncCooldown > 0f || !context.Motion.IsGrounded || newWaypoint == null)
            return false;

        Transform self = context.Agent.Controller.transform;
        Vector3 toTarget = newWaypoint.position - self.position;
        Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z);

        bool belowCurrent = (self.position.y + BotAiConstants.RESYNC_Y_BELOW) < newWaypoint.position.y;
        bool farInXZ = (flat.sqrMagnitude > BotAiConstants.RESYNC_FAR_XZ_SQR);
        
        if (!belowCurrent && !farInXZ)
            return false;

        Vector3 position = self.position;
        bool snapped = context.Track.SyncToNearestInYBand(position, BotAiConstants.RESYNC_MAX_ABOVE, BotAiConstants.RESYNC_MAX_BELOW);
        
        if (!snapped)
            context.Track.SyncToNearest(position);

        context.Failure.ResetOnAdvance();
        _resyncCooldown = BotAiConstants.RESYNC_COOLDOWN;

        newWaypoint = context.Track.Current;
        
        return true;
    }

    private void OnRespawned(BotAgent bot, Transform target)
    {
        context.Track.SyncToProgress(context.Respawn.LastCheckpointProgress);
        context.Failure.ResetOnAdvance();
        
        _synced = true;
        _resyncCooldown = 0f;
    }
}