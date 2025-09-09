using UnityEngine;

public class ThreeCheckpointsInventory
{
    private const int ADD_CHARGES_ON_PURCHASE = 3;
    
    public event OnCountChange OnCountChanged;
    public event OnEmpty OnEmptied;

    private readonly Sprite icon;

    private int _count;

    public ThreeCheckpointsInventory(Sprite icon)
    {
        this.icon = icon;
    }
    
    public int Count => _count;
    public Sprite Icon => icon;

    public bool TryConsumeOne()
    {
        if (_count <= 0)
            return false;
        
        _count = Mathf.Max(0, _count - 1);
        
        OnCountChanged?.Invoke(ItemType.ThreeCheckpoints, _count, icon);
        
        if (_count == 0)
            OnEmptied?.Invoke(ItemType.ThreeCheckpoints);
        
        return true;
    }
    
    public void AddPurchasePack()
    {
        _count += ADD_CHARGES_ON_PURCHASE;
        
        OnCountChanged?.Invoke(ItemType.ThreeCheckpoints, _count, icon);
    }
}