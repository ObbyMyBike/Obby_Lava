using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class ShopZoneTrigger : MonoBehaviour
{
    private const bool REQUIRE_TRIGGER_COLLIDER = true;
    
    [SerializeField] private bool _closeOnExit = true;
    
    private ShopSidebarPanelView _shopPanel;
    private ICursorVisible _cursor;
    private IPlatform _platform;
    
    private Collider _collider;
    private bool _openedByThisZine;

    [Inject]
    private void Construct(ShopSidebarPanelView shopPanel, ICursorVisible cursor, IPlatform platform)
    {
        _shopPanel = shopPanel;
        _cursor = cursor;
        _platform = platform;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        
        if (REQUIRE_TRIGGER_COLLIDER)
            _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player _))
            return;

        OpenShopPanel();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Player _))
            return;

        if (_closeOnExit)
            CloseShopPanel();
    }
    
    private void OnDisable()
    {
        CloseShopPanel();
    }
    
    private void OpenShopPanel()
    {
        if (_shopPanel == null)
            return;
        
        _shopPanel.Show();
        _cursor.ShowCursor();
        _openedByThisZine = true;
    }
    
    private void CloseShopPanel()
    {
        if (!_openedByThisZine || _shopPanel == null)
            return;
        
        _shopPanel.Hide();
        _cursor.HideCursor();
        _openedByThisZine = false;
    }
}