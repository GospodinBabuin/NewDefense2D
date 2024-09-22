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

        private void Start()
        {
            if (IsOwner)
            {
                HealthBar.Instance?.Initialize(this);
            }
            _healParticles = GetComponentInChildren<HealParticles>();
        }

        public override void Damage(int damageAmount)
        {
            base.Damage(damageAmount);
        
            OnHealthValueChangedEvent?.Invoke(CurrentHealth, MaxHealth);
        }

        public override void Heal(int healAmount)
        {
            base.Heal(healAmount);
        
            _healParticles.PlayHealParticles(healAmount);
        
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

        public override void SetMaxHealth(int newMaxHealth)
        {
            base.SetMaxHealth(newMaxHealth);
            
            OnHealthValueChangedEvent?.Invoke(CurrentHealth, MaxHealth);
        }

        public override void SetCurrentHealth(int newCurrentHealth)
        {
            base.SetCurrentHealth(newCurrentHealth);
            
            OnHealthValueChangedEvent?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}
