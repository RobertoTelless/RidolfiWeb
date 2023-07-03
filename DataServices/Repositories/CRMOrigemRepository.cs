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
    public class CRMOrigemRepository : RepositoryBase<CRM_ORIGEM>, ICRMOrigemRepository
    {
        public CRM_ORIGEM CheckExist(CRM_ORIGEM conta, Int32 idAss)
        {
            IQueryable<CRM_ORIGEM> query = Db.CRM_ORIGEM;
            query = query.Where(p => p.CROR_NM_NOME == conta.CROR_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CRM_ORIGEM GetItemById(Int32 id)
        {
            IQueryable<CRM_ORIGEM> query = Db.CRM_ORIGEM;
            query = query.Where(p => p.CROR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CRM_ORIGEM> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CRM_ORIGEM> query = Db.CRM_ORIGEM;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CRM_ORIGEM> GetAllItens(Int32 idAss)
        {
            IQueryable<CRM_ORIGEM> query = Db.CRM_ORIGEM.Where(p => p.CROR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 