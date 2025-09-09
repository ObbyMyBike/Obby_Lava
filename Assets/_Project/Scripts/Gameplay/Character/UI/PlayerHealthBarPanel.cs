using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class PlayerHealthBarPanel : MonoBehaviour
{
    [SerializeField] private Slider _bar;
    [SerializeField] private TextMeshProUGUI _valueText;
    
    private PlayerHealth _health;

    [Inject]
    public void Construct(PlayerHealth health)
    {
        _health = health;
        
        _health.OnHealthChanged += HandleChanged;
        _health.OnDied += HandleDied;
    }

    private void Start()
    { 
        if (_health != null)
            HandleChanged(_health.CurrentHealth, _health.MaxHealth);
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnHealthChanged -= HandleChanged;
            _health.OnDied -= HandleDied;
        }
    }

    private void HandleChanged(int current, int max)
    {
        if (_bar != null)
        {
            _bar.minValue = 0f;
            _bar.maxValue = max;
            _bar.value = current;
        }

        if (_valueText != null)
            _valueText.text = $"{current}/{max}";
    }
    
    private void HandleDied()
    {
        HandleChanged(0, _health.MaxHealth);
    }
}