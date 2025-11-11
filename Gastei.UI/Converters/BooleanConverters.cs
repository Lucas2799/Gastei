using System.Globalization;

namespace Gastei.UI.Converters;

public class BooleanToStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (bool)(value ?? false) ? "✅ Ativa" : "❌ Inativa";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BooleanToToggleTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (bool)(value ?? false) ? "Desativar" : "Ativar";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => !(bool)(value ?? false);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => !(bool)(value ?? false);
}

