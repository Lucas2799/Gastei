using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Entities;
using Gastei.Core.Interfaces;
using System.Collections.ObjectModel;

namespace Gastei.UI.ViewModels;

public partial class DividaViewModel : BaseViewModel
{
    private readonly IDividaRepository _dividaRepository;

    [ObservableProperty]
    private ObservableCollection<Divida> dividas = new();

    [ObservableProperty]
    private decimal totalDividas;

    [ObservableProperty]
    private bool mostrarSomenteAtivas = true;

    // ✅ Adicionados os campos reativos
    [ObservableProperty]
    private bool carregandoMais;

    [ObservableProperty]
    private bool temMaisItens = true;

    private const int LOTE_TAMANHO = 15;
    private int _paginaAtual = 0;
    private List<Divida> _todasDividas = new();

    public IRelayCommand CarregarMaisCommand { get; }

    public DividaViewModel(IDividaRepository dividaRepository)
    {
        _dividaRepository = dividaRepository;
        CarregarMaisCommand = new AsyncRelayCommand(CarregarMaisAsync);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var lista = mostrarSomenteAtivas
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

    public async Task CarregarDividasAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            _paginaAtual = 0;
            TemMaisItens = true;
            _todasDividas = await _dividaRepository.GetDividasAtivasAsync();

            foreach (var d in _todasDividas)
            {
                d.Status = d.Ativa ? "✅ Ativa" : "❌ Inativa";
                d.StatusCor = d.Ativa ? Colors.Green : Colors.Red;
                d.CategoriaCor = d.Categoria switch
                {
                    "Moradia" => Color.FromArgb("#1976D2"),
                    "Lazer" => Color.FromArgb("#E91E63"),
                    "Alimentação" => Color.FromArgb("#4CAF50"),
                    "Transporte" => Color.FromArgb("#FF9800"),
                    _ => Color.FromArgb("#9E9E9E")
                };
            }

            // Carrega o primeiro lote
            var primeiroLote = _todasDividas.Take(LOTE_TAMANHO).ToList();
            Dividas = new ObservableCollection<Divida>(primeiroLote);
            _paginaAtual++;

            TotalDividas = Dividas.Sum(d => d.Valor);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CarregarMaisAsync()
    {
        if (CarregandoMais || !TemMaisItens) return;

        CarregandoMais = true;
        await Task.Delay(300); // pequena pausa assíncrona

        var inicio = _paginaAtual * LOTE_TAMANHO;
        var lote = _todasDividas.Skip(inicio).Take(LOTE_TAMANHO).ToList();

        if (lote.Any())
        {
            foreach (var d in lote)
                Dividas.Add(d);

            _paginaAtual++;
        }
        else
        {
            TemMaisItens = false;
        }

        CarregandoMais = false;
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
