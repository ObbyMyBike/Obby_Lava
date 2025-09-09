using System.Collections.Generic;
using UnityEngine;

public class DeathToRespawnQueue
{
    private readonly ActiveBots activeBots;
    private readonly PooledBotCreator creator;

    private readonly List<PendingRespawn> queue = new List<PendingRespawn>(8);

    private int _lastEnqueueFrame;
    private int _enqueueIndexThisFrame;

    public DeathToRespawnQueue(ActiveBots activeBots, PooledBotCreator creator)
    {
        this.activeBots = activeBots;
        this.creator = creator;
        _lastEnqueueFrame = -1;
        _enqueueIndexThisFrame = 0;
    }

    public void OnBotDied(BotAgent bot)
    {
        if (bot == null)
            return;

        if (Time.frameCount == _lastEnqueueFrame)
        {
            _enqueueIndexThisFrame++;
        }
        else
        {
            _lastEnqueueFrame = Time.frameCount;
            _enqueueIndexThisFrame = 0;
        }

        float delay = BotSpawnerConstants.RESPAWN_DISABLE_SECONDS + _enqueueIndexThisFrame * BotSpawnerConstants.RESPAWN_STAGGER_STEP_SECONDS
                                                                  + Random.Range(0f, BotSpawnerConstants.RESPAWN_JITTER_SECONDS);

        activeBots.Unregister(bot);
        bot.gameObject.SetActive(false);
        creator.Release(bot);

        queue.Add(new PendingRespawn
        {
            OriginalBot = bot,
            RemainingSeconds = delay,
            CountsTowardInitial = false
        });
    }

    public List<PendingRespawn> Storage => queue;
}