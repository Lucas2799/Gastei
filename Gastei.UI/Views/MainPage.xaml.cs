namespace Gastei.UI.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        DisplayAlert("Gastei", "Aplicativo iniciado com sucesso!", "OK");
    }
}