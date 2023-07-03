using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVendaMensalAnotacaoRepository : IRepositoryBase<VENDA_MENSAL_ANOTACAO>
    {
        List<VENDA_MENSAL_ANOTACAO> GetAllItens();
        VENDA_MENSAL_ANOTACAO GetItemById(Int32 id);
    }
}
