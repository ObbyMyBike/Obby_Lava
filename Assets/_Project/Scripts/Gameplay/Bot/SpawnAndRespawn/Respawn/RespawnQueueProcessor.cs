using UnityEngine;

public class RespawnQueueProcessor
{
    private readonly DeathToRespawnQueue deathQueue;
    private readonly RespawnPointPicker pointPicker;
    private readonly SpawnPointOccupancyGuard spawnGuard;
    private readonly BotRespawnExecutor respawnExecutor;

    public RespawnQueueProcessor(DeathToRespawnQueue deathQueue, RespawnPointPicker pointPicker, SpawnPointOccupancyGuard spawnGuard,
        BotRespawnExecutor respawnExecutor)
    {
        this.deathQueue = deathQueue;
        this.pointPicker = pointPicker;
        this.spawnGuard = spawnGuard;
        this.respawnExecutor = respawnExecutor;
    }

    public void Tick(float deltaTime)
    {
        var list = deathQueue.Storage;
        
        if (list.Count == 0)
            return;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            PendingRespawn item = list[i];
            item.RemainingSeconds -= deltaTime;

            if (item.RemainingSeconds > 0f)
            {
                list[i] = item;
                
                continue;
            }

            if (!pointPicker.TryGetPosition(item.OriginalBot, out Vector3 position))
            {
                list.RemoveAt(i);
                
                continue;
            }

            if (spawnGuard.IsAreaFree(position))
            {
                spawnGuard.Reserve(position);
                respawnExecutor.TryRespawnReplacing(item.OriginalBot, position, out _);
                list.RemoveAt(i);
            }
            else
            {
                item.RemainingSeconds = BotSpawnerConstants.BLOCKED_RETRY_SECONDS + Random.Range(0f, BotSpawnerConstants.BLOCKED_RETRY_JITTER_SECONDS);
                list[i] = item;
            }
        }
    }
}