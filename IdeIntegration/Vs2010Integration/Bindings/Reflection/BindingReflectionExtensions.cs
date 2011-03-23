using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public static class BindingReflectionExtensions
    {
        public static bool MethodEquals(this IBindingMethod method1, IBindingMethod method2)
        {
            if (ReferenceEquals(method1, method2))
                return true;

            if (method1 == null || method2 == null)
                return false;

            return method1.Name == method2.Name &&
                   method1.Type.TypeEquals(method2.Type) &&
                   method1.Parameters.ParamsEquals(method2.Parameters);
        }

        public static bool TypeEquals(this IBindingType type1, IBindingType type2)
        {
            if (ReferenceEquals(type1, type2))
                return true;

            if (type1 == null || type2 == null)
                return false;

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

            return params1.Zip(params2, (p1, p2) => p1.ParamEquals(p2)).All(eq => eq);
        }
    }
}