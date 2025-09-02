using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Configs/Shop/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private ItemType _type;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [SerializeField] private int _price;

    [TextArea] [SerializeField] private string _description;

    [Header("Settings Values Effects")]
    [SerializeField] private float _effectJumpValue = 2f;

    public ItemType Type => _type;
    public Sprite Icon => _icon;
    public string Name => _name;
    public int Price => _price;
    public float EffectJumpValue => _effectJumpValue;
}