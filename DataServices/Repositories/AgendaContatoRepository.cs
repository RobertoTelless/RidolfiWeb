using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class AgendaContatoRepository : RepositoryBase<AGENDA_CONTATO>, IAgendaContatoRepository
    {
        public List<AGENDA_CONTATO> GetAllItens(Int32 idAgenda)
        {
            return Db.AGENDA_CONTATO.ToList();
        }

        public AGENDA_CONTATO GetItemById(Int32 id)
        {
            IQueryable<AGENDA_CONTATO> query = Db.AGENDA_CONTATO.Where(p => p.AGCO_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 