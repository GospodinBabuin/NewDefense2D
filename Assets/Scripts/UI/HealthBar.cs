using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider _slider;
    [SerializeField] private Text healthValue;
    [SerializeField] private PlayerHealth _playerHealth;
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        healthValue = GetComponentInChildren<Text>();
        
        SetMaxHealth(_playerHealth.MaxHealth);
        SetHealth(_playerHealth.CurrentHealth);
        SetHealthCount(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        
        _playerHealth.OnHealthValueChangedEvent += OnPlayerHealthValueChangedEvent;
    }

    private void OnPlayerHealthValueChangedEvent(int currentHealth, int maxHealth)
    {
        SetMaxHealth(maxHealth);
        SetHealth(currentHealth);
        SetHealthCount(currentHealth, maxHealth);
    }

    private void SetMaxHealth(int health)
    {
        _slider.maxValue = health;
    }
    private void SetHealth(int health)
    {
        _slider.value = health;
    }

    private void SetHealthCount(int currentHealth, int maxHealth)
    {
        healthValue.text = $"{currentHealth}/{maxHealth}";
    }
}
