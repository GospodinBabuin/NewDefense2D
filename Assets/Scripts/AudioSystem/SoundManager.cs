using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace AudioSystem
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        private IObjectPool<SoundEmitter> _soundEmitterPool;
        private readonly List<SoundEmitter> _activeSoundEmitters = new List<SoundEmitter>();

        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new Queue<SoundEmitter>();

        [SerializeField] private SoundEmitter soundEmitterPrefab;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxSoundInstances = 30;

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
            InitializePool();
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data)
        {
            if(data.AudioClips.Length == 0) return false;
            if (!data.FrequentSound) return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances &&
                FrequentSoundEmitters.TryDequeue(out SoundEmitter soundEmitter))
            {
                try
                {
                    soundEmitter.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter GetSoundEmitter()
        {
            return _soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            _soundEmitterPool.Release(soundEmitter);
        }

        private void InitializePool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize);
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitters.Add(soundEmitter);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitters.Remove(soundEmitter);
        }


        private SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }
    }
}