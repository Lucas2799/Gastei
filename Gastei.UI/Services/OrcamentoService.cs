using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;// Gastei/Services/OrcamentoService.cs
using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;

namespace Gastei.Services
{
    public class OrcamentoService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IDividaRepository _dividaRepository;

        public OrcamentoService(IUsuarioRepository usuarioRepository, IDividaRepository dividaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _dividaRepository = dividaRepository;
        }

        public async Task<OrcamentoMensal> CalcularOrcamentoMensalAsync(int mes, int ano)
        {
            var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
            var dividas = await _dividaRepository.GetDividasAtivasAsync();

            var salarioTotal = usuario.SalarioBase;
            var totalDividas = dividas.Sum(d => d.Valor);

            var distribuicao = CalcularDistribuicao(usuario.Perfil, salarioTotal);

            return new OrcamentoMensal
            {
                Mes = mes,
                Ano = ano,
                SalarioTotal = salarioTotal,
                TotalDividas = totalDividas,
                SaldoDisponivel = salarioTotal - totalDividas,
                Distribuicao = distribuicao,
                DataCriacao = DateTime.Now
            };
        }

        private Dictionary<CategoriaGasto, decimal> CalcularDistribuicao(PerfilFinanceiro perfil, decimal salarioTotal)
        {
            return perfil switch
            {
                PerfilFinanceiro.Equilibrado => new Dictionary<CategoriaGasto, decimal>
                {
                    [CategoriaGasto.NecessidadesBasicas] = salarioTotal * 0.5m,
                    [CategoriaGasto.Desejos] = salarioTotal * 0.3m,
                    [CategoriaGasto.Investimentos] = salarioTotal * 0.2m
                },
                PerfilFinanceiro.Investidor => new Dictionary<CategoriaGasto, decimal>
                {
                    [CategoriaGasto.NecessidadesBasicas] = salarioTotal * 0.6m,
                    [CategoriaGasto.Investimentos] = salarioTotal * 0.2m,
                    [CategoriaGasto.Desejos] = salarioTotal * 0.2m
                },
                PerfilFinanceiro.Conservador => new Dictionary<CategoriaGasto, decimal>
                {
                    [CategoriaGasto.NecessidadesBasicas] = salarioTotal * 0.4m,
                    [CategoriaGasto.EstiloVida] = salarioTotal * 0.3m,
                    [CategoriaGasto.Investimentos] = salarioTotal * 0.2m,
                    [CategoriaGasto.DoacoesMetas] = salarioTotal * 0.1m
                },
                _ => throw new ArgumentException("Perfil financeiro não reconhecido")
            };
        }

        public async Task<decimal> CalcularPrevisaoProximoMesAsync()
        {
            var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
            var dividasVariaveis = await _dividaRepository.GetDividasPorTipoAsync(TipoDivida.Variavel);

            var valorEstimadoDividas = dividasVariaveis.Sum(d => d.ValorEstimadoProximoMes ?? d.Valor);

            return usuario.SalarioBase - valorEstimadoDividas;
        }
    }
}