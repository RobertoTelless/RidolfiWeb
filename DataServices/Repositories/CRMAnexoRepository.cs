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
    public class CRMAnexoRepository : RepositoryBase<CRM_ANEXO>, ICRMAnexoRepository
    {
        public List<CRM_ANEXO> GetAllItens()
        {
            return Db.CRM_ANEXO.ToList();
        }

        public CRM_ANEXO GetItemById(Int32 id)
        {
            IQueryable<CRM_ANEXO> query = Db.CRM_ANEXO.Where(p => p.CRAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 