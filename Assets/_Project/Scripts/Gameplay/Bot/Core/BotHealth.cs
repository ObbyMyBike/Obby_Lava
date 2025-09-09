using UnityEngine;

public class BotHealth
{
    private const int MIN_HEALTH = 0;

    public event OnBotHealthChange OnChanged;
    public event OnDie OnDied;

    private int _currentHealth;
    private float _preciseHealth;

    public BotHealth(int maxHealth, int startHealth)
    {
        MaxHealth = Mathf.Max(1, maxHealth);
        _currentHealth = Mathf.Clamp(startHealth, MIN_HEALTH, MaxHealth);
        _preciseHealth = _currentHealth;
        
        OnChanged?.Invoke(_currentHealth, MaxHealth);
    }

    public int MaxHealth { get; private set; }
    public bool IsDead => _currentHealth <= 0;

    public void TryApplyDamage(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        _preciseHealth = Mathf.Max(MIN_HEALTH, _preciseHealth - amount);
        int newValue = Mathf.RoundToInt(_preciseHealth);

        if (newValue != _currentHealth)
        {
            _currentHealth = newValue;
            OnChanged?.Invoke(_currentHealth, MaxHealth);

            if (_currentHealth <= 0)
                OnDied?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        _preciseHealth = Mathf.Min(MaxHealth, _preciseHealth + amount);
        int newValue = Mathf.RoundToInt(_preciseHealth);

        if (newValue != _currentHealth)
            _currentHealth = newValue;

        OnChanged?.Invoke(_currentHealth, MaxHealth);
    }

    public void RestoreToMax()
    {
        if (_currentHealth == MaxHealth)
            return;

        _preciseHealth = MaxHealth;
        _currentHealth = MaxHealth;
        
        OnChanged?.Invoke(_currentHealth, MaxHealth);
    }
}