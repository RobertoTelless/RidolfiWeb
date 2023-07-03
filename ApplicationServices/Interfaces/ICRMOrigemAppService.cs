using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICRMOrigemAppService : IAppServiceBase<CRM_ORIGEM>
    {
        Int32 ValidateCreate(CRM_ORIGEM item, USUARIO usuario);
        Int32 ValidateEdit(CRM_ORIGEM item, CRM_ORIGEM itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CRM_ORIGEM item, USUARIO usuario);
        Int32 ValidateReativar(CRM_ORIGEM item, USUARIO usuario);

        CRM_ORIGEM CheckExist(CRM_ORIGEM item, Int32 idAss);
        List<CRM_ORIGEM> GetAllItens(Int32 idAss);
        CRM_ORIGEM GetItemById(Int32 id);
        List<CRM_ORIGEM> GetAllItensAdm(Int32 idAss);

    }
}
