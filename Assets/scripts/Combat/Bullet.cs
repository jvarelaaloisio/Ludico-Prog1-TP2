using System;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MacacoBehaviour
    {
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float force = 10;
        [AutoMap(How.GetComponent, When.ResetAndAwake)]
        private Rigidbody2D _rigidbody;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            Invoke(nameof(OnBecameInvisible), lifetime);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _rigidbody.AddForce(transform.up * force, ForceMode2D.Impulse);
        }

        private void OnBecameInvisible()
            => Destroy(gameObject);
    }
}