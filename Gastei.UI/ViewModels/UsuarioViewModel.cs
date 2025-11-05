using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;

namespace Gastei.UI.ViewModels;

public partial class UsuarioViewModel : BaseViewModel
{
    private readonly IUsuarioRepository _usuarioRepository;

    [ObservableProperty]
    private Usuario usuario = new();

    [ObservableProperty]
    private string nome = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private decimal salarioBase;

    [ObservableProperty]
    private int diaRecebimentoSalario;

    [ObservableProperty]
    private PerfilFinanceiro perfilSelecionado;

    public List<PerfilFinanceiro> PerfisDisponiveis { get; } =
        Enum.GetValues(typeof(PerfilFinanceiro)).Cast<PerfilFinanceiro>().ToList();

    public UsuarioViewModel(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
        Title = "Configurações do Usuário";
        _ = LoadUsuarioAsync();
    }

    private async Task LoadUsuarioAsync()
    {
        IsBusy = true;

        try
        {
            var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
            if (usuario != null)
            {
                Usuario = usuario;
                Nome = usuario.Nome;
                Email = usuario.Email;
                SalarioBase = usuario.SalarioBase;
                DiaRecebimentoSalario = usuario.DiaRecebimentoSalario;
                PerfilSelecionado = usuario.Perfil;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SalvarUsuarioAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome) || SalarioBase <= 0)
        {
            await Shell.Current.DisplayAlert("Erro", "Preencha todos os campos obrigatórios", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            usuario.Nome = Nome;
            usuario.Email = Email;
            usuario.SalarioBase = SalarioBase;
            usuario.DiaRecebimentoSalario = DiaRecebimentoSalario;
            usuario.Perfil = PerfilSelecionado;

            if (usuario.Id == 0)
                await _usuarioRepository.InsertAsync(usuario);
            else
                await _usuarioRepository.UpdateAsync(usuario);

            await Shell.Current.DisplayAlert("Sucesso", "Dados salvos com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}