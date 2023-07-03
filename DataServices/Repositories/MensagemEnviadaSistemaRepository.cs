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
    public class MensagemEnviadaSistemaRepository : RepositoryBase<MENSAGENS_ENVIADAS_SISTEMA>, IMensagemEnviadaSistemaRepository
    {
        public List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss)
        {
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) == DbFunctions.TruncateTime(DateTime.Today.Date));
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id)
        {
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            query = query.Where(p => p.MEEN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss)
        {
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.MEEN_DT_DATA_ENVIO);
            return query.ToList();
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> ExecuteFilter(Int32? escopo, Int32? tipo, DateTime? dataInicio, DateTime? dataFim, Int32? usuario, String titulo, String origem, Int32 idAss)
        {
            List<MENSAGENS_ENVIADAS_SISTEMA> lista = new List<MENSAGENS_ENVIADAS_SISTEMA>();
            IQueryable<MENSAGENS_ENVIADAS_SISTEMA> query = Db.MENSAGENS_ENVIADAS_SISTEMA;
            if (dataInicio != null & dataFim == null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if (dataInicio == null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (dataInicio != null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.MEEN_DT_DATA_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (usuario > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (tipo > 0)
            {
                query = query.Where(p => p.MEEN_IN_TIPO == tipo);
            }
            if (escopo > 0)
            {
                query = query.Where(p => p.MEEN_IN_ESCOPO == escopo);
            }
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.MEEN_NM_TITULO.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(origem))
            {
                query = query.Where(p => p.MEEN_NM_ORIGEM.Contains(origem));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.MEEN_DT_DATA_ENVIO);
                lista = query.ToList<MENSAGENS_ENVIADAS_SISTEMA>();
            }
            return lista;
        }

    }
}
 