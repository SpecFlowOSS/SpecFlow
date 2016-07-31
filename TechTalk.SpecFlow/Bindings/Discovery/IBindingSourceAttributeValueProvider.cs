namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public interface IBindingSourceAttributeValueProvider
    {
        TValue GetValue<TValue>();
    }

    public class BindingSourceAttributeValueProvider : IBindingSourceAttributeValueProvider
    {
        private readonly object value;

        public BindingSourceAttributeValueProvider(object value)
        {
            this.value = value;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)value;
        }
    }
}