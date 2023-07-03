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
    public class CRMContatoRepository : RepositoryBase<CRM_CONTATO>, ICRMContatoRepository
    {
        public List<CRM_CONTATO> GetAllItens()
        {
            return Db.CRM_CONTATO.ToList();
        }

        public CRM_CONTATO GetItemById(Int32 id)
        {
            IQueryable<CRM_CONTATO> query = Db.CRM_CONTATO.Where(p => p.CRCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 