using SQLite;
using Gastei.Core.Entities;

namespace Gastei.UI.Database;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _initialized = false;

    public DatabaseService()
    {
        try
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "gastei.db3");
            _database = new SQLiteAsyncConnection(databasePath);

            // Inicialização assíncrona
            _ = InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao criar DatabaseService: {ex.Message}");
        }
    }

    private async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            await _database.CreateTableAsync<Usuario>();

            // Verificar se já existe algum usuário
            var count = await _database.Table<Usuario>().CountAsync();

            if (count == 0)
            {
                // Criar usuário padrão
                var usuarioPadrao = new Usuario
                {
                    Nome = "Seu Nome",
                    Email = "",
                    SalarioBase = 3000,
                    DiaRecebimentoSalario = 5,
                    Perfil = Core.Enums.PerfilFinanceiro.Equilibrado,
                    DataCriacao = DateTime.Now,
                    DataAtualizacao = DateTime.Now
                };
                await _database.InsertAsync(usuarioPadrao);
                System.Diagnostics.Debug.WriteLine("Usuário padrão criado no banco de dados");
            }

            _initialized = true;
            System.Diagnostics.Debug.WriteLine("Banco de dados inicializado com sucesso");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco: {ex.Message}");
        }
    }

    public SQLiteAsyncConnection GetConnection() => _database;

    // Método helper para obter usuário
    public async Task<Usuario> GetUsuarioAsync()
    {
        await InitializeAsync(); // Garantir que está inicializado
        return await _database.Table<Usuario>().FirstOrDefaultAsync();
    }

    // Método helper para salvar usuário
    public async Task<bool> SalvarUsuarioAsync(Usuario usuario)
    {
        try
        {
            await InitializeAsync();

            var usuarioExistente = await _database.Table<Usuario>().FirstOrDefaultAsync();

            if (usuarioExistente != null)
            {
                // Atualizar
                usuario.Id = usuarioExistente.Id;
                usuario.DataCriacao = usuarioExistente.DataCriacao;
                usuario.DataAtualizacao = DateTime.Now;
                await _database.UpdateAsync(usuario);
            }
            else
            {
                // Inserir novo
                usuario.DataCriacao = DateTime.Now;
                usuario.DataAtualizacao = DateTime.Now;
                await _database.InsertAsync(usuario);
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao salvar usuário: {ex.Message}");
            return false;
        }
    }
}