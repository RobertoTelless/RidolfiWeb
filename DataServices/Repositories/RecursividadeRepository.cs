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
    public class RecursividadeRepository : RepositoryBase<RECURSIVIDADE>, IRecursividadeRepository
    {
        public RECURSIVIDADE CheckExist(RECURSIVIDADE conta, Int32 idAss)
        {
            IQueryable<RECURSIVIDADE> query = Db.RECURSIVIDADE;
            query = query.Where(p => p.RECU_NM_NOME == conta.RECU_NM_NOME);
            query = query.Where(p => p.RECU_DT_CRIACAO == conta.RECU_DT_CRIACAO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public RECURSIVIDADE GetItemById(Int32 id)
        {
            IQueryable<RECURSIVIDADE> query = Db.RECURSIVIDADE;
            query = query.Where(p => p.RECU_CD_ID == id);
            query = query.Include(p => p.RECURSIVIDADE_DATA);
            query = query.Include(p => p.RECURSIVIDADE_DESTINO);
            return query.FirstOrDefault();
        }

        public List<RECURSIVIDADE> GetAllItens(Int32 idAss)
        {
            IQueryable<RECURSIVIDADE> query = Db.RECURSIVIDADE.Where(p => p.RECU_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<RECURSIVIDADE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<RECURSIVIDADE> query = Db.RECURSIVIDADE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<RECURSIVIDADE> ExecuteFilter(Int32? tipoMensagem, String nome, DateTime? dataInicio, DateTime? dataFim, String texto, Int32 idAss)
        {
            List<RECURSIVIDADE> lista = new List<RECURSIVIDADE>();
            IQueryable<RECURSIVIDADE> query = Db.RECURSIVIDADE;
            if (tipoMensagem != null)
            {
                query = query.Where(p => p.RECU_IN_TIPO_MENSAGEM == tipoMensagem);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.RECU_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(texto))
            {
                query = query.Where(p => p.RECU_TX_TEXTO.Contains(texto));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RECU_DT_CRIACAO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RECU_DT_CRIACAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RECU_DT_CRIACAO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.RECU_DT_CRIACAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.RECU_IN_ATIVO == 1);
                query = query.OrderBy(a => a.RECU_DT_CRIACAO);
                lista = query.ToList<RECURSIVIDADE>();
            }
            return lista;
        }
    }
}
 