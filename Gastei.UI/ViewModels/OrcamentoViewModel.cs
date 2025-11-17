using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using static Gastei.Core.Rules.PerfilFinanceiroRules;

namespace Gastei.UI.ViewModels;

public partial class OrcamentoViewModel : BaseViewModel
{
    private readonly IDividaRepository _dividaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    // --------------------------
    // PROPRIEDADES OBSERVÁVEIS
    // --------------------------
    [ObservableProperty] private decimal salarioBase;
    [ObservableProperty] private decimal totalGastos;
    [ObservableProperty] private decimal saldoDisponivel;
    [ObservableProperty] private decimal totalProximoMes;
    [ObservableProperty] private ObservableCollection<CategoriaResumo> resumoCategorias = new();
    [ObservableProperty] private string mesSelecionado = DateTime.Now.ToString("MMMM yyyy");
    [ObservableProperty] private PerfilFinanceiro perfilSelecionado;
    [ObservableProperty] private int mesReferencia = DateTime.Now.Month;
    [ObservableProperty] private int anoReferencia = DateTime.Now.Year;


    // 🔥 Calcula o limite total dinâmico com base em todas as categorias
    public decimal LimiteTotal
    {
        get
        {
            if (SalarioBase <= 0) return 0;

            decimal limite = 0;

            // Todas as categorias definidas no Enum
            var categorias = Enum.GetValues(typeof(CategoriaDivida))
                                 .Cast<CategoriaDivida>();

            foreach (var categoria in categorias)
            {
                var pct = GetLimitePercentual(PerfilSelecionado, categoria);
                limite += SalarioBase * pct;
            }

            return limite;
        }
    }

    // Progresso total (para ProgressBar)
    public double PercentualTotal =>
        LimiteTotal <= 0 ? 0 : (double)(TotalGastos / LimiteTotal);

    public decimal GastoRestante => LimiteTotal - TotalGastos;

    public string UsoTotalFormatado => $"{PercentualTotal:P0} do limite utilizado";


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
            // --------------------------
            // CARREGAR USUÁRIO
            // --------------------------
            var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();

            SalarioBase = usuario?.SalarioBase ?? 0;
            PerfilSelecionado = usuario?.Perfil ?? PerfilFinanceiro.Equilibrado;

            // --------------------------
            // CARREGAR DÍVIDAS
            // --------------------------
            var dividas = await _dividaRepository.GetDividasPorMesAsync(mesReferencia,anoReferencia);
            var dividasAtivas = dividas.Where(d => d.Ativa).ToList();

            TotalGastos = dividasAtivas.Sum(d => d.Valor);
            TotalProximoMes = dividasAtivas
                .Where(d => d.ValorEstimadoProximoMes.HasValue)
                .Sum(d => d.ValorEstimadoProximoMes!.Value);

            SaldoDisponivel = SalarioBase - TotalGastos;


            // --------------------------
            // AGRUPAR POR CATEGORIA
            // E CALCULAR PERCENTUAL
            // --------------------------
            var categorias = dividasAtivas
                .GroupBy(d => d.Categoria)
                .Select(g =>
                {
                    var categoria = g.Key;
                    var totalCategoria = g.Sum(x => x.Valor);
                    var pctCategoria = GetLimitePercentual(PerfilSelecionado, categoria);
                    var limiteCategoria = SalarioBase * pctCategoria;

                    return new CategoriaResumo
                    {
                        Categoria = categoria.ToString(),
                        Total = totalCategoria,
                        Percentual = limiteCategoria > 0
                            ? (double)(totalCategoria / limiteCategoria)
                            : 0
                    };
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            ResumoCategorias = new ObservableCollection<CategoriaResumo>(categorias);

            // Atualiza a UI
            OnPropertyChanged(nameof(LimiteTotal));
            OnPropertyChanged(nameof(PercentualTotal));
            OnPropertyChanged(nameof(GastoRestante));
            OnPropertyChanged(nameof(UsoTotalFormatado));
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
    public double Percentual { get; set; } // progressbar 0–1
}
