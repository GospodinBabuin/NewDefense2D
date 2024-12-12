using UnityEngine;

namespace AudioSystem
{
    public class SoundBuilder
    {
        private readonly SoundManager _soundManager;
        private SoundData _soundData;
        private Vector3 _position = Vector3.zero;
        private bool _randomPitch;
        private Transform _parent;

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

        public SoundBuilder WithParent(Transform parent)
        {
            _parent = parent;
            return this;
        }

        public void Play()
        {
            if (!_soundManager.CanPlaySound(_soundData)) return;

            SoundEmitter soundEmitter = _soundManager.GetSoundEmitter();
            soundEmitter.Initialize(_soundData);
            soundEmitter.transform.position = _position;
            
            if (_parent != null) soundEmitter.transform.SetParent(_parent);
            else soundEmitter.transform.SetParent(SoundManager.Instance.transform);

            if (_randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (_soundData.FrequentSound)
            {
                _soundManager.FrequentSoundEmitters.Enqueue(soundEmitter);
            }
            soundEmitter.Play();
        }
    }
}
