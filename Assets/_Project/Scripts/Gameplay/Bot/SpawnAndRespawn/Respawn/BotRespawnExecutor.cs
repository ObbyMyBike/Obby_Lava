using UnityEngine;

public class BotRespawnExecutor
{
    private readonly PooledBotCreator creator;
    private readonly BotRespawnDirectory respawnDirectory;
    private readonly ActiveBots activeBots;
    private readonly BotDeathToRespawnLink deathLink;

    public BotRespawnExecutor(PooledBotCreator creator, BotRespawnDirectory respawnDirectory, ActiveBots activeBots,
        BotDeathToRespawnLink deathLink)
    {
        this.creator = creator;
        this.respawnDirectory = respawnDirectory;
        this.activeBots = activeBots;
        this.deathLink = deathLink;
    }

    public bool TryRespawnReplacing(BotAgent originalBot, Vector3 position, out BotAgent newBot)
    {
        newBot = null;

        if (!creator.TryCreateInactiveAt(position, out BotAgent created, out GameObject instance))
            return false;
        
        if (originalBot != null && respawnDirectory.TryGet(originalBot, out BotRespawn oldRespawn))
        {
            if (respawnDirectory.TryGet(created, out BotRespawn newRespawn))
            {
                newRespawn.TrySetCheckpoint(oldRespawn.LastCheckpointTransform, oldRespawn.LastCheckpointProgress);
            }
            
            respawnDirectory.Unregister(originalBot);
        }

        created.RestoreFullHealth();
        created.gameObject.SetActive(true);

        activeBots.Register(created);
        
        if (deathLink != null)
            deathLink.Attach(created);
        
        newBot = created;
        
        return true;
    }
}