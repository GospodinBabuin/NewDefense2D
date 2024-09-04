using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSlots
{
    public class UnitSlot : MonoBehaviour
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

        public void SelectUnit()
        {
            GameUI.Instance.UnitMenu.CurrentBarack.SpawnUnit(slotSO.Prefab, slotSO.Cost);
        }
    }
}
