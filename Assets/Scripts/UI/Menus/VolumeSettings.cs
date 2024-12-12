using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Menus
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] [Range(0, 1)] private float volumeFadeDuration = 0.25f;
        
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider environmentVolumeSlider;
        [SerializeField] private Slider SFXVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider UIVolumeSlider;

        private void Start()
        {
            masterVolumeSlider.value = MapVolumeValue(PlayerPrefs.GetFloat("MasterVolume"));
            environmentVolumeSlider.value = MapVolumeValue(PlayerPrefs.GetFloat("EnvironmentVolume"));
            SFXVolumeSlider.value = MapVolumeValue(PlayerPrefs.GetFloat("SFXVolume"));
            musicVolumeSlider.value = MapVolumeValue(PlayerPrefs.GetFloat("MusicVolume"));
            UIVolumeSlider.value = MapVolumeValue(PlayerPrefs.GetFloat("UIVolume"));
        }

        public async void ChangeMasterVolume(float targetVolume)
        {
            targetVolume = Mathf.Clamp(targetVolume, 0f, 10f);
            targetVolume = Mathf.Lerp(-80f, 0f, targetVolume / 10f);
            audioMixerGroup.audioMixer.GetFloat("MasterVolume", out float currentVolume);
            await SmoothVolumeChange(currentVolume, targetVolume, "MasterVolume");
        }
       
        public async void ChangeEnvironmentVolume(float targetVolume)
        {
            targetVolume = Mathf.Clamp(targetVolume, 0f, 10f);
            targetVolume = Mathf.Lerp(-80f, 0f, targetVolume / 10f);
            audioMixerGroup.audioMixer.GetFloat("EnvironmentVolume", out float currentVolume);
            await SmoothVolumeChange(currentVolume, targetVolume, "EnvironmentVolume");
        }
        
        public async void ChangeSFXVolume(float targetVolume)
        {
            targetVolume = Mathf.Clamp(targetVolume, 0f, 10f);
            targetVolume = Mathf.Lerp(-80f, 0f, targetVolume / 10f);
            audioMixerGroup.audioMixer.GetFloat("SFXVolume", out float currentVolume);
            await SmoothVolumeChange(currentVolume, targetVolume, "SFXVolume");
        }
        
        public async void ChangeMusicVolume(float targetVolume)
        {
            targetVolume = Mathf.Clamp(targetVolume, 0f, 10f);
            targetVolume = Mathf.Lerp(-80f, 0f, targetVolume / 10f);
            audioMixerGroup.audioMixer.GetFloat("MusicVolume", out float currentVolume);
            await SmoothVolumeChange(currentVolume, targetVolume, "MusicVolume");
        }
        
        public async void ChangeUIVolume(float targetVolume)
        {
            targetVolume = Mathf.Clamp(targetVolume, 0f, 10f);
            targetVolume = Mathf.Lerp(-80f, 0f, targetVolume / 10f);
            audioMixerGroup.audioMixer.GetFloat("UIVolume", out float currentVolume);
            await SmoothVolumeChange(currentVolume, targetVolume, "UIVolume");
        }
        
        private async Task SmoothVolumeChange(float startVolume, float targetVolume, string audioMixerName)
        {
            float elapsedTime = 0f;

            while (elapsedTime < volumeFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newDb = Mathf.Lerp(startVolume, targetVolume, elapsedTime / volumeFadeDuration);

                audioMixerGroup.audioMixer.SetFloat(audioMixerName, newDb);

                await Task.Yield();
            }

            audioMixerGroup.audioMixer.SetFloat(audioMixerName, targetVolume);
            PlayerPrefs.SetFloat(audioMixerName, targetVolume);
        }
        
        public float MapVolumeValue(float input)
        {
            Debug.Log(input);

            float minInput = -80f;
            float maxInput = 0f;

            float minOutput = 0f;
            float maxOutput = 10f;

            float result = (input - minInput) / (maxInput - minInput) * (maxOutput - minOutput) + minOutput;

            Debug.Log(result);
            return result;
        }
    }
}
