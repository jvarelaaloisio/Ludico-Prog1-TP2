using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;

namespace Combat
{
    public class Thrower : MacacoBehaviour, IThrower
    {
        [SerializeField] private DamageWithKnockback damageTrigger;
        [SerializeField] private float throwForce = 20;
        [SerializeField] private float throwTorque = 5;
        [SerializeField] private float velocityCancellationAfterThrow = 0.9f;
        [SerializeField] private float secondsBeforeReactivatingCollision = .15f;
        [SerializeField] private float secondsBeforeItCanBePickedUpAgain = .5f;

        [Header("Slow down")]
        [SerializeField] private float slowDownDamping = 3f;
        [SerializeField] private float slowDownDampingDuration = 2f;

        /// <inheritdoc />
        public event Action<Collider2D> OnHit;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            damageTrigger.OnHit += HandleHit;
        }

        /// <inheritdoc />
        public async Task Do(Rigidbody2D rigidbody, Collider2D groundCollider, Vector2 direction, string ownerTag)
        {
            damageTrigger.dontDamageTags.Add(ownerTag);
            damageTrigger.gameObject.SetActive(true);
            float originalLinearDamping = rigidbody.linearDamping;
            float originalAngularDamping = rigidbody.angularDamping;
            CancellationTokenRegistration cleanUpRegistration = DisableCancellationToken.Register(CleanUp);

            await Awaitable.FixedUpdateAsync();
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.AddForce(direction * throwForce, ForceMode2D.Impulse);
            rigidbody.AddTorque(throwTorque, ForceMode2D.Impulse);

        #region Activate ground collision

            await Awaitable.WaitForSecondsAsync(secondsBeforeReactivatingCollision);
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            Log($"{secondsBeforeReactivatingCollision} seconds have passed. Reactivating collision.");
            groundCollider.gameObject.SetActive(true);

        #endregion

        #region Slow down

            float secondsToActivatePickup = secondsBeforeItCanBePickedUpAgain - secondsBeforeReactivatingCollision;
            await Awaitable.WaitForSecondsAsync(secondsToActivatePickup / 4);
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            await Awaitable.FixedUpdateAsync();
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            Log($"Punching self with -Velocity * {velocityCancellationAfterThrow} to slow down");
            rigidbody.AddForce(-rigidbody.linearVelocity * velocityCancellationAfterThrow, ForceMode2D.Impulse);
            rigidbody.AddTorque(-rigidbody.totalTorque * velocityCancellationAfterThrow, ForceMode2D.Impulse);
            _ = ReturnToOriginalDampingAfter(slowDownDampingDuration, rigidbody, rigidbody.linearDamping, rigidbody.angularDamping);
            rigidbody.linearDamping = slowDownDamping;
            rigidbody.angularDamping = slowDownDamping;

            #endregion

        #region Pick-up delay

            await Awaitable.WaitForSecondsAsync(secondsToActivatePickup * 3 / 4);
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            Log($"{secondsToActivatePickup} seconds have passed. Reactivating pickup trigger.");
            CleanUp();
            await cleanUpRegistration.DisposeAsync();

        #endregion

            void CleanUp()
            {
                Log($"Cleaning up. Removing owner {ownerTag} from damage filter.");
                damageTrigger.gameObject.SetActive(false);
                damageTrigger.dontDamageTags.Remove(ownerTag);
            }
        }

        private async Task ReturnToOriginalDampingAfter(float seconds, Rigidbody2D rigidbody, float linearDamping, float angularDamping)
        {
            await Awaitable.WaitForSecondsAsync(seconds);
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            rigidbody.linearDamping = linearDamping;
            rigidbody.angularDamping = angularDamping;
        }

        /// <inheritdoc />
        public void DeactivateDamage()
            => damageTrigger.gameObject.SetActive(false);

        private void HandleHit(Collider2D other)
        {
            OnHit?.Invoke(other);
        }
    }
}