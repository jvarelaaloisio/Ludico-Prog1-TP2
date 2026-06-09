using System.Threading;
using Core;
using Core.Game;
using HealthSystem.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Core.Utils;

namespace Player
{
    public class Player : MacacoBehaviour
    {
        [SerializeField] private Ref<ICharacter> character;
        [SerializeField] private InputActionReference attackInput;
        [SerializeField] private InputActionReference throwInput;
        [SerializeField] private InputActionReference moveInput;
        [AutoMap(How.Service, When.Awake)]
        private IGameManager _gameManager;
        private CancellationTokenSource _moveSource = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!character.HasValue)
            {
                LogError("Character not set");
                return;
            }
            if (attackInput)
            {
                attackInput.action.Enable();
                attackInput.action.performed += HandleAttackInput;
            }

            if (throwInput)
            {
                throwInput.action.Enable();
                throwInput.action.performed += HandleThrowInput;
            }

            if (moveInput)
            {
                moveInput.action.Enable();
                moveInput.action.started += HandleMoveInput;
                moveInput.action.performed += HandleMoveInput;
                moveInput.action.canceled += HandleMoveInput;
            }

            if (character.Value.gameObject.TryGetComponent(out IHealthComponent health))
                health.Health.OnDeath += HandleDeath;
            else
                LogError("Character doesn't have a health component");
        }

        private void HandleDeath()
            => _gameManager.HandlePlayerDeath();

        private void HandleThrowInput(InputAction.CallbackContext data)
        {
            if (!character.HasValue)
                return;
            character.Value.TryThrowWeapon();
        }

        private void HandleMoveInput(InputAction.CallbackContext data)
        {
            if (!character.HasValue)
                return;
            var direction = data.ReadValue<Vector2>();
            if (direction.magnitude > 0)
            {
            #region Set Direction and start moving (if it's not moving already)

                character.Value.Direction = direction;
                if (_moveSource != null)
                    return;
                Log("Started moving");
                _moveSource = new CancellationTokenSource();
                character.Value.Move(LinkWithDisable(_moveSource.Token));

            #endregion
            }
            else
            {
            #region Stop moving

                Log("Stopped moving");
                TokenUtils.CancelAndDispose(ref _moveSource);

            #endregion
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (attackInput)
            {
                attackInput.action.Disable();
                attackInput.action.performed -= HandleAttackInput;
            }
        }

        private void HandleAttackInput(InputAction.CallbackContext _)
        {
            if (character.HasValue)
                character.Value.TryStartAttacking();
        }
    }
}