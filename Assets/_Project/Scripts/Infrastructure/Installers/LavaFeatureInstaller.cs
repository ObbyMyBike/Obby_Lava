using UnityEngine;
using Zenject;

public class LavaFeatureInstaller : MonoInstaller
{
    [SerializeField] private LavaProgressionConfig _progressionConfig;
    [SerializeField] private LavaSurfaceView _lavaSurfaceView;
    [SerializeField] private LavaCountdownPanel _countdownPanel;
    [SerializeField] private LavaPreCountdownPanel _preCountdownPanel;

    public override void InstallBindings()
    {
        Container.Bind<LavaProgressionConfig>().FromInstance(_progressionConfig).AsSingle();
        Container.Bind<ILavaSurface>().FromInstance(_lavaSurfaceView).AsSingle();
        
        Container.BindInterfacesAndSelfTo<LavaProgressionFlow>().AsSingle();
        
        Container.Bind<IInitializable>().To<PreCountdownBinder>().AsSingle().WithArguments(_preCountdownPanel);
        Container.Bind<IInitializable>().To<CountdownPanelBinder>().AsSingle().WithArguments(_countdownPanel);
        
        Container.Bind<IInitializable>().To<FlowStartup>().AsSingle();
    }
}