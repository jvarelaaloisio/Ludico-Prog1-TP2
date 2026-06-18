using System;
using System.Collections.Generic;
using Core.Combat;
using HealthSystem.Runtime.Components;
using HealthSystem.Runtime.Helpers;
using UnityEngine;
using UnityEngine.Events;
using VarelaAloisio.Core;

namespace Combat
{
    public class DamageWithKnockback : DamageDealer
    {
        [SerializeField] protected Ref<ILogger> logger;
        [SerializeField] private bool onlyDamageOnceAfterEnable = true;
        [SerializeField] private UnityEvent onDamage;
        private List<Collider2D> _attackedColliders = new();
        public List<string> dontDamageTags;
        [Header("Scaling")]
        [SerializeField] private bool shouldScaleWithDamage;
        [Tooltip("The scale for this object will be set to source.Damage * this value")]
        [SerializeField] private float scaleDamageMultiplier = 1;
        [SerializeField] private float minScale = 1.15f;

        private Vector3 _originalScale;
        public event Action<Collider2D> OnHit;
        [SerializeField] private Ref<IDamagePointsSource> damageSource;

        private void Awake()
            => _originalScale = transform.localScale;

        private void OnEnable()
        {
            _attackedColliders.Clear();

            if (shouldScaleWithDamage && damageSource.HasValue)
            {
                float scaleMultiplier = Mathf.Max(minScale, damageSource.Value.Damage * scaleDamageMultiplier);
                Vector3 scale = _originalScale * scaleMultiplier;
                (logger.HasValue ? logger.Value : Debug.unityLogger).Log(name, $"Setting scale to {scale}", this);
                transform.localScale = scale;
            }
        }

        private void OnDisable()
            => transform.localScale = _originalScale;

        private void OnTriggerEnter2D(Collider2D other)
        {
            int sourcedDamage = (damageSource.HasValue ? damageSource.Value.RoundedDamage : 1) * damage;
            if (dontDamageTags.Contains(other.gameObject.tag)
                || (_attackedColliders.Contains(other) && onlyDamageOnceAfterEnable)
                || !other.TryAttack(sourcedDamage))
                return;
            (logger.HasValue ? logger.Value : Debug.unityLogger).Log(name, $"Successfully attacked {other.gameObject.name} for {sourcedDamage} damage", this);
            _attackedColliders.Add(other);
            OnHit?.Invoke(other);
            onDamage?.Invoke();
        }
    }
}