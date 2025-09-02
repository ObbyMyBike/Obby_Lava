public interface ISkinUnlockAndApply
{
    public event OnSkinUnlock OnSkinUnlocked;
    
    public void UnlockAndApply(SkinIdType id);
}