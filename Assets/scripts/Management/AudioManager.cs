using System;
using System.Collections.Generic;
using System.Linq;
using Core.Audio;
using Prefs;
using Prefs.Runtime;
using UnityEngine;
using UnityEngine.Audio;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Management
{
    [Service(typeof(IAudioManager))]
    [DisallowMultipleComponent]
    public class AudioManager : MacacoBehaviour, IAudioManager
    {
        [Serializable]
        private class ExposedParameterByChannel : ISerializationCallbackReceiver
        {
            private static readonly PlayerPrefsAdapter PlayerPrefsAdapter = new ();
            [SerializeField, HideInInspector] private string editorName;
            [field: SerializeField] public VolumeChannel Channel { get; private set; }
            [field: SerializeField] public string Name { get; private set; }

            [SerializeField] private string prefKey = "audio.xx";
            private FloatPref _pref;
            public override string ToString()
                => $"{Channel.ToString()} => {Name}";

            public void OnBeforeSerialize()
                => editorName = $"{Channel.ToString()} => {Name}";

            public void OnAfterDeserialize() { }

            public void LoadAndSetPref(AudioMixer mixer, Action<string> log)
            {
                _pref ??= new FloatPref(PlayerPrefsAdapter, prefKey, 0);

                log(_pref.TryLoad()
                        ? $"Successfully loaded parameter {Name} with value {_pref.value}."
                        : $"Parameter {Name} not found.");
                mixer.SetFloat(Name, _pref.value);
            }

            public void SavePref(float value)
            {
                _pref.value = value;
                _pref?.Save();
            }
        }
        [SerializeField] private List<Ref<IAudioPlayer>> players;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private List<ExposedParameterByChannel> parametersByChannel = new ();
        protected override void Awake()
        {
            base.Awake();
            foreach (ExposedParameterByChannel parameterByChannel in parametersByChannel)
            {
                parameterByChannel.LoadAndSetPref(mixer, Log);
            }
        }

        public IAudioPlayer Play(Sound sound)
        {
            IAudioPlayer player = players.FirstOrDefault(player => player.HasValue && player.Value.IsFree)
                                         ?.Value;
            if (player is null)
            {
                LogWarning($"No player free to play sound: {sound.clip?.name}");
                return null;
            }
            Log($"Playing {sound.clip?.name}");
            player.Play(sound);
            return player;
        }

        /// <inheritdoc/>
        public void SetVolume(VolumeChannel channel, float value)
        {
            ExposedParameterByChannel parameter = parametersByChannel.FirstOrDefault(parameter => parameter.Channel == channel);
            if (parameter is not null)
            {
                Log($"Setting parameter {parameter.Name} to {value} and saving pref.");
                mixer.SetFloat(parameter.Name, value);
                parameter.SavePref(value);
            }
            else
                LogError($"Parameter not found for channel {channel}");
        }

        /// <inheritdoc/>
        public bool TryGetVolume(VolumeChannel channel, out float volume)
        {
            ExposedParameterByChannel parameter = parametersByChannel.FirstOrDefault(parameter => parameter.Channel == channel);
            if (parameter is null)
            {
                LogError($"Parameter not found for channel {channel}");
                volume = -1f;
                return false;
            }
            if (!mixer)
            {
                LogError($"Mixer is null");
                volume = -1f;
                return false;
            }

            return mixer.GetFloat(parameter.Name, out volume);
        }
    }
}