using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        private Slider _slider;
        private Text _healthValue;
        [SerializeField] private PlayerHealth playerHealth;
    
        private void Start()
        {
            _slider = GetComponent<Slider>();
            _healthValue = GetComponentInChildren<Text>();
        
            SetMaxHealth(playerHealth.MaxHealth);
            SetCurrentHealth(playerHealth.CurrentHealth);
            SetHealthCount(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        
            playerHealth.OnHealthValueChangedEvent += SetUIBarValues;
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
