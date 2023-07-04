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
    public class BeneficiarioService : ServiceBase<BENEFICIARIO>, IBeneficiarioService
    {
        private readonly IBeneficiarioRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IEstadoCivilRepository _estRepository;
        private readonly IBeneficiarioAnexoRepository _anexoRepository;
        private readonly IBeneficiarioComentarioRepository _comRepository;
        private readonly ITipoPessoaRepository _pesRepository;
        private readonly IUFRepository _ufRepository;
        private readonly ISexoRepository _sxRepository;
        private readonly IEscolaridadeRepository _escRepository;
        private readonly IParentescoRepository _parRepository;
        private readonly IContatoRepository _conRepository;
        private readonly IEnderecoRepository _endRepository;
        private readonly ITipoTelefoneBaseRepository _ttRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public BeneficiarioService(IBeneficiarioRepository baseRepository, ILogRepository logRepository, IEstadoCivilRepository estRepository, IBeneficiarioAnexoRepository anexoRepository, IBeneficiarioComentarioRepository comRepository, ITipoPessoaRepository pesRepository, IEscolaridadeRepository escRepository, IUFRepository ufRepository, ISexoRepository sxRepository, IParentescoRepository parRepository, IContatoRepository conRepository, IEnderecoRepository endRepository, ITipoTelefoneBaseRepository ttRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _estRepository = estRepository;
            _anexoRepository = anexoRepository;
            _comRepository = comRepository;
            _pesRepository = pesRepository;
            _ufRepository = ufRepository;
            _sxRepository = sxRepository;
            _escRepository = escRepository; 
            _parRepository = parRepository;
            _conRepository = conRepository;
            _endRepository = endRepository;
            _ttRepository = ttRepository;
        }

        public BENEFICIARIO CheckExist(BENEFICIARIO conta)
        {
            BENEFICIARIO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public BENEFICIARIO CheckExist(String nome, String cpf)
        {
            BENEFICIARIO item = _baseRepository.CheckExist(nome, cpf);
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

        public BENEFICIARIO GetItemById(Int32 id)
        {
            BENEFICIARIO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<BENEFICIARIO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<BENEFICIARIO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<SEXO> GetAllSexo()
        {
            return _sxRepository.GetAllItens();
        }

        public List<ESTADO_CIVIL> GetAllEstadoCivil()
        {
            return _estRepository.GetAllItens();
        }

        public List<TIPO_TELEFONE_BASE> GetAllTipoTelefone()
        {
            return _ttRepository.GetAllItens();
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _pesRepository.GetAllItens();
        }

        public List<ESCOLARIDADE> GetAllEscolaridade()
        {
            return _escRepository.GetAllItens();
        }

        public List<PARENTESCO> GetAllParentesco()
        {
            return _parRepository.GetAllItens();
        }

        public BENEFICIARIO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public BENEFICIARIO_ANOTACOES GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public List<BENEFICIARIO> ExecuteFilter(Int32? tipo, Int32? sexo, Int32? estado, Int32? escolaridade, Int32? parentesco, String razao, String nome, DateTime? dataNasc, String cpf, String cnpj, String parente)
        {
            return _baseRepository.ExecuteFilter(tipo, sexo, estado, escolaridade, parentesco, razao, nome, dataNasc, cpf, cnpj, parente);
        }

        public Int32 EditAnotacao(BENEFICIARIO_ANOTACOES item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    BENEFICIARIO_ANOTACOES obj = _comRepository.GetById(item.BECO_CD_ID);
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

        public Int32 Create(BENEFICIARIO item, LOG log)
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

        public Int32 Create(BENEFICIARIO item)
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


        public Int32 Edit(BENEFICIARIO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    BENEFICIARIO obj = _baseRepository.GetById(item.BENE_CD_ID);
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

        public Int32 Edit(BENEFICIARIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    BENEFICIARIO obj = _baseRepository.GetById(item.BENE_CD_ID);
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

        public Int32 Delete(BENEFICIARIO item, LOG log)
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

        public CONTATO GetContatoById(Int32 id)
        {
            return _conRepository.GetItemById(id);
        }

        public Int32 EditContato(CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTATO obj = _conRepository.GetById(item.CONT_CD_ID);
                    _conRepository.Detach(obj);
                    _conRepository.Update(item);
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

        public Int32 CreateContato(CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _conRepository.Add(item);
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


        public ENDERECO GetEnderecoById(Int32 id)
        {
            return _endRepository.GetItemById(id);
        }

        public Int32 EditEndereco(ENDERECO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ENDERECO obj = _endRepository.GetById(item.ENDE_CD_ID);
                    _endRepository.Detach(obj);
                    _endRepository.Update(item);
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

        public Int32 CreateEndereco(ENDERECO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _endRepository.Add(item);
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
