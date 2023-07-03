using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class TicketRepository : RepositoryBase<TICKET_ALIMENTACAO>, ITicketRepository
    {
        public TICKET_ALIMENTACAO CheckExist(TICKET_ALIMENTACAO tarefa, Int32 idUsu)
        {
            IQueryable<TICKET_ALIMENTACAO> query = Db.TICKET_ALIMENTACAO;
            query = query.Where(p => p.TICK_NM_NOME == tarefa.TICK_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == tarefa.ASSI_CD_ID);
            return query.FirstOrDefault();
        }

        public TICKET_ALIMENTACAO GetItemById(Int32 id)
        {
            IQueryable<TICKET_ALIMENTACAO> query = Db.TICKET_ALIMENTACAO;
            query = query.Where(p => p.TICK_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TICKET_ALIMENTACAO> GetAllItens(Int32 idUsu)
        {
            IQueryable<TICKET_ALIMENTACAO> query = Db.TICKET_ALIMENTACAO.Where(p => p.TICK_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<TICKET_ALIMENTACAO> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<TICKET_ALIMENTACAO> query = Db.TICKET_ALIMENTACAO;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

    }
}
 