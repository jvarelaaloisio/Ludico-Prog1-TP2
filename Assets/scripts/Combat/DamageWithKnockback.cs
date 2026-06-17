using System;
using System.Collections.Generic;
using Core.Combat;
using HealthSystem.Runtime.Components;
using HealthSystem.Runtime.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class DamageWithKnockback : DamageDealer
    {
        [SerializeField] private float  knockbackForce = 1;
        [SerializeField] private bool onlyDamageOnceAfterEnable = true;
        [SerializeField] private UnityEvent onDamage;
        private List<Collider2D> _attackedColliders = new();
        public List<string> dontDamageTags;

        private void OnEnable()
            => _attackedColliders.Clear();

        private async void OnTriggerEnter2D(Collider2D other)
        {
            try
            {
                if (dontDamageTags.Contains(other.gameObject.tag)
                    || _attackedColliders.Contains(other)
                    || !other.TryAttack(damage))
                    return;
                _attackedColliders.Add(other);

                if (!other.transform.TryGetComponent(out IStunnable stunnable)
                    && (!other.transform.parent
                    || !other.transform.parent.TryGetComponent(out stunnable)))
                {
                    Debug.LogWarning($"Neither attack target ({other.name}) nor it's parent has an {nameof(IStunnable)} component.");
                    return;
                }

                Vector2 direction = (other.transform.position - transform.position).normalized;
                onDamage.Invoke();
                await Awaitable.FixedUpdateAsync();
                if (destroyCancellationToken.IsCancellationRequested)
                    return;
                stunnable.Stun(knockbackForce, direction);
                Debug.DrawRay(other.transform.position, direction, Color.yellow);
            }
            catch (Exception e) { Debug.LogException(e); }
        }
    }
}