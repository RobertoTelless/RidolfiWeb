using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class TRFRepository : RepositoryBase<TRF>, ITRFRepository
    {
        public TRF CheckExist(TRF conta)
        {
            IQueryable<TRF> query = Db.TRF;
            query = query.Where(p => p.TRF1_NM_NOME == conta.TRF1_NM_NOME);
            return query.FirstOrDefault();
        }

        public TRF GetItemById(Int32 id)
        {
            IQueryable<TRF> query = Db.TRF;
            query = query.Where(p => p.TRF1_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TRF> GetAllItens()
        {
            IQueryable<TRF> query = Db.TRF.Where(p => p.TRF1_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<TRF> GetAllItensAdm()
        {
            IQueryable<TRF> query = Db.TRF;
            return query.ToList();
        }

        public List<TRF> ExecuteFilter(String nome, Int32? uf)
        {
            List<TRF> lista = new List<TRF>();
            IQueryable<TRF> query = Db.TRF;
            if (uf > 0)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.TRF1_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.TRF1_NM_NOME);
                lista = query.ToList<TRF>();
            }
            return lista;
        }
    }
}
 