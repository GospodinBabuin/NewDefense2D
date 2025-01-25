using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class SettingsPreferences : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        
        [SerializeField] [Range(-80, 20)] private float masterVolume = -40;
        [SerializeField] [Range(-80, 20)] private float environmentVolume = 0;
        [SerializeField] [Range(-80, 20)] private float musicVolume = 0;
        [SerializeField] [Range(-80, 20)] private float soundEffectsVolume = 0;
        [SerializeField] [Range(-80, 20)] private float uiVolume = 0;

        [SerializeField] [Range(0, 1)] private int vSync = 1;
        
        [SerializeField] [Range(1, 5)] private float cameraSensitivity = 3f;
        private void Start()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", masterVolume);
            environmentVolume = PlayerPrefs.GetFloat("EnvironmentVolume", environmentVolume);
            soundEffectsVolume = PlayerPrefs.GetFloat("SFXVolume", soundEffectsVolume);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
            uiVolume = PlayerPrefs.GetFloat("UIVolume", uiVolume);
            
            audioMixerGroup.audioMixer.SetFloat("MasterVolume", masterVolume);
            audioMixerGroup.audioMixer.SetFloat("EnvironmentVolume", environmentVolume);
            audioMixerGroup.audioMixer.SetFloat("SFXVolume", soundEffectsVolume);
            audioMixerGroup.audioMixer.SetFloat("MusicVolume", musicVolume);
            audioMixerGroup.audioMixer.SetFloat("UIVolume", uiVolume);
            
            vSync = PlayerPrefs.GetInt("VSync", vSync);
            QualitySettings.vSyncCount = vSync;
            
            cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", cameraSensitivity);
            
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("EnvironmentVolume", environmentVolume);
            PlayerPrefs.SetFloat("SFXVolume", soundEffectsVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("UIVolume", uiVolume);
            
            PlayerPrefs.SetInt("VSync", vSync);
            
            PlayerPrefs.SetFloat("CameraSensitivity", cameraSensitivity);
        }
    }
}
