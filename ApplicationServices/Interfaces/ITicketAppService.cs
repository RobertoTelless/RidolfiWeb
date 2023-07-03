using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITicketAppService : IAppServiceBase<TICKET_ALIMENTACAO>
    {
        Int32 ValidateCreate(TICKET_ALIMENTACAO item, USUARIO usuario);
        Int32 ValidateEdit(TICKET_ALIMENTACAO item, TICKET_ALIMENTACAO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(TICKET_ALIMENTACAO item, TICKET_ALIMENTACAO itemAntes);
        Int32 ValidateDelete(TICKET_ALIMENTACAO item, USUARIO usuario);
        Int32 ValidateReativar(TICKET_ALIMENTACAO item, USUARIO usuario);

        TICKET_ALIMENTACAO CheckExist(TICKET_ALIMENTACAO item, Int32 idAss);
        TICKET_ALIMENTACAO GetItemById(Int32 id);
        List<TICKET_ALIMENTACAO> GetAllItens(Int32 idAss);
        List<TICKET_ALIMENTACAO> GetAllItensAdm(Int32 idAss);

    }
}
