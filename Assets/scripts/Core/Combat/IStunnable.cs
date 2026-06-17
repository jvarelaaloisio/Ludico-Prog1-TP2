using UnityEngine;

namespace Core.Combat
{
    public interface IStunnable
    {
        /// <summary /> Stun the character, disabling its movement and attack and adding a knockback.
        /// <param name="duration">How much time to disable character actions</param>
        /// <param name="direction"></param>
        void Stun(float duration, Vector2 direction);
    }
}