using Gastei.Core.Enums;
using Gastei.UI.Reports;

namespace Gastei.UI.Services;

public interface IReportService
{
    /// <summary>
    /// Retorna histórico de gastos (tendência) dos últimos N meses.
    /// </summary>
    Task<IEnumerable<TrendPoint>> GetTrendAsync(int monthsBack, PerfilFinanceiro perfil);

    /// <summary>
    /// Retorna crescimento por categoria (mês atual x mês anterior).
    /// </summary>
    Task<IEnumerable<CategoryGrowth>> GetCategoryGrowthAsync(int monthsBack, PerfilFinanceiro perfil);

    /// <summary>
    /// Retorna a distribuição real x ideal das categorias (percentuais).
    /// </summary>
    Task<IEnumerable<DistributionItem>> GetDistributionAsync(PerfilFinanceiro perfil);

    /// <summary>
    /// Retorna ranking de gastos por categoria.
    /// </summary>
    Task<IEnumerable<RankingItem>> GetRankingAsync(int month, int year);

    /// <summary>
    /// Retorna alertas inteligentes (ultrapassou limites, crescimento alto, etc).
    /// </summary>
    Task<IEnumerable<AlertMessage>> GetAlertsAsync(int month, int year, PerfilFinanceiro perfil);

    /// <summary>
    /// Retorna resumo anual completo.
    /// </summary>
    Task<AnnualSummary> GetAnnualSummaryAsync(int year);

    /// <summary>
    /// Retorna consistência financeira (meses dentro/acima do limite).
    /// </summary>
    Task<ConsistencySummary> GetConsistencyAsync(int monthsBack, PerfilFinanceiro perfil);
}
