using UnityEngine;

public class StartSpawnPointCache
{
    private readonly Transform startSpawnPoint;
    
    private Vector3 _cachedStartPosition;
    private bool _hasCached;

    public StartSpawnPointCache(Transform startSpawnPoint)
    {
        this.startSpawnPoint = startSpawnPoint;
        _cachedStartPosition = Vector3.zero;
        _hasCached = false;
    }

    public bool HasStart => _hasCached;
    
    public Vector3 Position => _cachedStartPosition;
    
    public void Initialize()
    {
        if (startSpawnPoint != null)
        {
            _cachedStartPosition = startSpawnPoint.position;
            _hasCached = true;
        }
        else
        {
            _cachedStartPosition = Vector3.zero;
            _hasCached = false;
        }
    }

    public Vector3 GetFreshPosition()
    {
        if (startSpawnPoint == null)
            return _cachedStartPosition;

        if (!_hasCached || startSpawnPoint.position != _cachedStartPosition)
        {
            _cachedStartPosition = startSpawnPoint.position;
            _hasCached = true;
        }
        
        return _cachedStartPosition;
    }
}