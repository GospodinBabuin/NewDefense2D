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

            if (!IsDead())
            {
                SoundManager.Instance.CreateSound()
                .WithSoundData(soundDataHit)
                .WithRandomPitch()
                .WithPosition(transform.position)
                .Play();
            }

            CheckHealth();
        }

        public virtual void Heal(int healAmount)
        {
            currentHealth.Value += healAmount;

            CheckHealth();
        }

        public void HealToMaxHealth()
        {
            currentHealth.Value = maxHealth.Value;
        }

        public int HealthToMax()
        {
            return maxHealth.Value - currentHealth.Value;
        }

        public virtual void IncreaseMaxHealth(int increaseAmount)
        {
            maxHealth.Value += increaseAmount;
        }

        public virtual void ReduceMaxHealth(int reduceAmount)
        {
            maxHealth.Value -= reduceAmount;
        
            CheckHealth();
        }
    
        public bool IsMaxHealth()
        {
            return currentHealth.Value == maxHealth.Value;
        }

        public virtual void SetMaxHealth(int newMaxHealth)
        {
            maxHealth.Value = newMaxHealth;
            CheckHealth();
        }

        public virtual void SetCurrentHealth(int newCurrentHealth)
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

        private bool IsDead()
        {
            return currentHealth.Value <= 0;
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
