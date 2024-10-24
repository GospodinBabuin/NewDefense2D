using System;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    [Serializable]
    public class SoundData
    {
        public AudioClip[] AudioClips;
        public AudioMixerGroup AudioMixerGroup;
        public bool Loop;
        public bool PlayOnAwake;
        public bool FrequentSound;
    }
}

