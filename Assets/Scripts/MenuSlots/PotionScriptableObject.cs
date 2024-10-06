using UnityEngine;

namespace MenuSlots
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Potion", fileName = "Potion")]
    public class PotionScriptableObject : ScriptableObject
    {
        public Sprite Sprite { get => sprite; private set => value = sprite; }
        public string Description { get => description; private set => value = description; }
        public int Cost { get => cost; private set => value = cost; }
        public byte HealCount { get => healCount; private set => value = healCount; }
        public byte IncreaseHealthCount { get => increaseHealthCount; private set => value = increaseHealthCount; }
        public byte IncreaseDamageCount { get => increaseDamageCount; private set => value = increaseDamageCount; }
        public float IncreaseSpeedCount { get => increaseSpeedCount; private set => value = increaseSpeedCount; }


        [SerializeField] private Sprite sprite;
        [SerializeField] private string description;
        [SerializeField] private int cost;
        [SerializeField] private byte healCount;
        [SerializeField] private byte increaseHealthCount;
        [SerializeField] private byte increaseDamageCount;
        [SerializeField] private float increaseSpeedCount;
    }
}
