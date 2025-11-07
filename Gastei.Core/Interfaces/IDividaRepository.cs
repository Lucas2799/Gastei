using Gastei.Core.Entities;
using Gastei.Core.Enums;

namespace Gastei.Core.Interfaces;

public interface IDividaRepository
{
    Task<Divida> GetByIdAsync(int id);
    Task<List<Divida>> GetAllAsync();
    Task<int> InsertAsync(Divida entity);
    Task<int> UpdateAsync(Divida entity);
    Task<int> DeleteAsync(Divida entity);
    Task<List<Divida>> GetDividasAtivasAsync();
    Task<List<Divida>> GetDividasPorTipoAsync(TipoDivida tipo);
    Task<List<Divida>> GetDividasPorDiaVencimentoAsync(int dia);
}
