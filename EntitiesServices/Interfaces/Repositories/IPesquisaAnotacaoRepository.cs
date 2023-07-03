using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaAnotacaoRepository : IRepositoryBase<PESQUISA_ANOTACAO>
    {
        List<PESQUISA_ANOTACAO> GetAllItens();
        PESQUISA_ANOTACAO GetItemById(Int32 id);

    }
}
