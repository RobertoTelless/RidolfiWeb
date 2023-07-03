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
    public class PrecatorioService : ServiceBase<PRECATORIO>, IPrecatorioService
    {
        private readonly IPrecatorioRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IBeneficiarioRepository _beneRepository;
        private readonly IPrecatorioAnotacaoRepository _comRepository;
        private readonly ITRFRepository _trfRepository;
        private readonly IHonorarioRepository _honoRepository;
        private readonly INaturezaRepository _natuRepository;
        private readonly IPrecatorioEstadoRepository _presRepository;
        private readonly IPrecatorioAnexoRepository _anexoRepository;
        private readonly IBancoRepository _banRepository;
        private readonly IPrecatorioFalhaRepository _falRepository;
        private readonly IContatoRepository _conRepository;
        private readonly IContatoFalhaRepository _cfalRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public PrecatorioService(IPrecatorioRepository baseRepository, ILogRepository logRepository, IBeneficiarioRepository beneRepository, IPrecatorioAnotacaoRepository comRepository, ITRFRepository trfRepository, IHonorarioRepository honoRepository, INaturezaRepository natuRepository, IPrecatorioEstadoRepository presRepository, IPrecatorioAnexoRepository anexoRepository, IBancoRepository banRepository, IPrecatorioFalhaRepository falRepository, IContatoRepository conRepository, IContatoFalhaRepository cfalRepository ) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _beneRepository = beneRepository;
            _anexoRepository = anexoRepository;
            _comRepository = comRepository;
            _trfRepository = trfRepository;
            _honoRepository = honoRepository;
            _natuRepository = natuRepository;
            _presRepository = presRepository; 
            _banRepository = banRepository;
            _falRepository = falRepository;
            _conRepository = conRepository; 
            _cfalRepository = cfalRepository;
        }

        public PRECATORIO CheckExist(PRECATORIO conta)
        {
            PRECATORIO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public PRECATORIO CheckExist(String conta)
        {
            PRECATORIO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public CONTATO CheckExistContato(String prot)
        {
            CONTATO item = _conRepository.CheckExistProt(prot);
            return item;
        }

        public NATUREZA CheckExistNatureza(String conta)
        {
            NATUREZA item = _natuRepository.CheckExist(conta);
            return item;
        }

        public PRECATORIO GetItemById(Int32 id)
        {
            PRECATORIO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<PRECATORIO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<PRECATORIO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<BENEFICIARIO> GetAllBeneficiarios()
        {
            return _beneRepository.GetAllItens();
        }

        public List<TRF> GetAllTRF()
        {
            return _trfRepository.GetAllItens();
        }

        public List<HONORARIO> GetAllAdvogados()
        {
            return _honoRepository.GetAllItens();
        }

        public List<NATUREZA> GetAllNaturezas()
        {
            return _natuRepository.GetAllItens();
        }

        public List<PRECATORIO_ESTADO> GetAllEstados()
        {
            return _presRepository.GetAllItens();
        }

        public List<BANCO> GetAllBancos()
        {
            return _banRepository.GetAllItens(1);
        }

        public PRECATORIO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public PRECATORIO_ANOTACAO GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public List<PRECATORIO> ExecuteFilter(Int32? trf, Int32? beneficiario, Int32? advogado, Int32? natureza, Int32? estado, String nome, String ano, Int32? crm, Int32? pesquisa, Decimal? valor1, Decimal? valor2, Int32? situacao)
        {
            return _baseRepository.ExecuteFilter(trf, beneficiario, advogado, natureza, estado, nome, ano,crm, pesquisa, valor1, valor2, situacao);
        }

        public Int32 Create(PRECATORIO item, LOG log)
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

        public Int32 Create(PRECATORIO item)
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


        public Int32 Edit(PRECATORIO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRECATORIO obj = _baseRepository.GetById(item.PREC_CD_ID);
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

        public Int32 Edit(PRECATORIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRECATORIO obj = _baseRepository.GetById(item.PREC_CD_ID);
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

        public Int32 Delete(PRECATORIO item, LOG log)
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

        public Int32 CreateFalha(PRECATORIO_FALHA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _falRepository.Add(item);
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

        public Int32 CreateFalhaContato(CONTATO_FALHA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _cfalRepository.Add(item);
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
