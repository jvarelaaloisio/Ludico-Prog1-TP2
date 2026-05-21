using System.Threading;
using Core;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;

namespace Characters
{
    public class Character : MonoBehaviourAsync, ICharacter
    {
        [SerializeField] private Ref<IWeapon> currentWeapon;
        private CancellationTokenSource _attackSource;
        public IWeapon CurrentWeapon => currentWeapon.HasValue ? currentWeapon.Value : null;
        public void PickUp(IWeapon weapon)
        {
            weapon.SetOwner(transform);
            if (currentWeapon.HasValue)
                currentWeapon.Value.Release();
            currentWeapon.Value = weapon;
        }

        public bool TryAttack()
        {
            if (!currentWeapon.HasValue
                || currentWeapon.Value.IsOnCooldown)
                return false;
            _attackSource?.Cancel();
            _attackSource?.Dispose();
            _attackSource = new();
            currentWeapon.Value.Attack(_attackSource.Token);
            return true;
        }
    }
}