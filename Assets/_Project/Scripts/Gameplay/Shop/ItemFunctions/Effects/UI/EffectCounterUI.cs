using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EffectCounterUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Visuals")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countText;

    [Header("Mobile Actions (optional)")]
    [SerializeField] private GameObject _mobileActionsRoot;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    private ThreeCheckpointPlacer _placer;
    private PlatformDefinition _platform;
    private ItemType _type = ItemType.ThreeCheckpoints;
    
    private void OnEnable()
    {
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(OnConfirmClicked);

        if (_cancelButton != null)
            _cancelButton.onClick.AddListener(OnCancelClicked);
    }

    private void OnDisable()
    {
        if (_confirmButton != null)
            _confirmButton.onClick.RemoveListener(OnConfirmClicked);

        if (_cancelButton != null)
            _cancelButton.onClick.RemoveListener(OnCancelClicked);

        if (_placer != null)
            _placer.OnPlacementModeChanged -= OnPlacementModeChanged;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (_type != ItemType.ThreeCheckpoints || _placer == null)
            return;

        bool isMobile = _platform != null && _platform.IsMobile;

        if (isMobile)
        {
            if (!_placer.IsPlacementEnabled)
                _placer.TryBeginPlacement();
        }
    }
    
    public void SetCount(int count) => _countText.text = count.ToString();
    
    public void Setup(Sprite icon, int count)
    {
        _icon.sprite = icon;

        SetCount(count);
        RefreshMobileActionsVisible(false);
    }
    
    public void InjectDependencies(ThreeCheckpointPlacer placer, PlatformDefinition platform)
    {
        _placer = placer;
        _platform = platform;

        if (_placer != null)
            _placer.OnPlacementModeChanged += OnPlacementModeChanged;
    }

    private void RefreshMobileActionsVisible(bool visible)
    {
        if (_mobileActionsRoot != null)
            _mobileActionsRoot.SetActive(visible);
    }
    
    private void OnPlacementModeChanged(bool enabled)
    {
        bool isMobile = _platform != null && _platform.IsMobile;
        
        RefreshMobileActionsVisible(enabled && isMobile);
    }
    
    private void OnConfirmClicked() => _placer?.ConfirmPlacement();

    private void OnCancelClicked() => _placer?.CancelPlacement();
}