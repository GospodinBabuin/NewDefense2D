using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class SoundPreferences : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        
        [SerializeField] private float masterVolume = -40;
        [SerializeField] private float environmentVolume = 0;
        [SerializeField] private float musicVolume = 0;
        [SerializeField] private float soundEffectsVolume = 0;
        [SerializeField] private float uiVolume = 0;
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
            
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("EnvironmentVolume", environmentVolume);
            PlayerPrefs.SetFloat("SFXVolume", soundEffectsVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("UIVolume", uiVolume);
        }
    }
}
