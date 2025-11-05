using Gastei.Core.Entities;
using Gastei.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gastei.Core.Interfaces
{
    public interface IDividaRepository : IRepository<Divida>
    {
        Task<List<Divida>> GetDividasAtivasAsync();
        Task<List<Divida>> GetDividasPorTipoAsync(TipoDivida tipo);
    }
}