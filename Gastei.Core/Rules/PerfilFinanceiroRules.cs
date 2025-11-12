using Gastei.Core.Enums;

namespace Gastei.Core.Rules;

public static class PerfilFinanceiroRules
{
    // Percentual máximo permitido por categoria conforme o perfil
    // Valores expressos em frações (ex: 0.3 = 30%)
    public static Dictionary<PerfilFinanceiro, Dictionary<CategoriaDivida, decimal>> LimitesPorPerfil
        = new()
        {
            [PerfilFinanceiro.Equilibrado] = new()
            {
                // Essenciais (50% / 4 categorias = 12,5% cada)
                { CategoriaDivida.Moradia, 0.125m },
                { CategoriaDivida.Alimentacao, 0.125m },
                { CategoriaDivida.Transporte, 0.125m },
                { CategoriaDivida.Saude, 0.125m },

                // Investimentos (20% / 3 = 6,67% cada)
                { CategoriaDivida.Educacao, 0.0667m },
                { CategoriaDivida.Investimento, 0.0667m },
                { CategoriaDivida.Emergencias, 0.0667m },

                // Lazer (30% / 4 = 7,5% cada)
                { CategoriaDivida.Lazer, 0.075m },
                { CategoriaDivida.Compras, 0.075m },
                { CategoriaDivida.Outros, 0.075m },
                { CategoriaDivida.Dividas, 0.075m }
            },

            [PerfilFinanceiro.Investidor] = new()
            {
                // Essenciais (50% / 4 = 12,5%)
                { CategoriaDivida.Moradia, 0.125m },
                { CategoriaDivida.Alimentacao, 0.125m },
                { CategoriaDivida.Transporte, 0.125m },
                { CategoriaDivida.Saude, 0.125m },

                // Investimentos (40% / 3 = 13,33%)
                { CategoriaDivida.Educacao, 0.1333m },
                { CategoriaDivida.Investimento, 0.1333m },
                { CategoriaDivida.Emergencias, 0.1333m },

                // Lazer (10% / 4 = 2,5%)
                { CategoriaDivida.Lazer, 0.025m },
                { CategoriaDivida.Compras, 0.025m },
                { CategoriaDivida.Outros, 0.025m },
                { CategoriaDivida.Dividas, 0.025m }
            },

            [PerfilFinanceiro.Conservador] = new()
            {
                // Essenciais (60% / 4 = 15%)
                { CategoriaDivida.Moradia, 0.15m },
                { CategoriaDivida.Alimentacao, 0.15m },
                { CategoriaDivida.Transporte, 0.15m },
                { CategoriaDivida.Saude, 0.15m },

                // Investimentos (10% / 3 = 3,33%)
                { CategoriaDivida.Educacao, 0.0333m },
                { CategoriaDivida.Investimento, 0.0333m },
                { CategoriaDivida.Emergencias, 0.0333m },

                // Lazer (30% / 4 = 7,5%)
                { CategoriaDivida.Lazer, 0.075m },
                { CategoriaDivida.Compras, 0.075m },
                { CategoriaDivida.Outros, 0.075m },
                { CategoriaDivida.Dividas, 0.075m }
            }
        };

    public static decimal GetLimitePercentual(PerfilFinanceiro perfil, CategoriaDivida categoria)
    {
        if (LimitesPorPerfil.ContainsKey(perfil) && LimitesPorPerfil[perfil].ContainsKey(categoria))
            return LimitesPorPerfil[perfil][categoria];

        // fallback: 10%
        return 0.10m;
    }
}
