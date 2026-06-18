using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Combat
{
    public class DamageCharger : MacacoBehaviour, ICharger, IDamagePointsSource
    {
        [SerializeField] private int baseDamage = 1;
        [Tooltip("How much time it takes to reach max charge")]
        [SerializeField] private float duration = 1;

        /// <inheritdoc />
        [field: SerializeField] public float MinCharge { get; private set; } = 1;

        [Tooltip("Max Charge = This + MinCharge")]
        [SerializeField] private float extraCharge = 1;

        /// <inheritdoc />
        
        public float MaxCharge
            => MinCharge + (DamageMultiplier?.Invoke(extraCharge) ?? extraCharge);

        /// <inheritdoc />
        [field:SerializeField, SerializeReadOnly] public float Charge { get; private set; }
        /// <inheritdoc />
        [field:SerializeField, SerializeReadOnly] public bool IsCharging { get; private set; }

        /// <inheritdoc />
        public Func<float, float> DamageMultiplier { private get; set; }

        /// <inheritdoc />
        public int RoundedDamage => Mathf.RoundToInt(Damage);
        /// <inheritdoc />
        public float Damage => (MinCharge + DamageMultiplier?.Invoke(Charge) ?? Charge) * baseDamage;
        /// <inheritdoc />
        public void ResetCharge()
            => Charge = 0;

        /// <inheritdoc />
        public async Task StartCharging(CancellationToken token)
        {
            float start = Time.time;
            float now = 0;
            IsCharging = true;
            do
            {
                now = Time.time;
                float lerp = (now - start) / duration;
                Charge = Mathf.Lerp(MinCharge, MaxCharge, lerp);
                await Awaitable.NextFrameAsync();
            } while (now < start + duration && !token.IsCancellationRequested);

            IsCharging = false;
        }
    }
}