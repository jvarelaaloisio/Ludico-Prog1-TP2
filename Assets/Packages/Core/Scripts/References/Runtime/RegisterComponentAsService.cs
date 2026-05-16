using System;
using UnityEngine;

namespace VarelaAloisio.Core.Runtime
{
    /// <summary /> Register another component as a service. It registers every interface.
    /// <remarks>Only use this when you cannot use the ServiceAttribute</remarks>
    [AddComponentMenu("Register Component As Service")]
    [Tooltip("Only use this when you cannot use the ServiceAttribute")]
    public class RegisterComponentAsService : MonoBehaviour
    {
        [SerializeField] private Component target;
        [SerializeField] private bool overrideIfFound = true;

        private void Awake()
        {
            var interfaces = target.GetType().GetInterfaces();
            if (interfaces is null or {Length: 0})
            {
                Service.Add(target.GetType(), target, overrideIfFound);
                return;
            }
            foreach (Type type in interfaces)
                Service.Add(type, target, overrideIfFound);
        }
    }
}