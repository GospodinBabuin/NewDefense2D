using System;
using Game.Data;
using Game.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Text lobbyCodeText;
        [SerializeField] private Image mapImage;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Text mapName;
        [SerializeField] private MapSelectionData mapSelectionData;

        private int _currentMapIndex = 0;
        

        private void OnEnable()
        {
            if (GameLobbyManager.Instance.IsHost)
            {
                leftButton.onClick.AddListener(OnLeftButtonClicked);
                rightButton.onClick.AddListener(OnRightButtonClicked);
                startButton.onClick.AddListener(OnStartButtonClicked);
                LobbyEvents.OnLobbyReady += OnLobbyReady;
            }
            
            readyButton.onClick.AddListener(OnReadyPressed);

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveAllListeners();
            rightButton.onClick.RemoveAllListeners();
            readyButton.onClick.RemoveAllListeners();
            startButton.onClick.RemoveAllListeners();
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            LobbyEvents.OnLobbyReady -= OnLobbyReady;

        }

        private async void OnLeftButtonClicked()
        {
            if (_currentMapIndex - 1 > 0)
            {
                _currentMapIndex--;
            }
            else
            {
                _currentMapIndex = 0;
            }

            UpdateMap();
            GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, mapSelectionData.Maps[_currentMapIndex].SceneName);
        }

        private async void OnRightButtonClicked()
        {
            if (_currentMapIndex + 1 < mapSelectionData.Maps.Count - 1)
            {
                _currentMapIndex++;
            }
            else
            {
                _currentMapIndex = mapSelectionData.Maps.Count - 1;
            }

            UpdateMap();
            GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, mapSelectionData.Maps[_currentMapIndex].SceneName);
        }

        private async void OnReadyPressed()
        {
            bool succeed = await GameLobbyManager.Instance.SetPlayerReady();
            if (succeed)
            {
                readyButton.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            lobbyCodeText.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";

            if (!GameLobbyManager.Instance.IsHost)
            {
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
            }
            else
            {
                GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, mapSelectionData.Maps[_currentMapIndex].SceneName);
            }
        }

        private void UpdateMap()
        {
            mapImage.sprite = mapSelectionData.Maps[_currentMapIndex].MapSprite;
            mapName.text = mapSelectionData.Maps[_currentMapIndex].MapName;
        }
        
        private void OnLobbyUpdated()
        {
            _currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            UpdateMap();
        }
        
        private void OnLobbyReady()
        {
            startButton.gameObject.SetActive(true);
        }

        private async void OnStartButtonClicked()
        {
            await GameLobbyManager.Instance.StartGame();
        }
    }
}