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
    public class VaraRepository : RepositoryBase<VARA>, IVaraRepository
    {
        public VARA CheckExist(VARA conta)
        {
            IQueryable<VARA> query = Db.VARA;
            query = query.Where(p => p.VARA_NM_NOME == conta.VARA_NM_NOME);
            return query.FirstOrDefault();
        }

        public VARA GetItemById(Int32 id)
        {
            IQueryable<VARA> query = Db.VARA;
            query = query.Where(p => p.VARA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<VARA> GetAllItens()
        {
            IQueryable<VARA> query = Db.VARA.Where(p => p.VARA_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<VARA> GetAllItensAdm()
        {
            IQueryable<VARA> query = Db.VARA;
            return query.ToList();
        }

        public List<VARA> ExecuteFilter(String nome, Int32? trf)
        {
            List<VARA> lista = new List<VARA>();
            IQueryable<VARA> query = Db.VARA;
            if (trf > 0)
            {
                query = query.Where(p => p.TRF1_CD_ID == trf);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.VARA_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.VARA_NM_NOME);
                lista = query.ToList<VARA>();
            }
            return lista;
        }
    }
}
 