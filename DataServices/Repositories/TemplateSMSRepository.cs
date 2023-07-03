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
    public class TemplateSMSRepository : RepositoryBase<TEMPLATE_SMS>, ITemplateSMSRepository
    {
        public TEMPLATE_SMS GetByCode(String code, Int32 idAss)
        {
            IQueryable<TEMPLATE_SMS> query = Db.TEMPLATE_SMS.Where(p => p.TSMS_SG_SIGLA == code);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TEMPLATE_SMS GetItemById(Int32 id)
        {
            IQueryable<TEMPLATE_SMS> query = Db.TEMPLATE_SMS;
            query = query.Where(p => p.TSMS_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE_SMS> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TEMPLATE_SMS> query = Db.TEMPLATE_SMS;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TEMPLATE_SMS> GetAllItens(Int32 idAss)
        {
            IQueryable<TEMPLATE_SMS> query = Db.TEMPLATE_SMS.Where(p => p.TSMS_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public TEMPLATE_SMS CheckExist(TEMPLATE_SMS item, Int32 idAss)
        {
            IQueryable<TEMPLATE_SMS> query = Db.TEMPLATE_SMS;
            query = query.Where(p => p.TSMS_SG_SIGLA == item.TSMS_SG_SIGLA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE_SMS> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = new List<TEMPLATE_SMS>();
            IQueryable<TEMPLATE_SMS> query = Db.TEMPLATE_SMS;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.TSMS_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(sigla))
            {
                query = query.Where(p => p.TSMS_SG_SIGLA.Contains(sigla));
            }
            if (!String.IsNullOrEmpty(conteudo))
            {
                query = query.Where(p => p.TSMS_TX_CORPO.Contains(conteudo));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.TSMS_SG_SIGLA);
                lista = query.ToList<TEMPLATE_SMS>();
            }
            return lista;
        }
    }
}
 