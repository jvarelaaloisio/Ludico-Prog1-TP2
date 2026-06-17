using System;
using Core.Combat;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Combat
{
    public class KnockBackHandler : MacacoBehaviour, IKnockbackHandler
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake, OnError.Ignore)]
        [SerializeField] private new Rigidbody2D rigidbody;

        [SerializeField] private float force = 10f;
        public async void Handle(Vector2 direction, float forceMultiplier)
        {
            try
            {
                await Awaitable.FixedUpdateAsync();
                if (DisableCancellationToken.IsCancellationRequested || !rigidbody)
                    return;
                rigidbody.AddForce(direction * force * forceMultiplier, ForceMode2D.Impulse);
                DrawRay(rigidbody.position, direction * force * forceMultiplier, Color.red);
            }
            catch (Exception e) { LogException(e); }
        }
    }
}