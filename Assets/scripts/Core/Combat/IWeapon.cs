using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game;
using UnityEngine;

namespace Core.Combat
{
    public interface IWeapon
    {
        /// <summary /> When the weapon starts attacking
        event Action<Vector2> OnHoldingTrigger;
        /// <summary /> When the weapon stops attacking
        event Action<Vector2> OnReleasedTrigger;
        /// <summary /> When the weapon is thrown by it's owner
        event Action<Vector2> OnThrow;
        string name { get; }
        bool IsOnCooldown { get; }
        Task HoldTrigger(CancellationToken token);
        Task ReleaseTrigger();
        void SetOwner(ICharacter newOwner);
        void Throw(Vector2 direction);
    }
}