using Gastei.UI.ViewModels;
using Gastei.UI.Views;
using Gastei.UI.Database;
using Gastei.Core.Interfaces;

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

        // Repositories
        builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();

        // ViewModels
        builder.Services.AddTransient<UsuarioViewModel>();

        // Views
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}