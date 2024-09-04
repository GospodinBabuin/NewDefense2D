using BuildingSystem;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSlots
{
    public class BuildingSlot : MonoBehaviour
    {
        [SerializeField] private Image imageField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private Text costField;
        [SerializeField] private MenuSlotsScriptableObject slotSO;

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
            GameUI.Instance.buildingSpawner.StartPlacement(slotSO.Prefab, slotSO.NeedToInvoke,slotSO.Cost);
            GameUI.Instance.OpenOrCloseBuildingMenu();
        }
    }
}
