using UnityEngine;

public class PlayerRespawn
{
    private const float PROGRESS_EPS = 0.01f;
    
    public event OnRespawnRequest OnRespawnRequested;
    public event OnCheckpointActivate OnCheckpointActivated;
    
    private Transform _currentSpawnPoint;
    private Transform _startSpawnPoint;
    private float _currentProgress = float.NegativeInfinity;
    
    public Transform GetCurrentSpawnPoint() => _currentSpawnPoint != null ? _currentSpawnPoint : _startSpawnPoint;
    
    public bool TrySetCheckpoint(Transform checkPoint, float progress, bool force = false)
    {
        if (!force && progress <= _currentProgress + PROGRESS_EPS)
            return false;

        _currentSpawnPoint = checkPoint;
        _currentProgress = progress;
        
        OnCheckpointActivated?.Invoke(checkPoint);
        
        return true;
    }
    
    public void SetStartPoint(Transform startPoint)
    {
        _startSpawnPoint = startPoint;
        
        if (_currentSpawnPoint == null)
        {
            _currentSpawnPoint = _startSpawnPoint;
            _currentProgress = ProgressHeight(startPoint); 
        }
    }
    
    public void RequestRespawn() => OnRespawnRequested?.Invoke();
    
    private float ProgressHeight(Transform pointTransform) => pointTransform != null ? pointTransform.position.y : float.NegativeInfinity;
}