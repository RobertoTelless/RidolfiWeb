using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoPesquisaService : IServiceBase<TIPO_PESQUISA>
    {
        Int32 Create(TIPO_PESQUISA perfil, LOG log);
        Int32 Create(TIPO_PESQUISA perfil);
        Int32 Edit(TIPO_PESQUISA perfil, LOG log);
        Int32 Edit(TIPO_PESQUISA perfil);
        Int32 Delete(TIPO_PESQUISA perfil, LOG log);

        TIPO_PESQUISA CheckExist(TIPO_PESQUISA item, Int32 idAss);
        List<TIPO_PESQUISA> GetAllItens(Int32 idAss);
        TIPO_PESQUISA GetItemById(Int32 id);
        List<TIPO_PESQUISA> GetAllItensAdm(Int32 idAss);
    }
}
