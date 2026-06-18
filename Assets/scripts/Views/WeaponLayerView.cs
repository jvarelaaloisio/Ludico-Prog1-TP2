using System;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;

namespace Views
{
    [Obsolete("Use SetOrderingBasedOnY instead")]
    public class WeaponLayerView : MacacoBehaviour
    {
        [Serializable]
        private struct SortingByDirection
        {
            [field: SerializeField] public int Sorting { get; private set; }
            [field: SerializeField] public float DirectionY { get; private set; }
        }
        [SerializeField] private Ref<IWeapon> weapon;
        [SerializeField] private new SpriteRenderer renderer;
        [SerializeField] private SortingByDirection[] orderings;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!weapon.HasValue)
            {
                LogWarning("Weapon is null");
                return;
            }

            weapon.Value.OnHoldingTrigger += HandleHoldingTrigger;
        }

        private void HandleHoldingTrigger(Vector2 direction)
        {
            direction.Normalize();
            foreach (SortingByDirection orderingByDirection in orderings)
            {
                if (Mathf.Approximately(orderingByDirection.DirectionY, direction.y))
                {
                    renderer.sortingOrder = orderingByDirection.Sorting;
                    return;
                }
            }
        }
    }
}
