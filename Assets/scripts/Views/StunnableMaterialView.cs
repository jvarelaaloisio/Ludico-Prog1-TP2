using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Views
{
    public class StunnableMaterialView : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset, OnError.Ignore)]
        [SerializeField] private Ref<IStunnable> stunnable;
        [AutoMap(How.GetComponent, When.Reset | When.Awake, OnError.Ignore)]
        [SerializeField] private new SpriteRenderer renderer;

        [Header("Colors")]
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color stunnedColor = Color.darkRed;

        protected override void Reset()
        {
            base.Reset();
            if (renderer)
                defaultColor = renderer.color;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (stunnable.HasValue)
            {
                stunnable.Value.OnStun += HandleStun;
                stunnable.Value.OnRecovery += HandleStunRecovery;
            }
        }

        private void HandleStun()
        {
            if (renderer)
                renderer.color = stunnedColor;
        }

        private void HandleStunRecovery()
        {
            if (renderer)
                renderer.color = defaultColor;
        }
    }
}