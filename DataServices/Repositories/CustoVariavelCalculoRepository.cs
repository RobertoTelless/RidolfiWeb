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
    public class CustoVariavelCalculoRepository : RepositoryBase<CUSTO_VARIAVEL_CALCULO>, ICustoVariavelCalculoRepository
    {
        public CUSTO_VARIAVEL_CALCULO GetItemById(Int32 id)
        {
            IQueryable<CUSTO_VARIAVEL_CALCULO> query = Db.CUSTO_VARIAVEL_CALCULO;
            query = query.Where(p => p.CVCA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CUSTO_VARIAVEL_CALCULO> GetAllItens(Int32 idUsu)
        {
            IQueryable<CUSTO_VARIAVEL_CALCULO> query = Db.CUSTO_VARIAVEL_CALCULO.Where(p => p.CVCA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<CUSTO_VARIAVEL_CALCULO> ExecuteFilter(DateTime? dataRef, Int32? idAss)
        {
            List<CUSTO_VARIAVEL_CALCULO> lista = new List<CUSTO_VARIAVEL_CALCULO>();
            IQueryable<CUSTO_VARIAVEL_CALCULO> query = Db.CUSTO_VARIAVEL_CALCULO;
            if (dataRef != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CVCA_DT_REFERENCIA).Value.Month == DbFunctions.TruncateTime(dataRef).Value.Month & DbFunctions.TruncateTime(p.CVCA_DT_REFERENCIA).Value.Year == DbFunctions.TruncateTime(dataRef).Value.Year);
            }
            if (query != null)
            {
                query = query.OrderByDescending(a => a.CVCA_DT_REFERENCIA);
                lista = query.ToList<CUSTO_VARIAVEL_CALCULO>();
            }
            return lista;
        }
    }
}
 