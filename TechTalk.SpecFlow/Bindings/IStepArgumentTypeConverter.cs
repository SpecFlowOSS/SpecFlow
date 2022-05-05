using System.Globalization;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepArgumentTypeConverter
    {
        Task<object> ConvertAsync(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo);
        bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo);
    }
}
