
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ReservasBarberia.Converters
{
    public class InicialConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is string nombre &&
                !string.IsNullOrWhiteSpace(nombre))
            {
                return nombre.Trim().Substring(0, 1).ToUpper();
            }

            return "?";
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
