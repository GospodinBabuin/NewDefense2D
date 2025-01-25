using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class MainSettings : MonoBehaviour
    {
        [SerializeField] private Button vSyncButton;
        [SerializeField] private GameObject vSyncIndicator;
        
        [SerializeField] private Slider cameraSensitivitySlider;
        
        private bool _vSync = true;
        private float _cameraSensitivity = 3f;
        
        public delegate void CameraSensitivityHandler(float cameraSensitivity);
        public event CameraSensitivityHandler OnCameraSensitivityChangedEvent;
        
        private void Start()
        {
            _vSync = PlayerPrefs.GetInt("VSync") == 1;
            vSyncIndicator.SetActive(_vSync);
            Debug.Log("VSync: " + _vSync);

            _cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity");
            cameraSensitivitySlider.value = _cameraSensitivity;
        }
        
        public void OnVSyncButtonClicked()
        {
            _vSync = !_vSync;
            vSyncIndicator.SetActive(_vSync);

            QualitySettings.vSyncCount = _vSync ? 1 : 0;
            
            PlayerPrefs.SetInt("VSync", _vSync ? 1 : 0);
            Debug.Log("VSync: " + _vSync);
        }

        public void OnCameraSensitivityChanged(float value)
        {
            _cameraSensitivity = value;
            OnCameraSensitivityChangedEvent?.Invoke(_cameraSensitivity);
            PlayerPrefs.SetFloat("CameraSensitivity", _cameraSensitivity);
        }
    }
}