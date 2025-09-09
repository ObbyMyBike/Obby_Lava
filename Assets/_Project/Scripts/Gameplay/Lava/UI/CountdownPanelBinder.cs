using Zenject;

public class CountdownPanelBinder : IInitializable
{
    private readonly LavaCountdownPanel _panel;
    private readonly LavaProgressionFlow _flow;

    public CountdownPanelBinder(LavaCountdownPanel panel, LavaProgressionFlow flow)
    {
        _panel = panel;
        _flow = flow;
    }

    void IInitializable.Initialize() => _panel?.Connect(_flow);
}