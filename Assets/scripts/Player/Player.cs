using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using VarelaAloisio.Core;

namespace Player
{
    public class Player : MacacoBehaviour
    {
        [SerializeField] private Ref<ICharacter> character;
        [SerializeField] private InputActionReference attackInput;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (attackInput)
            {
                attackInput.action.Enable();
                attackInput.action.performed += HandleAttackInput;
            }
        }

        private void OnDisable()
        {
            if (attackInput)
            {
                attackInput.action.Disable();
                attackInput.action.performed -= HandleAttackInput;
            }
        }

        private void HandleAttackInput(InputAction.CallbackContext _)
        {
            if (character.HasValue)
                character.Value.TryAttack();
        }
    }
}