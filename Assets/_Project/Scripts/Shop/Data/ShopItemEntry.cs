using System;
using UnityEngine;

[Serializable]
public struct ShopItemEntry
{
    public ItemType Type;
    public Sprite Icon;
    public string Name;
    public int Price;
    [TextArea] public string Description;
}