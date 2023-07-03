using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmpresaTicketRepository : IRepositoryBase<EMPRESA_TICKET>
    {
        EMPRESA_TICKET CheckExist(EMPRESA_TICKET item, Int32 idAss);
        List<EMPRESA_TICKET> GetAllItens();
        EMPRESA_TICKET GetItemById(Int32 id);
        EMPRESA_TICKET GetByEmpresaTicket(Int32 empresa, Int32 ticket);
    }
}
