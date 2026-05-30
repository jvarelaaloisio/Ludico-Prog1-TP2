using HealthSystem.Runtime.Components;
using HealthSystem.Runtime.Helpers;
using UnityEngine;

namespace Combat
{
    public class DamageWithKnockback : DamageDealer
    {
        [SerializeField] private float  knockbackForce = 100;

        private async void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryAttack(damage)
                && other.transform.TryGetComponent(out Rigidbody2D rigidBody))
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                await Awaitable.FixedUpdateAsync();
                rigidBody.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
                Debug.DrawRay(other.transform.position, direction, Color.yellow);
            }
        }
    }
}