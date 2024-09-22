using Buildings;
using BuildingSystem;
using UnityEngine;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance { get; private set; }
    
        [SerializeField] private GameObject buildingMenu;
        [SerializeField] private UnitMenu unitMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private AltarMenu altarMenu;
        [SerializeField] private Notifications notifications;
    
        public UnitMenu UnitMenu => unitMenu;
        public Notifications Notifications => notifications;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
        
            Destroy(gameObject);
        }
    
        public bool IsMenuOpen()
        {
            return buildingMenu.activeInHierarchy || unitMenu.gameObject.activeInHierarchy || pauseMenu.activeInHierarchy;
        }

        public void OpenOrCloseBuildingMenu()
        {
            if (buildingMenu.activeInHierarchy)
            {
                buildingMenu.SetActive(false);
                PlayerResources.Instance.HideResources();
            }
            else
            {
                buildingMenu.SetActive(true);
                PlayerResources.Instance.ShowResources();
            }
        }

        public void CloseUnitMenu()
        {
            unitMenu.gameObject.SetActive(false);
            PlayerResources.Instance.HideResources();
        }

        public void OpenUnitMenu(int buildingLvl, Barracks currentBarack)
        {
            unitMenu.gameObject.SetActive(true);
            unitMenu.ShowMenu(buildingLvl, currentBarack);
            PlayerResources.Instance.ShowResources();
        }
        
        public void CloseAltarMenu()
        {
            altarMenu.gameObject.SetActive(false);
            PlayerResources.Instance.HideResources();
        }

        public void OpenAltarMenu(int buildingLvl, Altar currentAltar)
        {
            altarMenu.gameObject.SetActive(true);
            altarMenu.ShowMenu(buildingLvl, currentAltar);
            PlayerResources.Instance.ShowResources();
        }

        public void OnExitToMenuButtonClicked()
        {
            GameNetworkManager.Instance.Disconnected();
            SceneTransitionHandler.Instance.SwitchScene("MainMenu");
        }
        
        public void OnExitToDesktopButtonClicked()
        {
            Application.Quit();
        }
    }
}
