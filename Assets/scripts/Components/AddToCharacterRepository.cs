using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Components
{
    public class AddToCharacterRepository : MacacoBehaviour
    {
        [SerializeField] private string id;
        [AutoMap(How.GetComponent, When.Reset | When.Awake, OnError.Ignore)]
        [SerializeField] private Ref<ICharacter> character;

        [AutoMap(How.Service, When.OnEnable)]
        private ICharacterRepository _characterRepository;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = name;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!character.HasValue)
                LogError($"character is not set");
            _characterRepository?.AddCharacter(character.Value, id);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _characterRepository?.RemoveCharacter(id);
        }
    }
}