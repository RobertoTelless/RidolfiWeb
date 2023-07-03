using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoPesquisaAppService : IAppServiceBase<TIPO_PESQUISA>
    {
        Int32 ValidateCreate(TIPO_PESQUISA item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_PESQUISA item, TIPO_PESQUISA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_PESQUISA item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_PESQUISA item, USUARIO usuario);

        TIPO_PESQUISA CheckExist(TIPO_PESQUISA item, Int32 idAss);
        List<TIPO_PESQUISA> GetAllItens(Int32 idAss);
        TIPO_PESQUISA GetItemById(Int32 id);
        List<TIPO_PESQUISA> GetAllItensAdm(Int32 idAss);

    }
}
