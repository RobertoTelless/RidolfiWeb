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
using System.Net;

namespace ApplicationServices.Services
{
    public class PesquisaAppService : AppServiceBase<PESQUISA>, IPesquisaAppService
    {
        private readonly IPesquisaService _baseService;
        private readonly IConfiguracaoService _confService;
        private readonly ITemplateEMailService _teemService;
        private readonly IClienteService _cliService;
        private readonly IMensagemEnviadaSistemaService _meService;
        private readonly IGrupoService _gruService;

        public PesquisaAppService(IPesquisaService baseService, IConfiguracaoService confService, ITemplateEMailService teemService, IClienteService cliService, IMensagemEnviadaSistemaService meService, IGrupoService gruService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            _teemService = teemService;
            _cliService = cliService;   
            _meService = meService;
            _gruService = gruService;
        }

        public List<PESQUISA> GetAllItens(Int32 idAss)
        {
            List<PESQUISA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<PESQUISA_RESPOSTA> GetAllRespostas(Int32 idAss)
        {
            List<PESQUISA_RESPOSTA> lista = _baseService.GetAllRespostas(idAss);
            return lista;
        }

        public PESQUISA_ANOTACAO GetComentarioById(Int32 id)
        {
            return _baseService.GetComentarioById(id);
        }

        public List<PESQUISA> GetAllItensAdm(Int32 idAss)
        {
            List<PESQUISA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public PESQUISA GetItemById(Int32 id)
        {
            PESQUISA item = _baseService.GetItemById(id);
            return item;
        }

        public PESQUISA CheckExist(PESQUISA conta, Int32 idAss)
        {
            PESQUISA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_PESQUISA> GetAllTipos(Int32 idAss)
        {
            List<TIPO_PESQUISA> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<TIPO_ITEM_PESQUISA> GetAllTiposItem(Int32 idAss)
        {
            List<TIPO_ITEM_PESQUISA> lista = _baseService.GetAllTiposItem(idAss);
            return lista;
        }

        public PESQUISA_ANEXO GetAnexoById(Int32 id)
        {
            PESQUISA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public PESQUISA_ITEM GetItemPesquisaById(Int32 id)
        {
            PESQUISA_ITEM lista = _baseService.GetItemPesquisaById(id);
            return lista;
        }

        public PESQUISA_RESPOSTA GetPesquisaRespostaById(Int32 id)
        {
            PESQUISA_RESPOSTA lista = _baseService.GetPesquisaRespostaById(id);
            return lista;
        }

        public PESQUISA_ENVIO GetPesquisaEnvioById(Int32 id)
        {
            PESQUISA_ENVIO lista = _baseService.GetPesquisaEnvioById(id);
            return lista;
        }

        public PESQUISA_ITEM_OPCAO GetItemOpcaoPesquisaById(Int32 id)
        {
            PESQUISA_ITEM_OPCAO lista = _baseService.GetItemOpcaoPesquisaById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, String nome, String descricao, String campanha, Int32 idAss, out List<PESQUISA> objeto)
        {
            try
            {
                objeto = new List<PESQUISA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, nome, descricao, campanha, idAss);
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

        public Tuple<Int32, List<PESQUISA>, Boolean> ExecuteFilterTuple(Int32? catId, String nome, String descricao, String campanha, Int32 idAss)
        {
            try
            {
                List<PESQUISA> objeto = new List<PESQUISA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, nome, descricao, campanha, idAss);
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

        public Int32 ValidateCreate(PESQUISA item, USUARIO usuario)
        {
            try
            {
                // Verifica Existencia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Checa validade
                if (item.PESQ_DT_VALIDADE <= item.PESQ_DT_CRIACAO)
                {
                    return 2;
                }

                // Completa objeto
                item.PESQ_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPESQ",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PESQUISA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnotacao(PESQUISA_ANOTACAO item)
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

        public Int32 ValidateEdit(PESQUISA item, PESQUISA itemAntes, USUARIO usuario)
        {
            try
            {
                if (itemAntes.ASSINANTE != null)
                {
                    itemAntes.ASSINANTE = null;
                }
                if (itemAntes.USUARIO != null)
                {
                    itemAntes.USUARIO = null;
                }

                // Checa validade
                if (item.PESQ_DT_VALIDADE <= item.PESQ_DT_CRIACAO)
                {
                    return 2;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPESQ",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PESQUISA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PESQUISA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PESQUISA item, PESQUISA itemAntes)
        {
            try
            {
                // Checa validade
                if (item.PESQ_DT_VALIDADE <= item.PESQ_DT_CRIACAO)
                {
                    return 2;
                }

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PESQUISA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (true)
                {

                }

                // Acerta campos
                item.PESQ_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPESQ",
                    LOG_TX_REGISTRO = "Nome: " + item.PESQ_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PESQUISA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.PESQ_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPESQ",
                    LOG_TX_REGISTRO = "Nome: " + item.PESQ_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditItemPesquisa(PESQUISA_ITEM item, PESQUISA pesq)
        {
            try
            {
                // Verifica ordem
                List<PESQUISA_ITEM> its = pesq.PESQUISA_ITEM.ToList();
                PESQUISA_ITEM pi = its.Where(p => p.PEIT_IN_ORDEM == item.PEIT_IN_ORDEM).FirstOrDefault();
                if (pi != null)
                {
                    return 1;
                }

                // Persiste
                return _baseService.EditItemPesquisa(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateItemPesquisa(PESQUISA_ITEM item, PESQUISA pesq)
        {
            try
            {
                // Verifica ordem
                List<PESQUISA_ITEM> its = pesq.PESQUISA_ITEM.ToList();
                PESQUISA_ITEM pi = its.Where(p => p.PEIT_IN_ORDEM == item.PEIT_IN_ORDEM).FirstOrDefault();
                if (pi != null)
                {
                    return 1;
                }
                
                // Completa campos
                item.PEIT_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateItemPesquisa(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDeleteItemPesquisa(PESQUISA_ITEM item)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PESQUISA_RESPOSTA.Count > 0 || item.PESQUISA_RESPOSTA_ITEM.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PEIT_IN_ATIVO = 0;

                // Persiste
                return _baseService.EditItemPesquisa(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item, PESQUISA_ITEM pesq)
        {
            try
            {
                // Verifica ordem
                List<PESQUISA_ITEM_OPCAO> its = pesq.PESQUISA_ITEM_OPCAO.ToList();
                PESQUISA_ITEM_OPCAO pi = its.Where(p => p.PEIO_IN_ORDEM == item.PEIO_IN_ORDEM).FirstOrDefault();
                if (pi != null)
                {
                    return 1;
                }
               
                // Persiste
                return _baseService.EditItemOpcaoPesquisa(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item, PESQUISA_ITEM pesq)
        {
            try
            {
                // Verifica ordem
                List<PESQUISA_ITEM_OPCAO> its = pesq.PESQUISA_ITEM_OPCAO.ToList();
                PESQUISA_ITEM_OPCAO pi = its.Where(p => p.PEIO_IN_ORDEM == item.PEIO_IN_ORDEM).FirstOrDefault();
                if (pi != null)
                {
                    return 1;
                }

                // Completa campos
                item.PEIO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateItemOpcaoPesquisa(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDeleteItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PEIO_IN_ATIVO = 0;

                // Persiste
                return _baseService.EditItemOpcaoPesquisa(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, String, Boolean> ValidateEnviarPesquisa(PESQUISA item, USUARIO usuario)
        {
            try
            {
                // Checa validade
                if (item.GRUP_CD_ID == null & item.CLIE_CD_ID == null)
                {
                    var tupla = Tuple.Create(1, String.Empty, true);
                    return tupla;
                }
                if (item.TEEM_CD_ID == null)
                {
                    var tupla = Tuple.Create(2, String.Empty, true);
                    return tupla;
                }

                // Recupera modelo da mensagem e configuracao
                TEMPLATE_EMAIL teem = _teemService.GetItemById(item.TEEM_CD_ID.Value);
                CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);
                if (teem == null)
                {
                    var tupla = Tuple.Create(11, item.TEEM_CD_ID.Value.ToString(), true);
                    return tupla;
                }

                // Prepara rodape
                String rod = teem.TEEM_TX_DADOS.Replace("{Assinatura}", usuario.ASSINANTE.ASSI_NM_NOME);

                // Prepara corpo do e-mail e trata link
                String corpo = teem.TEEM_TX_CORPO + "<br /><br />";
                StringBuilder str = new StringBuilder();
                str.AppendLine(corpo);
                if (!String.IsNullOrEmpty(item.PESQ_LK_LINK))
                {
                    if (!item.PESQ_LK_LINK.Contains("www."))
                    {
                        item.PESQ_LK_LINK = "www." + item.PESQ_LK_LINK;
                    }
                    if (!item.PESQ_LK_LINK.Contains("http://"))
                    {
                        item.PESQ_LK_LINK = "http://" + item.PESQ_LK_LINK;
                    }
                    str.AppendLine("<a href='" + item.PESQ_LK_LINK + "'>Clique aqui para acessar a pesquisa</a>");
                }
                String body = str.ToString();
                String erro = null;

                // Processa cliente
                if (item.CLIE_CD_ID != null)
                {
                    // Recupera cliente
                    CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                    if (cli == null)
                    {
                        var tupla = Tuple.Create(3, item.CLIE_CD_ID.Value.ToString(), true);
                        return tupla;
                    }

                    // Prepara cabeçalho
                    String cab = teem.TEEM_TX_CABECALHO.Replace("{Nome}", cli.CLIE_NM_NOME);
                    String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

                    // Monta mensagem
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Envio da Pesquisa " + item.PESQ_NM_NOME;
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_TO_DESTINO = cli.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.IS_HTML = true;
                    mensagem.NETWORK_CREDENTIAL = net;

                    // Envia mensagem
                    try
                    {
                        var voltaMail = CrossCutting.CommunicationPackage.SendEmail(mensagem);
                    }
                    catch (Exception ex)
                    {
                        erro = ex.Message;
                    }

                    // Grava envio
                    try
                    {
                        MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                        env.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        env.USUA_CD_ID = usuario.USUA_CD_ID;
                        env.CLIE_CD_ID = cli.CLIE_CD_ID;
                        env.MEEN_IN_TIPO = 1;
                        env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                        env.MEEN_EM_EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
                        env.MEEN_NM_ORIGEM = "Mensagem para Cliente";
                        env.MEEN_TX_CORPO = body;
                        env.MEEN_IN_ANEXOS = 0;
                        env.MEEN_IN_ATIVO = 1;
                        env.MEEN_IN_ESCOPO = 1;
                        env.MEEN_TX_CORPO_COMPLETO = emailBody;
                        if (erro == null)
                        {
                            env.MEEN_IN_ENTREGUE = 1;
                        }
                        else
                        {
                            env.MEEN_IN_ENTREGUE = 0;
                            env.MEEN_TX_RETORNO = erro;
                        }
                        Int32 volta5 = _meService.Create(env);
                    }
                    catch (Exception ex)
                    {
                        var tupla = Tuple.Create(12, ex.Message, true);
                        return tupla;
                    }

                    try
                    {
                        PESQUISA_ENVIO pe = new PESQUISA_ENVIO();
                        pe.PESQ_CD_ID = item.PESQ_CD_ID;
                        pe.CLIE_CD_ID = cli.CLIE_CD_ID;
                        pe.PEEN_DT_ENVIO = DateTime.Now;
                        pe.PEEN_IN_ATIVO = 1;
                        pe.PEEN_IN_RESPONDIDA = 0;
                        Int32 volta1 = _baseService.CreatePesquisaEnvio(pe);
                    }
                    catch (Exception ex)
                    {
                        var tupla = Tuple.Create(13, ex.Message, true);
                        return tupla;
                    }
                }

                // Processa Grupo
                if (item.GRUP_CD_ID != null & item.CLIE_CD_ID == null)
                {
                    // Recupera grupo e clientes
                    GRUPO grupo = _gruService.GetItemById(item.GRUP_CD_ID.Value);
                    if (grupo == null)
                    {
                        var tupla = Tuple.Create(4, item.GRUP_CD_ID.Value.ToString(), true);
                        return tupla;
                    }
                    List<GRUPO_CLIENTE> clientes = grupo.GRUPO_CLIENTE.ToList();
                    if (clientes == null || clientes.Count == 0)
                    {
                        var tupla = Tuple.Create(5, item.GRUP_CD_ID.Value.ToString(), true);
                        return tupla;
                    }

                    // Loop de clientes
                    foreach (GRUPO_CLIENTE grCli in clientes)
                    {
                        // Recupera cliente
                        CLIENTE cli = _cliService.GetItemById(grCli.CLIE_CD_ID);
                        if (cli == null)
                        {
                            var tupla = Tuple.Create(3, grCli.CLIE_CD_ID.ToString(), true);
                            return tupla;
                        }

                        // Prepara cabeçalho
                        String cab = teem.TEEM_TX_CABECALHO.Replace("{Nome}", cli.CLIE_NM_NOME);
                        String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = "Envio da Pesquisa " + item.PESQ_NM_NOME;
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_TO_DESTINO = cli.CLIE_NM_EMAIL;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.IS_HTML = true;
                        mensagem.NETWORK_CREDENTIAL = net;

                        // Envia mensagem
                        try
                        {
                            var voltaMail = CrossCutting.CommunicationPackage.SendEmail(mensagem);
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                        }

                        // Grava envio
                        try
                        {
                            MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                            env.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            env.USUA_CD_ID = usuario.USUA_CD_ID;
                            env.CLIE_CD_ID = cli.CLIE_CD_ID;
                            env.MEEN_IN_TIPO = 1;
                            env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                            env.MEEN_EM_EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
                            env.MEEN_NM_ORIGEM = "Mensagem para Cliente";
                            env.MEEN_TX_CORPO = body;
                            env.MEEN_IN_ANEXOS = 0;
                            env.MEEN_IN_ATIVO = 1;
                            env.MEEN_IN_ESCOPO = 1;
                            env.MEEN_TX_CORPO_COMPLETO = emailBody;
                            if (erro == null)
                            {
                                env.MEEN_IN_ENTREGUE = 1;
                            }
                            else
                            {
                                env.MEEN_IN_ENTREGUE = 0;
                                env.MEEN_TX_RETORNO = erro;
                            }
                            Int32 volta5 = _meService.Create(env);
                        }
                        catch (Exception ex)
                        {
                            var tupla = Tuple.Create(12, ex.Message, true);
                            return tupla;
                        }

                        try
                        {
                            PESQUISA_ENVIO pe = new PESQUISA_ENVIO();
                            pe.PESQ_CD_ID = item.PESQ_CD_ID;
                            pe.CLIE_CD_ID = cli.CLIE_CD_ID;
                            pe.PEEN_DT_ENVIO = DateTime.Now;
                            pe.PEEN_IN_ATIVO = 1;
                            pe.PEEN_IN_RESPONDIDA = 0;
                            Int32 volta1 = _baseService.CreatePesquisaEnvio(pe);
                        }
                        catch (Exception ex)
                        {
                            var tupla = Tuple.Create(13, ex.Message, true);
                            return tupla;
                        }
                    }
                }

                // Retorno OK
                var tuplaVolta = Tuple.Create(0, String.Empty, true);
                return tuplaVolta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePesquisaEnvio(PESQUISA_ENVIO item)
        {
            try
            {
                // Completa campos
                item.PEEN_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreatePesquisaEnvio(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditPesquisaResposta(PESQUISA_RESPOSTA item)
        {
            try
            {
                // Persiste
                return _baseService.EditPesquisaResposta(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePesquisaResposta(PESQUISA_RESPOSTA item)
        {
            try
            {
                // Completa campos
                item.PERE_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreatePesquisaResposta(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
