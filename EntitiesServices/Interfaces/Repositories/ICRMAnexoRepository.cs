using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMAnexoRepository : IRepositoryBase<CRM_ANEXO>
    {
        List<CRM_ANEXO> GetAllItens();
        CRM_ANEXO GetItemById(Int32 id);
    }
}
