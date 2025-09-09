using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EffectCountersPanel : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private EffectCounterUI _counterPrefab;
    
    private readonly Dictionary<ItemType, EffectCounterUI> byType = new Dictionary<ItemType, EffectCounterUI>();
    
    private ThreeCheckpointsInventory _inventory;
    private ThreeCheckpointPlacer _placer;
    private PlatformDefinition _platform;

    [Inject]
    public void Construct(ThreeCheckpointsInventory inventory, ThreeCheckpointPlacer placer, PlatformDefinition platform)
    {
        _inventory = inventory;
        _placer = placer;
        _platform = platform;
    }

    private void OnEnable()
    {
        if (_inventory != null)
        {
            _inventory.OnCountChanged += OnCountChanged;

            if (_inventory.Count > 0)
                ShowOrUpdate(ItemType.ThreeCheckpoints, _inventory.Count, GetIconSafe());
        }
    }

    private void OnDisable()
    {
        if (_inventory != null)
            _inventory.OnCountChanged -= OnCountChanged;
    }
    
    private void OnCountChanged(ItemType type, int count, Sprite icon) => ShowOrUpdate(type, count, icon);

    private void ShowOrUpdate(ItemType type, int count, Sprite icon)
    {
        if (!byType.TryGetValue(type, out EffectCounterUI ui) || ui == null)
        {
            if (count <= 0)
                return;
            
            ui = Instantiate(_counterPrefab, _container);
            byType[type] = ui;
            ui.Setup(icon, count);
            
            if (type == ItemType.ThreeCheckpoints)
                ui.InjectDependencies(_placer, _platform);
            
            return;
        }

        if (count <= 0)
        {
            Destroy(ui.gameObject);
            
            byType.Remove(type);
            
            return;
        }
        
        ui.SetCount(count);
    }
    
    private Sprite GetIconSafe() => _inventory != null ? _inventory.Icon : null;
}