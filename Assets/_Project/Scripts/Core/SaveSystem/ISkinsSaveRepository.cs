public interface ISkinsSaveRepository
{
    public SkinIdType SelectedSkin { get; set; }
    
    public bool HasSkin(SkinIdType id);
    
    public void AddSkin(SkinIdType id);
    
    public void SaveNow();
}