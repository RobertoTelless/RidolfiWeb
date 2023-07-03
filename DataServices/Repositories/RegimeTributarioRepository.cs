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
    public class RegimeTributarioRepository : RepositoryBase<REGIME_TRIBUTARIO>, IRegimeTributarioRepository
    {
        public REGIME_TRIBUTARIO CheckExist(REGIME_TRIBUTARIO conta)
        {
            IQueryable<REGIME_TRIBUTARIO> query = Db.REGIME_TRIBUTARIO;
            query = query.Where(p => p.RETR_NM_NOME == conta.RETR_NM_NOME);
            return query.FirstOrDefault();
        }

        public REGIME_TRIBUTARIO GetItemById(Int32 id)
        {
            IQueryable<REGIME_TRIBUTARIO> query = Db.REGIME_TRIBUTARIO;
            query = query.Where(p => p.RETR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<REGIME_TRIBUTARIO> GetAllItensAdm()
        {
            IQueryable<REGIME_TRIBUTARIO> query = Db.REGIME_TRIBUTARIO;
            return query.ToList();
        }

        public List<REGIME_TRIBUTARIO> GetAllItens()
        {
            IQueryable<REGIME_TRIBUTARIO> query = Db.REGIME_TRIBUTARIO.Where(p => p.RETR_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 