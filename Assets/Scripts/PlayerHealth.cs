using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private HealthBar healthBar;
    
    public delegate void HealthHandler(int currentHealth, int maxHealth);
    public event HealthHandler OnHealthValueChangedEvent;
    
    public override void Damage(int damageAmount)
    {
        base.Damage(damageAmount);
        
        OnHealthValueChangedEvent?.Invoke(CurrentHealth, MaxHealth);
    }

    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        
        OnHealthValueChangedEvent?.Invoke(CurrentHealth, MaxHealth);
    }

    public override void IncreaseMaxHealth(int increaseAmount)
    {
        base.IncreaseMaxHealth(increaseAmount);
        
        OnHealthValueChangedEvent?.Invoke(CurrentHealth, MaxHealth);
    }

    public override void ReduceMaxHealth(int reduceAmount)
    {
        base.ReduceMaxHealth(reduceAmount);
        
        OnHealthValueChangedEvent?.Invoke(CurrentHealth, MaxHealth);
    }
}
