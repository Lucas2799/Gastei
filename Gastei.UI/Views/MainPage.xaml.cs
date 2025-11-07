using Gastei.UI.Database;

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
}