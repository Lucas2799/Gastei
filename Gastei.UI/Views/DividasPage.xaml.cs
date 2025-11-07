using Gastei.UI.Database;

namespace Gastei.UI.Views;

public partial class DividasPage : ContentPage
{
    public DividasPage()
    {
        try
        {
            InitializeComponent();

            // Obter serviços manualmente
            var databaseService = GetDatabaseService();
            if (databaseService != null)
            {
                var dividaRepository = new DividaRepository(databaseService);
                BindingContext = new ViewModels.DividaViewModel(dividaRepository);
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
            // Tentar obter via MauiContext
            if (Handler?.MauiContext?.Services != null)
            {
                var service = Handler.MauiContext.Services.GetService<DatabaseService>();
                if (service != null) return service;
            }

            // Tentar obter via Application.Current
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