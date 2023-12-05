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
    public class CRMService : ServiceBase<CRM>, ICRMService
    {
        private readonly ICRMRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoCRMRepository _tipoRepository;
        private readonly ICRMAnexoRepository _anexoRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly ICRMOrigemRepository _oriRepository;
        private readonly IMotivoCancelamentoRepository _mcRepository;
        private readonly IMotivoEncerramentoRepository _meRepository;
        private readonly ITipoAcaoRepository _taRepository;
        private readonly ICRMAcaoRepository _acaRepository;
        private readonly ICRMContatoRepository _conRepository;
        private readonly ICRMComentarioRepository _comRepository;
        private readonly ITemplatePropostaRepository _tpRepository;
        private readonly ICRMPedidoAnexoRepository _anepeRepository;
        private readonly ICRMPedidoComentarioRepository _compeRepository;
        private readonly ICRMPedidoRepository _pedRepository;
        private readonly ICRMDiarioRepository _diaRepository;
        private readonly ICRMFollowRepository _folRepository;
        private readonly ITipoFollowRepository _tfRepository;

        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public CRMService(ICRMRepository baseRepository, ILogRepository logRepository, ITipoCRMRepository tipoRepository, ICRMAnexoRepository anexoRepository, IUsuarioRepository usuRepository, ICRMOrigemRepository oriRepository, IMotivoCancelamentoRepository mcRepository, IMotivoEncerramentoRepository meRepository, ITipoAcaoRepository taRepository, ICRMAcaoRepository acaRepository, ICRMContatoRepository conRepository, ICRMComentarioRepository comRepository, ITemplatePropostaRepository tpRepository, ICRMPedidoAnexoRepository anepeRepository, ICRMPedidoComentarioRepository compeRepository, ICRMPedidoRepository pedRepository, ICRMDiarioRepository diaRepository, ICRMFollowRepository folRepository, ITipoFollowRepository tfRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _usuRepository = usuRepository;
            _oriRepository = oriRepository;
            _mcRepository = mcRepository;
            _meRepository = meRepository;
            _taRepository = taRepository;
            _acaRepository = acaRepository;
            _conRepository = conRepository;
            _comRepository = comRepository;
            _tpRepository = tpRepository;
            _anepeRepository = anepeRepository;
            _compeRepository = compeRepository;
            _pedRepository = pedRepository; 
            _diaRepository = diaRepository;
            _folRepository = folRepository;
            _tfRepository = tfRepository;
        }

        public CRM CheckExist(CRM tarefa, Int32 idUsu,  Int32 idAss)
        {
            CRM item = _baseRepository.CheckExist(tarefa, idUsu, idAss);
            return item;
        }

        public CRM GetItemById(Int32 id)
        {
            CRM item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CRM> GetByDate(DateTime data, Int32 idAss)
        {
            return _baseRepository.GetByDate(data, idAss);
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _usuRepository.GetItemById(id);
            return item;
        }

        public List<CRM> GetByUser(Int32 user)
        {
            return _baseRepository.GetByUser(user);
        }

        public List<CRM_ACAO> GetAllAcoes(Int32 idAss)
        {
            return _acaRepository.GetAllItens(idAss);
        }

        public List<CRM_FOLLOW> GetAllFollow(Int32 idAss)
        {
            return _folRepository.GetAllItens(idAss);
        }

        public List<CRM_COMENTARIO> GetAllAnotacao(Int32 idAss)
        {
            return _comRepository.GetAllItens(idAss);
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidos(Int32 idAss)
        {
            return _pedRepository.GetAllItens(idAss);
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidosVenda(Int32 idAss)
        {
            return _pedRepository.GetAllItensVenda(idAss);
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidosGeral(Int32 idAss)
        {
            return _pedRepository.GetAllItensGeral(idAss);
        }

        public List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            return _baseRepository.GetTarefaStatus(tipo, idAss);
        }

        public CRM_CONTATO GetContatoById(Int32 id)
        {
            return _conRepository.GetItemById(id);
        }

        public CRM_ACAO GetAcaoById(Int32 id)
        {
            return _acaRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA GetPedidoById(Int32 id)
        {
            return _pedRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA GetPedidoByNumero(String num, Int32 idAss)
        {
            return _pedRepository.GetItemByNumero(num, idAss);
        }

        public List<CRM> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CRM> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_CRM> GetAllTipos()
        {
            return _tipoRepository.GetAllItens();
        }

        public List<TEMPLATE_PROPOSTA> GetAllTemplateProposta(Int32 id)
        {
            return _tpRepository.GetAllItens(id);
        }

        public TEMPLATE_PROPOSTA GetTemplateById(Int32 id)
        {
            return _tpRepository.GetItemById(id);
        }

        public List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss)
        {
            return _taRepository.GetAllItens(idAss);
        }

        public List<TIPO_FOLLOW> GetAllTipoFollow(Int32 idAss)
        {
            return _tfRepository.GetAllItens(idAss);
        }

        public List<CRM_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            return _oriRepository.GetAllItens(idAss);
        }

        public List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss)
        {
            return _mcRepository.GetAllItens(idAss);
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss)
        {
            return _meRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsers(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public CRM_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA_ANEXO GetAnexoPedidoById(Int32 id)
        {
            return _anepeRepository.GetItemById(id);
        }

        public CRM_COMENTARIO GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public CRM_FOLLOW GetFollowById(Int32 id)
        {
            return _folRepository.GetItemById(id);
        }

        public CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetPedidoComentarioById(Int32 id)
        {
            return _compeRepository.GetItemById(id);
        }

        public List<CRM> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32? funil, String campanha, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(status, inicio, final, origem, adic, nome, busca, estrela, temperatura, funil, campanha, idAss);

        }

        public List<CRM_PEDIDO_VENDA> ExecuteFilterVenda(String busca, Int32? status, DateTime? inicio, DateTime? final, Int32? filial, Int32? usuario, Int32 idAss)
        {
            return _pedRepository.ExecuteFilter(busca, status, inicio, final, filial, usuario, idAss);

        }

        public Int32 CreateDiario(DIARIO_PROCESSO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _diaRepository.Add(item);
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

        public Int32 Create(CRM item, LOG log)
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

        public Int32 Create(CRM item)
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


        public Int32 Edit(CRM item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM obj = _baseRepository.GetById(item.CRM1_CD_ID);
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

        public Int32 Edit(CRM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM obj = _baseRepository.GetById(item.CRM1_CD_ID);
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

        public Int32 Delete(CRM item, LOG log)
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

        public Int32 EditContato(CRM_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_CONTATO obj = _conRepository.GetById(item.CRCO_CD_ID);
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

        public Int32 CreateContato(CRM_CONTATO item)
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

        public Int32 EditAnotacao(CRM_COMENTARIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    CRM_COMENTARIO obj = _comRepository.GetById(item.CRCM_CD_ID);
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

        public Int32 EditFollow(CRM_FOLLOW item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    item.TIPO_FOLLOW = null;
                    CRM_FOLLOW obj = _folRepository.GetById(item.CRFL_CD_ID);
                    _folRepository.Detach(obj);
                    _folRepository.Update(item);
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

        public Int32 EditAcao(CRM_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_ACAO obj = _acaRepository.GetById(item.CRAC_CD_ID);
                    _acaRepository.Detach(obj);
                    _acaRepository.Update(item);
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

        public Int32 CreateAcao(CRM_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _acaRepository.Add(item);
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

        public Int32 EditPedido(CRM_PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.FILIAL = null;
                    item.USUARIO = null;
                    item.TEMPLATE_PROPOSTA = null;
                    CRM_PEDIDO_VENDA obj = _pedRepository.GetById(item.CRPV_CD_ID);
                    _pedRepository.Detach(obj);
                    _pedRepository.Update(item);
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

        public Int32 CreatePedido(CRM_PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pedRepository.Add(item);
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

        public Int32 EditAnexo(CRM_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CRM_ANEXO obj = _anexoRepository.GetById(item.CRAN_CD_ID);
                    _anexoRepository.Detach(obj);
                    _anexoRepository.Update(item);
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

        //public CRM_PEDIDO_VENDA_ITEM GetItemPedidoById(Int32 id)
        //{
        //    CRM_PEDIDO_VENDA_ITEM item = _itepeRepository.GetItemById(id);
        //    return item;
        //}

        //public Int32 EditItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        //{
        //    using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
        //    {
        //        try
        //        {
        //            item.PRODUTO = null;
        //            item.UNIDADE = null;  
        //            item.SERVICO = null;    
        //            CRM_PEDIDO_VENDA_ITEM obj = _itepeRepository.GetItemById(item.CRPI_CD_ID);
        //            _itepeRepository.Detach(obj);
        //            _itepeRepository.Update(item);
        //            transaction.Commit();
        //            return 0;
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            throw ex;
        //        }
        //    }
        //}

        //public Int32 CreateItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        //{
        //    using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
        //    {
        //        try
        //        {
        //            _itepeRepository.Add(item);
        //            transaction.Commit();
        //            return 0;
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            throw ex;
        //        }
        //    }
        //}




    }
}
