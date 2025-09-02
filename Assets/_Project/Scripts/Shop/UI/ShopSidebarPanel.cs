using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

public class ShopSidebarPanel : MonoBehaviour
{
    private const float EFFECT_DURATION_SECONDS = 180f;

    [Header("Items")]
    [SerializeField] private List<ItemData> _allItems;
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private ShopItemSlotView _shopItemSlotPrefab;

    [Header("Settings UI Panel")]
    [SerializeField] private RectTransform _shopPanel;
    [SerializeField] private Button _toggleButton;
    [SerializeField] private float _hiddenX = -500f;
    [SerializeField] private float _shownX = 30f;
    [SerializeField] private float _tweenDuration = 0.5f;

    private Player _player;
    private GoldWallet _goldWallet;
    private TimedEffectsRunner _timedEffectsRunner;
    private DiContainer _container;
    
    private Dictionary<ItemType, ShopItemSlotView> _slotsByType;
    private bool _isVisible;

    [Inject]
    public void Construct(GoldWallet goldWallet, TimedEffectsRunner timedEffectsRunner, Player player, DiContainer container)
    {
        _goldWallet = goldWallet;
        _timedEffectsRunner = timedEffectsRunner;
        _player = player;
        _container = container;
    }

    private void Awake()
    {
        _shopPanel.anchoredPosition = new Vector2(_hiddenX, 0);
        
        _toggleButton.onClick.AddListener(ToggleVisibility);
        PopulateSlots();
    }

    private void PopulateSlots()
    {
        _slotsByType = new Dictionary<ItemType, ShopItemSlotView>();
        
        foreach (ItemData itemData in _allItems)
        {
            ShopItemSlotView slot = _container.InstantiatePrefabForComponent<ShopItemSlotView>(this._shopItemSlotPrefab.gameObject, _slotsContainer);
            slot.Setup(itemData);

            slot.OnPurchased += ItemOnPurchased;
            
            _slotsByType[itemData.Type] = slot;
        }
    }

    private void ToggleVisibility()
    {
        _isVisible = !_isVisible;
        
        float targetX = _isVisible ? _shownX : _hiddenX;

        _shopPanel.DOAnchorPosX(targetX, _tweenDuration).SetEase(Ease.OutCubic);
    }

    private void ItemOnPurchased(ItemType type)
    {
        ItemData data = _allItems.First(x => x.Type == type);
        Sprite icon = data.Icon;
        float boost = data.EffectJumpValue;

        _timedEffectsRunner.StartTimed(
            type,
            icon,
            EFFECT_DURATION_SECONDS,
            onStart: () =>
            {
                if (type == ItemType.HighJump)
                    _player.SetJumpMultiplier(boost);
            },
            onEnd: () =>
            {
                if (type == ItemType.HighJump)
                    _player.SetJumpMultiplier(_player.DataConfig.JumpForce);
                
                if (_slotsByType.TryGetValue(type, out ShopItemSlotView slot))
                    slot.ResetPurchase();
            }
        );
    }
}