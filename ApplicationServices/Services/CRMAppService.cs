using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class CRMAppService : AppServiceBase<CRM>, ICRMAppService
    {
        private readonly ICRMService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly IClienteService _cliService;
        private readonly IPrecatorioService _preService;

        public CRMAppService(ICRMService baseService, INotificacaoService notiService, IClienteService cliService, IPrecatorioService preService) : base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _cliService = cliService;
            _preService = preService;
        }

        public List<CRM> GetAllItens(Int32 idAss)
        {
            List<CRM> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CRM> GetAllItensAdm(Int32 idAss)
        {
            List<CRM> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            List<CRM> lista = _baseService.GetTarefaStatus(tipo, idAss);
            return lista;
        }

        public List<CRM> GetByDate(DateTime data, Int32 idAss)
        {
            List<CRM> lista = _baseService.GetByDate(data, idAss);
            return lista;
        }

        public List<CRM> GetByUser(Int32 user)
        {
            List<CRM> lista = _baseService.GetByUser(user);
            return lista;
        }

        public CRM GetItemById(Int32 id)
        {
            CRM item = _baseService.GetItemById(id);
            return item;
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _baseService.GetUserById(id);
            return item;
        }

        public CRM_CONTATO GetContatoById(Int32 id)
        {
            CRM_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public CRM_ACAO GetAcaoById(Int32 id)
        {
            CRM_ACAO lista = _baseService.GetAcaoById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA GetPedidoById(Int32 id)
        {
            CRM_PEDIDO_VENDA lista = _baseService.GetPedidoById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA GetPedidoByNumero(String num, Int32 idAss)
        {
            CRM_PEDIDO_VENDA lista = _baseService.GetPedidoByNumero(num, idAss);
            return lista;
        }

        public CRM CheckExist(CRM tarefa, Int32 idUsu, Int32 idAss)
        {
            CRM item = _baseService.CheckExist(tarefa, idUsu, idAss);
            return item;
        }

        public List<TIPO_CRM> GetAllTipos()
        {
            List<TIPO_CRM> lista = _baseService.GetAllTipos();
            return lista;
        }

        public List<TEMPLATE_PROPOSTA> GetAllTemplateProposta(Int32 id)
        {
            List<TEMPLATE_PROPOSTA> lista = _baseService.GetAllTemplateProposta(id);
            return lista;
        }

        public TEMPLATE_PROPOSTA GetTemplateById(Int32 id)
        {
            TEMPLATE_PROPOSTA lista = _baseService.GetTemplateById(id);
            return lista;
        }

        public List<CRM_ACAO> GetAllAcoes(Int32 idAss)
        {
            List<CRM_ACAO> lista = _baseService.GetAllAcoes(idAss);
            return lista;
        }

        public List<CRM_FOLLOW> GetAllFollow(Int32 idAss)
        {
            List<CRM_FOLLOW> lista = _baseService.GetAllFollow(idAss);
            return lista;
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidos(Int32 idAss)
        {
            List<CRM_PEDIDO_VENDA> lista = _baseService.GetAllPedidos(idAss);
            return lista;
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidosVenda(Int32 idAss)
        {
            List<CRM_PEDIDO_VENDA> lista = _baseService.GetAllPedidosVenda(idAss);
            return lista;
        }

        public List<CRM_PEDIDO_VENDA> GetAllPedidosGeral(Int32 idAss)
        {
            List<CRM_PEDIDO_VENDA> lista = _baseService.GetAllPedidosGeral(idAss);
            return lista;
        }

        public List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss)
        {
            List<TIPO_ACAO> lista = _baseService.GetAllTipoAcao(idAss);
            return lista;
        }

        public List<TIPO_FOLLOW> GetAllTipoFollow(Int32 idAss)
        {
            List<TIPO_FOLLOW> lista = _baseService.GetAllTipoFollow(idAss);
            return lista;
        }

        public List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss)
        {
            List<MOTIVO_CANCELAMENTO> lista = _baseService.GetAllMotivoCancelamento(idAss);
            return lista;
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss)
        {
            List<MOTIVO_ENCERRAMENTO> lista = _baseService.GetAllMotivoEncerramento(idAss);
            return lista;
        }

        public List<CRM_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            List<CRM_ORIGEM> lista = _baseService.GetAllOrigens(idAss);
            return lista;
        }

        public CRM_ANEXO GetAnexoById(Int32 id)
        {
            CRM_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA_ANEXO GetAnexoPedidoById(Int32 id)
        {
            CRM_PEDIDO_VENDA_ANEXO lista = _baseService.GetAnexoPedidoById(id);
            return lista;
        }

        public CRM_COMENTARIO GetComentarioById(Int32 id)
        {
            CRM_COMENTARIO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public CRM_FOLLOW GetFollowById(Int32 id)
        {
            CRM_FOLLOW lista = _baseService.GetFollowById(id);
            return lista;
        }

        public CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetPedidoComentarioById(Int32 id)
        {
            CRM_PEDIDO_VENDA_ACOMPANHAMENTO lista = _baseService.GetPedidoComentarioById(id);
            return lista;
        }

        public Tuple<Int32, List<CRM>, Boolean> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca,  Int32? estrela, Int32? temperatura, Int32? funil, String campanha, Int32 idAss)
        {
            try
            {
                List<CRM> objeto = new List<CRM>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(status, inicio, final, origem, adic, nome, busca, estrela, temperatura, funil, campanha, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ExecuteFilterVenda(String busca, Int32? status, DateTime? inicio, DateTime? final, Int32? filial, Int32? usuario, Int32 idAss, out List<CRM_PEDIDO_VENDA> objeto)
        {
            try
            {
                objeto = new List<CRM_PEDIDO_VENDA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterVenda(busca, status, inicio, final, filial, usuario, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(CRM item, USUARIO usuario)
        {
            try
            {
                //Verifica Campos
                if (item.TIPO_CRM != null)
                {
                    item.TIPO_CRM = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.USUA_CD_ID, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.CRM1_IN_ATIVO = 1;

                // Serializa registro
                PRECATORIO cli = _preService.GetItemById(item.PREC_CD_ID.Value);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.PREC_NM_PRECATORIO + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME;

                item.USUA_CD_ID = cli.USUA_CD_ID;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddCRM",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = serial
                };
                
                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera diario
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Criação de Processo";
                dia.DIPR_DS_DESCRICAO = "Criação do Processo " + item.CRM1_NM_NOME + ". Precatório: " + cli.PREC_NM_PRECATORIO;
                Int32 volta1 = _baseService.CreateDiario(dia);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Now;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Atribuição de Processo CRM";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Processo CRM " + item.CRM1_NM_NOME + " do cliente " + cli.PREC_NM_PRECATORIO + " foi colocado sob sua responsabilidade em "  + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta5 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(CRM item, CRM itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME;
                String antes = itemAntes.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + itemAntes.CRM1_CD_ID.ToString() + "|" + itemAntes.CRM1_DS_DESCRICAO + "|" + itemAntes.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + itemAntes.CRM1_IN_ATIVO.ToString() + "|" + itemAntes.CRM1_IN_STATUS.ToString() + "|" + itemAntes.CRM1_NM_NOME;

                // Monta Log
                LOG log = new LOG();
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "CancCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }
                else if (item.CRM1_DT_PREVISAO_ENTREGA != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "ExpeCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }
                else
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "EditCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item, log);

                // Gera diario
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Alteração de Processo";
                dia.DIPR_DS_DESCRICAO = "Alteração do Processo " + item.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                Int32 volta3 = _baseService.CreateDiario(dia);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Alteração de Processo CRM";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Processo CRM " + item.CRM1_NM_NOME + " do cliente " + cli.CLIE_NM_NOME + " sob sua responsabilidade, foi alterado em " + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = usuario.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditSimples(CRM item, CRM itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME;
                String antes = itemAntes.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + itemAntes.CRM1_CD_ID.ToString() + "|" + itemAntes.CRM1_DS_DESCRICAO + "|" + itemAntes.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + itemAntes.CRM1_IN_ATIVO.ToString() + "|" + itemAntes.CRM1_IN_STATUS.ToString() + "|" + itemAntes.CRM1_NM_NOME;

                // Monta Log
                LOG log = new LOG();
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "CancCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }
                else
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "EditCRM",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CRM item, CRM itemAntes)
        {
            try
            {
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CRM item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                // Acerta campos
                item.CRM1_IN_ATIVO = 2;

                // Verifica integridade
                List<CRM_ACAO> acao = item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList();
                if (acao.Count > 0)
                {
                    return 1;
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME + "|" + item.CRM_ORIGEM.CROR_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCRM",
                    LOG_TX_REGISTRO = serial
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);

                // Gera diario
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Exclusão de Processo";
                dia.DIPR_DS_DESCRICAO = "Exclusão do Processo " + item.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                Int32 volta3 = _baseService.CreateDiario(dia);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CRM item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CRM1_IN_ATIVO = 1;

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRM1_CD_ID.ToString() + "|" + item.CRM1_DS_DESCRICAO + "|" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "|" + item.CRM1_IN_ATIVO.ToString() + "|" + item.CRM1_IN_STATUS.ToString() + "|" + item.CRM1_NM_NOME + "|" + item.CRM_ORIGEM.CROR_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCRM",
                    LOG_TX_REGISTRO = serial
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                // Gera diario
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Reativação de Processo";
                dia.DIPR_DS_DESCRICAO = "Reativação do Processo " + item.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                Int32 volta3 = _baseService.CreateDiario(dia);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditContato(CRM_CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(CRM_CONTATO item)
        {
            try
            {
                item.CRCO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnotacao(CRM_COMENTARIO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditFollow(CRM_FOLLOW item)
        {
            try
            {
                // Persiste
                return _baseService.EditFollow(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAcao(CRM_ACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAcao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAcao(CRM_ACAO item, USUARIO usuario)
        {
            try
            {
                item.CRAC_IN_ATIVO = 1;

                // Recupera CRM
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);
                CLIENTE cli = _cliService.GetItemById(crm.CLIE_CD_ID);

                // Persiste
                Int32 volta = _baseService.CreateAcao(item);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Atribuição de Ação de Processo CRM";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Ação " + item.CRAC_NM_TITULO + " do processo CRM " + crm.CRM1_NM_NOME + " foi colocada sob sua responsabilidade em " + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = usuario.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditPedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEditPedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                item.CRPV_IN_ATIVO = 1;
                item.CRPV_IN_STATUS = 1;

                // Recupera CRM
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);

                // Persiste
                Int32 volta = _baseService.CreatePedido(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                item.CRPV_IN_ATIVO = 1;
                item.CRPV_IN_STATUS = 1;
                item.CRPV_VL_TOTAL_ITENS = 0;
                item.CRPV_VL_TOTAL_SERVICOS = 0;
                item.CRPV_VL_TOTAL = 0;
                item.CRPV_VL_DESCONTO = 0;
                item.CRPV_VL_FRETE = 0;
                item.CRPV_VL_ICMS = 0;
                item.CRPV_VL_IPI = 0;
                item.CRPV_VL_PESO_BRUTO = 0;
                item.CRPV_VL_PESO_LIQUIDO = 0;
                item.CRPV_IN_PRAZO_ENTREGA = 0;

                // Persiste
                Int32 volta = _baseService.CreatePedido(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public CRM_PEDIDO_VENDA_ITEM GetItemPedidoById(Int32 id)
        //{
        //    CRM_PEDIDO_VENDA_ITEM lista = _baseService.GetItemPedidoById(id);
        //    return lista;
        //}

        //public Int32 ValidateCreateItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        //{
        //    item.CRPI_IN_ATIVO = 1;

        //    try
        //    {
        //        // Persiste
        //        Int32 volta = _baseService.CreateItemPedido(item);
        //        return volta;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public Int32 ValidateEditItemPedido(CRM_PEDIDO_VENDA_ITEM item)
        //{
        //    try
        //    {
        //        // Persiste
        //        return _baseService.EditItemPedido(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public Int32 ValidateCancelarPedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 3;
                if (item.CRPV_DT_CANCELAMENTO == null)
                {
                    return 1;
                }
                if (item.CRPV_DS_CANCELAMENTO == null)
                {
                    return 2;
                }
                if (item.CRPV_DT_CANCELAMENTO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_CANCELAMENTO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Cancelamento de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Pedido " + item.CRPV_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi cancelado em " + item.CRPV_DT_CANCELAMENTO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCancelarPedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 3;
                if (item.CRPV_DT_CANCELAMENTO == null)
                {
                    return 1;
                }
                if (item.CRPV_DS_CANCELAMENTO == null)
                {
                    return 2;
                }
                if (item.CRPV_DT_CANCELAMENTO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_CANCELAMENTO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Cancelamento de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Pedido " + item.CRPV_NR_NUMERO + " foi cancelado em " + item.CRPV_DT_CANCELAMENTO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReprovarPedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 4;
                if (item.CRPV_DT_REPROVACAO == null)
                {
                    return 1;
                }
                if (item.CRPV_DS_REPROVACAO == null)
                {
                    return 2;
                }
                if (item.CRPV_DT_REPROVACAO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_REPROVACAO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Reprovação de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: P Pedido " + item.CRPV_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi reprovado pelo cliente em " + item.CRPV_DT_REPROVACAO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReprovarPedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 4;
                if (item.CRPV_DT_REPROVACAO == null)
                {
                    return 1;
                }
                if (item.CRPV_DS_REPROVACAO == null)
                {
                    return 2;
                }
                if (item.CRPV_DT_REPROVACAO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_REPROVACAO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Reprovação de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Pedido " + item.CRPV_NR_NUMERO + " foi reprovado pelo cliente em " + item.CRPV_DT_REPROVACAO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateAprovarPedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 5;
                if (item.CRPV_DT_APROVACAO == null)
                {
                    return 1;
                }
                //if (item.CRPV_DS_APROVACAO == null)
                //{
                //    return 2;
                //}
                if (item.CRPV_DT_APROVACAO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_APROVACAO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Aprovação de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Pedido " + item.CRPV_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi aprovado pelo cliente em " + item.CRPV_DT_APROVACAO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateAprovarPedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 6;
                if (item.CRPV_DT_FATURAMENTO == null)
                {
                    return 1;
                }
                if (item.CRPV_DT_FATURAMENTO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_FATURAMENTO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Faturamento de Venda";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Venda " + item.CRPV_NR_NUMERO + " foi faturada em " + item.CRPV_DT_FATURAMENTO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateExpedicaoPedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 7;
                if (item.CRPV_DT_EXPEDICAO == null)
                {
                    return 1;
                }
                if (item.CRPV_DT_EXPEDICAO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_EXPEDICAO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Expedição de Venda";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Venda " + item.CRPV_NR_NUMERO + " foi para expedição em " + item.CRPV_DT_EXPEDICAO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEntregarPedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 7;
                if (item.CRPV_DT_ENTREGA == null)
                {
                    return 1;
                }
                if (item.CRPV_DT_ENTREGA < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_ENTREGA > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Expedição de Venda";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Venda " + item.CRPV_NR_NUMERO + " foi entregue em " + item.CRPV_DT_ENTREGA.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateAprovarPedidoDireto(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 5;
                if (item.CRPV_DT_APROVACAO == null)
                {
                    return 1;
                }
                //if (item.CRPV_DS_APROVACAO == null)
                //{
                //    return 2;
                //}
                if (item.CRPV_DT_APROVACAO < item.CRPV_DT_PEDIDO)
                {
                    return 3;
                }
                if (item.CRPV_DT_APROVACAO > DateTime.Today.Date)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Aprovação de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Pedido " + item.CRPV_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi aprovado pelo cliente em " + item.CRPV_DT_APROVACAO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnviarPedido(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 2;
                if (item.CRPV_DT_ENVIO == null)
                {
                    return 1;
                }
                if (item.CRPV_DT_ENVIO < item.CRPV_DT_PEDIDO)
                {
                    return 2;
                }
                if (item.CRPV_DT_ENVIO > DateTime.Today.Date)
                {
                    return 3;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID.Value);
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Envio de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Pedido " + item.CRPV_NR_NUMERO + " do processo CRM " + crm.CRM1_NM_NOME + " foi enviado para o cliente em " + item.CRPV_DT_ENVIO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnviarPedidoVenda(CRM_PEDIDO_VENDA item)
        {
            try
            {
                // Verificação
                item.CRPV_IN_STATUS = 2;
                if (item.CRPV_DT_ENVIO == null)
                {
                    return 1;
                }
                if (item.CRPV_DT_ENVIO < item.CRPV_DT_PEDIDO)
                {
                    return 2;
                }
                if (item.CRPV_DT_ENVIO > DateTime.Today.Date)
                {
                    return 3;
                }
                if (item.TEPR_CD_ID == null)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.EditPedido(item);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID.Value;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Envio de Pedido";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Pedido " + item.CRPV_NR_NUMERO + " foi enviado para o cliente em " + item.CRPV_DT_ENVIO.Value.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
