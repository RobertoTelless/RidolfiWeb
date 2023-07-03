using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITicketRepository : IRepositoryBase<TICKET_ALIMENTACAO>
    {
        TICKET_ALIMENTACAO CheckExist(TICKET_ALIMENTACAO item, Int32 idAss);
        TICKET_ALIMENTACAO GetItemById(Int32 id);
        List<TICKET_ALIMENTACAO> GetAllItens(Int32 idAss);
        List<TICKET_ALIMENTACAO> GetAllItensAdm(Int32 idAss);
    }
}
