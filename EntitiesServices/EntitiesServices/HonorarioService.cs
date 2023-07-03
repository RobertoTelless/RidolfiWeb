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
    public class HonorarioService : ServiceBase<HONORARIO>, IHonorarioService
    {
        private readonly IHonorarioRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IHonorarioAnexoRepository _anexoRepository;
        private readonly IHonorarioComentarioRepository _comRepository;
        private readonly ITipoPessoaRepository _pesRepository;
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public HonorarioService(IHonorarioRepository baseRepository, ILogRepository logRepository, IHonorarioAnexoRepository anexoRepository, IHonorarioComentarioRepository comRepository, ITipoPessoaRepository pesRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _comRepository = comRepository;
            _pesRepository = pesRepository;
        }

        public HONORARIO CheckExist(HONORARIO conta)
        {
            HONORARIO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public HONORARIO CheckExist(String nome, String cpf)
        {
            HONORARIO item = _baseRepository.CheckExist(nome, cpf);
            return item;
        }

        public HONORARIO GetItemById(Int32 id)
        {
            HONORARIO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<HONORARIO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<HONORARIO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _pesRepository.GetAllItens();
        }

        public HONORARIO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public HONORARIO_ANOTACOES GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public List<HONORARIO> ExecuteFilter(Int32? tipo, String cpf, String cnpj, String razao, String nome)
        {
            return _baseRepository.ExecuteFilter(tipo, cpf, cnpj, razao, nome);
        }

        public Int32 Create(HONORARIO item, LOG log)
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

        public Int32 Create(HONORARIO item)
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


        public Int32 Edit(HONORARIO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    HONORARIO obj = _baseRepository.GetById(item.HONO_CD_ID);
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

        public Int32 Edit(HONORARIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    HONORARIO obj = _baseRepository.GetById(item.HONO_CD_ID);
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

        public Int32 Delete(HONORARIO item, LOG log)
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
