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
    public class PlanoRepository : RepositoryBase<PLANO>, IPlanoRepository
    {
        public PLANO CheckExist(PLANO conta)
        {
            IQueryable<PLANO> query = Db.PLANO;
            query = query.Where(p => p.PLAN_NM_NOME == conta.PLAN_NM_NOME);
            return query.FirstOrDefault();
        }

        public PLANO GetItemById(Int32 id)
        {
            IQueryable<PLANO> query = Db.PLANO;
            query = query.Where(p => p.PLAN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PLANO> GetAllItens()
        {
            IQueryable<PLANO> query = Db.PLANO.Where(p => p.PLAN_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<PLANO> GetAllValidos()
        {
            IQueryable<PLANO> query = Db.PLANO.Where(p => p.PLAN_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.PLAN_DT_VALIDADE) >= DbFunctions.TruncateTime(DateTime.Today.Date));
            return query.ToList();
        }

        public List<PLANO> GetAllItensAdm()
        {
            IQueryable<PLANO> query = Db.PLANO;
            return query.ToList();
        }

        public List<PLANO> ExecuteFilter(String nome, String descricao)
        {
            List<PLANO> lista = new List<PLANO>();
            IQueryable<PLANO> query = Db.PLANO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PLAN_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.PLAN_DS_DESCRICAO.Contains(descricao));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.PLAN_NM_NOME);
                lista = query.ToList<PLANO>();
            }
            return lista;
        }

    }
}
 