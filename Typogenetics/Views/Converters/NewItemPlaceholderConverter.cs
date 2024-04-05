using System;
using System.Globalization;
using System.Windows.Data;


namespace Typogenetics.Views.Converters
{
    public class NewItemPlaceholderConverter : IValueConverter
    {
        private object _placeholder;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? _placeholder;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == "{NewItemPlaceholder}")
            {
                _placeholder = value;
                return null;
            }
            return value;
        }
    }
}
