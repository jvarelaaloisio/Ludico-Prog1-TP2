using System;
using System.Threading;
using Core.Combat;
using Core.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Utils;

namespace Combat
{
    public class MeleeWeapon : MacacoBehaviour, IWeapon
    {
        [SerializeField] private Transform spriteToSwing;
        [SerializeField] private DamageWithKnockback damageTrigger;
        [SerializeField] private Collider2D thrownDamageTrigger;
        [SerializeField] private Collider2D pickUpTrigger;
        [Tooltip("The collider for when the weapon is on the ground")]
        [SerializeField] private Collider2D groundCollider;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private float scale = 1;
        [SerializeField] private float duration = .25f;
        [SerializeField] private float rotationOffset;
        [SerializeField] private AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float cooldown = .25f;
        [SerializeField] private Ref<ICharacter> owner;
        [SerializeField] private float triggerDistanceFromOwner = 1f;
        [SerializeField] private float  stunDuration = .5f;
        [SerializeField] private float secondsBeforeReactivatingCollision = .15f;
        [SerializeField] private float secondsBeforeItCanBePickedUpAgain = .5f;
        [SerializeField] private Vector3 pickUpOffset = new (-0.45f, -0.45f, 0f);
        [SerializeField] private float throwForce = 10;
        [SerializeField] private float throwTorque = 10;
        [SerializeField] private UnityEvent onPrepareAttack;
        [SerializeField] private UnityEvent onDoAttack;
        private CancellationTokenSource _attackTokenSource;
        private float _currentCharge = float.NaN;
        public bool IsOnCooldown { get; private set; }
        public event Action<Vector2> OnAttack;
        [ContextMenu("Swing")]
        private void DoTestRotation()
            => HoldTrigger(DisableCancellationToken);

        protected override void OnEnable()
        {
            base.OnEnable();
            if (damageTrigger)
                damageTrigger.OnHit += HandleHit;
        }

        protected override void Start()
        {
            base.Start();
            if (damageTrigger)
                damageTrigger.gameObject.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (damageTrigger)
                damageTrigger.OnHit -= HandleHit;
        }

        public void HoldTrigger(CancellationToken token)
        {
            OnAttack?.Invoke(owner.Value.Direction);
            onPrepareAttack.Invoke();
            _currentCharge = 0;
        }

        public async void ReleaseTrigger()
        {
            if(float.IsNaN(_currentCharge)
               || IsOnCooldown)
                return;
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
            IsOnCooldown = true;
            Vector3 originalPosition = spriteToSwing.localPosition;
            Quaternion originalRotation = spriteToSwing.localRotation;
            if (damageTrigger)
            {
                damageTrigger.gameObject.SetActive(true);
                damageTrigger.transform.position = owner.Value.transform.position
                                                   + (Vector3)(owner.Value.Direction * triggerDistanceFromOwner);
                damageTrigger.transform.up = owner.Value.Direction;
            }
            onDoAttack.Invoke();
            CancellationTokenRegistration registration = token.Register(CleanUp);
            const float pi = Mathf.PI;
            float now = Time.time;
            float start = Time.time;
            float rotationOffsetBasedOnDirection = GetRotationBasedOnDirection(owner.Value.Direction);
            while (now - start < duration)
            {
                now = Time.time;
                float lerp = swingCurve.Evaluate((now - start) / duration);
                float x = (lerp + rotationOffset + rotationOffsetBasedOnDirection) * pi;
                Vector3 position = owner.Value.transform.position + new Vector3(Mathf.Cos(x) * scale, Mathf.Sin(x) * scale);
                Vector3 direction = new Vector3(-Mathf.Sin(x), Mathf.Cos(x));
                DrawRay(position, direction, Color.red, 1);
                spriteToSwing.SetPositionAndRotation(position, Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0, 0, -90));
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
                if (damageTrigger)
                    damageTrigger.gameObject.SetActive(false);
                spriteToSwing.SetLocalPositionAndRotation(originalPosition, originalRotation);
                IsOnCooldown = false;
            }
        }

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
        }

        public async void Throw(Vector2 direction)
        {
            TokenUtils.CancelAndDispose(ref _attackTokenSource);
            owner.Value = null;
            transform.SetParent(null);
            thrownDamageTrigger.gameObject.SetActive(true);
            CancellationTokenRegistration cleanUpRegistration = DisableCancellationToken.Register(CleanUp);

            await Awaitable.FixedUpdateAsync();
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.AddForce(direction * throwForce, ForceMode2D.Impulse);
            rigidbody.AddTorque(throwTorque, ForceMode2D.Impulse);

            await Awaitable.WaitForSecondsAsync(secondsBeforeReactivatingCollision);
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            groundCollider.gameObject.SetActive(true);

            await Awaitable.WaitForSecondsAsync(secondsBeforeItCanBePickedUpAgain - secondsBeforeReactivatingCollision);
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            pickUpTrigger.gameObject.SetActive(true);
            CleanUp();
            await cleanUpRegistration.DisposeAsync();
            return;

            void CleanUp()
            {
                if (owner.HasValue)
                    damageTrigger.dontDamageTags.Remove(owner.Value.gameObject.tag);
            }
        }

        private async void HandleHit(Collider2D other)
        {
            try
            {
                if (!owner.HasValue)
                {
                    LogError($"{nameof(HandleHit)} called without an owner set");
                    return;
                }
                if (!other.transform.TryGetComponent(out IStunnable stunnable)
                    && (!other.transform.parent
                        || !other.transform.parent.TryGetComponent(out stunnable)))
                {
                    Debug.LogWarning($"Neither attack target ({other.name}) nor it's parent has an {nameof(IStunnable)} component.");
                    return;
                }

                Vector2 direction = (other.transform.position - owner.Value.transform.position).normalized;
                await Awaitable.FixedUpdateAsync();
                if (destroyCancellationToken.IsCancellationRequested)
                    return;
                stunnable.Stun(stunDuration, direction);
                Debug.DrawRay(other.transform.position, direction, Color.yellow);
            }
            catch (Exception e) { LogException(e); }
        }
    }
}
