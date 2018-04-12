using System;

namespace TechTalk.SpecFlow.Rpc.Shared.Request
{
    public class Request : IEquatable<Request>
    {
        public string Assembly { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public string Arguments { get; set; }
        public bool IsShutDown { get; set; }
        public bool IsPing { get; set; }

        public bool Equals(Request other)
            => ReferenceEquals(this, other)
               || !ReferenceEquals(null, other)
               && string.Equals(Assembly, other.Assembly)
               && string.Equals(Type, other.Type)
               && string.Equals(Method, other.Method)
               && string.Equals(Arguments, other.Arguments)
               && IsShutDown == other.IsShutDown
               && IsPing == other.IsPing;

        public override bool Equals(object obj) => obj is Request other && Equals(other);

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            unchecked
            {
                int hashCode = (Assembly?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Method?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Arguments?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ IsShutDown.GetHashCode();
                hashCode = (hashCode * 397) ^ IsPing.GetHashCode();
                return hashCode;
            }
        }
    }
}
