using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Player/PlayerDestructorConfig", fileName = "PlayerDestructorConfig")]
public class PlayerDestructorConfig : ScriptableObject
{
    [System.Serializable]
    private class DestructorConfig
    {
        [SerializeField] private SkinIdType _skinIdType;
        [SerializeField] private PlayerDestructorView _distructorPrefab;

        public SkinIdType SkinIdType => _skinIdType;
        public PlayerDestructorView DistructorPrefab => _distructorPrefab;
    }

    [SerializeField] private float _demonstrateTime;
    [SerializeField] private List<DestructorConfig> _configs;
    private Dictionary<SkinIdType, DestructorConfig> _skinIdViewMapper = null;

    public float DemonstrateTime => _demonstrateTime;

    public PlayerDestructorView GetPrefabBySkin(SkinIdType skinIdType)
    {
        if (_skinIdViewMapper == null)
        {
            InitSkinViewMapper();
        }

        if (_skinIdViewMapper.TryGetValue(skinIdType, out DestructorConfig config))
        {
            return config.DistructorPrefab;
        }
        else
        {
            throw new System.Exception($"PlayerDistructorConfig does not contain SkinIdType {skinIdType}! Configure PlayerDistructorConfig!");
        }
    }

    private void InitSkinViewMapper()
    {
        _skinIdViewMapper = new();
        _configs.ForEach(config =>
        {
            _skinIdViewMapper.Add(config.SkinIdType, config);
        });
    }
}