using Core;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;

namespace Combat
{
    public class AutoPickupWeapon : MacacoBehaviour
    {
        [SerializeField] private Ref<ICharacter> character;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (character.HasValue
                && !character.Value.HasWeapon)
            {
                var weapon = other.GetComponentInParent<IWeapon>();
                if (weapon is null)
                    return;
                Log($"picking up weapon: {weapon.name}");
                character.Value.PickUp(weapon);
            }
        }
    }
}