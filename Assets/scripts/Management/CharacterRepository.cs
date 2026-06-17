using System.Collections.Generic;
using Core.Game;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Management
{
    [Service(typeof(ICharacterRepository))]
    public class CharacterRepository : MacacoBehaviour, ICharacterRepository
    {
        private Dictionary<string, ICharacter> _characters;
        public void AddCharacter(ICharacter character, string id)
            => _characters[id] = character;

        public void RemoveCharacter(string id)
            => _characters.Remove(id);

        public bool TryGet(string id, out ICharacter character)
            => _characters.TryGetValue(id, out character);
    }
}