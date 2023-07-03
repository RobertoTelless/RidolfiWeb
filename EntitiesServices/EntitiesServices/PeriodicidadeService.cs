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
    public class PeriodicidadeService : ServiceBase<PERIODICIDADE_TAREFA>, IPeriodicidadeService
    {
        private readonly IPeriodicidadeRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public PeriodicidadeService(IPeriodicidadeRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;

        }

        public PERIODICIDADE_TAREFA GetItemById(Int32 id)
        {
            PERIODICIDADE_TAREFA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<PERIODICIDADE_TAREFA> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<PERIODICIDADE_TAREFA> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public PERIODICIDADE_TAREFA CheckExist(PERIODICIDADE_TAREFA conta)
        {
            PERIODICIDADE_TAREFA item = _baseRepository.CheckExist(conta);
            return item;
        }

        public Int32 Create(PERIODICIDADE_TAREFA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
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

        public Int32 Create(PERIODICIDADE_TAREFA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
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


        public Int32 Edit(PERIODICIDADE_TAREFA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PERIODICIDADE_TAREFA obj = _baseRepository.GetById(item.PETA_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
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

        public Int32 Edit(PERIODICIDADE_TAREFA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PERIODICIDADE_TAREFA obj = _baseRepository.GetById(item.PETA_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
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

        public Int32 Delete(PERIODICIDADE_TAREFA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
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
