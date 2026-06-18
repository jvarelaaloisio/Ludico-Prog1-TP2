using Core;
using Core.Combat;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;

namespace Combat
{
    public class AutoPickupWeapon : MacacoBehaviour
    {
        [SerializeField] private Ref<ICharacter> character;
        [SerializeField] private new Collider2D collider2D;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (character.HasValue)
            {
                character.Value.OnThrow += HandleThrow;
            }
        }

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
                collider2D.enabled = false;
            }
        }

        private void HandleThrow(IWeapon weapon)
            => collider2D.enabled = true;
    }
}