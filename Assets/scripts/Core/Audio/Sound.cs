using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
    /// <summary /> An audio configuration to give to an <see cref="IAudioManager"/>
    /// <remarks>Can be an sfx or music</remarks>
    public readonly struct Sound
    {
        public readonly AudioClip clip;
        public readonly float volume;
        public readonly float pitch;
        public readonly bool loop;
        public readonly float spatialBlend;
        public readonly float minDistance;
        public readonly float maxDistance;
        public readonly AudioMixerGroup outputMixer;

        public Sound(AudioClip clip,
                     float volume,
                     float pitch,
                     bool loop,
                     float spatialBlend,
                     float minDistance,
                     float maxDistance,
                     AudioMixerGroup outputMixer)
        {
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
            this.spatialBlend = spatialBlend;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.outputMixer = outputMixer;
        }
    }
}