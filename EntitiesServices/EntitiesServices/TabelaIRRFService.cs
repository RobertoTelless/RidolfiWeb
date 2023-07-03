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
    public class TabelaIRRFService : ServiceBase<TABELA_IRRF>, ITabelaIRRFService
    {
        private readonly ITabelaIRRFRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public TabelaIRRFService(ITabelaIRRFRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
        }

        public TABELA_IRRF CheckExist(TABELA_IRRF conta)
        {
            TABELA_IRRF item = _baseRepository.CheckExist(conta);
            return item;
        }

        public TABELA_IRRF GetItemById(Int32 id)
        {
            TABELA_IRRF item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<TABELA_IRRF> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<TABELA_IRRF> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public Int32 Create(TABELA_IRRF item, LOG log)
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

        public Int32 Create(TABELA_IRRF item)
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


        public Int32 Edit(TABELA_IRRF item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TABELA_IRRF obj = _baseRepository.GetById(item.IRRF_CD_ID);
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

        public Int32 Edit(TABELA_IRRF item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TABELA_IRRF obj = _baseRepository.GetById(item.IRRF_CD_ID);
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

        public Int32 Delete(TABELA_IRRF item, LOG log)
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
