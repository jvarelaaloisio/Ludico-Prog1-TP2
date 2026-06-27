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
    public class Shotgun : MacacoBehaviour, IWeapon
    {
        [SerializeField] private Collider2D pickUpTrigger;
        [Tooltip("The collider for when the weapon is on the ground")]
        [SerializeField] private Collider2D groundCollider;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private Ref<IThrower> thrower;
        [SerializeField] private Transform nozzle;
        [SerializeField] private Transform bulletPrefab;
        [SerializeField] private Ref<ICharacter> owner;

        [Space]
        [SerializeField] private float cooldown = .25f;
        [SerializeField] private float  stunDuration = .5f;
        [SerializeField] private float pickUpOffset = 1;
        [SerializeField] private int pellets = 3;
        [SerializeField] private int spreadDegrees = 15;

        [SerializeField] private UnityEvent onShoot;

        private CancellationTokenSource _attackTokenSource;
        private bool _isHoldingTrigger;
        private float _lastShotTime;

        /// <inheritdoc />
        public event Action<Vector2> OnHoldingTrigger;

        /// <inheritdoc />
        public event Action<Vector2> OnReleasedTrigger;

        /// <inheritdoc />
        public event Action<Vector2> OnThrow;

        public bool IsOnCooldown => _lastShotTime + cooldown > Time.time;
        public float CooldownLeft => cooldown - (Time.time - _lastShotTime);

        private void Update()
        {
            if (owner.HasValue)
            {
                Vector2 direction = owner.Value.Direction.normalized;
                transform.up = direction;
                transform.localPosition = direction * pickUpOffset;
            }
        }

        /// <inheritdoc />
        public async Task HoldTrigger(CancellationToken token)
        {
            if (_isHoldingTrigger)
                return;

            Log($"Holding trigger.");
            _isHoldingTrigger = true;
            OnHoldingTrigger?.Invoke(owner.Value.Direction);
            TokenUtils.Recreate(ref _attackTokenSource);
            try
            {
                if (IsOnCooldown)
                    await Awaitable.WaitForSecondsAsync(CooldownLeft);
                while (!token.IsCancellationRequested && !_attackTokenSource.Token.IsCancellationRequested)
                {
                    int side = 1;
                    for (int i = 0; i < pellets; i++, side *= -1)
                    {
                        Quaternion rotation = nozzle.rotation * Quaternion.AngleAxis(spreadDegrees * i * side, Vector3.forward);
                        Transform bullet = Instantiate(bulletPrefab, nozzle.position, rotation);

                        if (!owner.HasValue)
                            continue;

                        foreach (DamageWithKnockback damageSource in bullet.GetComponentsInChildren<DamageWithKnockback>())
                            damageSource.dontDamageTags.Add(owner.Value.gameObject.tag);
                        _lastShotTime = Time.time;
                    }
                    onShoot.Invoke();
                    await Awaitable.WaitForSecondsAsync(cooldown);
                }
            }
            catch (Exception e) { LogException(e); }
        }

        /// <inheritdoc />
        public Task ReleaseTrigger()
        {
            _isHoldingTrigger = false;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void SetOwner(ICharacter newOwner)
        {
            Log($"Setting owner to {(owner.HasValue ? owner.Value.transform.name : null)}");
            owner.Value = newOwner;
            transform.SetParent(newOwner.transform);
            pickUpTrigger.gameObject.SetActive(false);
            groundCollider.gameObject.SetActive(false);
            if (thrower.HasValue)
                thrower.Value.DeactivateDamage();
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = 0;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        /// <inheritdoc />
        public async void Throw(Vector2 direction)
        {
            if (!thrower.HasValue)
            {
                LogError($"Thrower is null.");
                return;
            }
            try
            {
                Log($"Throwing. Cancelling attack token.");
                TokenUtils.CancelAndDispose(ref _attackTokenSource);
                string ownerTag = owner.Value.gameObject.tag;
                owner.Value = null;
                transform.SetParent(null);

                OnThrow?.Invoke(direction);
                await thrower.Value.Do(rigidbody, groundCollider, direction, ownerTag);
                pickUpTrigger.gameObject.SetActive(true);
            }
            catch (Exception e) { LogException(e); }
        }
    }
}