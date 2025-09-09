using UnityEngine;

public class PlayerHealth
{
    private const int MIN_HEALTH = 0;
    
    public event OnHealthChange OnHealthChanged;
    public event OnDie OnDied;
    
    private float _preciseHealth;

    public PlayerHealth(int maxHealth, int startHealth)
    {
        MaxHealth = Mathf.Max(1, maxHealth);
        CurrentHealth = Mathf.Clamp(startHealth, MIN_HEALTH, MaxHealth);
        _preciseHealth = CurrentHealth;

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public void TryApplyDamage(float amount)
    {
        if (IsDead || amount <= 0f)
            return;
        
        _preciseHealth = Mathf.Max(MIN_HEALTH, _preciseHealth - amount);

        int newValue = Mathf.RoundToInt(_preciseHealth);
        
        if (newValue != CurrentHealth)
        {
            CurrentHealth = newValue;
            
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0)
                OnDied?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;
        
        _preciseHealth = Mathf.Min(MaxHealth, _preciseHealth + amount);

        int newValue = Mathf.RoundToInt(_preciseHealth);
        
        if (newValue != CurrentHealth)
        {
            CurrentHealth = newValue;
            
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }
    }

    public void RestoreToMax()
    {
        if (CurrentHealth == MaxHealth && Mathf.Approximately(_preciseHealth, MaxHealth))
            return;

        _preciseHealth = MaxHealth;
        CurrentHealth = MaxHealth;
        
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
    
    public void ForceKill()
    {
        if (IsDead)
            return;

        _preciseHealth = MIN_HEALTH;
        CurrentHealth = MIN_HEALTH;
        
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnDied?.Invoke();
    }

    public void SetMaxHealth(int maxHealth, bool fill = true)
    {
        MaxHealth = Mathf.Max(1, maxHealth);

        if (fill)
            CurrentHealth = MaxHealth;

        CurrentHealth = Mathf.Clamp(CurrentHealth, MIN_HEALTH, MaxHealth);
        _preciseHealth = CurrentHealth;

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}