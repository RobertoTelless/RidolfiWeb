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
    public class MensagemService : ServiceBase<MENSAGENS>, IMensagemService
    {
        private readonly IMensagemRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITemplateRepository _tempRepository;
        private readonly ICategoriaClienteRepository _tipoRepository;
        private readonly IUFRepository _ufRepository;
        private readonly IMensagemAnexoRepository _anexoRepository;
        private readonly IMensagemDestinoRepository _destRepository;
        private readonly ITemplateSMSRepository _tsmsRepository;
        private readonly IResultadoRobotRepository _robRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public MensagemService(IMensagemRepository baseRepository, ILogRepository logRepository, ITemplateRepository tempRepository, ICategoriaClienteRepository tipoRepository, IUFRepository ufRepository, IMensagemAnexoRepository anexoRepository, IMensagemDestinoRepository destRepository, ITemplateSMSRepository tsmsRepository, IResultadoRobotRepository robRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tempRepository = tempRepository;
            _tipoRepository = tipoRepository;
            _ufRepository = ufRepository;
            _anexoRepository = anexoRepository;
            _tsmsRepository = tsmsRepository;
            _destRepository = destRepository;
            _robRepository = robRepository;
        }

        public MENSAGENS CheckExist(MENSAGENS conta, Int32 idAss)
        {
            MENSAGENS item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public List<UF> GetAllUF()
        {
            return _ufRepository.GetAllItens();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _ufRepository.GetItemBySigla(sigla);
        }

        public MENSAGENS_DESTINOS GetDestinoById(Int32 id)
        {
            return _destRepository.GetItemById(id);
        }

        public MENSAGENS GetItemById(Int32 id)
        {
            MENSAGENS item = _baseRepository.GetItemById(id);
            return item;
        }

        public MENSAGEM_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<MENSAGENS> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<RESULTADO_ROBOT> GetAllEnviosRobot(Int32 idAss)
        {
            return _robRepository.GetAllItens(idAss);
        }

        public List<MENSAGENS> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<TEMPLATE> GetAllTemplates(Int32 idAss)
        {
            return _tempRepository.GetAllItens(idAss);
        }

        public List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss)
        {
            return _tsmsRepository.GetAllItens(idAss);
        }

        public List<MENSAGENS> ExecuteFilterSMS(DateTime? envio, Int32 cliente, String texto, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterSMS(envio, cliente, texto, idAss);
        }

        public List<MENSAGENS> ExecuteFilterEMail(DateTime? envio, Int32 cliente, String texto, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterEMail(envio, cliente, texto, idAss);
        }

        public List<RESULTADO_ROBOT> ExecuteFilterRobot(Int32? tipo, DateTime? inicio, DateTime? final, String cliente, String email, String celular, Int32? status, Int32 idAss)
        {
            return _robRepository.ExecuteFilter(tipo, inicio, final, cliente, email, celular, status, idAss);

        }

        public Int32 Create(MENSAGENS item, LOG log)
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

        public Int32 Create(MENSAGENS item)
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


        public Int32 Edit(MENSAGENS item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGENS obj = _baseRepository.GetById(item.MENS_CD_ID);
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

        public Int32 Edit(MENSAGENS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGENS obj = _baseRepository.GetById(item.MENS_CD_ID);
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

        public Int32 Delete(MENSAGENS item, LOG log)
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

        public Int32 EditDestino(MENSAGENS_DESTINOS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MENSAGENS_DESTINOS obj = _destRepository.GetById(item.MEDE_CD_ID);
                    _destRepository.Detach(obj);
                    _destRepository.Update(item);
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

        public Int32 CreateDestino(MENSAGENS_DESTINOS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _destRepository.Add(item);
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
