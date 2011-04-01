//
// 
//
// Methods originaly written by
//   Jb Evain (jbevain@novell.com)
//   Miguel de Icaza (miguel@novell.com)
//
// (C) 2008 Novell, Inc. (http://www.novell.com)
//
//  modified by (C) 2010 siaqodb (http://siaqodb.com)

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;

using System.Reflection;
using System.Linq.Expressions;

namespace System.Linq.jvm
{
    public class ExpressionHelper
    {
        internal const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
		
        internal static MethodInfo GetFalseOperator(Type self)
        {
            return GetBooleanOperator("op_False", self);
        }

        static MethodInfo GetBooleanOperator(string op, Type self)
        {
            return GetUnaryOperator(op, self, self, typeof(bool));
        }
        static MethodInfo GetUnaryOperator(string oper_name, Type declaring, Type param)
        {
            return GetUnaryOperator(oper_name, declaring, param, null);
        }

        static MethodInfo GetUnaryOperator(string oper_name, Type declaring, Type param, Type ret)
        {
            var methods = declaring.GetNotNullableType().GetMethods(PublicStatic);

            foreach (var method in methods)
            {
                if (method.Name != oper_name)
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;

                if (method.IsGenericMethod)
                    continue;

                if (!IsAssignableToParameterType(param.GetNotNullableType(), parameters[0]))
                    continue;

                if (ret != null && method.ReturnType != ret.GetNotNullableType())
                    continue;

                return method;
            }

            return null;
        }
        static bool IsAssignableToParameterType(Type type, ParameterInfo param)
        {
            var ptype = param.ParameterType;
            if (ptype.IsByRef)
                ptype = ptype.GetElementType();

            return type.GetNotNullableType().IsAssignableTo(ptype);
        }
        internal static MethodInfo GetTrueOperator(Type self)
        {
            return GetBooleanOperator("op_True", self);
        }
        internal static bool IsPrimitiveConversion(Type type, Type target)
        {
            if (type == target)
                return true;

            if (type.IsNullable() && target == type.GetNotNullableType())
                return true;

            if (target.IsNullable() && type == target.GetNotNullableType())
                return true;

            if (IsConvertiblePrimitive(type) && IsConvertiblePrimitive(target))
                return true;

            return false;
        }
        static bool IsConvertiblePrimitive(Type type)
        {
            var t = type.GetNotNullableType();

            if (t == typeof(bool))
                return false;

            if (t.IsEnum)
                return true;

            return t.IsPrimitive;
        }

       
       
    }
}
