using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace TheEggOfFortune.ViewLogic
{
    internal class IntToStyledStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return i.ToString("N0", culture);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
