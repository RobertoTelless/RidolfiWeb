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
    public class CRMDiarioService : ServiceBase<DIARIO_PROCESSO>, ICRMDiarioService
    {
        private readonly ICRMDiarioRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IUsuarioRepository _usuRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public CRMDiarioService(ICRMDiarioRepository baseRepository, ILogRepository logRepository, IUsuarioRepository usuRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _usuRepository = usuRepository;
        }

        public DIARIO_PROCESSO GetItemById(Int32 id)
        {
            DIARIO_PROCESSO item = _baseRepository.GetItemById(id);
            return item;
        }

        public DIARIO_PROCESSO GetByDate(DateTime data)
        {
            DIARIO_PROCESSO item = _baseRepository.GetByDate(data);
            return item;
        }

        public List<DIARIO_PROCESSO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public List<DIARIO_PROCESSO> ExecuteFilter(Int32? processo, DateTime? inicio, DateTime? final, Int32? usuario, String operacao, String descricao, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(processo, inicio, final, usuario, operacao, descricao, idAss);
        }

        public Int32 Create(DIARIO_PROCESSO item, LOG log)
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

        public Int32 Create(DIARIO_PROCESSO item)
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


        public Int32 Edit(DIARIO_PROCESSO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    DIARIO_PROCESSO obj = _baseRepository.GetById(item.DIPR_CD_ID);
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

        public Int32 Edit(DIARIO_PROCESSO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    DIARIO_PROCESSO obj = _baseRepository.GetById(item.DIPR_CD_ID);
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

        public Int32 Delete(DIARIO_PROCESSO item, LOG log)
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
