using SQLite;

namespace Gastei.UI.Database;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "gastei.db3");
        _database = new SQLiteAsyncConnection(databasePath);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _database.CreateTableAsync<Core.Entities.Usuario>();
        // Adicionaremos outras tabelas depois
    }

    public SQLiteAsyncConnection GetConnection() => _database;
}