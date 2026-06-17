namespace Core.Game
{
    public interface ICharacterRepository
    {
        public void AddCharacter(ICharacter character, string id);
        public void RemoveCharacter(string id);
        bool TryGet(string id, out ICharacter character);
    }
}