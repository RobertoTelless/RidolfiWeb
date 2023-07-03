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
    public class CRMFollowRepository : RepositoryBase<CRM_FOLLOW>, ICRMFollowRepository
    {
        public List<CRM_FOLLOW> GetAllItens(Int32 idAss)
        {
            IQueryable<CRM_FOLLOW> query = Db.CRM_FOLLOW.Where(p => p.CRFL_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public CRM_FOLLOW GetItemById(Int32 id)
        {
            IQueryable<CRM_FOLLOW> query = Db.CRM_FOLLOW.Where(p => p.CRFL_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }
    }
}
 