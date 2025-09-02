public class SkinUnlockAndApply : ISkinUnlockAndApply
{
    public event OnSkinUnlock OnSkinUnlocked;

    private readonly ISkinsSaveRepository repository;
    private readonly IPlayerSkinApplier applier;

    public SkinUnlockAndApply(ISkinsSaveRepository repository, IPlayerSkinApplier applier)
    {
        this.repository = repository;
        this.applier = applier;
    }

    void ISkinUnlockAndApply.UnlockAndApply(SkinIdType id)
    {
        if (!repository.HasSkin(id))
            repository.AddSkin(id);

        repository.SelectedSkin = id;
        applier.ApplySkin(id);
        repository.SaveNow();
        
        OnSkinUnlocked?.Invoke(id);
    }
}