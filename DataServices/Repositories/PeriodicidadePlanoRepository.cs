using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PeriodicidadePlanoRepository : RepositoryBase<PLANO_PERIODICIDADE>, IPeriodicidadePlanoRepository
    {
        public PLANO_PERIODICIDADE GetItemById(Int32 id)
        {
            IQueryable<PLANO_PERIODICIDADE> query = Db.PLANO_PERIODICIDADE;
            query = query.Where(p => p.PLPE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PLANO_PERIODICIDADE> GetAllItens()
        {
            IQueryable<PLANO_PERIODICIDADE> query = Db.PLANO_PERIODICIDADE.Where(p => p.PLPE_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<PLANO_PERIODICIDADE> GetAllItensAdm()
        {
            IQueryable<PLANO_PERIODICIDADE> query = Db.PLANO_PERIODICIDADE;
            return query.ToList();
        }
    }
}
