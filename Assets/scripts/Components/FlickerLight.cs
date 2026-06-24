using UnityEngine;
using UnityEngine.Rendering.Universal;
using VarelaAloisio.Core;

namespace Components
{
    public class FlickerLight : MacacoBehaviour
    {
        [SerializeField] private new Light2D light;
        [SerializeField] private AnimationCurve flickerCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float frequency = 1;

        private void Update()
        {
            float duration = flickerCurve.keys[flickerCurve.length - 1].time -  flickerCurve.keys[0].time;
            light.intensity = flickerCurve.Evaluate(Mathf.Sin(Time.time * frequency) / duration);
        }
    }
}