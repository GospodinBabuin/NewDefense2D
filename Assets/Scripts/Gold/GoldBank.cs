using System;
using UnityEngine;

public class GoldBank : MonoBehaviour
{
    public static GoldBank Instance { get; private set; }
    
    public delegate void BankHandler(object sender, int oldGoldValue, int newGoldValue);
    public event BankHandler OnGoldValueChangedEvent;
    
    public int Gold { get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        
        Destroy(gameObject);
    }

    private void Start()
    {
        AddGold(this, 100);
    }

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

    [ContextMenu("Add100Gold")]
    private void DebugAdd100Gold()
    {
        AddGold(this, 100);
    }
}
