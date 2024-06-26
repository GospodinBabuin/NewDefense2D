using Buildings;
using Unity.Netcode;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth => maxHealth;
    public int CurrentHealth => _currentHealth;

    [SerializeField] private int maxHealth;
    [SerializeField] private int _currentHealth;
    
    private int _animIDDie;

    protected Animator animator;
    
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        SetAnimIDs();
        
        _currentHealth = maxHealth;
    }
    [ServerRpc]
    public virtual void DamageServerRpc(int damageAmount)
    {
        _currentHealth -= damageAmount;
        
        CheckHealth();
    }

    public virtual void Heal(int healAmount)
    {
        _currentHealth += healAmount;

        CheckHealth();
    }

    public void HealToMaxHealth()
    {
        _currentHealth = maxHealth;
    }

    public int HealthToMax()
    {
        return maxHealth - _currentHealth;
    }

    public virtual void IncreaseMaxHealth(int increaseAmount)
    {
        maxHealth += increaseAmount;
    }

    public virtual void ReduceMaxHealth(int reduceAmount)
    {
        maxHealth -= reduceAmount;
        
        CheckHealth();
    }
    
    public bool IsMaxHealth()
    {
        return _currentHealth == maxHealth;
    }

    private void CheckHealth()
    {
        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
            return;
        }

        if (_currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        animator.SetBool(_animIDDie, true);
    }
    
    protected virtual void SetAnimIDs()
    {
        _animIDDie = Animator.StringToHash("IsDead");
    }
}
