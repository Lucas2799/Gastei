using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Enums;
using Gastei.UI.Services;
using Gastei.UI.Reports;
using System.Collections.ObjectModel;

namespace Gastei.UI.ViewModels;

public partial class RelatoriosViewModel : BaseViewModel
{
    private readonly IReportService _reportService;

    [ObservableProperty] private ObservableCollection<TrendPoint> graficoTendencia = new();
    [ObservableProperty] private ObservableCollection<CategoryGrowth> crescimentoCategorias = new();
    [ObservableProperty] private ObservableCollection<DistributionItem> distribuicaoCategorias = new();
    [ObservableProperty] private ObservableCollection<RankingItem> rankingCategorias = new();
    [ObservableProperty] private ObservableCollection<string> alertas = new();

    [ObservableProperty] private decimal totalAno;
    [ObservableProperty] private string mesMaisCaro;
    [ObservableProperty] private string mesMaisBarato;

    [ObservableProperty] private string mesesDentroLimite;
    [ObservableProperty] private string mesesAcimaLimite;

    [ObservableProperty] private List<string> meses;
    [ObservableProperty] private List<int> anos;

    [ObservableProperty] private string mesSelecionado;
    [ObservableProperty] private int anoSelecionado;

    public RelatoriosViewModel(IReportService reportService)
    {
        _reportService = reportService;
        Title = "Relatórios";

        Meses = Enumerable.Range(1, 12).Select(m => new DateTime(1, m, 1).ToString("MMMM")).ToList();
        Anos = Enumerable.Range(DateTime.Now.Year - 5, 6).ToList();

        MesSelecionado = DateTime.Now.ToString("MMMM");
        AnoSelecionado = DateTime.Now.Year;

        _ = CarregarTudoAsync();
    }

    [RelayCommand]
    public async Task CarregarTudoAsync()
    {
        IsBusy = true;
        try
        {
            var perfil = PerfilFinanceiro.Equilibrado; // você pode ligar isso ao usuário

            GraficoTendencia = new ObservableCollection<TrendPoint>(
                await _reportService.GetTrendAsync(6, perfil));

            CrescimentoCategorias = new ObservableCollection<CategoryGrowth>(
                await _reportService.GetCategoryGrowthAsync(2, perfil));

            DistribuicaoCategorias = new ObservableCollection<DistributionItem>(
                await _reportService.GetDistributionAsync(perfil));

            RankingCategorias = new ObservableCollection<RankingItem>(
                await _reportService.GetRankingAsync(DateTime.Now.Month, DateTime.Now.Year));

            Alertas = new ObservableCollection<string>(
                (await _reportService.GetAlertsAsync(DateTime.Now.Month, DateTime.Now.Year, perfil))
                .Select(a => a.Mensagem));

            var annual = await _reportService.GetAnnualSummaryAsync(AnoSelecionado);
            TotalAno = annual.TotalAno;
            MesMaisCaro = "Mês mais caro: " + annual.MesMaisCaro;
            MesMaisBarato = "Mês mais barato: " + annual.MesMaisBarato;

        }
        finally
        {
            IsBusy = false;
        }
    }
}
