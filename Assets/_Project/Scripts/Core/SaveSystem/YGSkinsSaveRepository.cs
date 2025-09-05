using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class YGSkinsSaveRepository
{
    private const string LOG_PREFIX = "[SkinRepo]";
    private const string PREFS_KEY_SELECTED = "Skins.Selected";
    private const string PREFS_KEY_OWNED = "Skins.Owned"; 
    
    private readonly HashSet<string> ownedCache = new HashSet<string>();
    
    private bool _loadedFromYG;

    public YGSkinsSaveRepository()
    {
        Debug.Log(LOG_PREFIX + " Ctor start");

        _loadedFromYG = TryLoadFromYG();
        Debug.Log(LOG_PREFIX + $" TryLoadFromYG: loaded={_loadedFromYG}, ownedCache={string.Join(",", ownedCache)}");
        
        if (!_loadedFromYG)
        {
            Debug.Log(LOG_PREFIX + " Fallback to PlayerPrefs (YG not loaded)");
            LoadFromPlayerPrefs();
            Debug.Log(LOG_PREFIX + $" After LoadFromPlayerPrefs ownedCache={string.Join(",", ownedCache)}");
        }

        SyncCacheIntoYG();
        Debug.Log(LOG_PREFIX + " Ctor done");
    }
    
    public SkinIdType SelectedSkin
    {
        get
        {
            string ygSkin  = (YG2.saves != null) ? YG2.saves.SelectedSkinId : string.Empty;
            string ppSkin  = PlayerPrefs.GetString(PREFS_KEY_SELECTED, SkinIdType.Default.ToString());

            Debug.Log(LOG_PREFIX + $"SelectedSkin.get  YG='{ygSkin}'  PP='{ppSkin}'");

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
            Debug.Log(LOG_PREFIX + $"SelectedSkin.set -> {value}");

            if (YG2.saves != null)
                YG2.saves.SelectedSkinId = value.ToString();

            PlayerPrefs.SetString(PREFS_KEY_SELECTED, value.ToString());
            PlayerPrefs.Save();

            Debug.Log(LOG_PREFIX + $"SelectedSkin.set saved  YG='{(YG2.saves!=null?YG2.saves.SelectedSkinId:"<yg=null>")}',  PP='{PlayerPrefs.GetString(PREFS_KEY_SELECTED,"")}'");
        }
    }

    public bool HasSkin(SkinIdType id)
    {
        bool has = ownedCache.Contains(id.ToString());
        Debug.Log(LOG_PREFIX + $" HasSkin({id}) -> {has}");
        return has;
    }

    public void AddSkin(SkinIdType id)
    {
        string key = id.ToString();
        Debug.Log(LOG_PREFIX + $" AddSkin({id})");

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

        Debug.Log(LOG_PREFIX + $" Owned now: {string.Join(",", ownedCache)}  | YG:{(YG2.saves != null ? string.Join(",", YG2.saves.OwnedSkinsIds ?? new List<string>()) : "<null>")}");
    }

    public void SaveNow()
    {
        Debug.Log(LOG_PREFIX + " SaveNow() begin");

        if (YG2.saves != null)
            YG2.SaveProgress();

        SaveOwnedToPlayerPrefs();

        string selected =
            (YG2.saves != null && !string.IsNullOrEmpty(YG2.saves.SelectedSkinId))
                ? YG2.saves.SelectedSkinId
                : PlayerPrefs.GetString(PREFS_KEY_SELECTED, SkinIdType.Default.ToString());

        PlayerPrefs.SetString(PREFS_KEY_SELECTED, selected);
        PlayerPrefs.Save();

        Debug.Log(LOG_PREFIX + $" SaveNow() done. YG='{(YG2.saves != null ? YG2.saves.SelectedSkinId : "<yg=null>")}', PP='{PlayerPrefs.GetString(PREFS_KEY_SELECTED, "")}'");
    }
    
    private bool TryLoadFromYG()
    {
        if (YG2.saves == null)
        {
            Debug.Log(LOG_PREFIX + " TryLoadFromYG: YG2.saves == null");
            return false;
        }

        ownedCache.Clear();
        var list = YG2.saves.OwnedSkinsIds;

        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
                ownedCache.Add(list[i]);
        }

        Debug.Log(LOG_PREFIX + $" TryLoadFromYG OK. Owned={string.Join(",", ownedCache)}, Selected='{YG2.saves.SelectedSkinId}'");
        return true;
    }

    private void LoadFromPlayerPrefs()
    {
        ownedCache.Clear();

        string csv = PlayerPrefs.GetString(PREFS_KEY_OWNED, string.Empty);
        Debug.Log(LOG_PREFIX + $" LoadFromPlayerPrefs: csv='{csv}'");

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
        {
            Debug.Log(LOG_PREFIX + " SaveOwnedToPlayerPrefs: nothing to save (empty cache)");
            return;
        }

        string csv = string.Join(",", ownedCache);
        PlayerPrefs.SetString(PREFS_KEY_OWNED, csv);
        PlayerPrefs.Save();

        Debug.Log(LOG_PREFIX + $" SaveOwnedToPlayerPrefs: '{csv}'");
    }
    
    private void SyncCacheIntoYG()
    {
        Debug.Log(LOG_PREFIX + " SyncCacheIntoYG begin");

        // Собираем всё, что знаем, в общий сет
        var union = new HashSet<string>(ownedCache);

        // Из YG
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

        string DecideWinner(string yg, string pp)
        {
            if (IsValid(yg) && !IsDefault(yg)) return yg;
            if (IsValid(pp) && !IsDefault(pp)) return pp;
            return SkinIdType.Default.ToString();
        }

        string winner = DecideWinner(ygSelected, ppSelected);

        if (YG2.saves != null)
            YG2.saves.SelectedSkinId = winner;

        PlayerPrefs.SetString(PREFS_KEY_SELECTED, winner);
        PlayerPrefs.Save();

        string ygAfter = (YG2.saves != null) ? YG2.saves.SelectedSkinId : "<null>";
        string ppAfter = PlayerPrefs.GetString(PREFS_KEY_SELECTED, SkinIdType.Default.ToString());
        Debug.Log(LOG_PREFIX + $" SyncCacheIntoYG end. YG='{ygAfter}', PP='{ppAfter}', Owned={string.Join(",", ownedCache)}");
    }
}