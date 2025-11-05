using Gastei.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gastei.Core.Entities
{
    public class OrcamentoMensal
    {
        public int Id { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public decimal SalarioTotal { get; set; }
        public decimal TotalDividas { get; set; }
        public decimal SaldoDisponivel { get; set; }
        public Dictionary<CategoriaGasto, decimal> Distribuicao { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}