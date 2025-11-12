using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gastei.Core.Entities;
using Gastei.Core.Enums;
using Gastei.Core.Interfaces;
using Gastei.Core.Rules;

namespace Gastei.UI.ViewModels;

[QueryProperty(nameof(DividaSelecionada), "DividaSelecionada")]
public partial class NovaDividaViewModel : BaseViewModel
{
    private readonly IDividaRepository _dividaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    [ObservableProperty]
    private Divida _dividaSelecionada = new();

    public List<TipoDivida> TiposDisponiveis { get; } =
        Enum.GetValues(typeof(TipoDivida)).Cast<TipoDivida>().ToList();

    public List<CategoriaDivida> CategoriasDisponiveis { get; } =
        Enum.GetValues(typeof(CategoriaDivida)).Cast<CategoriaDivida>().ToList();

    public List<SubcategoriaDivida> SubcategoriasDisponiveis { get; } =
        Enum.GetValues(typeof(SubcategoriaDivida)).Cast<SubcategoriaDivida>().ToList();

    public NovaDividaViewModel(IDividaRepository dividaRepository)
    {
        _dividaRepository = dividaRepository;
        Title = "Nova Dívida";

        // ✅ Preencher mês/ano automaticamente
        DividaSelecionada.ReferenciaMes = DateTime.Now.Month;
        DividaSelecionada.ReferenciaAno = DateTime.Now.Year;
        DividaSelecionada.Ativa = true;
    }

    public NovaDividaViewModel(IDividaRepository dividaRepository, IUsuarioRepository usuarioRepository)
    {
        _dividaRepository = dividaRepository;
        _usuarioRepository = usuarioRepository;
        Title = "Nova Dívida";
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(DividaSelecionada.Descricao) || DividaSelecionada.Valor <= 0)
        {
            await Shell.Current.DisplayAlert("Erro", "Preencha todos os campos obrigatórios", "OK");
            return;
        }

        var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
        if (usuario == null)
        {
            await Shell.Current.DisplayAlert("Erro", "Usuário ativo não encontrado.", "OK");
            return;
        }

        var perfil = usuario.Perfil;
        var salario = usuario.SalarioBase;

        // Todas as dívidas do mês atual
        var dividasMes = await _dividaRepository.GetDividasPorMesAsync(DateTime.Now.Month, DateTime.Now.Year);
        var dividasCategoria = dividasMes.Where(d => d.Categoria == DividaSelecionada.Categoria).ToList();

        var totalCategoria = dividasCategoria.Sum(d => d.Valor);

        // Limite permitido pelo perfil
        var limitePercentual = PerfilFinanceiroRules.GetLimitePercentual(perfil, DividaSelecionada.Categoria);
        var limiteValor = usuario.SalarioBase * limitePercentual;

        // Valor total da categoria se incluir a nova dívida
        var totalComNova = totalCategoria + DividaSelecionada.Valor;

        if (totalComNova > limiteValor)
        {
            var percentualUsado = totalCategoria / salario;
            var percentualNovo = totalComNova / salario;

            var msg = $"⚠️ O limite da categoria '{DividaSelecionada.Categoria}' para o perfil '{perfil}' é de {limitePercentual:P0}.\n\n" +
                      $"Atualmente você já está utilizando {percentualUsado:P1} do salário nessa categoria.\n" +
                      $"Se adicionar essa dívida, passará a utilizar {percentualNovo:P1} — acima do permitido.";
            await Shell.Current.DisplayAlert("Limite Atingido", msg, "OK");
            return;
        }

        // Persistência normal
        DividaSelecionada.ReferenciaMes = DateTime.Now.Month;
        DividaSelecionada.ReferenciaAno = DateTime.Now.Year;

        if (DividaSelecionada.Id == 0)
            await _dividaRepository.InsertAsync(DividaSelecionada);
        else
            await _dividaRepository.UpdateAsync(DividaSelecionada);

        await Shell.Current.DisplayAlert("Sucesso", "Dívida salva com sucesso!", "OK");
        await Shell.Current.GoToAsync("..");
    }

}
