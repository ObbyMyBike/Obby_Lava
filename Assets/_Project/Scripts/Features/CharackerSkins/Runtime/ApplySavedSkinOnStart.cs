using Zenject;

public class ApplySavedSkinOnStart : IInitializable
{
    private readonly ISkinsSaveRepository repository;
    private readonly IPlayerSkinApplier applier;

    public ApplySavedSkinOnStart(ISkinsSaveRepository repository, IPlayerSkinApplier applier)
    {
        this.repository = repository;
        this.applier = applier;
    }

    void IInitializable.Initialize() => applier.ApplySkin(repository.SelectedSkin);
}