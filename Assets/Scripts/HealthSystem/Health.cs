using AudioSystem;
using Cainos.LucidEditor;
using Unity.Netcode;
using UnityEngine;

namespace HealthSystem
{
    public class Health : NetworkBehaviour
    {
        public int GetMaxHealth() => maxHealth.Value;
        public int GetCurrentHealth() => currentHealth.Value;

        [SerializeField] private NetworkVariable<int> maxHealth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] private NetworkVariable<int> currentHealth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [SerializeField] private SoundData soundDataHit;
        [SerializeField] private SoundData soundDataDeath;
        [SerializeField] private SoundData soundDataHealing;
        
        private int _animIDDie;
        protected Animator animator;
        
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();

            SetAnimIDs();

            currentHealth.Value = maxHealth.Value;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DamageServerRPC(int damageAmount)
        {
            Damage(damageAmount);
        }
        protected virtual void Damage(int damageAmount)
        {
            currentHealth.Value -= damageAmount;
            
            Debug.Log($"{gameObject.name}, now have {GetCurrentHealth()} health");

            if (currentHealth.Value > 0)
            {
                Debug.Log(transform.position);
                
                SoundManager.Instance.CreateSound()
                .WithSoundData(soundDataHit)
                .WithRandomPitch()
                .WithPosition(transform.position)
                .Play();
            }

            CheckHealth();
        }

        [ServerRpc(RequireOwnership = false)]
        public void HealServerRPC(int healAmount)
        {
            Heal(healAmount);
        }
        protected virtual void Heal(int healAmount)
        {
            currentHealth.Value += healAmount;
            PlayHealSound();
            
            CheckHealth();
        }

        
        [ServerRpc(RequireOwnership = false)]
        public void HealToMaxHealthServerRPC()
        {
            HealToMaxHealth();
        }
        protected void HealToMaxHealth()
        {
            currentHealth.Value = maxHealth.Value;
            PlayHealSound();
        }

        public int HealthToMaxValueRemained()
        {
            return maxHealth.Value - currentHealth.Value;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void IncreaseMaxHealthServerRPC(int increaseAmount)
        {
            IncreaseMaxHealth(increaseAmount);
        }
        protected virtual void IncreaseMaxHealth(int increaseAmount)
        {
            maxHealth.Value += increaseAmount;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReduceMaxHealthServerRPC(int reduceAmount)
        {
            ReduceMaxHealth(reduceAmount);
        }
        protected virtual void ReduceMaxHealth(int reduceAmount)
        {
            maxHealth.Value -= reduceAmount;
        
            CheckHealth();
        }
    
        public bool IsMaxHealth()
        {
            return currentHealth.Value == maxHealth.Value;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SetMaxHealthServerRPC(int newMaxHealth)
        {
            SetMaxHealth(newMaxHealth);
        }
        protected virtual void SetMaxHealth(int newMaxHealth)
        {
            maxHealth.Value = newMaxHealth;
            CheckHealth();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetCurrentHealthServerRPC(int newCurrentHealth)
        {
            SetCurrentHealth(newCurrentHealth);
        }
        protected virtual void SetCurrentHealth(int newCurrentHealth)
        {
            currentHealth.Value = newCurrentHealth;
            CheckHealth();
        }

        private void CheckHealth()
        {
            if (currentHealth.Value > maxHealth.Value)
            {
                currentHealth.Value = maxHealth.Value;
                return;
            }

            if (currentHealth.Value <= 0)
                Die();
        }

        public bool IsDead()
        {
            return currentHealth.Value <= 0;
        }
        
        private void PlayHealSound()
        {
            SoundManager.Instance.CreateSound()
                .WithSoundData(soundDataHealing)
                .WithRandomPitch()
                .WithPosition(transform.position)
                .Play();
        }

        [ContextMenu("Damage")]
        public void DebugDamage()
        {
            DamageServerRPC(1);
        }

        protected virtual void Die()
        {
            animator.SetBool(_animIDDie, true);

            SoundManager.Instance.CreateSound()
            .WithSoundData(soundDataDeath)
            .WithRandomPitch()
            .WithPosition(transform.position)
            .Play();
        }
    
        protected virtual void SetAnimIDs()
        {
            _animIDDie = Animator.StringToHash("IsDead");
        }
    }
}
