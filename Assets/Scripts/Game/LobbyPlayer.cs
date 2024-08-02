using Game.Data;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private TextMeshProUGUI readyState;

        private LobbyPlayerData _data;
        
        public void SetData(LobbyPlayerData data)
        {
            _data = data;
            playerName.text = _data.GamerTag;

            if (data.IsReady)
            {
                readyState.text = "Ready";
                readyState.color = Color.green;
            }
            
            gameObject.SetActive(true);
        }
    }
}