using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Text playerName;
    public string steamName;
    public ulong steamId;
    public ulong localId;
    public GameObject readyIndicator;
    public bool isReady;
    public bool isPlayerScreenFaded;
    public bool isInGame;

    private void Start()
    {
        readyIndicator.SetActive(false);
        playerName.text = steamName;
    }
}
