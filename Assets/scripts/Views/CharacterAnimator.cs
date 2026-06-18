using Core.Combat;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;

namespace Views
{
    public class CharacterAnimator : MacacoBehaviour
    {
        [SerializeField] private Ref<ICharacter> character;
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private Animator animator;
        [Header("Animation ids")]
        [SerializeField] private AnimatorParameter isMovingParameter = new("isWalking");
        [SerializeField] private AnimatorParameter isAttackingParameter = new("Attack");
        [SerializeField] private AnimatorParameter velocityXParameter = new("directionX");
        [SerializeField] private AnimatorParameter velocityYParameter = new("directionY");
        [SerializeField] private float minVelocityToMove = 0.25f;
        private IStunnable _stunnable;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!character.HasValue)
                return;

            character.Value.OnPickUp += HandleWeaponPickedUp;
            character.Value.OnThrow += HandleWeaponThrown;
            if (character.Value is IStunnable stunnable)
                _stunnable = stunnable;
        }

        private void Update()
        {
            if (!character.HasValue
                || !rigidBody)
                return;

            if (_stunnable is null or {IsStunned: false})
            {
                Vector2 direction = character.Value.Direction;
                velocityXParameter.SetFloat(animator, direction.x);
                velocityYParameter.SetFloat(animator, direction.y);
            }

            bool isWalking = rigidBody.linearVelocity.magnitude > minVelocityToMove;
            isMovingParameter.SetBool(animator, isWalking);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (animator)
            {
                isMovingParameter.SetBool(animator, false);
                isAttackingParameter.SetBool(animator, false);
                velocityXParameter.SetFloat(animator, 0);
                velocityYParameter.SetFloat(animator, 0);
            }
        }

        private void HandleWeaponPickedUp(IWeapon weapon)
            => weapon.OnHoldingTrigger += HandleAttack;

        private void HandleWeaponThrown(IWeapon weapon)
            => weapon.OnHoldingTrigger -= HandleAttack;

        private void HandleAttack(Vector2 direction)
            => isAttackingParameter.SetTrigger(animator);
    }
}
