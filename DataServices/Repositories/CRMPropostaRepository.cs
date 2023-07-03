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
    public class CRMPropostaRepository : RepositoryBase<CRM_PROPOSTA>, ICRMPropostaRepository
    {
        public List<CRM_PROPOSTA> GetAllItens(Int32 idUsu)
        {
            IQueryable<CRM_PROPOSTA> query = Db.CRM_PROPOSTA.Where(p => p.CRPR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public CRM_PROPOSTA GetItemById(Int32 id)
        {
            IQueryable<CRM_PROPOSTA> query = Db.CRM_PROPOSTA.Where(p => p.CRPR_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.FORMA_ENVIO);
            query = query.Include(p => p.FORMA_FRETE);
            query = query.Include(p => p.MOTIVO_CANCELAMENTO);
            query = query.Include(p => p.TEMPLATE_PROPOSTA);
            query = query.Include(p => p.CRM_PROPOSTA_ACOMPANHAMENTO);
            query = query.Include(p => p.CRM_PROPOSTA_ANEXO);
            return query.FirstOrDefault();
        }
    }
}
 