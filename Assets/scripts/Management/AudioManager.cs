using System.Collections.Generic;
using System.Linq;
using Core.Audio;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Management
{
    [Service(typeof(IAudioManager))]
    [DisallowMultipleComponent]
    public class AudioManager : MacacoBehaviour, IAudioManager
    {
        [SerializeField] private List<Ref<IAudioPlayer>> players;
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
    }
}