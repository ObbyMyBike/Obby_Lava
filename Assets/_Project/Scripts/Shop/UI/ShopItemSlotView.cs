using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Zenject;

public class ShopItemSlotView : MonoBehaviour
{
    public event OnItemPurchased OnPurchased;

    [SerializeField] private ButtonAnimationSettings _buttonAnimationSettings;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _purchasedImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;
    
    private ItemData _data;
    private GoldWallet _goldWallet;
    private PurchaseButtonAnimator _animator;

    [Inject]
    public void Construct(GoldWallet goldWallet)
    {
        _goldWallet = goldWallet;
    }

    private void Awake()
    {
        _animator = new PurchaseButtonAnimator(_buyButton.transform, _purchasedImage, _buttonAnimationSettings.PressScale, _buttonAnimationSettings.PressTime,
            _buttonAnimationSettings.ShakeTime, _buttonAnimationSettings.ShakeStrength, _buttonAnimationSettings.ShakeVibrato);

        EventTrigger trigger = _buyButton.gameObject.GetComponent<EventTrigger>() ?? _buyButton.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();
        
        EventTrigger.Entry entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entryDown.callback.AddListener(_ => { _animator.PressDown(); });
        trigger.triggers.Add(entryDown);
        
        EventTrigger.Entry entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entryUp.callback.AddListener((_ => { OnPointerUp(); }));
        trigger.triggers.Add(entryUp);
    }
    
    public void Setup(ItemData data)
    {
        _data = data;
        _iconImage.sprite = _data.Icon;
        _nameText.text = _data.Name;
        _priceText.text = _data.Price.ToString();

        _buyButton.interactable = true;
        
        _purchasedImage.gameObject.SetActive(false);
    }

    public void ResetPurchase()
    {
        _buyButton.interactable = true;
        _purchasedImage.gameObject.SetActive(false);
        _buyButton.transform.localScale = Vector3.one;
    }
    
    private bool TryBuy()
    {
        if (_goldWallet.TrySpend(_data.Price))
        {
            _buyButton.interactable = false;

            OnPurchased?.Invoke(_data.Type);

            return true;
        }
        
        Debug.Log("Not enough gold!");

        return false;
    }
    
    private void OnPointerUp()
    {
        bool success = TryBuy();

        if (success)
            _animator.Success();
        else
            _animator.Fail();
    }
}