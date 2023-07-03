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
    public class PesquisaRepository : RepositoryBase<PESQUISA>, IPesquisaRepository
    {
        public PESQUISA CheckExist(PESQUISA conta, Int32 idAss)
        {
            IQueryable<PESQUISA> query = Db.PESQUISA;
            query = query.Where(p => p.PESQ_NM_NOME == conta.PESQ_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PESQUISA GetItemById(Int32 id)
        {
            IQueryable<PESQUISA> query = Db.PESQUISA;
            query = query.Where(p => p.PESQ_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PESQUISA> GetAllItens(Int32 idAss)
        {
            IQueryable<PESQUISA> query = Db.PESQUISA.Where(p => p.PESQ_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PESQUISA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PESQUISA> query = Db.PESQUISA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PESQUISA> ExecuteFilter(Int32? catId, String nome, String descricao, String campanha, Int32 idAss)
        {
            List<PESQUISA> lista = new List<PESQUISA>();
            IQueryable<PESQUISA> query = Db.PESQUISA;
            if (catId != null & catId > 0)
            {
                query = query.Where(p => p.TIPS_CD_ID == catId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PESQ_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(campanha))
            {
                query = query.Where(p => p.PESQ_NM_CAMPANHA.Contains(campanha));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.PESQ_DS_DESCRICAO.Contains(descricao));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.PESQ_NM_NOME);
                lista = query.ToList<PESQUISA>();
            }
            return lista;
        }
    }
}
 