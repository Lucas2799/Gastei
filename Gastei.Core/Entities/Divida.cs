using Gastei.Core.Enums;
using SQLite;

namespace Gastei.Core.Entities;

[Table("Dividas")]
public class Divida
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public decimal? ValorEstimadoProximoMes { get; set; }
    public TipoDivida Tipo { get; set; }
    public int DiaVencimento { get; set; }
    public bool Ativa { get; set; }
    public bool Fixa { get; set; }

    public CategoriaDivida Categoria { get; set; }
    public SubcategoriaDivida Subcategoria { get; set; }

    [Ignore] public string Status { get; set; } = string.Empty;
    [Ignore] public Color StatusCor { get; set; } = Colors.White;
    [Ignore] public Color CategoriaCor { get; set; } = Colors.Gray;

    public int? ReferenciaMes { get; set; } 
    public int? ReferenciaAno { get; set; } 

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
