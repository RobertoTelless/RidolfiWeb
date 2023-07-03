using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMFollowRepository : IRepositoryBase<CRM_FOLLOW>
    {
        List<CRM_FOLLOW> GetAllItens(Int32 idAss);
        CRM_FOLLOW GetItemById(Int32 id);
    }
}
