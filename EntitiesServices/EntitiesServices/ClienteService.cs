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
    public class ClienteService : ServiceBase<CLIENTE>, IClienteService
    {
        private readonly IClienteRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaClienteRepository _tipoRepository;
        private readonly IClienteAnexoRepository _anexoRepository;
        private readonly IClienteContatoRepository _contRepository;
        private readonly IClienteAnotacaoRepository _tagRepository;
        private readonly ITipoPessoaRepository _pesRepository;
        private readonly ITipoContribuinteRepository _tcRepository;
        private readonly IClienteReferenciaRepository _refRepository;
        private readonly IUFRepository _ufRepository;
        private readonly IRegimeTributarioRepository _rtRepository;
        private readonly ISexoRepository _sxRepository;
        private readonly IClienteFalhaRepository _flRepository;
        private readonly IGrupoContatoRepository _gruRepository;
        private readonly IClienteAnotacaoRepository _anoRepository;
        private readonly ILinguaRepository _linRepository;
        private readonly INacionalidadeRepository _nacRepository;
        private readonly IMunicipioRepository _munRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public ClienteService(IClienteRepository baseRepository, ILogRepository logRepository, ICategoriaClienteRepository tipoRepository, IClienteAnexoRepository anexoRepository, ITipoPessoaRepository pesRepository, IClienteAnotacaoRepository tagRepository, IClienteContatoRepository contRepository, IClienteReferenciaRepository refRepository, IUFRepository ufRepository, ITipoContribuinteRepository tcRepository, IRegimeTributarioRepository rtRepository, ISexoRepository sxRepository, IClienteFalhaRepository flRepository,IGrupoContatoRepository gruRepository, IClienteAnotacaoRepository anoRepository, ILinguaRepository linRepository, INacionalidadeRepository nacRepository, IMunicipioRepository munRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _contRepository = contRepository;
            _pesRepository = pesRepository;
            _tagRepository = tagRepository;
            _refRepository = refRepository;
            _ufRepository = ufRepository;
            _tcRepository = tcRepository;
            _rtRepository = rtRepository;
            _sxRepository = sxRepository;
            _flRepository = flRepository;
            _gruRepository = gruRepository;
            _anoRepository = anoRepository;
            _linRepository = linRepository;
            _nacRepository = nacRepository;
            _munRepository = munRepository;
        }

        public CLIENTE CheckExist(CLIENTE conta, Int32 idAss)
        {
            CLIENTE item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public CLIENTE CheckExistDoctos(String cpf, String cnpj, Int32 idAss)
        {
            CLIENTE item = _baseRepository.CheckExistDoctos(cpf, cnpj, idAss);
            return item;
        }

        public MUNICIPIO CheckExistMunicipio(MUNICIPIO conta)
        {
            MUNICIPIO item = _munRepository.CheckExist(conta);
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

        public CLIENTE GetItemById(Int32 id)
        {
            CLIENTE item = _baseRepository.GetItemById(id);
            return item;
        }

        public CLIENTE_ANOTACAO GetComentarioById(Int32 id)
        {
            return _tagRepository.GetItemById(id);
        }

        public CLIENTE GetByEmail(String email)
        {
            CLIENTE item = _baseRepository.GetByEmail(email);
            return item;
        }

        public List<CLIENTE> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CLIENTE> GetAllItensUltimos(Int32 num, Int32 idAss)
        {
            return _baseRepository.GetAllItensUltimos(num, idAss);
        }

        public List<CLIENTE> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<REGIME_TRIBUTARIO> GetAllRegimes(Int32 idAss)
        {
            return _rtRepository.GetAllItens();
        }

        public List<LINGUA> GetAllLinguas()
        {
            return _linRepository.GetAllItens();
        }

        public List<NACIONALIDADE> GetAllNacionalidades()
        {
            return _nacRepository.GetAllItens();
        }

        public List<MUNICIPIO> GetAllMunicipios()
        {
            return _munRepository.GetAllItens();
        }

        public List<MUNICIPIO> GetMunicipioByUF(Int32 uf)
        {
            return _munRepository.GetMunicipioByUF(uf);
        }

        public List<CLIENTE_FALHA> GetAllFalhas(Int32 idAss)
        {
            return _flRepository.GetAllItens();
        }

        public List<SEXO> GetAllSexo()
        {
            return _sxRepository.GetAllItens();
        }

        public List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _pesRepository.GetAllItens();
        }

        public CLIENTE_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public GRUPO_CLIENTE GetGrupoById(Int32 id)
        {
            return _gruRepository.GetItemById(id);
        }

        public List<TIPO_CONTRIBUINTE> GetAllContribuinte(Int32 idAss)
        {
            return _tcRepository.GetAllItens(idAss);
        }

        public CLIENTE_CONTATO GetContatoById(Int32 id)
        {
            return _contRepository.GetItemById(id);
        }

        public CLIENTE_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public CLIENTE_REFERENCIA GetReferenciaById(Int32 id)
        {
            return _refRepository.GetItemById(id);
        }

        public List<CLIENTE> ExecuteFilter(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32? filial, Int32? usu, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(id, catId, razao, nome, cpf, cnpj, email, cidade, uf, ativo, filial, usu, idAss);
        }

        public Int32 CreateMunicipio(MUNICIPIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _munRepository.Add(item);
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

        public Int32 Create(CLIENTE item, LOG log)
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

        public Int32 Create(CLIENTE item)
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


        public Int32 Edit(CLIENTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CLIENTE obj = _baseRepository.GetById(item.CLIE_CD_ID);
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

        public Int32 Edit(CLIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CLIENTE obj = _baseRepository.GetById(item.CLIE_CD_ID);
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

        public Int32 Delete(CLIENTE item, LOG log)
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

        public Int32 EditContato(CLIENTE_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CLIENTE_CONTATO obj = _contRepository.GetById(item.CLCO_CD_ID);
                    _contRepository.Detach(obj);
                    _contRepository.Update(item);
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

        public Int32 EditAnotacao(CLIENTE_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    CLIENTE_ANOTACAO obj = _anoRepository.GetById(item.CLAN_CD_ID);
                    _anoRepository.Detach(obj);
                    _anoRepository.Update(item);
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

        public Int32 CreateContato(CLIENTE_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _contRepository.Add(item);
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

        public Int32 EditReferencia(CLIENTE_REFERENCIA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CLIENTE_REFERENCIA obj = _refRepository.GetById(item.CLRE_CD_ID);
                    _refRepository.Detach(obj);
                    _refRepository.Update(item);
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

        public Int32 CreateReferencia(CLIENTE_REFERENCIA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _refRepository.Add(item);
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

        public Int32 EditFalha(CLIENTE_FALHA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CLIENTE_FALHA obj = _flRepository.GetById(item.CLFA_CD_ID);
                    _flRepository.Detach(obj);
                    _flRepository.Update(item);
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

        public Int32 CreateFalha(CLIENTE_FALHA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _flRepository.Add(item);
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

        public Int32 CreateGrupo(GRUPO_CLIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _gruRepository.Add(item);
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

        public Int32 EditGrupo(GRUPO_CLIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    GRUPO_CLIENTE obj = _gruRepository.GetById(item.GRCL_CD_ID);
                    _gruRepository.Detach(obj);
                    _gruRepository.Update(item);
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
