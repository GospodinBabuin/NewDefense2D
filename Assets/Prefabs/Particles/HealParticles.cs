using UnityEngine;

namespace Prefabs.Particles
{
    public class HealParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
    
        private void Start()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public void PlayHealParticles(int cycleCounts)
        {
            _particleSystem.emission.SetBurst(0, new ParticleSystem.Burst(0,1,1, cycleCounts,.2f));
            _particleSystem.Play();
        }
    }
}
