using Gastei.Core.Enums;

namespace Gastei.Core.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal SalarioBase { get; set; }
    public int DiaRecebimentoSalario { get; set; }
    public PerfilFinanceiro Perfil { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}



//C:\Users\lucas.tavares\Documents\Lucas\Gastei\Gastei.UI\Resources\AppIcon\appicon.svg