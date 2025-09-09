using UnityEngine;

public class StartSpawnAttempt
{
    private readonly StartSpawnPointCache startCache;
    private readonly SpawnPointOccupancyGuard spawnGuard;
    private readonly PooledBotCreator creator;

    public StartSpawnAttempt(StartSpawnPointCache startCache, SpawnPointOccupancyGuard spawnGuard, PooledBotCreator creator)
    {
        this.startCache = startCache;
        this.spawnGuard = spawnGuard;
        this.creator = creator;
    }

    private static Vector3 AddStartJitter(Vector3 basePosition)
    {
        if (BotSpawnerConstants.START_SPAWN_JITTER_RADIUS <= 0f)
            return basePosition;

        Vector2 jitter = Random.insideUnitCircle * BotSpawnerConstants.START_SPAWN_JITTER_RADIUS;
        basePosition.x += jitter.x;
        basePosition.z += jitter.y;
        
        return basePosition;
    }

    public bool TrySpawn(out BotAgent bot)
    {
        bot = null;
        Vector3 desiredPosition = startCache.GetFreshPosition();
        desiredPosition = AddStartJitter(desiredPosition);

        if (!spawnGuard.IsAreaFree(desiredPosition))
            return false;

        spawnGuard.Reserve(desiredPosition);
        
        return creator.TryCreate(desiredPosition, out bot);
    }
}