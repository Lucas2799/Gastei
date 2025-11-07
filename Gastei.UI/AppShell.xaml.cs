using Gastei.UI.Views;

namespace Gastei.UI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(UsuarioPage), typeof(UsuarioPage));
    }
}


