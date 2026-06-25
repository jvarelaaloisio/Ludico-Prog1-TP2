using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Combat
{
    public interface IThrower
    {
        event Action<Collider2D> OnHit;
        /// <summary /> Throw self towards a direction and harm any non-owner damageable hit.
        /// <param name="rigidbody">Self rigidbody</param>
        /// <param name="groundCollider"></param>
        /// <param name="direction"></param>
        /// <param name="ownerTag"></param>
        /// <returns></returns>
        Task Do(Rigidbody2D rigidbody, Collider2D groundCollider, Vector2 direction, string ownerTag);

        /// <summary /> Deactivate damage trigger
        void DeactivateDamage();
    }
}