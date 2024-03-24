using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSlots
{
    public class AltarSlot : MonoBehaviour
    {
        [SerializeField] private Image imageField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private Text costField;
        [SerializeField] private AltarSlotsScriptableObject slotSO;

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
            imageField.sprite = slotSO.Sprite;
            descriptionField.text = slotSO.Description;
            costField.text = slotSO.Cost.ToString();
        }

        public void SelectHealPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(slotSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            if (_player.Health.IsMaxHealth())
            {
                GameUI.Instance.Notifications.ShowCantAddMoreHealthNotification();
                return;
            }

            _player.Health.Heal(slotSO.HealCount);
            GoldBank.Instance.SpendGold(this, slotSO.Cost);
        }
        
        public void SelectBuffDamagePotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(slotSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }
            
            _player.Combat.IncreaseDamage(slotSO.IncreaseDamageCount);
            GoldBank.Instance.SpendGold(this, slotSO.Cost);
        }
        
        public void SelectBuffHealthPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(slotSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }
            
            _player.Health.IncreaseMaxHealth(slotSO.IncreaseHealthCount);
            GoldBank.Instance.SpendGold(this, slotSO.Cost);
        }
        
        public void SelectBuffSpeedPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(slotSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }
            
            _player.Locomotion.IncreaseSpeed(slotSO.IncreaseSpeedCount);
            GoldBank.Instance.SpendGold(this, slotSO.Cost);
        }
    }
}
