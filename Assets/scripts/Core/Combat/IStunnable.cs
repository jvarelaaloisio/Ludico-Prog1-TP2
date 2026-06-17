using System;
using UnityEngine;

namespace Core.Combat
{
    public interface IStunnable
    {
        /// <summary /> Stun this unit, disabling its movement and attack and adding a knockback.
        /// <param name="duration">How much time to disable actions</param>
        /// <param name="direction"></param>
        void Stun(float duration, Vector2 direction);

        /// <summary /> If this unit is currently stunned
        bool IsStunned { get; }
        /// <summary /> Called when the stun begins
        event Action OnStun;
        /// <summary /> Called when the stun ends
        event Action OnRecovery;
    }
}