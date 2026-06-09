using System;
using System.Threading;
using Core.Game;
using UnityEngine;

namespace Core.Combat
{
    public interface IWeapon
    {
        string name { get; }
        void HoldTrigger(CancellationToken token);
        void ReleaseTrigger();
        void SetOwner(ICharacter newOwner);
        void Throw(Vector2 direction);
        bool IsOnCooldown { get; }
        event Action<Vector2> OnAttack;
    }
}