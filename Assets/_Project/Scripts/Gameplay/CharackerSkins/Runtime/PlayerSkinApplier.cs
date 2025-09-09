using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerSkinApplier
{
    private const string DEFAULT_SKIN_NAME = "Default";
    
    public event OnSkinApply OnSkinApplied;

    private readonly SpawnedPlayerAccessor accessor;
    private readonly SkinsCatalogConfig catalog;
    private readonly Dictionary<string, GameObject> childrenByName = new Dictionary<string, GameObject>();
    
    private Transform _playerModelRoot;

    public PlayerSkinApplier(SkinsCatalogConfig catalog, SpawnedPlayerAccessor accessor, [InjectOptional] PlayerPrefabSpawn spawner)
    {
        this.catalog = catalog;
        this.accessor = accessor;

        TryInitFromAccessor();
        
        if (!IsReady && spawner != null)
            spawner.OnPlayerSpawned += _ => TryInitFromAccessor();
    }
    
    public bool IsReady => _playerModelRoot != null && childrenByName.Count > 0;

    public void ApplySkin(SkinIdType id)
    {
        if (!IsReady)
            TryInitFromAccessor();
        
        if (!IsReady)
        {
            MonoBehaviour.print("");
            
            return;
        }
        
        if (_playerModelRoot == null)
            TryInitFromAccessor();
        
        if (_playerModelRoot == null)
            return;
        
        if (!catalog.TryGetDefinition(id, out SkinDefinition definition))
            return;
        
        for (int i = 0; i < _playerModelRoot.childCount; i++)
            _playerModelRoot.GetChild(i).gameObject.SetActive(false);

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
    
    private void TryInitFromAccessor()
    {
        if (_playerModelRoot == null)
            _playerModelRoot = accessor.PlayerModelRoot;

        if (_playerModelRoot == null)
            return;

        CacheChildren();
    }
    
    private void CacheChildren()
    {
        childrenByName.Clear();

        if (_playerModelRoot == null)
            return;
        
        for (int i = 0; i < _playerModelRoot.childCount; i++)
        {
            GameObject child = _playerModelRoot.GetChild(i).gameObject;
            string key = child.name.Trim();
            
            if (!childrenByName.ContainsKey(key))
                childrenByName.Add(key, child);
        }
    }
}