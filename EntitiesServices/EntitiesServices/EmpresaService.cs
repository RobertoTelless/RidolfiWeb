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
    public class EmpresaService : ServiceBase<EMPRESA>, IEmpresaService
    {
        private readonly IEmpresaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IMaquinaRepository _maqRepository;
        private readonly IRegimeTributarioRepository _regRepository;
        private readonly IEmpresaAnexoRepository _anexoRepository;
        private readonly IEmpresaMaquinaRepository _emaqRepository;
        private readonly IUFRepository _ufRepository;
        private readonly IEmpresaPlataformaRepository _eplaRepository;
        private readonly IEmpresaCustoVariavelRepository _cvRepository;
        private readonly IEmpresaTicketRepository _tkRepository;
        private readonly ICustoVariavelCalculoRepository _calRepository;
        private readonly ICustoVariavelHistoricoRepository _hisRepository;
        private readonly ICustoHistoricoRepository _cuhRepository;
        private readonly IComissaoRepository _comRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public EmpresaService(IEmpresaRepository baseRepository, ILogRepository logRepository, IMaquinaRepository maqRepository, IRegimeTributarioRepository regRepository, IEmpresaAnexoRepository anexoRepository, IEmpresaMaquinaRepository emaqRepository, IUFRepository ufRepository, IEmpresaPlataformaRepository eplaRepository, IEmpresaCustoVariavelRepository cvRepository, IEmpresaTicketRepository tkRepository, ICustoVariavelCalculoRepository calRepository, ICustoVariavelHistoricoRepository hisRepository, ICustoHistoricoRepository cuhRepository, IComissaoRepository comRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _maqRepository = maqRepository;
            _regRepository = regRepository;
            _anexoRepository = anexoRepository;
            _emaqRepository = emaqRepository;
            _ufRepository = ufRepository;
            _eplaRepository = eplaRepository;
            _cvRepository = cvRepository;
            _tkRepository = tkRepository;
            _calRepository = calRepository;
            _hisRepository = hisRepository;
            _cuhRepository = cuhRepository;
            _comRepository = comRepository;
        }

        public EMPRESA GetItemById(Int32 id)
        {
            EMPRESA item = _baseRepository.GetItemById(id);
            return item;
        }

        public EMPRESA GetItemByAssinante(Int32 id)
        {
            EMPRESA item = _baseRepository.GetItemByAssinante(id);
            return item;
        }

        public List<EMPRESA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<EMPRESA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public EMPRESA CheckExist(EMPRESA conta, Int32 idAss)
        {
            EMPRESA item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public EMPRESA_MAQUINA CheckExistMaquina(EMPRESA_MAQUINA conta, Int32 idAss)
        {
            EMPRESA_MAQUINA item = _emaqRepository.CheckExist(conta, idAss);
            return item;
        }

        public EMPRESA_PLATAFORMA CheckExistPlataforma(EMPRESA_PLATAFORMA conta, Int32 idAss)
        {
            EMPRESA_PLATAFORMA item = _eplaRepository.CheckExist(conta, idAss);
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

        public EMPRESA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<EMPRESA> ExecuteFilter(String nome, Int32? idAss)
        {
            List<EMPRESA> lista = _baseRepository.ExecuteFilter(nome, idAss);
            return lista;
        }

        public List<MAQUINA> GetAllMaquinas(Int32 idAss)
        {
            return _maqRepository.GetAllItens(idAss);
        }

        public List<REGIME_TRIBUTARIO> GetAllRegimes()
        {
            return _regRepository.GetAllItens();
        }

        public Int32 Create(EMPRESA item, LOG log)
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

        public Int32 Create(EMPRESA item)
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


        public Int32 Edit(EMPRESA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA obj = _baseRepository.GetById(item.EMPR_CD_ID);
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

        public Int32 Edit(EMPRESA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA obj = _baseRepository.GetById(item.EMPR_CD_ID);
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

        public Int32 Delete(EMPRESA item, LOG log)
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

        public EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina)
        {
            return _emaqRepository.GetByEmpresaMaquina(empresa, maquina);
        }

        public EMPRESA_PLATAFORMA GetByEmpresaPlataforma(Int32 empresa, Int32 plataforma)
        {
            return _eplaRepository.GetByEmpresaPlataforma(empresa, plataforma);
        }

        public EMPRESA_MAQUINA GetMaquinaById(Int32 id)
        {
            return _emaqRepository.GetItemById(id);
        }

        public EMPRESA_PLATAFORMA GetPlataformaById(Int32 id)
        {
            return _eplaRepository.GetItemById(id);
        }

        public REGIME_TRIBUTARIO GetRegimeById(Int32 id)
        {
            return _regRepository.GetItemById(id);
        }

        public Int32 EditMaquina(EMPRESA_MAQUINA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA_MAQUINA obj = _emaqRepository.GetById(item.EMMA_CD_ID);
                    _emaqRepository.Detach(obj);
                    _emaqRepository.Update(item);
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

        public Int32 CreateMaquina(EMPRESA_MAQUINA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _emaqRepository.Add(item);
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

        public Int32 EditPlataforma(EMPRESA_PLATAFORMA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA_PLATAFORMA obj = _eplaRepository.GetById(item.EMPL_CD_ID);
                    _eplaRepository.Detach(obj);
                    _eplaRepository.Update(item);
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

        public Int32 CreatePlataforma(EMPRESA_PLATAFORMA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _eplaRepository.Add(item);
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

        public EMPRESA_CUSTO_VARIAVEL CheckExistCustoVariavel(EMPRESA_CUSTO_VARIAVEL conta, Int32 idAss)
        {
            EMPRESA_CUSTO_VARIAVEL item = _cvRepository.CheckExistCustoVariavel(conta, idAss);
            return item;
        }

        public EMPRESA_CUSTO_VARIAVEL GetCustoVariavelById(Int32 id)
        {
            return _cvRepository.GetItemById(id);
        }

        public Int32 EditCustoVariavel(EMPRESA_CUSTO_VARIAVEL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA_CUSTO_VARIAVEL obj = _cvRepository.GetById(item.EMCV_CD_ID);
                    _cvRepository.Detach(obj);
                    _cvRepository.Update(item);
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

        public Int32 CreateCustoVariavel(EMPRESA_CUSTO_VARIAVEL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _cvRepository.Add(item);
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

        public EMPRESA_TICKET CheckExistTicket(EMPRESA_TICKET conta, Int32 idAss)
        {
            EMPRESA_TICKET item = _tkRepository.CheckExist(conta, idAss);
            return item;
        }

        public EMPRESA_TICKET GetTicketById(Int32 id)
        {
            return _tkRepository.GetItemById(id);
        }

        public Int32 EditTicket(EMPRESA_TICKET item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA_TICKET obj = _tkRepository.GetById(item.EMTI_CD_ID);
                    _tkRepository.Detach(obj);
                    _tkRepository.Update(item);
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

        public Int32 CreateTicket(EMPRESA_TICKET item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _tkRepository.Add(item);
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

        public EMPRESA_TICKET GetByEmpresaTicket(Int32 empresa, Int32 ticket)
        {
            return _tkRepository.GetByEmpresaTicket(empresa, ticket);
        }

        public CUSTO_VARIAVEL_CALCULO GetCustoVariavelCalculoById(Int32 id)
        {
            return _calRepository.GetItemById(id);
        }

        public Int32 EditCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CUSTO_VARIAVEL_CALCULO obj = _calRepository.GetById(item.CVCA_CD_ID);
                    _calRepository.Detach(obj);
                    _calRepository.Update(item);
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

        public Int32 CreateCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _calRepository.Add(item);
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

        public List<CUSTO_VARIAVEL_CALCULO> ExecuteFilterCalculo(DateTime? data, Int32? idAss)
        {
            List<CUSTO_VARIAVEL_CALCULO> lista = _calRepository.ExecuteFilter(data, idAss);
            return lista;
        }

        public List<CUSTO_VARIAVEL_CALCULO> GetAllCalculo(Int32 idAss)
        {
            return _calRepository.GetAllItens(idAss);
        }

        public CUSTO_VARIAVEL_HISTORICO GetCustoVariavelHistoricoById(Int32 id)
        {
            return _hisRepository.GetItemById(id);
        }

        public Int32 EditCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CUSTO_VARIAVEL_HISTORICO obj = _hisRepository.GetById(item.CVHI_CD_ID);
                    _hisRepository.Detach(obj);
                    _hisRepository.Update(item);
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

        public Int32 CreateCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _hisRepository.Add(item);
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

        public List<CUSTO_VARIAVEL_HISTORICO> ExecuteFilterHistorico(DateTime? data, Int32? idAss)
        {
            List<CUSTO_VARIAVEL_HISTORICO> lista = _hisRepository.ExecuteFilter(data, idAss);
            return lista;
        }

        public List<CUSTO_VARIAVEL_HISTORICO> GetAllHistorico(Int32 idAss)
        {
            return _hisRepository.GetAllItens(idAss);
        }

        public List<EMPRESA_CUSTO_VARIAVEL> GetAllCustoVariavel(Int32 idAss)
        {
            return _cvRepository.GetAllItens(idAss);
        }

        public CUSTO_HISTORICO GetCustoHistoricoById(Int32 id)
        {
            return _cuhRepository.GetItemById(id);
        }

        public Int32 CreateCustoHistorico(CUSTO_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _cuhRepository.Add(item);
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

        public Int32 EditCustoHistorico(CUSTO_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CUSTO_HISTORICO obj = _cuhRepository.GetById(item.CUHI_CD_ID);
                    _cuhRepository.Detach(obj);
                    _cuhRepository.Update(item);
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

        public List<CUSTO_HISTORICO> ExecuteFilterCustoHistorico(DateTime? data, Int32? idAss)
        {
            List<CUSTO_HISTORICO> lista = _cuhRepository.ExecuteFilter(data, idAss);
            return lista;
        }

        public List<CUSTO_HISTORICO> GetAllCustoHistorico(Int32 idAss)
        {
            return _cuhRepository.GetAllItens(idAss);
        }

        public COMISSAO GetComissaoById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public Int32 CreateComissao(COMISSAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _comRepository.Add(item);
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

        public Int32 EditComissao(COMISSAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    COMISSAO obj = _comRepository.GetById(item.COMI_CD_ID);
                    _comRepository.Detach(obj);
                    _comRepository.Update(item);
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

        public List<COMISSAO> ExecuteFilterComissao(DateTime? data, Int32? idAss)
        {
            List<COMISSAO> lista = _comRepository.ExecuteFilter(data, idAss);
            return lista;
        }

        public List<COMISSAO> GetAllComissao(Int32 idAss)
        {
            return _comRepository.GetAllItens(idAss);
        }

    }
}
