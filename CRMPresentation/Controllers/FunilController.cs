using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using Canducci.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EntitiesServices.Attributes;
using OfficeOpenXml.Table;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class FunilController : Controller
    {
        private readonly IFunilAppService baseApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ICRMAppService crmApp;

        private String msg;
        private Exception exception;
        FUNIL objeto = new FUNIL();
        FUNIL objetoAntes = new FUNIL();
        List<FUNIL> listaMaster = new List<FUNIL>();
        String extensao;

        public FunilController(IFunilAppService baseApps, IConfiguracaoAppService confApps, ICRMAppService crmApps, IUsuarioAppService usuApps)
        {
            baseApp = baseApps;
            usuApp = usuApps;
            confApp = confApps;
            crmApp = crmApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        [HttpGet]
        public ActionResult MontarTelaFunil()
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
            if ((List<FUNIL>)Session["ListaFunil"] == null)
            {
                listaMaster = CarregaFunil();
                Session["ListaFunil"] = listaMaster;
            }
            ViewBag.Listas = (List<FUNIL>)Session["ListaFunil"];
            ViewBag.Title = "Funil";

            // Mensagens
            if (Session["MensFunil"] != null)
            {
                if ((Int32)Session["MensFunil"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0193", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0194", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensFunil"] = null;
            Session["VoltaFunil"] = 1;
            objeto = new FUNIL();
            objeto.FUNI_IN_ATIVO = 1;
            return View(objeto);
        }

        public ActionResult RetirarFiltroFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFunil"] = null;
            return RedirectToAction("MontarTelaFunil");
        }

        public ActionResult MostrarTudoFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaFunil"] = listaMaster;
            return RedirectToAction("MontarTelaFunil");
        }

        public ActionResult VoltarBaseFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaFunil");
        }

       [HttpGet]
        public ActionResult IncluirFunil()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            FUNIL item = new FUNIL();
            FunilViewModel vm = Mapper.Map<FUNIL, FunilViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.FUNI_DT_CADASTRO = DateTime.Today;
            vm.FUNI_IN_ATIVO = 1;
            vm.FUNI_IN_FIXO = 0;
            vm.FUNI_IN_TIPO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirFunil(FunilViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FUNIL item = Mapper.Map<FunilViewModel, FUNIL>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensFunil"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0193", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMaster = new List<FUNIL>();
                    Session["ListaFunil"] = null;
                    Session["IdFunil"] = item.FUNI_CD_ID;
                    Session["FunilAlterada"] = 1;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarFunil(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensFunil"] = 2;
                    return RedirectToAction("MontarTelaFunil");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagens
            if (Session["MensFunil"] != null)
            {
                if ((Int32)Session["MensFunil"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0195", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            FUNIL item = baseApp.GetItemById(id);
            CONFIGURACAO conf = confApp.GetItemById(idAss);
            Int32 etapa = 0;
            if (item.FUNIL_ETAPA.Count < conf.CONF_IN_ETAPAS_CRM)
            {
                etapa = 1;
            }
            ViewBag.Etapa = etapa;

            // Indicadores
            Session["VoltaFunil"] = 1;
            objetoAntes = item;
            Session["Funil"] = item;
            Session["IdFunil"] = id;
            FunilViewModel vm = Mapper.Map<FUNIL, FunilViewModel>(item);
            return View(vm);

        }

        [HttpPost]
        public ActionResult EditarFunil(FunilViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {   
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FUNIL item = Mapper.Map<FunilViewModel, FUNIL>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<FUNIL>();
                    Session["ListaFunil"] = null;
                    Session["FunilAlterada"] = 1;
                    return RedirectToAction("MontarTelaFunil");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirFunil(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                //Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensFunil"] = 2;
                    return RedirectToAction("MontarTelaFunil");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                FUNIL item = baseApp.GetItemById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUNI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensFunil"] = 4;
                    return RedirectToAction("MontarTelaFunil", "Funil");
                }
                listaMaster = new List<FUNIL>();
                Session["ListaFunil"] = null;
                Session["FunilAlterada"] = 1;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarFunil(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                //Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensFunil"] = 2;
                    return RedirectToAction("MontarTelaFunil");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                FUNIL item = baseApp.GetItemById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUNI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                listaMaster = new List<FUNIL>();
                Session["ListaFunil"] = null;
                Session["FunilAlterada"] = 1;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarFunil", new { id = (Int32)Session["IdFunil"] });
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
        }

        [HttpGet]
        public ActionResult EditarEtapa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagens
            if (Session["MensFunil"] != null)
            {
                if ((Int32)Session["MensFunil"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0196", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 8)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 9)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0199", CultureInfo.CurrentCulture));
                }
            }
                Session["MensFunil"] = null;

            // Recupera ultima etapa
            FUNIL funil = (FUNIL)Session["Funil"];
            Int32? ordem = funil.FUNIL_ETAPA.OrderByDescending(p => p.FUET_IN_ORDEM).FirstOrDefault().FUET_IN_ORDEM;
            Session["Ordem"] = ordem;

            // Recupera maximo
            CONFIGURACAO conf = confApp.GetItemById(idAss);
            Session["OrdemMax"] = conf.CONF_IN_ETAPAS_CRM;

            // Prepara view
            FUNIL_ETAPA item = baseApp.GetEtapaById(id);
            objetoAntes = (FUNIL)Session["Funil"];
            FunilEtapaViewModel vm = Mapper.Map<FUNIL_ETAPA, FunilEtapaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEtapa(FunilEtapaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Valida flags
                    Int32 id = vm.FUET_CD_ID;
                    List<FUNIL_ETAPA> lista = baseApp.GetItemById(vm.FUNI_CD_ID).FUNIL_ETAPA.ToList();
                    if (vm.FUET_IN_ENCERRA == 1)
                    {
                        lista = lista.Where(p => p.FUET_IN_ENCERRA == 1 & p.FUET_CD_ID != id).ToList();
                        if (lista.Count > 0)
                        {
                            Session["MensFunil"] = 8;
                            return RedirectToAction("EditarEtapa");
                        }
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FUNIL_ETAPA item = Mapper.Map<FunilEtapaViewModel, FUNIL_ETAPA>(vm);
                    Int32 volta = baseApp.ValidateEditEtapa(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirEtapa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUET_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditEtapa(item);
                return RedirectToAction("VoltarAnexoFunil");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEtapa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUET_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditEtapa(item);
                return RedirectToAction("VoltarAnexoFunil");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirEtapa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagens
            if (Session["MensFunil"] != null)
            {
                if ((Int32)Session["MensFunil"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0196", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 8)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFunil"] == 9)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0199", CultureInfo.CurrentCulture));
                }
            }
            Session["MensFunil"] = null;

            // Recupera ultima etapa
            FUNIL funil = baseApp.GetItemById((Int32)Session["IdFunil"]);
            Int32? ordem = 1;
            if (funil.FUNIL_ETAPA.Count > 0)
            {
                ordem = funil.FUNIL_ETAPA.OrderByDescending(p => p.FUET_IN_ORDEM).FirstOrDefault().FUET_IN_ORDEM;
                ordem++;
            }
            Session["Ordem"] = ordem;

            // Verifica
            CONFIGURACAO conf = confApp.GetItemById(idAss);
            if (ordem > conf.CONF_IN_ETAPAS_CRM)
            {
                Session["MensFunil"] = 5;
                return RedirectToAction("VoltarAnexoFunil");
            }
            Session["OrdemMax"] = conf.CONF_IN_ETAPAS_CRM;

            // Prepara view
            FUNIL_ETAPA item = new FUNIL_ETAPA();
            FunilEtapaViewModel vm = Mapper.Map<FUNIL_ETAPA, FunilEtapaViewModel>(item);
            vm.FUNI_CD_ID = (Int32)Session["IdFunil"];
            vm.FUET_IN_ATIVO = 1;
            vm.FUET_IN_ORDEM = ordem;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEtapa(FunilEtapaViewModel vm)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Valida ordem
                    Int32 ordem = (Int32)Session["Ordem"];
                    Int32 ordemMax = (Int32)Session["OrdemMax"];
                    if (vm.FUET_IN_ORDEM < ordem)
                    {
                        Session["MensFunil"] = 6;
                        return RedirectToAction("IncluirEtapa");
                    }
                    if (vm.FUET_IN_ORDEM > ordemMax)
                    {
                        Session["MensFunil"] = 7;
                        return RedirectToAction("IncluirEtapa");
                    }
                    if (vm.FUET_IN_ORDEM > ordem)
                    {
                        Session["MensFunil"] = 6;
                        return RedirectToAction("IncluirEtapa");
                    }

                    // Valida flags
                    List<FUNIL_ETAPA> lista = baseApp.GetItemById(vm.FUNI_CD_ID).FUNIL_ETAPA.ToList();
                    if (vm.FUET_IN_ENCERRA == 1)
                    {
                        lista = lista.Where(p => p.FUET_IN_ENCERRA == 1).ToList();
                        if (lista.Count > 0)
                        {
                            Session["MensFunil"] = 8;
                            return RedirectToAction("IncluirEtapa");
                        }
                    }
                    //if (vm.FUET_IN_PROPOSTA == 1)
                    //{
                    //    lista = lista.Where(p => p.FUET_IN_PROPOSTA == 1).ToList();
                    //    if (lista.Count > 0)
                    //    {
                    //        Session["MensFunil"] = 9;
                    //        return RedirectToAction("IncluirEtapa");
                    //    }
                    //}

                    // Executa a operação
                    FUNIL_ETAPA item = Mapper.Map<FunilEtapaViewModel, FUNIL_ETAPA>(vm);
                    Int32 volta = baseApp.ValidateCreateEtapa(item);
                    // Verifica retorno
                    return RedirectToAction("IncluirEtapa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public List<FUNIL> CarregaFunil()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<FUNIL> conf = new List<FUNIL>();
            if (Session["Funis"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["FunilAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<FUNIL>)Session["Funis"];
                }
            }
            Session["Funis"] = conf;
            Session["FunilAlterada"] = 0;
            return conf;
        }

    }
}