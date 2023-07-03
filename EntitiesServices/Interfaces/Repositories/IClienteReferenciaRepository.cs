using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IClienteReferenciaRepository : IRepositoryBase<CLIENTE_REFERENCIA>
    {
        List<CLIENTE_REFERENCIA> GetAllItens();
        CLIENTE_REFERENCIA GetItemById(Int32 id);

    }
}
