using System;
using System.Linq;
using System.Reflection;
using VarelaAloisio.Core.Attributes;

namespace VarelaAloisio.Core.Utils
{
    public static class AutoMapUtils
    {
        private const BindingFlags Flags = BindingFlags.Public
                                         | BindingFlags.NonPublic
                                         | BindingFlags.Instance
                                         | BindingFlags.Static
                                         | BindingFlags.FlattenHierarchy;

        public static MemberInfo[] GetAutoMappedMembers(Type type)
        {
            var props = GetDeclaredProperties(type);

            var fields = GetDeclaredFields(type);

            var members = props.Cast<MemberInfo>()
                               .Concat(fields)
                               .ToArray();

            return members;
        }

        public static FieldInfo[] GetDeclaredFields(Type type)
            => type.GetFields(Flags)
                   .Where(f => f.IsDefined(typeof(AutoMap), inherit: true))
                   .ToArray();

        public static PropertyInfo[] GetDeclaredProperties(Type type)
            => type.GetProperties(Flags)
                   .Where(p => p.IsDefined(typeof(AutoMap), inherit: true))
                   .ToArray();
    }
}