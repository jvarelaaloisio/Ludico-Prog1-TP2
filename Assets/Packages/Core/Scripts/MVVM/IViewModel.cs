using System;

namespace VarelaAloisio.Core
{
    [Obsolete("Use IProgress<T> instead")]
    public interface IViewModel<in T>
    {
        /// <summary /> Called when the value changes.
        /// <param name="value">The new value</param>
        void HandleValueChanged(T value);
    }
}