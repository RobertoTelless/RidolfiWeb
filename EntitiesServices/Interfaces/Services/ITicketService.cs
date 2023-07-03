using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITicketService : IServiceBase<TICKET_ALIMENTACAO>
    {
        Int32 Create(TICKET_ALIMENTACAO perfil, LOG log);
        Int32 Create(TICKET_ALIMENTACAO perfil);
        Int32 Edit(TICKET_ALIMENTACAO perfil, LOG log);
        Int32 Edit(TICKET_ALIMENTACAO perfil);
        Int32 Delete(TICKET_ALIMENTACAO perfil, LOG log);

        TICKET_ALIMENTACAO CheckExist(TICKET_ALIMENTACAO item, Int32 idAss);
        TICKET_ALIMENTACAO GetItemById(Int32 id);
        List<TICKET_ALIMENTACAO> GetAllItens(Int32 idAss);
        List<TICKET_ALIMENTACAO> GetAllItensAdm(Int32 idAss);

    }
}
