using Gastei.Core.Entities;
using Gastei.Core.Enums;
using SQLite;

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
            // Criar tabelas
            await _database.CreateTableAsync<Usuario>();
            await _database.CreateTableAsync<Divida>(); // ← NOVA TABELA

            // Verificar se já existe algum usuário
            var countUsuario = await _database.Table<Usuario>().CountAsync();

            if (countUsuario == 0)
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
            }

            // Adicionar algumas dívidas de exemplo (opcional)
            var countDividas = await _database.Table<Divida>().CountAsync();
            if (countDividas == 0)
            {
                var dividasExemplo = new List<Divida>
            {
                new Divida
                {
                    Descricao = "Aluguel",
                    Valor = 1300m,
                    Tipo = TipoDivida.Fixa,
                    DiaVencimento = 10,
                    Ativa = true,
                    DataCriacao = DateTime.Now
                },
                new Divida
                {
                    Descricao = "Internet",
                    Valor = 99.90m,
                    Tipo = TipoDivida.Fixa,
                    DiaVencimento = 10,
                    Ativa = true,
                    DataCriacao = DateTime.Now
                }
            };

                foreach (var divida in dividasExemplo)
                {
                    await _database.InsertAsync(divida);
                }
            }

            _initialized = true;
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