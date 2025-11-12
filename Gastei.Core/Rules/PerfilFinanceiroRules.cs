using Gastei.Core.Enums;

namespace Gastei.Core.Rules;

public static class PerfilFinanceiroRules
{
    // Percentuais por bucket (valores em decimal: 0.5 = 50%)
    private static readonly Dictionary<PerfilFinanceiro, (decimal Essenciais, decimal Investimentos, decimal Lazer)> BucketPercents
        = new()
        {
            [PerfilFinanceiro.Equilibrado] = (Essenciais: 0.50m, Investimentos: 0.20m, Lazer: 0.30m),
            [PerfilFinanceiro.Investidor] = (Essenciais: 0.50m, Investimentos: 0.40m, Lazer: 0.10m),
            [PerfilFinanceiro.Conservador] = (Essenciais: 0.60m, Investimentos: 0.10m, Lazer: 0.30m)
        };

    // Mapeamento de categorias para buckets
    private static readonly Dictionary<string, string> CategoriaParaBucket = new()
    {
        // Essenciais
        ["Moradia"] = "Essenciais",
        ["Alimentacao"] = "Essenciais",
        ["Transporte"] = "Essenciais",
        ["Saude"] = "Essenciais",

        // Investimentos (aqui incluí Educacao / Investimento / Emergencias)
        ["Educacao"] = "Investimentos",
        ["Investimento"] = "Investimentos",
        ["Emergencias"] = "Investimentos",

        // Lazer
        ["Lazer"] = "Lazer",
        ["Compras"] = "Lazer",
        ["Outros"] = "Lazer",
        ["Dividas"] = "Lazer"
    };

    // Se você usar enums (recomendado), mapeie CategoriaDivida -> string nome:
    public static string BucketOf(CategoriaDivida categoria)
    {
        var key = categoria.ToString();
        if (CategoriaParaBucket.TryGetValue(key, out var bucket))
            return bucket;

        return "Lazer"; // fallback seguro — ou "Outros"
    }

    // Retorna dicionário Categoria -> percentual (soma aproximada 1.0)
    public static Dictionary<CategoriaDivida, decimal> GetDistribuicaoPorPerfil(PerfilFinanceiro perfil)
    {
        if (!BucketPercents.TryGetValue(perfil, out var buckets))
            throw new ArgumentException("Perfil desconhecido", nameof(perfil));

        // Agrupa categorias por bucket usando todos os valores do enum CategoriaDivida
        var todasCategorias = Enum.GetValues(typeof(CategoriaDivida))
                                 .Cast<CategoriaDivida>()
                                 .ToList();

        var bucketGroups = new Dictionary<string, List<CategoriaDivida>>();
        foreach (var cat in todasCategorias)
        {
            var bucket = BucketOf(cat);
            if (!bucketGroups.ContainsKey(bucket))
                bucketGroups[bucket] = new List<CategoriaDivida>();

            bucketGroups[bucket].Add(cat);
        }

        // Percentual disponível por bucket
        var bucketPercentMap = new Dictionary<string, decimal>
        {
            ["Essenciais"] = buckets.Essenciais,
            ["Investimentos"] = buckets.Investimentos,
            ["Lazer"] = buckets.Lazer
        };

        var result = new Dictionary<CategoriaDivida, decimal>();
        decimal acumulado = 0m;

        // Para cada bucket, divide igualmente entre categorias
        foreach (var kv in bucketGroups)
        {
            var bucketName = kv.Key;
            var categoriasNoBucket = kv.Value;
            var count = categoriasNoBucket.Count;
            var bucketPercent = bucketPercentMap.ContainsKey(bucketName) ? bucketPercentMap[bucketName] : 0m;

            if (count == 0)
                continue;

            // divisão base
            var baseShare = Math.Round(bucketPercent / count, 6); // precisão razoável
            foreach (var cat in categoriasNoBucket)
            {
                result[cat] = baseShare;
                acumulado += baseShare;
            }
        }

        // Ajuste pelo resto causado pelo arredondamento: distribuir diferença proporcionalmente
        var diferenca = Math.Round(1.0m - acumulado, 6);
        if (Math.Abs(diferenca) > 0m)
        {
            // distribui o resto nas primeiras categorias (pode ajustar estratégia)
            var keys = result.Keys.ToList();
            var idx = 0;
            while (Math.Abs(diferenca) > 0m)
            {
                var k = keys[idx % keys.Count];
                // incrementa/subtrai pequenos passos para compensar
                var passo = Math.Sign(diferenca) * 0.000001m;
                result[k] += passo;
                diferenca -= passo;
                idx++;
                if (idx > keys.Count * 10) break; // safety
            }
        }

        return result;
    }

    // Helper para pegar apenas um limite
    public static decimal GetLimitePercentual(PerfilFinanceiro perfil, CategoriaDivida categoria)
    {
        var map = GetDistribuicaoPorPerfil(perfil);
        return map.TryGetValue(categoria, out var pct) ? pct : 0m;
    }
}