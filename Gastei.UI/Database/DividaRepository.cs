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

    public async Task<List<Divida>> GetAllAsync()
    {
        return await _database.Table<Divida>().ToListAsync();
    }

    public async Task<int> InsertAsync(Divida entity)
    {
        entity.DataCriacao = DateTime.Now;
        return await _database.InsertAsync(entity);
    }

    public async Task<int> UpdateAsync(Divida entity)
    {
        entity.DataAtualizacao = DateTime.Now;
        return await _database.UpdateAsync(entity);
    }

    public async Task<int> DeleteAsync(Divida entity)
    {
        return await _database.DeleteAsync(entity);
    }

    public async Task<List<Divida>> GetDividasAtivasAsync()
    {
        return await _database.Table<Divida>()
            .Where(d => d.Ativa)
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