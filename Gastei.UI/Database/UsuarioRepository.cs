using SQLite;
using Gastei.Core.Interfaces;
using Gastei.Core.Entities;

namespace Gastei.UI.Database;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SQLiteAsyncConnection _database;

    public UsuarioRepository(DatabaseService databaseService)
    {
        _database = databaseService.GetConnection();
    }

    public async Task<Usuario> GetByIdAsync(int id)
    {
        return await _database.Table<Usuario>()
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        return await _database.Table<Usuario>().ToListAsync();
    }

    public async Task<int> InsertAsync(Usuario entity)
    {
        entity.DataCriacao = DateTime.Now;
        entity.DataAtualizacao = DateTime.Now;
        return await _database.InsertAsync(entity);
    }

    public async Task<int> UpdateAsync(Usuario entity)
    {
        entity.DataAtualizacao = DateTime.Now;
        return await _database.UpdateAsync(entity);
    }

    public async Task<int> DeleteAsync(Usuario entity)
    {
        return await _database.DeleteAsync(entity);
    }

    public async Task<Usuario> GetUsuarioAtivoAsync()
    {
        return await _database.Table<Usuario>()
            .FirstOrDefaultAsync() ?? new Usuario();
    }
}