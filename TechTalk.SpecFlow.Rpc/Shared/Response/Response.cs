using System;

namespace TechTalk.SpecFlow.Rpc.Shared.Response
{
    public class Response : IEquatable<Response>
    {
        public string Assembly { get; set; }

        public string Type { get; set; }
        public string Method { get; set; }
        public string Result { get; set; }

        public bool Equals(Response other)
            => ReferenceEquals(this, other)
               || !ReferenceEquals(null, other)
               && string.Equals(Assembly, other.Assembly)
               && string.Equals(Type, other.Type)
               && string.Equals(Method, other.Method)
               && string.Equals(Result, other.Result);

        public override bool Equals(object obj) => obj is Response other && Equals(other);

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            unchecked
            {
                int hashCode = Assembly?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Method?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Result?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
