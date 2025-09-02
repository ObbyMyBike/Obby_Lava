public interface IPlayerSkinApplier
{
    public event OnSkinApply OnSkinApplied;
    
    public void ApplySkin(SkinIdType id);
}