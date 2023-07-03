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
    public class CRMComentarioRepository : RepositoryBase<CRM_COMENTARIO>, ICRMComentarioRepository
    {
        public List<CRM_COMENTARIO> GetAllItens()
        {
            return Db.CRM_COMENTARIO.ToList();
        }

        public CRM_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<CRM_COMENTARIO> query = Db.CRM_COMENTARIO.Where(p => p.CRCM_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 