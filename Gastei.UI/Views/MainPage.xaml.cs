using Gastei.UI.Database;
using Gastei.UI.ViewModels;
using Gastei.UI.Helpers;    

namespace Gastei.UI.Views;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MainPage(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        InitializeComponent();
    }

    private async void OnUsuarioClicked(object sender, EventArgs e)
    {
        // Agora o DatabaseService será injetado automaticamente no UsuarioPage
        await Navigation.PushAsync(new UsuarioPage(_databaseService));
    }
    private async void OnDividasClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DividasPage());
    }

    private async void OnOrcamentoClicked(object sender, EventArgs e)
    {
        var page = ServiceHelper.GetService<OrcamentoPage>();
        await Navigation.PushAsync(page);
    }

    private async void OnRelatorioClicked(object sender, EventArgs e)
    {
        var page = ServiceHelper.GetService<RelatoriosPage>();
        await Navigation.PushAsync(page);
    }
}