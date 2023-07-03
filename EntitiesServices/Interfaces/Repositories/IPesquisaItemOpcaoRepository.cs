using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaItemOpcaoRepository : IRepositoryBase<PESQUISA_ITEM_OPCAO>
    {
        List<PESQUISA_ITEM_OPCAO> GetAllItens();
        PESQUISA_ITEM_OPCAO GetItemById(Int32 id);
    }
}
