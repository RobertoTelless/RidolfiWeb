using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoFollowAppService : IAppServiceBase<TIPO_FOLLOW>
    {
        Int32 ValidateCreate(TIPO_FOLLOW item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_FOLLOW item, TIPO_FOLLOW itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_FOLLOW item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_FOLLOW item, USUARIO usuario);

        TIPO_FOLLOW CheckExist(TIPO_FOLLOW item, Int32 idAss);
        List<TIPO_FOLLOW> GetAllItens(Int32 idAss);
        TIPO_FOLLOW GetItemById(Int32 id);
        List<TIPO_FOLLOW> GetAllItensAdm(Int32 idAss);

    }
}
