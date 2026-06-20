using System;
using UnityEngine;
using UnityEngine.Rendering;
using VarelaAloisio.Core;

namespace Views
{
    public class LiftGammaView : MacacoBehaviour, IProgress<float>
    {
        [SerializeField] private Volume volume;

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
            volume = GetComponent<Volume>();
            if (!volume)
                volume = gameObject.AddComponent<Volume>();
        }

        public void Report(float value)
        {
            if (volume)
                volume.weight = value;
            else
                LogError($"Volume is null.");
        }
    }
}