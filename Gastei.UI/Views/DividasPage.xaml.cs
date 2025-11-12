using Gastei.UI.Database;
using Gastei.UI.ViewModels;

namespace Gastei.UI.Views;

public partial class DividasPage : ContentPage
{
    public DividasPage()
    {
        InitializeComponent();

        var databaseService = GetDatabaseService();
        if (databaseService != null)
        {
            var dividaRepository = new DividaRepository(databaseService);
            BindingContext = new DividaViewModel(dividaRepository);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DividaViewModel vm && !vm.Dividas.Any())
            await vm.CarregarDividasAsync();
    }

    private DatabaseService GetDatabaseService()
    {
        return Application.Current?.Handler?.MauiContext?.Services?.GetService<DatabaseService>();
    }
}
