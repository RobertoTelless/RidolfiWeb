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
    public class PesquisaService : ServiceBase<PESQUISA>, IPesquisaService
    {
        private readonly IPesquisaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoPesquisaRepository _tipoRepository;
        private readonly IPesquisaAnexoRepository _anexoRepository;
        private readonly IPesquisaItemRepository _pesRepository;
        private readonly IPesquisaAnotacaoRepository _anoRepository;
        private readonly ITipoItemPesquisaRepository _tipRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IPesquisaRespostaRepository _resRepository;
        private readonly IPesquisaItemOpcaoRepository _opRepository;
        private readonly IPesquisaEnvioRepository _envRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public PesquisaService(IPesquisaRepository baseRepository, ILogRepository logRepository, ITipoPesquisaRepository tipoRepository, IPesquisaAnexoRepository anexoRepository, IPesquisaItemRepository pesRepository, IPesquisaAnotacaoRepository anoRepository, ITipoItemPesquisaRepository tipRepository, IUsuarioRepository usuRepository, IPesquisaRespostaRepository resRepository, IPesquisaItemOpcaoRepository opRepository, IPesquisaEnvioRepository envRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _anoRepository = anoRepository;
            _pesRepository = pesRepository;
            _tipRepository = tipRepository;
            _usuRepository = usuRepository;
            _resRepository = resRepository;
            _opRepository = opRepository;
            _envRepository = envRepository;
        }

        public PESQUISA CheckExist(PESQUISA conta, Int32 idAss)
        {
            PESQUISA item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public PESQUISA GetItemById(Int32 id)
        {
            PESQUISA item = _baseRepository.GetItemById(id);
            return item;
        }

        public PESQUISA_ANOTACAO GetComentarioById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public List<PESQUISA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<PESQUISA_RESPOSTA> GetAllRespostas(Int32 idAss)
        {
            return _resRepository.GetAllItens(idAss);
        }

        public List<PESQUISA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_PESQUISA> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public List<TIPO_ITEM_PESQUISA> GetAllTiposItem(Int32 idAss)
        {
            return _tipRepository.GetAllItens(idAss);
        }

        public PESQUISA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public PESQUISA_ITEM GetItemPesquisaById(Int32 id)
        {
            return _pesRepository.GetItemById(id);
        }

        public PESQUISA_ENVIO GetPesquisaEnvioById(Int32 id)
        {
            return _envRepository.GetItemById(id);
        }

        public PESQUISA_RESPOSTA GetPesquisaRespostaById(Int32 id)
        {
            return _resRepository.GetItemById(id);
        }

        public PESQUISA_ITEM_OPCAO GetItemOpcaoPesquisaById(Int32 id)
        {
            return _opRepository.GetItemById(id);
        }

        public List<PESQUISA> ExecuteFilter(Int32? catId, String nome, String descricao, String campanha, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(catId, nome, descricao, campanha, idAss);
        }

        public Int32 Create(PESQUISA item, LOG log)
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

        public Int32 Create(PESQUISA item)
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


        public Int32 Edit(PESQUISA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PESQUISA obj = _baseRepository.GetById(item.PESQ_CD_ID);
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

        public Int32 Edit(PESQUISA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PESQUISA obj = _baseRepository.GetById(item.PESQ_CD_ID);
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

        public Int32 Delete(PESQUISA item, LOG log)
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

        public Int32 EditItemPesquisa(PESQUISA_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PESQUISA_ITEM obj = _pesRepository.GetById(item.PEIT_CD_ID);
                    _pesRepository.Detach(obj);
                    _pesRepository.Update(item);
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

        public Int32 CreateItemPesquisa(PESQUISA_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pesRepository.Add(item);
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

        public Int32 EditAnotacao(PESQUISA_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    PESQUISA_ANOTACAO obj = _anoRepository.GetById(item.PECO_CD_ID);
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

        public Int32 EditItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PESQUISA_ITEM_OPCAO obj = _opRepository.GetById(item.PEIO_CD_ID);
                    _opRepository.Detach(obj);
                    _opRepository.Update(item);
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

        public Int32 CreateItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _opRepository.Add(item);
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

        public Int32 CreatePesquisaEnvio(PESQUISA_ENVIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _envRepository.Add(item);
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

        public Int32 EditPesquisaResposta(PESQUISA_RESPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PESQUISA_RESPOSTA obj = _resRepository.GetById(item.PERE_CD_ID);
                    _resRepository.Detach(obj);
                    _resRepository.Update(item);
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

        public Int32 CreatePesquisaResposta(PESQUISA_RESPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _resRepository.Add(item);
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
