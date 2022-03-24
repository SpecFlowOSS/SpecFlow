using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public static class BindingReflectionExtensions
    {
        public static string GetShortDisplayText(this IBindingMethod bindingMethod)
        {
            return string.Format("{2}.{0}({1})", bindingMethod.Name, string.Join(", ", bindingMethod.Parameters.Select(p => p.Type.Name).ToArray()), bindingMethod.Type.Name);
        }

        public static bool IsAssignableTo(this Type type, IBindingType baseType)
        {
            if (baseType is RuntimeBindingType runtimeBindingType)
                return runtimeBindingType.Type.IsAssignableFrom(type);

            if (type.FullName == baseType.FullName)
                return true;

            if (type.BaseType != null && IsAssignableTo(type.BaseType, baseType))
                return true;

            return type.GetInterfaces().Any(_if => IsAssignableTo(_if, baseType));
        }

        public static bool IsAssignableFrom(this Type baseType, IBindingType type)
        {
            if (type is IPolymorphicBindingType polymorphicBindingType)
                return polymorphicBindingType.IsAssignableTo(new RuntimeBindingType(baseType));

            if (type.FullName == baseType.FullName)
                return true;

            return false;
        }

        public static bool MethodEquals(this IBindingMethod method1, IBindingMethod method2)
        {
            if (ReferenceEquals(method1, method2))
                return true;

            if (method1 == null || method2 == null)
                return false;

            if (method1 is RuntimeBindingMethod bindingMethod1 && method2 is RuntimeBindingMethod bindingMethod2)
                return bindingMethod1.MethodInfo.Equals(bindingMethod2.MethodInfo);

            return method1.Name == method2.Name &&
                   method1.Type.TypeEquals(method2.Type) &&
                   method1.Parameters.ParamsEquals(method2.Parameters);
        }

        public static bool TypeEquals(this IBindingType type1, Type type2)
        {
            if (type1 is RuntimeBindingType runtimeBindingType)
                return runtimeBindingType.Type == type2;

            return TypeEquals(type1, new RuntimeBindingType(type2));
        }

        public static bool TypeEquals(this IBindingType type1, IBindingType type2)
        {
            if (ReferenceEquals(type1, type2))
                return true;

            if (type1 == null || type2 == null)
                return false;

            if (type1 is RuntimeBindingType bindingType1 && type2 is RuntimeBindingType bindingType2)
                return bindingType1.Type == bindingType2.Type;

            return type1.FullName == type2.FullName;
        }

        public static bool ParamEquals(this IBindingParameter param1, IBindingParameter param2)
        {
            if (ReferenceEquals(param1, param2))
                return true;

            if (param1 == null || param2 == null)
                return false;

            return param1.Type.TypeEquals(param2.Type);
        }

        private static bool ParamsEquals(this IEnumerable<IBindingParameter> params1, IEnumerable<IBindingParameter> params2)
        {
            if (ReferenceEquals(params1, params2))
                return true;

            if (params1 == null || params2 == null || params1.Count() != params2.Count())
                return false;

            return !params1.Where((t, i) => !t.ParamEquals(params2.ElementAt(i))).Any();
            //TODO: use this in .NET4: return params1.Zip(params2, (p1, p2) => p1.ParamEquals(p2)).All(eq => eq);
        }

        internal static MethodInfo AssertMethodInfo(this IBindingMethod bindingMethod)
        {
            if (bindingMethod is RuntimeBindingMethod reflectionBindingMethod)
            {
                return reflectionBindingMethod.MethodInfo;
            }

            throw new SpecFlowException("The binding method cannot be used for reflection: " + bindingMethod);
        }
    }
}