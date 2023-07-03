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
    public class CRMPropostaComentarioRepository : RepositoryBase<CRM_PROPOSTA_ACOMPANHAMENTO>, ICRMPropostaComentarioRepository
    {
        public List<CRM_PROPOSTA_ACOMPANHAMENTO> GetAllItens()
        {
            return Db.CRM_PROPOSTA_ACOMPANHAMENTO.ToList();
        }

        public CRM_PROPOSTA_ACOMPANHAMENTO GetItemById(Int32 id)
        {
            IQueryable<CRM_PROPOSTA_ACOMPANHAMENTO> query = Db.CRM_PROPOSTA_ACOMPANHAMENTO.Where(p => p.PRAC_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 