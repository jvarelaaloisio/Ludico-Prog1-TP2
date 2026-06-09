using System;
using System.Threading;
using Core.Audio;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class AudioPlayer : MacacoBehaviour, IAudioPlayer
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake)]
        private AudioSource _audioSource;

        /// <summary /> If the player is free to be used.
        /// <remarks>Set to false when playing a new audio and to true when a non-looping audio finishes or when calling <see cref="Stop"/></remarks>
        public bool IsFree { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            IsFree = true;
        }

        /// <summary /> Plays the given sound, applying its configuration into the audio source.
        public void Play(Sound sound)
        {
            _audioSource.clip = sound.clip;
            _audioSource.volume = sound.volume;
            _audioSource.pitch = sound.pitch;
            _audioSource.loop = sound.loop;
            _audioSource.spatialBlend = sound.spatialBlend;
            _audioSource.minDistance = sound.minDistance;
            _audioSource.maxDistance = sound.maxDistance;
            _audioSource.outputAudioMixerGroup = sound.outputMixer;
            _audioSource.enabled = true;
            _audioSource.Play();
            IsFree = false;
            if (!sound.loop)
                ResetIsFreeIn(sound.clip.length, DisableCancellationToken);
        }

        /// <summary /> Pause the audio clip
        /// <remarks>Doesn't free the player (see <see cref="IsFree"/>)</remarks>
        public void Pause()
        {
            Log($"Pausing {_audioSource.clip?.name}");
            _audioSource.Pause();
        }

        /// <summary /> Resume playing the audio clip
        public void Resume()
        {
            Log($"Resuming {_audioSource.clip?.name}");
            _audioSource.UnPause();
        }

        /// <summary /> Stops the audio clip and frees the player (see <see cref="IsFree"/>)
        public void Stop()
        {
            Log($"Stopping source: {_audioSource.clip?.name}");
            _audioSource.Stop();
            IsFree = true;
        }

        private async void ResetIsFreeIn(float clipLength, CancellationToken token)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(clipLength);
                if (token.IsCancellationRequested)
                    return;

                _audioSource.enabled = false;
                IsFree = true;
            }
            catch (Exception e) { LogException(e); }
        }
    }
}