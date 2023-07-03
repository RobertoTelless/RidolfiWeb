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
    public class EmpresaTicketRepository : RepositoryBase<EMPRESA_TICKET>, IEmpresaTicketRepository
    {
        public EMPRESA_TICKET CheckExist(EMPRESA_TICKET tarefa, Int32 idUsu)
        {
            IQueryable<EMPRESA_TICKET> query = Db.EMPRESA_TICKET;
            query = query.Where(p => p.EMPR_CD_ID == tarefa.EMPR_CD_ID);
            query = query.Where(p => p.TICK_CD_ID == tarefa.TICK_CD_ID);
            query = query.Where(p => p.EMTI_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public List<EMPRESA_TICKET> GetAllItens()
        {
            return Db.EMPRESA_TICKET.ToList();
        }

        public EMPRESA_TICKET GetItemById(Int32 id)
        {
            IQueryable<EMPRESA_TICKET> query = Db.EMPRESA_TICKET.Where(p => p.EMTI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public EMPRESA_TICKET GetByEmpresaTicket(Int32 empresa, Int32 ticket)
        {
            IQueryable<EMPRESA_TICKET> query = Db.EMPRESA_TICKET;
            query = query.Where(x => x.EMPR_CD_ID == empresa);
            query = query.Where(x => x.TICK_CD_ID == ticket);
            return query.FirstOrDefault();
        }
    }
}
 