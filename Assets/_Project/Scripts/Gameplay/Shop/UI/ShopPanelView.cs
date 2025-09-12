using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System.Linq;

public class ShopPanelView : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private ShopItemSlotView _shopItemSlotPrefab;

    [Header("Settings UI Panel")]
    [SerializeField] private RectTransform _shopPanel;
    [SerializeField] private float _hiddenX = -500f;
    [SerializeField] private float _shownX = 30f;
    [SerializeField] private float _tweenDuration = 0.5f;

    private ShopItemConfig _shopItemsConfig;
    private SpawnedPlayerAccessor _accessor;
    private AutoPush _autoPush;
    private Wallet _wallet;
    private TimedEffectsRunner _timedEffectsRunner;
    private ThreeCheckpointsInventory _threeCheckpointsInventory;
    private PlayerRespawn _playerRespawn;
    private SpawnedPlayerAccessor _playerAccessor;

    private DiContainer _container;

    private Dictionary<ItemType, ShopItemSlotView> _slotsByType;
    private bool _isVisible;
    private List<CheckpointTrigger> _allCheckpoints;

    [Inject]
    public void Construct(Wallet wallet, TimedEffectsRunner timedEffectsRunner, SpawnedPlayerAccessor accessor, DiContainer container, AutoPush autoPush,
        ShopItemConfig shopItemConfig, ThreeCheckpointsInventory threeCheckpointsInventory, PlayerRespawn playerRespawn, SpawnedPlayerAccessor playerAccessor)
    {
        _wallet = wallet;
        _timedEffectsRunner = timedEffectsRunner;
        _accessor = accessor;
        _container = container;
        _autoPush = autoPush;
        _shopItemsConfig = shopItemConfig;
        _threeCheckpointsInventory = threeCheckpointsInventory;
        _playerRespawn = playerRespawn;
        _playerAccessor = playerAccessor;
        _allCheckpoints = FindObjectsOfType<CheckpointTrigger>().ToList();
    }

    private void Awake()
    {
        _shopPanel.anchoredPosition = new Vector2(_hiddenX, 0);
    }

    private void OnEnable()
    {
        _shopPanel?.DOKill(); 
        
        if (_threeCheckpointsInventory != null)
            _threeCheckpointsInventory.OnEmptied += OnThreeCheckpointsEmptied;
    }

    private void OnDisable()
    {
        _shopPanel?.DOKill();
        
        if (_threeCheckpointsInventory != null)
            _threeCheckpointsInventory.OnEmptied -= OnThreeCheckpointsEmptied;
    }
    
    private void Start()
    {
        if (_shopItemsConfig == null || _container == null)
            return;
        
        PopulateSlots();
    }
    
    public void Show()
    {
        if (_isVisible)
            return;

        _isVisible = true;

        _shopPanel.DOAnchorPosX(_shownX, _tweenDuration).SetEase(Ease.OutCubic);
    }

    public void Hide()
    {
        if (!_isVisible)
            return;

        _isVisible = false;

        _shopPanel.DOAnchorPosX(_hiddenX, _tweenDuration).SetEase(Ease.OutCubic);
    }

    private void OnThreeCheckpointsEmptied(ItemType type) => ResetSlot(ItemType.ThreeCheckpoints);
    
    private void PopulateSlots()
    {
        _slotsByType = new Dictionary<ItemType, ShopItemSlotView>();

        IReadOnlyList<ShopItemEntry> items = _shopItemsConfig.Items;
        
        for (int i = 0; i < items.Count; i++)
        {
            ShopItemEntry entry = items[i];

            ShopItemSlotView slot = _container.InstantiatePrefabForComponent<ShopItemSlotView>(_shopItemSlotPrefab.gameObject, _slotsContainer);
            slot.Setup(entry);
            
            slot.OnPurchased += ItemOnPurchased;

            _slotsByType[entry.Type] = slot;
        }
    }

    private void ItemOnPurchased(ItemType type)
    {
        if (!_shopItemsConfig.TryGet(type, out ShopItemEntry entry))
            return;

        Player player = _accessor.Player;
        
        if (player == null)
            return;
        
        float duration = Mathf.Max(0.01f, _shopItemsConfig.EffectDurationSeconds);
        float jumpMultiplier = Mathf.Max(0.01f, _shopItemsConfig.JumpForceMultiplier);
        float movementSpeedMultiplier = Mathf.Max(0.01f, _shopItemsConfig.MovementSpeedMultiplier);
        float animationSpeedMultiplier = Mathf.Max(0.1f,  _shopItemsConfig.AnimationSpeedMultiplier);
        float trailLifeOverride = _shopItemsConfig.TrackingVisualLifetimeOverrideSeconds;
        
        switch (type)
        {
            case ItemType.HighJump:
            {
                _timedEffectsRunner.StartTimed(type, entry.Icon, duration, () =>
                    {
                        player.DataConfig.OverrideJumpForceMultiplier(jumpMultiplier);
                        player.SetJumpMultiplier(jumpMultiplier);
                    }, () =>
                    {
                        player.DataConfig.ResetJumpForceMultiplier();
                        player.ResetJumpMultiplier();
                        ResetSlot(type);
                    }
                );
                
                break;
            }

            case ItemType.SpeedBoost:
            {
                _timedEffectsRunner.StartTimed(type, entry.Icon, duration, () =>
                    {
                        player.DataConfig.OverrideMoveSpeedMultiplier(movementSpeedMultiplier);
                        player.SetMoveSpeedMultiplier(movementSpeedMultiplier);
                        player.SetAnimationSpeedMultiplier(animationSpeedMultiplier);
                    }, () =>
                    {
                        player.DataConfig.ResetMoveSpeedMultiplier();
                        player.ResetMoveSpeedMultiplier();
                        player.ResetAnimationSpeedMultiplier();
                        
                        ResetSlot(type);
                    }
                );
                
                break;
            }

            case ItemType.TrackingLine:
            {
                _timedEffectsRunner.StartTimed(type, entry.Icon, duration, () =>
                    {
                        if (player.Trail != null)
                        {
                            if (trailLifeOverride  > 0f)
                                player.Trail.TryApplyLifetimeOverride(trailLifeOverride);

                            player.Trail.Enable();
                        }
                    }, () =>
                    {
                        if (player.Trail != null)
                            player.Trail.Disable();

                        ResetSlot(type);
                    }
                );
                
                break;
            }

            case ItemType.AutoPush:
            {
                _timedEffectsRunner.StartTimed(type, entry.Icon, duration, () => { _autoPush.Enable(); }, () =>
                    {
                        _autoPush.Disable();
                        
                        ResetSlot(type);
                    }
                );
                
                break;
            }
            
            case ItemType.ThreeCheckpoints:
            {
                _threeCheckpointsInventory.AddPurchasePack();
                
                break;
            }

            case ItemType.Teleport:
            {
                    var currentCheckpoint = _playerRespawn.GetCurrentSpawnPoint();
                    var nextCheckpoint = _allCheckpoints
                        .OrderBy(checkpoint => checkpoint.transform.position.y)
                        .First(checkpoint => checkpoint.transform.position.y > currentCheckpoint.position.y);
                    _playerRespawn.TrySetCheckpoint(nextCheckpoint.transform, nextCheckpoint.transform.position.y);
                    _playerRespawn.RequestRespawn();
                    ResetSlot(type);
                    Hide();
                break;
            }

            default:
                
                Debug.LogWarning($"Item type {type} is not implemented.");
                
                break;
        }
    }

    private void ResetSlot(ItemType type)
    {
        if (_slotsByType != null && _slotsByType.TryGetValue(type, out ShopItemSlotView slot))
            slot.ResetPurchase();
    }
}