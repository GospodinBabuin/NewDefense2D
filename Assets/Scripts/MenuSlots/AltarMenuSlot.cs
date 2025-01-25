using GoldSystem;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSlots
{
    public class AltarMenuSlot : NetworkBehaviour
    {
        [SerializeField] private Image imageField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private Text costField;
        [SerializeField] private PotionScriptableObject potionSO;

        [SerializeField] private PotionScriptableObject.PotionType potionType;

        private PlayerController _player;

        private void Awake()
        {
            SetFieldsValues();
        }

        private void Start()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.GetComponent<NetworkObject>().IsLocalPlayer)
                    _player = player.GetComponent<PlayerController>();
            }
        }

        private void SetFieldsValues()
        {
            imageField.sprite = potionSO.Sprite;
            imageField.gameObject.SetActive(false);
            Instantiate(potionSO.Prefab, imageField.transform.parent);
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

            _player.Health.HealServerRPC(potionSO.HealCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);
        }

        public void SelectBuffDamagePotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            _player.Combat.IncreaseDamageServerRPC(potionSO.IncreaseDamageCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);
            
            switch (potionSO.Type)
            {
                case PotionScriptableObject.PotionType.IncreaseDamageLvl1:
                    GameManager.Instance.IncreaseDamagePotionLvl1HasBeenUsed = true;
                    break;
                case PotionScriptableObject.PotionType.IncreaseDamageLvl2:
                    GameManager.Instance.IncreaseDamagePotionLvl2HasBeenUsed = true;
                    break;
            }
            
            ManageMenuSlots();
        }

        public void SelectBuffHealthPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            _player.Health.IncreaseMaxHealthServerRPC(potionSO.IncreaseHealthCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);

            switch (potionSO.Type)
            {
                case PotionScriptableObject.PotionType.IncreaseHealthLvl1:
                    GameManager.Instance.IncreaseHealthPotionLvl1HasBeenUsed = true;
                    break;
                case PotionScriptableObject.PotionType.IncreaseHealthLvl2:
                    GameManager.Instance.IncreaseHealthPotionLvl2HasBeenUsed = true;
                    break;
                case PotionScriptableObject.PotionType.IncreaseHealthLvl3:
                    GameManager.Instance.IncreaseHealthPotionLvl3HasBeenUsed = true;
                    break;
            }
            
            ManageMenuSlots();
        }

        public void SelectBuffSpeedPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            _player.Locomotion.IncreaseSpeedServerRPC(potionSO.IncreaseSpeedCount);
            GoldBank.Instance.SpendGold(this, potionSO.Cost);

            switch (potionSO.Type)
            {
                case PotionScriptableObject.PotionType.IncreaseSpeedLvl1:
                    GameManager.Instance.IncreaseSpeedPotionLvl1HasBeenUsed = true;
                    break;
                case PotionScriptableObject.PotionType.IncreaseSpeedLvl2:
                    GameManager.Instance.IncreaseSpeedPotionLvl2HasBeenUsed = true;
                    break;
            }
            
            ManageMenuSlots();
        }

        public void SelectRevivePlayerPotion()
        {
            if (!GoldBank.Instance.IsEnoughGold(potionSO.Cost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }
            NetworkManager.Singleton.id
            NetworkManager.Singleton.ConnectedClientsList[]
                
            
            ManageMenuSlots();
        }

        private void ManageMenuSlots() // rename
        {
            switch (potionType)
            {
                case PotionScriptableObject.PotionType.IncreaseSpeedLvl1:
                    if (GameManager.Instance.IncreaseSpeedPotionLvl1HasBeenUsed)
                        gameObject.SetActive(false);
                    break;
                case PotionScriptableObject.PotionType.IncreaseSpeedLvl2:
                    if (GameManager.Instance.IncreaseSpeedPotionLvl2HasBeenUsed)
                        gameObject.SetActive(false);
                    break;
                case PotionScriptableObject.PotionType.IncreaseDamageLvl1:
                    if (GameManager.Instance.IncreaseDamagePotionLvl1HasBeenUsed)
                        gameObject.SetActive(false);
                    break;
                case PotionScriptableObject.PotionType.IncreaseDamageLvl2:
                    if (GameManager.Instance.IncreaseDamagePotionLvl2HasBeenUsed)
                        gameObject.SetActive(false);
                    break;
                case PotionScriptableObject.PotionType.IncreaseHealthLvl1:
                    if (GameManager.Instance.IncreaseHealthPotionLvl1HasBeenUsed)
                        gameObject.SetActive(false);
                    break;
                case PotionScriptableObject.PotionType.IncreaseHealthLvl2:
                    if (GameManager.Instance.IncreaseHealthPotionLvl2HasBeenUsed)
                        gameObject.SetActive(false);
                    break;
                case PotionScriptableObject.PotionType.IncreaseHealthLvl3:
                    if (GameManager.Instance.IncreaseHealthPotionLvl3HasBeenUsed)
                        gameObject.SetActive(false);
                    break;
                case PotionScriptableObject.PotionType.RevivePlayer:
                    if (!DeadPlayersHandler.Instance.AreThereAnyDeadPlayers())
                        gameObject.SetActive(false);
                    break;
            }
        }

        private void OnEnable()
        {
            ManageMenuSlots();
        }
    }
}
