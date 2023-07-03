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
    public class CRMPropostaAnexoRepository : RepositoryBase<CRM_PROPOSTA_ANEXO>, ICRMPropostaAnexoRepository
    {
        public List<CRM_PROPOSTA_ANEXO> GetAllItens()
        {
            return Db.CRM_PROPOSTA_ANEXO.ToList();
        }

        public CRM_PROPOSTA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<CRM_PROPOSTA_ANEXO> query = Db.CRM_PROPOSTA_ANEXO.Where(p => p.CRPA_IN_ATIVO == id);
            return query.FirstOrDefault();
        }
    }
}
 