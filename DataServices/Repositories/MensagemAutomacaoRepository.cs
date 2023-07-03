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
using CrossCutting;

namespace DataServices.Repositories
{
    public class MensagemAutomacaoRepository : RepositoryBase<MENSAGEM_AUTOMACAO>, IMensagemAutomacaoRepository
    {
        public MENSAGEM_AUTOMACAO CheckExist(MENSAGEM_AUTOMACAO conta, Int32 idAss)
        {
            IQueryable<MENSAGEM_AUTOMACAO> query = Db.MENSAGEM_AUTOMACAO;
            query = query.Where(p => p.GRUP_CD_ID == conta.GRUP_CD_ID);
            query = query.Where(p => p.MEAU_IN_TIPO == conta.MEAU_IN_TIPO);
            if (conta.MEAU_IN_TIPO == 1)
            {
                query = query.Where(p => p.TEEM_CD_ID == conta.TEEM_CD_ID);
            }
            if (conta.MEAU_IN_TIPO == 2)
            {
                query = query.Where(p => p.TSMS_CD_ID == conta.TSMS_CD_ID);
            }
            query = query.Where(p => p. MEAU_IN_TIPO_ENVIO == conta.MEAU_IN_TIPO_ENVIO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public MENSAGEM_AUTOMACAO GetItemById(Int32 id)
        {
            IQueryable<MENSAGEM_AUTOMACAO> query = Db.MENSAGEM_AUTOMACAO;
            query = query.Where(p => p.MEAU_CD_ID == id);
            query = query.Include(p => p.MENSAGEM_AUTOMACAO_DATAS);
            return query.FirstOrDefault();
        }

        public List<MENSAGEM_AUTOMACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<MENSAGEM_AUTOMACAO> query = Db.MENSAGEM_AUTOMACAO.Where(p => p.MEAU_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<MENSAGEM_AUTOMACAO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MENSAGEM_AUTOMACAO> query = Db.MENSAGEM_AUTOMACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }


        public List<MENSAGEM_AUTOMACAO> ExecuteFilter(Int32? tipo, Int32? grupo, String nome, Int32 idAss)
        {
            List<MENSAGEM_AUTOMACAO> lista = new List<MENSAGEM_AUTOMACAO>();
            IQueryable<MENSAGEM_AUTOMACAO> query = Db.MENSAGEM_AUTOMACAO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.MEAU_DS_DESCRICAO.Contains(nome));
            }
            if (tipo > 0)
            {
                query = query.Where(p => p.MEAU_IN_TIPO == tipo);
            }
            if (grupo > 0)
            {
                query = query.Where(p => p.GRUP_CD_ID == grupo);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                lista = query.ToList<MENSAGEM_AUTOMACAO>();
            }
            return lista;
        }
    }
}
 