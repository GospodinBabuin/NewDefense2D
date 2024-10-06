using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MenuSlots
{
    public class UnitMenuSlot : MonoBehaviour
    {
        [SerializeField] private Image imageField;
        [SerializeField] private Text descriptionField;
        [SerializeField] private Text costField;
        [SerializeField] private UnitScriptableObject unitSO;

        private void Awake()
        {
            SetFieldsValues();
        }

        private void SetFieldsValues()
        {
            imageField.sprite = unitSO.Sprite;
            descriptionField.text = unitSO.Description;
            costField.text = unitSO.Cost.ToString();
        }

        public void SelectUnit()
        {
            GameUI.Instance.UnitMenu.CurrentBarack.SpawnUnit(unitSO.ID, unitSO.Cost);
        }
    }
}
