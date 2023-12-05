using ApplicationServices.Interfaces;
using Azure.Communication.Email;
using CrossCutting;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ERP_Condominios_Solution.Controllers
{
    public class EnvioEMailGeralBase
    {
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IAssinanteAppService assApp;

        public EnvioEMailGeralBase(IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IMensagemEnviadaSistemaAppService meApps)
        {
            usuApp = usuApps;
            confApp = confApps;
            meApp = meApps;
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailGeral(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = usuario.ASSI_CD_ID;

            // Recupera configuracao
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + vm.NOME + "</b><br />";

            // Prepara assinatura
            String rod = String.Empty;
            if (vm.MENS_NM_ASSINATURA == null)
            {
                rod = "<b>" + "Enviado por RidolfiWeb" + "</b>";
            }
            else
            {
                rod = "<b>" + vm.MENS_NM_ASSINATURA + "</b>";
            }

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO;
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui acessar a reunião</a>");
            }
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br />" + body + "<br />" + rod;
            String status = "Succeeded";
            String iD = "xyz";
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = vm.MENS_NM_NOME;
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MENS_NM_CAMPANHA;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.DISPLAY_NAME = vm.NOME;
            mensagem.ConnectionString = conn;

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }

            // Grava envio
            Int32 volta = GravarMensagemEnviada(vm, usuario, body, status, iD, erro, vm.MENS_NM_NOME);
            return 0;
        }

        public async Task<Int32> ProcessaEnvioEMailLista(MensagemViewModel vm, List<MensagemViewModel> lista, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = usuario.ASSI_CD_ID;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            String erro = null;
            Int32 volta = 0;
            Int32 totMens = 0;
            RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();
            List<CLIENTE> nova = new List<CLIENTE>();
            ASSINANTE assi = assApp.GetItemById(idAss);

            // Recupera configuração
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + vm.NOME + "</b>";

            // Prepara rodape
            String rod = "<b>" + "CRMSys" + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui acessar a reunião</a>");
            }
            String status = "Succeeded";
            String iD = "xyz";
            String resposta = String.Empty;

            // Monta corpo
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            body = body.Replace("<p>", "");
            body = body.Replace("</p>", "<br />");
            String emailBody = body;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            try
            {
                // Envio para grupo
                List<EmailAddress> emails = new List<EmailAddress>();
                String data = String.Empty;
                String json = String.Empty;
                List<AttachmentModel> models = new List<AttachmentModel>();
                models = null;

                // Checa se todos tem e-mail e monta lista
                foreach (MensagemViewModel cli in lista)
                {
                    totMens++;
                    EmailAddress add = new EmailAddress(
                            address: cli.MODELO,
                            displayName: cli.NOME);
                    emails.Add(add);
                }

                // Envio
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA;
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = "www@www.com";
                mensagem.NOME_EMISSOR_AZURE = emissor;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;
                mensagem.ConnectionString = conn;

                // Envia mensagem
                try
                {
                    await CrossCutting.CommunicationAzurePackage.SendMailListAsync(mensagem, models, emails);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // Grava mensagem/destino e erros
                foreach (MensagemViewModel item in lista)
                {
                    String guid = new Guid().ToString();
                    Int32 voltax = GravarMensagemEnviada(vm, usuario, body, status, guid, erro, vm.MENS_NM_NOME);                    
                }
                erro = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return totMens;
        }

        public async Task<Int32> ProcessaEnvioEMailGeralReuniao(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = usuario.ASSI_CD_ID;

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui acessar a reunião</a>");
            }
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String status = "Succeeded";
            String iD = "xyz";
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = vm.MENS_NM_NOME;
            mensagem.CORPO = body;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MENS_NM_CAMPANHA;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conn;

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }

            // Grava envio
            Int32 volta = GravarMensagemEnviada(vm, usuario, body, status, iD, erro, vm.MENS_NM_NOME);
            return 0;
        }

        public Int32 GravarMensagemEnviada(MensagemViewModel vm, USUARIO usuario, String emailBody, String status, String iD, String erro, String titulo)
        {
            Int32 idAss = usuario.ASSI_CD_ID;
            Int32 volta = 1;
            if (status == "Succeeded")
            {
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.CLIE_CD_ID = vm.CLIE_CD_ID;
                env.MEEN_IN_USUARIO = vm.MENS_IN_USUARIO;
                env.MEEN_IN_TIPO = 1;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN_EM_EMAIL_DESTINO = vm.MENS_NM_CAMPANHA;
                env.MEEN_NM_ORIGEM = vm.MENS_NM_NOME;
                env.MEEN_TX_CORPO = emailBody;
                env.MEEN_NM_TITULO = titulo;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 2;
                env.MEEN_TX_CORPO_COMPLETO = emailBody;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                if (erro == null)
                {
                    env.MEEN_IN_ENTREGUE = 1;
                }
                else
                {
                    env.MEEN_IN_ENTREGUE = 0;
                    env.MEEN_TX_RETORNO = erro;
                }
                env.MEEN_SG_STATUS = status;
                env.MEEN_GU_ID_MENSAGEM = iD;
                env.EMPR_CD_ID = usuario.EMPR_CD_ID;
                env.MEEN_IN_SISTEMA = 1;
                volta = meApp.ValidateCreate(env);
            }
            return volta;
        }

        public Int32 GravarMensagemEnviadaSMS(MensagemViewModel vm, USUARIO usuario, String smsBody)
        {
            Int32 idAss = usuario.ASSI_CD_ID;
            Int32 volta = 1;
            MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
            env.ASSI_CD_ID = idAss;
            env.USUA_CD_ID = usuario.USUA_CD_ID;
            env.CLIE_CD_ID = vm.CLIE_CD_ID;
            env.MEEN_IN_USUARIO = vm.MENS_IN_USUARIO;
            env.MEEN_IN_TIPO = 2;
            env.MEEN_DT_DATA_ENVIO = DateTime.Now;
            env.MEEN__CELULAR_DESTINO = vm.MENS_NM_CAMPANHA;
            env.MEEN_NM_ORIGEM = vm.MENS_NM_NOME;
            env.MEEN_TX_CORPO = smsBody;
            env.MEEN_IN_ANEXOS = 0;
            env.MEEN_IN_ATIVO = 1;
            env.MEEN_IN_ESCOPO = 2;
            env.MEEN_TX_CORPO_COMPLETO = smsBody;
            env.EMPR_CD_ID = usuario.EMPR_CD_ID;
            env.MEEN_IN_SISTEMA = 1;
            volta = meApp.ValidateCreate(env);
            return volta;
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailFaleConosco(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = usuario.ASSI_CD_ID;

            // Prepara corpo do e-mail e trata link
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String status = "Succeeded";
            String iD = "xyz";
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA;
            mensagem.CORPO = body;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conn;

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }
            return 0;
        }
    }

    public static class SessionMocks
    {
        public static List<ModeloViewModel> CustosFixos { get; set; }
        public static List<ModeloViewModel> ListaProdutoCats { get; set; }
        public static List<ModeloViewModel> ListaProdutoTipo { get; set; }
        public static List<ModeloViewModel> ListaProdutoEspecie { get; set; }
        public static List<ModeloViewModel> ListaProdutoAcima { get; set; }
        public static List<ModeloViewModel> ListaProdutoAbaixo { get; set; }
        public static List<ModeloViewModel> ListaProdutoZerado { get; set; }
        public static List<ModeloViewModel> ListaProdutoEsgota { get; set; }
        public static List<ModeloViewModel> ListaTipoPrec { get; set; }
        public static List<ModeloViewModel> ListaEscopoPrec { get; set; }
        public static List<ModeloViewModel> ListaMetodoPrec { get; set; }
        public static Int32 FlagFornecedor { get; set; }
        public static Int32 FlagProduto { get; set; }
        public static Int32 FlagPrecificacao { get; set; }
    }
}