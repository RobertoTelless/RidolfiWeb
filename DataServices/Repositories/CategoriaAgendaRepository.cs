using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class CategoriaAgendaRepository : RepositoryBase<CATEGORIA_AGENDA>, ICategoriaAgendaRepository
    {
        public CATEGORIA_AGENDA CheckExist(CATEGORIA_AGENDA conta, Int32 idAss)
        {
            IQueryable<CATEGORIA_AGENDA> query = Db.CATEGORIA_AGENDA;
            query = query.Where(p => p.CAAG_NM_NOME == conta.CAAG_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CATEGORIA_AGENDA GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_AGENDA> query = Db.CATEGORIA_AGENDA;
            query = query.Where(p => p.CAAG_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_AGENDA> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_AGENDA> query = Db.CATEGORIA_AGENDA.Where(p => p.CAAG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_AGENDA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_AGENDA> query = Db.CATEGORIA_AGENDA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public async Task<IEnumerable<CATEGORIA_AGENDA>> GetAllItensAsync(Int32 idAss)
        {
            IQueryable<CATEGORIA_AGENDA> query = Db.CATEGORIA_AGENDA.Where(p => p.CAAG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return await query.ToListAsync();
        }
    }
}
