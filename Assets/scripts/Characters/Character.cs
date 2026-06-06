using System;
using System.Threading;
using Core;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;

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
            weapon.SetOwner(this);
            if (currentWeapon.HasValue)
                currentWeapon.Value.Release();
            currentWeapon.Value = weapon;
        }

        public bool TryAttack()
        {
            if (!currentWeapon.HasValue
                || currentWeapon.Value.IsOnCooldown)
                return false;
            _attackSource?.Cancel();
            _attackSource?.Dispose();
            _attackSource = new();
            currentWeapon.Value.Attack(LinkWithDisable(_attackSource.Token));
            return true;
        }

        public bool TryThrowWeapon()
        {
            LogError("Not Implemented");
            return false;
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