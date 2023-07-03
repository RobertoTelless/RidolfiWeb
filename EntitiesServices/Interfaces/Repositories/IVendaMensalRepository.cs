using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVendaMensalRepository : IRepositoryBase<VENDA_MENSAL>
    {
        VENDA_MENSAL CheckExist(VENDA_MENSAL item, Int32 idAss);
        VENDA_MENSAL GetItemById(Int32 id);
        List<VENDA_MENSAL> GetAllItens(Int32 idAss);
        List<VENDA_MENSAL> GetAllItensAdm(Int32 idAss);
        List<VENDA_MENSAL> ExecuteFilter(DateTime? dataRef, Int32? tipo, Int32 idAss);
    }
}
