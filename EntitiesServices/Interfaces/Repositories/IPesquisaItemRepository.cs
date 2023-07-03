using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaItemRepository : IRepositoryBase<PESQUISA_ITEM>
    {
        List<PESQUISA_ITEM> GetAllItens();
        PESQUISA_ITEM GetItemById(Int32 id);
    }
}
