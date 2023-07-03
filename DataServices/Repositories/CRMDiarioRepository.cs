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
    public class CRMDiarioRepository : RepositoryBase<DIARIO_PROCESSO>, ICRMDiarioRepository
    {
        public DIARIO_PROCESSO GetByDate(DateTime data)
        {
            IQueryable<DIARIO_PROCESSO> query = Db.DIARIO_PROCESSO;
            query = query.Where(p => p.DIPR_DT_DATA == data);
            return query.FirstOrDefault();
        }

        public DIARIO_PROCESSO GetItemById(Int32 id)
        {
            IQueryable<DIARIO_PROCESSO> query = Db.DIARIO_PROCESSO;
            query = query.Where(p => p.DIPR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<DIARIO_PROCESSO> GetAllItens(Int32 idAss)
        {
            IQueryable<DIARIO_PROCESSO> query = Db.DIARIO_PROCESSO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.DIPR_DT_DATA);
            return query.ToList();
        }

        public List<DIARIO_PROCESSO> ExecuteFilter(Int32? processo, DateTime? dataInicio, DateTime? dataFim, Int32? usuario, String operacao, String descricao, Int32 idAss)
        {
            List<DIARIO_PROCESSO> lista = new List<DIARIO_PROCESSO>();
            IQueryable<DIARIO_PROCESSO> query = Db.DIARIO_PROCESSO;
            if (dataInicio != DateTime.MinValue & dataFim == DateTime.MinValue)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.DIPR_DT_DATA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if (dataInicio == DateTime.MinValue & dataFim != DateTime.MinValue)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.DIPR_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (dataInicio != DateTime.MinValue & dataFim != DateTime.MinValue)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.DIPR_DT_DATA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.DIPR_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (usuario > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (processo > 0)
            {
                query = query.Where(p => p.CRM1_CD_ID == processo);
            }
            if (!String.IsNullOrEmpty(operacao))
            {
                query = query.Where(p => p.DIPR_NM_OPERACAO.Contains(operacao));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.DIPR_DS_DESCRICAO.Contains(descricao));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.DIPR_DT_DATA);
                lista = query.ToList<DIARIO_PROCESSO>();
            }
            return lista;
        }

    }
}
 