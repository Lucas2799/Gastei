using Gastei.UI.Views;
using Gastei.UI.Database;

namespace Gastei.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<DatabaseService>();

        // Views (com injeção de dependência)
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<UsuarioPage>();

        return builder.Build();
    }
}