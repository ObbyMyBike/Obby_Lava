using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Player/Skins Catalog", fileName = "SkinsCatalogConfig")]
public class SkinsCatalogConfig : ScriptableObject
{
    [SerializeField] private List<SkinDefinition> _skins;
    
    public IReadOnlyList<SkinDefinition> Skins => _skins;

    public bool TryGetDefinition(SkinIdType id, out SkinDefinition definition)
    {
        for (int i = 0; i < _skins.Count; i++)
        {
            if (_skins[i].Id.Equals(id))
            {
                definition = _skins[i];
                
                return true;
            }
        }

        definition = default;
        
        return false;
    }
}