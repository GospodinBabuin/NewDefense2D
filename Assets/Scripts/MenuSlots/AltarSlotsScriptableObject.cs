using UnityEngine;

namespace MenuSlots
{
    [CreateAssetMenu(menuName = "AltarSlots", fileName = "AltarSlot")]
    public class AltarSlotsScriptableObject : MenuSlotsScriptableObject
    {
        public byte HealCount { get => healCount; private set => value = healCount; }
        public byte IncreaseHealthCount { get => increaseHealthCount; private set => value = increaseHealthCount; }
        public byte IncreaseDamageCount { get => increaseDamageCount; private set => value = increaseDamageCount; }
        public float IncreaseSpeedCount { get => increaseSpeedCount; private set => value = increaseSpeedCount; }

        [SerializeField] private byte healCount;
        [SerializeField] private byte increaseHealthCount;
        [SerializeField] private byte increaseDamageCount;
        [SerializeField] private float increaseSpeedCount;
    }
}
