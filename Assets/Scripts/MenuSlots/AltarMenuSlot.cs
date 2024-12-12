using GoldSystem;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSlots
{
    public class AltarMenuSlot : MonoBehaviour
    {
        [SerializeField] private Image imageField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private Text costField;
        [SerializeField] private PotionScriptableObject potionSO;

        private PlayerController _player;

        private void Awake()
        {
            SetFieldsValues();
        }

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        private void SetFieldsValues()
        {
            imageField.sprite = potionSO.Sprite;
            descriptionField.text = potionSO.Description;
            costField.text = potionSO.Cost.ToString();
        }

        public void SelectHealPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            if (_player.Health.IsMaxHealth())
            {
                GameUI.Instance.Notifications.ShowCantAddMoreHealthNotification();
                return;
            }

            _player.Health.Heal(potionSO.HealCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);
        }

        public void SelectBuffDamagePotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            _player.Combat.IncreaseDamage(potionSO.IncreaseDamageCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);
        }

        public void SelectBuffHealthPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            _player.Health.IncreaseMaxHealth(potionSO.IncreaseHealthCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);
        }

        public void SelectBuffSpeedPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            _player.Locomotion.IncreaseSpeed(potionSO.IncreaseSpeedCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);
        }
    }
}
