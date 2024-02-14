using Buildings;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth => maxHealth;
    public int CurrentHealth => _currentHealth;

    [SerializeField] private int maxHealth;
    private int _currentHealth;
    
    private int _animIDDie;
    private int _animIDTakeHit;

    private Animator _animator;
    private bool _haveAnimator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _haveAnimator = _animator;
        
        if (_haveAnimator) 
            SetAnimIDs();
        
        _currentHealth = maxHealth;
    }

    public virtual void Damage(int damageAmount, GameObject damager)
    {
        _currentHealth -= damageAmount;

        Debug.Log(damager);
        Debug.Log(_currentHealth);
        
        if (_haveAnimator)
            _animator.SetTrigger(_animIDTakeHit);

        if (TryGetComponent(out Entity entity))
        {
            entity.Combat.StopAllCoroutines();
            entity.Combat.StartCoroutine(entity.Combat.StartNextAttackCooldown(entity.Combat.AttackDelay / 4));
        }
        
        CheckHealth();
    }

    public virtual void Heal(int healAmount)
    {
        _currentHealth = healAmount;

        CheckHealth();
    }

    public virtual void IncreaseMaxHealth(int increaseAmount)
    {
        maxHealth = increaseAmount;
    }

    public virtual void ReduceMaxHealth(int reduceAmount)
    {
        maxHealth = reduceAmount;
        
        CheckHealth();
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

    private void Die()
    {
        if (_haveAnimator)
            _animator.SetBool(_animIDDie, true);
        
        if (TryGetComponent(out Entity entity))
        {
            Destroy(entity);
            return;
        }

        if (TryGetComponent(out Building building))
        {
            Destroy(building.gameObject);
            return;
        }
        
    }
    
    private void SetAnimIDs()
    {
        _animIDDie = Animator.StringToHash("IsDead");
        _animIDTakeHit = Animator.StringToHash("TakeDamage");
    }
}
