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

        // pegar usuário e salário
        var usuario = await _usuarioRepository.GetUsuarioAtivoAsync();
        if (usuario == null)
        {
            await Shell.Current.DisplayAlert("Erro", "Usuário não encontrado", "OK");
            return;
        }

        var perfil = usuario.Perfil;
        var salario = usuario.SalarioBase;

        // bucket da categoria da dívida
        var bucket = PerfilFinanceiroRules.GetBucketName(DividaSelecionada.Categoria);

        // bucket percentual e limite em reais
        var bucketPct = PerfilFinanceiroRules.GetBucketPercent(perfil, bucket);
        var bucketLimite = salario * bucketPct;

        // soma atual do bucket no mês/ano da dívida (você deve ter mes/ano na Divida ou DataReferencia)
        var mes = DividaSelecionada.ReferenciaMes ?? DateTime.Now.Month; // adapte conforme campo
        var ano = DividaSelecionada.ReferenciaAno ?? DateTime.Now.Year;

        var somaAtualBucket = await _dividaRepository.GetSumValorByBucketAsync(mes, ano, bucket);

        var novoTotalBucket = somaAtualBucket + DividaSelecionada.Valor;

        if (novoTotalBucket > bucketLimite)
        {
            // calcular fatia "ideal" da categoria para mostrar sugestão
            var categoriasNoBucket = PerfilFinanceiroRules.GetCategoriasPorBucket(bucket);
            var idealPorCategoriaPct = bucketPct / categoriasNoBucket.Count;
            var idealPorCategoriaValor = salario * idealPorCategoriaPct;

            var mensagem =
                $"Ao adicionar essa dívida, o total do bucket '{bucket}' ficaria em R$ {novoTotalBucket:F2},\n" +
                $"limite do bucket: R$ {bucketLimite:F2}.\n\n" +
                $"Sugestão: cada categoria idealmente tem ~{idealPorCategoriaPct:P2} ({idealPorCategoriaValor:C}).";

            await Shell.Current.DisplayAlert("Limite do bucket excedido", mensagem, "OK");
            return;
        }

        // tudo certo: inserir/atualizar (ajuste DataCriacao/Referencia)
        if (DividaSelecionada.Id == 0)
        {
            DividaSelecionada.DataCriacao = DateTime.Now;
            // garantir mes/ano de referência
            DividaSelecionada.ReferenciaMes = mes;
            DividaSelecionada.ReferenciaAno = ano;
            await _dividaRepository.InsertAsync(DividaSelecionada);
        }
        else
        {
            DividaSelecionada.DataAtualizacao = DateTime.Now;
            await _dividaRepository.UpdateAsync(DividaSelecionada);
        }

        await Shell.Current.DisplayAlert("Sucesso", "Dívida salva com sucesso!", "OK");
        await Shell.Current.GoToAsync("..");
    }

}
