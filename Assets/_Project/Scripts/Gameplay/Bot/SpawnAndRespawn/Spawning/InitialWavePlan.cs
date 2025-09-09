using UnityEngine;

public class InitialWavePlan
{
    private readonly int initialBotsLimit;
    private readonly float spawnIntervalSeconds;

    private float _spawnTimer;
    private int _plannedCount;

    public InitialWavePlan(int initialBotsLimit, float spawnIntervalSeconds)
    {
        this.initialBotsLimit = Mathf.Max(0, initialBotsLimit);
        this.spawnIntervalSeconds = Mathf.Max(BotSpawnerConstants.MIN_SPAWN_INTERVAL, spawnIntervalSeconds);
        _spawnTimer = this.spawnIntervalSeconds;
        _plannedCount = 0;
    }

    public void Initialize()
    {
        _plannedCount = 0;
        _spawnTimer = spawnIntervalSeconds;
    }
    
    public void MarkPlannedBySuccess() => _plannedCount++;
    
    public void MarkPlannedByBlockedRetry() => _plannedCount++;

    public bool ShouldTrySpawn(float deltaTime)
    {
        if (_plannedCount >= initialBotsLimit)
            return false;

        _spawnTimer -= deltaTime;
        
        if (_spawnTimer <= 0f)
        {
            _spawnTimer = spawnIntervalSeconds;
            
            return true;
        }
        
        return false;
    }
}