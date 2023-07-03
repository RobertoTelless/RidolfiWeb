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
    public class ContatoService : ServiceBase<CONTATO>, IContatoService
    {
        private readonly IContatoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IBeneficiarioRepository _beneRepository;
        private readonly IPrecatorioRepository _preRepository;
        private readonly IQualificacaoRepository _quaRepository;
        private readonly IQuemDesligouRepository _quemRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public ContatoService(IContatoRepository baseRepository, ILogRepository logRepository, IBeneficiarioRepository beneRepository, IPrecatorioRepository preRepository, IQualificacaoRepository quaRepository, IQuemDesligouRepository quemRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _beneRepository = beneRepository;
            _preRepository = preRepository;
            _quemRepository = quemRepository;
            _quaRepository = quaRepository;
        }

        public CONTATO CheckExistProt(String prot)
        {
            CONTATO item = _baseRepository.CheckExistProt(prot);
            return item;
        }

        public QUALIFICACAO CheckExistQualificacao(String quali)
        {
            QUALIFICACAO item = _quaRepository.CheckExist(quali);
            return item;
        }

        public QUEM_DESLIGOU CheckExistDesligamento(String quali)
        {
            QUEM_DESLIGOU item = _quemRepository.CheckExist(quali);
            return item;
        }

        public CONTATO GetItemById(Int32 id)
        {
            CONTATO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CONTATO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<CONTATO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<BENEFICIARIO> GetAllBeneficiarios()
        {
            return _beneRepository.GetAllItens();
        }

        public List<PRECATORIO> GetAllPrecatorios()
        {
            return _preRepository.GetAllItens();
        }

        public List<QUALIFICACAO> GetAllQualificacao()
        {
            return _quaRepository.GetAllItens();
        }

        public List<QUEM_DESLIGOU> GetAllDesligamento()
        {
            return _quemRepository.GetAllItens();
        }

        public List<CONTATO> ExecuteFilter(Int32? precatorio, Int32? beneficiario, Int32? quali, Int32? quem, String protocolo, DateTime? inicio, DateTime? final, String telefone, String agente, String campanha)
        {
            return _baseRepository.ExecuteFilter(precatorio, beneficiario, quali, quem, protocolo, inicio, final, telefone, agente, campanha);
        }

        public Int32 Create(CONTATO item, LOG log)
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

        public Int32 Create(CONTATO item)
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


        public Int32 Edit(CONTATO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTATO obj = _baseRepository.GetById(item.CONT_CD_ID);
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

        public Int32 Edit(CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTATO obj = _baseRepository.GetById(item.CONT_CD_ID);
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

        public Int32 Delete(CONTATO item, LOG log)
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

        public Int32 CreateQualificacao(QUALIFICACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _quaRepository.Add(item);
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

        public Int32 CreateDesligamento(QUEM_DESLIGOU item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _quemRepository.Add(item);
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
