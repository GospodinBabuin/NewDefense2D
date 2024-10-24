using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData SoundData { get; private set; }

        private AudioSource _audioSource;
        private Coroutine _playingCoroutine;
        private float _defaultPitchValue = 1f;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }

            _audioSource.Play();
            _playingCoroutine = StartCoroutine(WaitForSecondsToEnd());
        }

        private IEnumerator WaitForSecondsToEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            SoundManager.Instance.ReturnToPool(this);
        }

        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }

        public void Initialize(SoundData soundData)
        {
            SoundData = soundData;
            _audioSource.clip = soundData.AudioClips[Random.Range(0, soundData.AudioClips.Length)];
            _audioSource.outputAudioMixerGroup = soundData.AudioMixerGroup;
            _audioSource.loop = soundData.Loop;
            _audioSource.playOnAwake = soundData.PlayOnAwake;
        }

        public void WithRandomPitch(float min = -0.1f, float max = 0.1f)
        {
            _audioSource.pitch = _defaultPitchValue + Random.Range(min, max);
        }
    }
}
