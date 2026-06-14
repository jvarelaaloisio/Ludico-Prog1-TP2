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
        [SerializeField] private float  knockbackForce = 100;
        [SerializeField] private bool onlyDamageOnceAfterEnable = true;
        [SerializeField] private UnityEvent onDamage;
        private List<Collider2D> _attackedColliders = new();

        private void OnEnable()
            => _attackedColliders.Clear();

        private async void OnTriggerEnter2D(Collider2D other)
        {
            try
            {
                if (_attackedColliders.Contains(other)
                    || !other.TryAttack(damage))
                    return;
                _attackedColliders.Add(other);

                if (!other.transform.TryGetComponent(out Rigidbody2D rigidBody))
                    return;

                Vector2 direction = (other.transform.position - transform.position).normalized;
                onDamage.Invoke();
                await Awaitable.FixedUpdateAsync();
                if (destroyCancellationToken.IsCancellationRequested)
                    return;
                rigidBody.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
                Debug.DrawRay(other.transform.position, direction, Color.yellow);
            }
            catch (Exception e) { Debug.LogException(e); }
        }
    }
}