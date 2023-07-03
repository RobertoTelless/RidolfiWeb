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
    public class ResultadoRobotRepository : RepositoryBase<RESULTADO_ROBOT>, IResultadoRobotRepository
    {
        public List<RESULTADO_ROBOT> GetAllItens(Int32 idAss)
        {
            IQueryable<RESULTADO_ROBOT> query = Db.RESULTADO_ROBOT.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public RESULTADO_ROBOT GetItemById(Int32 id)
        {
            IQueryable<RESULTADO_ROBOT> query = Db.RESULTADO_ROBOT.Where(p => p.RERO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<RESULTADO_ROBOT> ExecuteFilter(Int32? tipo, DateTime? dataInicio, DateTime? dataFim, String cliente, String email, String celular, Int32? status, Int32 idAss)
        {
            List<RESULTADO_ROBOT> lista = new List<RESULTADO_ROBOT>();
            IQueryable<RESULTADO_ROBOT> query = Db.RESULTADO_ROBOT;
            if (tipo > 0)
            {
                query = query.Where(p => p.RERO_IN_TIPO == tipo);
            }

            if (status > 0)
            {
                query = query.Where(p => p.RERO_IN_STATUS == status);
            }
            if (!String.IsNullOrEmpty(cliente))
            {
                query = query.Where(p => p.CLIENTE.CLIE_NM_NOME.Contains(cliente) || p.CLIENTE.CLIE_NM_RAZAO.Contains(cliente) || p.CLIENTE.CLIE_NR_CPF.Contains(cliente));
            }
            if (email != null)
            {
                query = query.Where(p => p.RERO_NM_EMAIL.Contains(email));
            }
            if (celular != null)
            {
                query = query.Where(p => p.RERO_NR_CELULAR.Contains(celular));
            }

            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RERO_DT_ENVIO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RERO_DT_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.RERO_DT_ENVIO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.RERO_DT_ENVIO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.RERO_IN_ATIVO == 1);
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.RERO_DT_ENVIO);
                lista = query.ToList<RESULTADO_ROBOT>();
            }
            return lista;
        }
    }
}
 