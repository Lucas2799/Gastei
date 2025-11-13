namespace Gastei.UI.Reports;

public record TrendPoint(string MesAno, decimal Valor, int Mes, int Ano);

public record CategoryGrowth(string Categoria, decimal CrescimentoPercentual);

public record DistributionItem(string Categoria, decimal RealPercent, decimal IdealPercent);

public record RankingItem(int Posicao, string Categoria, decimal Valor);

public record AlertMessage(string Mensagem);

public record AnnualSummary(decimal TotalAno, string MesMaisCaro, string MesMaisBarato);

public record ConsistencySummary(int MesesDentroLimite, int MesesAcimaLimite);
