using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;

namespace Gastei.UI.ViewModels;

[QueryProperty(nameof(DividaSelecionada), "DividaSelecionada")]
public partial class NovaDividaViewModel : BaseViewModel
{
    private readonly IDividaRepository _dividaRepository;

    [ObservableProperty]
    private Divida _dividaSelecionada = new();

    public List<TipoDivida> TiposDisponiveis { get; } =
        Enum.GetValues(typeof(TipoDivida)).Cast<TipoDivida>().ToList();

    public NovaDividaViewModel(IDividaRepository dividaRepository)
    {
        _dividaRepository = dividaRepository;
        Title = "Nova Dívida";
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(DividaSelecionada.Descricao) || DividaSelecionada.Valor <= 0)
        {
            await Shell.Current.DisplayAlert("Erro", "Preencha todos os campos obrigatórios", "OK");
            return;
        }

        if (DividaSelecionada.Id == 0)
            await _dividaRepository.InsertAsync(DividaSelecionada);
        else
            await _dividaRepository.UpdateAsync(DividaSelecionada);

        await Shell.Current.DisplayAlert("Sucesso", "Dívida salva com sucesso!", "OK");
        await Shell.Current.GoToAsync("..");
    }
}
