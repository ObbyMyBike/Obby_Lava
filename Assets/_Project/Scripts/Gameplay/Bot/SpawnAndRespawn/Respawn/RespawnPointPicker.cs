using UnityEngine;

public class RespawnPointPicker
{
    private readonly StartSpawnPointCache startCache;
    private readonly BotRespawnDirectory respawnDirectory;

    public RespawnPointPicker(StartSpawnPointCache startCache, BotRespawnDirectory respawnDirectory)
    {
        this.startCache = startCache;
        this.respawnDirectory = respawnDirectory;
    }

    public bool TryGetPosition(BotAgent originalBot, out Vector3 position)
    {
        position = startCache.HasStart ? startCache.Position : Vector3.zero;

        if (originalBot != null && respawnDirectory.TryGet(originalBot, out BotRespawn oldRespawn))
        {
            Transform checkpoint = oldRespawn.LastCheckpointTransform;
            
            if (checkpoint != null)
            {
                position = checkpoint.position;
                
                return true;
            }
        }

        return startCache.HasStart;
    }
}