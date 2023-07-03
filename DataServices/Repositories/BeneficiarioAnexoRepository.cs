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
    public class BeneficiarioAnexoRepository : RepositoryBase<BENEFICIARIO_ANEXO>, IBeneficiarioAnexoRepository
    {
        public List<BENEFICIARIO_ANEXO> GetAllItens()
        {
            return Db.BENEFICIARIO_ANEXO.ToList();
        }

        public BENEFICIARIO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<BENEFICIARIO_ANEXO> query = Db.BENEFICIARIO_ANEXO.Where(p => p.BEAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 