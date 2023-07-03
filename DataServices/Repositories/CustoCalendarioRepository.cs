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
    public class CustoCalendarioRepository : RepositoryBase<CUSTO_FIXO_CALENDARIO>, ICustoCalendarioRepository
    {
        public CUSTO_FIXO_CALENDARIO CheckExist(CUSTO_FIXO_CALENDARIO conta, Int32 idAss)
        {
            IQueryable<CUSTO_FIXO_CALENDARIO> query = Db.CUSTO_FIXO_CALENDARIO;
            query = query.Where(p => p.CFCD_DT_VENCIMENTO == conta.CFCD_DT_VENCIMENTO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.CUFX_CD_ID == conta.CUFX_CD_ID);
            return query.FirstOrDefault();
        }

        public List<CUSTO_FIXO_CALENDARIO> GetAllItens(Int32 idAss)
        {
            return Db.CUSTO_FIXO_CALENDARIO.ToList();
        }

        public CUSTO_FIXO_CALENDARIO GetItemById(Int32 id)
        {
            IQueryable<CUSTO_FIXO_CALENDARIO> query = Db.CUSTO_FIXO_CALENDARIO.Where(p => p.CFCD_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 