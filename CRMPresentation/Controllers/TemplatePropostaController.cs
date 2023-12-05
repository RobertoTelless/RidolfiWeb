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
using ERP_Condominios_Solution.Classes;
using System.Diagnostics;

namespace ERP_Condominios_Solution.Controllers
{
    public class TemplatePropostaController : Controller
    {
        private readonly ITemplatePropostaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;

        private String msg;
        private Exception exception;
        TEMPLATE_PROPOSTA objeto = new TEMPLATE_PROPOSTA();
        TEMPLATE_PROPOSTA objetoAntes = new TEMPLATE_PROPOSTA();
        List<TEMPLATE_PROPOSTA> listaMaster = new List<TEMPLATE_PROPOSTA>();
        String extensao;

        public TemplatePropostaController(ITemplatePropostaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
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
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaTemplateProposta()
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
                        Session["ModuloPermissao"] = "Modelos de Proposta";
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];


                // Carrega listas
                if ((List<TEMPLATE_PROPOSTA>)Session["ListaTemplateProposta"] == null)
                {
                    listaMaster = CarregarModeloProposta();
                    Session["ListaTemplateProposta"] = listaMaster;
                }
                ViewBag.Listas = (List<TEMPLATE_PROPOSTA>)Session["ListaTemplateProposta"];
                ViewBag.Title = "TemplateProposta";
                Session["TemplateProposta"] = null;
                Session["IncluirTemplateProposta"] = 0;
                Session["LinhaAlterada"] = 0;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensTemplateProposta"] != null)
                {
                    if ((Int32)Session["MensTemplateProposta"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateProposta"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateProposta"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0191", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateProposta"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0192", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateProposta"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }

                // Abre view
                Session["VoltaTemplateProposta"] = 1;
                objeto = new TEMPLATE_PROPOSTA();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo Proposta";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTemplateProposta()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaTemplateProposta"] = null;
                return RedirectToAction("MontarTelaTemplateProposta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo Proposta";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoTemplateProposta()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaTemplateProposta"] = listaMaster;
                return RedirectToAction("MontarTelaTemplateProposta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo Proposta";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTemplateProposta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTemplateProposta");
        }

        [HttpPost]
        public ActionResult FiltrarTemplateProposta(TEMPLATE_PROPOSTA item)
        {

            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<TEMPLATE_PROPOSTA> listaObj = new List<TEMPLATE_PROPOSTA>();
                Session["FiltroCustoFixo"] = item;
                Tuple<Int32, List<TEMPLATE_PROPOSTA>, Boolean> volta = baseApp.ExecuteFilter(item.TEPR_SG_SIGLA, item.TEPR_NM_NOME, item.TEPR_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensTemplateProposta"] = 1;
                    return RedirectToAction("MontarTelaTemplateProposta");
                }

                // Sucesso
                Session["MensTemplateEMail"] = 0;
                listaMaster = volta.Item2;
                Session["ListaCustoFixo"] = volta.Item2;
                return RedirectToAction("MontarTelaTemplateProposta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos Propostas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirTemplateProposta()
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
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Inclusão";
                        return RedirectToAction("MontarTelaTemplateProposta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                List<SelectListItem> ativo = new List<SelectListItem>();
                ativo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ativo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Proposta", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "E-Mail de Envio", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

                // Prepara view
                TEMPLATE_PROPOSTA item = new TEMPLATE_PROPOSTA();
                TemplatePropostaViewModel vm = Mapper.Map<TEMPLATE_PROPOSTA, TemplatePropostaViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.TEPR_IN_ATIVO = 1;
                vm.TEPR_IN_FIXO = 0;
                vm.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos Propostas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplateProposta(TemplatePropostaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    List<SelectListItem> ativo = new List<SelectListItem>();
                    ativo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                    ativo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                    ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
                    List<SelectListItem> tipo = new List<SelectListItem>();
                    tipo.Add(new SelectListItem() { Text = "Proposta", Value = "1" });
                    tipo.Add(new SelectListItem() { Text = "E-Mail de Envio", Value = "2" });
                    ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

                    // Preparação
                    TEMPLATE_PROPOSTA item = Mapper.Map<TemplatePropostaViewModel, TEMPLATE_PROPOSTA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTemplateProposta"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0191", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMaster = new List<TEMPLATE_PROPOSTA>();
                    Session["ListaTemplateProposta"] = null;
                    Session["IdTemplateProposta"] = item.TEPR_CD_ID;
                    Session["TemplatePropostaAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TEPR_CD_ID;
                    return RedirectToAction("MontarTelaTemplateProposta");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Modelos Propostas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTemplateProposta(Int32 id)
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
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Edição";
                        return RedirectToAction("MontarTelaTemplateProposta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                TEMPLATE_PROPOSTA item = baseApp.GetItemById(id);
                Session["TemplateProposta"] = item;

                // Indicadores
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Proposta", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "E-Mail de Envio", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

                // Mensagens
                if (Session["MensTemplateProposta"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensTemplateProposta"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                }

                Session["VoltaTemplateProposta"] = 1;
                objetoAntes = item;
                Session["IdTemplateProposta"] = id;
                TemplatePropostaViewModel vm = Mapper.Map<TEMPLATE_PROPOSTA, TemplatePropostaViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos Propostas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplateProposta(TemplatePropostaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<SelectListItem> tipo = new List<SelectListItem>();
                    tipo.Add(new SelectListItem() { Text = "Proposta", Value = "1" });
                    tipo.Add(new SelectListItem() { Text = "E-Mail de Envio", Value = "2" });
                    ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TEMPLATE_PROPOSTA item = Mapper.Map<TemplatePropostaViewModel, TEMPLATE_PROPOSTA>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Sucesso
                    listaMaster = new List<TEMPLATE_PROPOSTA>();
                    Session["ListaTemplateProposta"] = null;
                    Session["TemplatePropostaAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TEPR_CD_ID;
                    return RedirectToAction("MontarTelaTemplateProposta");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Templates Propostas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Templates Propostas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


        public ActionResult VoltarAnexoTemplateProposta()
        {

            return RedirectToAction("EditarTemplateProposta", new { id = (Int32)Session["IdTemplateProposta"] });
        }

        [HttpGet]
        public ActionResult ExcluirTemplateProposta(Int32 id)
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
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Exclusão";
                        return RedirectToAction("MontarTelaTemplateProposta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                TEMPLATE_PROPOSTA item = baseApp.GetItemById(id);
                objetoAntes = (TEMPLATE_PROPOSTA)Session["TemplateProposta"];
                item.TEPR_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensTemplateProposta"] = 4;
                    return RedirectToAction("MontarTelaTemplateProposta");
                }

                Session["ListaTemplateProposta"] = null;
                Session["TemplatePropostaAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateProposta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates Propostas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates Propostas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTemplateProposta(Int32 id)
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
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Reativação";
                        return RedirectToAction("MontarTelaTemplateProposta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                TEMPLATE_PROPOSTA item = baseApp.GetItemById(id);
                objetoAntes = (TEMPLATE_PROPOSTA)Session["TemplateProposta"];
                item.TEPR_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                Session["ListaTemplateProposta"] = null;
                Session["TemplatePropostaAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateProposta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos Proposta";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerTemplateProposta(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                Session["IdTemplateProposta"] = id;
                TEMPLATE_PROPOSTA item = baseApp.GetItemById(id);
                TemplatePropostaViewModel vm = Mapper.Map<TEMPLATE_PROPOSTA, TemplatePropostaViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos Proposta";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<EMPRESA> CarregaEmpresa()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<EMPRESA> conf = new List<EMPRESA>();
                if (Session["Empresas"] == null)
                {
                    conf = empApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["EmpresaAlterada"] == 1)
                    {
                        conf = empApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<EMPRESA>)Session["Empresas"];
                    }
                }
                Session["Empresas"] = conf;
                Session["EmpresaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos Propostas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TEMPLATE_PROPOSTA> CarregarModeloProposta()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_PROPOSTA> conf = new List<TEMPLATE_PROPOSTA>();
                if (Session["ModeloPropostas"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ModeloPropostaAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TEMPLATE_PROPOSTA>)Session["ModeloPropostas"];
                    }
                }
                Session["ModeloPropostaAlterada"] = 0;
                Session["ModeloPropostas"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos Propostas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }
    }
}