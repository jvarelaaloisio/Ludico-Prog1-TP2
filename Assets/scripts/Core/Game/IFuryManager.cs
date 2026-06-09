using System;

namespace Core.Game
{
    public interface IFuryManager
    {
        float Fury { get; set; }
        event Action<float, float> OnFuryUpdated;
        void AddFury(float amount);
        void ResetFury();
    }
}