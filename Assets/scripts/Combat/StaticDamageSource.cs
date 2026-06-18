using System;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;

namespace Combat
{
    public class StaticDamageSource : MacacoBehaviour, IDamagePointsSource
    {
        [SerializeField] private int baseDamage;
        /// <inheritdoc />
        public Func<float, float> DamageMultiplier { private get; set; }

        /// <inheritdoc />
        public int RoundedDamage => Mathf.RoundToInt(Damage);

        /// <inheritdoc />
        public float Damage => (1 + (DamageMultiplier?.Invoke(1) ?? 0)) * baseDamage;
    }
}