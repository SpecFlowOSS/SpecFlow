using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Client
{
    public class Client<TClientInterface> : IDisposable
    {
        private RawClient _rawClient;

        public Client()
        {
            _rawClient = new RawClient(4635);
        }

        public void Dispose()
        {
            _rawClient?.Dispose();
            _rawClient = null;
        }

        public async Task<TResult> Execute<TResult>(Expression<Func<TClientInterface, TResult>> p)
        {
            var methodInfo = ExtractMethodInfos(p);
            var request = CreateRequest(methodInfo);

            var response = await _rawClient.SendRequest<Response>(request).ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<TResult>(response.Result, SerializationOptions.Current);

            return result;
        }

        private Request CreateRequest((string Typename, string Methodname, Dictionary<int, object> Arguments) methodInfo)
        {
            var request = new Request
            {
                Type = methodInfo.Typename,
                Method = methodInfo.Methodname,
                Arguments = JsonConvert.SerializeObject(methodInfo.Arguments, SerializationOptions.Current)
            };
            return request;
        }

        private (string Typename, string Methodname, Dictionary<int, object> Arguments) ExtractMethodInfos<TResult>(
            Expression<Func<TClientInterface, TResult>> p)
        {
            var body = p.Body as MethodCallExpression;

            var methodName = body.Method.Name;
            var typeName = body.Method.DeclaringType.Name;
            var arguments = body.Arguments.Cast<ConstantExpression>().Select((s, i) => new {s, i}).ToDictionary(x => x.i, x => x.s.Value);
            return (typeName, methodName, arguments);
        }

        private (string Typename, string Methodname, Dictionary<int, object> Arguments) ExtractMethodInfos(Expression<Action<TClientInterface>> p)
        {
            var body = p.Body as MethodCallExpression;

            var methodName = body.Method.Name;
            var typeName = body.Method.DeclaringType.Name;
            var arguments = body.Arguments.Cast<ConstantExpression>().Select((s, i) => new {s, i}).ToDictionary(x => x.i, x => x.s.Value);
            return (typeName, methodName, arguments);
        }

        public async Task Execute(Expression<Action<TClientInterface>> p)
        {
            var methodInfo = ExtractMethodInfos(p);
            var request = CreateRequest(methodInfo);

            await _rawClient.SendRequest<Response>(request).ConfigureAwait(false);
        }
    }
}