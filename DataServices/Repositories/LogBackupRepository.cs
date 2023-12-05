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
    public class LogBackupRepository : RepositoryBase<LOG_BACKUP>, ILogBackupRepository
    {
        public LOG_BACKUP GetById(Int32 id)
        {
            IQueryable<LOG_BACKUP> query = bkp.LOG_BACKUP.Where(p => p.LOG_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<LOG_BACKUP> GetAllItens(Int32 idAss)
        {
            IQueryable<LOG_BACKUP> query = bkp.LOG_BACKUP.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => p.LOG_IN_SISTEMA == 2);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            return query.ToList();
        }

        public List<LOG_BACKUP> GetAllItensUsuario(Int32 id, Int32 idAss)
        {
            IQueryable<LOG_BACKUP> query = bkp.LOG_BACKUP.Where(p => p.LOG_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Where(p => p.LOG_IN_SISTEMA == 2);
            query = query.OrderByDescending(a => a.LOG_DT_DATA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<LOG_BACKUP> ExecuteFilter(Int32? usuId, DateTime? data, String operacao, Int32 idAss)
        {
            List<LOG_BACKUP> lista = new List<LOG_BACKUP>();
            IQueryable<LOG_BACKUP> query = bkp.LOG_BACKUP;
            if (!String.IsNullOrEmpty(operacao))
            {
                query = query.Where(p => p.LOG_NM_OPERACAO == operacao);
            }
            if (usuId != 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuId);
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LOG_DT_DATA) == DbFunctions.TruncateTime(data));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.LOG_IN_SISTEMA == 2);
                query = query.OrderByDescending(a => a.LOG_DT_DATA);
                lista = query.ToList<LOG_BACKUP>();
            }
            return lista;
        }
    }
}
 