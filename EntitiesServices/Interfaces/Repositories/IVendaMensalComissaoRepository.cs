using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVendaMensalComissaoRepository : IRepositoryBase<VENDA_MENSAL_COMISSAO>
    {
        List<VENDA_MENSAL_COMISSAO> GetAllItens();
        VENDA_MENSAL_COMISSAO GetItemById(Int32 id);
    }
}
