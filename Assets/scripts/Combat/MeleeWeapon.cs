using System;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Combat;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;

namespace Combat
{
    public class MeleeWeapon : MacacoBehaviour, IWeapon
    {
        [SerializeField] private Transform spriteToSwing;
        [SerializeField] private Collider2D damageTrigger;
        [SerializeField] private Collider2D pickUpTrigger;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private float scale = 1;
        [SerializeField] private float duration = .25f;
        [SerializeField] private float rotationOffset;
        [SerializeField] private AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float cooldown = .25f;
        [SerializeField] private Ref<ICharacter> owner;
        [SerializeField] private float triggerDistanceFromOwner = 1f;
        [SerializeField] private float secondsBeforeItCanBePickedUpAgain = .5f;
        [SerializeField] private Vector3 pickUpOffset = new (-0.45f, -0.45f, 0f);
        [SerializeField] private float throwForce = 10;
        public bool IsOnCooldown { get; private set; }
        public event Action<Vector2> OnAttack; 
        [ContextMenu("Swing")]
        private void DoTestRotation()
            => HoldTrigger(DisableCancellationToken);

        protected override void Reset()
        {
            base.Reset();
            damageTrigger = GetComponent<Collider2D>();
            damageTrigger ??= gameObject.AddComponent<BoxCollider2D>();
            damageTrigger.isTrigger = true;
        }

        protected override void Start()
        {
            base.Start();
            if (damageTrigger)
                damageTrigger.gameObject.SetActive(false);
        }

        public async void HoldTrigger(CancellationToken token)
        {
            if (!owner.HasValue)
                LogError("Need an owner to swing");
            if (!spriteToSwing)
                LogError("No sprite to swing.");
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
            OnAttack?.Invoke(owner.Value.Direction);
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

        public void ReleaseTrigger()
        {
            LogError("Not Implemented");
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
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public async void Throw(Vector2 direction)
        {
            owner = null;
            transform.SetParent(null);
            await Awaitable.FixedUpdateAsync();
            if (DisableCancellationToken.IsCancellationRequested)
                return;
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.AddForce(direction * throwForce, ForceMode2D.Impulse);
            await Awaitable.WaitForSecondsAsync(secondsBeforeItCanBePickedUpAgain);
            if (!DisableCancellationToken.IsCancellationRequested)
                pickUpTrigger.gameObject.SetActive(true);
        }
    }
}
