using UnityEngine;
using YG;

public class YGGameSaveResetter
{
    private const string PREFS_KEY_SELECTED = "Skins.Selected";
    private const string PREFS_KEY_OWNED = "Skins.Owned"; 
    private const string DEFAULT_SKIN_STRING = nameof(SkinIdType.Default);

    public void ResetAllSaves()
    {
        if (YG2.saves != null)
        {
            if (YG2.saves.OwnedSkinsIds != null)
                YG2.saves.OwnedSkinsIds.Clear();
            
            YG2.saves.SelectedSkinId = DEFAULT_SKIN_STRING;
            YG2.SaveProgress();
        }

        PlayerPrefs.DeleteKey(PREFS_KEY_OWNED);
        PlayerPrefs.SetString(PREFS_KEY_SELECTED, DEFAULT_SKIN_STRING);
        PlayerPrefs.Save();
    }
}