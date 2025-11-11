using System.Globalization;
using Microsoft.Maui.Graphics;

namespace Gastei.UI.Converters;

public class CategoriaToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string categoria)
            return Colors.Gray;

        return categoria.ToLower() switch
        {
            "moradia" => Color.FromArgb("#1E88E5"),       // Azul
            "automóvel" or "carro" => Color.FromArgb("#43A047"), // Verde
            "alimentação" => Color.FromArgb("#FB8C00"),   // Laranja
            "lazer" => Color.FromArgb("#8E24AA"),         // Roxo
            "educação" => Color.FromArgb("#3949AB"),      // Azul escuro
            "investimento" => Color.FromArgb("#00ACC1"),  // Azul claro
            "saúde" => Color.FromArgb("#E53935"),         // Vermelho
            _ => Color.FromArgb("#757575"),               // Cinza padrão
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
