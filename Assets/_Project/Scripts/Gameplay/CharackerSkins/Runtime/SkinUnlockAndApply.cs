public class SkinUnlockAndApply
{
    public event OnSkinUnlock OnSkinUnlocked;

    private readonly YGSkinsSaveRepository repository;
    private readonly PlayerSkinApplier applier;

    public SkinUnlockAndApply(YGSkinsSaveRepository repository, PlayerSkinApplier applier)
    {
        this.repository = repository;
        this.applier = applier;
    }

    public void UnlockAndApply(SkinIdType id)
    {
        if (!repository.HasSkin(id))
            repository.AddSkin(id);

        repository.SelectedSkin = id;
        applier.ApplySkin(id);
        repository.SaveNow();
        
        
        OnSkinUnlocked?.Invoke(id);
    }
}