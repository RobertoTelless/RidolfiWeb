using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaRespostaItemRepository : IRepositoryBase<PESQUISA_RESPOSTA_ITEM>
    {
        List<PESQUISA_RESPOSTA_ITEM> GetAllItens();
        PESQUISA_RESPOSTA_ITEM GetItemById(Int32 id);
    }
}
