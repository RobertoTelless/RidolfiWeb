using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IClienteEnderecoRepository : IRepositoryBase<CLIENTE_ENDERECO>
    {
        List<CLIENTE_ENDERECO> GetAllItens();
        CLIENTE_ENDERECO GetItemById(Int32 id);
    }
}
