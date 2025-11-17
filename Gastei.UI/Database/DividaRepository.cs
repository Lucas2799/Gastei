using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;
using Gastei.Core.Rules;
using SQLite;

namespace Gastei.UI.Database;

public class DividaRepository : IDividaRepository
{
    private readonly SQLiteAsyncConnection _database;

    public DividaRepository(DatabaseService databaseService)
    {
        _database = databaseService.GetConnection();
    }

    public async Task<Divida> GetByIdAsync(int id)
    {
        return await _database.Table<Divida>()
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Divida>> GetDividasPorMesAsync(int mes, int ano)
    {
        return await _database.Table<Divida>()
            .Where(d => d.ReferenciaMes == mes && d.ReferenciaAno == ano)
            .OrderBy(d => d.DiaVencimento)
            .ToListAsync();
    }

    public async Task<List<Divida>> GetDividasFixasAsync()
    {
        return await _database.Table<Divida>()
            .Where(d => d.Fixa && d.Ativa)
            .ToListAsync();
    }

    public async Task<decimal> GetSumValorByBucketAsync(int mes, int ano, string bucketName)
    {
        // carrega todas do mês e filtra buckets (se você armazenar Categoria como enum -> ToString)
        var todas = await GetDividasPorMesAsync(mes, ano); // implemente este método se ainda não
        var soma = todas
            .Where(d => PerfilFinanceiroRules.GetBucketName(d.Categoria) == bucketName && d.Ativa)
            .Sum(d => d.Valor);
        return soma;
    }

    public async Task ReplicarFixasParaMesSeguinteAsync()
    {
        var hoje = DateTime.Now;
        var mesAtual = hoje.Month;
        var anoAtual = hoje.Year;

        var proximoMes = mesAtual == 12 ? 1 : mesAtual + 1;
        var proximoAno = mesAtual == 12 ? anoAtual + 1 : anoAtual;

        // ✅ Verifica se já existem registros do próximo mês
        var fixasProximoMes = await _database.Table<Divida>()
     .Where(d => d.Tipo == TipoDivida.Fixa &&
                 d.ReferenciaMes == proximoMes &&
                 d.ReferenciaAno == proximoAno)
     .CountAsync();

        if (fixasProximoMes < 0)
            return;

        // ✅ Busca apenas as FIXAS do mês atual
        var fixas = await _database.Table<Divida>()
            .Where(d => d.Tipo == TipoDivida.Fixa && d.ReferenciaMes == mesAtual && d.ReferenciaAno == anoAtual)
            .ToListAsync();

        if (!fixas.Any())
            return;

        foreach (var f in fixas)
        {
            var nova = new Divida
            {
                Descricao = f.Descricao,
                Valor = f.Valor,
                ValorEstimadoProximoMes = f.ValorEstimadoProximoMes,
                Tipo = f.Tipo,
                DiaVencimento = f.DiaVencimento,
                Categoria = f.Categoria,
                Subcategoria = f.Subcategoria,
                Ativa = f.Ativa,
                DataCriacao = DateTime.Now,
                ReferenciaMes = proximoMes,
                ReferenciaAno = proximoAno
            };

            await _database.InsertAsync(nova);
        }

        System.Diagnostics.Debug.WriteLine($"✅ {fixas.Count} dívidas fixas replicadas para {proximoMes}/{proximoAno}");
    }


    public async Task<List<Divida>> GetAllAsync()
    {
        return await _database.Table<Divida>().ToListAsync();
    }

    public async Task<int> InsertAsync(Divida entity)
    {
        entity.DataCriacao = DateTime.Now;

        if (entity.ReferenciaMes == 0 || entity.ReferenciaAno == 0)
        {
            var now = DateTime.Now;
            entity.ReferenciaMes = now.Month;
            entity.ReferenciaAno = now.Year;
        }

        return await _database.InsertAsync(entity);
    }


    public async Task<int> UpdateAsync(Divida entity)
    {
        entity.DataAtualizacao = DateTime.Now;

        // Se por alguma razão a referência estiver vazia, garantir algo sensato
        if (entity.ReferenciaMes == 0 || entity.ReferenciaAno == 0)
        {
            var now = DateTime.Now;
            entity.ReferenciaMes = now.Month;
            entity.ReferenciaAno = now.Year;
        }

        return await _database.UpdateAsync(entity);
    }

    public async Task<int> DeleteAsync(Divida entity)
    {
        return await _database.DeleteAsync(entity);
    }

    public async Task<List<Divida>> GetDividasAtivasAsync()
    {
        var mes = DateTime.Now.Month;
        var ano = DateTime.Now.Year;

        return await _database.Table<Divida>()
            .Where(d => d.Ativa && d.ReferenciaMes == mes && d.ReferenciaAno == ano)
            .OrderBy(d => d.DiaVencimento)
            .ToListAsync();
    }

    public async Task<List<Divida>> GetDividasPorTipoAsync(TipoDivida tipo)
    {
        return await _database.Table<Divida>()
            .Where(d => d.Tipo == tipo && d.Ativa)
            .OrderBy(d => d.DiaVencimento)
            .ToListAsync();
    }

    public async Task<List<Divida>> GetDividasPorDiaVencimentoAsync(int dia)
    {
        return await _database.Table<Divida>()
            .Where(d => d.DiaVencimento == dia && d.Ativa)
            .ToListAsync();
    }
}