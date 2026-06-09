using System.Collections.Generic;
using Core.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(menuName = "Models/Sound", fileName = "sound", order = 0)]
    public class SoundContainer : ScriptableObject
    {
        //TODO: Checkout AudioRandomContainer. It's a Unity-native asset which seems to solve what this class is solving.
        public enum SequenceMode
        {
            Random,
            RandomNoImmediateRepeat,
            Sequential,
        }

        [SerializeField] private SequenceMode sequenceMode = SequenceMode.Random;
        [SerializeField] private List<AudioClip> clips;
        [SerializeField] private float volume = 1;
        [SerializeField] private float minPitch = 1;
        [SerializeField] private float maxPitch = 1;
        [SerializeField] private bool loop = false;
        [SerializeField] private float spatialBlend = 0;
        [SerializeField] private float minDistance = 1;
        [SerializeField] private float maxDistance = 500;
        [SerializeField] private AudioMixerGroup outputMixer;

        private int _lastClipIndex = -1;

        public static implicit operator Sound(SoundContainer container)
        {
            int clipIndex = container.sequenceMode switch
                            {
                                SequenceMode.Random => UnityEngine.Random.Range(0, container.clips.Count),
                                SequenceMode.RandomNoImmediateRepeat => GetNonRepeatingClipIndex(),
                                SequenceMode.Sequential => (int)Mathf.Repeat(container._lastClipIndex + 1, container.clips.Count),
                                _ => 0,
                            };

            container._lastClipIndex = clipIndex;

            return new Sound(container.clips[clipIndex],
                             container.volume,
                             UnityEngine.Random.Range(container.minPitch, container.maxPitch),
                             container.loop,
                             container.spatialBlend,
                             container.minDistance,
                             container.maxDistance,
                             container.outputMixer);

            int GetNonRepeatingClipIndex()
            {
                int selection;
                do
                    selection = UnityEngine.Random.Range(0, container.clips.Count);
                while (selection == container._lastClipIndex);

                return selection;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Play", true)]
        private bool PlayInEditor_Validate()
            => UnityEditor.EditorApplication.isPlaying;

        [ContextMenu("Play", false)]
        private void PlayInEditor()
        {
            if (VarelaAloisio.Core.Service.TryGet(out IAudioManager audioManager))
                audioManager.Play(this);
        }
#endif
    }
}