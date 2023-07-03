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
    public class MensagemAutomacaoService : ServiceBase<MENSAGEM_AUTOMACAO>, IMensagemAutomacaoService
    {
        private readonly IMensagemAutomacaoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IGrupoRepository _grupRepository;
        private readonly IMensagemAutomacaoDatasRepository _dataRepository;
        private readonly ITemplateSMSRepository _tsmsRepository;
        private readonly ITemplateEMailRepository _temaRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public MensagemAutomacaoService(IMensagemAutomacaoRepository baseRepository, ILogRepository logRepository, IGrupoRepository grupRepository, IMensagemAutomacaoDatasRepository dataRepository, ITemplateSMSRepository tsmsRepository, ITemplateEMailRepository temaRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _grupRepository = grupRepository;
            _dataRepository = dataRepository;
            _tsmsRepository = tsmsRepository;
            _temaRepository = temaRepository;
        }

        public MENSAGEM_AUTOMACAO CheckExist(MENSAGEM_AUTOMACAO conta, Int32 idAss)
        {
            MENSAGEM_AUTOMACAO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public MENSAGEM_AUTOMACAO GetItemById(Int32 id)
        {
            MENSAGEM_AUTOMACAO item = _baseRepository.GetItemById(id);
            return item;
        }

        public MENSAGEM_AUTOMACAO_DATAS GetDatasById(Int32 id)
        {
            return _dataRepository.GetItemById(id);
        }

        public List<MENSAGEM_AUTOMACAO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<MENSAGEM_AUTOMACAO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<GRUPO> GetAllGrupos(Int32 idAss)
        {
            return _grupRepository.GetAllItens(idAss);
        }

        public List<TEMPLATE_EMAIL> GetAllTemplatesEMail(Int32 idAss)
        {
            return _temaRepository.GetAllItens(idAss);
        }

        public List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss)
        {
            return _tsmsRepository.GetAllItens(idAss);
        }

        public List<MENSAGEM_AUTOMACAO> ExecuteFilter(Int32? tipo, Int32? grupo, String nome, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(tipo, grupo, nome, idAss);
        }

        public Int32 Create(MENSAGEM_AUTOMACAO item, LOG log)
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

        public Int32 Create(MENSAGEM_AUTOMACAO item)
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


        public Int32 Edit(MENSAGEM_AUTOMACAO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGEM_AUTOMACAO obj = _baseRepository.GetById(item.MEAU_CD_ID);
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

        public Int32 Edit(MENSAGEM_AUTOMACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGEM_AUTOMACAO obj = _baseRepository.GetById(item.MEAU_CD_ID);
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

        public Int32 Delete(MENSAGEM_AUTOMACAO item, LOG log)
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

        public Int32 EditDatas(MENSAGEM_AUTOMACAO_DATAS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGEM_AUTOMACAO_DATAS obj = _dataRepository.GetById(item.MEAD_CD_ID);
                    _dataRepository.Detach(obj);
                    _dataRepository.Update(item);
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

        public Int32 CreateDatas(MENSAGEM_AUTOMACAO_DATAS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _dataRepository.Add(item);
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
