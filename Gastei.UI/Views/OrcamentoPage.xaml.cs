using Gastei.UI.ViewModels;

namespace Gastei.UI.Views;

public partial class OrcamentoPage : ContentPage
{
    public OrcamentoPage(OrcamentoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
