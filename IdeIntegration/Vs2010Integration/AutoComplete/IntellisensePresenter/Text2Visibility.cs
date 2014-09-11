using System;
using System.Globalization;
using System.Windows.Data;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete.IntellisensePresenter
{
    public sealed class Text2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrWhiteSpace((string)value))
                return "Visible";
            
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}