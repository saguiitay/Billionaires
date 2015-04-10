using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Billionaires.Converters
{
    public class HiddenWhenEmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            return (str == null) || string.IsNullOrEmpty(str)
                       ? Visibility.Collapsed
                       : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}