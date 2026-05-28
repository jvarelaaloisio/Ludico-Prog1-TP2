using System;

namespace Core
{
    public interface IFuryManager
    {
        float Fury { get; set; }
        event Action<float, float> OnFuryUpdated;
        void AddFury(float amount);
    }
}