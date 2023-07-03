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
    public class AssinantePlanoRepository : RepositoryBase<ASSINANTE_PLANO>, IAssinantePlanoRepository
    {
        public List<ASSINANTE_PLANO> GetAllItens()
        {
            IQueryable<ASSINANTE_PLANO> query = Db.ASSINANTE_PLANO.Where(p => p.ASPL_IN_ATIVO == 1);
            return query.ToList();
        }

        public ASSINANTE_PLANO GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_PLANO> query = Db.ASSINANTE_PLANO.Where(p => p.ASPL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public ASSINANTE_PLANO GetByAssPlan(Int32 plan, Int32 assi)
        {
            IQueryable<ASSINANTE_PLANO> query = Db.ASSINANTE_PLANO;
            query = query.Where(x => x.PLAN_CD_ID == plan);
            query = query.Where(x => x.ASSI_CD_ID == assi);
            return query.FirstOrDefault();
        }

    }
}
 