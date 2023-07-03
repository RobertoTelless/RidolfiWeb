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
    public class TipoAcaoService : ServiceBase<TIPO_ACAO>, ITipoAcaoService
    {
        private readonly ITipoAcaoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public TipoAcaoService(ITipoAcaoRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;

        }

        public TIPO_ACAO GetItemById(Int32 id)
        {
            TIPO_ACAO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<TIPO_ACAO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<TIPO_ACAO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public TIPO_ACAO CheckExist(TIPO_ACAO conta, Int32 idAss)
        {
            TIPO_ACAO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public Int32 Create(TIPO_ACAO item, LOG log)
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

        public Int32 Create(TIPO_ACAO item)
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


        public Int32 Edit(TIPO_ACAO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TIPO_ACAO obj = _baseRepository.GetById(item.TIAC_CD_ID);
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

        public Int32 Edit(TIPO_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TIPO_ACAO obj = _baseRepository.GetById(item.TIAC_CD_ID);
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

        public Int32 Delete(TIPO_ACAO item, LOG log)
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
