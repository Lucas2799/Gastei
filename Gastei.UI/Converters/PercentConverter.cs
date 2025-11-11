using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Gastei.UI.Converters
{
    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0.0;
            double percent = 0.0;

            if (value is double d)
                percent = d / 100.0;
            else if (value is decimal dec)
                percent = (double)(dec / 100m);

            return percent; // ProgressBar espera valor entre 0 e 1
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
