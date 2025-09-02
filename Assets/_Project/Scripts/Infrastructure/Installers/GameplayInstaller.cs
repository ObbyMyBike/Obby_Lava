using UnityEngine;
using Cinemachine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private PlayerDataConfig  _playerDataConfig;
    [SerializeField] private InputConfig _inputConfig;
    [SerializeField] private MobileInput _mobileInput;
    [SerializeField] private Camera _mainCamera;
    
    [Header("Ladder")]
    [SerializeField] private LadderSettingsConfig _ladderSettings;
    [SerializeField] private LayerMask _ladderMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private CharacterController _playerController;
    [SerializeField] private Transform _playerTransform;
    
    [Header("Skins System")]
    [SerializeField] private SkinsCatalogConfig _skinsCatalog;
    [SerializeField] private Transform _playerModelRoot;
    
    [Header("Other Settings")]
    [SerializeField] private bool _enableDevHotkeys = true;
    [SerializeField] private bool _isMobile = false;
    
    public override void InstallBindings()
    {
        bool isMobile = Application.isMobilePlatform || _isMobile;
        
        Container.Bind<IPlatform>().To<PlatformDefinition>().AsSingle().WithArguments(isMobile);
        Container.Bind<ICursorVisible>().To<CursorVisibility>().AsSingle().NonLazy();
        
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
        
        Container.Bind<ICameraProvider>().To<MainCameraProvider>().AsSingle().WithArguments(_mainCamera != null ? _mainCamera.transform : null).NonLazy();
        Container.BindInterfacesAndSelfTo<ShopBillboardToCamera>().FromComponentsInHierarchy().AsTransient();
        Container.BindInterfacesAndSelfTo<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<Player>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CharacterController>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<GoldWallet>().AsSingle().WithArguments(_playerDataConfig.StartingGold).NonLazy();
        Container.Bind<ShopSidebarPanelView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<MonoBehaviour>().WithId("CoroutineRunner").FromComponentInHierarchy().AsSingle();
        Container.Bind<TimedEffectsRunner>().AsSingle().NonLazy();
        
        Container.Bind<PlayerHealth>().AsSingle().WithArguments(_playerDataConfig.MaxHealth, _playerDataConfig.MaxHealth);
        Container.Bind<PlayerHealthBarPanel>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<LavaContactHealthDrain>().AsSingle().WithArguments(_playerDataConfig.SecondsToDieInLava, _playerDataConfig.RegenPerSecondOutsideLava).NonLazy();
        
        
        Container.Bind<LadderSettingsConfig>().FromInstance(_ladderSettings).AsSingle();
        Container.BindInterfacesAndSelfTo<LadderTriggerVolume>().FromComponentsInHierarchy().AsTransient();
        Container.Bind<ILadderClimbService>().To<LadderClimbService>().AsSingle().WithArguments(_playerController, _playerTransform, _ladderMask, _groundMask, _ladderSettings);
        
        Container.Bind<SkinsCatalogConfig>().FromInstance(_skinsCatalog).AsSingle();
        Container.Bind<IRewardedAdService>().To<YandexGamesRewardedAdService>().AsSingle().NonLazy();
        Container.Bind<ISkinsSaveRepository>().To<Yg2SkinsSaveRepository>().AsSingle().NonLazy();
        Container.Bind<IPlayerSkinApplier>().To<PlayerSkinApplier>().AsSingle().WithArguments(_playerModelRoot, _skinsCatalog).NonLazy();
        Container.BindInterfacesTo<SkinUnlockAndApply>().AsSingle().NonLazy();
        Container.BindInterfacesTo<ApplySavedSkinOnStart>().AsSingle().NonLazy();
        
        Container.Bind<Yg2GameSaveResetter>().AsSingle().NonLazy();
        
        if (_enableDevHotkeys)
            Container.BindInterfacesTo<DevResetProgressOnKeyPress>().AsSingle().NonLazy();
    }
}