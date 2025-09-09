using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class YGSkinsSaveRepository
{
    private const string PREFS_KEY_SELECTED = "Skins.Selected";
    private const string PREFS_KEY_OWNED = "Skins.Owned"; 
    
    private readonly HashSet<string> ownedCache = new HashSet<string>();
    
    private bool _loadedFromYG;

    public YGSkinsSaveRepository()
    {
        _loadedFromYG = TryLoadFromYG();
        
        if (!_loadedFromYG)
            LoadFromPlayerPrefs();

        SyncCacheIntoYG();
    }
    
    public SkinIdType SelectedSkin
    {
        get
        {
            string ygSkin  = (YG2.saves != null) ? YG2.saves.SelectedSkinId : string.Empty;
            string ppSkin  = PlayerPrefs.GetString(PREFS_KEY_SELECTED, SkinIdType.Default.ToString());

            bool IsValid(string s) => !string.IsNullOrEmpty(s) && Enum.TryParse<SkinIdType>(s, out _);
            bool IsDefault(string s) => string.Equals(s, SkinIdType.Default.ToString(), StringComparison.Ordinal);

            string DecideWinnerLocal(string yg, string pp)
            {
                if (IsValid(yg) && !IsDefault(yg)) return yg;
                if (IsValid(pp) && !IsDefault(pp)) return pp;
                return SkinIdType.Default.ToString();
            }

            string winner = DecideWinnerLocal(ygSkin, ppSkin);
            return Enum.TryParse(winner, out SkinIdType parsed) ? parsed : SkinIdType.Default;
        }
        set
        {
            if (YG2.saves != null)
                YG2.saves.SelectedSkinId = value.ToString();

            PlayerPrefs.SetString(PREFS_KEY_SELECTED, value.ToString());
            PlayerPrefs.Save();
        }
    }

    public bool HasSkin(SkinIdType id)
    {
        bool has = ownedCache.Contains(id.ToString());
        
        return has;
    }

    public void AddSkin(SkinIdType id)
    {
        string key = id.ToString();

        if (ownedCache.Add(key))
        {
            if (YG2.saves != null)
            {
                if (YG2.saves.OwnedSkinsIds == null)
                    YG2.saves.OwnedSkinsIds = new List<string>();

                if (!YG2.saves.OwnedSkinsIds.Contains(key))
                    YG2.saves.OwnedSkinsIds.Add(key);
            }

            SaveOwnedToPlayerPrefs();
        }
    }

    public void SaveNow()
    {
        if (YG2.saves != null)
            YG2.SaveProgress();

        SaveOwnedToPlayerPrefs();

        string selected = (YG2.saves != null && !string.IsNullOrEmpty(YG2.saves.SelectedSkinId)) ? YG2.saves.SelectedSkinId
                : PlayerPrefs.GetString(PREFS_KEY_SELECTED, SkinIdType.Default.ToString());

        PlayerPrefs.SetString(PREFS_KEY_SELECTED, selected);
        PlayerPrefs.Save();
    }
    
    private bool TryLoadFromYG()
    {
        if (YG2.saves == null)
            return false;

        ownedCache.Clear();
        
        var list = YG2.saves.OwnedSkinsIds;

        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
                ownedCache.Add(list[i]);
        }
        
        return true;
    }

    private void LoadFromPlayerPrefs()
    {
        ownedCache.Clear();

        string csv = PlayerPrefs.GetString(PREFS_KEY_OWNED, string.Empty);

        if (!string.IsNullOrEmpty(csv))
        {
            string[] parts = csv.Split(',');

            for (int i = 0; i < parts.Length; i++)
            {
                string token = parts[i].Trim();
                
                if (!string.IsNullOrEmpty(token))
                    ownedCache.Add(token);
            }
        }

        if (YG2.saves != null)
        {
            if (YG2.saves.OwnedSkinsIds == null)
                YG2.saves.OwnedSkinsIds = new List<string>(ownedCache);
        }
    }

    private void SaveOwnedToPlayerPrefs()
    {
        if (ownedCache.Count == 0)
            return;

        string csv = string.Join(",", ownedCache);
        PlayerPrefs.SetString(PREFS_KEY_OWNED, csv);
        PlayerPrefs.Save();
    }
    
    private void SyncCacheIntoYG()
    {
        var union = new HashSet<string>(ownedCache);
        
        if (YG2.saves != null && YG2.saves.OwnedSkinsIds != null)
        {
            for (int i = 0; i < YG2.saves.OwnedSkinsIds.Count; i++)
                union.Add(YG2.saves.OwnedSkinsIds[i]);
        }
        
        string ppCsv = PlayerPrefs.GetString(PREFS_KEY_OWNED, string.Empty);
        
        if (!string.IsNullOrEmpty(ppCsv))
        {
            string[] parts = ppCsv.Split(',');
            
            for (int i = 0; i < parts.Length; i++)
            {
                string token = parts[i].Trim();
                if (!string.IsNullOrEmpty(token))
                    union.Add(token);
            }
        }
        
        ownedCache.Clear();
        
        foreach (var id in union)
            ownedCache.Add(id);
        
        if (YG2.saves != null)
        {
            if (YG2.saves.OwnedSkinsIds == null)
                YG2.saves.OwnedSkinsIds = new List<string>();
            else
                YG2.saves.OwnedSkinsIds.Clear();

            YG2.saves.OwnedSkinsIds.AddRange(ownedCache);
        }
        
        SaveOwnedToPlayerPrefs();
        
        string ygSelected = (YG2.saves != null) ? YG2.saves.SelectedSkinId : string.Empty;
        string ppSelected = PlayerPrefs.GetString(PREFS_KEY_SELECTED, string.Empty);

        bool IsValid(string s) => !string.IsNullOrEmpty(s) && Enum.TryParse<SkinIdType>(s, out _);
        bool IsDefault(string s) => string.Equals(s, SkinIdType.Default.ToString(), StringComparison.Ordinal);

        string DecideWinner(string yandexSkin, string prefsSkin)
        {
            if (IsValid(yandexSkin) && !IsDefault(yandexSkin))
                return yandexSkin;
            
            if (IsValid(prefsSkin) && !IsDefault(prefsSkin))
                return prefsSkin;
            
            return SkinIdType.Default.ToString();
        }

        string winner = DecideWinner(ygSelected, ppSelected);

        if (YG2.saves != null)
            YG2.saves.SelectedSkinId = winner;

        PlayerPrefs.SetString(PREFS_KEY_SELECTED, winner);
        PlayerPrefs.Save();
    }
}