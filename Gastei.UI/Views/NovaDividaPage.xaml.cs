using Gastei.UI.Database;
using Gastei.UI.ViewModels;

namespace Gastei.UI.Views;

public partial class NovaDividaPage : ContentPage
{
    public NovaDividaPage()
    {
        try
        {
            InitializeComponent();

            var databaseService = GetDatabaseService();
            if (databaseService != null)
            {
                var dividaRepository = new DividaRepository(databaseService);
                BindingContext = new NovaDividaViewModel(dividaRepository);
            }
            else
            {
                DisplayAlert("Erro", "Não foi possível acessar o banco de dados", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao carregar página: {ex.Message}", "OK");
        }
    }

    private DatabaseService GetDatabaseService()
    {
        try
        {
            // Tentativa via MauiContext (padrão preferido)
            if (Handler?.MauiContext?.Services != null)
            {
                var service = Handler.MauiContext.Services.GetService<DatabaseService>();
                if (service != null) return service;
            }

            // Tentativa via Application.Current
            if (Application.Current?.Handler?.MauiContext?.Services != null)
            {
                var service = Application.Current.Handler.MauiContext.Services.GetService<DatabaseService>();
                if (service != null) return service;
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao obter DatabaseService: {ex.Message}");
            return null;
        }
    }
}
