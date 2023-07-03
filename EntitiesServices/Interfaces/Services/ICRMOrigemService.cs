using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICRMOrigemService : IServiceBase<CRM_ORIGEM>
    {
        Int32 Create(CRM_ORIGEM perfil, LOG log);
        Int32 Create(CRM_ORIGEM perfil);
        Int32 Edit(CRM_ORIGEM perfil, LOG log);
        Int32 Edit(CRM_ORIGEM perfil);
        Int32 Delete(CRM_ORIGEM perfil, LOG log);

        CRM_ORIGEM CheckExist(CRM_ORIGEM item, Int32 idAss);
        CRM_ORIGEM GetItemById(Int32 id);
        List<CRM_ORIGEM> GetAllItens(Int32 idAss);
        List<CRM_ORIGEM> GetAllItensAdm(Int32 idAss);
    }
}
