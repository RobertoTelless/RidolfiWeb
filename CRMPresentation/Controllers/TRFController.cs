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
    public class TRFController : Controller
    {
        private readonly ITRFAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        TRF objetoForn = new TRF();
        TRF objetoFornAntes = new TRF();
        List<TRF> listaMasterForn = new List<TRF>();
        String extensao;

        public TRFController(ITRFAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps)
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

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaTRF()
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
            if (Session["ListaTRF"] == null)
            {
                listaMasterForn = CarregaTRF();
                Session["ListaTRF"] = listaMasterForn;
            }
            ViewBag.Listas = (List<TRF>)Session["ListaTRF"];
            ViewBag.Title = "TRF";
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["IncluirTRF"] = 0;
            Session["VoltarVara"] = 1;

            // Indicadores
            ViewBag.TRF = ((List<TRF>)Session["ListaTRF"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensTRF"] != null)
            {
                if ((Int32)Session["MensTRF"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTRF"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0270", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTRF"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0271", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new TRF();
            objetoForn.TRF1_IN_ATIVO = 1;
            Session["MensTRF"] = 0;
            Session["VoltaTRF"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroTRF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaTRF"] = null;
            Session["FiltroTRF"] = null;
            return RedirectToAction("MontarTelaTRF");
        }

        public ActionResult MostrarTudoTRF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm();
            Session["FiltroTRF"] = null;
            Session["ListaTRF"] = listaMasterForn;
            return RedirectToAction("MontarTelaTRF");
        }

        [HttpPost]
        public ActionResult FiltrarTRF(TRF item)
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
                Session["FiltroTRF"] = item;
                Tuple<Int32, List<TRF>, Boolean> volta = fornApp.ExecuteFilterTuple(item.TRF1_NM_NOME, item.UF_CD_ID, 1);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensTRF"] = 1;
                    return RedirectToAction("MontarTelaTRF");
                }

                // Sucesso
                listaMasterForn = volta.Item2;
                Session["ListaTRF"] = listaMasterForn;
                Session["TipoCarga"] = 2;
                return RedirectToAction("MontarTelaTRF");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "TRF";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "TRF", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTRF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaTRF");
        }

        [HttpGet]
        public ActionResult IncluirTRF()
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
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");

            // Prepara view
            TRF item = new TRF();
            TRFViewModel vm = Mapper.Map<TRF, TRFViewModel>(item);
            vm.TRF1_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirTRF(TRFViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TRF item = Mapper.Map<TRFViewModel, TRF>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTRF"] = 3;
                        return RedirectToAction("MontarTelaTRF", "TRF");
                    }

                    // Sucesso
                    listaMasterForn = new List<TRF>();
                    Session["ListaTRF"] = null;
                    Session["IdVolta"] = item.TRF1_CD_ID;
                    return RedirectToAction("MontarTelaTRF");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "TRF";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "TRF", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTRF(Int32 id)
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
                    Session["MensTRF"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");

            TRF item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["TRF"] = item;
            Session["IdVolta"] = id;
            Session["IdTRF"] = id;
            TRFViewModel vm = Mapper.Map<TRF, TRFViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTRF(TRFViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TRF item = Mapper.Map<TRFViewModel, TRF>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<TRF>();
                    Session["ListaTRF"] = null;
                    return RedirectToAction("MontarTelaTRF");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "TRF";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "TRF", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTRF(Int32 id)
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

            TRF item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["TRF"] = item;
            Session["IdVolta"] = id;
            Session["IdTRF"] = id;
            TRFViewModel vm = Mapper.Map<TRF, TRFViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirTRF(Int32 id)
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
                TRF item = fornApp.GetItemById(id);
                item.TRF1_IN_ATIVO = 0;
                Int32 volta = fornApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensTRF"] = 5;
                    return RedirectToAction("MontarTelaTRF", "TRF");
                }
                listaMasterForn = new List<TRF>();
                Session["ListaTRF"] = null;
                Session["FiltroTRF"] = null;
                return RedirectToAction("MontarTelaTRF");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "TRF";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "TRF", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTRF(Int32 id)
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
                TRF item = fornApp.GetItemById(id);
                item.TRF1_IN_ATIVO = 1;
                Int32 volta = fornApp.ValidateReativar(item, usuario);
                listaMasterForn = new List<TRF>();
                Session["ListaTRF"] = null;
                Session["FiltroTRF"] = null;
                return RedirectToAction("MontarTelaTRF");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "TRF";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "TRF", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult EditarVara(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltarVara"] = 1;
            return RedirectToAction("EditarVara", "Vara", new { id = id });
        }

        public ActionResult IncluirVara()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltarVara"] = 1;
            return RedirectToAction("IncluirVara", "Vara");
        }

        public ActionResult VoltarAnexoTRF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            return RedirectToAction("EditarTRF", new { id = (Int32)Session["IdTRF"] });
        }

        public List<TRF> CarregaTRF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TRF> conf = new List<TRF>();
            if (Session["TRFs"] == null)
            {
                conf = fornApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["TRFAlterada"] == 1)
                {
                    conf = fornApp.GetAllItens();
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

        public List<UF> CarregaUF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<UF> conf = new List<UF>();
            if (Session["UF"] == null)
            {
                conf = fornApp.GetAllUF();
            }
            else
            {
                conf = (List<UF>)Session["UF"];
            }
            Session["UF"] = conf;
            return conf;
        }
    }
}