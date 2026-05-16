using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using VarelaAloisio.Core.Debugging;

namespace VarelaAloisio.Core.Editor
{
    [CustomEditor(typeof(MacacoBehaviour), true, isFallback = true)]
    public class MacacoBehaviourEditor : UnityEditor.Editor
    {
        private readonly FieldInfo _loggerField = typeof(MacacoBehaviour).GetField("logger",
                                                                                      BindingFlags.NonPublic
                                                                                      | BindingFlags.Instance);
        private readonly FieldInfo _gizmoField = typeof(MacacoBehaviour).GetField("gizmo",
                                                                                      BindingFlags.NonPublic
                                                                                      | BindingFlags.Instance);

        private readonly string _targetNotSuperMonoException = $"Target is not {nameof(MacacoBehaviour)}";

        private void OnEnable()
        {
            if (target is not MacacoBehaviour superMonoBehaviour)
                throw new Exception(_targetNotSuperMonoException);

            bool isDirty = TryPopulateRef<ILogger>(superMonoBehaviour, _loggerField);
            isDirty |= TryPopulateRef<IGizmoDrawer>(superMonoBehaviour, _gizmoField);
            if (isDirty)
                EditorUtility.SetDirty(superMonoBehaviour);
        }

        private bool TryPopulateRef<T>(MacacoBehaviour macacoBehaviour, FieldInfo field)
        {
            object obj = field.GetValue(macacoBehaviour);
            if (obj is not Ref<T> @ref)
                throw new Exception($"The field {field.Name} in {nameof(MacacoBehaviour)} is not of type {nameof(Ref<T>)}");

            if (@ref.HasValue)
                return false;

            SearchContext searchContext = SearchUtil.GetContextFor(typeof(T));
            var items = SearchService.GetItems(searchContext);
            var item = items.FirstOrDefault();
            if (item is null)
            {
                Debug.LogWarning($"{name}: No Logger found in project. Consider creating a Debugger via \"Create/Debug/Debugger\"");
                return false;
            }
            if (item.ToObject() is not T logger)
                throw new Exception($"The search item is not of type {typeof(T).Name}");
            @ref.Value = logger;
            return true;
        }
    }
}
