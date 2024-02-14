using UnityEngine;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance { get; private set; }
    
        [SerializeField] private GameObject buildingMenu;
        [SerializeField] private GameObject unitMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Notifications notifications;
    
        public GameObject BuildingMenu => buildingMenu;
        public GameObject UnitMenu => unitMenu;
        public GameObject PauseMenu => pauseMenu;
        public Notifications Notifications => notifications;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                return;
            }
        
            Destroy(gameObject);
        }
    
        public bool IsMenuOpen()
        {
            return buildingMenu.activeInHierarchy || unitMenu.activeInHierarchy || pauseMenu.activeInHierarchy;
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
            unitMenu.SetActive(false);
            PlayerResources.Instance.HideResources();
        }

        public void OpenUnitMenu()
        {
            unitMenu.SetActive(true);
            PlayerResources.Instance.ShowResources();
        }
    }
}
