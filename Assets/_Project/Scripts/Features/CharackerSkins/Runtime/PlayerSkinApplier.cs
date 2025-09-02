using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinApplier : IPlayerSkinApplier
{
    private const string DEFAULT_SKIN_NAME = "Default";
    
    public event OnSkinApply OnSkinApplied;

    private readonly Transform playerModelRoot;
    private readonly SkinsCatalogConfig catalog;
    private readonly Dictionary<string, GameObject> childrenByName = new Dictionary<string, GameObject>();

    public PlayerSkinApplier(Transform playerModelRoot, SkinsCatalogConfig catalog)
    {
        this.playerModelRoot = playerModelRoot;
        this.catalog = catalog;

        
        if (playerModelRoot == null)
            return;
        
        CacheChildren();
        
        string names = string.Empty;
        
        for (int i = 0; i < playerModelRoot.childCount; i++)
            
            names += playerModelRoot.GetChild(i).name;
    }

    void IPlayerSkinApplier.ApplySkin(SkinIdType id)
    {
        if (playerModelRoot == null)
            return;
        
        if (!catalog.TryGetDefinition(id, out SkinDefinition definition))
            return;

        for (int i = 0; i < playerModelRoot.childCount; i++)
            playerModelRoot.GetChild(i).gameObject.SetActive(false);

        if (childrenByName.TryGetValue(definition.ChildNameUnderPlayerModel, out GameObject child))
        {
            child.SetActive(true);
            
            OnSkinApplied?.Invoke(id);
        }
        else
        {
            if (childrenByName.TryGetValue(DEFAULT_SKIN_NAME, out GameObject fallback))
                fallback.SetActive(true);
        }
    }
    
    private void CacheChildren()
    {
        childrenByName.Clear();

        if (playerModelRoot == null)
            return;
        
        for (int i = 0; i < playerModelRoot.childCount; i++)
        {
            GameObject child = playerModelRoot.GetChild(i).gameObject;
            
            string key = child.name.Trim();
            
            if (!childrenByName.ContainsKey(key))
                childrenByName.Add(key, child);
        }
    }
}