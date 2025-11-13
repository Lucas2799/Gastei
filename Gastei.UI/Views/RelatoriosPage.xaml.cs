using Gastei.UI.ViewModels;

namespace Gastei.UI.Views;

public partial class RelatoriosPage : ContentPage
{
    public RelatoriosPage(RelatoriosViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}