using BuildingSystem;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSlots
{
    public class BuildingMenuSlot : MonoBehaviour
    {
        [SerializeField] private Image imageField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private Text costField;
        [SerializeField] private BuildingScriptableObject slotSO;

        private void Awake()
        {
            SetFieldsValues();
        }

        private void SetFieldsValues()
        {
            imageField.sprite = slotSO.Sprite;
            descriptionField.text = slotSO.Description;
            costField.text = slotSO.Cost.ToString();
        }

        public void SelectBuilding()
        {
            BuildingSpawner.Instance.StartPlacement(slotSO.ID ,slotSO.Cost);
            GameUI.Instance.OpenOrCloseBuildingMenu();
        }
    }
}
