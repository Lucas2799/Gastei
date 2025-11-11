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
        builder.Services.AddSingleton<IDividaRepository, DividaRepository>();
        // ViewModels
        builder.Services.AddTransient<DividaViewModel>();
        builder.Services.AddTransient<NovaDividaViewModel>();
        builder.Services.AddTransient<DividasPage>();
        builder.Services.AddTransient<NovaDividaPage>();
        builder.Services.AddTransient<UsuarioViewModel>();
        builder.Services.AddTransient<DividaViewModel>();
        builder.Services.AddTransient<NovaDividaViewModel>();
        builder.Services.AddTransient<OrcamentoViewModel>();

        // Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<UsuarioPage>();
        builder.Services.AddTransient<DividasPage>();
        builder.Services.AddTransient<OrcamentoPage>();

        return builder.Build();
    }
}