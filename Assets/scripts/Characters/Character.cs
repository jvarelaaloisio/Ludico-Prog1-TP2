using System;
using System.Threading;
using Core;
using Core.Combat;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Utils;

namespace Characters
{
    public class Character : MacacoBehaviour, ICharacter
    {
        [SerializeField] private Ref<IWeapon> currentWeapon;

        [Header("Movement")]
        [SerializeField] private float goalSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private float brakeMultiplier = .85f;

        private CancellationTokenSource _attackSource;
        public Vector2 Direction { get; set; } = Vector2.down;
        public IWeapon CurrentWeapon => currentWeapon.HasValue ? currentWeapon.Value : null;
        public bool HasWeapon => currentWeapon.HasValue;

        public void PickUp(IWeapon weapon)
        {
            if (currentWeapon.HasValue)
                return;
            weapon.SetOwner(this);
            currentWeapon.Value = weapon;
        }

        public bool TryStartAttacking()
        {
            if (!currentWeapon.HasValue
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
            if (!HasWeapon)
                return false;

            TokenUtils.CancelAndDispose(ref _attackSource);
            CurrentWeapon.Throw(Direction);
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
                    Vector2 currentVelocity = rigidBody.linearVelocity;
                    if (Vector2.Angle(currentVelocity, Direction) > 45)
                        ApplyBrakeForce();
                    float currentSpeed = currentVelocity.magnitude;
                    float speedDifferential = goalSpeed - currentSpeed;
                    rigidBody.AddForce(Direction * speedDifferential, ForceMode2D.Force);
                    await Awaitable.FixedUpdateAsync();
                }
                await registration.DisposeAsync();
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