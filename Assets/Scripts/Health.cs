using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth => maxHealth;
    public int CurrentHealth => _currentHealth;

    [SerializeField] private int maxHealth;
    private int _currentHealth;
    
    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public virtual void Damage(int damageAmount)
    {
        _currentHealth -= damageAmount;
    }

    public virtual void Heal(int healAmount)
    {
        _currentHealth = healAmount;

        if (_currentHealth > maxHealth)
            _currentHealth = maxHealth;
    }

    public virtual void IncreaseMaxHealth(int increaseAmount)
    {
        maxHealth = increaseAmount;
    }

    public virtual void ReduceMaxHealth(int reduceAmount)
    {
        maxHealth = reduceAmount;
        
        if (_currentHealth > maxHealth)
            _currentHealth = maxHealth;
    }
}
