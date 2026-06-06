using System.Threading;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Views
{
    public class InjectTimeIntoRenderer : MacacoBehaviour
    {
        private static readonly int TimeProperty = Shader.PropertyToID("_InjectedTime");
        [AutoMap(How.GetComponent, When.Reset | When.OnEnable)]
        [SerializeField] private new Renderer renderer;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            InjectTime(DisableCancellationToken);
        }

        private async void InjectTime(CancellationToken token)
        {
            var block = new MaterialPropertyBlock();
            float start = Time.time;
            while (!token.IsCancellationRequested)
            {
                float time = Time.time - start;
                block.SetFloat(TimeProperty, time);
                renderer.SetPropertyBlock(block);
                await Awaitable.NextFrameAsync();
            }
        }
    }
}