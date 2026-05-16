using UnityEngine;
// ReSharper disable InconsistentNaming

namespace VarelaAloisio.Core
{
    public interface IMonoBehaviour
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
    }
}