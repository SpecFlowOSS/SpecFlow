using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TechTalk.SpecFlow.Rpc.Client
{
    public class ExtractMethodInfo<TClientInterface>
    {
        public InterfaceMethodInfo ExtractFunction<TResult>(Expression<Func<TClientInterface, TResult>> p)
        {
            var body = p.Body as MethodCallExpression;

            return ExtractInfo(body);
        }

        public InterfaceMethodInfo ExtractMethod(Expression<Action<TClientInterface>> p)
        {
            var body = p.Body as MethodCallExpression;

            return ExtractInfo(body);
        }

        private InterfaceMethodInfo ExtractInfo(MethodCallExpression body)
        {
            var methodName = body.Method.Name;
            var typeName = body.Method.DeclaringType.FullName;


            var arguments = ExtractArguments(body);

            return new InterfaceMethodInfo(body.Method.DeclaringType.Assembly.FullName, typeName, methodName, arguments);
        }

        private Dictionary<int, object> ExtractArguments(MethodCallExpression body)
        {
            Dictionary<int, object> arguments = new Dictionary<int, object>();

            int index = 0;
            foreach (var bodyArgument in body.Arguments)
            {
                switch(bodyArgument)
                {
                    case ConstantExpression constantExpression:
                        arguments[index] = constantExpression.Value;
                        break;
                    case MemberExpression memberExpression:
                        arguments[index] = GetValue(memberExpression);
                        break;
                    default:
                        throw new NotSupportedException($"Expression of type {bodyArgument.GetType()} is not supported");
                }

                index++;
            }
            return arguments;
        }

        private object GetValue(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }
    }
}