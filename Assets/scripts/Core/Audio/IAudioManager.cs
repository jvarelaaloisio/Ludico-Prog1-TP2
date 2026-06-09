namespace Core.Audio
{
    public interface IAudioManager
    {
        /// <returns>The player for this sound.</returns>
        /// <remarks>Can be null.</remarks>
        IAudioPlayer Play(Sound sound);
    }
}