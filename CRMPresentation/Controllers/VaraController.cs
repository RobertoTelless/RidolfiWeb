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
using System.Net.Mail;
using System.Net.Http;
using HtmlAgilityPack;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using Azure.Communication.Email;
using System.Net.Mime;
using ERP_Condominios_Solution.Classes;
using iText.IO.Util;

namespace ERP_Condominios_Solution.Controllers
{
    public class VaraController : Controller
    {
        private readonly IVaraAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        VARA objetoForn = new VARA();
        VARA objetoFornAntes = new VARA();
        List<VARA> listaMasterForn = new List<VARA>();
        String extensao;

        public VaraController(IVaraAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
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


        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaVARA()
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
                    return RedirectToAction("MontarTelaDashboardCadastro", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaVara"] == null)
            {
                listaMasterForn = CarregaVara();
                Session["ListaVara"] = listaMasterForn;
            }
            ViewBag.Listas = (List<VARA>)Session["ListaVara"];
            ViewBag.Title = "Vara";
            ViewBag.TRF = new SelectList(CarregaTRF(), "TRF1_CD_ID", "TRF1_NM_NOME");
            Session["VoltarVara"] = 2;

            // Indicadores
            ViewBag.Vara = ((List<VARA>)Session["ListaVara"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensVara"] != null)
            {
                if ((Int32)Session["MensVara"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensVara"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0272", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensVara"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0273", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new VARA();
            objetoForn.VARA_IN_ATIVO = 1;
            Session["MensVara"] = 0;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroVara()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaVara"] = null;
            Session["FiltroVara"] = null;
            return RedirectToAction("MontarTelaVara");
        }

        public ActionResult MostrarTudoVara()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm();
            Session["FiltroVara"] = null;
            Session["ListaVara"] = listaMasterForn;
            return RedirectToAction("MontarTelaVara");
        }

        [HttpPost]
        public ActionResult FiltrarVara(VARA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TRF> listaObj = new List<TRF>();
                Session["FiltroVara"] = item;
                Tuple<Int32, List<VARA>, Boolean> volta = fornApp.ExecuteFilterTuple(item.VARA_NM_NOME, item.TRF1_CD_ID, 1);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensVara"] = 1;
                    return RedirectToAction("MontarTelaVara");
                }

                // Sucesso
                listaMasterForn = volta.Item2;
                Session["ListaVara"] = listaMasterForn;
                return RedirectToAction("MontarTelaVara");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Vara";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Vara", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseVara()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltarVara"] == 2)
            {
                return RedirectToAction("MontarTelaVara");
            }
            if ((Int32)Session["VoltarVara"] == 1)
            {
                return RedirectToAction("VoltarAnexoTRF", "TRF");
            }
            return RedirectToAction("MontarTelaVara");
        }

        [HttpGet]
        public ActionResult IncluirVara()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.TRF = new SelectList(CarregaTRF(), "TRF1_CD_ID", "TRF1_NM_NOME");

            // Prepara view
            VARA item = new VARA();
            VaraViewModel vm = Mapper.Map<VARA, VaraViewModel>(item);
            vm.VARA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirVara(VaraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TRF = new SelectList(CarregaTRF(), "TRF1_CD_ID", "TRF1_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    VARA item = Mapper.Map<VaraViewModel, VARA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensVara"] = 3;
                        return RedirectToAction("MontarTelaVara", "Vara");
                    }

                    // Sucesso
                    listaMasterForn = new List<VARA>();
                    Session["ListaVara"] = null;
                    Session["IdVolta"] = item.VARA_CD_ID;
                    if ((Int32)Session["VoltarVara"] == 2)
                    {
                        return RedirectToAction("MontarTelaVara");
                    }
                    if ((Int32)Session["VoltarVara"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoTRF", "TRF");
                    }
                    return RedirectToAction("MontarTelaVara");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Vara";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Vara", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarVara(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensVara"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.TRF = new SelectList(CarregaTRF(), "TRF1_CD_ID", "TRF1_NM_NOME");

            VARA item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Vara"] = item;
            Session["IdVolta"] = id;
            Session["IdVara"] = id;
            VaraViewModel vm = Mapper.Map<VARA, VaraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarVara(VaraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TRF = new SelectList(CarregaTRF(), "TRF1_CD_ID", "TRF1_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    VARA item = Mapper.Map<VaraViewModel, VARA>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<VARA>();
                    Session["ListaVara"] = null;
                    if ((Int32)Session["VoltarVara"] == 2)
                    {
                        return RedirectToAction("MontarTelaVara");
                    }
                    if ((Int32)Session["VoltarVara"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoTRF", "TRF");
                    }
                    return RedirectToAction("MontarTelaVara");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Vara";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Vara", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerVara(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirForn"];

            VARA item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Vara"] = item;
            Session["IdVolta"] = id;
            Session["IdVara"] = id;
            VaraViewModel vm = Mapper.Map<VARA, VaraViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirVara(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                VARA item = fornApp.GetItemById(id);
                item.VARA_IN_ATIVO = 0;
                Int32 volta = fornApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensVara"] = 5;
                    return RedirectToAction("MontarTelaVara", "Vara");
                }
                listaMasterForn = new List<VARA>();
                Session["ListaVara"] = null;
                Session["FiltroVara"] = null;
                return RedirectToAction("MontarTelaVara");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Vara";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Vara", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarVara(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                VARA item = fornApp.GetItemById(id);
                item.VARA_IN_ATIVO = 1;
                Int32 volta = fornApp.ValidateReativar(item, usuario);
                listaMasterForn = new List<VARA>();
                Session["ListaVara"] = null;
                Session["FiltroVara"] = null;
                return RedirectToAction("MontarTelaVara");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Vara";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Vara", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<TRF> CarregaTRF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TRF> conf = new List<TRF>();
            if (Session["TRFs"] == null)
            {
                conf = fornApp.GetAllTRF();
            }
            else
            {
                if ((Int32)Session["TRFAlterada"] == 1)
                {
                    conf = fornApp.GetAllTRF();
                }
                else
                {
                    conf = (List<TRF>)Session["TRFs"];
                }
            }
            Session["TRFs"] = conf;
            Session["TRFAlterada"] = 0;
            return conf;
        }

        public List<VARA> CarregaVara()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<VARA> conf = new List<VARA>();
            if (Session["Varas"] == null)
            {
                conf = fornApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["VaraAlterada"] == 1)
                {
                    conf = fornApp.GetAllItens();
                }
                else
                {
                    conf = (List<VARA>)Session["Varas"];
                }
            }
            Session["Varas"] = conf;
            Session["VaraAlterada"] = 0;
            return conf;
        }
    }
}