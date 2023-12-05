using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAgendaContatoRepository : IRepositoryBase<AGENDA_CONTATO>
    {
        List<AGENDA_CONTATO> GetAllItens(Int32 idAgenda);
        AGENDA_CONTATO GetItemById(Int32 id);

    }
}
