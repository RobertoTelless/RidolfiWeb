using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoFollowService : IServiceBase<TIPO_FOLLOW>
    {
        Int32 Create(TIPO_FOLLOW perfil, LOG log);
        Int32 Create(TIPO_FOLLOW perfil);
        Int32 Edit(TIPO_FOLLOW perfil, LOG log);
        Int32 Edit(TIPO_FOLLOW perfil);
        Int32 Delete(TIPO_FOLLOW perfil, LOG log);

        TIPO_FOLLOW CheckExist(TIPO_FOLLOW item, Int32 idAss);
        List<TIPO_FOLLOW> GetAllItens(Int32 idAss);
        TIPO_FOLLOW GetItemById(Int32 id);
        List<TIPO_FOLLOW> GetAllItensAdm(Int32 idAss);
    }
}
