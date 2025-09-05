using UnityEngine;

public class SkinUnlockAndApply
{
    private const string LOG_PREFIX = "[SkinUnlock] ";
    
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
        Debug.Log(LOG_PREFIX + $"UnlockAndApply({id}) start");

        if (!repository.HasSkin(id))
        {
            Debug.Log(LOG_PREFIX + $"AddSkin({id})");
            repository.AddSkin(id);
        }

        repository.SelectedSkin = id;
        applier.ApplySkin(id);
        repository.SaveNow();
        
        Debug.Log(LOG_PREFIX + $"UnlockAndApply({id}) done. Selected now: {repository.SelectedSkin}");
        
        OnSkinUnlocked?.Invoke(id);
    }
}