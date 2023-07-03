using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMComentarioRepository : IRepositoryBase<CRM_COMENTARIO>
    {
        List<CRM_COMENTARIO> GetAllItens();
        CRM_COMENTARIO GetItemById(Int32 id);
    }
}
