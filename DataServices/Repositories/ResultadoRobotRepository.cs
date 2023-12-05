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
    public class ResultadoRobotRepository : RepositoryBase<RESULTADO_ROBOT_CUSTO>, IResultadoRobotRepository
    {
        public List<RESULTADO_ROBOT_CUSTO> GetAllItens(Int32 idAss)
        {
            IQueryable<RESULTADO_ROBOT_CUSTO> query = Db.RESULTADO_ROBOT_CUSTO.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public RESULTADO_ROBOT_CUSTO GetItemById(Int32 id)
        {
            IQueryable<RESULTADO_ROBOT_CUSTO> query = Db.RESULTADO_ROBOT_CUSTO.Where(p => p.RERC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<RESULTADO_ROBOT_CUSTO> ExecuteFilter(Int32? tipo, DateTime? dataInicio, DateTime? dataFim, Int32? usuario, Int32 idAss)
        {
            List<RESULTADO_ROBOT_CUSTO> lista = new List<RESULTADO_ROBOT_CUSTO>();
            IQueryable<RESULTADO_ROBOT_CUSTO> query = Db.RESULTADO_ROBOT_CUSTO;
            if (dataInicio != null & dataFim == null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RERC_DT_ENVIO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if (dataInicio == null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RERC_DT_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (dataInicio != null & dataFim != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RERC_DT_ENVIO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.RERC_DT_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (usuario > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (tipo > 0)
            {
                query = query.Where(p => p.RERC_IN_TIPO == tipo);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.RERC_DT_ENVIO);
                lista = query.ToList<RESULTADO_ROBOT_CUSTO>();
            }
            return lista;
        }

    }
}
 