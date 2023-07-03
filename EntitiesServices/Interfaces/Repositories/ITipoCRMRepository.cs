using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoCRMRepository : IRepositoryBase<TIPO_CRM>
    {
        List<TIPO_CRM> GetAllItens();
        TIPO_CRM GetItemById(Int32 id);
        List<TIPO_CRM> GetAllItensAdm();
    }
}
