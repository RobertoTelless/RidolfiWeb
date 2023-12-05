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
using System.Diagnostics;
using ApplicationServices.Services;

namespace ERP_Condominios_Solution.Controllers
{
    public class FunilController : Controller
    {
        private readonly IFunilAppService baseApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ICRMAppService crmApp;
        private readonly ILogAppService logApp;

        private String msg;
        private Exception exception;
        FUNIL objeto = new FUNIL();
        FUNIL objetoAntes = new FUNIL();
        List<FUNIL> listaMaster = new List<FUNIL>();
        String extensao;

        public FunilController(IFunilAppService baseApps, IConfiguracaoAppService confApps, ICRMAppService crmApps, IUsuarioAppService usuApps, LogAppService logApps)
        {
            baseApp = baseApps;
            usuApp = usuApps;
            confApp = confApps;
            crmApp = crmApps;
            logApp = logApps;
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

        [HttpGet]
        public ActionResult MontarTelaFunil()
        {
            try
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if ((List<FUNIL>)Session["ListaFunilX"] == null)
                {
                    listaMaster = CarregaFunil();
                    Session["ListaFunilX"] = listaMaster;
                }
                ViewBag.Listas = (List<FUNIL>)Session["ListaFunilX"];
                ViewBag.Title = "Funil";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

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

                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "accFUNI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Abre view
                Session["MensFunil"] = null;
                Session["VoltaFunil"] = 1;
                Session["TabFunil"] = 1;
                objeto = new FUNIL();
                objeto.FUNI_IN_ATIVO = 1;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funis";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funis", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFunilX"] = null;
            return RedirectToAction("MontarTelaFunil");
        }

        public ActionResult MostrarTudoFunil()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss);
                Session["ListaFunilX"] = listaMaster;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funis";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funis", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFunilX"] = null;
            return RedirectToAction("MontarTelaFunil");
        }

       [HttpGet]
        public ActionResult IncluirFunil()
        {
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
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
                vm.FUNI_IN_CLIENTE = 0;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funis";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funis", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirFunil(FunilViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.FUNI_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUNI_DS_DESCRICAO);
                    vm.FUNI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUNI_NM_NOME);
                    vm.FUNI_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUNI_SG_SIGLA);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
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
                    Session["ListaFunilX"] = null;
                    Session["IdFunil"] = item.FUNI_CD_ID;
                    Session["FunilAlterada"] = 1;
                    Session["TabFunil"] = 2;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
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
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
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
                if (item.FUNIL_ETAPA.Where(p => p.FUET_IN_ATIVO == 1).ToList().Count < conf.CONF_IN_ETAPAS_CRM)
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
            catch (Exception ex)
            {
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

        [HttpPost]
        public ActionResult EditarFunil(FunilViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.FUNI_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUNI_DS_DESCRICAO);
                    vm.FUNI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUNI_NM_NOME);
                    vm.FUNI_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUNI_SG_SIGLA);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FUNIL item = Mapper.Map<FunilViewModel, FUNIL>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<FUNIL>();
                    Session["ListaFunilX"] = null;
                    Session["FunilAlterada"] = 1;
                    return RedirectToAction("MontarTelaFunil");
                }
                catch (Exception ex)
                {
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
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
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
                Session["ListaFunilX"] = null;
                Session["FunilAlterada"] = 1;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
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
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                FUNIL item = baseApp.GetItemById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUNI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                listaMaster = new List<FUNIL>();
                Session["ListaFunilX"] = null;
                Session["FunilAlterada"] = 1;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
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
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
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
                Int32? ordem = 1;
                List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.Where(p => p.FUET_IN_ATIVO == 1).ToList();
                if (etapas.Count > 0)
                {
                    ordem = etapas.OrderByDescending(p => p.FUET_IN_ORDEM).FirstOrDefault().FUET_IN_ORDEM;
                }
                Session["Ordem"] = ordem;
                Session["Etapas"] = etapas;

                // Verifica
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                if (etapas.Count > conf.CONF_IN_ETAPAS_CRM)
                {
                    Session["MensFunil"] = 5;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                Session["OrdemMax"] = conf.CONF_IN_ETAPAS_CRM;

                // Prepara view
                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                FunilEtapaViewModel vm = Mapper.Map<FUNIL_ETAPA, FunilEtapaViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEtapa(FunilEtapaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

                    // Sanitização
                    vm.FUET_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUET_DS_DESCRICAO);
                    vm.FUET_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUET_NM_NOME);
                    vm.FUET_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUET_SG_SIGLA);

                    // Recupera ordem
                    Int32 ordem = (Int32)Session["Ordem"];
                    Int32 ordemMax = (Int32)Session["OrdemMax"];
                    List<FUNIL_ETAPA> etapas = (List<FUNIL_ETAPA>)Session["Etapas"];
                    Int32? atual = vm.FUET_IN_ORDEM;

                    // Verifica existencia da ordem
                    Int32 flagOrdem = 0;
                    FUNIL_ETAPA etapa = etapas.Find(p => p.FUET_IN_ORDEM == atual);
                    if (etapa != null)
                    {
                        flagOrdem = 1;
                    }

                    // Verifica ultima
                    if (atual > ordem)
                    {
                        Session["MensFunil"] = 6;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

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
                    FUNIL funil = baseApp.GetItemById(item.FUNI_CD_ID);
                    etapas = funil.FUNIL_ETAPA.ToList();
                    Int32 indice = item.FUET_CD_ID;

                    // Rearruma etapas
                    Int32? nova = 0;
                    if (flagOrdem == 1)
                    {
                        etapas = etapas.Where(p => p.FUET_IN_ORDEM >= atual & p.FUET_CD_ID != indice).OrderBy(x => x.FUET_IN_ORDEM).ToList();
                        foreach (FUNIL_ETAPA eta in etapas)
                        {
                            nova = eta.FUET_IN_ORDEM + 1;
                            eta.FUET_IN_ORDEM = nova;
                            Int32 volta1 = GravaOrdem(eta);
                        }
                    }

                    // Verifica retorno
                    Session["TabFunil"] = 2;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
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
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUET_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditEtapa(item);
                Session["TabFunil"] = 2;
                return RedirectToAction("VoltarAnexoFunil");
            }
            catch (Exception ex)
            {
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
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUET_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditEtapa(item);
                return RedirectToAction("VoltarAnexoFunil");
            }
            catch (Exception ex)
            {
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
            try
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
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
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
                List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.Where(p => p.FUET_IN_ATIVO == 1).ToList();
                if (etapas.Count > 0)
                {
                    ordem = etapas.OrderByDescending(p => p.FUET_IN_ORDEM).FirstOrDefault().FUET_IN_ORDEM;
                    ordem++;
                }
                Session["Ordem"] = ordem;
                Session["Etapas"] = etapas;

                // Verifica
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                if (etapas.Count > conf.CONF_IN_ETAPAS_CRM)
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
            catch (Exception ex)
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEtapa(FunilEtapaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.FUET_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUET_DS_DESCRICAO);
                    vm.FUET_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUET_NM_NOME);
                    vm.FUET_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FUET_SG_SIGLA);

                    // Recupera ordem
                    Int32 ordem = (Int32)Session["Ordem"];
                    Int32 ordemMax = (Int32)Session["OrdemMax"];
                    List<FUNIL_ETAPA> etapas = (List<FUNIL_ETAPA>)Session["Etapas"];
                    Int32? atual = vm.FUET_IN_ORDEM;

                    // Verifica existencia da ordem
                    Int32 flagOrdem = 0;
                    FUNIL_ETAPA etapa = etapas.Find(p => p.FUET_IN_ORDEM == atual);
                    if (etapa != null)
                    {
                        flagOrdem = 1;
                    }

                    // Verifica ultima
                    if (atual > ordem)
                    {
                        Session["MensFunil"] = 6;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Valida flags
                    List<FUNIL_ETAPA> lista = baseApp.GetItemById(vm.FUNI_CD_ID).FUNIL_ETAPA.ToList();
                    if (vm.FUET_IN_ENCERRA == 1)
                    {
                        lista = lista.Where(p => p.FUET_IN_ENCERRA == 1).ToList();
                        if (lista.Count > 0)
                        {
                            Session["MensFunil"] = 8;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Executa a operação
                    FUNIL_ETAPA item = Mapper.Map<FunilEtapaViewModel, FUNIL_ETAPA>(vm);
                    Int32 volta = baseApp.ValidateCreateEtapa(item);
                    FUNIL funil = baseApp.GetItemById(item.FUNI_CD_ID);
                    etapas = funil.FUNIL_ETAPA.ToList();
                    Int32 indice = item.FUET_CD_ID;

                    // Rearruma etapas
                    Int32? nova = 0;
                    if (flagOrdem == 1)
                    {
                        etapas = etapas.Where(p => p.FUET_IN_ORDEM >= atual & p.FUET_CD_ID != indice).OrderBy(x => x.FUET_IN_ORDEM).ToList();
                        foreach (FUNIL_ETAPA eta in etapas)
                        {
                            nova = eta.FUET_IN_ORDEM + 1;
                            eta.FUET_IN_ORDEM = nova;
                            Int32 volta1 = GravaOrdem(eta);
                        }
                    }

                    // Verifica retorno
                    Session["TabFunil"] = 2;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
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

        public Int32 GravaOrdem(FUNIL_ETAPA etapa)
        {   
            FUNIL_ETAPA nova = new FUNIL_ETAPA();
            nova.FUET_CD_ID = etapa.FUET_CD_ID;
            nova.FUET_DS_DESCRICAO = etapa.FUET_DS_DESCRICAO;
            nova.FUET_IN_ATIVO = etapa.FUET_IN_ATIVO;
            nova.FUET_IN_EMAIL = etapa.FUET_IN_EMAIL;
            nova.FUET_IN_ENCERRA = etapa.FUET_IN_ENCERRA;
            nova.FUET_IN_ORDEM = etapa.FUET_IN_ORDEM;
            nova.FUET_IN_PROPOSTA = etapa.FUET_IN_PROPOSTA;
            nova.FUET_IN_SMS = etapa.FUET_IN_SMS;
            nova.FUET_NM_NOME = etapa.FUET_NM_NOME;
            nova.FUET_SG_SIGLA = etapa.FUET_SG_SIGLA;
            nova.FUNI_CD_ID = etapa.FUNI_CD_ID;

            Int32 volta = baseApp.ValidateEditEtapa(nova);
            return volta;

        }

        public List<FUNIL> CarregaFunil()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }
    }
}