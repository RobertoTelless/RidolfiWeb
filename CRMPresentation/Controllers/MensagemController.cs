using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;

using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;

using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;

using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;
using System.Net.Mail;
using static iTextSharp.text.pdf.AcroFields;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using CRMPresentation.Controllers;
using DHTMLX.Scheduler.Settings;
using Azure.Communication.Email;
using System.Net.Mime;
using ERP_Condominios_Solution.Classes;


namespace ERP_Condominios_Solution.Controllers
{
    public class MensagemController : ControleCache
    {
        private readonly IMensagemAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IClienteAppService cliApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateSMSAppService temApp;
        private readonly IGrupoAppService gruApp;
        private readonly ITemplateEMailAppService temaApp;
        private readonly IEMailAgendaAppService emApp;
        private readonly ICRMAppService crmApp;
        private readonly IPeriodicidadeAppService periodicidadeApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAssinanteAppService assApp;
        private readonly IRecursividadeAppService recApp;

        private String msg;
        private Exception exception;

        MENSAGENS objeto = new MENSAGENS();
        MENSAGENS objetoAntes = new MENSAGENS();
        List<MENSAGENS> listaMaster = new List<MENSAGENS>();
        List<RECURSIVIDADE> listaMasterRec = new List<RECURSIVIDADE>();
        RECURSIVIDADE objetoRec = new RECURSIVIDADE();
        List<RESULTADO_ROBOT> listaMasterRobot = new List<RESULTADO_ROBOT>();
        RESULTADO_ROBOT objetoRobot = new RESULTADO_ROBOT();
        String extensao;

        public MensagemController(IMensagemAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IClienteAppService cliApps, IConfiguracaoAppService confApps, ITemplateSMSAppService temApps, IGrupoAppService gruApps, ITemplateEMailAppService temaApps, IEMailAgendaAppService emApps, ICRMAppService crmApps, IPeriodicidadeAppService periodicidadeApps, IEmpresaAppService empApps, IAssinanteAppService assApps, IRecursividadeAppService recApps) :
            base(baseApps, temApps, temaApps, gruApps, cliApps, periodicidadeApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            cliApp = cliApps;
            confApp = confApps;
            temApp = temApps;
            gruApp = gruApps;
            temaApp = temaApps;
            emApp = emApps;
            crmApp = crmApps;
            periodicidadeApp = periodicidadeApps;
            empApp = empApps;
            assApp = assApps;
            recApp = recApps;
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

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<Hashtable> listResult = new List<Hashtable>();
            List<CLIENTE> clientes = cliApp.GetAllItens(idAss);
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                clientes = clientes.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }

            if (nome != null)
            {
                List<CLIENTE> lstCliente = clientes.Where(x => x.CLIE_NM_NOME != null && x.CLIE_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = clientes.Where(x => x.CLIE_NM_RAZAO != null).ToList<CLIENTE>();
                    lstCliente = lstCliente.Where(x => x.CLIE_NM_RAZAO.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.CLIE_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.CLIE_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.CLIE_NM_NOME + " (" + item.CLIE_NM_RAZAO + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaMensagemSMS()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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

            // Carrega numeros
            Int32 volta = CarregaNumeroMensagem();

            // Carrega listas
            if (Session["ListaMensagem"] == null)
            {
                listaMaster = CarregaMensagem().Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaMensagem"] = listaMaster;
            }
            ViewBag.Listas = (List<MENSAGENS>)Session["ListaMensagem"];
            Session["Mensagem"] = null;
            Session["IncluirMensagem"] = 0;

            ViewBag.SMSTotalEnvio = (Int32)Session["SMSTotalEnvio"];
            ViewBag.SMSTotalEnvioMes = (Int32)Session["SMSTotalEnvioMes"];
            ViewBag.SMSTotalEnvioDia = (Int32)Session["SMSTotalEnvioDia"];
            ViewBag.SMSAguarda = (Int32)Session["SMSAguarda"];
            ViewBag.SMSFalha = (Int32)Session["SMSFalha"];
            ViewBag.SMSAgenda = (Int32)Session["SMSAgendaNum"];

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0251", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0252", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 60)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0064", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 61)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0258", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            Session["MensMensagem"] = null;
            Session["VoltaRec"] = 2;
            objeto = new MENSAGENS();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult MontarTelaResumoEnvios()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if (Session["ListaEnvios"] == null)
            {
                listaMasterRobot = CarregarEnvios().Where(p => p.RERO_DT_ENVIO.Date == DateTime.Today.Date).OrderByDescending(m => m.RERO_DT_ENVIO).ToList();
                Session["ListaEnvios"] = listaMasterRobot;
            }
            listaMasterRobot = (List<RESULTADO_ROBOT>)Session["ListaEnvios"];
            ViewBag.Listas = listaMasterRobot;

            List<RESULTADO_ROBOT> emails = listaMasterRobot.Where(p => p.RERO_IN_TIPO == 1).ToList();
            List<RESULTADO_ROBOT> sms = listaMasterRobot.Where(p => p.RERO_IN_TIPO == 2).ToList();
            List<RESULTADO_ROBOT> smsDia = sms.Where(p => p.RERO_DT_ENVIO.Date == DateTime.Today.Date).ToList();
            List<RESULTADO_ROBOT> smsMes = sms.Where(p => p.RERO_DT_ENVIO.Month == DateTime.Today.Date.Month & p.RERO_DT_ENVIO.Year == DateTime.Today.Date.Year).ToList();
            List<RESULTADO_ROBOT> emailsDia = emails.Where(p => p.RERO_DT_ENVIO.Date == DateTime.Today.Date).ToList();
            List<RESULTADO_ROBOT> emailsMes = emails.Where(p => p.RERO_DT_ENVIO.Month == DateTime.Today.Date.Month & p.RERO_DT_ENVIO.Year == DateTime.Today.Date.Year).ToList();

            ViewBag.SMSTotalEnvio = sms.Count;
            ViewBag.SMSTotalEnvioMes = smsMes.Count;
            ViewBag.SMSTotalEnvioDia = smsDia.Count;
            ViewBag.EMailTotalEnvio = emails.Count;
            ViewBag.EMailTotalEnvioMes = emailsMes.Count;
            ViewBag.EMailTotalEnvioDia = emailsDia.Count;


            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "SMS", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Sucesso", Value = "1" });
            status.Add(new SelectListItem() { Text = "Falha", Value = "2" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            Session["MensMensagem"] = null;
            objetoRobot = new RESULTADO_ROBOT();
            objetoRobot.RERO_DT_ENVIO = DateTime.Today.Date;
            objetoRobot.RERO_DT_DUMMY = DateTime.Today.Date;
            return View(objetoRobot);
        }

        public ActionResult RetirarFiltroMensagemSMS()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMensagem"] = null;
            Session["FiltroMensagem"] = null;
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult RetirarFiltroEnviosRobot()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaEnvios"] = null;
            return RedirectToAction("MontarTelaResumoEnvios");
        }

        public ActionResult MostrarMesEnviosRobot()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterRobot = baseApp.GetAllEnviosRobot(idAss).Where(p => p.RERO_DT_ENVIO.Month == DateTime.Today.Date.Month & p.RERO_DT_ENVIO.Year == DateTime.Today.Date.Year).OrderByDescending(m => m.RERO_DT_ENVIO).ToList();
            Session["ListaEnvios"] = listaMasterRobot;
            return RedirectToAction("MontarTelaResumoEnvios");
        }

        public ActionResult MostrarEnviosRobot()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterRobot = baseApp.GetAllEnviosRobot(idAss).OrderByDescending(m => m.RERO_DT_ENVIO).ToList();
            Session["ListaEnvios"] = listaMasterRobot;
            return RedirectToAction("MontarTelaResumoEnvios");
        }

        public ActionResult MostrarTudoMensagemSMS()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult MostrarMesesMensagemSMS()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregaMensagem().Where(p => p.MENS_IN_TIPO == 2).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult MostrarMesAtualMensagemSMS()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregaMensagem().Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        [HttpPost]
        public ActionResult FiltrarMensagemSMS(MENSAGENS item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                Session["FiltroMensagem"] = item;
                Tuple<Int32, List<MENSAGENS>, Boolean> volta = baseApp.ExecuteFilterSMS(item.MENS_DT_ENVIO, item.MENS_IN_ATIVO.Value, item.MENS_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaMensagemSMS");
                }

                // Sucesso
                listaMaster = volta.Item2;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaMensagem"] = listaMaster;
                return RedirectToAction("MontarTelaMensagemSMS");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseMensagemSMS()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult VoltarMensagemAnexoSMS()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 volta = (Int32)Session["VoltaMensagem"];
            if (volta == 1)
            {
                return RedirectToAction("MontarTelaMensagemSMS");
            }
            else if (volta == 2)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            else if (volta == 3)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        [HttpGet]
        public ActionResult ExcluirMensagemSMS(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemSMS");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                MENSAGENS item = baseApp.GetItemById(id);
                item.MENS_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                Session["ListaMensagem"] = null;
                Session["MensagemAlterada"] = 1;
                return RedirectToAction("VoltarBaseMensagemSMS");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarMensagemSMS(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemSMS");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                MENSAGENS item = baseApp.GetItemById(id);
                item.MENS_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                Session["ListaMensagem"] = null;
                Session["MensagemAlterada"] = 1;
                return RedirectToAction("VoltarBaseMensagemSMS");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public JsonResult PesquisaTemplateSMS(String temp)
        {
            var hash = new Hashtable();
            if (!string.IsNullOrEmpty(temp))
            {
                // Recupera Template
                TEMPLATE_SMS tmp = temApp.GetItemById(Convert.ToInt32(temp));

                // Atualiza
                hash.Add("TSMS_TX_CORPO", tmp.TSMS_TX_CORPO);
            }

            // Retorna
            return Json(hash);
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemSMS()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemSMS");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            //Verifica possibilidade
            Int32 num = CarregaSMSPadraoDia().Count;
            if ((Int32)Session["NumSMS"] <= num)
            {
                Session["MensSMS"] = 50;
                return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
            }
            Int32 num1 = CarregaSMSPrioritarioDia().Count;
            if ((Int32)Session["NumSMSPrior"] <= num)
            {
                Session["MensSMS"] = 51;
                return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
            }

            // Prepara listas   
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Prioritário", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<CLIENTE> listaTotal = CarregaCliente();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            List<GRUPO> listaTotal1 = CarregaGrupo();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal1 = listaTotal1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            ViewBag.Periodicidades = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(CarregaTemplateSMS().OrderBy(p => p.TSMS_NM_NOME), "TSMS_CD_ID", "TSMS_NM_NOME");

            // Prepara view
            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0250", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 52)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0259", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 62)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0263", CultureInfo.CurrentCulture));
                }
            }

            Session["MensagemNovo"] = 0;
            MENSAGENS item = new MENSAGENS();
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.MENS_DT_CRIACAO = DateTime.Now;
            vm.MENS_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.MENS_IN_TIPO = 2;
            vm.MENS_TX_SMS = null;
            vm.ID = 0;
            vm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
            vm.MENS_IN_REPETICAO = 0;
            vm.MENS_IN_STATUS = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemSMS(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Prioritário", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<CLIENTE> listaTotal = CarregaCliente();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            List<GRUPO> listaTotal1 = CarregaGrupo();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal1 = listaTotal1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            ViewBag.Periodicidades = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(CarregaTemplateSMS().OrderBy(p => p.TSMS_NM_NOME), "TSMS_CD_ID", "TSMS_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Checa preenchimento
                    if (String.IsNullOrEmpty(vm.MENS_TX_SMS) & vm.TSMS_CD_ID == null)
                    {
                        Session["MensMensagem"] = 3;
                        return RedirectToAction("IncluirMensagemSMS");
                    }
                    if (vm.ID == null & vm.GRUP_CD_ID == null)
                    {
                        Session["MensMensagem"] = 52;
                        return RedirectToAction("IncluirMensagemSMS");
                    }
                    if (vm.MENS_DT_AGENDAMENTO != null)
                    {
                        if (vm.MENS_DT_AGENDAMENTO < DateTime.Now.AddMinutes(30))
                        {
                            Session["MensMensagem"] = 62;
                            return RedirectToAction("IncluirMensagemSMS");
                        }
                    }

                    // Verifica possibilidade
                    Int32 num = 0;
                    Int32 numBase = 0;
                    Int32 destino = 0;

                    if (vm.MENS_IN_TIPO_SMS == 1)
                    {
                        num = CarregaSMSPadraoDia().Count;
                    }
                    else
                    {
                        num = CarregaSMSPrioritarioDia().Count;
                    }

                    if (vm.ID > 0)
                    {
                        numBase = num + 1;
                        if (vm.MENS_IN_TIPO_SMS == 1)
                        {
                            if ((Int32)Session["NumSMS"] <= numBase)
                            {
                                Session["MensSMS"] = 50;
                                return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
                            }
                        }
                        else
                        {
                            if ((Int32)Session["NumSMSPrior"] <= numBase)
                            {
                                Session["MensSMS"] = 51;
                                return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
                            }
                        }
                        destino = 1;
                    }
                    else if (vm.GRUP_CD_ID > 0)
                    {
                        GRUPO grupo = gruApp.GetItemById((int)vm.GRUP_CD_ID);
                        Int32 numGrupo = grupo.GRUPO_CLIENTE.Count;
                        numBase = num + numGrupo;
                        if (vm.MENS_IN_TIPO_SMS == 1)
                        {
                            if ((Int32)Session["NumSMS"] <= numBase)
                            {
                                Session["MensSMS"] = 50;
                                return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
                            }
                        }
                        else
                        {
                            if ((Int32)Session["NumSMSPrior"] <= numBase)
                            {
                                Session["MensSMS"] = 51;
                                return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
                            }
                        }
                        destino = numGrupo;
                    }

                    // Prepara a operação
                    MENSAGENS item = Mapper.Map<MensagemViewModel, MENSAGENS>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    item.PESQ_CD_ID = null;
                    item.PESQUISA = null;
                    item.MENS_IN_DESTINOS = destino;
                    item.MENS_IN_STATUS = 1;
                    item.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Processa
                    MENSAGENS mens = baseApp.GetItemById(item.MENS_CD_ID);
                    Session["IdMensagem"] = mens.MENS_CD_ID;
                    vm.MENS_CD_ID = mens.MENS_CD_ID;
                    vm.MENSAGEM_ANEXO = mens.MENSAGEM_ANEXO;
                    Int32 retGrava = ProcessarEnvioMensagemSMS(vm, usuario);

                    // Retornos de erros
                    if (retGrava == 1)
                    {
                        Session["MensMensagem"] = 51;
                        return RedirectToAction("MontarTelaMensagemSMS");
                    }
                    if (retGrava == 2)
                    {
                        Session["MensMensagem"] = 52;
                        return RedirectToAction("IncluirMensagemSMS");
                    }

                    // Sucesso
                    if (vm.MENS_IN_TIPO_SMS == 1)
                    {
                        Session["SMSPadraoAlterada"] = 1;
                    }
                    else
                    {
                        Session["SMSPriorAlterada"] = 1;
                    }

                    // Acerta sessions
                    Int32 totMens = (Int32)Session["TotMens"];
                    Int32 enviado = ((Int32)Session["SMSTotalEnvio"]) + totMens;
                    Int32 enviadoMes = ((Int32)Session["SMSTotalEnvioMes"]) + totMens;
                    Int32 enviadoDia = ((Int32)Session["SMSTotalEnvioDia"]) + totMens;
                    if (mens.MENS_DT_AGENDAMENTO != null)
                    {
                        Int32 agenda = ((Int32)Session["SMSAgendaNum"]) + totMens;
                        Session["SMSAgendaNum"] = agenda;
                    }
                    Session["SMSTotalEnvio"] = enviado;
                    Session["SMSTotalEnvioMes"] = enviadoMes;
                    Session["SMSTotalEnvioDia"] = enviadoDia;

                    listaMaster = new List<MENSAGENS>();
                    Session["ListaMensagem"] = null;
                    Session["MensagemNovo"] = item.MENS_CD_ID;
                    Session["MensagemAlterada"] = 1;
                    return RedirectToAction("MontarTelaMensagemSMS");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();
            List<System.Net.Mail.Attachment> att = new List<System.Net.Mail.Attachment>();
            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }
                att.Add(new System.Net.Mail.Attachment(file.InputStream, f.Name));
                queue.Add(f);
            }
            Session["FileQueueMensagem"] = queue;
            Session["Attachments"] = att;
        }

        [HttpPost]
        public ActionResult UploadFileQueueMensagem(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idNot = (Int32)Session["IdMensagem"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensMensagem"] = 10;
                return RedirectToAction("VoltarBaseMensagemSMS");
            }

            MENSAGENS item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensMensagem"] = 11;
                return RedirectToAction("VoltarBaseMensagemSMS");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                MENSAGEM_ANEXO foto = new MENSAGEM_ANEXO();
                foto.MEAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.MEAN_DT_ANEXO = DateTime.Today;
                foto.MEAN_IN_ATIVO = 1;
                Int32 tipo = 3;
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    tipo = 1;
                }
                else if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
                {
                    tipo = 2;
                }
                else if (extensao.ToUpper() == ".PDF")
                {
                    tipo = 3;
                }
                else if (extensao.ToUpper() == ".MP3" || extensao.ToUpper() == ".MPEG")
                {
                    tipo = 4;
                }
                else if (extensao.ToUpper() == ".DOCX" || extensao.ToUpper() == ".DOC" || extensao.ToUpper() == ".ODT")
                {
                    tipo = 5;
                }
                else if (extensao.ToUpper() == ".XLSX" || extensao.ToUpper() == ".XLS" || extensao.ToUpper() == ".ODS")
                {
                    tipo = 6;
                }
                else
                {
                    tipo = 7;
                }
                foto.MEAN_IN_TIPO = tipo;
                foto.MEAN_NM_TITULO = fileName.Length < 49 ? fileName : fileName.Substring(0, 48);
                foto.MENS_CD_ID = item.MENS_CD_ID;
                foto.MEAN_BN_BINARIO = System.IO.File.ReadAllBytes(path);
                item.MENSAGEM_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);
                return RedirectToAction("VoltarBaseMensagemEMail");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoMensagemSMS(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagens"] = 2;
                    return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
                }
                if ((Int32)Session["PermMens"] == 0)
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

            // Prepara view
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoMensagemSMS()
        {
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("VerMensagemSMS", new { id = (Int32)Session["IdMensagem"] });
        }

        public FileResult DownloadMensagemSMS(Int32 id)
        {
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.MEAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            else if (arquivo.Contains(".docx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            else if (arquivo.Contains(".xlsx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else if (arquivo.Contains(".pptx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            }
            else if (arquivo.Contains(".mp3"))
            {
                contentType = "audio/mpeg";
            }
            else if (arquivo.Contains(".mpeg"))
            {
                contentType = "audio/mpeg";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpGet]
        public ActionResult VerMensagemSMS(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
                }
                if ((Int32)Session["PermMens"] == 0)
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

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
            }
            Session["IdMensagem"] = id;
            Session["VoltaMensagem"] = 1;
            MENSAGENS item = baseApp.GetItemById(id);
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            return View(vm);
        }

        [ValidateInput(false)]
        public Int32 ProcessarEnvioMensagemSMS(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            GRUPO grupo = null;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            Int32 totMens = 0;
            CRMSysDBEntities Db = new CRMSysDBEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);
            Session["Erro"] = null;
            List<CLIENTE> nova = new List<CLIENTE>();
            ASSINANTE assi = assApp.GetItemById(idAss);

            // Verifica possibilidade   
            if (vm.MENS_IN_TIPO_SMS == 1)
            {
                Int32 num = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO.Value == 2 & p.MENS_IN_TIPO_SMS == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    Session["MensMensagem"] = 51;
                    return 1;
                }
            }
            if (vm.MENS_IN_TIPO_SMS == 2)
            {
                Int32 num = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO.Value == 2 & p.MENS_IN_TIPO_SMS == 2 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                if ((Int32)Session["NumSMSPrior"] <= num)
                {
                    Session["MensMensagem"] = 51;
                    return 1;
                }
            }

            // Nomes
            if (vm.ID > 0)
            {
                cliente = cliApp.GetItemById(vm.ID.Value);
                escopo = 1;
            }
            else if (vm.GRUP_CD_ID > 0)
            {
                listaCli = new List<CLIENTE>();
                grupo = gruApp.GetItemById((int)vm.GRUP_CD_ID);
                foreach (GRUPO_CLIENTE item in grupo.GRUPO_CLIENTE)
                {
                    if (item.GRCL_IN_ATIVO == 1)
                    {
                        listaCli.Add(item.CLIENTE);
                    }
                }
                escopo = 2;
            }
            Session["ClienteSMS"] = cliente;
            Session["ListaClienteSMS"] = listaCli;
            Session["Escopo"] = escopo;

            // Monta token
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
            String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);
            String loginPrior = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP);
            String senhaPrior = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_PRIORITARIO_CRIP);

            String text = String.Empty;
            if (vm.MENS_IN_TIPO_SMS == 1)
            {
                text = login + ":" + senha;
            }
            if (vm.MENS_IN_TIPO_SMS == 2)
            {
                text = loginPrior + ":" + senhaPrior;
            }
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = String.Empty;
            String link = String.Empty;
            if (vm.TSMS_CD_ID != null)
            {
                TEMPLATE_SMS temp = temApp.GetItemById(vm.TSMS_CD_ID.Value);
                texto = temp.TSMS_TX_CORPO;
            }
            else
            {
                texto = vm.MENS_TX_SMS;
            }
            link = vm.MENS_NM_LINK;

            // Susbtitui texto
            texto = texto.Replace("{Assinante}", assi.ASSI_NM_NOME);

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(texto);
            if (!String.IsNullOrEmpty(link))
            {
                if (!link.Contains("www."))
                {
                    link = "www." + link;
                }
                if (!link.Contains("http://"))
                {
                    link = "http://" + link;
                }
                str.AppendLine(link + " Clique aqui para maiores informações");
                texto += "  " + link;
            }
            String body = str.ToString();
            body = body.Replace("\r\n", " ");
            String smsBody = body;
            Session["BodySMS"] = body;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            if (escopo == 1)
            {
                try
                {
                    // Susbtitui texto
                    body = body.Replace("{Nome}", cliente.CLIE_NM_NOME);
                    Session["BodySMS"] = body;

                    // Envio
                    if (vm.MENS_DT_AGENDAMENTO == null)
                    {
                        String listaDest = "55" + Regex.Replace(cliente.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                        httpWebRequest.Headers["Authorization"] = auth;
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        String customId = Cryptography.GenerateRandomPassword(8);
                        String data = String.Empty;
                        String json = String.Empty;

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", body, "\", \"customId\": \"" + customId + "\", \"from\": \"CRMSys\"}]}");
                            streamWriter.Write(json);
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            resposta = result;
                            if (resposta.Contains("Fail"))
                            {
                                erro = "Falha de envio do SMS";
                            }
                        }

                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            MENSAGENS_DESTINOS dest1 = new MENSAGENS_DESTINOS();
                            dest1.MEDE_IN_ATIVO = 1;
                            dest1.MEDE_IN_POSICAO = 1;
                            dest1.MEDE_IN_STATUS = 2;
                            dest1.CLIE_CD_ID = cliente.CLIE_CD_ID;
                            dest1.MEDE_DS_ERRO_ENVIO = resposta;
                            dest1.MENS_CD_ID = mens.MENS_CD_ID;
                            dest1.MEDE_IN_CRM = 0;
                            mens.MENSAGENS_DESTINOS.Add(dest1);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_TX_SMS = body;
                            mens.MENS_IN_STATUS = 2;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_IN_STATUS = 3;
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        Session["Erro"] = erro;
                        erro = null;
                    }

                    // Grava status de agendamento
                    if (mens.MENS_DT_AGENDAMENTO != null)
                    {
                        mens.MENS_IN_STATUS = 1;
                        volta = baseApp.ValidateEdit(mens, mens);
                    }

                    if (mens.MENS_NR_REPETICOES != null)
                    {
                        // Grava status
                        mens.MENS_IN_STATUS = 4;
                        volta = baseApp.ValidateEdit(mens, mens);

                        // Monta recursividade
                        RECURSIVIDADE rec = new RECURSIVIDADE();
                        rec.ASSI_CD_ID = idAss;
                        rec.MENS_CD_ID = mens.MENS_CD_ID;
                        rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                        rec.RECU_IN_TIPO_MENSAGEM = 2;
                        if (mens.MENS_DT_AGENDAMENTO == null)
                        {
                            rec.RECU_DT_CRIACAO = DateTime.Now;
                        }
                        else
                        {
                            rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                        }
                        rec.RECU_IN_TIPO_SMS = mens.MENS_IN_TIPO_SMS.Value;
                        rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                        rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                        rec.RECU_TX_TEXTO = (String)Session["BodySMS"];
                        rec.RECU_IN_ATIVO = 1;

                        // Monta destinos
                        CLIENTE cli = (CLIENTE)Session["ClienteSMS"];
                        RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                        dest.CLIE_CD_ID = cli.CLIE_CD_ID;
                        dest.REDE_NR_CELULAR = cli.CLIE_NR_CELULAR;
                        dest.REDE_NM_NOME = cli.CLIE_NM_NOME;
                        dest.REDE_TX_CORPO = (String)Session["BodySMS"];
                        dest.REDE_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DESTINO.Add(dest);

                        // Monta Datas
                        Int32 dias = 0;
                        Int32 numRep = 1;
                        if ((mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0) & mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                        {
                            dias = 0;
                            numRep = 1;
                        }
                        else
                        {
                            if (mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0)
                            {
                                dias = 30;
                            }
                            PERIODICIDADE_TAREFA peri = periodicidadeApp.GetItemById(mens.PETA_CD_ID.Value);
                            if (peri != null)
                            {
                                dias = peri.PETA_NR_DIAS;
                            }

                            if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                            {
                                numRep = 1;
                            }
                            else
                            {
                                numRep = mens.MENS_NR_REPETICOES.Value;
                            }
                        }

                        DateTime datax = DateTime.Now.AddMinutes(30);
                        if (mens.MENS_DT_AGENDAMENTO != null)
                        {
                            datax = mens.MENS_DT_AGENDAMENTO.Value;
                        }
                        for (Int32 i = 1; i <= numRep; i++)
                        {
                            RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                            data1.REDA_DT_PROGRAMADA = datax;
                            data1.REDA_IN_PROCESSADA = 0;
                            data1.REDA_IN_ATIVO = 1;
                            rec.RECURSIVIDADE_DATA.Add(data1);
                            datax = datax.AddDays(dias);
                        }

                        // Grava recursividade
                        Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                    }
                    Session["ListaRecursividade"] = null;
                    Session["TotMens"] = 1;
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }

                return 0;
            }
            else
            {
                try
                {
                    // Configura
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String customBase = Cryptography.GenerateRandomPassword(6);
                    String data = String.Empty;
                    String json = String.Empty;
                    Int32 i = 10;
                    String customId = String.Empty;
                    Session["TotMens"] = 0;

                    // Monta Array de envio
                    if (vm.MENS_DT_AGENDAMENTO == null)
                    {
                        String vetor = String.Empty;
                        foreach (CLIENTE cli in listaCli)
                        {
                            if (cli.CLIE_NR_CELULAR != null)
                            {
                                String bodyA = body.Replace("{Nome}", cli.CLIE_NM_NOME);

                                customId = customBase + i.ToString();
                                i++;
                                String listaDest = "55" + Regex.Replace(cli.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();

                                if (vetor == String.Empty)
                                {
                                    vetor = String.Concat("{\"to\": \"", listaDest, "\", \"text\": \"", bodyA, "\", \"customId\": \"" + customId + "\", \"from\": \"CRMSys\"}");
                                }
                                else
                                {
                                    vetor += String.Concat(",{\"to\": \"", listaDest, "\", \"text\": \"", bodyA, "\", \"customId\": \"" + customId + "\", \"from\": \"CRMSys\"}");
                                }
                                totMens++;
                                nova.Add(cli);
                            }
                        }

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            json = String.Concat("{\"destinations\": [", vetor, "]}");
                            streamWriter.Write(json);
                        }

                        // Envia mensagem
                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            resposta = result;
                            if (resposta.Contains("Fail"))
                            {
                                erro = "Falha de envio do SMS";
                            }
                        }
                        Session["TotMens"] = totMens;

                        // Grava mensagem/destino e erros
                        Session["Erro"] = erro;
                        if (erro == null)
                        {
                            foreach (CLIENTE cli in nova)
                            {
                                MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                dest.MEDE_IN_ATIVO = 1;
                                dest.MEDE_IN_POSICAO = 1;
                                dest.MEDE_IN_STATUS = 2;
                                dest.CLIE_CD_ID = cli.CLIE_CD_ID;
                                dest.MEDE_DS_ERRO_ENVIO = resposta;
                                dest.MENS_CD_ID = mens.MENS_CD_ID;
                                dest.MEDE_IN_CRM = 0;
                                mens.MENSAGENS_DESTINOS.Add(dest);
                            }
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_IN_STATUS = 2;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_IN_STATUS = 3;
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        erro = null;
                    }
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }

                if (mens.MENS_NR_REPETICOES != null || mens.MENS_DT_AGENDAMENTO != null)
                {
                    // Monta registro de recursividade
                    RECURSIVIDADE rec = new RECURSIVIDADE();
                    rec.ASSI_CD_ID = idAss;
                    rec.MENS_CD_ID = mens.MENS_CD_ID;
                    rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                    rec.RECU_IN_TIPO_MENSAGEM = 2;
                    if (mens.MENS_DT_AGENDAMENTO == null)
                    {
                        rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                    }
                    else
                    {
                        rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                    }
                    rec.RECU_IN_TIPO_SMS = mens.MENS_IN_TIPO_SMS.Value;
                    rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                    rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                    rec.RECU_TX_TEXTO = (String)Session["BodySMS"];
                    rec.RECU_IN_ATIVO = 1;

                    // Monta destinos
                    List<CLIENTE> lista = (List<CLIENTE>)Session["ListaClienteSMS"];
                    foreach (CLIENTE cli in lista)
                    {
                        RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                        dest.CLIE_CD_ID = cli.CLIE_CD_ID;
                        dest.REDE_NR_CELULAR = cli.CLIE_NR_CELULAR;
                        dest.REDE_NM_NOME = cli.CLIE_NM_NOME;
                        dest.REDE_TX_CORPO = ((String)Session["BodySMS"]).Replace("{Nome}", cli.CLIE_NM_NOME);
                        dest.REDE_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DESTINO.Add(dest);
                    }

                    // Monta Datas
                    Int32 dias = 0;
                    Int32 numRep = 1;
                    if ((mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0) & mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                    {
                        dias = 0;
                        numRep = 1;
                    }
                    else
                    {
                        if (mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0)
                        {
                            dias = 30;
                        }
                        PERIODICIDADE_TAREFA peri = periodicidadeApp.GetItemById(mens.PETA_CD_ID.Value);
                        if (peri != null)
                        {
                            dias = peri.PETA_NR_DIAS;
                        }

                        if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                        {
                            numRep = 1;
                        }
                        else
                        {
                            numRep = mens.MENS_NR_REPETICOES.Value;
                        }
                    }

                    DateTime datax = DateTime.Now;
                    datax = datax.AddDays(dias);
                    if (mens.MENS_DT_AGENDAMENTO != null)
                    {
                        datax = mens.MENS_DT_AGENDAMENTO.Value;
                    }
                    for (Int32 i = 1; i <= numRep; i++)
                    {
                        RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                        data1.REDA_DT_PROGRAMADA = datax;
                        data1.REDA_IN_PROCESSADA = 0;
                        data1.REDA_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DATA.Add(data1);
                        datax = datax.AddDays(dias);
                    }

                    // Grava recursividade
                    Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                }

                Session["ListaRecursividade"] = null;
                return 0;
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessarEnvioMensagemEMail(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            GRUPO grupo = null;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            Int32 totMens = 0;
            CRMSysDBEntities Db = new CRMSysDBEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);
            Session["Erro"] = null;
            List<CLIENTE> nova = new List<CLIENTE>();
            ASSINANTE assi = assApp.GetItemById(idAss);

            // Verifica possibilidade   
            Int32 num = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO.Value == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
            if ((Int32)Session["NumEMail"] <= num)
            {
                Session["MensMensagem"] = 51;
                return 1;
            }

            // Monta lista de destinatarios
            if (vm.ID > 0)
            {
                cliente = cliApp.GetItemById(vm.ID.Value);
                escopo = 1;
            }
            else if (vm.GRUP_CD_ID > 0)
            {
                listaCli = new List<CLIENTE>();
                grupo = gruApp.GetItemById((int)vm.GRUP_CD_ID);
                foreach (GRUPO_CLIENTE item in grupo.GRUPO_CLIENTE)
                {
                    if (item.GRCL_IN_ATIVO == 1)
                    {
                        listaCli.Add(item.CLIENTE);
                    }
                }
                escopo = 2;
            }
            Session["ClienteEMail"] = cliente;
            Session["ListaClienteEMail"] = listaCli;
            Session["Escopo"] = escopo;

            // Monta token
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara texto
            String texto = String.Empty;
            String link = vm.MENS_NM_LINK;
            if (vm.MENS_AQ_ARQUIVO == null)
            {
                if (vm.TEEM_CD_ID != null)
                {
                    TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                    texto = temp.TEEM_TX_CORPO;
                }
                else
                {
                    texto = vm.MENS_TX_TEXTO;
                }
            }
            else
            {
                texto = System.IO.File.ReadAllText(vm.MENS_AQ_ARQUIVO);
            }

            // Susbtitui texto
            texto = texto.Replace("{Assinante}", assi.ASSI_NM_NOME);

            // Prepara  link
            StringBuilder str = new StringBuilder();
            str.AppendLine(texto);
            if (!String.IsNullOrEmpty(link))
            {
                if (!link.Contains("www."))
                {
                    link = "www." + link;
                }
                if (!link.Contains("http://"))
                {
                    link = "http://" + link;
                }
                str.AppendLine(link + " Clique aqui para maiores informações");
                texto += "  " + link;
            }
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            body = body.Replace("<p>", "");
            body = body.Replace("</p>", "<br />");
            String emailBody = body;
            Session["BodyEMail"] = body;

            // Trata anexos
            List<MENSAGEM_ANEXO> anexos = mens.MENSAGEM_ANEXO.ToList();
            List<AttachmentModel> models = new List<AttachmentModel>();
            if (anexos.Count > 0)
            {
                String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + mens.MENS_CD_ID.ToString() + "/Anexos/";
                foreach (MENSAGEM_ANEXO anexo in anexos)
                {
                    String path = Path.Combine(Server.MapPath(caminho), anexo.MEAN_NM_TITULO);

                    AttachmentModel model = new AttachmentModel();
                    model.PATH = path;
                    model.ATTACHMENT_NAME = anexo.MEAN_NM_TITULO;
                    if (anexo.MEAN_IN_TIPO == 1)
                    {
                        model.CONTENT_TYPE = MediaTypeNames.Image.Jpeg;
                    }
                    if (anexo.MEAN_IN_TIPO == 3)
                    {
                        model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
                    }
                    models.Add(model);
                }
            }
            else
            {
                models = null;
            }

            // inicia processo
            String resposta = String.Empty;
            String status = "Succeeded";
            String iD = "xyz";

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Processa mensagens
            if (escopo == 1)
            {
                // Envio unico
                try
                {
                    // Susbtitui texto
                    body = body.Replace("{Nome}", cliente.CLIE_NM_NOME);
                    Session["BodyEMail"] = body;

                    // Envio
                    if (mens.MENS_DT_AGENDAMENTO == null)
                    {
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                        EmailAzure mensagem = new EmailAzure();
                        mensagem.ASSUNTO = mens.MENS_NM_NOME;
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_TO_DESTINO = cliente.CLIE_NM_EMAIL;
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
                            Tuple<EmailSendStatus, String, Boolean> voltaMail = CrossCutting.CommunicationAzurePackage.SendMail(mensagem, models);
                            status = voltaMail.Item1.ToString();
                            iD = voltaMail.Item2;
                            Session["IdMail"] = iD;
                            Session["StatusMail"] = status;
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            LogError(ex.Message);
                            ViewBag.Message = ex.Message;
                            Session["TipoVolta"] = 2;
                            Session["VoltaExcecao"] = "Mensagens";
                            Session["Excecao"] = ex;
                            Session["ExcecaoTipo"] = ex.GetType().ToString();
                            GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                            Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                            throw;
                        }

                        // Grava mensagem/destino e erros
                        if (status == "Succeeded")
                        {
                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = status;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            dest.MEDE_SG_STATUS = status;
                            dest.MEDE_GU_ID_MENSAGEM = iD;
                            dest.MEDE_IN_CRM = 0;
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_TX_TEXTO = body;
                            mens.MENS_IN_STATUS = 2;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            mens.MENS_IN_STATUS = 3;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        Session["Erro"] = erro;
                        erro = null;
                        Session["TotMens"] = 1;
                    }

                    // Grava status de agendamento
                    if (mens.MENS_DT_AGENDAMENTO != null)
                    {
                        mens.MENS_IN_STATUS = 1;
                        volta = baseApp.ValidateEdit(mens, mens);
                    }

                    // Monta registro de recursividade
                    if (mens.MENS_NR_REPETICOES != null)
                    {
                        // Grava status
                        mens.MENS_IN_STATUS = 4;
                        volta = baseApp.ValidateEdit(mens, mens);

                        // Monta recursividade
                        RECURSIVIDADE rec = new RECURSIVIDADE();
                        rec.ASSI_CD_ID = idAss;
                        rec.MENS_CD_ID = mens.MENS_CD_ID;
                        rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                        rec.RECU_IN_TIPO_MENSAGEM = 1;
                        if (mens.MENS_DT_AGENDAMENTO == null)
                        {
                            rec.RECU_DT_CRIACAO = DateTime.Now;
                        }
                        else
                        {
                            rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                        }
                        rec.RECU_IN_TIPO_SMS = 0;
                        rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                        rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                        rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                        rec.RECU_IN_ATIVO = 1;

                        // Monta destinos
                        CLIENTE cli = (CLIENTE)Session["ClienteEMail"];
                        RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                        dest.CLIE_CD_ID = cli.CLIE_CD_ID;
                        dest.REDE_EM_EMAIL = cli.CLIE_NM_EMAIL;
                        dest.REDE_NM_NOME = cli.CLIE_NM_NOME;
                        dest.REDE_TX_CORPO = ((String)Session["BodyEMail"]).Replace("{Nome}", cli.CLIE_NM_NOME);
                        dest.REDE_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DESTINO.Add(dest);

                        // Monta Datas
                        Int32 dias = 0;
                        Int32 numRep = 1;
                        if ((mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0) & mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                        {
                            dias = 0;
                            numRep = 1;
                        }
                        else
                        {
                            if (mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0)
                            {
                                dias = 30;
                            }
                            PERIODICIDADE_TAREFA peri = periodicidadeApp.GetItemById(mens.PETA_CD_ID.Value);
                            if (peri != null)
                            {
                                dias = peri.PETA_NR_DIAS;
                            }

                            if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                            {
                                numRep = 1;
                            }
                            else
                            {
                                numRep = mens.MENS_NR_REPETICOES.Value;
                            }
                        }

                        DateTime datax = DateTime.Now.AddMinutes(30);
                        if (mens.MENS_DT_AGENDAMENTO != null)
                        {
                            datax = mens.MENS_DT_AGENDAMENTO.Value;
                        }
                        for (Int32 i = 1; i <= numRep; i++)
                        {
                            RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                            data1.REDA_DT_PROGRAMADA = datax;
                            data1.REDA_IN_PROCESSADA = 0;
                            data1.REDA_IN_ATIVO = 1;
                            rec.RECURSIVIDADE_DATA.Add(data1);
                            datax = datax.AddDays(dias);
                        }

                        // Grava recursividade
                        Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                    }
                    Session["ListaRecursividade"] = null;
                    Session["TotMens"] = 1;
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }
                return 0;
            }
            else
            {
                try
                {
                    // Envio para grupo
                    List<EmailAddress> emails = new List<EmailAddress>();
                    String data = String.Empty;
                    String json = String.Empty;

                    // Checa se todos tem e-mail e monta lista
                    foreach (CLIENTE cli in listaCli)
                    {
                        if (cli.CLIE_NM_EMAIL != null)
                        {
                            String bodyA = body.Replace("{Nome}", cli.CLIE_NM_NOME);
                            totMens++;
                            nova.Add(cli);

                            EmailAddress add = new EmailAddress(
                                    address: cli.CLIE_NM_EMAIL,
                                    displayName: cli.CLIE_NM_NOME);
                            emails.Add(add);
                        }
                    }

                    // Envio
                    if (mens.MENS_DT_AGENDAMENTO == null)
                    {
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                        EmailAzure mensagem = new EmailAzure();
                        mensagem.ASSUNTO = mens.MENS_NM_NOME;
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_TO_DESTINO = "www@www.com";
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
                            Tuple<EmailSendStatus, String, Boolean> voltaMail = CrossCutting.CommunicationAzurePackage.SendMailList(mensagem, models, emails);
                            status = voltaMail.Item1.ToString();
                            iD = voltaMail.Item2;
                            Session["IdMail"] = iD;
                            Session["StatusMail"] = status;
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            LogError(ex.Message);
                            ViewBag.Message = ex.Message;
                            Session["TipoVolta"] = 2;
                            Session["VoltaExcecao"] = "Mensagens";
                            Session["Excecao"] = ex;
                            Session["ExcecaoTipo"] = ex.GetType().ToString();
                            GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                            Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                            throw;
                        }
                        Session["TotMens"] = totMens;

                        // Grava mensagem/destino e erros
                        Session["Erro"] = erro;
                        if (status == "Succeeded")
                        {
                            foreach (CLIENTE cli in nova)
                            {
                                MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                dest.MEDE_IN_ATIVO = 1;
                                dest.MEDE_IN_POSICAO = 1;
                                dest.MEDE_IN_STATUS = 1;
                                dest.CLIE_CD_ID = cli.CLIE_CD_ID;
                                dest.MEDE_DS_ERRO_ENVIO = resposta;
                                dest.MENS_CD_ID = mens.MENS_CD_ID;
                                dest.MEDE_SG_STATUS = status;
                                dest.MEDE_GU_ID_MENSAGEM = iD;
                                dest.MEDE_IN_CRM = 0;
                                mens.MENSAGENS_DESTINOS.Add(dest);
                                mens.MENSAGENS_DESTINOS.Add(dest);
                                mens.MENS_DT_ENVIO = DateTime.Now;
                                mens.MENS_IN_STATUS = 2;
                                volta = baseApp.ValidateEdit(mens, mens);
                            }
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            mens.MENS_IN_STATUS = 3;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        erro = null;
                    }
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }

                if (mens.MENS_NR_REPETICOES != null || mens.MENS_DT_AGENDAMENTO != null)
                {
                    // Monta registro de recursividade
                    RECURSIVIDADE rec = new RECURSIVIDADE();
                    rec.ASSI_CD_ID = idAss;
                    rec.MENS_CD_ID = mens.MENS_CD_ID;
                    rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                    rec.RECU_IN_TIPO_MENSAGEM = 1;
                    if (mens.MENS_DT_AGENDAMENTO == null)
                    {
                        rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                    }
                    else
                    {
                        rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                    }
                    rec.RECU_IN_TIPO_SMS = 0;
                    rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                    rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                    rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                    rec.RECU_IN_ATIVO = 1;

                    // Monta destinos
                    List<CLIENTE> lista = (List<CLIENTE>)Session["ListaClienteEMail"];
                    foreach (CLIENTE cli in lista)
                    {
                        RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                        dest.CLIE_CD_ID = cli.CLIE_CD_ID;
                        dest.REDE_EM_EMAIL = cli.CLIE_NM_EMAIL;
                        dest.REDE_NM_NOME = cli.CLIE_NM_NOME;
                        dest.REDE_TX_CORPO = ((String)Session["BodyEMail"]).Replace("{Nome}", cli.CLIE_NM_NOME);
                        dest.REDE_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DESTINO.Add(dest);
                    }

                    Int32 dias = 0;
                    Int32 numRep = 1;
                    if ((mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0) & mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                    {
                        dias = 0;
                        numRep = 1;
                    }
                    else
                    {
                        if (mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0)
                        {
                            dias = 30;
                        }
                        PERIODICIDADE_TAREFA peri = periodicidadeApp.GetItemById(mens.PETA_CD_ID.Value);
                        if (peri != null)
                        {
                            dias = peri.PETA_NR_DIAS;
                        }

                        if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                        {
                            numRep = 1;
                        }
                        else
                        {
                            numRep = mens.MENS_NR_REPETICOES.Value;
                        }
                    }

                    DateTime datax = DateTime.Now;
                    datax = datax.AddDays(dias);
                    if (mens.MENS_DT_AGENDAMENTO != null)
                    {
                        datax = mens.MENS_DT_AGENDAMENTO.Value;
                    }
                    for (Int32 i = 1; i <= numRep; i++)
                    {
                        RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                        data1.REDA_DT_PROGRAMADA = datax;
                        data1.REDA_IN_PROCESSADA = 0;
                        data1.REDA_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DATA.Add(data1);
                        datax = datax.AddDays(dias);
                    }

                    // Grava recursividade
                    Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                    Session["TotMens"] = totMens;
                    Session["ListaRecursividade"] = null;
                }
                return 0;
            }
        }

        [HttpGet]
        public ActionResult ConverterMensagemCRMSMS(Int32 id)
        {
            // Recupera Mensagem e contato
            MENSAGENS_DESTINOS dest = baseApp.GetDestinoById(id);
            MENSAGENS mensagem = baseApp.GetItemById(dest.MENS_CD_ID);
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            if (dest.MEDE_IN_CRM == 0 || dest.MEDE_IN_CRM == null)
            {
                Int32 num = crmApp.GetAllItens(idAss).Count;
                if ((Int32)Session["NumProc"] <= num)
                {
                    Session["MensMensagem"] = 50;
                    return RedirectToAction("VoltarAnexoMensagemSMS");
                }
            }
            else
            {
                Session["MensMensagem"] = 60;
                return RedirectToAction("VoltarAnexoMensagemSMS");
            }
            if (dest.CLIE_CD_ID == null)
            {
                Session["MensMensagem"] = 80;
                return RedirectToAction("VoltarAnexoMensagemEMail");
            }

            // Recupera cliente
            CLIENTE cli = cliApp.GetItemById(dest.CLIE_CD_ID.Value);

            // Cria CRM
            CRM crm = new CRM();
            crm.ASSI_CD_ID = mensagem.ASSI_CD_ID;
            crm.CLIE_CD_ID = dest.CLIE_CD_ID.Value;
            crm.CRM1_DS_DESCRICAO = "Processo criado a partir de SMS";
            crm.CRM1_DT_CRIACAO = DateTime.Today.Date;
            crm.CRM1_IN_ATIVO = 1;
            crm.CRM1_IN_STATUS = 1;
            crm.CRM1_NM_NOME = "Processo criado a partir de SMS";
            crm.TICR_CD_ID = 1;
            crm.USUA_CD_ID = usuario.USUA_CD_ID;
            crm.MENS_CD_ID = mensagem.MENS_CD_ID;
            crm.ORIG_CD_ID = 1;
            crm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
            Int32 volta = crmApp.ValidateCreate(crm, usuario);

            // Atualiza destino
            dest.MEDE_IN_CRM = 1;
            Int32 volta1 = baseApp.ValidateEditDestino(dest);

            // Atualiza mensagem
            mensagem.MENS_IN_CRM = 1;
            Int32 volta2 = baseApp.ValidateEdit(mensagem, mensagem);

            // Retorno
            Session["MensMensagem"] = 40;
            Session["MensagemAlterada"] = 1;
            return RedirectToAction("VoltarAnexoMensagemSMS");
        }

        [HttpGet]
        public ActionResult ConverterMensagemCRMEMail(Int32 id)
        {         
            // Recupera Mensagem e contato
            MENSAGENS_DESTINOS dest = baseApp.GetDestinoById(id);
            MENSAGENS mensagem = baseApp.GetItemById(dest.MENS_CD_ID);
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            if (dest.MEDE_IN_CRM == 0 || dest.MEDE_IN_CRM == null)
            {
                Int32 num = crmApp.GetAllItens(idAss).Count;
                if ((Int32)Session["NumProcessos"] <= num)
                {
                    Session["MensMensagem"] = 50;
                    return RedirectToAction("VoltarAnexoMensagemEMail");
                }
            }
            else
            {
                Session["MensMensagem"] = 60;
                return RedirectToAction("VoltarAnexoMensagemEMail");
            }
            if (dest.CLIE_CD_ID == null)
            {
                Session["MensMensagem"] = 80;
                return RedirectToAction("VoltarAnexoMensagemEMail");
            }

            // Recupera cliente
            CLIENTE cli = cliApp.GetItemById(dest.CLIE_CD_ID.Value);

            // Cria CRM
            Session["CliCRM"] = cli.CLIE_NM_NOME;
            CRM crm = new CRM();
            crm.ASSI_CD_ID = mensagem.ASSI_CD_ID;
            crm.CLIE_CD_ID = dest.CLIE_CD_ID.Value;
            crm.CRM1_DS_DESCRICAO = "Processo criado a partir de E-Mail";
            crm.CRM1_DT_CRIACAO = DateTime.Today.Date;
            crm.CRM1_IN_ATIVO = 1;
            crm.CRM1_IN_STATUS = 1;
            crm.CRM1_NM_NOME = "Processo criado a partir de E-Mail";
            crm.TICR_CD_ID = 1;
            crm.USUA_CD_ID = usuario.USUA_CD_ID;
            crm.MENS_CD_ID = mensagem.MENS_CD_ID;
            crm.ORIG_CD_ID = 1;
            crm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
            Int32 volta = crmApp.ValidateCreate(crm, usuario);

            // Atualiza destino
            dest.MEDE_IN_CRM = 1;
            Int32 volta1 = baseApp.ValidateEditDestino(dest);

            // Atualiza mensagem
            mensagem.MENS_IN_CRM = 1;
            Int32 volta2 = baseApp.ValidateEdit(mensagem, mensagem);

            // Retorno
            Session["MensMensagem"] = 40;
            Session["MensagemAlterada"] = 1;
            return RedirectToAction("VoltarAnexoMensagemEMail");
        }

        [HttpGet]
        public ActionResult MontarTelaMensagemEMail()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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

            // Carrega numeros
            Int32 volta = CarregaNumeroMensagem();

            // Carrega listas
            if (Session["ListaMensagemEMail"] == null)
            {
                listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaMensagemEMail"] = listaMaster;
            }
            ViewBag.Listas = (List<MENSAGENS>)Session["ListaMensagemEMail"];
            Session["Mensagem"] = null;
            Session["IncluirMensagem"] = 0;

            ViewBag.EMailTotalEnvio = (Int32)Session["EMailTotalEnvio"];
            ViewBag.EMailTotalEnvioMes = (Int32)Session["EMailTotalEnvioMes"];
            ViewBag.EMailTotalEnvioDia = (Int32)Session["EMailTotalEnvioDia"];
            ViewBag.EMailAguarda = (Int32)Session["EMailAguarda"];
            ViewBag.EMailFalha = (Int32)Session["EMailFalha"];
            ViewBag.EMailAgenda = (Int32)Session["EMailAgendaNum"];

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture);
                    frase += " - " + (String)Session["CliCRM"];                  
                    ModelState.AddModelError("", frase);              
                }
                if ((Int32)Session["MensMensagem"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0260", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0252", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 60)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 61)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0258", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 80)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0269", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            Session["MensMensagem"] = null;
            Session["VoltaRec"] = 1;
            objeto = new MENSAGENS();
            return View(objeto);
        }

        public ActionResult RetirarFiltroMensagemEMail()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMensagemEMail"] = null;
            Session["FiltroMensagem"] = null;
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult MostrarMesAtualMensagemEMail()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaMensagemEMail"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult MostrarTudoMensagemEMail()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult MostrarMesesMensagemEMail()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaMensagemEMail"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        [HttpPost]
        public ActionResult FiltrarMensagemEMail(MENSAGENS item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<MENSAGENS> listaObj = new List<MENSAGENS>();
                Session["FiltroMensagem"] = item;
                Tuple<Int32, List<MENSAGENS>, Boolean> volta = baseApp.ExecuteFilterEMail(item.MENS_DT_ENVIO, item.MENS_IN_ATIVO.Value, item.MENS_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaMensagemEmail");
                }

                // Sucesso
                listaMaster = volta.Item2;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaMensagemEMail"] = listaMaster;
                return RedirectToAction("MontarTelaMensagemEmail");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarEnviosRobot(RESULTADO_ROBOT item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<RESULTADO_ROBOT> listaObj = new List<RESULTADO_ROBOT>();
                Tuple<Int32, List<RESULTADO_ROBOT>, Boolean> volta = baseApp.ExecuteFilterRobot(item.RERO_IN_TIPO, item.RERO_DT_ENVIO, item.RERO_DT_DUMMY, item.RERO_NM_BUSCA, item.RERO_NM_EMAIL, item.RERO_NR_CELULAR, item.RERO_IN_STATUS, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaResumoEnvios");
                }

                // Sucesso
                listaMasterRobot = volta.Item2;
                Session["ListaEnvios"] = volta.Item2;
                return RedirectToAction("MontarTelaResumoEnvios");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseMensagemEMail()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult VoltarRecursivo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaRec"] == 1)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult VoltarAnexoMensagemEMail()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 volta = (Int32)Session["VoltaMensagem"];
            if (volta == 1)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            else if (volta == 2)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            else if (volta == 3)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        [HttpGet]
        public ActionResult ExcluirMensagemEMail(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemEMail");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                MENSAGENS item = baseApp.GetItemById(id);
                item.MENS_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                Session["ListaMensagem"] = null;
                Session["MensagemAlterada"] = 1;
                return RedirectToAction("VoltarBaseMensagemEMail");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarMensagemEMail(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemEMail");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                MENSAGENS item = baseApp.GetItemById(id);
                item.MENS_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                Session["ListaMensagem"] = null;
                Session["MensagemAlterada"] = 1;
                return RedirectToAction("VoltarBaseMensagemEMail");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public JsonResult PesquisaTemplateEMail(String temp)
        {
            var hash = new Hashtable();
            if (!string.IsNullOrEmpty(temp))
            {
                // Recupera Template
                TEMPLATE_EMAIL tmp = temaApp.GetItemById(Convert.ToInt32(temp));

                // Atualiza
                hash.Add("TEEM_TX_CORPO", tmp.TEEM_TX_CORPO);
            }

            // Retorna
            return Json(hash);
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemEMail()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemEMail");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = CarregaMensagem().Where(p => p.MENS_IN_TIPO.Value == 1 & p.MENS_DT_CRIACAO.Value == DateTime.Today.Date).ToList().Count;
            if ((Int32)Session["NumEMail"] <= num)
            {
                Session["MensMensagem"] = 51;
                return RedirectToAction("VoltarBaseMensagemEMail");
            }

            // Prepara listas   
            List<CLIENTE> listaTotal = CarregaCliente();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            List<GRUPO> listaTotal1 = CarregaGrupo();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal1 = listaTotal1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(CarregaTemplateEMail().Where(p => p.TEEM_IN_HTML == 1).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
            ViewBag.Usuario = GetUsuarioLogado();
            ViewBag.Modelos = new SelectList(CarregarModelosHtml(), "Value", "Text");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");

            // Prepara view
            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0250", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 52)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0259", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 72)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0263", CultureInfo.CurrentCulture));
                }
            }

            Session["MensagemNovo"] = 0;
            MENSAGENS item = new MENSAGENS();
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.MENS_DT_CRIACAO = DateTime.Now;
            vm.MENS_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.MENS_IN_TIPO = 1;
            vm.MENS_TX_TEXTO = null;
            vm.ID = 0;
            vm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
            vm.MENS_IN_REPETICAO = 0;
            vm.MENS_IN_STATUS = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemEMail(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<CLIENTE> listaTotal = CarregaCliente();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            List<GRUPO> listaTotal1 = CarregaGrupo();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal1 = listaTotal1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(CarregaTemplateEMail().Where(p => p.TEEM_IN_HTML == 1).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
            ViewBag.Usuario = GetUsuarioLogado();
            ViewBag.Modelos = new SelectList(CarregarModelosHtml(), "Value", "Text");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Checa preenchimento
                    if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO) & vm.TEEM_CD_ID == null & vm.MENS_AQ_ARQUIVO == null)
                    {
                        Session["MensMensagem"] = 3;
                        return RedirectToAction("IncluirMensagemEmail");
                    }
                    if (vm.ID == null & vm.GRUP_CD_ID == null)
                    {
                        Session["MensMensagem"] = 52;
                        return RedirectToAction("IncluirMensagemEmail");
                    }

                    // Checa agendamento
                    if (vm.MENS_DT_AGENDAMENTO != null)
                    {
                        if (vm.MENS_DT_AGENDAMENTO < DateTime.Today.Date.AddMinutes(30))
                        {
                            Session["MensMensagem"] = 72;
                            return RedirectToAction("IncluirMensagemEmail");
                        }
                    }

                    // Verifica possibilidade
                    Int32 numBase = 0;
                    Int32 num = CarregaEMailDia().Count;
                    Int32 destinos = 0;

                    if (vm.ID > 0)
                    {
                        numBase = num + 1;
                        if ((Int32)Session["NumEMail"] <= numBase)
                        {
                            Session["MensEMail"] = 50;
                            return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
                        }
                        destinos = 1;
                    }
                    else if (vm.GRUP_CD_ID > 0)
                    {
                        GRUPO grupo = gruApp.GetItemById((int)vm.GRUP_CD_ID);
                        Int32 numGrupo = grupo.GRUPO_CLIENTE.Count;
                        numBase = num + numGrupo;
                        if ((Int32)Session["NumSMS"] <= numBase)
                        {
                            Session["MensSMS"] = 50;
                            return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
                        }
                        destinos = numGrupo;
                    }

                    // Prepara a operação
                    MENSAGENS item = Mapper.Map<MensagemViewModel, MENSAGENS>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    item.PESQ_CD_ID = null;
                    item.PESQUISA = null;
                    item.MENS_IN_DESTINOS = destinos;
                    item.MENS_IN_STATUS = 1;
                    item.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);
                    Session["IdMensagem"] = item.MENS_CD_ID;

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Trata anexos
                    if (Session["FileQueueMensagem"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueMensagem"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueMensagem(file);
                            }
                        }
                        Session["FileQueueMensagem"] = null;
                    }

                    // Processa
                    MENSAGENS mens = baseApp.GetItemById(item.MENS_CD_ID);
                    Session["IdMensagem"] = mens.MENS_CD_ID;
                    vm.MENS_CD_ID = mens.MENS_CD_ID;
                    vm.MENSAGEM_ANEXO = mens.MENSAGEM_ANEXO;
                    Int32 retGrava = ProcessarEnvioMensagemEMail(vm, usuario);

                    // Retornos de erros
                    if (retGrava == 1)
                    {
                        Session["MensMensagem"] = 51;
                        return RedirectToAction("MontarTelaMensagemEMail");
                    }

                    // Sucesso
                    Session["EMailPadraoAlterada"] = 1;

                    // Acerta sessions
                    Int32 totMens = (Int32)Session["TotMens"];
                    Int32 enviado = ((Int32)Session["EMailTotalEnvio"]) + totMens;
                    Int32 enviadoMes = ((Int32)Session["EMailTotalEnvioMes"]) + totMens;
                    Int32 enviadoDia = ((Int32)Session["EMailTotalEnvioDia"]) + totMens;
                    if (mens.MENS_DT_AGENDAMENTO != null)
                    {
                        Int32 agenda = ((Int32)Session["EMailAgendaNum"]) + totMens;
                        Session["EMailAgendaNum"] = agenda;
                    }
                    Session["EMailTotalEnvio"] = enviado;
                    Session["EMailTotalEnvioMes"] = enviadoMes;
                    Session["EMailTotalEnvioDia"] = enviadoDia;

                    listaMaster = new List<MENSAGENS>();
                    Session["ListaMensagemEMail"] = null;
                    Session["MensagemNovo"] = item.MENS_CD_ID;

                    Session["ListaMensagemEMail"] = null;
                    Session["MensagemAlterada"] = 1;
                    return RedirectToAction("MontarTelaMensagemEMail");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerEMailAgendados()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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

            List<MENSAGENS> agSMS = CarregarMensagem().Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_TIPO == 1).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                agSMS = agSMS.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            agSMS = agSMS.Where(p => p.MENS_DT_AGENDAMENTO > DateTime.Now).ToList();
            Session["EMailAgenda"] = agSMS;

            MENSAGENS mens = new MENSAGENS();
            ViewBag.Listas = (List<MENSAGENS>)Session["EMailAgenda"];
            Session["VoltaMensagem"] = 3;
            return View(mens);
        }

        [HttpGet]
        public ActionResult VerSMSAgendados()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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


            List<MENSAGENS> agSMS = CarregarMensagem().Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_TIPO == 2).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                agSMS = agSMS.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            agSMS = agSMS.Where(p => p.MENS_DT_AGENDAMENTO > DateTime.Now).ToList();
            Session["SMSAgenda"] = agSMS;

            MENSAGENS mens = new MENSAGENS();
            ViewBag.Listas = (List<MENSAGENS>)Session["SMSAgenda"];
            Session["VoltaMensagem"] = 3;
            return View(mens);
        }

        public static async Task SendEmailAssync(Email email)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
                mensagem.To.Add(email.EMAIL_TO_DESTINO);
                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                await smtp.SendMailAsync(mensagem);
            }
            catch (Exception ex)
            {
            }
        }

        public string SendEmail(Email email)
        {
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            
            var tasks = new List<Task>();
            var smtp = new SmtpClient();
            smtp.EnableSsl = email.ENABLE_SSL;
            smtp.Port = Convert.ToInt32(email.PORTA);
            smtp.Host = email.SMTP;
            smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
            smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);

            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
            mensagem.To.Add(email.EMAIL_TO_DESTINO);
            mensagem.Subject = email.ASSUNTO;
            mensagem.IsBodyHtml = true;
            mensagem.Body = email.CORPO;
            mensagem.Priority = email.PRIORIDADE;
            mensagem.IsBodyHtml = true;
            if (email.ATTACHMENT != null)
            {
                foreach (var attachment in email.ATTACHMENT)
                {
                    mensagem.Attachments.Add(attachment);
                }
            }
            tasks.Add(smtp.SendMailAsync(mensagem));
            while (tasks.Count > 0)
            {
                var idx = Task.WaitAny(tasks.ToArray());
                tasks.RemoveAt(idx);
            }
            return "done";
        }

        //public static async Task Execute(Email email, String apiKey)
        //{
        //    //var apiKey = "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";
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
        //    var subject = email.ASSUNTO;
        //    var to = new EmailAddress(email.EMAIL_DESTINO);
        //    var plainTextContent = "and easy to do anywhere, even with C#";
        //    var htmlContent = email.CORPO;
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    var response = await client.SendEmailAsync(msg);
        //}

        [ValidateInput(false)]
        public async Task<int> ProcessarEnvioMensagemEMailAsync(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            GRUPO grupo = null;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            CRMSysDBEntities Db = new CRMSysDBEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);

            // Nome
            if (vm.ID > 0)
            {                
                cliente = cliApp.GetItemById(vm.ID.Value);
                escopo = 1;
            }
            else if (vm.GRUP_CD_ID > 0)
            {
                listaCli = new List<CLIENTE>();
                grupo = gruApp.GetItemById((int)vm.GRUP_CD_ID);
                foreach (GRUPO_CLIENTE item in grupo.GRUPO_CLIENTE)
                {
                    if (item.GRCL_IN_ATIVO == 1)
                    {
                        listaCli.Add(item.CLIENTE);
                    }
                }
                escopo = 2;
            }

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            if (escopo == 1)
            {
                // Template HTML
                if (vm.TEEM_CD_ID != null)
                {
                    TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                    if (temp.TEEM_IN_HTML == 1)
                    {
                        // Prepara corpo do e-mail e trata link
                        String corpo = String.Empty;
                        String caminho = "/Imagens/" + idAss.ToString() + "/TemplatesHTML/" + temp.TEEM_CD_ID.ToString() + "/Arquivos/" + temp.TEEM_AQ_ARQUIVO;
                        String path = Path.Combine(Server.MapPath(caminho));
                        using (StreamReader reader = new System.IO.StreamReader(path))
                        {
                            corpo = reader.ReadToEnd();
                        }
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(corpo);
                        String body = str.ToString();
                        String emailBody = body;

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_TO_DESTINO = cliente.CLIE_NM_EMAIL;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = cliente.CLIE_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.IS_HTML = true;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Envia mensagem
                        try
                        {
                            var voltaMail = CrossCutting.CommunicationPackage.SendEmail(mensagem);
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            if (ex.InnerException != null)
                            {
                                erro += ex.InnerException.Message;
                            }
                            if (ex.GetType() == typeof(SmtpFailedRecipientException))
                            {
                                var se = (SmtpFailedRecipientException)ex;
                                erro += se.FailedRecipient;
                            }
                        }

                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = erro;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_TX_TEXTO = corpo;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }

                        // Envia assincrono
                        erro = null;
                        return volta;
                    }
                    else
                    {
                        // Prepara inicial
                        String body = String.Empty;
                        String header = String.Empty;
                        String footer = String.Empty;
                        String link = String.Empty;
                        if (vm.TEEM_CD_ID != null)
                        {
                            body = temp.TEEM_TX_CORPO;
                            header = temp.TEEM_TX_CABECALHO;
                            footer = temp.TEEM_TX_DADOS;
                            link = temp.TEEM_LK_LINK;                    
                        }

                        // Prepara cabeçalho
                        header = header.Replace("{Nome}", cliente.CLIE_NM_NOME);

                        // Prepara rodape
                        ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                        footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);
                    
                        // Trata corpo
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(body);

                        // Trata link
                        if (!String.IsNullOrEmpty(link))
                        {
                            if (!link.Contains("www."))
                            {
                                link = "www." + link;
                            }
                            if (!link.Contains("http://"))
                            {
                                link = "http://" + link;
                            }
                            str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                        }
                        body = str.ToString();                  
                        String emailBody = header + body + footer;

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_TO_DESTINO = cliente.CLIE_NM_EMAIL;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = cliente.CLIE_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.IS_HTML = true;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Envia mensagem
                        try
                        {
                            var voltaMail = CrossCutting.CommunicationPackage.SendEmail(mensagem);
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            if (ex.InnerException != null)
                            {
                                erro += ex.InnerException.Message;
                            }
                            if (ex.GetType() == typeof(SmtpFailedRecipientException))
                            {
                                var se = (SmtpFailedRecipientException)ex;
                                erro += se.FailedRecipient;
                            }
                        }

                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            String cab = Regex.Replace(header, "<.*?>", String.Empty);
                            String cor = Regex.Replace(body, "<.*?>", String.Empty);
                            String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                            String tex = cab + " " + cor + " " + foot;
                            tex = tex.Replace("\r\n", " ");

                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = erro;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_NM_CABECALHO = header;
                            mens.MENS_NM_RODAPE = footer;
                            mens.MENS_TX_TEXTO = body;
                            mens.MENS_TX_TEXTO_LIMPO = tex;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }

                        // Envia assincrono
                        erro = null;
                        return volta;
                    }
                }
                // Normal
                else
                {
                    // Prepara inicial
                    String body = String.Empty;
                    String header = String.Empty;
                    String footer = String.Empty;
                    String link = String.Empty;
                    body = vm.MENS_TX_TEXTO;
                    header = vm.MENS_NM_CABECALHO;
                    footer = vm.MENS_NM_RODAPE;
                    link = vm.MENS_NM_LINK;

                    // Prepara cabeçalho
                    header = header.Replace("{Nome}", cliente.CLIE_NM_NOME);

                    // Prepara rodape
                    ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                    footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);
                    
                    // Trata corpo
                    StringBuilder str = new StringBuilder();
                    str.AppendLine(body);

                    // Trata link
                    if (!String.IsNullOrEmpty(link))
                    {
                        if (!link.Contains("www."))
                        {
                            link = "www." + link;
                        }
                        if (!link.Contains("http://"))
                        {
                            link = "http://" + link;
                        }
                        str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                    }
                    body = str.ToString();                  
                    String emailBody = header + body + footer;

                    // Checa e monta anexos
                    List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                    if (vm.MENSAGEM_ANEXO.Count > 0)
                    {
                        foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                        {
                            String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                            System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                            listaAnexo.Add(anexo);
                        }
                    }

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_TO_DESTINO = cliente.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = cliente.CLIE_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.IS_HTML = true;
                    mensagem.NETWORK_CREDENTIAL = net;
                    mensagem.ATTACHMENT = listaAnexo;

                    // Envia mensagem
                    try
                    {
                        var voltaMail = CrossCutting.CommunicationPackage.SendEmail(mensagem);
                        //Execute(mensagem).Wait();
                        //var task = SendEmailAssync(mensagem);
                        //String ret = SendEmail(mensagem);
                    }
                    catch (Exception ex)
                    {
                        erro = ex.Message;
                        if (ex.InnerException != null)
                        {
                            erro += ex.InnerException.Message;
                        }
                        if (ex.GetType() == typeof(SmtpFailedRecipientException))
                        {
                            var se = (SmtpFailedRecipientException)ex;
                            erro += se.FailedRecipient;
                        }
                    }

                    // Grava mensagem/destino e erros
                    if (erro == null)
                    {
                        String cab = Regex.Replace(header, "<.*?>", String.Empty);
                        String cor = Regex.Replace(body, "<.*?>", String.Empty);
                        String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                        String tex = cab + " " + cor + " " + foot;
                        tex = tex.Replace("\r\n", " ");

                        MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                        dest.MEDE_IN_ATIVO = 1;
                        dest.MEDE_IN_POSICAO = 1;
                        dest.MEDE_IN_STATUS = 1;
                        dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                        dest.MEDE_DS_ERRO_ENVIO = erro;
                        dest.MENS_CD_ID = mens.MENS_CD_ID;
                        mens.MENSAGENS_DESTINOS.Add(dest);
                        mens.MENS_DT_ENVIO = DateTime.Now;
                        mens.MENS_TX_TEXTO = body;
                        mens.MENS_NM_CABECALHO = header;
                        mens.MENS_NM_RODAPE = footer;
                        mens.MENS_TX_TEXTO_LIMPO = tex;
                        volta = baseApp.ValidateEdit(mens, mens);
                    }
                    else
                    {
                        mens.MENS_TX_RETORNO = erro;
                        volta = baseApp.ValidateEdit(mens, mens);
                    }

                    // Envia assincrono
                    erro = null;
                    return volta;
                }
            }
            else
            {
                // Template HTML
                if (vm.TEEM_CD_ID != null)
                {
                    TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                    if (temp.TEEM_IN_HTML == 1)
                    {
                        // Prepara corpo do e-mail e trata link
                        String corpo = String.Empty;
                        String caminho = "/Imagens/" + idAss.ToString() + "/TemplatesHTML/" + temp.TEEM_CD_ID.ToString() + "/Arquivos/" + temp.TEEM_AQ_ARQUIVO;
                        String path = Path.Combine(Server.MapPath(caminho));
                        using (StreamReader reader = new System.IO.StreamReader(path))
                        {
                            corpo = reader.ReadToEnd();
                        }
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(corpo);
                        String body = str.ToString();
                        String emailBody = body;

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Monta mensagem
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Monta destinos
                        MailAddressCollection col = new MailAddressCollection();                       

                        // Envia mensagem
                        try
                        {
                            foreach (CLIENTE item in listaCli)
                            {
                                col.Clear();

                                col.Add(new MailAddress(item.CLIE_NM_NOME, item.CLIE_NM_EMAIL));
                                CommunicationPackage.SendEmailCollectionAsync(mensagem, col);
                            }                            
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            if (ex.GetType() == typeof(SmtpFailedRecipientException))
                            {
                                var se = (SmtpFailedRecipientException)ex;
                                erro += se.FailedRecipient;
                            }
                        }

                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            foreach (CLIENTE item in listaCli)
                            {
                                MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                dest.MEDE_IN_ATIVO = 1;
                                dest.MEDE_IN_POSICAO = 1;
                                dest.MEDE_IN_STATUS = 1;
                                dest.CLIE_CD_ID = item.CLIE_CD_ID;
                                dest.MEDE_DS_ERRO_ENVIO = erro;
                                dest.MENS_CD_ID = mens.MENS_CD_ID;
                                mens.MENSAGENS_DESTINOS.Add(dest);
                                mens.MENS_DT_ENVIO = DateTime.Now;
                                mens.MENS_TX_TEXTO = body;
                                volta = baseApp.ValidateEdit(mens, mens);
                            }
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        erro = null;
                        return volta;
                    }
                    else
                    {
                        // Prepara inicial
                        String body = String.Empty;
                        String header = String.Empty;
                        String footer = String.Empty;
                        String link = String.Empty;
                        if (vm.TEEM_CD_ID != null)
                        {
                            body = temp.TEEM_TX_CORPO;
                            header = temp.TEEM_TX_CABECALHO;
                            footer = temp.TEEM_TX_DADOS;
                            link = temp.TEEM_LK_LINK;                    
                        }
                   
                        // Trata corpo
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(body);

                        // Trata link
                        if (!String.IsNullOrEmpty(link))
                        {
                            if (!link.Contains("www."))
                            {
                                link = "www." + link;
                            }
                            if (!link.Contains("http://"))
                            {
                                link = "http://" + link;
                            }
                            str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                        }
                        body = str.ToString();                  

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Prepara rodape
                        ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                        footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);

                        // Monta Mensagem
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Checa cabeçalho e envia
                        if (header.Contains("{Nome}"))
                        {
                            foreach (CLIENTE item in listaCli)
                            {
                                // Prepara cabeçalho
                                header = header.Replace("{Nome}", item.CLIE_NM_NOME);
                                String emailBody = header + body + footer;

                                // Monta e-mail
                                mensagem.CORPO = emailBody;
                                mensagem.EMAIL_TO_DESTINO = item.CLIE_NM_EMAIL;

                                // Envia mensagem
                                try
                                {
                                    var voltaMail = CrossCutting.CommunicationPackage.SendEmail(mensagem);

                                }
                                catch (Exception ex)
                                {
                                    erro = ex.Message;
                                    if (ex.GetType() == typeof(SmtpFailedRecipientException))
                                    {
                                        var se = (SmtpFailedRecipientException)ex;
                                        erro += se.FailedRecipient;
                                    }
                                }
                            }
                        }
                        else
                        {
                            String emailBody = header + body + footer;
                            mensagem.CORPO = emailBody;
                            MailAddressCollection col = new MailAddressCollection();                           

                            // Envia mensagem
                            try
                            {
                                foreach (CLIENTE item in listaCli)
                                {
                                    col.Clear();
                                    col.Add(new MailAddress(item.CLIE_NM_NOME, item.CLIE_NM_EMAIL));
                                    CommunicationPackage.SendEmailCollectionAsync(mensagem, col);
                                }
                            }
                            catch (Exception ex)
                            {
                                erro = ex.Message;
                                if (ex.GetType() == typeof(SmtpFailedRecipientException))
                                {
                                    var se = (SmtpFailedRecipientException)ex;
                                    erro += se.FailedRecipient;
                                }
                            }
                        }

                        // Loop de retorno
                        foreach (CLIENTE item in listaCli)
                        {
                            // Grava mensagem/destino e erros
                            if (erro == null)
                            {
                                String cab = Regex.Replace(header, "<.*?>", String.Empty);
                                String cor = Regex.Replace(body, "<.*?>", String.Empty);
                                String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                                String tex = cab + " " + cor + " " + foot;
                                tex = tex.Replace("\r\n", " ");

                                MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                dest.MEDE_IN_ATIVO = 1;
                                dest.MEDE_IN_POSICAO = 1;
                                dest.MEDE_IN_STATUS = 1;
                                dest.CLIE_CD_ID = item.CLIE_CD_ID;
                                dest.MEDE_DS_ERRO_ENVIO = erro;
                                dest.MENS_CD_ID = mens.MENS_CD_ID;
                                mens.MENSAGENS_DESTINOS.Add(dest);
                                mens.MENS_DT_ENVIO = DateTime.Now;
                                mens.MENS_TX_TEXTO = body;
                                mens.MENS_NM_CABECALHO = header;
                                mens.MENS_NM_RODAPE = footer;
                                mens.MENS_TX_TEXTO_LIMPO = tex;
                                volta = baseApp.ValidateEdit(mens, mens);
                            }
                            else
                            {
                                mens.MENS_TX_RETORNO = erro;
                                volta = baseApp.ValidateEdit(mens, mens);
                            }
                            erro = null;
                        }
                        return volta;
                    }
                }
                // Normal
                else
                {
                    // Prepara inicial
                    String body = String.Empty;
                    String header = String.Empty;
                    String footer = String.Empty;
                    String link = String.Empty;
                    body = vm.MENS_TX_TEXTO;
                    header = vm.MENS_NM_CABECALHO;
                    footer = vm.MENS_NM_RODAPE;
                    link = vm.MENS_NM_LINK;

                    // Prepara rodape
                    ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                    footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);
                    
                    // Trata corpo
                    StringBuilder str = new StringBuilder();
                    str.AppendLine(body);

                    // Trata link
                    if (!String.IsNullOrEmpty(link))
                    {
                        if (!link.Contains("www."))
                        {
                            link = "www." + link;
                        }
                        if (!link.Contains("http://"))
                        {
                            link = "http://" + link;
                        }
                        str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                    }
                    body = str.ToString();                  

                    // Checa e monta anexos
                    List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                    if (vm.MENSAGEM_ANEXO.Count > 0)
                    {
                        foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                        {
                            String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                            System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                            listaAnexo.Add(anexo);
                        }
                    }

                    // Monta Mensagem
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;
                    mensagem.ATTACHMENT = listaAnexo;

                    String emailBody = header + body + footer;
                    mensagem.CORPO = emailBody;
                    MailAddressCollection col = new MailAddressCollection();
                    
                    // Envia mensagem
                    try
                    {
                        foreach (CLIENTE item in listaCli)
                        {
                            col.Clear();
                            col.Add(new MailAddress(item.CLIE_NM_NOME, item.CLIE_NM_EMAIL));
                            CommunicationPackage.SendEmailCollectionAsync(mensagem, col);
                        }

                    }
                    catch (Exception ex)
                    {
                        erro = ex.Message;
                        if (ex.GetType() == typeof(SmtpFailedRecipientException))
                        {
                            var se = (SmtpFailedRecipientException)ex;
                            erro += se.FailedRecipient;
                        }
                    }

                    // Loop de retorno
                    foreach (CLIENTE item in listaCli)
                    {
                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            String cab = Regex.Replace(header, "<.*?>", String.Empty);
                            String cor = Regex.Replace(body, "<.*?>", String.Empty);
                            String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                            String tex = cab + " " + cor + " " + foot;
                            tex = tex.Replace("\r\n", " ");

                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.CLIE_CD_ID = item.CLIE_CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = erro;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_TX_TEXTO = body;
                            mens.MENS_NM_CABECALHO = header;
                            mens.MENS_NM_RODAPE = footer;
                            mens.MENS_TX_TEXTO_LIMPO = tex;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        erro = null;
                    }
                    return volta;
                }
            }
            //return 0;
        }

        [ValidateInput(false)]
        public Int32 ProcessarAgendamentoMensagemEMail(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            GRUPO grupo = null;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            CRMSysDBEntities Db = new CRMSysDBEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);

            // Nome
            if (vm.ID > 0)
            {                
                cliente = cliApp.GetItemById(vm.ID.Value);
                escopo = 1;
            }
            else if (vm.GRUP_CD_ID > 0)
            {
                listaCli = new List<CLIENTE>();
                grupo = gruApp.GetItemById((int)vm.GRUP_CD_ID);
                foreach (GRUPO_CLIENTE item in grupo.GRUPO_CLIENTE)
                {
                    if (item.GRCL_IN_ATIVO == 1)
                    {
                        listaCli.Add(item.CLIENTE);
                    }
                }
                escopo = 2;
            }

            // Recupera textos
            String body = String.Empty;
            String header = String.Empty;
            String footer = String.Empty;
            if (vm.TEEM_CD_ID != null)
            {
                TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                body = temp.TEEM_TX_CORPO;
                header = temp.TEEM_TX_CABECALHO;
                footer = temp.TEEM_TX_DADOS;
            }
            else
            {
                body = vm.MENS_TX_TEXTO;
                header = vm.MENS_NM_CABECALHO;
                footer = vm.MENS_NM_RODAPE;
            }
            String cab = Regex.Replace(header, "<.*?>", String.Empty);
            String cor = Regex.Replace(body, "<.*?>", String.Empty);
            String foot = Regex.Replace(footer, "<.*?>", String.Empty);
            String tex = cab + " " + cor + " " + foot;
            tex = tex.Replace("\r\n", " ");

            // Regrava mensagem
            MENSAGENS mensX = baseApp.GetItemById(vm.MENS_CD_ID);
            mensX.MENS_TX_TEXTO_LIMPO = tex;
            Int32 vol1 = baseApp.ValidateEdit(mensX, mensX);

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            if (escopo == 1)
            {
                // Grava Agendamento
                EMAIL_AGENDAMENTO ag = new EMAIL_AGENDAMENTO();
                ag.ASSI_CD_ID = idAss;
                ag.CLIE_CD_ID = cliente.CLIE_CD_ID;
                ag.MENS_CD_ID = vm.MENS_CD_ID;
                ag.EMAG_DT_AGENDAMENTO = vm.MENS_DT_AGENDAMENTO;
                ag.EMAG_IN_ENVIADO = 0;
                Int32 volta1 = emApp.ValidateCreate(ag);
                return volta;
            }
            else
            {
                foreach (CLIENTE item in listaCli)
                {

                    // Grava Agendamento
                    EMAIL_AGENDAMENTO ag = new EMAIL_AGENDAMENTO();
                    ag.ASSI_CD_ID = idAss;
                    ag.CLIE_CD_ID = item.CLIE_CD_ID;
                    ag.MENS_CD_ID = vm.MENS_CD_ID;
                    ag.EMAG_DT_AGENDAMENTO = vm.MENS_DT_AGENDAMENTO;
                    ag.EMAG_IN_ENVIADO = 0;
                    Int32 volta1 = emApp.ValidateCreate(ag);
                }
                return volta;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult VerAnexoMensagem(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoMensagem()
        {

            return RedirectToAction("VerMensagem", new { id = (Int32)Session["IdMensagem"] });
        }

        public FileResult DownloadMensagem(Int32 id)
        {
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.MEAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            else if (arquivo.Contains(".docx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            else if (arquivo.Contains(".xlsx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else if (arquivo.Contains(".pptx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            }
            else if (arquivo.Contains(".mp3"))
            {
                contentType = "audio/mpeg";
            }
            else if (arquivo.Contains(".mpeg"))
            {
                contentType = "audio/mpeg";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpGet]
        public ActionResult VerMensagemEMail(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
                }
                if ((Int32)Session["PermMens"] == 0)
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

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
            }
            Session["IdMensagem"] = id;
            Session["VoltaMensagem"] = 1;
            MENSAGENS item = baseApp.GetItemById(id);
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardMensagens()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega numeros
            Int32 volta = CarregaNumeroMensagem();

            // Resumo Mes E-Mail
            List<MENSAGENS> listaMesEMail = (List<MENSAGENS>)Session["ListaMesEMail"];  
            List<DateTime> datas = listaMesEMail.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaMesEMail.Where(p => p.MENS_DT_CRIACAO.Value.Date == item & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaEMailMes = lista;
            ViewBag.ContaEMailMes = listaMesEMail.Count;

            Session["ListaEMail"] = listaMesEMail;
            Session["ListaDatasEMail"] = datas;

            // Resumo Mes SMS
            List<MENSAGENS> listaMesSMS = (List<MENSAGENS>)Session["ListaMesSMS"];
            List<DateTime> datas1 = listaMesSMS.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            foreach (DateTime item in datas1)
            {
                Int32 conta = listaMesSMS.Where(p => p.MENS_DT_CRIACAO.Value.Date == item & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista1.Add(mod);
            }
            ViewBag.ListaSMSMes = lista1;
            ViewBag.ContaSMSMes = listaMesSMS.Count;
            Session["ListaSMS"] = listaMesSMS;
            Session["ListaDatasSMS"] = datas1;
            return View(usuario);
        }


        public Int32 CarregaNumeroMensagem()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Recupera listas Mensagens - Email
            List<MENSAGENS> lt = CarregarMensagem();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                lt = lt.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<MENSAGENS> emails = lt.Where(p => p.MENS_IN_TIPO == 1 & p.MENS_IN_DESTINOS != null).ToList();
            List<MENSAGENS> listaMesEMail = emails.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<MENSAGENS> listaDiaEMail = emails.Where(p => p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).ToList();

            List<MENSAGENS> emailsMes = emails.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<MENSAGENS> emailsDia = emails.Where(p => p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).ToList();
            
            List<MENSAGENS> emailsAguarda = emails.Where(p => p.MENS_IN_STATUS == 1 & p.MENS_DT_AGENDAMENTO != null).ToList();
            emailsAguarda = emailsAguarda.Where(p => p.MENS_DT_AGENDAMENTO > DateTime.Now).ToList();
            Int32 aguarda = emailsAguarda.Sum(p => p.MENS_IN_DESTINOS.Value);
            
            List<MENSAGENS> emailsEnviado = emails.Where(p => p.MENS_IN_STATUS == 2).ToList();
            List<MENSAGENS> emailsFalha = emails.Where(p => p.MENS_IN_STATUS == 3).ToList();
            Int32 emailTot = emails.Where(p => p.MENS_IN_TIPO == 1 & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
            Int32 emailTotMes = listaMesEMail.Where(p => p.MENS_IN_TIPO == 1 & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
            Int32 emailTotDia = listaDiaEMail.Where(p => p.MENS_IN_TIPO == 1 & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);

            // Recupera listas Mensagens - SMS
            List<MENSAGENS> sms = lt.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_DESTINOS != null).ToList();
            List<MENSAGENS> listaMesSMS = sms.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<MENSAGENS> listaDiaSMS = sms.Where(p => p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).ToList();

            List<MENSAGENS> smsMes = sms.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<MENSAGENS> smsDia = sms.Where(p => p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).ToList();

            List<MENSAGENS> smsAguarda = sms.Where(p => p.MENS_DT_AGENDAMENTO != null).ToList();
            smsAguarda = smsAguarda.Where(p => p.MENS_DT_AGENDAMENTO > DateTime.Now).ToList();
            Int32 aguardaSMS = smsAguarda.Sum(p => p.MENS_IN_DESTINOS.Value);

            List<MENSAGENS> smsEnviado = sms.Where(p => p.MENS_IN_STATUS == 2).ToList();
            List<MENSAGENS> smsFalha = sms.Where(p => p.MENS_IN_STATUS == 3).ToList();

            Int32 smsTot = sms.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
            Int32 smsTotMes = listaMesSMS.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
            Int32 smsTotDia = listaDiaSMS.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);

            // Viewbags
            ViewBag.Total = lt.Count;
            ViewBag.TotalEMails= emails.Count;
            ViewBag.TotalSMS = sms.Count;

            ViewBag.EMailsAguarda = aguarda;
            ViewBag.EMailsEnviado= emailsEnviado.Count;
            ViewBag.EMailsFalha= emailsFalha.Count;
            ViewBag.EMailsMes= emailsMes.Count;
            ViewBag.EMailsDia = emailsDia.Count;
            ViewBag.EMailsAgenda = aguarda;
            ViewBag.EMailsTotalEnvio = emailTot;
            ViewBag.EMailsTotalEnvioMes = emailTotMes;
            ViewBag.EMailsTotalEnvioDia = emailTotDia;

            ViewBag.SMSAguarda = aguardaSMS;
            ViewBag.SMSEnviado = smsEnviado.Count;
            ViewBag.SMSFalha = smsFalha.Count;
            ViewBag.SMSMes = smsMes.Count;
            ViewBag.SMSDia = smsDia.Count;
            ViewBag.SMSAgenda = aguardaSMS;
            ViewBag.SMSTotalEnvio = smsTot;
            ViewBag.SMSTotalEnvioMes = smsTotMes;
            ViewBag.SMSTotalEnvioDia = smsTotDia;

            Session["EMailTotalEnvio"] = emailTot;
            Session["EMailTotalEnvioMes"] = emailTotMes;
            Session["EMailTotalEnvioDia"] = emailTotDia;
            Session["EMailAguarda"] = aguarda;
            Session["EMailFalha"] = emailsFalha.Count;
            Session["EMailAgendaNum"] = aguarda;
            Session["ListaMesEMail"] = listaMesEMail;
            Session["ListaEMailTudo"] = emails;
            Session["EMailAgenda"] = aguarda;

            Session["SMSTotalEnvio"] = smsTot;
            Session["SMSTotalEnvioMes"] = smsTotMes;
            Session["SMSTotalEnvioDia"] = smsTotDia;
            Session["SMSAguarda"] = aguardaSMS;
            Session["SMSFalha"] = smsFalha.Count;
            Session["SMSAgendaNum"] = aguardaSMS;
            Session["ListaMesSMS"] = listaMesSMS;
            Session["ListaSMSTudo"] = sms;
            Session["SMSAgenda"] = aguardaSMS;

            Session["VoltaMensagem"] = 0;
            Session["ListaMensagem"] = null;
            return 0;
        }

        public ActionResult VerTotalExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = CarregaMensagem().Where(p => p.MENS_DT_CRIACAO != null).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaBase1 = listaBase1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaTotal"] = listaBase;
            Session["ListaDatasTotal"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoTotal()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaTotalTodas"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasTotalTodas"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoTotalTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaTotal"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasTotal"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerTotalExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = CarregaMensagem().Where(p => p.MENS_DT_CRIACAO != null).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaBase = listaBase.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaTotalTodas"] = listaBase;
            Session["ListaDatasTotalTodas"] = datas;
            return View();
        }

        public ActionResult VerEMailExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = CarregaMensagem().Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO != null).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaBase1 = listaBase1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaEMail"] = listaBase;
            Session["ListaDatas"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoEmail()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaEMail"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasEMail"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                Int32 contaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item  & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoEmailTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaEMailTudo"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasEMailTudo"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerEMailExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = (List<MENSAGENS>)Session["ListaEMailTudo"];
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaEMailTudo = lista;
            ViewBag.ContaEMailTudo = listaBase.Count;
            Session["ListaEMailTudo"] = listaBase;
            Session["ListaDatasEMailTudo"] = datas;
            return View();
        }

        public ActionResult VerSMSExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = CarregaMensagem().Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_CRIACAO != null).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaBase1 = listaBase1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaSMS"] = listaBase;
            Session["ListaDatasSMS"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoSMS()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMS"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMS"];
            List<MENSAGENS_DESTINOS> listaDia = new List<MENSAGENS_DESTINOS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                Int32 contaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoSMSTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMSTudo"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMSTudo"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerSMSExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = (List<MENSAGENS>)Session["ListaSMSTudo"];
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaSMSTudo = lista;
            ViewBag.ContaSMSTudo = listaBase.Count;
            Session["ListaSMSTudo"] = listaBase;
            Session["ListaDatasSMSTudo"] = datas;
            return View();
        }

        public ActionResult VerFalhasExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = CarregaMensagem().Where(p => p.MENS_TX_RETORNO != null).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaBase1 = listaBase1.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaFalha"] = listaBase;
            Session["ListaDatasFalha"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoFalhas()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaFalha"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasFalha"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoFalhasTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaFalhaTodas"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasFalhaTodas"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerFalhasExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = CarregaMensagem().Where(p => p.MENS_TX_RETORNO != null).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaBase = listaBase.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaFalha"] = listaBase;
            Session["ListaDatasFalha"] = datas;
            return View();
        }

        public ActionResult MostrarClientes()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 30;
            return RedirectToAction("MontarTelaCliente", "Cliente");
        }

        public ActionResult MostrarGrupos()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            return RedirectToAction("MontarTelaGrupo", "Grupo");
        }

        public ActionResult IncluirClienteRapido()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 30;
            return RedirectToAction("IncluirClienteRapido", "Cliente");
        }

        public string EnviarEmailsAgendados()
        {
            baseApp.SendEmailScheduleAsync(null, Server);

            return "FIM";
        }

        public string EnviarSmsAgendados()
        {
            baseApp.SendSmsSchedule(null, temApp);

            return "FIM";
        }

        public List<MENSAGENS> CarregaSMSPadraoDia()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> conf = new List<MENSAGENS>();
            if (Session["SMSPadrao"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
                conf = conf.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_TIPO_SMS == 1 & p.MENS_DT_CRIACAO == DateTime.Today.Date).ToList();
            }
            else
            {
                if ((Int32)Session["SMSPadraoAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
                    conf = conf.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_TIPO_SMS == 1 & p.MENS_DT_CRIACAO == DateTime.Today.Date).ToList();
                }
                else
                {
                    conf = (List<MENSAGENS>)Session["SMSPadrao"];
                }
            }
            Session["SMSPadrao"] = conf;
            Session["SMSPadraoAlterada"] = 0;
            return conf;
        }

        public List<MENSAGENS> CarregaSMSPrioritarioDia()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> conf = new List<MENSAGENS>();
            if (Session["SMSPrior"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
                conf = conf.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_TIPO_SMS == 2 & p.MENS_DT_CRIACAO == DateTime.Today.Date).ToList();
            }
            else
            {
                if ((Int32)Session["SMSPriorAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
                    conf = conf.Where(p => p.MENS_IN_TIPO == 2 & p.MENS_IN_TIPO_SMS == 2 & p.MENS_DT_CRIACAO == DateTime.Today.Date).ToList();
                }
                else
                {
                    conf = (List<MENSAGENS>)Session["SMSPrior"];
                }
            }
            Session["SMSPrior"] = conf;
            Session["SMSPriorAlterada"] = 0;
            return conf;
        }

        public List<MENSAGENS> CarregaEMailDia()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> conf = new List<MENSAGENS>();
            if (Session["EMails"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
                conf = conf.Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO == DateTime.Today.Date).ToList();
            }
            else
            {
                if ((Int32)Session["EMailPadraoAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
                    conf = conf.Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO == DateTime.Today.Date).ToList();
                }
                else
                {
                    conf = (List<MENSAGENS>)Session["EMailPadrao"];
                }
            }
            Session["EMailPadrao"] = conf;
            Session["EMailPadraoAlterada"] = 0;
            return conf;
        }

        public List<PERIODICIDADE_TAREFA> CarregaPeriodicidade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PERIODICIDADE_TAREFA> conf = new List<PERIODICIDADE_TAREFA>();
            if (Session["Periodicidades"] == null)
            {
                conf = periodicidadeApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["PeriodicidadeAlterada"] == 1)
                {
                    conf = periodicidadeApp.GetAllItens();
                }
                else
                {
                    conf = (List<PERIODICIDADE_TAREFA>)Session["Periodicidades"];
                }
            }
            Session["PeriodicidadeAlterada"] = 0;
            Session["Periodicidades"] = conf;
            return conf;
        }

        [HttpGet]
        public ActionResult MontarTelaRecursiva()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if (Session["ListaRecursividade"] == null)
            {
                listaMasterRec = CarregaRecursividade().Where(p => p.RECU_IN_TIPO_MENSAGEM == 2).OrderByDescending(m => m.RECU_DT_CRIACAO).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMasterRec = listaMasterRec.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaRecursividade"] = listaMasterRec;
            }
            ViewBag.Listas = (List<RECURSIVIDADE>)Session["ListaRecursividade"];

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            Session["MensMensagem"] = null;
            objetoRec = new RECURSIVIDADE();
            objetoRec.RECU_DT_CRIACAO = DateTime.Today.Date;
            return View(objetoRec);
        }

        [HttpGet]
        public ActionResult MontarTelaRecursivaEMail()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if (Session["ListaRecursividadeEMail"] == null)
            {
                listaMasterRec = CarregaRecursividade().Where(p => p.RECU_IN_TIPO_MENSAGEM == 1).OrderByDescending(m => m.RECU_DT_CRIACAO).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMasterRec = listaMasterRec.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaRecursividadeEMail"] = listaMasterRec;
            }
            ViewBag.Listas = (List<RECURSIVIDADE>)Session["ListaRecursividadeEMail"];

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            Session["MensMensagem"] = null;
            objetoRec = new RECURSIVIDADE();
            objetoRec.RECU_DT_CRIACAO = DateTime.Today.Date;
            return View(objetoRec);
        }

        [HttpPost]
        public ActionResult FiltrarRecursividade(RECURSIVIDADE item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                Tuple<Int32, List<RECURSIVIDADE>, Boolean> volta = recApp.ExecuteFilter(2, item.RECU_NM_NOME, item.RECU_DT_CRIACAO, item.RECU_DT_DUMMY, item.RECU_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaRecursiva");
                }

                // Sucesso
                listaMasterRec = volta.Item2;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMasterRec = listaMasterRec.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaRecursividade"] = listaMasterRec;
                return RedirectToAction("MontarTelaRecursiva");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarRecursividadeEMail(RECURSIVIDADE item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                Tuple<Int32, List<RECURSIVIDADE>, Boolean> volta = recApp.ExecuteFilter(1, item.RECU_NM_NOME, item.RECU_DT_CRIACAO, item.RECU_DT_DUMMY, item.RECU_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaRecursivaEMail");
                }

                // Sucesso
                listaMasterRec = volta.Item2;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMasterRec = listaMasterRec.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaRecursividadeEMail"] = listaMasterRec;
                return RedirectToAction("MontarTelaRecursivaEMail");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroRecursividade()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaRecursividade"] = null;
            return RedirectToAction("MontarTelaRecursiva");
        }

        public ActionResult RetirarFiltroRecursividadeEMail()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaRecursividadeEMail"] = null;
            return RedirectToAction("MontarTelaRecursivaEMail");
        }

        [HttpGet]
        public ActionResult VerRecursividades(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MENSAGENS mens = baseApp.GetItemById(id);
            List<RECURSIVIDADE> recs = mens.RECURSIVIDADE.ToList();
            RECURSIVIDADE item = recs.Where(p => p.MENS_CD_ID == id).FirstOrDefault();
            Session["IdRecursividade"] = item.RECU_CD_ID;
            ViewBag.Destinos = item.RECURSIVIDADE_DESTINO.ToList();
            ViewBag.Datas = item.RECURSIVIDADE_DATA.ToList();
            return View(item);
        }

        public List<RECURSIVIDADE> CarregaRecursividade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<RECURSIVIDADE> conf = new List<RECURSIVIDADE>();
            if (Session["Recursividades"] == null)
            {
                conf = recApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["RecursividadeAlterada"] == 1)
                {
                    conf = recApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<RECURSIVIDADE>)Session["Recursividades"];
                }
            }
            Session["RecursividadeAlterada"] = 0;
            Session["Recursividades"] = conf;
            return conf;
        }

        public List<MENSAGENS> CarregarMensagem()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> conf = new List<MENSAGENS>();
            //if (Session["Mensagens"] == null)
            //{
                conf = baseApp.GetAllItens(idAss);
            //}
            //else
            //{
            //    if ((Int32)Session["MensagemAlterada"] == 1)
            //    {
            //        conf = baseApp.GetAllItens(idAss);
            //    }
            //    else
            //    {
            //        conf = (List<MENSAGENS>)Session["Mensagens"];
            //    }
            //}
            Session["MensagemAlterada"] = 0;
            Session["Mensagens"] = conf;
            return conf;
        }

        public List<RESULTADO_ROBOT> CarregarEnvios()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<RESULTADO_ROBOT> conf = new List<RESULTADO_ROBOT>();
            conf = baseApp.GetAllEnviosRobot(idAss);
            return conf;
        }

        [NonAction]
        public List<SelectListItem> CarregarModelosHtml()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            String caminho = "/TemplateEMail/Modelos/" + idAss.ToString() + "/";
            String path = Path.Combine(Server.MapPath(caminho));
            String[] files = Directory.GetFiles(path, "*.html");
            List<SelectListItem> mod = new List<SelectListItem>();
            foreach (String file in files)
            {
                mod.Add(new SelectListItem() { Text = System.IO.Path.GetFileNameWithoutExtension(file), Value = file });
            }

            return mod;
        }

        #region Métodos

        [NonAction]
        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        

        #endregion


    }
}