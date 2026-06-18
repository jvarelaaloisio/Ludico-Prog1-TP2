using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Combat;
using Core.Game;
using UnityEngine;
using UnityEngine.Events;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Utils;

namespace Combat
{
    public class MeleeWeapon : MacacoBehaviour, IWeapon, ISwing
    {
        [SerializeField] private Transform spriteToSwing;
        [SerializeField] private DamageWithKnockback damageTrigger;
        [SerializeField] private DamageWithKnockback thrownDamageTrigger;
        [SerializeField] private Collider2D pickUpTrigger;
        [Tooltip("The collider for when the weapon is on the ground")]
        [SerializeField] private Collider2D groundCollider;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private Ref<ICharger> attackCharger;

        [Space]
        [SerializeField] private float scale = 1;
        [SerializeField] private float duration = .25f;
        [SerializeField] private float rotationOffset;
        [SerializeField] private AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float cooldown = .25f;

        [Space]
        [SerializeField] private Ref<ICharacter> owner;
        [SerializeField] private float triggerDistanceFromOwner = 1f;
        [SerializeField] private float  stunDuration = .5f;
        [SerializeField] private float secondsBeforeReactivatingCollision = .15f;
        [SerializeField] private float secondsBeforeItCanBePickedUpAgain = .5f;
        [SerializeField] private Vector3 pickUpOffset = new (-0.45f, -0.45f, 0f);
        [SerializeField] private float throwForce = 10;
        [SerializeField] private float throwTorque = 10;
        [SerializeField] private float velocityCancellationAfterThrow = 0.9f;
        [SerializeField] private UnityEvent onPrepareAttack;
        [SerializeField] private UnityEvent onDoAttack;
        private CancellationTokenSource _attackTokenSource;
        private bool _isHoldingTrigger;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        public bool IsOnCooldown { get; private set; }
        public event Action<Vector2> OnHoldingTrigger;

        /// <inheritdoc />
        public event Action<Vector2> OnReleasedTrigger;

        /// <inheritdoc />
        public event Action<Vector2> OnThrow;

        /// <inheritdoc />
        public event Action OnSwing;

        /// <inheritdoc />
        public event Action OnSwung;

        [ContextMenu("Swing")]
        private void DoTestRotation()
            => _ = HoldTrigger(DisableCancellationToken);

        protected override void OnEnable()
        {
            base.OnEnable();
            if (damageTrigger)
                damageTrigger.OnHit += HandleHit;
            if (thrownDamageTrigger)
                thrownDamageTrigger.OnHit += HandleHit;
        }

        protected override void Start()
        {
            base.Start();
            if (damageTrigger)
                damageTrigger.gameObject.SetActive(false);
            CacheSwingPositionAndRotation(out _originalPosition, out _originalRotation);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (damageTrigger)
                damageTrigger.OnHit -= HandleHit;
            if (thrownDamageTrigger)
                thrownDamageTrigger.OnHit -= HandleHit;
        }

        public async Task HoldTrigger(CancellationToken token)
        {
            while (IsOnCooldown && !token.IsCancellationRequested)
                await Awaitable.NextFrameAsync();
            if (token.IsCancellationRequested)
                return;

            Log($"Holding trigger.");
            _isHoldingTrigger = true;
            attackCharger.Value?.StartCharging(token);
            OnHoldingTrigger?.Invoke(owner.Value.Direction);
            onPrepareAttack.Invoke();
        }

        public async Task ReleaseTrigger()
        {
            if(!_isHoldingTrigger || IsOnCooldown)
                return;
            Log("Trigger released. Swinging.");
            _attackTokenSource = new CancellationTokenSource();
            CancellationToken token = LinkWithDisable(_attackTokenSource.Token);
            if (!owner.HasValue)
            {
                LogError("Need an owner to swing");
                return;
            }
            if (!spriteToSwing)
            {
                LogError("No sprite to swing.");
                return;
            }

            _isHoldingTrigger = false;
            IsOnCooldown = true;
            try
            {
                onDoAttack.Invoke();
                OnSwing?.Invoke();
            }
            catch (Exception e) { LogException(e); }
            if (damageTrigger)
            {
                damageTrigger.gameObject.SetActive(true);
                damageTrigger.transform.position = owner.Value.transform.position
                                                   + (Vector3)(owner.Value.Direction * triggerDistanceFromOwner);
                damageTrigger.transform.up = owner.Value.Direction;
            }
            OnReleasedTrigger?.Invoke(_originalPosition);
            CancellationTokenRegistration registration = token.Register(CleanUp);
            float now = Time.time;
            float start = Time.time;
            while (now - start < duration)
            {
                now = Time.time;
                float lerp = swingCurve.Evaluate((now - start) / duration);
                Vector3 position = CalculatePosition(lerp);
                Vector3 direction = CalculateDirection(lerp);
                SetPositionAndDirection(position, direction);
                await Awaitable.NextFrameAsync();
                if (token.IsCancellationRequested)
                    return;
            }

            await Awaitable.WaitForSecondsAsync(cooldown);
            if (token.IsCancellationRequested)
                return;
            CleanUp();
            await registration.DisposeAsync();
            return;

            void CleanUp()
            {
                Log($"Finished swinging. Cleaning up.");
                if (damageTrigger)
                    damageTrigger.gameObject.SetActive(false);
                SetPositionAndRotation(_originalPosition, _originalRotation);
                IsOnCooldown = false;
                attackCharger.Value?.ResetCharge();
                OnSwung?.Invoke();
            }
        }

        public void CacheSwingPositionAndRotation(out Vector3 originalPosition, out Quaternion originalRotation)
        {
            originalPosition = spriteToSwing.localPosition;
            originalRotation = spriteToSwing.localRotation;
        }

        /// <summary /> Calculate the position for a given point in the rotation, using owner's direction as base.
        /// <param name="lerp">A [0..1] range representing the state of the rotation.
        /// <para>0 is the start of the rotation, 1 is the end of it</para> </param>
        public Vector3 CalculatePosition(float lerp)
        {
            const float pi = Mathf.PI;
            Vector2 ownerDirection = owner.HasValue ? owner.Value.Direction : transform.up;
            float rotationOffsetBasedOnDirection = GetRotationBasedOnDirection(ownerDirection);
            float x = (lerp + rotationOffset + rotationOffsetBasedOnDirection) * pi;
            Vector3 ownerPosition = owner.HasValue ? owner.Value.transform.position : transform.position;
            return ownerPosition + new Vector3(Mathf.Cos(x) * scale, Mathf.Sin(x) * scale);
        }

        /// <summary /> Calculate the direction for a given point in the rotation, using owner's direction as base.
        /// <param name="lerp">A [0..1] range representing the state of the rotation.
        /// <para>0 is the start of the rotation, 1 is the end of it</para> </param>
        public Vector3 CalculateDirection(float lerp)
        {
            const float pi = Mathf.PI;
            Vector2 ownerDirection = owner.HasValue ? owner.Value.Direction : transform.up;
            float rotationOffsetBasedOnDirection = GetRotationBasedOnDirection(ownerDirection);
            float x = (lerp + rotationOffset + rotationOffsetBasedOnDirection) * pi;
            return new Vector3(-Mathf.Sin(x), Mathf.Cos(x));
        }

        /// <summary>
        /// Set the position and direction of the sprite. Use <see cref="CalculatePosition"/> and <see cref="CalculateDirection"/> to get the values you need.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public void SetPositionAndDirection(Vector3 position, Vector3 direction)
        {
            DrawRay(position, direction, Color.red, 1);
            spriteToSwing.SetPositionAndRotation(position, Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0, 0, -90));
        }

        /// <inheritdoc />
        public void SetPositionAndRotation(Vector3 originalPosition, Quaternion originalRotation)
            => spriteToSwing.SetLocalPositionAndRotation(originalPosition, originalRotation);

        /// <summary /> This formula converts angles into a rotation offset.
        /// The conversion is based on this table of values:
        /// <p>Direction | Angles | result</p>
        /// <p>    Up    |   0°   |   0</p>
        /// <p>   Left   |  -90°  |  0.5</p>
        /// <p>   Right  |   90°  | -0.5</p>
        /// <p>   Down   |  180°  |  -1</p>
        /// <param name="direction">The direction the character is facing</param>
        /// <returns>The Rotation offset to get the weapon swinging into the given direction</returns>
        private float GetRotationBasedOnDirection(Vector2 direction)
            => Vector2.SignedAngle(direction, Vector2.up) / 180 * -1;

        public void SetOwner(ICharacter newOwner)
        {
            Log($"Setting owner to {(owner.HasValue ? owner.Value.transform.name : null)}");
            owner.Value = newOwner;
            transform.SetParent(newOwner.transform);
            transform.SetLocalPositionAndRotation(pickUpOffset, Quaternion.identity);
            pickUpTrigger.gameObject.SetActive(false);
            groundCollider.gameObject.SetActive(false);
            thrownDamageTrigger.gameObject.SetActive(false);
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = 0;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            damageTrigger.dontDamageTags.Add(newOwner.gameObject.tag);
            thrownDamageTrigger.dontDamageTags.Add(newOwner.gameObject.tag);
        }

        public async void Throw(Vector2 direction)
        {
            try
            {
                Log($"Throwing. Cancelling attack token.");
                TokenUtils.CancelAndDispose(ref _attackTokenSource);
                string ownerTag = owner.Value.gameObject.tag;
                owner.Value = null;
                transform.SetParent(null);
                thrownDamageTrigger.gameObject.SetActive(true);
                spriteToSwing.localPosition = Vector3.zero;
                CancellationTokenRegistration cleanUpRegistration = DisableCancellationToken.Register(CleanUp);
                OnThrow?.Invoke(direction);

                await Awaitable.FixedUpdateAsync();
                if (DisableCancellationToken.IsCancellationRequested)
                    return;
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                rigidbody.AddForce(direction * throwForce, ForceMode2D.Impulse);
                rigidbody.AddTorque(throwTorque, ForceMode2D.Impulse);

                await Awaitable.WaitForSecondsAsync(secondsBeforeReactivatingCollision);
                if (DisableCancellationToken.IsCancellationRequested)
                    return;
                Log($"{secondsBeforeReactivatingCollision} seconds have passed. Reactivating collision.");
                groundCollider.gameObject.SetActive(true);
                float secondsToActivatePickup = secondsBeforeItCanBePickedUpAgain - secondsBeforeReactivatingCollision;

                await Awaitable.WaitForSecondsAsync(secondsToActivatePickup / 4);
                if (DisableCancellationToken.IsCancellationRequested)
                    return;
                await Awaitable.FixedUpdateAsync();
                if (DisableCancellationToken.IsCancellationRequested)
                    return;
                Log($"Punching self with -Velocity * {velocityCancellationAfterThrow} to slow down");
                rigidbody.AddForce(-rigidbody.linearVelocity * velocityCancellationAfterThrow, ForceMode2D.Impulse);
                rigidbody.AddTorque(-rigidbody.angularVelocity * velocityCancellationAfterThrow, ForceMode2D.Impulse);

                await Awaitable.WaitForSecondsAsync(secondsToActivatePickup * 3 / 4);
                if (DisableCancellationToken.IsCancellationRequested)
                    return;
                Log($"{secondsToActivatePickup} seconds have passed. Reactivating pickup trigger.");
                pickUpTrigger.gameObject.SetActive(true);

                CleanUp();
                await cleanUpRegistration.DisposeAsync();

                void CleanUp()
                {
                    Log($"Cleaning up. Removing owner {ownerTag} from damage filter.");
                    damageTrigger.dontDamageTags.Remove(ownerTag);
                    thrownDamageTrigger.gameObject.SetActive(false);
                    thrownDamageTrigger.dontDamageTags.Remove(ownerTag);
                }
            }
            catch (Exception e) { LogException(e); }
        }

        private async void HandleHit(Collider2D other)
        {
            try
            {
                Log($"Handling hit. Target: {other.gameObject.name}");
                if (!other.transform.TryGetComponent(out IStunnable stunnable)
                    && (!other.transform.parent
                        || !other.transform.parent.TryGetComponent(out stunnable)))
                {
                    Debug.LogWarning($"Neither attack target ({other.name}) nor it's parent has an {nameof(IStunnable)} component.");
                    return;
                }

                Vector3 stunOrigin = owner.HasValue ? owner.Value.transform.position : transform.position;
                Vector2 direction = (other.transform.position - stunOrigin).normalized;
                await Awaitable.FixedUpdateAsync();
                if (destroyCancellationToken.IsCancellationRequested)
                    return;
                Log($"Stunning target ({other.name}) towards {direction} for {duration} seconds.");
                stunnable.Stun(stunDuration, direction);
                Debug.DrawRay(other.transform.position, direction, Color.yellow);
            }
            catch (Exception e) { LogException(e); }
        }
    }
}
