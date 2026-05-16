namespace VarelaAloisio.Scenes
{
    public interface ILevelService
    {
        /// <summary>
        /// Handle Load level event being raised in the <see cref="sceneDataChannel"/>
        /// </summary>
        /// <param name="newLevel">The level to change to</param>
        void LoadLevel(ILevel newLevel);
    }
}