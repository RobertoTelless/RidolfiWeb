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
    public class MensagemEnviadaSistemaService : ServiceBase<MENSAGENS_ENVIADAS_SISTEMA>, IMensagemEnviadaSistemaService
    {
        private readonly IMensagemEnviadaSistemaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IUsuarioRepository _usuRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public MensagemEnviadaSistemaService(IMensagemEnviadaSistemaRepository baseRepository, ILogRepository logRepository, IUsuarioRepository usuRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _usuRepository = usuRepository;
        }

        public MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id)
        {
            MENSAGENS_ENVIADAS_SISTEMA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss)
        {
            List<MENSAGENS_ENVIADAS_SISTEMA> item = _baseRepository.GetByDate(data, idAss);
            return item;
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> ExecuteFilter(Int32? escopo, Int32? tipo, DateTime? inicio, DateTime? final, Int32? usuario, String titulo, String origem, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(escopo, tipo, inicio, final, usuario, titulo, origem, idAss);
        }

        public Int32 Create(MENSAGENS_ENVIADAS_SISTEMA item, LOG log)
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

        public Int32 Create(MENSAGENS_ENVIADAS_SISTEMA item)
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


        public Int32 Edit(MENSAGENS_ENVIADAS_SISTEMA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGENS_ENVIADAS_SISTEMA obj = _baseRepository.GetById(item.MEEN_CD_ID);
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

        public Int32 Edit(MENSAGENS_ENVIADAS_SISTEMA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGENS_ENVIADAS_SISTEMA obj = _baseRepository.GetById(item.MEEN_CD_ID);
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

        public Int32 Delete(MENSAGENS_ENVIADAS_SISTEMA item, LOG log)
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
