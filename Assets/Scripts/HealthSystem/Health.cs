using Unity.Netcode;
using UnityEngine;

namespace HealthSystem
{
    public class Health : NetworkBehaviour
    {
        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = value;
        }

        [SerializeField] private int maxHealth;
        private int _currentHealth;
    
        private int _animIDDie;

        protected Animator animator;
    
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();

            SetAnimIDs();
        
            _currentHealth = maxHealth;
        }
        
        public virtual void Damage(int damageAmount)
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
}
