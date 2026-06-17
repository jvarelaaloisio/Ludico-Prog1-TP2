using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Views
{
    public class SetOrderingBasedOnY : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake)]
        [SerializeField] private new SpriteRenderer renderer;

        [Tooltip("This offset will be added to the final result of the calculation." +
                 "\nUse this to correct any ordering errors that the calculations can't fix.")]
        [SerializeField] private int offset;
        [Tooltip("How many numbers after the comma for the Y position are taken in account." +
                 "\ne.g.: If precision is 10, the first decimal is taken into account. If its 100, the first 2 are taken into account and so on.")]
        [SerializeField] private int precision = 10;

        private void OnValidate()
        {
            if (!Application.isPlaying && renderer)
                SetSortingOrder();
        }

        protected override void Start()
        {
            base.Start();
            if (!gameObject.isStatic)
                return;

            SetSortingOrder();
            enabled = false;
        }

        private void Update()
        {
            if (transform.hasChanged)
                SetSortingOrder();
        }

        private void SetSortingOrder()
            => renderer.sortingOrder = (int)(transform.position.y * -precision) + offset;
    }
}
