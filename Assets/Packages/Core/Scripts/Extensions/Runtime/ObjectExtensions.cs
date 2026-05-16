using UnityEngine;

namespace VarelaAloisio.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Checks if target is null and logs if needed.
        /// </summary>
        /// <param name="caller">The object that is calling this method</param>
        /// <param name="target">The object to null-check</param>
        /// <param name="shouldLogErrorIfNull">If the target is null, should an error be logged</param>
        /// <typeparam name="TCaller">The Type of the object that is calling this method</typeparam>
        /// <typeparam name="TTarget">The type of the object to null-check</typeparam>
        /// <returns><para>False if target is null</para>
        /// <para>True if target is <b>not</b> null</para>
        /// </returns>
        [HideInCallstack]
        public static bool IsNotNull<TCaller, TTarget>(this TCaller caller, TTarget target, bool shouldLogErrorIfNull = true)
        {
            var isNull = target == null;
            if (isNull && shouldLogErrorIfNull)
                if (caller is UnityEngine.Object sourceUnityObject)
                    Debug.LogError($"{nameof(TTarget)} is null!", sourceUnityObject);
                else
                    Debug.LogError($"{nameof(TCaller)}: {nameof(TTarget)} is null!");
            return !isNull;
        }

        /// <summary>
        /// Checks if target is null and logs if needed.
        /// </summary>
        /// <param name="target">The object to null-check</param>
        /// <param name="shouldLogErrorIfNull">If the target is null, should an error be logged</param>
        /// <typeparam name="T">The Type of the object that is calling this method</typeparam>
        /// <returns><para>False if target is null</para>
        /// <para>True if target is <b>not</b> null</para>
        /// </returns>
        [HideInCallstack]
        public static bool IsNotNull<T>(this T target, bool shouldLogErrorIfNull = true)
        {
            var isNull = target == null;
            if (isNull && shouldLogErrorIfNull)
                Debug.LogError($"{nameof(T)}: Reference is null!");
            return !isNull;
        }
    }
}
