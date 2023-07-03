using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMContatoRepository : IRepositoryBase<CRM_CONTATO>
    {
        List<CRM_CONTATO> GetAllItens();
        CRM_CONTATO GetItemById(Int32 id);
    }
}
