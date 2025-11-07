using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;
using System.Collections.ObjectModel;

namespace Gastei.UI.ViewModels;

public partial class DividaViewModel : BaseViewModel
{
    private readonly IDividaRepository _dividaRepository;

    [ObservableProperty]
    private ObservableCollection<Divida> _dividas = new();

    [ObservableProperty]
    private decimal _totalDividas;

    [ObservableProperty]
    private bool _mostrarSomenteAtivas = true;

    public DividaViewModel(IDividaRepository dividaRepository)
    {
        _dividaRepository = dividaRepository;
        Title = "Dívidas";
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var lista = _mostrarSomenteAtivas
                ? await _dividaRepository.GetDividasAtivasAsync()
                : await _dividaRepository.GetAllAsync();

            Dividas = new ObservableCollection<Divida>(lista);
            TotalDividas = lista.Sum(x => x.Valor);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NovaDividaAsync()
    {
        await Shell.Current.GoToAsync("NovaDividaPage");
    }

    [RelayCommand]
    private async Task EditarDividaAsync(Divida divida)
    {
        var parametros = new Dictionary<string, object> { { "DividaSelecionada", divida } };
        await Shell.Current.GoToAsync("NovaDividaPage", parametros);
    }

    [RelayCommand]
    private async Task AlternarStatusDividaAsync(Divida divida)
    {
        divida.Ativa = !divida.Ativa;
        await _dividaRepository.UpdateAsync(divida);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task AlternarFiltroAsync()
    {
        MostrarSomenteAtivas = !MostrarSomenteAtivas;
        await LoadAsync();
    }
}
