using System.Threading;
using System.Threading.Tasks;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Utils;

namespace Views
{
    public class MeleeWeaponView : MacacoBehaviour
    {
        [SerializeField] private Ref<IWeapon> weapon;
        [SerializeField] private Ref<ISwing> swingable;
        [SerializeField] private Ref<ICharger> damageCharger;
        [SerializeField] private Transform pivot;
        [SerializeField] private new SpriteRenderer renderer;
        [SerializeField] private string layerWhileCharging = "Effects";
        [SerializeField] private float maxScale = 2;
        private int _defaultRendererSortingLayer;
        private CancellationTokenSource _controlSizeTokenSource;
        private CancellationTokenSource _controlPositionAndRotationTokenSource;

        protected override void OnEnable()
        {
            if (renderer)
                _defaultRendererSortingLayer = renderer.sortingLayerID;
            base.OnEnable();
            if (weapon.HasValue)
            {
                weapon.Value.OnHoldingTrigger += HandleHoldingTrigger;
                weapon.Value.OnThrow += HandleThrow;
            }

            if (swingable.HasValue)
            {
                swingable.Value.OnSwung += HandleSwung;
                swingable.Value.OnSwing += HandleSwing;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (weapon.HasValue)
                weapon.Value.OnHoldingTrigger -= HandleHoldingTrigger;

            if (swingable.HasValue)
            {
                swingable.Value.OnSwung -= HandleSwung;
                swingable.Value.OnSwing -= HandleSwing;
            }
        }

        private void HandleHoldingTrigger(Vector2 direction)
        {
            TokenUtils.Recreate(ref _controlSizeTokenSource);
            _ = ControlSize(LinkWithDisable(_controlSizeTokenSource.Token));

            TokenUtils.Recreate(ref _controlPositionAndRotationTokenSource);
            _ = ControlPositionWhileChargingWeapon(LinkWithDisable(_controlPositionAndRotationTokenSource.Token));
        }

        private void HandleSwing()
        {
            if (renderer)
                renderer.sortingLayerID = SortingLayer.NameToID(layerWhileCharging);
            TokenUtils.CancelAndDispose(ref _controlPositionAndRotationTokenSource);
        }

        private void HandleSwung()
        {
            TokenUtils.CancelAndDispose(ref _controlSizeTokenSource);
            TokenUtils.CancelAndDispose(ref _controlPositionAndRotationTokenSource);
            if (renderer)
                renderer.sortingLayerID = _defaultRendererSortingLayer;
        }

        private void HandleThrow(Vector2 direction)
        {
            TokenUtils.CancelAndDispose(ref _controlSizeTokenSource);
            TokenUtils.CancelAndDispose(ref _controlPositionAndRotationTokenSource);
            if (renderer)
                renderer.sortingLayerID = _defaultRendererSortingLayer;
        }

        private async Task ControlPositionWhileChargingWeapon(CancellationToken token)
        {
            if (!swingable.HasValue)
            {
                LogError($"Swingable is null.");
                return;
            }
            swingable.Value.CacheSwingPositionAndRotation(out Vector3 swingableOriginalPosition, out Quaternion swingableOriginalRotation);
            token.Register(CleanUp);

            while (!token.IsCancellationRequested)
            {
                Vector3 position = swingable.Value.CalculatePosition(0);
                Vector3 direction = swingable.Value.CalculateDirection(0);
                swingable.Value.SetPositionAndDirection(position, direction);
                await Awaitable.NextFrameAsync();
            }

            void CleanUp()
            {
                if (swingable.HasValue)
                    swingable.Value.SetPositionAndRotation(swingableOriginalPosition, swingableOriginalRotation);
            }
        }
        private async Task ControlSize(CancellationToken token)
        {
            if (!pivot || !damageCharger.HasValue)
            {
                LogError($"Some value needed is null." +
                         $"\nSprite Pivot: {(pivot ? pivot.name : null)}." +
                         $"\nDamage Charger: {(damageCharger.HasValue ? damageCharger.Value : null)}");
                return;
            }

            Vector3 spriteOriginalScale = pivot.localScale;
            token.Register(CleanUp);

            while (!token.IsCancellationRequested)
            {
                float lerp = Mathf.InverseLerp(damageCharger.Value.MinCharge, damageCharger.Value.MaxCharge, damageCharger.Value.Charge);
                pivot.localScale = Vector3.one * Mathf.Lerp(spriteOriginalScale.x, maxScale * damageCharger.Value.MaxCharge, lerp);
                await Awaitable.NextFrameAsync();
            }

            void CleanUp()
            {
                if (pivot)
                    pivot.localScale = spriteOriginalScale;
            }
        }
    }
}
