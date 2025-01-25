using UnityEngine;

namespace MenuSlots
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Potion", fileName = "Potion")]
    public class PotionScriptableObject : ScriptableObject
    {
        public Sprite Sprite { get => sprite; private set => value = sprite; }
        public GameObject Prefab { get => prefab; private set => value = prefab; }
        public string Description { get => description; private set => value = description; }
        public int Cost { get => cost; private set => value = cost; }
        public byte HealCount { get => healCount; private set => value = healCount; }
        public byte IncreaseHealthCount { get => increaseHealthCount; private set => value = increaseHealthCount; }
        public byte IncreaseDamageCount { get => increaseDamageCount; private set => value = increaseDamageCount; }
        public float IncreaseSpeedCount { get => increaseSpeedCount; private set => value = increaseSpeedCount; }
        public PotionType Type { get => potionType; private set => value = potionType; }

        [SerializeField] private Sprite sprite;
        [SerializeField] private GameObject prefab;
        [SerializeField] private string description;
        [SerializeField] private int cost;
        [SerializeField] private byte healCount;
        [SerializeField] private byte increaseHealthCount;
        [SerializeField] private byte increaseDamageCount;
        [SerializeField] private float increaseSpeedCount;
        [SerializeField] private PotionType potionType;
        
        public enum PotionType
        {
            HealLvl1,
            HealLvl2,
            IncreaseHealthLvl1,
            IncreaseHealthLvl2,
            IncreaseHealthLvl3,
            IncreaseDamageLvl1,
            IncreaseDamageLvl2,
            IncreaseSpeedLvl1,
            IncreaseSpeedLvl2,
            RevivePlayer
        }
    }
}
