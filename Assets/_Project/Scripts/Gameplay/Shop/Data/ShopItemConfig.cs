using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Configs/Shop/Item Data")]
public class ShopItemConfig : ScriptableObject
{
    [SerializeField] private List<ShopItemEntry> _items = new List<ShopItemEntry>();

    [Header("Effect Settings")]
    [SerializeField] private float _effectDurationSeconds;
    [SerializeField] private float _jumpForceMultiplier;
    [SerializeField] private float _movementSpeedMultiplier;
    [SerializeField] private float _animationSpeedMultiplier;
    [SerializeField] private float _trackingVisualLifetimeOverrideSeconds;

    public IReadOnlyList<ShopItemEntry> Items => _items;
    public float EffectDurationSeconds => _effectDurationSeconds;
    public float JumpForceMultiplier => _jumpForceMultiplier;
    public float MovementSpeedMultiplier => _movementSpeedMultiplier;
    public float AnimationSpeedMultiplier => _animationSpeedMultiplier;
    public float TrackingVisualLifetimeOverrideSeconds => _trackingVisualLifetimeOverrideSeconds;

    public bool TryGet(ItemType type, out ShopItemEntry entry)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Type == type)
            {
                entry = _items[i];
                
                return true;
            }
        }

        entry = default;
        
        return false;
    }
}