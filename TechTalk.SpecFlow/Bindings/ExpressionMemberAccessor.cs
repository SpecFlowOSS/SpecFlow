using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    internal static class ExpressionMemberAccessor
    {
        public static PropertyInfo GetPropertyInfo<T, TResult>(this Expression<Func<T, TResult>> expression)
        {
            return (PropertyInfo)GetMemberInfo(expression);
        }

        public static MethodInfo GetMethodInfo<T, TResult>(this Expression<Func<T, TResult>> expression)
        {
            return (MethodInfo)GetMemberInfo(expression);
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            if (lambda.Body is MethodCallExpression callExpression)
                return callExpression.Method;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression unaryExpression)
            {
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member;
        }
    }
}