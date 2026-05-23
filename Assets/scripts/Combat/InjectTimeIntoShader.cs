using System.Threading;
using UnityEngine;
using VarelaAloisio.Core;

namespace Combat
{
    public class InjectTimeIntoShader : MacacoBehaviour
    {
        [SerializeField] private Material material;
        protected override void OnEnable()
        {
            base.OnEnable();
            InjectTime(DisableCancellationToken);
        }

        private async void InjectTime(CancellationToken token)
        {
            float start = Time.time;
            while (!token.IsCancellationRequested)
            {
                float time = Time.time - start;
                material.SetFloat("_InjectedTime", time);
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
