using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoItemPesquisaAppService : IAppServiceBase<TIPO_ITEM_PESQUISA>
    {
        Int32 ValidateCreate(TIPO_ITEM_PESQUISA item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_ITEM_PESQUISA item, TIPO_ITEM_PESQUISA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_ITEM_PESQUISA item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_ITEM_PESQUISA item, USUARIO usuario);

        TIPO_ITEM_PESQUISA CheckExist(TIPO_ITEM_PESQUISA item, Int32 idAss);
        List<TIPO_ITEM_PESQUISA> GetAllItens(Int32 idAss);
        TIPO_ITEM_PESQUISA GetItemById(Int32 id);
        List<TIPO_ITEM_PESQUISA> GetAllItensAdm(Int32 idAss);

    }
}
