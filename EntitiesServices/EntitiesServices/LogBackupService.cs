using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class LogBackupService : ServiceBase<LOG_BACKUP>, ILogBackupService
    {
        private readonly ILogBackupRepository _logRepository;
        protected DbBackupEntities bkp = new DbBackupEntities();

        public LogBackupService(ILogBackupRepository logRepository) : base(logRepository)
        {
            _logRepository = logRepository;
        }

        public LOG_BACKUP GetById(Int32 id)
        {
            LOG_BACKUP item = _logRepository.GetById(id);
            return item;
        }

        public List<LOG_BACKUP> GetAllItens(Int32 idAss)
        {
            return _logRepository.GetAllItens(idAss);
        }

        public List<LOG_BACKUP> GetAllItensUsuario(Int32 id, Int32 idAss)
        {
            return _logRepository.GetAllItensUsuario(id, idAss);
        }

        public List<LOG_BACKUP> ExecuteFilter(Int32? usuId, DateTime? data, String operacao, Int32 idAss)
        {
            List<LOG_BACKUP> lista = _logRepository.ExecuteFilter(usuId, data, operacao, idAss);
            return lista;
        }

        public Int32 Delete(LOG_BACKUP item)
        {
            using (DbContextTransaction transaction = bkp.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(LOG_BACKUP item)
        {
            using (DbContextTransaction transaction = bkp.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
