using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoItemPesquisaService : IServiceBase<TIPO_ITEM_PESQUISA>
    {
        Int32 Create(TIPO_ITEM_PESQUISA perfil, LOG log);
        Int32 Create(TIPO_ITEM_PESQUISA perfil);
        Int32 Edit(TIPO_ITEM_PESQUISA perfil, LOG log);
        Int32 Edit(TIPO_ITEM_PESQUISA perfil);
        Int32 Delete(TIPO_ITEM_PESQUISA perfil, LOG log);

        TIPO_ITEM_PESQUISA CheckExist(TIPO_ITEM_PESQUISA item, Int32 idAss);
        List<TIPO_ITEM_PESQUISA> GetAllItens(Int32 idAss);
        TIPO_ITEM_PESQUISA GetItemById(Int32 id);
        List<TIPO_ITEM_PESQUISA> GetAllItensAdm(Int32 idAss);
    }
}
