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
    public class TemplateEMailRepository : RepositoryBase<TEMPLATE_EMAIL>, ITemplateEMailRepository
    {
        public TEMPLATE_EMAIL GetByCode(String code, Int32 idAss)
        {
            IQueryable<TEMPLATE_EMAIL> query = Db.TEMPLATE_EMAIL.Where(p => p.TEEM_SG_SIGLA == code);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TEMPLATE_EMAIL GetItemById(Int32 id)
        {
            IQueryable<TEMPLATE_EMAIL> query = Db.TEMPLATE_EMAIL;
            query = query.Where(p => p.TEEM_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE_EMAIL> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TEMPLATE_EMAIL> query = Db.TEMPLATE_EMAIL;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TEMPLATE_EMAIL> GetAllItens(Int32 idAss)
        {
            IQueryable<TEMPLATE_EMAIL> query = Db.TEMPLATE_EMAIL.Where(p => p.TEEM_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public TEMPLATE_EMAIL CheckExist(TEMPLATE_EMAIL item, Int32 idAss)
        {
            IQueryable<TEMPLATE_EMAIL> query = Db.TEMPLATE_EMAIL;
            query = query.Where(p => p.TEEM_SG_SIGLA == item.TEEM_SG_SIGLA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE_EMAIL> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss)
        {
            List<TEMPLATE_EMAIL> lista = new List<TEMPLATE_EMAIL>();
            IQueryable<TEMPLATE_EMAIL> query = Db.TEMPLATE_EMAIL;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.TEEM_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(sigla))
            {
                query = query.Where(p => p.TEEM_SG_SIGLA.Contains(sigla));
            }
            if (!String.IsNullOrEmpty(conteudo))
            {
                query = query.Where(p => p.TEEM_TX_CORPO.Contains(conteudo));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.TEEM_SG_SIGLA);
                lista = query.ToList<TEMPLATE_EMAIL>();
            }
            return lista;
        }
    }
}
 