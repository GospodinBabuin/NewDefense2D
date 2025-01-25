using UnityEngine;

public class DeadPlayersHandler : MonoBehaviour
{
    public static DeadPlayersHandler Instance;
    public ulong deadPlayersID = ulong.MaxValue; 
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayerDead(ulong playerID)
    {
        deadPlayersID = playerID;
    }

    public void PlayerRespawned(ulong playerID)
    {
        deadPlayersID = ulong.MaxValue;
    }

    public bool AreThereAnyDeadPlayers()
    {
        return deadPlayersID != ulong.MaxValue;
    }
}
