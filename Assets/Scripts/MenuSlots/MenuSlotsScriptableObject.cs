using UnityEngine;

namespace MenuSlots
{
    [CreateAssetMenu(menuName = "MenuSlots", fileName = "MenuSlot")]
    public class MenuSlotsScriptableObject : ScriptableObject
    {
        public Sprite Sprite { get => sprite; private set => value = sprite; }
        public string Description { get => description; private set => value = description; }
        public int Cost { get => cost; private set => value = cost; }
        public GameObject Prefab { get => prefab; private set => value = prefab; }
        public bool NeedToInvoke { get => needToInvoke; private set => value = needToInvoke; }

        [SerializeField] private Sprite sprite;
        [SerializeField] private string description;
        [SerializeField] private int cost;
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool needToInvoke;
    }
}
