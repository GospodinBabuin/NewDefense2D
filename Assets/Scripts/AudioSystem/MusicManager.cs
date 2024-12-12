using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        private int _dayMusicIndex = 0;
        private int _nightMusicIndex = 0;
        private int _mainMenuMusicIndex = 0;
        private int _lobbyMusicIndex = 0;
        
        [SerializeField] private List<AudioClip> daytimeMusic;
        [SerializeField] private List<AudioClip> nighttimeMusic;

        [SerializeField] private AudioSource dayMusicAudioSource;
        [SerializeField] private AudioSource nightMusicAudioSource;
        [SerializeField] private AudioSource mainMenuMusicAudioSource;
        [SerializeField] private AudioSource lobbyMusicAudioSource;
        
        private readonly List<AudioSource> _audioSources = new List<AudioSource>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SceneTransitionHandler.Instance.OnFadeOutStartedEvent += ChangeMusicInNewScene;
            SubscribeToDayManager();
            
            FillAudioSourcesList();
        }

        private void SubscribeToDayManager()
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnDayStateChangedEvent += ChangeMusicByDayState;
                DayManager.OnDayManagerCreated -= SubscribeToDayManager;
            }
            else
            {
                DayManager.OnDayManagerCreated += SubscribeToDayManager;
            }
        }

        private void ChangeMusicByDayState(DayManager.DayState dayState, int currentDay)
        {
            switch (dayState)
            {
                case DayManager.DayState.Night:
                    PlayNighttimeMusic();
                    break;
                case DayManager.DayState.Day:
                    PlayDaytimeMusic();
                    break;
            }
        }

        private void ChangeMusicInNewScene(string sceneName)
        {
            switch (sceneName)
            {
                case "Lobby":
                    PlayLobbyMusic();
                    break;
                case "Game":
                    PlayDaytimeMusic();
                    break;
                case "MainMenu":
                    if (!mainMenuMusicAudioSource.isPlaying)
                        PlayMainMenuMusic();
                    break;
            }
        }

        private void PlayDaytimeMusic()
        {
            if (dayMusicAudioSource.isPlaying) return;
            ChangeClip(ref _dayMusicIndex, dayMusicAudioSource, daytimeMusic);
            StartCoroutine(CrossFade(dayMusicAudioSource));
        }

        private void PlayNighttimeMusic()
        {
            if (nightMusicAudioSource.isPlaying) return;
            ChangeClip(ref _nightMusicIndex, nightMusicAudioSource, nighttimeMusic);
            StartCoroutine(CrossFade(nightMusicAudioSource));
        }

        private void PlayMainMenuMusic()
        {
            if (mainMenuMusicAudioSource.isPlaying) return;
            StartCoroutine(CrossFade(mainMenuMusicAudioSource));
        }

        private void PlayLobbyMusic()
        {
            if (lobbyMusicAudioSource.isPlaying) return;
            StartCoroutine(CrossFade(lobbyMusicAudioSource));
        }
        
        private IEnumerator CrossFade(AudioSource volumeUpAudioSource)
        {
            if (volumeUpAudioSource) volumeUpAudioSource.Play();

            for (float volume = 1f; volume >= 0; volume -= 0.1f)
            {
                foreach (AudioSource audioSource in _audioSources)
                {
                    if (!audioSource.isPlaying) continue;
                    if (volumeUpAudioSource && audioSource == volumeUpAudioSource) continue;
                    audioSource.volume = volume;
                }

                if (volumeUpAudioSource) volumeUpAudioSource.volume = 1 - volume;
                yield return new WaitForSeconds(.1f);
            }
            
            foreach (AudioSource audioSource in _audioSources)
            {
                if (!audioSource.isPlaying) continue;
                if (volumeUpAudioSource && audioSource == volumeUpAudioSource) continue;
                audioSource.Stop();
            }
        }

        private void ChangeClip(ref int index, AudioSource audioSource, IReadOnlyList<AudioClip> music)
        {
            if (index < music.Count)
            {
                audioSource.volume = 0;
                audioSource.clip = music[index];
                index++;
            }
            else
            {
                index = 0;
                audioSource.volume = 0;
                audioSource.clip = music[index];
            }
        }

        private void FillAudioSourcesList()
        {
            _audioSources.Add(dayMusicAudioSource);
            _audioSources.Add(nightMusicAudioSource);
            _audioSources.Add(mainMenuMusicAudioSource);
            _audioSources.Add(lobbyMusicAudioSource);
        }

        private void OnDestroy()
        {
            SceneTransitionHandler.Instance.OnFadeOutStartedEvent -= ChangeMusicInNewScene;
        }
    }
}