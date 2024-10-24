using UnityEngine;

namespace AudioSystem
{
    public class SoundBuilder : MonoBehaviour
    {
        readonly private SoundManager _soundManager;
        private SoundData _soundData;
        private Vector3 _position = Vector3.zero;
        private bool _randomPitch;

        public SoundBuilder(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        public SoundBuilder WithSoundData(SoundData soundData)
        {
            _soundData = soundData;
            return this;
        }

        public SoundBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch()
        {
            _randomPitch = true;
            return this;
        }

        public void Play()
        {
            if (!_soundManager.CanPlaySound(_soundData)) return;

            SoundEmitter soundEmitter = _soundManager.GetSoundEmitter();
            soundEmitter.Initialize(_soundData);
            soundEmitter.transform.position = _position;
            soundEmitter.transform.parent = SoundManager.Instance.transform;

            if (_randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (_soundData.FrequentSound)
            {
                _soundManager.FrequentSoundEmmiters.Enqueue(soundEmitter);
            }
            soundEmitter.Play();
        }
    }
}
