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
    public class VaraService : ServiceBase<VARA>, IVaraService
    {
        private readonly IVaraRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITRFRepository _trfRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public VaraService(IVaraRepository baseRepository, ILogRepository logRepository, ITRFRepository trfRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _trfRepository = trfRepository;

        }

        public VARA CheckExist(VARA conta)
        {
            VARA item = _baseRepository.CheckExist(conta);
            return item;
        }

        public List<TRF> GetAllTRF()
        {
            return _trfRepository.GetAllItens();
        }

        public VARA GetItemById(Int32 id)
        {
            VARA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<VARA> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<VARA> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<VARA> ExecuteFilter(String nome, Int32? trf)
        {
            List<VARA> lista = _baseRepository.ExecuteFilter(nome, trf);
            return lista;
        }

        public Int32 Create(VARA item, LOG log)
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

        public Int32 Create(VARA item)
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


        public Int32 Edit(VARA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.TRF = null;
                    VARA obj = _baseRepository.GetById(item.VARA_CD_ID);
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

        public Int32 Edit(VARA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VARA obj = _baseRepository.GetById(item.VARA_CD_ID);
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

        public Int32 Delete(VARA item, LOG log)
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
