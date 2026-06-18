using System.Threading;
using Core.Combat;
using Core.Game;
using HealthSystem.Runtime.Components;
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
        [AutoMap(How.Service, When.Awake, OnError.Ignore)]
        private IGameManager _gameManager;
        private CancellationTokenSource _moveSource = null;
        [AutoMap(How.Service, When.OnEnable, OnError.Ignore)]
        private IFuryManager _furyManager;

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
                attackInput.action.started += HandleAttackInputStarted;
                attackInput.action.canceled += HandleAttackInputCanceled;
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

            character.Value.OnPickUp += HandlePickUp;
            character.Value.OnThrow += HandleThrow;
            if (!character.Value.gameObject.TryGetComponent(out HealthComponent health))
            {
                health = character.Value.gameObject.GetComponentInChildren<HealthComponent>();
                if (health is null)
                {
                    LogError("Character doesn't have a health component");
                    return;
                }
            }
            health.Setup();
            health.Health.OnDeath += HandleDeath;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (attackInput)
            {
                attackInput.action.Disable();
                attackInput.action.started -= HandleAttackInputStarted;
                attackInput.action.canceled -= HandleAttackInputCanceled;
            }
        }

    #region Fury weapon damage multiplication

        private void HandlePickUp(IWeapon weapon)
        {
            if (weapon is Component component)
                foreach (IDamagePointsSource damageSource in component.GetComponentsInChildren<IDamagePointsSource>(true))
                    damageSource.DamageMultiplier = MultiplyDamageByFury;
            else
                LogWarning($"Weapon {weapon?.name} is either null or has no damage source. Fury won't apply to the damage!");
        }

        private void HandleThrow(IWeapon weapon)
        {
            //TODO: These lines are commented because, when throwing a weapon, we need it to still have the fury multiplier. At least until it lands.
            // if (weapon is Component component)
            //     foreach (IDamagePointsSource damageSource in component.GetComponentsInChildren<IDamagePointsSource>())
            //         damageSource.DamageMultiplier = null;
        }

        private float MultiplyDamageByFury(float original)
            => original * (_furyManager?.Fury ?? 1);

    #endregion

        private void HandleDeath()
            => _gameManager?.HandlePlayerDeath();

    #region Input handling

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

        private void HandleAttackInputStarted(InputAction.CallbackContext _)
        {
            if (character.HasValue)
                character.Value.TryStartAttacking();
        }

        private void HandleAttackInputCanceled(InputAction.CallbackContext _)
        {
            if (character.HasValue)
                character.Value.StopAttacking();
        }

        private void HandleThrowInput(InputAction.CallbackContext _)
        {
            if (!character.HasValue)
                return;
            character.Value.TryThrowWeapon();
        }

    #endregion
    }
}