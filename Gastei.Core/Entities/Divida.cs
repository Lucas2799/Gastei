using Gastei.Core.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gastei.Core.Entities
{
    [Table("Dividas")]
    public class Divida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public decimal? ValorEstimadoProximoMes { get; set; } // Para dividas variáveis
        public TipoDivida Tipo { get; set; }
        public int DiaVencimento { get; set; }
        public bool Ativa { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}