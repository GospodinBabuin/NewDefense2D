using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject continueGame;
        [SerializeField] private GameObject startNewGameConfirmation;

        private void Start()
        {
            if (!File.Exists(Application.persistentDataPath + "/GameSave.save"))
            {
                continueGame.SetActive(false);
            }
        }
        
        public void OnContinueGameButtonClicked()
        {
            SceneManager.LoadSceneAsync("Lobby");
        }
        
        public void OnStartNewGameButtonClicked()
        {
            if (!continueGame.activeInHierarchy)
            {
                OnStartNewGameConfirmationButtonClicked();
            }
            else
            {
                startNewGameConfirmation.SetActive(true);
            }
        }
        
        public void OnStartNewGameConfirmationButtonClicked()
        {
            if (File.Exists(Application.persistentDataPath + "/GameSave.save"))
            {
                File.Delete(Application.persistentDataPath + "/GameSave.save");
            }

            SceneTransitionHandler.Instance.SwitchScene("Lobby");
        }

        public void OnExitButtonClicked()
        {
            Application.Quit();
        }
    }
}
