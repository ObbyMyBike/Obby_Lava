using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BotInstaller : MonoInstaller
{
    private const int MIN_PRELOAD = 0;
    private const int MIN_INITIAL = 0;
    
    [Header("Bots")]
    [SerializeField] private BotAgent _botPrefab;
    [SerializeField] private Transform _botParent;
    [SerializeField] private Transform _startSpawnPoint;

    [Header("Per-Bot Settings")]
    [SerializeField] private List<BotDataConfig> _botConfigs = new List<BotDataConfig>();
    [SerializeField] private List<WaypointPath> _waypointPaths = new List<WaypointPath>();
    [SerializeField] private float _spawnIntervalSeconds = 1.5f;
    
    [Header("Pool And Spawn")]
    [SerializeField] private int _preloadCount = 8;
    [SerializeField] private int _initialBotsCount = 5;

    public override void InstallBindings()
    {
        Container.Bind<BotRespawnDirectory>().AsSingle();
        Container.Bind<ActiveBots>().AsSingle();

        int preload = Mathf.Max(MIN_PRELOAD, _preloadCount);
        int initial = Mathf.Max(MIN_INITIAL, _initialBotsCount);

        Container.BindInterfacesAndSelfTo<BotSpawner>().AsSingle().WithArguments(new object[]
        {
            _botPrefab, _botParent, _startSpawnPoint, preload, initial, _botConfigs, _waypointPaths, _spawnIntervalSeconds
                
        }).NonLazy();
    }
}