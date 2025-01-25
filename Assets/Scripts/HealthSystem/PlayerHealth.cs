using Prefabs.Particles;
using UI.ScreenCanvas;
using Unity.Netcode;

namespace HealthSystem
{
    public class PlayerHealth : EntityHealth
    {
        public delegate void HealthHandler(int currentHealth, int maxHealth);
        public event HealthHandler OnHealthValueChangedEvent;
        private HealParticles _healParticles;
        
        public bool IsDead { get; private set; }

        private void Start()
        {
            if (IsOwner)
            {
                if (HealthBar.Instance != null)
                    HealthBar.Instance.Initialize(this);
            }
            _healParticles = GetComponentInChildren<HealParticles>();
        }

        protected override void Damage(int damageAmount)
        {
            base.Damage(damageAmount);
            
            OnHealthValueChangedEvent?.Invoke(GetCurrentHealth(), GetMaxHealth());
        }

        protected override void Heal(int healAmount)
        {
            base.Heal(healAmount);
        
            _healParticles.PlayHealParticles(healAmount);
        
            OnHealthValueChangedEvent?.Invoke(GetCurrentHealth(), GetMaxHealth());
        }

        protected override void IncreaseMaxHealth(int increaseAmount)
        {
            base.IncreaseMaxHealth(increaseAmount);
        
            OnHealthValueChangedEvent?.Invoke(GetCurrentHealth(), GetMaxHealth());
        }

        protected override void ReduceMaxHealth(int reduceAmount)
        {
            base.ReduceMaxHealth(reduceAmount);
        
            OnHealthValueChangedEvent?.Invoke(GetCurrentHealth(), GetMaxHealth());
        }

        protected override void SetMaxHealth(int newMaxHealth)
        {
            base.SetMaxHealth(newMaxHealth);
            
            OnHealthValueChangedEvent?.Invoke(GetCurrentHealth(), GetMaxHealth());
        }

        protected override void SetCurrentHealth(int newCurrentHealth)
        {
            base.SetCurrentHealth(newCurrentHealth);
            
            OnHealthValueChangedEvent?.Invoke(GetCurrentHealth(), GetMaxHealth());
        }

        protected override void Die()
        {
            base.Die();

            GetComponent<PlayerController>().Die();
        }
    }
}
