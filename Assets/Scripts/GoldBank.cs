using UnityEngine;

public class GoldBank : MonoBehaviour
{
    public delegate void BankHandler(object sender, int oldGoldValue, int newGoldValue);
    public event BankHandler OnGoldValueChangedEvent;
    
    public int Gold { get; private set;}

    public void AddGold(object sender, int amount)
    {
        int oldGoldValue = Gold;
        Gold += amount;

        OnGoldValueChangedEvent?.Invoke(sender, oldGoldValue, Gold);
    }

    public void SpendGold(object sender, int amount)
    {
        int oldGoldValue = Gold;
        Gold -= amount;

        OnGoldValueChangedEvent?.Invoke(sender, oldGoldValue, Gold);
    }

    public bool IsEnoughGold(int amount)
    {
        return amount <= Gold;
    }
}
