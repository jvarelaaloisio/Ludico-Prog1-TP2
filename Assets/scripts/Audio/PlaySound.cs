using Core.Audio;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Audio
{
    public class PlaySound : MacacoBehaviour
    {
        [SerializeField] private SoundContainer sound;
        [SerializeField] private bool avoidPlayingMultipleTimesInParallel;
        [AutoMap(How.Service, When.Awake)]
        private IAudioManager _audioManager;

        private IAudioPlayer _currentPlayer;

        [ContextMenu("Play")]
        public void Play()
        {
            if (_currentPlayer is { IsFree: false }
                && avoidPlayingMultipleTimesInParallel
                || !sound)
                return;
            Log($"Playing {sound.name}");
            _currentPlayer = _audioManager?.Play(sound);
        }

        public void Stop()
        {
            Log($"Stopping {sound?.name}");
            _currentPlayer?.Stop();
        }
    }
}