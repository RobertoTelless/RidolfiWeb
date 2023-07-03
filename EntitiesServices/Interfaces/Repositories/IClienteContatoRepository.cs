using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IClienteContatoRepository : IRepositoryBase<CLIENTE_CONTATO>
    {
        List<CLIENTE_CONTATO> GetAllItens();
        CLIENTE_CONTATO GetItemById(Int32 id);

    }
}
