using Zenject;

public class PreCountdownBinder : IInitializable
{
    private readonly LavaPreCountdownPanel panel;
    private readonly LavaProgressionFlow flow;

    public PreCountdownBinder(LavaPreCountdownPanel panel, LavaProgressionFlow flow)
    {
        this.panel = panel;
        this.flow = flow;
    }

    void IInitializable.Initialize() => panel?.Connect(flow);
}