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
    public class PlanoService : ServiceBase<PLANO>, IPlanoService
    {
        private readonly IPlanoRepository _baseRepository;
        private readonly IPeriodicidadePlanoRepository _perRepository;
        private readonly ILogRepository _logRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public PlanoService(IPlanoRepository baseRepository, IPeriodicidadePlanoRepository perRepository, ILogRepository logRepository ): base(baseRepository)
        {
            _baseRepository = baseRepository;
            _perRepository = perRepository;
            _logRepository = logRepository;
        }

        public PLANO CheckExist(PLANO conta)
        {
            PLANO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public PLANO GetItemById(Int32 id)
        {
            PLANO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<PLANO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<PLANO_PERIODICIDADE> GetAllPeriodicidades()
        {
            return _perRepository.GetAllItens();
        }

        public PLANO_PERIODICIDADE GetPeriodicidadeById(Int32 id)
        {
            return _perRepository.GetItemById(id);
        }

        public List<PLANO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<PLANO> GetAllValidos()
        {
            return _baseRepository.GetAllValidos();
        }

        public List<PLANO> ExecuteFilter(String nome, String descricao)
        {
            return _baseRepository.ExecuteFilter(nome, descricao);

        }


        public Int32 Create(PLANO item, LOG log)
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

        public Int32 Create(PLANO item)
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


        public Int32 Edit(PLANO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PLANO_PERIODICIDADE = null;
                    PLANO obj = _baseRepository.GetById(item.PLAN_CD_ID);
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

        public Int32 Edit(PLANO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PLANO obj = _baseRepository.GetById(item.PLAN_CD_ID);
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

        public Int32 Delete(PLANO item, LOG log)
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
