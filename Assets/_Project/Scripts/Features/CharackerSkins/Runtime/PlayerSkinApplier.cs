using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerSkinApplier
{
    private const string LOG_PREFIX = "[SkinApplier]";
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
        if (!IsReady) TryInitFromAccessor();
        if (!IsReady)
        {
            Debug.LogWarning("[SkinApplier]ApplySkin aborted: not ready (retry next frame)");
            // План Б: один безопасный «ленивый» повтор в следующий кадр
            MonoBehaviour.print("");
            // coroutineRunner.StartCoroutine(ApplyNextFrame(id));
            return;
        }
        
        if (_playerModelRoot == null)
            TryInitFromAccessor();
        
        if (_playerModelRoot == null)
        {
            Debug.LogWarning(LOG_PREFIX + $"ApplySkin({id}) aborted: playerModelRoot is null");
            return;
        }
        
        if (!catalog.TryGetDefinition(id, out SkinDefinition definition))
        {
            Debug.LogWarning(LOG_PREFIX + $"ApplySkin({id}) aborted: no definition in catalog");
            return;
        }

        Debug.Log(LOG_PREFIX + $"ApplySkin({id}) -> ChildName='{definition.ChildNameUnderPlayerModel}'");
        
        for (int i = 0; i < _playerModelRoot.childCount; i++)
            _playerModelRoot.GetChild(i).gameObject.SetActive(false);

        if (childrenByName.TryGetValue(definition.ChildNameUnderPlayerModel, out GameObject child))
        {
            child.SetActive(true);
            Debug.Log(LOG_PREFIX + $"Enabled child '{child.name}'");
            OnSkinApplied?.Invoke(id);
        }
        else
        {
            Debug.LogWarning(LOG_PREFIX + $"Child '{definition.ChildNameUnderPlayerModel}' not found under PlayerModel. Fallback to '{DEFAULT_SKIN_NAME}'");

            if (childrenByName.TryGetValue(DEFAULT_SKIN_NAME, out GameObject fallback))
            {
                fallback.SetActive(true);
                Debug.Log(LOG_PREFIX + $"Enabled fallback '{fallback.name}'");
            }
        }
    }
    
    private void TryInitFromAccessor()
    {
        if (_playerModelRoot == null)
            _playerModelRoot = accessor.PlayerModelRoot;

        if (_playerModelRoot == null)
        {
            Debug.LogWarning("[SkinApplier]PlayerModelRoot not ready yet");
            return;
        }

        CacheChildren();
        Debug.Log("[SkinApplier]Cached children: " + string.Join(", ", childrenByName.Keys));
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