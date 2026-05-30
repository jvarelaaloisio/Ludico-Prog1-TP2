using System;
using Core;
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
        [SerializeField] private AnimatorParameter velocityXParameter = new("directionX");
        [SerializeField] private AnimatorParameter velocityYParameter = new("directionY");
        [SerializeField] private float minVelocityToMove = 0.25f;

        private void Update()
        {
            if (!character.HasValue
                || !rigidBody)
                return;

            Vector2 direction = character.Value.Direction;
            velocityXParameter.SetFloat(animator, direction.x);
            velocityYParameter.SetFloat(animator, direction.y);

            bool isWalking = rigidBody.linearVelocity.magnitude > minVelocityToMove;
            isMovingParameter.SetBool(animator, isWalking);

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (animator)
            {
                isMovingParameter.SetBool(animator, false);
                velocityXParameter.SetFloat(animator, 0);
                velocityYParameter.SetFloat(animator, 0);
            }
        }
    }
}
