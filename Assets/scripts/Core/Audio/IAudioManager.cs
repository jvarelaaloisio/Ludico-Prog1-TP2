namespace Core.Audio
{
    public interface IAudioManager
    {
        /// <returns>The player for this sound.</returns>
        /// <remarks>Can be null.</remarks>
        IAudioPlayer Play(Sound sound);

        /// <summary /> Sets the volume for the given audio channel
        /// <param name="channel">Correspondent to the <see cref="UnityEngine.Audio.AudioMixerGroup"/></param>
        /// <param name="value">The volume value to set</param>
        void SetVolume(VolumeChannel channel, float value);

        /// <summary /> Fetches the volume for the given audio channel
        /// <param name="channel">Correspondent to the <see cref="UnityEngine.Audio.AudioMixerGroup"/></param>
        /// <param name="volume">The volume for the channel</param>
        /// <returns>If the volume for the given channel was found</returns>
        bool TryGetVolume(VolumeChannel channel, out float volume);
    }
}