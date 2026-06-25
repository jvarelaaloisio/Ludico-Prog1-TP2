using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Combat;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;

namespace Combat
{
    public class Shotgun : MacacoBehaviour, IWeapon
    {
        [SerializeField] private Collider2D pickUpTrigger;
        [Tooltip("The collider for when the weapon is on the ground")]
        [SerializeField] private Collider2D groundCollider;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private Ref<IThrower> thrower;

        [Space]
        [SerializeField] private float cooldown = .25f;
        [SerializeField] private float  stunDuration = .5f;
        [SerializeField] private Vector3 pickUpOffset = new (-0.45f, -0.45f, 0f);

        private CancellationTokenSource _attackTokenSource;
        private bool _isHoldingTrigger;
        /// <inheritdoc />
        public event Action<Vector2> OnHoldingTrigger;

        /// <inheritdoc />
        public event Action<Vector2> OnReleasedTrigger;

        /// <inheritdoc />
        public event Action<Vector2> OnThrow;

        public bool IsOnCooldown { get; private set; }

        /// <inheritdoc />
        public Task HoldTrigger(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ReleaseTrigger()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SetOwner(ICharacter newOwner)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Throw(Vector2 direction)
        {
            throw new NotImplementedException();
        }
    }
}