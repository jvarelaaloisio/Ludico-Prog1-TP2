using UnityEngine;

namespace VarelaAloisio.Core.Extensions {
    public static class CoroutineExtensions {
        public static void TryStop(this Coroutine coroutine, MonoBehaviour runner)
        {
            if (coroutine != null && runner != null)
                runner.StopCoroutine(coroutine);
        }
    }
}