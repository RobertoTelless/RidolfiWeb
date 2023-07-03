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
    public class CustoHistoricoRepository : RepositoryBase<CUSTO_HISTORICO>, ICustoHistoricoRepository
    {
        public CUSTO_HISTORICO GetItemById(Int32 id)
        {
            IQueryable<CUSTO_HISTORICO> query = Db.CUSTO_HISTORICO;
            query = query.Where(p => p.CUHI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CUSTO_HISTORICO> GetAllItens(Int32 idUsu)
        {
            IQueryable<CUSTO_HISTORICO> query = Db.CUSTO_HISTORICO.Where(p => p.CUHI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<CUSTO_HISTORICO> ExecuteFilter(DateTime? dataRef, Int32? idAss)
        {
            List<CUSTO_HISTORICO> lista = new List<CUSTO_HISTORICO>();
            IQueryable<CUSTO_HISTORICO> query = Db.CUSTO_HISTORICO;
            if (dataRef != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CUHI_DT_REFERENCIA).Value.Month == DbFunctions.TruncateTime(dataRef).Value.Month & DbFunctions.TruncateTime(p.CUHI_DT_REFERENCIA).Value.Year == DbFunctions.TruncateTime(dataRef).Value.Year);
            }
            if (query != null)
            {
                query = query.OrderByDescending(a => a.CUHI_DT_REFERENCIA);
                lista = query.ToList<CUSTO_HISTORICO>();
            }
            return lista;
        }
    }
}
 