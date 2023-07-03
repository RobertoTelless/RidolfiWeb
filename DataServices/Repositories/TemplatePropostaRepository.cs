using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class TemplatePropostaRepository : RepositoryBase<TEMPLATE_PROPOSTA>, ITemplatePropostaRepository
    {
        public TEMPLATE_PROPOSTA GetByCode(String code, Int32 idAss)
        {
            IQueryable<TEMPLATE_PROPOSTA> query = Db.TEMPLATE_PROPOSTA.Where(p => p.TEPR_SG_SIGLA == code);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TEMPLATE_PROPOSTA GetItemById(Int32 id)
        {
            IQueryable<TEMPLATE_PROPOSTA> query = Db.TEMPLATE_PROPOSTA;
            query = query.Where(p => p.TEPR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE_PROPOSTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TEMPLATE_PROPOSTA> query = Db.TEMPLATE_PROPOSTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TEMPLATE_PROPOSTA> GetAllItens(Int32 idAss)
        {
            IQueryable<TEMPLATE_PROPOSTA> query = Db.TEMPLATE_PROPOSTA.Where(p => p.TEPR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public TEMPLATE_PROPOSTA CheckExist(TEMPLATE_PROPOSTA item, Int32 idAss)
        {
            IQueryable<TEMPLATE_PROPOSTA> query = Db.TEMPLATE_PROPOSTA;
            query = query.Where(p => p.TEPR_SG_SIGLA == item.TEPR_SG_SIGLA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE_PROPOSTA> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss)
        {
            List<TEMPLATE_PROPOSTA> lista = new List<TEMPLATE_PROPOSTA>();
            IQueryable<TEMPLATE_PROPOSTA> query = Db.TEMPLATE_PROPOSTA;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.TEPR_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(sigla))
            {
                query = query.Where(p => p.TEPR_SG_SIGLA.Contains(sigla));
            }
            if (!String.IsNullOrEmpty(conteudo))
            {
                query = query.Where(p => p.TEPR_TX_TEXTO.Contains(conteudo));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.TEPR_SG_SIGLA);
                lista = query.ToList<TEMPLATE_PROPOSTA>();
            }
            return lista;
        }
    }
}
 