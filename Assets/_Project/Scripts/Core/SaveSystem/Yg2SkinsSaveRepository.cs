using System.Collections.Generic;
using YG;

public class Yg2SkinsSaveRepository : ISkinsSaveRepository
{
    private readonly HashSet<string> ownedCache = new HashSet<string>();

    public Yg2SkinsSaveRepository()
    {
        if (YG2.saves.OwnedSkinsIds != null)
        {
            for (int i = 0; i < YG2.saves.OwnedSkinsIds.Count; i++)
            {
                ownedCache.Add(YG2.saves.OwnedSkinsIds[i]);
            }
        }
    }
    
    public SkinIdType SelectedSkin
    {
        get
        {
            if (System.Enum.TryParse<SkinIdType>(YG2.saves.SelectedSkinId, out var id))
                return id;
            return SkinIdType.Default;
        }
        set
        {
            YG2.saves.SelectedSkinId = value.ToString();
        }
    }

    public bool HasSkin(SkinIdType id) => ownedCache.Contains(id.ToString());

    public void AddSkin(SkinIdType id)
    {
        string key = id.ToString();
        if (ownedCache.Add(key))
        {
            if (YG2.saves.OwnedSkinsIds == null)
                YG2.saves.OwnedSkinsIds = new List<string>();
            if (!YG2.saves.OwnedSkinsIds.Contains(key))
                YG2.saves.OwnedSkinsIds.Add(key);
        }
    }

    public void SaveNow()
    {
        YG2.SaveProgress();
    }
}