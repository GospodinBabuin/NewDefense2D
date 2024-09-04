using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Text playerName;
    public string steamName;
    public ulong steamId;
    public GameObject readyIndicator;
    public bool isReady;
    public bool isPlayerScreenFaded;

    private void Start()
    {
        readyIndicator.SetActive(false);
        playerName.text = steamName;
    }
}
