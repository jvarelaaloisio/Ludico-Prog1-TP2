using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VarelaAloisio.Core
{
    [Serializable]
    public class Ref<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private UnityEngine.Object reference;
        private UnityEngine.Object _cachedReference;

        /// <summary /> true if there is a reference assigned.
        public bool HasValue => reference;

        /// <summary /> The Reference
        public T Value
        {
            get => HasValue ? (T)(object)reference : default;
            set
            {
                reference = value as UnityEngine.Object;
                Validate();
            }
        }

        /// <summary /> Called on EditorApplication.update, used before serializing the object from memory to text.
        public void OnBeforeSerialize()
        {
            if (_cachedReference != reference || (reference && reference is not T))
                Validate();
        }

        /// <summary /> Called when unity loads the object and deserializes it.
        public void OnAfterDeserialize()
            => _cachedReference = reference;

        /// <summary /> Instantiates one object and tries to cast it into the type T.
        /// <param name="parent">The parent for the new instance</param>
        /// <returns>The new instance</returns>
        /// <exception cref="Exception">If this has no value, the instantiation yields no results or if the cast fails.</exception>
        public async Task<T> InstantiateAsync(Transform parent = null)
        {
            var type = typeof(T);
            if (!HasValue)
                throw new Exception($"Ref has no value assigned. Type: {type.Name}");
            var result = await Object.InstantiateAsync(reference, parent);
            if (result.Length < 1)
                throw new Exception($"Instantiation yielded no objects. Type: {type.Name}");
            return result[0] switch
                   {
                       T directCast
                           => directCast,
                       MonoBehaviour monoBehaviour when monoBehaviour.TryGetComponent(out T component)
                           => component,
                       _
                           => throw new Exception($"Instantiated object of type {result[0].GetType().FullName} cannot be casted into {type.Name}")
                   };
        }

        /// <summary /> Instantiates one object and tries to cast it into the type T.
        /// <param name="count">How many objects to instantiate</param>
        /// <param name="positions">Positions for the objects, positions count must be equal to count</param>
        /// <param name="rotations">Rotations for the objects, rotations count must be equal to count</param>
        /// <param name="parent">The parent for the new instance</param>
        /// <param name="token"></param>
        /// <returns>The new instance</returns>
        /// <exception cref="Exception">If this has no value or if the cast fails.</exception>
        public async Task<T[]> InstantiateAsync(int count,
                                              Vector3[] positions,
                                              Quaternion[] rotations,
                                              CancellationToken token,
                                              Transform parent = null)
        {
            Type type = typeof(T);
            if (!HasValue)
                throw new Exception($"Ref has no value assigned. Type: {type.Name}");
            var result = await Object.InstantiateAsync(reference, count, parent, positions, rotations, token);
            var resultArray = new T[count];

            for (int i = 0; i < result.Length; i++)
            {
                resultArray[i] = result[i] switch
                                 {
                                     T directCast
                                         => directCast,
                                     MonoBehaviour monoBehaviour when monoBehaviour.TryGetComponent(out T component)
                                         => component,
                                     _ => throw new
                                              Exception($"Instantiated object of type {result[0].GetType().FullName} cannot be casted into {type.Name}")
                                 };
            }

            return resultArray;
        }

        private void Validate()
        {
            if (reference is GameObject gameObject
                && gameObject.TryGetComponent(out T target))
                reference = target as UnityEngine.Object;

            if (reference && reference is not T)
            {
                Debug.LogError($"{reference.GetType().Name} does not implement {typeof(T)}");
                if (reference != _cachedReference)
                    reference = _cachedReference;
                else
                {
                    reference = null;
                    _cachedReference = null;
                }
            }
            else
            {
                _cachedReference = reference;
            }
        }

        /// <summary /> Checks if the reference is valid for the given T. Currently not in use.
        /// <param name="reference">The target</param>
        /// <returns>true if reference implements T or has a component which implements T</returns>
        public static bool IsValid(UnityEngine.Object reference)
        {
            if (reference is GameObject gameObject
                && gameObject.TryGetComponent(out T target))
                reference = target as UnityEngine.Object;

            return !reference || reference is T;
        }
    }
}