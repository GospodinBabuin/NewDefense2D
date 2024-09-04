using System;
using HealthSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScreenCanvas
{
    public class HealthBar : NetworkBehaviour
    {
        public static HealthBar Instance;
        
        private Slider _slider;
        private Text _healthValue;
        private PlayerHealth _playerHealth;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            _slider = GetComponent<Slider>();
            _healthValue = GetComponentInChildren<Text>();
        }

        public void Initialize(PlayerHealth playerHealth)
        {
            _playerHealth = playerHealth;
            
            SetMaxHealth(_playerHealth.MaxHealth);
            SetCurrentHealth(_playerHealth.CurrentHealth);
            SetHealthCount(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        
            _playerHealth.OnHealthValueChangedEvent += SetUIBarValues;
        }

        private void SetUIBarValues(int currentHealth, int maxHealth)
        {
            SetMaxHealth(maxHealth);
            SetCurrentHealth(currentHealth);
            SetHealthCount(currentHealth, maxHealth);
        }

        private void SetMaxHealth(int maxHealth)
        {
            _slider.maxValue = maxHealth;
        }
        private void SetCurrentHealth(int currentHealth)
        {
            _slider.value = currentHealth;
        }

        
        private void SetHealthCount(int currentHealth, int maxHealth)
        {
            _healthValue.text = $"{currentHealth}/{maxHealth}";
        }
    }
}
