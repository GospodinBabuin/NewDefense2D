using UnityEngine;

namespace MenuSlots
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Building", fileName = "Building")]
    public class BuildingScriptableObject : ScriptableObject
    {
        public Sprite Sprite { get => sprite; private set => value = sprite; }
        public string Description { get => description; private set => value = description; }
        public int Cost { get => cost; private set => value = cost; }
        public GameObject Prefab { get => prefab; private set => value = prefab; }
        public GameObject TempPrefab { get => tempPrefab; private set => value = tempPrefab; }
        public int ID { get => id; private set => value = id; }

        [SerializeField] private Sprite sprite;
        [SerializeField] private string description;
        [SerializeField] private int cost;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject tempPrefab;
        [SerializeField] private int id;
    }
}
