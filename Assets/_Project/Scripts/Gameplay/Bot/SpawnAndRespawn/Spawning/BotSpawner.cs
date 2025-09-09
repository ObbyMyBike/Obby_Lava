using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Zenject;

public class BotSpawner : IInitializable, ITickable, IDisposable
{
    private readonly DiContainer container;
    private readonly BotAgent botPrefab;
    private readonly Transform botParent;
    private readonly Transform startSpawnPoint;
    private readonly ActiveBots activeBots;
    private readonly BotRespawnDirectory respawnDirectory;
    private readonly IReadOnlyList<BotDataConfig> botConfigs;
    private readonly IReadOnlyList<WaypointPath> waypointPaths;
    private readonly float spawnIntervalSeconds;
    private readonly int initialBotsCount;

    private readonly BotPool pool;
    private readonly SpawnPointOccupancyGuard spawnGuard;
    
    private readonly StartSpawnPointCache startCache;
    private readonly InitialWavePlan initialWave;
    private readonly PooledBotCreator pooledCreator;
    private readonly StartSpawnAttempt startAttempt;

    private readonly DeathToRespawnQueue deathQueue;
    private readonly RespawnPointPicker respawnPointPicker;
    private readonly BotRespawnExecutor respawnExecutor;
    private readonly RespawnQueueProcessor respawnProcessor;
    private readonly BotDeathToRespawnLink _deathToRespawnLink;

    public BotSpawner(DiContainer container, BotAgent botPrefab, Transform botParent, Transform startSpawnPoint, int preload,
        int initialBotsCount, IReadOnlyList<BotDataConfig> botConfigs, IReadOnlyList<WaypointPath> waypointPaths,
        float spawnIntervalSeconds, ActiveBots agentsRegistry, BotRespawnDirectory respawnDirectory, [InjectOptional] SpawnedPlayerAccessor playerAccessor)
    {
        this.container = container;
        this.botPrefab = botPrefab;
        this.botParent = botParent;
        this.startSpawnPoint = startSpawnPoint;
        this.initialBotsCount = Mathf.Max(0, initialBotsCount);
        this.botConfigs = botConfigs ?? Array.Empty<BotDataConfig>();
        this.waypointPaths = waypointPaths ?? Array.Empty<WaypointPath>();
        this.spawnIntervalSeconds = Mathf.Max(BotSpawnerConstants.MIN_SPAWN_INTERVAL, spawnIntervalSeconds);
        activeBots = agentsRegistry;
        this.respawnDirectory = respawnDirectory;

        pool = new BotPool(this.botPrefab, this.botParent, preload);

        CharacterController prefabCharacterController = botPrefab != null ? botPrefab.GetComponent<CharacterController>() : null;
        spawnGuard = new SpawnPointOccupancyGuard(activeBots, playerAccessor, prefabCharacterController,
            BotSpawnerConstants.GUARD_EXTRA_RADIUS, BotSpawnerConstants.GUARD_EXTRA_HEIGHT, BotSpawnerConstants.RESERVATION_TTL_SECONDS);

        startCache = new StartSpawnPointCache(this.startSpawnPoint);
        initialWave = new InitialWavePlan(this.initialBotsCount, this.spawnIntervalSeconds);
        pooledCreator = new PooledBotCreator(this.container, pool, this.startSpawnPoint, this.botConfigs, this.waypointPaths);
        startAttempt = new StartSpawnAttempt(startCache, spawnGuard, pooledCreator);

        deathQueue = new DeathToRespawnQueue(activeBots, pooledCreator);
        respawnPointPicker = new RespawnPointPicker(startCache, this.respawnDirectory);
        _deathToRespawnLink = new BotDeathToRespawnLink(deathQueue); //activeBots, pooledCreator
        respawnExecutor = new BotRespawnExecutor(pooledCreator, this.respawnDirectory, activeBots, _deathToRespawnLink);
        respawnProcessor = new RespawnQueueProcessor(deathQueue, respawnPointPicker, spawnGuard, respawnExecutor);
    }

    void IInitializable.Initialize()
    {
        startCache.Initialize();
        initialWave.Initialize();
    }

    void ITickable.Tick()
    {
        spawnGuard.Update(Time.deltaTime);
        
        if (initialWave.ShouldTrySpawn(Time.deltaTime))
        {
            if (TrySpawnOneAtStart())
            {
                initialWave.MarkPlannedBySuccess();
            }
            else
            {
                EnqueueDelayedStart();
                initialWave.MarkPlannedByBlockedRetry();
            }
        }
        
        respawnProcessor.Tick(Time.deltaTime);
    }

    void IDisposable.Dispose()
    {
        foreach (var bot in new List<BotAgent>(activeBots.Active))
            DespawnNow(bot);
    }

    private bool TrySpawnOneAtStart()
    {
        if (!startCache.HasStart || botConfigs.Count == 0)
            return false;

        if (!startAttempt.TrySpawn(out BotAgent bot))
            return false;

        activeBots.Register(bot);
        _deathToRespawnLink.Attach(bot);
        
        return true;
    }

    private void EnqueueDelayedStart()
    {
        deathQueue.Storage.Add(new PendingRespawn
        {
            OriginalBot = null,
            RemainingSeconds = BotSpawnerConstants.BLOCKED_RETRY_SECONDS + Random.Range(0f, BotSpawnerConstants.BLOCKED_RETRY_JITTER_SECONDS),
            CountsTowardInitial = true
        });
    }

    private void DespawnNow(BotAgent bot)
    {
        if (bot == null)
            return;

        activeBots.Unregister(bot);
        bot.gameObject.SetActive(false);
        pooledCreator.Release(bot);
    }
}

// private const float MIN_SPAWN_INTERVAL = 0.05f;
    // private const float RESPAWN_DISABLE_SECONDS = 0.02f;
    // private const float RESPAWN_STAGGER_STEP_SECONDS = 0.12f;
    // private const float RESPAWN_JITTER_SECONDS = 0.25f;
    // private const float BLOCKED_RETRY_SECONDS = 0.08f;
    // private const float BLOCKED_RETRY_JITTER_SECONDS = 0.12f;
    // private const float GUARD_EXTRA_RADIUS = 0.05f;
    // private const float GUARD_EXTRA_HEIGHT = 0.05f;
    // private const float RESERVATION_TTL_SECONDS = 0.5f;
    // private const float START_SPAWN_JITTER_RADIUS = 0.20f;
    //
    // private readonly BotAgent botPrefab;
    // private readonly ActiveBots agentsRegistry;
    // private readonly BotRespawnDirectory respawn;
    // private readonly SpawnPointOccupancyGuard spawnGuard;
    //
    // private readonly DiContainer container; 
    // private readonly BotPool pool;
    // private readonly Transform botParent;
    // private readonly Transform startSpawnPoint;
    //
    // private readonly List<PendingRespawn> pendingRespawns = new List<PendingRespawn>(8);
    // private readonly List<BotAgent> activeBots = new List<BotAgent>();
    // private readonly IReadOnlyList<BotDataConfig> botConfigs;
    // private readonly IReadOnlyList<WaypointPath> paths;
    //
    // private readonly float spawnIntervalSeconds;
    // private readonly int initialBotsCount;
    //
    // private Vector3 _cachedStartPosition;
    // private float _spawnTimer;
    // private int _seedCounter = 1000;
    // private int _spawnedCount;
    // private int _lastEnqueueFrame;
    // private int _enqueueIndexThisFrame;
    // private bool _hasCachedStart;
    //
    // public BotSpawner(DiContainer container, BotAgent  botPrefab, Transform botParent, Transform startSpawnPoint, int preload, int initialBotsCount,
    //     IReadOnlyList<BotDataConfig> botConfigs, IReadOnlyList<WaypointPath> paths, float spawnIntervalSeconds, ActiveBots agentsRegistry,
    //     BotRespawnDirectory respawn, [InjectOptional] SpawnedPlayerAccessor playerAccessor)
    // {
    //     this.container = container;
    //     this.botPrefab = botPrefab;
    //     this.botParent = botParent;
    //     this.startSpawnPoint = startSpawnPoint;
    //     this.initialBotsCount = Mathf.Max(0, initialBotsCount);
    //     this.botConfigs = botConfigs ?? Array.Empty<BotDataConfig>();
    //     this.paths = paths ?? Array.Empty<WaypointPath>();
    //     this.spawnIntervalSeconds = Mathf.Max(MIN_SPAWN_INTERVAL, spawnIntervalSeconds);
    //     this.agentsRegistry = agentsRegistry;
    //     this.respawn = respawn;
    //     
    //     pool = new BotPool(this.botPrefab, this.botParent, preload);
    //     
    //     CharacterController prefabCharacterController = botPrefab != null ? botPrefab.GetComponent<CharacterController>() : null;
    //     
    //     spawnGuard = new SpawnPointOccupancyGuard(this.agentsRegistry, playerAccessor, prefabCharacterController, GUARD_EXTRA_RADIUS,
    //         GUARD_EXTRA_HEIGHT, RESERVATION_TTL_SECONDS);
    // }
    //
    // void IInitializable.Initialize()
    // {
    //     _spawnedCount = 0;
    //     _spawnTimer = spawnIntervalSeconds;
    //     
    //     if (startSpawnPoint != null)
    //     {
    //         _cachedStartPosition = startSpawnPoint.position;
    //         _hasCachedStart = true;
    //     }
    //     else
    //     {
    //         _cachedStartPosition = Vector3.zero;
    //         _hasCachedStart = false;
    //     }
    // }
    //
    // void ITickable.Tick()
    // {
    //     spawnGuard.Update(Time.deltaTime);
    //     
    //     if (_spawnedCount < initialBotsCount)
    //     {
    //         _spawnTimer -= Time.deltaTime;
    //
    //         if (_spawnTimer <= 0f)
    //         {
    //             TrySpawnNextBot();
    //             
    //             _spawnTimer = spawnIntervalSeconds;
    //         }
    //     }
    //     
    //     if (pendingRespawns.Count > 0)
    //     {
    //         for (int i = pendingRespawns.Count - 1; i >= 0; i--)
    //         {
    //             PendingRespawn item = pendingRespawns[i];
    //             item.RemainingSeconds -= Time.deltaTime;
    //
    //             if (item.RemainingSeconds > 0f)
    //             {
    //                 pendingRespawns[i] = item;
    //                 
    //                 continue;
    //             }
    //
    //             if (!TryComputeRespawnPosition(item.OriginalBot, out Vector3 pos))
    //             {
    //                 pendingRespawns.RemoveAt(i);
    //                 
    //                 continue;
    //             }
    //
    //             if (spawnGuard.IsAreaFree(pos))
    //             {
    //                 spawnGuard.Reserve(pos);
    //                 PerformRespawnAtPosition(item.OriginalBot, pos);
    //                 pendingRespawns.RemoveAt(i);
    //             }
    //             else
    //             {
    //                 item.RemainingSeconds = BLOCKED_RETRY_SECONDS + Random.Range(0f, BLOCKED_RETRY_JITTER_SECONDS);
    //                 pendingRespawns[i] = item;
    //             }
    //         }
    //     }
    // }
    //
    // void IDisposable.Dispose()
    // {
    //     for (int i = activeBots.Count - 1; i >= 0; i--)
    //         DespawnBot(activeBots[i]);
    // }
    //
    // private void TrySpawnNextBot()
    // {
    //     if (botConfigs.Count == 0)
    //         return;
    //
    //     BotDataConfig config = botConfigs[_spawnedCount % botConfigs.Count];
    //     WaypointPath path = paths.Count > 0 ? paths[_spawnedCount % paths.Count] : null;
    //
    //     if (!SpawnBot(config, path))
    //         return;
    //
    //     _spawnedCount++;
    // }
    //
    // private bool SpawnBot(BotDataConfig config, WaypointPath path)
    // {
    //     GameObject instance = pool.Get();
    //     Vector3 spawnPosition = _cachedStartPosition;
    //     
    //     if (startSpawnPoint != null)
    //     {
    //         if (!_hasCachedStart || startSpawnPoint.position != _cachedStartPosition)
    //         {
    //             _cachedStartPosition = startSpawnPoint.position;
    //             _hasCachedStart = true;
    //         }
    //         
    //         spawnPosition = _cachedStartPosition;
    //     }
    //
    //     if (START_SPAWN_JITTER_RADIUS > 0f)
    //     {
    //         Vector2 jitter = Random.insideUnitCircle * START_SPAWN_JITTER_RADIUS;
    //         spawnPosition.x += jitter.x;
    //         spawnPosition.z += jitter.y;
    //     }
    //
    //     if (!spawnGuard.IsAreaFree(spawnPosition))
    //     {
    //         pool.Release(instance);
    //         EnqueueRetryStartSpawn();
    //         
    //         return false;
    //     }
    //     
    //     spawnGuard.Reserve(spawnPosition);
    //
    //     instance.transform.position = spawnPosition;
    //
    //     if (!instance.TryGetComponent(out BotAgent bot))
    //     {
    //         pool.Release(instance);
    //         
    //         return false;
    //     }
    //
    //     container.Inject(bot, new object[] { _seedCounter++, startSpawnPoint, config, path });
    //
    //     bot.gameObject.SetActive(true);
    //     activeBots.Add(bot);
    //     agentsRegistry.Register(bot);
    //
    //     SubscribeBotDeath(bot);
    //
    //     return true;
    // }
    //
    // private void EnqueueRetryStartSpawn()
    // {
    //     float delay = BLOCKED_RETRY_SECONDS + Random.Range(0f, BLOCKED_RETRY_JITTER_SECONDS);
    //     
    //     _spawnedCount++;
    //
    //     pendingRespawns.Add(new PendingRespawn
    //     {
    //         OriginalBot = null,
    //         RemainingSeconds = delay,
    //         CountsTowardInitial = true
    //     });
    // }
    //
    // private void SubscribeBotDeath(BotAgent bot)
    // {
    //     if (bot == null)
    //         return;
    //
    //     BotAI aiField = GetPrivateAI(bot);
    //     
    //     if (aiField != null)
    //         aiField.OnBotDied += _ => OnBotDiedInternal(bot);
    // }
    //
    // private BotAI GetPrivateAI(BotAgent bot)
    // {
    //     var field = typeof(BotAgent).GetField("_ai", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    //     
    //     return field?.GetValue(bot) as BotAI;
    // }
    //
    // private void OnBotDiedInternal(BotAgent bot)
    // {
    //     if (bot == null)
    //         return;
    //
    //     if (Time.frameCount == _lastEnqueueFrame)
    //     {
    //         _enqueueIndexThisFrame++;
    //     }
    //     else
    //     {
    //         _lastEnqueueFrame = Time.frameCount;
    //         _enqueueIndexThisFrame = 0;
    //     }
    //
    //     float delay = RESPAWN_DISABLE_SECONDS + (_enqueueIndexThisFrame * RESPAWN_STAGGER_STEP_SECONDS) + Random.Range(0f, RESPAWN_JITTER_SECONDS);
    //
    //     agentsRegistry.Unregister(bot);
    //     activeBots.Remove(bot);
    //     bot.gameObject.SetActive(false);
    //     pool.Release(bot.gameObject);
    //     
    //     pendingRespawns.Add(new PendingRespawn
    //     {
    //         OriginalBot = bot,
    //         RemainingSeconds = delay
    //     });
    // }
    //
    // private bool TryComputeRespawnPosition(BotAgent originalBot, out Vector3 position)
    // {
    //     position = _hasCachedStart ? _cachedStartPosition : Vector3.zero;
    //     
    //     if (originalBot != null && respawn.TryGet(originalBot, out BotRespawn oldRespawn))
    //     {
    //         Transform spawnPoint = oldRespawn.LastCheckpointTransform;
    //         
    //         if (spawnPoint != null)
    //         {
    //             position = spawnPoint.position;
    //             
    //             return true;
    //         }
    //     }
    //     
    //     return _hasCachedStart;
    // }
    //
    // private void PerformRespawnAtPosition(BotAgent originalBot, Vector3 position)
    // {
    //     Transform snapLastCP = null;
    //     float snapLastY = 0f;
    //     
    //     if (originalBot != null && respawn.TryGet(originalBot, out BotRespawn oldRespawn))
    //     {
    //         snapLastCP = oldRespawn.LastCheckpointTransform;
    //         snapLastY = oldRespawn.LastCheckpointProgress;
    //     }
    //
    //     GameObject instance = pool.GetInactive();
    //     
    //     if (!instance.TryGetComponent(out BotAgent newBot))
    //     {
    //         pool.Release(instance);
    //         
    //         return;
    //     }
    //     
    //     instance.transform.position = position;
    //     
    //     BotDataConfig config = botConfigs.Count > 0 ? botConfigs[_spawnedCount % botConfigs.Count] : null;
    //     WaypointPath  path   = paths.Count > 0 ? paths[_spawnedCount % paths.Count] : null;
    //     container.Inject(newBot, new object[] { _seedCounter++, startSpawnPoint, config, path });
    //     
    //     if (snapLastCP != null && respawn.TryGet(newBot, out BotRespawn newRespawn))
    //         newRespawn.TrySetCheckpoint(snapLastCP, snapLastY);
    //     
    //     if (originalBot != null)
    //         respawn.Unregister(originalBot);
    //     
    //     newBot.RestoreFullHealth();
    //     newBot.gameObject.SetActive(true);
    //
    //     activeBots.Add(newBot);
    //     agentsRegistry.Register(newBot);
    //     SubscribeBotDeath(newBot);
    // }
    //
    // private void DespawnBot(BotAgent bot)
    // {
    //     if (bot == null)
    //         return;
    //
    //     agentsRegistry.Unregister(bot);
    //     bot.gameObject.SetActive(false);
    //     activeBots.Remove(bot);
    //     pool.Release(bot.gameObject);
    // }