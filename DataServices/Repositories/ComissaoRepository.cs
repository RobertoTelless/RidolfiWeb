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
    public class ComissaoRepository : RepositoryBase<COMISSAO>, IComissaoRepository
    {
        public COMISSAO GetItemById(Int32 id)
        {
            IQueryable<COMISSAO> query = Db.COMISSAO;
            query = query.Where(p => p.COMI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<COMISSAO> GetAllItens(Int32 idUsu)
        {
            IQueryable<COMISSAO> query = Db.COMISSAO.Where(p => p.COMI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<COMISSAO> ExecuteFilter(DateTime? dataRef, Int32? idAss)
        {
            List<COMISSAO> lista = new List<COMISSAO>();
            IQueryable<COMISSAO> query = Db.COMISSAO;
            if (dataRef != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.COMI_DT_REFERENCIA).Value.Month == DbFunctions.TruncateTime(dataRef).Value.Month & DbFunctions.TruncateTime(p.COMI_DT_REFERENCIA).Value.Year == DbFunctions.TruncateTime(dataRef).Value.Year);
            }
            if (query != null)
            {
                query = query.OrderByDescending(a => a.COMI_DT_REFERENCIA);
                lista = query.ToList<COMISSAO>();
            }
            return lista;
        }
    }
}
 