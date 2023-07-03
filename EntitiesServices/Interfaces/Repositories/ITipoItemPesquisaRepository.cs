using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoItemPesquisaRepository : IRepositoryBase<TIPO_ITEM_PESQUISA>
    {
        TIPO_ITEM_PESQUISA CheckExist(TIPO_ITEM_PESQUISA item, Int32 idAss);
        List<TIPO_ITEM_PESQUISA> GetAllItens(Int32 idAss);
        TIPO_ITEM_PESQUISA GetItemById(Int32 id);
        List<TIPO_ITEM_PESQUISA> GetAllItensAdm(Int32 idAss);
    }
}
