using UnityEngine;

namespace UI.Menus
{
    public class SettingsMenu : MonoBehaviour
    {
        public SoundSettings SoundSettings { get; private set; }
        public MainSettings MainSettings { get; private set; }

        public void Initialize()
        {
            MainSettings = GetComponentInChildren<MainSettings>();
            SoundSettings = GetComponentInChildren<SoundSettings>();
            Debug.Log("Initialized");
        }
    }
}
