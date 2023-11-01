using UnityEngine;
using UnityEngine.UI;

public class BuildingSlot : MonoBehaviour
{
    [SerializeField] private Image imageField;
    [SerializeField] private Text descriptionField;
    [SerializeField] private Text costField;
    [SerializeField] private MenuSlotsScriptableObject slotSO;

    [SerializeField] private GameObject buildingMenu;
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
        GameObject.FindWithTag("Player").GetComponentInChildren<BuildingSpawner>().StartPlacement(slotSO.Prefab);
        buildingMenu.SetActive(false);
    }
}
