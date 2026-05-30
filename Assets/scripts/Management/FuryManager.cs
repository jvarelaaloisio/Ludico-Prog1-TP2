using System;
using Core;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Management
{
    [Service(typeof(IFuryManager))]
    public class FuryManager : MacacoBehaviour, IFuryManager
    {
        [Tooltip("Fury is meant to be capped between 0 (meaning start of the level or no fury)" +
                 "\nand 1 (meaning, end of the level or full fury)")]
        [UnityEngine.Range(0, 1)]
        [SerializeField] private float fury;

        public event Action<float, float> OnFuryUpdated;

        public float Fury
        {
            get => fury;
            set
            {
                float old = fury;
                fury = value;
                OnFuryUpdated?.Invoke(old, value);
            }
        }

        public void AddFury(float amount)
        {
            float newFury = Mathf.Clamp01(Fury + amount);
            Log($"{amount} fury received. Fury: {Fury} -> {newFury}");
            Fury = newFury;
        }
    }
}
