using System.Threading;
using Core.Combat;
using UnityEngine;

namespace Core
{
    public interface ICharacter
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        Vector2 Direction { get; set; }
        IWeapon CurrentWeapon { get; }
        bool HasWeapon { get; }
        void PickUp(IWeapon weapon);
        bool TryAttack();
        bool TryThrowWeapon();
        /// <summary /> Start moving, based on <see cref="Direction"/>
        /// <param name="token">Use to stop movement</param>
        void Move(CancellationToken token);
    }
}