using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaEnvioRepository : IRepositoryBase<PESQUISA_ENVIO>
    {
        List<PESQUISA_ENVIO> GetAllItens();
        PESQUISA_ENVIO GetItemById(Int32 id);
    }
}
