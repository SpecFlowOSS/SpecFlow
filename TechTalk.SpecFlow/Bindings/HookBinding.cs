using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class HookBinding : MethodBinding, IHookBinding
    {
        public HookType HookType { get; set; }
        public int HookOrder { get; private set; }
        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public HookBinding(IBindingMethod bindingMethod, HookType hookType, BindingScope bindingScope, int hookOrder) : base(bindingMethod)
        {
            HookOrder = hookOrder;
            HookType = hookType;
            BindingScope = bindingScope;
        }

        protected bool Equals(HookBinding other)
        {
            return HookType == other.HookType && HookOrder == other.HookOrder && Equals(BindingScope, other.BindingScope) && base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HookBinding) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) HookType;
                hashCode = (hashCode*397) ^ HookOrder;
                hashCode = (hashCode*397) ^ (BindingScope != null ? BindingScope.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}