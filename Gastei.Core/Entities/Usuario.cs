using Gastei.Core.Enums;
using SQLite;

namespace Gastei.Core.Entities;

[Table("Usuarios")]
public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal SalarioBase { get; set; }
    public int DiaRecebimentoSalario { get; set; }
    public PerfilFinanceiro Perfil { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}


