using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        private AudioSource _dayMusic;
        private AudioSource _nightMusic;
        private int dayMusicIndex = 0;
        private int nightMusicIndex = 0;

        [SerializeField] private List<AudioClip> daytimeMusic;
        [SerializeField] private List<AudioClip> nighttimeMusic;

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

            _dayMusic = GetComponent<AudioSource>();
            _nightMusic = GetComponent<AudioSource>();
        }

        void Start()
        {
            ChangeClip(ref dayMusicIndex, _dayMusic, daytimeMusic);
            StartCoroutine(CrossFade(_dayMusic, null));
        }

        public void PlayDaytimeMusic()
        {
            ChangeClip(ref dayMusicIndex, _dayMusic, daytimeMusic);
            StartCoroutine(CrossFade(_dayMusic, _nightMusic));
        }

        public void PlayNighttimeMusic()
        {
            ChangeClip(ref nightMusicIndex, _nightMusic, nighttimeMusic);
            StartCoroutine(CrossFade(_nightMusic, _dayMusic));
        }

        private IEnumerator CrossFade(AudioSource volumeUpAudioSource, AudioSource volumeDownAudioSource)
        {
            if (volumeUpAudioSource) volumeUpAudioSource.Play();

            for (float volume = 1f; volume >= 0; volume -= 0.1f)
            {
                if (volumeDownAudioSource) volumeDownAudioSource.volume = volume;
                if (volumeUpAudioSource) volumeUpAudioSource.volume = 1 - volume;
                yield return new WaitForSeconds(.1f);
            }

            if (volumeDownAudioSource) volumeDownAudioSource.Stop();
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

    }
}