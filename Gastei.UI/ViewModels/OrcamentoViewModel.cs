using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Entities;
using Gastei.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gastei.UI.ViewModels;

public partial class OrcamentoViewModel : BaseViewModel
{
    private readonly IDividaRepository _dividaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    [ObservableProperty] private decimal salarioBase;
    [ObservableProperty] private decimal totalGastos;
    [ObservableProperty] private decimal saldoDisponivel;
    [ObservableProperty] private decimal totalProximoMes;
    [ObservableProperty] private ObservableCollection<CategoriaResumo> resumoCategorias = new();
    [ObservableProperty] private string mesSelecionado = DateTime.Now.ToString("MMMM yyyy");

    public OrcamentoViewModel(IDividaRepository dividaRepository, IUsuarioRepository usuarioRepository)
    {
        _dividaRepository = dividaRepository;
        _usuarioRepository = usuarioRepository;
        Title = "Orçamento";

        _ = CarregarOrcamentoAsync();
    }

    [RelayCommand]
    private async Task CarregarOrcamentoAsync()
    {
        IsBusy = true;
        try
        {
            var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
            SalarioBase = usuario?.SalarioBase ?? 0;

            var dividas = await _dividaRepository.GetAllAsync();

            var dividasAtivas = dividas.Where(d => d.Ativa).ToList();

            TotalGastos = dividasAtivas.Sum(d => d.Valor);
            TotalProximoMes = dividasAtivas
                .Where(d => d.ValorEstimadoProximoMes.HasValue)
                .Sum(d => d.ValorEstimadoProximoMes!.Value);

            SaldoDisponivel = SalarioBase - TotalGastos;

            var agrupado = dividasAtivas
                .GroupBy(d => string.IsNullOrWhiteSpace(d.Categoria) ? "Outros" : d.Categoria)
                .Select(g => new CategoriaResumo
                {
                    Categoria = g.Key,
                    Total = g.Sum(x => x.Valor),
                    Percentual = SalarioBase > 0 ? ((double)(g.Sum(x => x.Valor) / SalarioBase) * 100) : 0
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            ResumoCategorias = new ObservableCollection<CategoriaResumo>(agrupado);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public class CategoriaResumo
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public double Percentual { get; set; }
}
