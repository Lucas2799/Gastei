using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;
using Gastei.UI.Reports;
using static Gastei.Core.Rules.PerfilFinanceiroRules;

namespace Gastei.UI.Services;

public class ReportService : IReportService
{
    private readonly IDividaRepository _dividaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ReportService(IDividaRepository dividaRepository, IUsuarioRepository usuarioRepository)
    {
        _dividaRepository = dividaRepository;
        _usuarioRepository = usuarioRepository;
    }

    // -----------------------------------------
    // 1) Tendência (histórico de gastos mensais)
    // -----------------------------------------
    public async Task<IEnumerable<TrendPoint>> GetTrendAsync(int monthsBack, PerfilFinanceiro perfil)
    {
        var dividas = await _dividaRepository.GetAllAsync();
        var hoje = DateTime.Now;

        var result = new List<TrendPoint>();

        for (int i = monthsBack - 1; i >= 0; i--)
        {
            var data = hoje.AddMonths(-i);
            int mes = data.Month;
            int ano = data.Year;

            var totalMes = dividas
                .Where(d => d.Ativa && d.ReferenciaMes == mes && d.ReferenciaAno == ano)
                .Sum(d => d.Valor);

            result.Add(new TrendPoint(
                data.ToString("MMM yyyy"),
                totalMes,
                mes,
                ano
            ));
        }

        return result;
    }

    // ------------------------------------------------------
    // 2) Crescimento por categoria (mês atual x mês anterior)
    // ------------------------------------------------------
    public async Task<IEnumerable<CategoryGrowth>> GetCategoryGrowthAsync(int monthsBack, PerfilFinanceiro perfil)
    {
        var dividas = await _dividaRepository.GetAllAsync();
        var hoje = DateTime.Now;

        int mesAtual = hoje.Month;
        int anoAtual = hoje.Year;

        var anterior = hoje.AddMonths(-1);
        int mesAnterior = anterior.Month;
        int anoAnterior = anterior.Year;

        var result = new List<CategoryGrowth>();

        foreach (CategoriaDivida categoria in Enum.GetValues(typeof(CategoriaDivida)))
        {
            var totalAtual = dividas
                .Where(d => d.Ativa && d.Categoria == categoria &&
                            d.ReferenciaMes == mesAtual && d.ReferenciaAno == anoAtual)
                .Sum(d => d.Valor);

            var totalAnt = dividas
                .Where(d => d.Ativa && d.Categoria == categoria &&
                            d.ReferenciaMes == mesAnterior && d.ReferenciaAno == anoAnterior)
                .Sum(d => d.Valor);

            decimal crescimento;

            if (totalAnt == 0 && totalAtual > 0)
                crescimento = 100m;
            else if (totalAnt == 0 && totalAtual == 0)
                crescimento = 0;
            else
                crescimento = Math.Round(((totalAtual - totalAnt) / totalAnt) * 100m, 2);

            result.Add(new CategoryGrowth(categoria.ToString(), crescimento));
        }

        return result.OrderByDescending(x => x.CrescimentoPercentual);
    }

    // -------------------------------------------
    // 3) Distribuição Real x Ideal por categoria
    // -------------------------------------------
    public async Task<IEnumerable<DistributionItem>> GetDistributionAsync(PerfilFinanceiro perfil)
    {
        var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
        decimal salario = usuario?.SalarioBase ?? 0;

        var dividas = await _dividaRepository.GetAllAsync();
        var hoje = DateTime.Now;

        var dividasMes = dividas
            .Where(d => d.Ativa && d.ReferenciaMes == hoje.Month && d.ReferenciaAno == hoje.Year)
            .ToList();

        decimal totalMes = dividasMes.Sum(d => d.Valor);

        var idealMap = GetDistribuicaoPorPerfil(perfil);

        var result = new List<DistributionItem>();

        foreach (CategoriaDivida categoria in Enum.GetValues(typeof(CategoriaDivida)))
        {
            decimal totalCat = dividasMes.Where(d => d.Categoria == categoria).Sum(d => d.Valor);

            decimal realPct = totalMes > 0 ? (totalCat / totalMes) : 0;
            decimal idealPct = idealMap.ContainsKey(categoria) ? idealMap[categoria] : 0;

            result.Add(new DistributionItem(
                categoria.ToString(),
                Math.Round(realPct, 4),
                Math.Round(idealPct, 4)
            ));
        }

        return result.OrderByDescending(d => d.RealPercent);
    }

    // -----------------------------
    // 4) Ranking por categoria
    // -----------------------------
    public async Task<IEnumerable<RankingItem>> GetRankingAsync(int month, int year)
    {
        var dividas = await _dividaRepository.GetAllAsync();

        var data = dividas
            .Where(d => d.Ativa && d.ReferenciaMes == month && d.ReferenciaAno == year)
            .GroupBy(d => d.Categoria)
            .Select((g, index) => new RankingItem(
                index + 1,
                g.Key.ToString(),
                g.Sum(x => x.Valor)
            ))
            .OrderByDescending(x => x.Valor)
            .ToList();

        return data;
    }

    // -----------------------------
    // 5) Alertas inteligentes
    // -----------------------------
    public async Task<IEnumerable<AlertMessage>> GetAlertsAsync(int month, int year, PerfilFinanceiro perfil)
    {
        var alerts = new List<AlertMessage>();

        var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
        decimal salario = usuario?.SalarioBase ?? 0;

        var dividas = await _dividaRepository.GetAllAsync();

        var dividasMes = dividas
            .Where(d => d.Ativa && d.ReferenciaMes == month && d.ReferenciaAno == year)
            .ToList();

        // Total do mês
        decimal totalMes = dividasMes.Sum(d => d.Valor);

        // Limite total do perfil
        decimal limiteTotal = Enum.GetValues(typeof(CategoriaDivida))
            .Cast<CategoriaDivida>()
            .Sum(cat => salario * GetLimitePercentual(perfil, cat));

        // Alerta: passo do limite total
        if (limiteTotal > 0)
        {
            var pct = totalMes / limiteTotal;

            if (pct >= 0.80m)
                alerts.Add(new AlertMessage($"Você já usou {pct:P0} do seu limite total."));
        }

        // Alerta: crescimento alto
        var growth = await GetCategoryGrowthAsync(2, perfil);

        foreach (var g in growth.Where(x => x.CrescimentoPercentual > 20))
            alerts.Add(new AlertMessage($"Categoria {g.Categoria} cresceu {g.CrescimentoPercentual}% no mês."));

        return alerts;
    }

    // -----------------------------
    // 6) Resumo anual
    // -----------------------------
    public async Task<AnnualSummary> GetAnnualSummaryAsync(int year)
    {
        var dividas = await _dividaRepository.GetAllAsync();

        var meses = Enumerable.Range(1, 12)
            .Select(m => new
            {
                Mes = m,
                Total = dividas.Where(d => d.Ativa && d.ReferenciaMes == m && d.ReferenciaAno == year).Sum(d => d.Valor)
            })
            .ToList();

        decimal totalAno = meses.Sum(m => m.Total);
        var mesMaior = meses.OrderByDescending(m => m.Total).First();
        var mesMenor = meses.OrderBy(m => m.Total).First();

        return new AnnualSummary(
            totalAno,
            new DateTime(year, mesMaior.Mes, 1).ToString("MMMM"),
            new DateTime(year, mesMenor.Mes, 1).ToString("MMMM")
        );
    }

    // --------------------------------------------
    // 7) Consistência financeira (6 meses)
    // --------------------------------------------
    public async Task<ConsistencySummary> GetConsistencyAsync(int monthsBack, PerfilFinanceiro perfil)
    {
        var dividas = await _dividaRepository.GetAllAsync();
        var hoje = DateTime.Now;

        var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
        decimal salario = usuario?.SalarioBase ?? 0;

        int dentro = 0;
        int acima = 0;

        for (int i = 0; i < monthsBack; i++)
        {
            var data = hoje.AddMonths(-i);

            var dividasMes = dividas
                .Where(d => d.Ativa &&
                            d.ReferenciaMes == data.Month &&
                            d.ReferenciaAno == data.Year)
                .ToList();

            decimal totalMes = dividasMes.Sum(d => d.Valor);

            decimal limiteMensal = Enum
                .GetValues(typeof(CategoriaDivida))
                .Cast<CategoriaDivida>()
                .Sum(cat => salario * GetLimitePercentual(perfil, cat));

            if (totalMes <= limiteMensal)
                dentro++;
            else
                acima++;
        }

        return new ConsistencySummary(dentro, acima);
    }
}
