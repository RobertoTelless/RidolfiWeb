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
    public class CustoVariavelHistoricoRepository : RepositoryBase<CUSTO_VARIAVEL_HISTORICO>, ICustoVariavelHistoricoRepository
    {
        public CUSTO_VARIAVEL_HISTORICO GetItemById(Int32 id)
        {
            IQueryable<CUSTO_VARIAVEL_HISTORICO> query = Db.CUSTO_VARIAVEL_HISTORICO;
            query = query.Where(p => p.CVHI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CUSTO_VARIAVEL_HISTORICO> GetAllItens(Int32 idUsu)
        {
            IQueryable<CUSTO_VARIAVEL_HISTORICO> query = Db.CUSTO_VARIAVEL_HISTORICO.Where(p => p.CVHI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<CUSTO_VARIAVEL_HISTORICO> ExecuteFilter(DateTime? dataRef, Int32? idAss)
        {
            List<CUSTO_VARIAVEL_HISTORICO> lista = new List<CUSTO_VARIAVEL_HISTORICO>();
            IQueryable<CUSTO_VARIAVEL_HISTORICO> query = Db.CUSTO_VARIAVEL_HISTORICO;
            if (dataRef != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CVHI_DT_REFERENCIA).Value.Month == DbFunctions.TruncateTime(dataRef).Value.Month & DbFunctions.TruncateTime(p.CVHI_DT_REFERENCIA).Value.Year == DbFunctions.TruncateTime(dataRef).Value.Year);
            }
            if (query != null)
            {
                query = query.OrderByDescending(a => a.CVHI_DT_REFERENCIA);
                lista = query.ToList<CUSTO_VARIAVEL_HISTORICO>();
            }
            return lista;
        }
    }
}
 