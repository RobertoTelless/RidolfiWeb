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
    public class CRMAcaoRepository : RepositoryBase<CRM_ACAO>, ICRMAcaoRepository
    {
        public List<CRM_ACAO> GetAllItens(Int32 idUsu)
        {
            IQueryable<CRM_ACAO> query = Db.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            query = query.Where(p => p.CRAC_IN_ATIVO == 1);
            return query.ToList();
        }

        public CRM_ACAO GetItemById(Int32 id)
        {
            IQueryable<CRM_ACAO> query = Db.CRM_ACAO.Where(p => p.CRAC_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 