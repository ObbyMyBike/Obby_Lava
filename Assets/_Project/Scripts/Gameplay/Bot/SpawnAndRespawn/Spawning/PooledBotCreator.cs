using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PooledBotCreator
{
    private readonly DiContainer container;
    private readonly BotPool pool;
    private readonly Transform startSpawnPoint;
    private readonly IReadOnlyList<BotDataConfig> botConfigs;
    private readonly IReadOnlyList<WaypointPath> waypointPaths;

    private int _uniqueSeedCounter = 1000;
    private int _createdCount;

    public PooledBotCreator(DiContainer container, BotPool pool, Transform startSpawnPoint, IReadOnlyList<BotDataConfig> botConfigs, IReadOnlyList<WaypointPath> waypointPaths)
    {
        this.container = container;
        this.pool = pool;
        this.startSpawnPoint = startSpawnPoint;
        this.botConfigs = botConfigs ?? Array.Empty<BotDataConfig>();
        this.waypointPaths = waypointPaths ?? Array.Empty<WaypointPath>();
        _createdCount = 0;
    }

    public bool TryCreate(Vector3 position, out BotAgent bot)
    {
        bot = null;

        GameObject instance = pool.Get();
        instance.transform.position = position;

        if (!instance.TryGetComponent(out BotAgent found))
        {
            pool.Release(instance);
            
            return false;
        }
        
        BotDataConfig config = botConfigs.Count > 0 ? botConfigs[_createdCount % botConfigs.Count] : null;
        WaypointPath path = waypointPaths.Count > 0 ? waypointPaths[_createdCount % waypointPaths.Count] : null;

        container.Inject(found, new object[] { _uniqueSeedCounter++, startSpawnPoint, config, path });

        found.gameObject.SetActive(true);
        bot = found;

        _createdCount++;
        
        return true;
    }

    public bool TryCreateInactiveAt(Vector3 position, out BotAgent bot, out GameObject instance)
    {
        bot = null;
        instance = pool.GetInactive();
        instance.transform.position = position;

        if (!instance.TryGetComponent(out BotAgent found))
        {
            pool.Release(instance);
            
            return false;
        }
        
        BotDataConfig config = botConfigs.Count > 0 ? botConfigs[_createdCount % botConfigs.Count] : null;
        WaypointPath path  = waypointPaths.Count > 0 ? waypointPaths[_createdCount % waypointPaths.Count] : null;

        container.Inject(found, new object[] { _uniqueSeedCounter++, startSpawnPoint, config, path });
        bot = found;
        
        _createdCount++;
        
        return true;
    }

    public void Release(BotAgent bot) => pool.Release(bot.gameObject);
}