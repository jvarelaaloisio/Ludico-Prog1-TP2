using System;
using System.Collections.Generic;
using HealthSystem.Runtime.Components;
using HealthSystem.Runtime.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class DamageWithKnockback : DamageDealer
    {
        [SerializeField] private bool onlyDamageOnceAfterEnable = true;
        [SerializeField] private UnityEvent onDamage;
        private List<Collider2D> _attackedColliders = new();
        public List<string> dontDamageTags;
        public event Action<Collider2D> OnHit;

        private void OnEnable()
            => _attackedColliders.Clear();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (dontDamageTags.Contains(other.gameObject.tag)
                || _attackedColliders.Contains(other)
                || !other.TryAttack(damage))
                return;
            _attackedColliders.Add(other);
            OnHit?.Invoke(other);
            onDamage?.Invoke();
        }
    }
}