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
    public class CustoFixoRepository : RepositoryBase<CUSTO_FIXO>, ICustoFixoRepository
    {
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public CUSTO_FIXO CheckExist(CUSTO_FIXO cliente, Int32 idAss)
        {
            IQueryable<CUSTO_FIXO> query = Db.CUSTO_FIXO;
            if (cliente.CACF_CD_ID != null)
            {
                query = query.Where(p => p.CACF_CD_ID == cliente.CACF_CD_ID);
            }
            if (cliente.CUFX_NM_NOME != null)
            {
                query = query.Where(p => p.CUFX_NM_NOME == cliente.CUFX_NM_NOME);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CUSTO_FIXO GetItemById(Int32 id)
        {
            IQueryable<CUSTO_FIXO> query = Db.CUSTO_FIXO;
            query = query.Where(p => p.CUFX_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CUSTO_FIXO> GetAllItens(Int32 idAss)
        {
            IQueryable<CUSTO_FIXO> query = Db.CUSTO_FIXO.Where(p => p.CUFX_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CUFX_NM_NOME);
            return query.ToList();
        }

        public List<CUSTO_FIXO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CUSTO_FIXO> query = Db.CUSTO_FIXO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CUFX_NM_NOME);
            return query.ToList();
        }

        public List<CUSTO_FIXO> ExecuteFilter(Int32? catId, String nome, DateTime? dataInicio, DateTime? dataFinal, Int32 idAss)
        {
            List<CUSTO_FIXO> lista = new List<CUSTO_FIXO>();
            IQueryable<CUSTO_FIXO> query = Db.CUSTO_FIXO;
            if (catId != null)
            {
                query = query.Where(p => p.CACF_CD_ID== catId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CUFX_NM_NOME.Contains(nome));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFinal == DateTime.MinValue || dataFinal == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CUFX_DT_INICIO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFinal != DateTime.MinValue & dataFinal != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CUFX_DT_TERMINO) <= DbFunctions.TruncateTime(dataFinal));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFinal != DateTime.MinValue & dataFinal != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CUFX_DT_INICIO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.CUFX_DT_TERMINO) <= DbFunctions.TruncateTime(dataFinal));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CUFX_NM_NOME);
                lista = query.ToList<CUSTO_FIXO>();
            }
            return lista;
        }

        public async Task<List<CUSTO_FIXO>> GetAllItensAsync(Int32 idAss)
        {
            
            IQueryable<CUSTO_FIXO> query = Db.CUSTO_FIXO.Where(p => p.CUFX_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CUFX_NM_NOME);
            return await query.ToListAsync();
        }
    }
}
 