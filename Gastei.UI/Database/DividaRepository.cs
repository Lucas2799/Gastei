using SQLite;
using Gastei.Core.Interfaces;
using Gastei.Core.Entities;
using Gastei.Core.Enums;

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

    public async Task ReplicarFixasParaMesSeguinteAsync()
    {
        var next = DateTime.Now.AddMonths(1);
        var mesNext = next.Month;
        var anoNext = next.Year;

        // buscar todas fixas que existem no banco (independente do mês)
        var fixas = await _database.Table<Divida>()
            .Where(d => d.Tipo == TipoDivida.Fixa)
            .ToListAsync();

        foreach (var f in fixas)
        {
            // verificar se já existe uma cópia no mês seguinte com mesma descrição/valor/dia
            var existe = await _database.Table<Divida>()
                .Where(d => d.ReferenciaMes == mesNext && d.ReferenciaAno == anoNext && d.Descricao == f.Descricao && d.DiaVencimento == f.DiaVencimento)
                .FirstOrDefaultAsync();

            if (existe == null)
            {
                var nova = new Divida
                {
                    Descricao = f.Descricao,
                    Valor = f.Valor,
                    ValorEstimadoProximoMes = f.ValorEstimadoProximoMes,
                    Tipo = f.Tipo,
                    DiaVencimento = f.DiaVencimento,
                    Ativa = f.Ativa,
                    DataCriacao = DateTime.Now,
                    ReferenciaMes = mesNext,
                    ReferenciaAno = anoNext,
                    // não copiar Id
                };
                await _database.InsertAsync(nova);
            }
        }
    }

    public async Task<List<Divida>> GetAllAsync()
    {
        return await _database.Table<Divida>().ToListAsync();
    }

    public async Task<int> InsertAsync(Divida entity)
    {
        entity.DataCriacao = DateTime.Now;

        // Se não tiver referência, definir automaticamente:
        if (entity.ReferenciaMes == 0 || entity.ReferenciaAno == 0)
        {
            if (entity.Tipo == TipoDivida.Fixa)
            {
                var next = DateTime.Now.AddMonths(1);
                entity.ReferenciaMes = next.Month;
                entity.ReferenciaAno = next.Year;
            }
            else
            {
                var now = DateTime.Now;
                entity.ReferenciaMes = now.Month;
                entity.ReferenciaAno = now.Year;
            }
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