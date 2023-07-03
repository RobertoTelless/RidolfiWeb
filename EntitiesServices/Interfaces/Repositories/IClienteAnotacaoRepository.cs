using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IClienteAnotacaoRepository : IRepositoryBase<CLIENTE_ANOTACAO>
    {
        List<CLIENTE_ANOTACAO> GetAllItens();
        CLIENTE_ANOTACAO GetItemById(Int32 id);
    }
}
