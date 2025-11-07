using Gastei.UI.Views;

namespace Gastei.UI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rotas de navegação
        Routing.RegisterRoute(nameof(UsuarioPage), typeof(UsuarioPage));
        Routing.RegisterRoute(nameof(DividasPage), typeof(DividasPage));
        Routing.RegisterRoute(nameof(NovaDividaPage), typeof(NovaDividaPage));
    }
}