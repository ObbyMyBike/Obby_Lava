using Zenject;

public class FlowStartup : IInitializable
{
    private readonly LavaProgressionFlow flow;

    public FlowStartup(LavaProgressionFlow flow) => this.flow = flow;

    void IInitializable.Initialize() => flow.StartSequence();
}