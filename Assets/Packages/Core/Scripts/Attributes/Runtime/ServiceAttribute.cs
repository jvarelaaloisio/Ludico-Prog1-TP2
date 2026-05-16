using System;

namespace VarelaAloisio.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public Type[] Interfaces { get; }
        public bool OverrideIfFound { get; }

        public ServiceAttribute(Type interfaceType, bool overrideIfFound = true)
        {
            Interfaces = new []{ interfaceType };
            OverrideIfFound = overrideIfFound;
        }

        public ServiceAttribute(bool overrideIfFound = true, params Type[] interfaces)
        {
            Interfaces = interfaces;
            OverrideIfFound = overrideIfFound;
        }
    }
}