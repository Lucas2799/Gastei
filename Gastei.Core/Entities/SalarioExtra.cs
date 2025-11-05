using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gastei.Core.Entities
{
    public class SalarioExtra
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataRecebimento { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
