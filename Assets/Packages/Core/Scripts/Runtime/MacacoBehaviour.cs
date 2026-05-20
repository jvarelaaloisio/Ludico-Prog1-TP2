using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Core.Debugging;
using VarelaAloisio.Core.Extensions;
using VarelaAloisio.Core.Runtime;
using VarelaAloisio.Core.Utils;

namespace VarelaAloisio.Core
{
    public class MacacoBehaviour : MonoBehaviour, IMonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] protected Ref<ILogger> logger;
        [SerializeField] protected Ref<IGizmoDrawer> gizmo;

        [AutoMap(How.GameObjectName, When.Awake)]
        [ContextMenuItem("Reset Log Tag", nameof(ResetLogTag))]
        [SerializeField] protected string logTag;
        private (FieldInfo info, AutoMap attr)[] _autoMappedFields;
        private (PropertyInfo info, AutoMap attr)[] _autoMappedProperties;
        private ServiceAttribute _serviceAttribute;

        protected virtual void Reset()
        {
            ResetLogTag();
            SetupAutomapFields();
            MapMembersIn(When.Reset);
        }

        private void ResetLogTag()
            => logTag = $"{GetType().Name}";

        protected virtual void Awake()
        {
            logger.Value ??= Debug.unityLogger;

        #region Add service
            _serviceAttribute = GetType().GetCustomAttribute<ServiceAttribute>();
            if (_serviceAttribute is not null)
            {
                foreach (Type @interface in _serviceAttribute.Interfaces)
                {
                    if (@interface.IsInstanceOfType(this))
                        Service.Add(@interface, this, _serviceAttribute.OverrideIfFound);
                    else
                        LogError($"Object is marked as Service<{@interface.Name}> but it doesn't implement that type.");
                }
            }
        #endregion

            SetupAutomapFields();
            MapMembersIn(When.Awake);
        }

        protected virtual void OnEnable()
            => MapMembersIn(When.OnEnable);

        protected virtual void Start()
            => MapMembersIn(When.Start);

        protected virtual void OnDestroy()
        {
            if (_serviceAttribute is null)
                return;
            foreach (Type @interface in _serviceAttribute.Interfaces)
                Service.Remove(@interface);
        }

    #region Logging

        [HideInCallstack]
        protected void Log(string message)
            => logger.Log(logTag, message, this);

        [HideInCallstack]
        protected void LogWarning(string message)
            => logger.LogWarning(logTag, message, this);

        [HideInCallstack]
        protected void LogError(string message)
            => logger.LogError(logTag, message, this);

        [HideInCallstack]
        protected void LogException(Exception exception)
            => logger.LogException(logTag, exception, this);

    #endregion

    #region Drawing

        [HideInCallstack]
        protected void DrawLine(Vector3 from, Vector3 to, Color color)
            => gizmo.Value?.DrawLine(logTag, from, to, color);

        [HideInCallstack]
        protected void DrawLine(Vector3 from, Vector3 to, Color color, float duration)
            => gizmo.Value?.DrawLine(logTag, from, to, color, duration);

        [HideInCallstack]
        protected void DrawRay(Vector3 from, Vector3 dir, Color color)
            => gizmo.Value?.DrawRay(logTag, from, dir, color);

        [HideInCallstack]
        protected void DrawRay(Vector3 from, Vector3 dir, Color color, float duration)
            => gizmo.Value?.DrawRay(logTag, from, dir, color, duration);

    #endregion

    #region Member mapping

        private void SetupAutomapFields()
        {
            Type ownType = GetType();

            _autoMappedFields = AutoMapUtils.GetDeclaredFields(ownType)
                                            .Select(field => (field, field.GetCustomAttribute<AutoMap>()))
                                            .ToArray();
            _autoMappedProperties = AutoMapUtils.GetDeclaredProperties(ownType)
                                                .Select(field => (field, field.GetCustomAttribute<AutoMap>()))
                                                .ToArray();
        }

        private void MapMembersIn(When moment)
        {
            if (_autoMappedFields is null)
            {
                LogError($"Seems {nameof(SetupAutomapFields)} wasn't called correctly. Are you overriding Awake without calling base?");
                return;
            }
            foreach ((FieldInfo field, AutoMap attr) in _autoMappedFields.Where(FieldIsAtMoment))
                if (!TryMapField(field, attr))
                    return;

            foreach ((PropertyInfo field, AutoMap attr) in _autoMappedProperties.Where(PropertyIsAtMoment))
                if(!TryMapProperty(field, attr))
                    return;
            return;

            bool FieldIsAtMoment((FieldInfo _, AutoMap attr) field)
                => (moment & field.attr.When) == moment;

            bool PropertyIsAtMoment((PropertyInfo _, AutoMap attr) field)
                => (moment & field.attr.When) == moment;
        }

        private bool TryMapField(FieldInfo info, AutoMap attr)
        {
            Type type = info.FieldType;
            if (!IsValid(info, attr, type, out bool isRef, out object obj))
                return false;
            if (isRef)
            {
                PropertyInfo valueProp = info.FieldType.GetProperty("Value");
                object refTarget = info.GetValue(this);
                valueProp?.SetMethod?.Invoke(refTarget, new[] { obj });
            }
            else
                info.SetValue(this, obj);

            return true;
        }

        private bool TryMapProperty(PropertyInfo info, AutoMap attr)
        {
            MethodInfo setter = info.GetSetMethod()
                                ?? info.GetSetMethod(true);
            if (setter is null && !info.CanWrite)
                throw new Exception($"{info.Name} has no setter");

            Type type = info.PropertyType;
            if (!IsValid(info, attr, type, out bool isRef, out object obj))
                return false;
            if (isRef)
            {
                PropertyInfo valueProp = info.PropertyType.GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance);
                valueProp?.SetMethod?.Invoke(obj, new[] { obj });
            }
            else
                setter?.Invoke(this, new[] { obj });

            return true;
        }

        /// <summary /> Validates and fetches the object to map. Also handles errors.
        /// <param name="member">The field or property to fetch</param>
        /// <param name="attr">The attribute found in the member</param>
        /// <param name="type">The type of the member</param>
        /// <param name="isRef"></param>
        /// <param name="obj"></param>
        private bool IsValid(MemberInfo member, AutoMap attr, Type type, out bool isRef, out object obj)
        {
            isRef = IsRef();
            if (isRef)
                type = type.GetGenericArguments()[0];

            obj = attr.How switch
                  {
                      How.GetComponent => GetComponent(type),
                      How.GetComponentInChildren => GetComponentInChildren(type),
                      How.Service => Service.Get(type),
                      How.GameObjectName => name,
                      _ => throw new ArgumentOutOfRangeException()
                  };
            if (obj is null)
            {
                Color color = Color.cornflowerBlue;
                logger.LogError(logTag,
                                $"Member {member.Name.Colored(color)} {"not found".Colored(Color.darkRed)} via {attr.How.Colored(color)} at {attr.When.Colored(color)}",
                                this);
                switch (attr.OnError)
                {
                    case OnError.DisableComponent:
                        logger.LogWarning(logTag, $"Disabling {name.Colored(color)}.{GetType().Name.Colored(color)}.", this);
                        enabled = false;
                        break;
                    case OnError.ThrowException:
                        throw new Exception($"Failed to map {name}.{GetType().Name}.{member.Name}");
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return false;
            }
            Type objType = obj.GetType();
            if (objType != type
                && objType.IsAssignableFrom(type))
                logger.LogError(logTag, $"obj.type ({objType}) is not assignable to {type})");
            return true;

            bool IsRef()
                => type.IsGenericType
                   && typeof(Ref<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }

    #endregion
    }
}