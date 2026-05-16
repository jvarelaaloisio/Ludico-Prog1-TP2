using System;

namespace VarelaAloisio.Core.Attributes
{
    public enum How
    {
        GetComponent,
        GetComponentInChildren,
        Service,
        GameObjectName,
    }
    [Flags]
    public enum When
    {
        Reset = 1,
        Awake = 2,
        OnEnable = 4,
        Start = 8,
    }
    [Flags]
    public enum OnError
    {
        DisableComponent = 1,
        ThrowException = 2,
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class AutoMap : Attribute
    {
        public AutoMap(How how, When when, OnError onError = OnError.DisableComponent)
        {
            How = how;
            When = when;
            OnError = onError;
        }

        public How How { get; }
        public When When { get; }
        public OnError OnError { get; }
    }
}