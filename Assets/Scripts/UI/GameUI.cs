using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }
    
    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private GameObject unitMenu;
    [SerializeField] private GameObject pauseMenu;
    
    public GameObject BuildingMenu => buildingMenu;
    public GameObject UnitMenu => unitMenu;
    public GameObject PauseMenu => pauseMenu;

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
}
