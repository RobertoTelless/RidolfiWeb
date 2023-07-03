using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVendaMensalAnexoRepository : IRepositoryBase<VENDA_MENSAL_ANEXO>
    {
        List<VENDA_MENSAL_ANEXO> GetAllItens();
        VENDA_MENSAL_ANEXO GetItemById(Int32 id);

    }
}
