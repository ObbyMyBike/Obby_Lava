using UnityEngine;

public class BotRespawn
{
    public event OnBotRespawn OnRespawned;
    public event OnBotCheckpointCollect OnCheckpointCollected;
    
    private readonly Transform startSpawnPoint;
    private readonly float minProgressDeltaToAccept;
    
    private Transform _lastCheckpointTransform;
    private float _lastCheckpointProgress;

    public BotRespawn(Transform startSpawnPoint, float minProgressDeltaToAccept)
    {
        this.startSpawnPoint = startSpawnPoint;
        this.minProgressDeltaToAccept = Mathf.Max(0f, minProgressDeltaToAccept);
        
        _lastCheckpointTransform = this.startSpawnPoint;
        _lastCheckpointProgress = this.startSpawnPoint != null ? this.startSpawnPoint.position.y : 0f;

        LastCheckpointTransform = _lastCheckpointTransform;
        LastCheckpointProgress = _lastCheckpointProgress;
    }
    
    public Transform LastCheckpointTransform { get; private set; }
    public float LastCheckpointProgress { get; private set; }

    public bool TrySetCheckpoint(Transform checkpoint, float progress)
    {
        if (checkpoint == null)
            return false;

        if (progress + minProgressDeltaToAccept <= _lastCheckpointProgress)
            return false;

        _lastCheckpointTransform = checkpoint;
        _lastCheckpointProgress = progress;

        LastCheckpointTransform = _lastCheckpointTransform;
        LastCheckpointProgress = _lastCheckpointProgress;
        
        OnCheckpointCollected?.Invoke(null, checkpoint);
        
        return true;
    }

    public void Respawn(BotAgent bot)
    {
        Transform target = _lastCheckpointTransform != null ? _lastCheckpointTransform : startSpawnPoint;
        
        if (bot != null && target != null)
        {
            bot.TryTeleport(target.position, true);
            
            OnRespawned?.Invoke(bot, target);
        }
    }
}