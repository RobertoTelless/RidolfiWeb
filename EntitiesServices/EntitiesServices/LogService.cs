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
    public class LogService : ServiceBase<LOG>, ILogService
    {
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public LogService(ILogRepository logRepository) : base(logRepository)
        {
            _logRepository = logRepository;
        }

        public LOG GetById(Int32 id)
        {
            LOG item = _logRepository.GetById(id);
            return item;
        }

        public List<LOG> GetAllItens(Int32 idAss)
        {
            return _logRepository.GetAllItens(idAss);
        }

        public List<LOG> GetAllItensDataCorrente(Int32 idAss)
        {
            return _logRepository.GetAllItensDataCorrente(idAss);
        }

        public List<LOG> GetAllItensMesCorrente(Int32 idAss)
        {
            return _logRepository.GetAllItensMesCorrente(idAss);
        }

        public List<LOG> GetAllItensMesAnterior(Int32 idAss)
        {
            return _logRepository.GetAllItensMesAnterior(idAss);
        }

        public List<LOG> GetLogByFaixa(DateTime inicio, DateTime final, Int32 idAss)
        {
            return _logRepository.GetLogByFaixa(inicio, final, idAss);
        }

        public List<LOG> GetAllItensUsuario(Int32 id, Int32 idAss)
        {
            return _logRepository.GetAllItensUsuario(id, idAss);
        }

        public List<LOG> ExecuteFilter(Int32? usuId, DateTime? data, String operacao, Int32 idAss)
        {
            List<LOG> lista = _logRepository.ExecuteFilter(usuId, data, operacao, idAss);
            return lista;
        }

        public Int32 Create(LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
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

        public Int32 Delete(LOG item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
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

    }
}
