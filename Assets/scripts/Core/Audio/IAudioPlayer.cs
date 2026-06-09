namespace Core.Audio
{
    public interface IAudioPlayer
    {
        /// <summary /> If the player is free to be used.
        /// <remarks>Set to false when playing a new audio and to true when a non-looping audio finishes or when calling <see cref="Stop"/></remarks>
        bool IsFree { get; }

        /// <summary /> Plays the given sound, applying its configuration into the audio source.
        void Play(Sound sound);

        /// <summary /> Pause the audio clip
        /// <remarks>Doesn't free the player (see <see cref="AudioPlayer.IsFree"/>)</remarks>
        void Pause();

        /// <summary /> Resume playing the audio clip
        void Resume();

        /// <summary /> Stops the audio clip and frees the player (see <see cref="AudioPlayer.IsFree"/>)
        void Stop();
    }
}