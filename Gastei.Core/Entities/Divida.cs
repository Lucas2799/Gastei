using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gastei.Core.Enums;

namespace Gastei.Core.Entities
{
    public class Divida
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public decimal? ValorEstimadoProximoMes { get; set; } // Para dividas variáveis
        public TipoDivida Tipo { get; set; }
        public int DiaVencimento { get; set; }
        public bool Ativa { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}