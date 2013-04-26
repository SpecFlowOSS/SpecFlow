
namespace System
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static bool IsAssignableFrom(this Type self, Type baseType)
        {
            return self.GetTypeInfo().IsAssignableFrom(baseType.GetTypeInfo());
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type self)
        {
            return self.GetRuntimeProperties();
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this Type self)
        {
            return self.GetTypeInfo().DeclaredConstructors;
        }

        public static IEnumerable<ConstructorInfo> GetPublicInstanceConstructors(this Type self)
        {
            return self.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsStatic && c.IsPublic);
        }

        public static IEnumerable<ConstructorInfo> GetPrivateInstanceConstructors(this Type self)
        {
            return self.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsStatic && !c.IsPublic);
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GenericTypeArguments;
        }

        public static IEnumerable<Type> GetInterfaces(this Type self)
        {
            return self.GetTypeInfo().ImplementedInterfaces;
        } 

        public static bool IsInstanceOfType(this Type self, object other)
        {
            return other != null && other.GetType().IsAssignableFrom(self);
        }

        public static IEnumerable<Type> GetTypes(this Assembly assembly)
        {
            return assembly.DefinedTypes.Select(t => t.AsType());
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this Type self, Type attributeType, bool inherit)
        {
            return (IEnumerable<Attribute>)self.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
        }
    }
}
