using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMOrigemRepository : IRepositoryBase<CRM_ORIGEM>
    {
        List<CRM_ORIGEM> GetAllItens(Int32 idAss);
        CRM_ORIGEM GetItemById(Int32 id);
        CRM_ORIGEM CheckExist(CRM_ORIGEM item, Int32 idAss);
        List<CRM_ORIGEM> GetAllItensAdm(Int32 idAss);
    }
}
