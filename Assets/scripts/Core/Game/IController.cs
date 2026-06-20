namespace Core.Game
{
    public interface IController
    {
        ICharacter Character { get; }
        void Setup(ICharacter injectedCharacter);
    }
}