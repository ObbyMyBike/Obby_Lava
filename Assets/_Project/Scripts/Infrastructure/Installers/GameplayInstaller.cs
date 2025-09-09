using UnityEngine;
using Cinemachine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    private const string ID_THREE_CHECKPOINT_PREFAB = "ThreeCheckpointPrefab";
    private const string ID_THREE_CHECKPOINT_PREVIEW_PREFAB = "ThreeCheckpointPreviewPrefab";
    private const string ID_THREE_CHECKPOINT_PARENT = "ThreeCheckpointParent";
    
    [SerializeField] private GlobalCoroutineRunner _globalCoroutineRunner;
    [SerializeField] private MobileInput _mobileInput;
    [SerializeField] private Camera _mainCamera;
    
    [Header("Player Prefab Spawn")]
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform _playerParent; 
    
    [Header("Game Configs")]
    [SerializeField] private InputConfig _inputConfig;
    [SerializeField] private PlayerDataConfig  _playerDataConfig;
    [SerializeField] private ShopItemConfig _shopItemsConfig;
    [SerializeField] private SkinsCatalogConfig _skinsCatalog;
    [SerializeField] private LadderSettingsConfig _ladderSettings;
    
    [Header("Ladder")]
    [SerializeField] private LayerMask _ladderMask;
    [SerializeField] private LayerMask _groundMask;
    
    [Header("Other Settings")]
    [SerializeField] private LayerMask _pushableMask;
    [SerializeField] private bool _enableDevHotkeys = false;
    [SerializeField] private bool _isMobile = false;
    
    [Header("Respawn / Checkpoints")]
    [SerializeField] private Transform _startSpawnPoint;
    
    [Header("Three Checkpoints")]
    [SerializeField] private GameObject _threeCheckpointPrefab;
    [SerializeField] private GameObject _threeCheckpointPreviewPrefab;
    [SerializeField] private Transform _threeCheckpointParent;  
    
    public override void InstallBindings()
    {
        bool isMobile = Application.isMobilePlatform || _isMobile;
        
        Container.Bind<PlatformDefinition>().AsSingle().WithArguments(isMobile);
        Container.Bind<CursorVisibility>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FollowCameraCinemachineBinder>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        if (isMobile)
        {
            _mobileInput.gameObject.SetActive(true);
            Container.BindInterfacesAndSelfTo<MobileInput>().FromInstance(_mobileInput).AsSingle();
        }
        else
        {
            _mobileInput.gameObject.SetActive(false);
            Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle().WithArguments(_inputConfig).NonLazy();
        }
        
        // Камера / billboard / virtual camera
        Transform cameraTransform = _mainCamera != null ? _mainCamera.transform : (Camera.main != null ? Camera.main.transform : null);
        Container.Bind<MainCameraProvider>().AsSingle().WithArguments(cameraTransform).NonLazy();
        Container.BindInterfacesAndSelfTo<ShopBillboardToCamera>().FromComponentsInHierarchy().AsTransient().NonLazy();
        Container.BindInterfacesAndSelfTo<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        // --- Runtime-спавн игрока ---
        Container.Bind<SpawnedPlayerAccessor>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerPrefabSpawn>().AsSingle().WithArguments(_playerPrefab, _startSpawnPoint, _playerParent).NonLazy();
        
        // --- Экономика/интерфейс/эффекты ---
        Container.Bind<ShopItemConfig>().FromInstance(_shopItemsConfig).AsSingle();
        Container.Bind<Wallet>().AsSingle().WithArguments(_playerDataConfig.StartingGold).NonLazy();
        Container.Bind<ShopPanelView>().FromComponentInHierarchy().AsSingle().NonLazy(); // принудительно инжектим
        Container.Bind<GlobalCoroutineRunner>().FromInstance(_globalCoroutineRunner).AsSingle();
        Container.Bind<TimedEffectsRunner>().AsSingle().NonLazy();
        
        // --- Здоровье/окружение ---
        Container.Bind<PlayerHealth>().AsSingle().WithArguments(_playerDataConfig.MaxHealth, _playerDataConfig.MaxHealth);
        Container.Bind<PlayerHealthBarPanel>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LavaContactHealthDrainSystem>().AsSingle().WithArguments(_playerDataConfig.SecondsToDieInLava, _playerDataConfig.RegenPerSecondOutsideLava).NonLazy();
        Container.BindInterfacesAndSelfTo<NonPenetrationSystem>().AsSingle().NonLazy();
        
        // --- Лестницы/прыжки/толчки ---
        Container.Bind<LadderSettingsConfig>().FromInstance(_ladderSettings).AsSingle();
        Container.BindInterfacesAndSelfTo<LadderTriggerVolume>().FromComponentsInHierarchy().AsTransient().NonLazy();

        // Push / AutoPush без прямого Transform — они читают его из SpawnedPlayerAccessor
        Container.Bind<LadderClimb>().AsSingle().WithArguments(_ladderMask, _groundMask, _ladderSettings);
        Container.Bind<PushInteraction>().AsSingle().WithArguments(_pushableMask, _playerDataConfig.PushForce);
        Container.BindInterfacesAndSelfTo<AutoPush>().AsSingle().WithArguments(_playerDataConfig.PushRange).NonLazy();

        // --- Скины/реклама/сейвы ---
        Container.BindInterfacesAndSelfTo<SkinPickupTrigger>().FromComponentsInHierarchy().AsTransient().NonLazy();
        Container.Bind<SkinsCatalogConfig>().FromInstance(_skinsCatalog).AsSingle();
        Container.Bind<YandexGamesRewardedAd>().AsSingle();
        Container.Bind<YGSkinsSaveRepository>().AsSingle();
        Container.Bind<PlayerSkinApplier>().AsSingle();
        Container.Bind<ApplySavedSkinOnStart>().AsSingle().NonLazy();
        Container.Bind<SkinUnlockAndApply>().AsSingle();

        // --- Dev-хоткеи (опционально) ---
        Container.Bind<YGGameSaveResetter>().AsSingle().NonLazy();
        
        if (_enableDevHotkeys)
        {
            Container.BindInterfacesTo<DevResetProgressOnKeyPress>().AsSingle().NonLazy();
            Container.BindInterfacesTo<DevReloadSceneOnKeyPress>().AsSingle().NonLazy();
        }

        // --- Респавн/чекпоинты ---
        Container.Bind<PlayerRespawn>().AsSingle().NonLazy();
        Container.BindInterfacesTo<PlayerDeathSubscription>().AsSingle().NonLazy();
        Container.BindInterfacesTo<RespawnStartBinder>().AsSingle().WithArguments(_startSpawnPoint).NonLazy();

        // --- Три чекпоинта / UI эффектов ---
        Sprite threeCheckpointIcon = null;
        
        if (_shopItemsConfig != null && _shopItemsConfig.TryGet(ItemType.ThreeCheckpoints, out ShopItemEntry entryForIcon))
            threeCheckpointIcon = entryForIcon.Icon;

        Container.Bind<ThreeCheckpointsInventory>().AsSingle().WithArguments(threeCheckpointIcon).NonLazy();
        Container.BindInstance(_threeCheckpointPrefab).WithId(ID_THREE_CHECKPOINT_PREFAB);
        Container.BindInstance(_threeCheckpointPreviewPrefab).WithId(ID_THREE_CHECKPOINT_PREVIEW_PREFAB);
        Container.BindInstance(_threeCheckpointParent).WithId(ID_THREE_CHECKPOINT_PARENT);
        Container.BindInterfacesAndSelfTo<ThreeCheckpointPlacer>().AsSingle().NonLazy();
        Container.Bind<EffectCountersPanel>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}