using UnityEngine;

[CreateAssetMenu(menuName = "MenuSlots", fileName = "MenuSlot")]
public class MenuSlotsScriptableObject : ScriptableObject
{
    public Sprite Sprite { get => sprite; private set => value = sprite; }
    public string Description { get => description; private set => value = description; }
    public int Cost { get => cost; private set => value = cost; }
    public GameObject Prefab { get => prefab; private set => value = prefab; }
    public bool NeedToIvoke { get => needToIvoke; private set => value = needToIvoke; }

    [SerializeField] private Sprite sprite;
    [SerializeField] private string description;
    [SerializeField] private int cost;
    [SerializeField] private GameObject prefab;
    [SerializeField] private bool needToIvoke;
}
