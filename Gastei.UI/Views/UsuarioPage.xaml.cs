using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.UI.Database;

namespace Gastei.UI.Views;

public partial class UsuarioPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public UsuarioPage(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        InitializeComponent();
        _ = CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        try
        {
            var connection = _databaseService.GetConnection();
            var usuario = await connection.Table<Usuario>().FirstOrDefaultAsync();

            if (usuario != null)
            {
                // Preencher os campos com dados do banco
                EntryNome.Text = usuario.Nome;
                EntryEmail.Text = usuario.Email;
                EntrySalario.Text = usuario.SalarioBase.ToString("F2");
                PickerDia.SelectedItem = usuario.DiaRecebimentoSalario.ToString();
                PickerPerfil.SelectedItem = usuario.Perfil.ToString();
            }
            else
            {
                // Dados padrão se não existir usuário
                EntryNome.Text = "Seu Nome";
                EntrySalario.Text = "3000";
                PickerDia.SelectedItem = "5";
                PickerPerfil.SelectedItem = "Equilibrado";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao carregar dados: {ex.Message}", "OK");
        }
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        if (_databaseService == null)
        {
            await DisplayAlert("Erro", "Banco de dados não disponível", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EntryNome.Text) ||
            string.IsNullOrWhiteSpace(EntrySalario.Text) ||
            PickerDia.SelectedItem == null ||
            PickerPerfil.SelectedItem == null)
        {
            await DisplayAlert("Atenção", "Preencha todos os campos obrigatórios (*)", "OK");
            return;
        }

        try
        {
            var nome = EntryNome.Text.Trim();
            var email = EntryEmail.Text?.Trim() ?? "";
            var salario = decimal.Parse(EntrySalario.Text);
            var dia = int.Parse(PickerDia.SelectedItem.ToString());

            var perfil = PickerPerfil.SelectedItem.ToString() switch
            {
                "Equilibrado" => PerfilFinanceiro.Equilibrado,
                "Investidor" => PerfilFinanceiro.Investidor,
                "Conservador" => PerfilFinanceiro.Conservador,
                _ => PerfilFinanceiro.Equilibrado
            };

            var connection = _databaseService.GetConnection();
            var usuarioExistente = await connection.Table<Usuario>().FirstOrDefaultAsync();

            if (usuarioExistente != null)
            {
                // MANTER O ID EXISTENTE para o UPDATE funcionar
                usuarioExistente.Nome = nome;
                usuarioExistente.Email = email;
                usuarioExistente.SalarioBase = salario;
                usuarioExistente.DiaRecebimentoSalario = dia;
                usuarioExistente.Perfil = perfil;
                usuarioExistente.DataAtualizacao = DateTime.Now;

                var resultado = await connection.UpdateAsync(usuarioExistente);

                if (resultado == 1) // UPDATE retorna 1 se uma linha foi afetada
                {
                    await DisplayAlert("Sucesso", "Configurações atualizadas com sucesso!", "OK");
                }
                else
                {
                    await DisplayAlert("Aviso", "Nenhuma alteração foi salva", "OK");
                }
            }
            else
            {
                var novoUsuario = new Usuario
                {
                    Nome = nome,
                    Email = email,
                    SalarioBase = salario,
                    DiaRecebimentoSalario = dia,
                    Perfil = perfil,
                    DataCriacao = DateTime.Now,
                    DataAtualizacao = DateTime.Now
                };

                var resultado = await connection.InsertAsync(novoUsuario);

                if (resultado == 1) // INSERT retorna 1 se uma linha foi inserida
                {
                    await DisplayAlert("Sucesso", "Configurações salvas com sucesso!", "OK");
                }
                else
                {
                    await DisplayAlert("Erro", "Erro ao salvar configurações", "OK");
                }
            }

            await Navigation.PopAsync();
        }
        catch (FormatException)
        {
            await DisplayAlert("Erro", "Formato de salário inválido. Use apenas números.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao salvar: {ex.Message}", "OK");
        }
    }
}