using System;
using System.Threading;
using Core.Combat;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Extensions;
using VarelaAloisio.Core.Utils;

namespace Characters
{
    public class Character : MacacoBehaviour, ICharacter, IStunnable
    {
        [SerializeField] private Ref<IWeapon> currentWeapon;
        [SerializeField] private Ref<IKnockbackHandler> knockbackHandler;
        [Header("Movement")]
        [SerializeField] private float goalSpeed = 30;
        [SerializeField] private float acceleration = 60;
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private float brakeMultiplier = .85f;

        private CancellationTokenSource _attackSource;
        private bool _isStunned;
        public event Action<IWeapon> OnPickUp;
        public event Action<IWeapon> OnThrow;
        public Vector2 Direction { get; set; } = Vector2.down;
        public IWeapon CurrentWeapon => currentWeapon.HasValue ? currentWeapon.Value : null;
        public bool HasWeapon => currentWeapon.HasValue;

        protected override void Reset()
        {
            base.Reset();
            rigidBody = GetComponent<Rigidbody2D>();
            if (!rigidBody)
                rigidBody = gameObject.AddComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0;
            rigidBody.linearDamping = 6;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _isStunned = false;
        }

        public void PickUp(IWeapon weapon)
        {
            if (currentWeapon.HasValue)
                return;
            weapon.SetOwner(this);
            currentWeapon.Value = weapon;
            OnPickUp?.Invoke(weapon);
        }

        public bool TryStartAttacking()
        {
            if (_isStunned
                || !currentWeapon.HasValue
                || currentWeapon.Value.IsOnCooldown)
                return false;
            _attackSource?.Cancel();
            _attackSource?.Dispose();
            _attackSource = new();
            currentWeapon.Value.HoldTrigger(LinkWithDisable(_attackSource.Token));
            return true;
        }

        public void StopAttacking()
        {
            TokenUtils.CancelAndDispose(ref _attackSource);
            if (HasWeapon)
                CurrentWeapon.ReleaseTrigger();
        }

        public bool TryThrowWeapon()
        {
            if (_isStunned
                || !HasWeapon)
                return false;

            TokenUtils.CancelAndDispose(ref _attackSource);
            CurrentWeapon.Throw(Direction);
            OnThrow?.Invoke(currentWeapon.Value);
            currentWeapon.Value = null;
            return true;
        }

        public async void Move(CancellationToken token)
        {
            try
            {
                CancellationTokenRegistration registration = token.Register(Brake);
                while (!token.IsCancellationRequested)
                {
                    if (!_isStunned)
                    {
                        Vector2 currentVelocity = rigidBody.linearVelocity;
                        if (Vector2.Angle(currentVelocity, Direction) > 45)
                            ApplyBrakeForce();
                        float currentSpeed = currentVelocity.magnitude;
                        float speedDifferential = goalSpeed - currentSpeed;
                        rigidBody.AddForce(Direction * speedDifferential, ForceMode2D.Force);
                    }
                    await Awaitable.FixedUpdateAsync();
                }
                await registration.DisposeAsync();
            }
            catch (Exception e) { LogException(e); }
        }

        /// <summary /> Stun the character, disabling its movement and attack and adding a knockback.
        /// <param name="duration">How much time to disable character actions</param>
        /// <param name="direction"></param>
        public async void Stun(float duration, Vector2 direction)
        {
            try
            {
                if (knockbackHandler.HasValue)
                    knockbackHandler.Value.Handle(direction, 1);
                _isStunned = true;
                await Awaitable.WaitForSecondsAsync(duration);
                _isStunned = false;
            }
            catch (Exception e) { LogException(e); }
        }

        private async void Brake()
        {
            try
            {
                if (DisableCancellationToken.IsCancellationRequested)
                    return;
                await Awaitable.FixedUpdateAsync();
                if (DisableCancellationToken.IsCancellationRequested)
                    return;
                ApplyBrakeForce();
            }
            catch (Exception e) { LogException(e); }
        }

        private void ApplyBrakeForce()
        {
            Log("Braking");
            rigidBody.AddForce(-rigidBody.linearVelocity * brakeMultiplier, ForceMode2D.Impulse);
        }
    }
}