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
using log4net.Config;
using log4net.Core;
using Common.Logging;
using System.Diagnostics;

namespace ERP_Condominios_Solution.Controllers
{
    public class TemplateEMailController : Controller
    {
        private readonly ITemplateEMailAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;

        private String msg;
        private Exception exception;
        TEMPLATE_EMAIL objeto = new TEMPLATE_EMAIL();
        TEMPLATE_EMAIL objetoAntes = new TEMPLATE_EMAIL();
        List<TEMPLATE_EMAIL> listaMaster = new List<TEMPLATE_EMAIL>();
        String extensao;

        public TemplateEMailController(ITemplateEMailAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps)
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
            return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
        }

        [HttpGet]
        public ActionResult MontarTelaTemplateEMail()
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
                        Session["ModuloPermissao"] = "Modelos de Mensagens";
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if ((List<TEMPLATE_EMAIL>)Session["ListaTemplateEMail"] == null)
                {
                    listaMaster = CarregarModeloEMail();
                    Session["ListaTemplateEMail"] = listaMaster;
                }
                ViewBag.Listas = (List<TEMPLATE_EMAIL>)Session["ListaTemplateEMail"];
                Session["TemplateEMail"] = null;
                Session["IncluirTemplateEMail"] = 0;
                Session["LinhaAlterada"] = 0;

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensTemplateEMail"] != null)
                {
                    if ((Int32)Session["MensTemplateEMail"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0218", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0219", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 11)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0254", CultureInfo.CurrentCulture) + " - " + (String)Session["NomeImagem"];
                        ModelState.AddModelError("", frase);
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }

                // Abre view
                Session["MensTemplateEMail"] = null;
                Session["VoltaTemplateEMail"] = 1;
                objeto = new TEMPLATE_EMAIL();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTemplateEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaTemplateEMail"] = null;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoTemplateEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.TEEM_IN_FIXO == 0).ToList();
                Session["ListaTemplateEMail"] = listaMaster;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTemplateEMail()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTemplateEMail");
        }

        [HttpPost]
        public ActionResult FiltrarTemplateEMail(TEMPLATE_EMAIL item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<TEMPLATE_EMAIL> listaObj = new List<TEMPLATE_EMAIL>();
                Session["FiltroCustoFixo"] = item;
                Tuple<Int32, List<TEMPLATE_EMAIL>, Boolean> volta = baseApp.ExecuteFilter(item.TEEM_SG_SIGLA, item.TEEM_NM_NOME, item.TEEM_TX_CORPO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensTemplateEMail"] = 1;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }

                // Sucesso
                Session["MensTemplateEMail"] = 0;
                listaMaster = volta.Item2;
                Session["ListaCustoFixo"] = volta.Item2;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult IncluirTemplateEMail()
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
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> imagem = new List<SelectListItem>();
                imagem.Add(new SelectListItem() { Text = "Imagens Externas", Value = "1" });
                imagem.Add(new SelectListItem() { Text = "Imagens Embutidas", Value = "2" });
                ViewBag.Imagem = new SelectList(imagem, "Value", "Text");

                // Prepara view
                Session["MensTemplateEMail"] = null;
                TEMPLATE_EMAIL item = new TEMPLATE_EMAIL();
                TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.TEEM_IN_ATIVO = 1;
                vm.TEEM_IN_FIXO = 0;
                vm.TEEM_IN_HTML = 1;
                vm.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplateEMail(TemplateEMailViewModel vm)
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
                    List<SelectListItem> tipo = new List<SelectListItem>();
                    tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
                    tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
                    ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                    List<SelectListItem> imagem = new List<SelectListItem>();
                    imagem.Add(new SelectListItem() { Text = "Imagens Externas", Value = "1" });
                    imagem.Add(new SelectListItem() { Text = "Imagens Embutidas", Value = "2" });
                    ViewBag.Imagem = new SelectList(imagem, "Value", "Text");

                    // Preparação
                    TEMPLATE_EMAIL item = Mapper.Map<TemplateEMailViewModel, TEMPLATE_EMAIL>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTemplateEMail"] = 3;
                        return View(vm);
                    }

                    // Sucesso
                    listaMaster = new List<TEMPLATE_EMAIL>();
                    Session["ListaTemplateEMail"] = null;
                    Session["IdTemplateEMail"] = item.TEEM_CD_ID;
                    Session["TemplatesEMailAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TEEM_CD_ID;

                    // Trata anexos
                    if (item.TEEM_IN_HTML == 2)
                    {
                        if (Session["FileQueueEMail"] != null)
                        {
                            List<FileQueue> fq = (List<FileQueue>)Session["FileQueueEMail"];

                            foreach (var file in fq)
                            {
                                if (file.Profile == null)
                                {
                                    UploadFileQueueEMail(file);
                                }
                            }
                            Session["FileQueueEMail"] = null;
                        }
                    }
                    return RedirectToAction("MontarTelaTemplateEMail");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Templates E-Mail";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTemplateEMail(Int32 id)
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
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                Session["TemplateEMail"] = item;

                // Indicadores
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

                // Mensagens
                if (Session["MensTemplateEMail"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensTemplateEMail"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                }

                Session["MensTemplateEMail"] = null;
                Session["VoltaTemplateEMail"] = 1;
                objetoAntes = item;
                Session["IdTemplateEMail"] = id;
                TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplateEMail(TemplateEMailViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<SelectListItem> tipo = new List<SelectListItem>();
                    tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
                    tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
                    ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TEMPLATE_EMAIL item = Mapper.Map<TemplateEMailViewModel, TEMPLATE_EMAIL>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Sucesso
                    listaMaster = new List<TEMPLATE_EMAIL>();
                    Session["ListaTemplateEMail"] = null;
                    Session["TemplatesEMailAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TEEM_CD_ID;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Templates E-Mail";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


        public ActionResult VoltarAnexoTemplateEMail()
        {

            return RedirectToAction("EditarTemplateEMail", new { id = (Int32)Session["IdTemplateEMail"] });
        }

        [HttpGet]
        public ActionResult ExcluirTemplateEMail(Int32 id)
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
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                objetoAntes = (TEMPLATE_EMAIL)Session["TemplateEMail"];
                item.TEEM_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensTemplateEMail"] = 4;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }
                Session["ListaTemplateEMail"] = null;
                Session["TemplatesEMailAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTemplateEMail(Int32 id)
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
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                objetoAntes = (TEMPLATE_EMAIL)Session["TemplateEMail"];
                item.TEEM_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                Session["ListaTemplateEMail"] = null;
                Session["TemplatesEMailAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerTemplateEMail(Int32 id)
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

                Session["IdTemplateEMail"] = id;
                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();
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

                queue.Add(f);
            }

            Session["FileQueueEMail"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueEMail(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdTemplateEMail"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensTemplateEMail"] = 5;
                    return RedirectToAction("VoltarBaseTemplateEMail");
                }

                TEMPLATE_EMAIL item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 50)
                {
                    Session["MensTemplateEMail"] = 6;
                    return RedirectToAction("VoltarBaseTemplateEMail");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                extensao = extensao.ToUpper();

                // Monta pasta, se for HTML
                if (extensao == ".HTM" || extensao == ".HTML")
                {
                    String caminho = "Modelos/" + idAss.ToString() + "/";
                    String path = Path.Combine(Server.MapPath(caminho), fileName);
                    System.IO.File.WriteAllBytes(path, file.Contents);

                    // Gravar registro
                    item.TEEM_AQ_ARQUIVO = fileName;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
                }
                else
                {
                    String caminho1 = "Images/" + idAss.ToString() + "/";
                    String path = Path.Combine(Server.MapPath(caminho1), fileName);
                    Tuple<Int32, String, Boolean> existe = CrossCutting.FileSystemLibrary.FileCheckExist(path);
                    if (existe.Item3)
                    {
                        Session["MensTemplateEMail"] = 11;
                        Session["NomeImagem"] = fileName;
                    }
                    else
                    {
                        System.IO.File.WriteAllBytes(path, file.Contents);
                    }
                }
                return RedirectToAction("VoltarBaseTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TEMPLATE_EMAIL> CarregarModeloEMail()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_EMAIL> conf = new List<TEMPLATE_EMAIL>();
                if (Session["ModeloEMails"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ModeloEMailAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TEMPLATE_EMAIL>)Session["ModeloEMails"];
                    }
                }
                Session["ModeloEMailAlterada"] = 0;
                Session["ModeloEMails"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos E-Mails";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

    }
}