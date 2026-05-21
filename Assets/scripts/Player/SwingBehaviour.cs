using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using VarelaAloisio.Core;

namespace Player
{
    public class SwingBehaviour : MacacoBehaviour
    {
        [SerializeField] private InputActionReference input;
        [SerializeField] private Rigidbody2D rigidBody;

        private PlayerWeaponHandler weaponHandler;

        [Header("Configuración de Ataque")]
        [Tooltip("How much time does the axe swing take")]
        [SerializeField] private float attackDuration = 0.3f;
        private bool isAttacking = false;
        private Vector2 _lastDirection;

        protected override void Reset()
        {
            base.Reset();
            rigidBody = GetComponent<Rigidbody2D>();
        }

        protected override void Awake()
        {
            base.Awake();
            weaponHandler = GetComponentInChildren<PlayerWeaponHandler>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (input)
            {
                input.action.Enable();
                input.action.started += HandleAttackInput;
            }
            else
                LogError("Input is null");
        }

        private void FixedUpdate()
        {
            if (rigidBody
                && rigidBody.linearVelocity.magnitude > 0.1f)
                _lastDirection = rigidBody.linearVelocity;
        }

        private void OnDisable()
        {
            if (input)
            {
                input.action.Disable();
                input.action.started -= HandleAttackInput;
            }
        }

        private void HandleAttackInput(InputAction.CallbackContext obj)
        {
            if (!isAttacking
                && weaponHandler
                && weaponHandler.HasAxeEquipped())
                StartCoroutine(PerformAttack());
        
        }

        private IEnumerator PerformAttack()
        {
            isAttacking = true;

            // el handler acomoda el hacha
            weaponHandler.PrepareWeaponDirection(_lastDirection.x, _lastDirection.y);

            // muestra el hacha y activa el daño
            weaponHandler.ToggleAxeVisibility(true);
            weaponHandler.ToggleAxeDamage(true);

            // dispara animación de swing en el hacha
            Animator axeAnim = weaponHandler.GetAxeAnimator();
            if (axeAnim != null)
            {
                axeAnim.SetTrigger("TriggerSwing");
            }

            // espera a que termine el movimiento
            yield return new WaitForSeconds(attackDuration);

            // oculta el hacha y apaga el daño
            weaponHandler.ToggleAxeVisibility(false);
            weaponHandler.ToggleAxeDamage(false);

            isAttacking = false;
        }
    }
}