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
    public class AuditoriaController : Controller
    {
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        LOG objeto = new LOG();
        LOG objetoAntes = new LOG();
        List<LOG> listaMaster = new List<LOG>();
        String extensao;

        public AuditoriaController(ILogAppService logApps, IUsuarioAppService usuApps)
        {
            logApp = logApps;
            usuApp = usuApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
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

        [HttpGet]
        public ActionResult MontarTelaLog()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
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
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if ((List<LOG>)Session["ListaLog"] == null)
            {
                listaMaster = logApp.GetAllItensMesCorrente(idAss);
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUARIO.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaLog"] = listaMaster;
                Session["FiltroLog"] = null;
                Session["MensagemLonga"] = 0;
            }
            ViewBag.Listas = (List<LOG>)Session["ListaLog"];
            ViewBag.Logs = ((List<LOG>)Session["ListaLog"]).Count;
            ViewBag.LogsDataCorrente = logApp.GetAllItensDataCorrente(idAss).Count;
            ViewBag.LogsMesCorrente = ((List<LOG>)Session["ListaLog"]).Count;

            List<LOG> listAnt = logApp.GetAllItensMesAnterior(idAss);
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listAnt = listAnt.Where(p => p.USUARIO.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.LogsMesAnterior = listAnt.Count;

            // Mensagens
            if ((Int32)Session["MensLog"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensLog"] = 0;
            objeto = new LOG();
            objeto.LOG_DT_DATA = DateTime.Today.Date;
            return View(objeto);
        }

        public ActionResult RetirarFiltroLog()
        {
            Session["ListaLog"] = null;
            Session["FiltroLog"] = null;
            return RedirectToAction("MontarTelaLog");
        }

        public ActionResult VerTodosLog()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = logApp.GetAllItens(idAss);
            Session["ListaLog"] = listaMaster;
            Session["MensagemLonga"] = 1;
            return RedirectToAction("MontarTelaLog");
        }

        [HttpPost]
        public ActionResult FiltrarLog(LOG item)
        {
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOG> listaObj = new List<LOG>();
                Session["FiltroLog"] = item;
                Tuple<Int32, List<LOG>, Boolean> volta = logApp.ExecuteFilterTuple(item.USUA_CD_ID, item.LOG_DT_DATA, item.LOG_NM_OPERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLog"] = 1;
                    return RedirectToAction("MontarTelaLog");
                }

                // Sucesso
                listaMaster = volta.Item2;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUARIO.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaLog"] = listaMaster;
                return RedirectToAction("MontarTelaLog");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerLog(Int32 id)
        {

            // Prepara view
            LOG item = logApp.GetById(id);
            LogViewModel vm = Mapper.Map<LOG, LogViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarBaseLog()
        {

            return RedirectToAction("MontarTelaLog");
        }
    }
}