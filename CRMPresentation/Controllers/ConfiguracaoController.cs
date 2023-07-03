using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution;
using CRMPresentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text;
using System.Net;
using CrossCutting;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class ConfiguracaoController : Controller
    {
        private readonly IConfiguracaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        CONFIGURACAO objeto = new CONFIGURACAO();
        CONFIGURACAO objetoAntes = new CONFIGURACAO();
        List<CONFIGURACAO> listaMaster = new List<CONFIGURACAO>();
        String extensao;

        public ConfiguracaoController(IConfiguracaoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        [HttpPost]
        public JsonResult GetConfiguracao()
        {
            var config = baseApp.GetItemById(2);
            var serialConfig = new CONFIGURACAO
            {
                CONF_CD_ID = config.CONF_CD_ID,
                ASSI_CD_ID = config.ASSI_CD_ID,
                CONF_NR_FALHAS_DIA = config.CONF_NR_FALHAS_DIA,
                CONF_NM_HOST_SMTP = config.CONF_NM_HOST_SMTP,
                CONF_NM_PORTA_SMTP = config.CONF_NM_PORTA_SMTP,
                CONF_NM_EMAIL_EMISSOO = config.CONF_NM_EMAIL_EMISSOO,
                CONF_NM_SENHA_EMISSOR = config.CONF_NM_SENHA_EMISSOR,
                CONF_NR_REFRESH_DASH = config.CONF_NR_REFRESH_DASH,
                CONF_NM_ARQUIVO_ALARME = config.CONF_NM_ARQUIVO_ALARME,
                CONF_NR_REFRESH_NOTIFICACAO = config.CONF_NR_REFRESH_NOTIFICACAO,
                CONF_SG_LOGIN_SMS = config.CONF_SG_LOGIN_SMS,
                CONF_SG_SENHA_SMS = config.CONF_SG_SENHA_SMS,
                CONF_SG_LOGIN_SMS_PRIORITARIO = config.CONF_SG_LOGIN_SMS_PRIORITARIO,
                CONF_SG_SENHA_SMS_PRIORITARIO = config.CONF_SG_SENHA_SMS_PRIORITARIO,
                CONF_NR_DIAS_ACAO = config.CONF_NR_DIAS_ACAO,
                CONF_IN_CNPJ_DUPLICADO = config.CONF_IN_CNPJ_DUPLICADO,
                CONF_IN_ASSINANTE_FILIAL = config.CONF_IN_ASSINANTE_FILIAL,
                CONF_IN_FALHA_IMPORTACAO = config.CONF_IN_FALHA_IMPORTACAO,
                CONF_IN_ETAPAS_CRM = config.CONF_IN_ETAPAS_CRM,
                CONF_IN_NOTIF_ACAO_ADM = config.CONF_IN_NOTIF_ACAO_ADM,
                CONF_IN_NOTIF_ACAO_GER = config.CONF_IN_NOTIF_ACAO_GER,
                CONF_IN_NOTIF_ACAO_VEN = config.CONF_IN_NOTIF_ACAO_VEN,
                CONF_IN_NOTIF_ACAO_OPR = config.CONF_IN_NOTIF_ACAO_OPR,
                CONF_IN_NOTIF_ACAO_USU = config.CONF_IN_NOTIF_ACAO_USU,
                CONF_IN_LOGO_EMPRESA = config.CONF_IN_LOGO_EMPRESA,
                CONF_NR_GRID_CLIENTE = config.CONF_NR_GRID_CLIENTE,
                CONF_NR_GRID_MENSAGEM = config.CONF_NR_GRID_MENSAGEM,
                CONF_NR_GRID_PRODUTO = config.CONF_NR_GRID_PRODUTO,
            };
            return Json(serialConfig);
        }

        [HttpGet]
        public ActionResult MontarTelaConfiguracao()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            objeto = baseApp.GetItemById(idAss);
            Session["Configuracao"] = objeto;

            ViewBag.Listas = (CONFIGURACAO)Session["Configuracao"];
            ViewBag.Title = "Configuracao";
            var listaGrid = new List<SelectListItem>();
            listaGrid.Add(new SelectListItem() { Text = "10", Value = "10" });
            listaGrid.Add(new SelectListItem() { Text = "25", Value = "25" });
            listaGrid.Add(new SelectListItem() { Text = "50", Value = "50" });
            listaGrid.Add(new SelectListItem() { Text = "100", Value = "100" });
            ViewBag.ListaGrid = new SelectList(listaGrid, "Value", "Text");
            var cnpj = new List<SelectListItem>();
            cnpj.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cnpj.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.CNPJ = new SelectList(cnpj, "Value", "Text");
            var assina = new List<SelectListItem>();
            assina.Add(new SelectListItem() { Text = "Assinante", Value = "1" });
            assina.Add(new SelectListItem() { Text = "Filial", Value = "2" });
            ViewBag.Assina = new SelectList(assina, "Value", "Text");
            var logo = new List<SelectListItem>();
            logo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            logo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Logotipo = new SelectList(cnpj, "Value", "Text");

            // Indicadores

            // Mensagem

            // Abre view
            Session["MensConfiguracao"] = 0;
            objetoAntes = objeto;
            if (objeto.CONF_NR_FALHAS_DIA == null)
            {
                objeto.CONF_NR_FALHAS_DIA = 3;
            }
            Session["Configuracao"] = objeto;
            Session["IdConf"] = 1;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(objeto);
            vm.CONF_SG_LOGIN_SMS = CrossCutting.Cryptography.Decrypt(objeto.CONF_SG_LOGIN_SMS_CRIP);
            vm.CONF_SG_SENHA_SMS = CrossCutting.Cryptography.Decrypt(objeto.CONF_SG_SENHA_SMS_CRIP);
            vm.CONF_SG_LOGIN_SMS_PRIORITARIO = CrossCutting.Cryptography.Decrypt(objeto.CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP);
            vm.CONF_SG_SENHA_SMS_PRIORITARIO = CrossCutting.Cryptography.Decrypt(objeto.CONF_SG_SENHA_SMS_PRIORITARIO_CRIP);
            vm.CONF_NM_ENDPOINT_AZURE = CrossCutting.Cryptography.Decrypt(objeto.CONF_NM_ENDPOINT_AZURE_CRIP);
            vm.CONF_NM_EMISSOR_AZURE = CrossCutting.Cryptography.Decrypt(objeto.CONF_NM_EMISSOR_AZURE_CRIP);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaConfiguracao(ConfiguracaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaGrid = new List<SelectListItem>();
            listaGrid.Add(new SelectListItem() { Text = "10", Value = "10" });
            listaGrid.Add(new SelectListItem() { Text = "25", Value = "25" });
            listaGrid.Add(new SelectListItem() { Text = "50", Value = "50" });
            listaGrid.Add(new SelectListItem() { Text = "100", Value = "100" });
            ViewBag.ListaGrid = new SelectList(listaGrid, "Value", "Text");
            var cnpj = new List<SelectListItem>();
            cnpj.Add(new SelectListItem() { Text = "1", Value = "Sim" });
            cnpj.Add(new SelectListItem() { Text = "0", Value = "Não" });
            ViewBag.CNPJ = new SelectList(cnpj, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);

                    // Criptografa chaves
                    vm.CONF_SG_LOGIN_SMS_CRIP = CrossCutting.Cryptography.Encrypt(objeto.CONF_SG_LOGIN_SMS);
                    vm.CONF_SG_SENHA_SMS_CRIP = CrossCutting.Cryptography.Encrypt(objeto.CONF_SG_SENHA_SMS);
                    vm.CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP = CrossCutting.Cryptography.Encrypt(objeto.CONF_SG_LOGIN_SMS_PRIORITARIO);
                    vm.CONF_SG_SENHA_SMS_PRIORITARIO_CRIP = CrossCutting.Cryptography.Encrypt(objeto.CONF_SG_SENHA_SMS_PRIORITARIO);

                    // Grava alteracoes
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["Configuracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Cliente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarBaseConfiguracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaConfiguracao");
        }

        [HttpGet]
        public ActionResult EditarConfiguracao(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONFIGURACAO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Configuracao"] = item;
            Session["IdVolta"] = id;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarConfiguracao(ConfiguracaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Cliente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult CriptoChave()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONFIGURACAO conf = baseApp.GetItemById(usuario.ASSI_CD_ID);

            conf.CONF_SG_LOGIN_SMS_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_LOGIN_SMS);
            conf.CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_LOGIN_SMS_PRIORITARIO);
            conf.CONF_SG_SENHA_SMS_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_SENHA_SMS);
            conf.CONF_SG_SENHA_SMS_PRIORITARIO_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_SENHA_SMS_PRIORITARIO);

            conf.CONF_NM_SENDGRID_APIKEY_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_SENDGRID_APIKEY);
            conf.CONF_NM_SENDGRID_LOGIN_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_SENDGRID_LOGIN);
            conf.CONF_NM_SENDGRID_PWD_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_SENDGRID_PWD);

            conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_CS_CONNECTION_STRING_AZURE);
            conf.CONF_NM_EMISSOR_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_EMISSOR_AZURE);
            conf.CONF_NM_ENDPOINT_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_ENDPOINT_AZURE);
            conf.CONF_NM_KEY_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_KEY_AZURE);

            Int32 volta = baseApp.ValidateEdit(conf);
            return RedirectToAction("MontarTelaConfiguracao", "Configuracao");
        }

    }
}