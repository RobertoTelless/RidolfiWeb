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
using System.Diagnostics;


namespace ERP_Condominios_Solution.Controllers
{
    public class CRMController : Controller
    {
        private readonly ICRMAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IAgendaAppService ageApp;
        private readonly IClienteAppService cliApp;
        private readonly ITemplateEMailAppService temaApp;
        private readonly ITemplateAppService tempApp;
        private readonly ICRMDiarioAppService diaApp;
        private readonly IFunilAppService funApp;
        private readonly ITemplatePropostaAppService tpApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IEmpresaAppService empApp;
        private readonly IMotivoCancelamentoAppService mcApp;
        private readonly ITemplateEMailAppService teApp;
        private readonly IMotivoEncerramentoAppService encApp;
        private readonly IRecursividadeAppService recApp;

        private String msg;
        private Exception exception;
        CRM objeto = new CRM();
        CRM objetoAntes = new CRM();
        List<CRM> listaMaster = new List<CRM>();
        CRM_PEDIDO_VENDA objetoVenda = new CRM_PEDIDO_VENDA();
        CRM_PEDIDO_VENDA objetoAntesVenda = new CRM_PEDIDO_VENDA();
        List<CRM_PEDIDO_VENDA> listaMasterVenda = new List<CRM_PEDIDO_VENDA>();
        List<DIARIO_PROCESSO> listaMasterDiario = new List<DIARIO_PROCESSO>();
        DIARIO_PROCESSO objetoDiario = new DIARIO_PROCESSO();
        DIARIO_PROCESSO objetoAntesDiario = new DIARIO_PROCESSO();
        String extensao;

        public CRMController(ICRMAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IAgendaAppService ageApps, IClienteAppService cliApps, ITemplateEMailAppService temaApps, ITemplateAppService tempApps, ICRMDiarioAppService diaApps, IFunilAppService funApps, ITemplatePropostaAppService tpApps, IMensagemEnviadaSistemaAppService meApps, IEmpresaAppService empApps, IMotivoCancelamentoAppService mcApps, ITemplateEMailAppService teApps, IMotivoEncerramentoAppService encApps, IRecursividadeAppService recApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            ageApp = ageApps;
            cliApp = cliApps;
            temaApp = temaApps;
            tempApp = tempApps;
            diaApp = diaApps;
            funApp = funApps;
            tpApp = tpApps;
            meApp = meApps;
            empApp = empApps;
            mcApp = mcApps;
            teApp = teApps;
            encApp = encApps;
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
        public ActionResult MontarTelaCRM()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if ((List<CRM>)Session["ListaCRM"] == null)
                {
                    listaMaster = CarregaCRM();
                    Session["ListaCRM"] = listaMaster;
                }
                Session["CRM"] = null;
                List<CRM> list = (List<CRM>)Session["ListaCRM"];
                list = list.OrderByDescending(p => p.CRM1_DT_CRIACAO).ToList();
                ViewBag.Listas = list;
                ViewBag.Title = "CRM";

                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
                List<SelectListItem> visao = new List<SelectListItem>();
                visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
                visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
                ViewBag.Visao = new SelectList(visao, "Value", "Text");
                //List<SelectListItem> status = new List<SelectListItem>();
                //status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
                //status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
                //status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
                //status.Add(new SelectListItem() { Text = "Em Negociação", Value = "4" });
                //status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
                //ViewBag.Status = new SelectList(status, "Value", "Text");
                List<SelectListItem> adic = new List<SelectListItem>();
                adic.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
                adic.Add(new SelectListItem() { Text = "Arquivado", Value = "2" });
                adic.Add(new SelectListItem() { Text = "Cancelado", Value = "3" });
                adic.Add(new SelectListItem() { Text = "Falhado", Value = "4" });
                adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
                ViewBag.Adic = new SelectList(adic, "Value", "Text");
                List<SelectListItem> fav = new List<SelectListItem>();
                fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Favorito = new SelectList(fav, "Value", "Text");
                List<SelectListItem> temp = new List<SelectListItem>();
                temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
                temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
                temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
                temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
                ViewBag.Temp = new SelectList(temp, "Value", "Text");
                Session["IncluirCRM"] = 0;
                Session["CRMVoltaAtendimento"] = 0;
                Session["VoltaAgenda"] = 11;
                Session["VoltaCRMBase"] = 0;
                Session["LinkAprova"] = null;
                Session["VoltaPedido"] = 2;
                Session["VoltaHistorico"] = 0;
                Session["VoltaTela"] = 0;
                Session["FlagMensagensEnviadas"] = 7;
                Session["VerDia"] = 1;
                Session["LinhaAlterada"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 30)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 31)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 60)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 61)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 62)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 63)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture));
                    }

                    if ((Int32)Session["MensCRM"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0256", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensCRM"] == 101)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensCRM"] == 32)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0366", CultureInfo.CurrentCulture));
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
                    LOG_NM_OPERACAO = "accCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["IdCRM"] = null;
                Session["MensCRM"] = null;
                Session["VoltaCRM"] = 1;
                Session["IncluirCliente"] = 0;
                Session["VoltaPedido"] = 2;
                Session["FlagMensagensEnviadas"] = 6;
                Session["PontoAcao"] = 101;
                objeto = new CRM();
                if (Session["FiltroCRM"] != null)
                {
                    objeto = (CRM)Session["FiltroCRM"];
                }
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirCliente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["VoltaClienteCRM"] = 1;
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("IncluirCliente", "Cliente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaKanbanCRM()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if ((List<CRM>)Session["ListaCRM"] == null)
                {
                    listaMaster = CarregaCRM();
                    Session["ListaCRM"] = listaMaster;
                }
                Session["CRM"] = null;
                ViewBag.Listas = (List<CRM>)Session["ListaCRM"];
                ViewBag.Title = "CRM";
                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
                List<SelectListItem> visao = new List<SelectListItem>();
                visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
                visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
                ViewBag.Visao = new SelectList(visao, "Value", "Text");
                List<SelectListItem> adic = new List<SelectListItem>();
                adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
                adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
                adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
                adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
                adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
                ViewBag.Adic = new SelectList(adic, "Value", "Text");
                List<SelectListItem> fav = new List<SelectListItem>();
                fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Favorito = new SelectList(fav, "Value", "Text");
                List<SelectListItem> temp = new List<SelectListItem>();
                temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
                temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
                temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
                temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
                ViewBag.Temp = new SelectList(temp, "Value", "Text");
                Session["IncluirCRM"] = 0;
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 30)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 31)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 60)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 61)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 62)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 63)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                    }
                }

                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }

                // Recupera etapas do funil






                // Abre view
                MontarLogGeral log = new MontarLogGeral();
                String mensagem = log.MontarLog(19, 1, 0, usuario.USUA_NM_NOME);
                Trace.WriteLine(mensagem);

                Session["IdCRM"] = null;
                Session["VoltaCRM"] = 2;
                Session["IncluirCliente"] = 0;
                objeto = new CRM();
                if (Session["FiltroCRM"] != null)
                {
                    objeto = (CRM)Session["FiltroCRM"];
                }
                return View(objeto);
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        [HttpGet]
        public ActionResult MontarTelaKanbanCRM_Nova(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if ((List<CRM>)Session["ListaCRM"] == null)
                {
                    listaMaster = CarregaCRM();
                    Session["ListaCRM"] = listaMaster;
                }
                Session["CRM"] = null;
                ViewBag.Listas = (List<CRM>)Session["ListaCRM"];
                ViewBag.Title = "CRM";
                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
                List<SelectListItem> visao = new List<SelectListItem>();
                visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
                visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
                ViewBag.Visao = new SelectList(visao, "Value", "Text");
                List<SelectListItem> adic = new List<SelectListItem>();
                adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
                adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
                adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
                adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
                adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
                ViewBag.Adic = new SelectList(adic, "Value", "Text");
                List<SelectListItem> fav = new List<SelectListItem>();
                fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Favorito = new SelectList(fav, "Value", "Text");
                List<SelectListItem> temp = new List<SelectListItem>();
                temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
                temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
                temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
                temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
                ViewBag.Temp = new SelectList(temp, "Value", "Text");
                Session["IncluirCRM"] = 0;

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 30)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 31)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 60)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 61)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 62)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 63)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                    }
                }

                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }

                // Recupera etapas do funil
                FUNIL funil = funApp.GetItemById(id);
                Session["IdFunil"] = id;
                Int32 numEtapas = funil.FUNIL_ETAPA.Count;
                ViewBag.NumEtapas = numEtapas;
                Int32 i = 1;
                String nome = String.Empty;
                foreach (FUNIL_ETAPA item in funil.FUNIL_ETAPA)
                {
                    nome = "Session" + i.ToString();
                    Session[nome] = item.FUET_NM_NOME;
                    i++;
                }

                // Abre view
                Session["IdCRM"] = null;
                Session["VoltaCRM"] = 2;
                Session["IncluirCliente"] = 0;
                Session["FlagMensagensEnviadas"] = 6;
                objeto = new CRM();
                if (Session["FiltroCRM"] != null)
                {
                    objeto = (CRM)Session["FiltroCRM"];
                }
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroCRM()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaCRM"] = null;
                Session["FiltroCRM"] = null;
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTodosCRM()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM> todos = CarregaCRMGeral();
                Session["ListaCRM"] = todos;
                Session["FiltroCRM"] = null;
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarCRM(CRM item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Sanitização
                item.CRM1_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(item.CRM1_NM_NOME);
                item.CRM1_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.CRM1_DS_DESCRICAO);
                item.CRM1_NM_CAMPANHA = CrossCutting.UtilitariosGeral.CleanStringGeral(item.CRM1_NM_CAMPANHA);

                // Executa a operação
                List<CRM> listaObj = new List<CRM>();
                Session["FiltroCRM"] = item;
                Tuple<Int32, List<CRM>, Boolean> volta = baseApp.ExecuteFilter(item.CRM1_IN_STATUS, item.CRM1_DT_CRIACAO, item.CRM1_DT_CANCELAMENTO, item.ORIG_CD_ID, item.CRM1_IN_ATIVO, item.CRM1_NM_NOME, item.CRM1_DS_DESCRICAO, item.CRM1_IN_ESTRELA, item.CRM1_NR_TEMPERATURA, item.FUNI_CD_ID, item.CRM1_NM_CAMPANHA, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensCRM"] = 1;
                    return RedirectToAction("MontarTelaCRM");
                }

                // Sucesso
                Session["MensCRM"] = 0;
                listaMaster = volta.Item2;
                Session["ListaCRM"] = listaMaster;
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCRM"] = null;
            if ((Int32)Session["VoltaCRMBase"] == 99)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["VoltaCRM"] == 2)
            {
                return RedirectToAction("MontarTelaKanbanCRM", "CRM");
            }
            if ((Int32)Session["VoltaCRMBase"] == 90)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            if ((Int32)Session["VoltaCRM"] == 11)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["VoltaCRM"] == 22)
            {
                return RedirectToAction("MontarTelaHistorico", "CRM");
            }
            Session["VoltaCRM"] = 0;
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult ExcluirProcesso(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM item = baseApp.GetItemById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRM1_IN_ATIVO = 2;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensCRM"] = 4;
                    return RedirectToAction("MontarTelaCRM");
                }

                Session["ListaCRM"] = null;
                Session["CRMAlterada"] = 1;
                Session["VoltaTela"] = 0;
                Session["FlagCRM"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarProcesso(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVA_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade
                List<CRM> lista = CarregaCRM();
                lista = lista.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
                Int32 num = lista.Count;
                if ((Int32)Session["NumProc"] <= num)
                {
                    Session["MensCRM"] = 50;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }

                // Processa
                CRM item = baseApp.GetItemById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRM1_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                Session["FlagCRM"] = 1;
                Session["ListaCRM"] = null;
                Session["CRMAlterada"] = 1;
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EstrelaSim(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDITAR_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM item = baseApp.GetItemById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRM1_IN_ESTRELA = 1;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "estCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM>(item),
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["ListaCRM"] = null;
                Session["CRMAlterada"] = 1;
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EstrelaNao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDITAR_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM item = baseApp.GetItemById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRM1_IN_ESTRELA = 0;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "esnCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM>(item),
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["ListaCRM"] = null;
                Session["CRMAlterada"] = 1;
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EncerrarAcao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERACAO_ACAO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_ACAO item = baseApp.GetAcaoById(id);
                item.CRAC_IN_STATUS = 3;
                Int32 volta = baseApp.ValidateEditAcao(item);
                Session["CRMAlterada"] = 1;

                // Gera diario
                CRM crm = baseApp.GetItemById(item.CRM1_CD_ID);
                CLIENTE cli = cliApp.GetItemById(crm.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.CRAC_CD_ID = item.CRAC_CD_ID;
                dia.DIPR_NM_OPERACAO = "Encerramento de Ação";
                dia.DIPR_DS_DESCRICAO = "Encerramento de Ação " + item.CRAC_NM_TITULO + ". Processo: " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "encCRAC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + crm.CRM1_NM_NOME + " - Ação: " + item.CRAC_NM_TITULO,
                    LOG_TX_REGISTRO_ANTES = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioListaCRM()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "ProcessosLista" + "_" + data + ".pdf";
                List<CRM> lista = (List<CRM>)Session["ListaCRM"];
                CRM filtro = (CRM)Session["FiltroCRM"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontO = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.ORANGE);
                Font meuFontP = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Font meuFontE = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
                Font meuFontD = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);
                Font meuFontS = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Processos - Listagem", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 4;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 100f, 70f, 150f, 160f, 100f, 80f, 150f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Processos selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 9;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Favorito", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cliente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Processo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Funil", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Etapa", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Próxima Ação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data Prevista", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Origem", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Situação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CRM item in lista)
                {
                    if (item.CRM1_IN_ESTRELA == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Sim", meuFontP))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ESTRELA == 0)
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFontO))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.CRM1_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    FUNIL funil = funApp.GetItemById(item.FUNI_CD_ID.Value);
                    cell = new PdfPCell(new Paragraph(funil.FUNI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    FUNIL_ETAPA etapa = funil.FUNIL_ETAPA.Where(p => p.FUET_CD_ID == item.CRM1_IN_STATUS).FirstOrDefault();
                    cell = new PdfPCell(new Paragraph(etapa.FUET_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.CRM_ACAO.Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DS_DESCRICAO, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFontP))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    if (item.CRM_ACAO.Count > 0)
                    {
                        if (item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.Date >= DateTime.Today.Date)
                        {
                            cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFontP))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.CRM_ORIGEM.CROR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.CRM1_IN_ATIVO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Arquivado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Cancelado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Falhado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 5)
                    {
                        cell = new PdfPCell(new Paragraph("Sucesso", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Rodapé
                Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                String parametros = String.Empty;
                Int32 ja = 0;
                if (filtro != null)
                {
                    if (filtro.CRM1_IN_STATUS > 0)
                    {
                        if (filtro.CRM1_IN_STATUS == 1)
                        {
                            parametros += "Status: Prospecção";
                        }
                        else if (filtro.CRM1_IN_STATUS == 2)
                        {
                            parametros += "Status: Contato Realizado";
                        }
                        else if (filtro.CRM1_IN_STATUS == 3)
                        {
                            parametros += "Status: Proposta Apresentada";
                        }
                        else if (filtro.CRM1_IN_STATUS == 4)
                        {
                            parametros += "Status: Em Negociação";
                        }
                        else if (filtro.CRM1_IN_STATUS == 5)
                        {
                            parametros += "Status: Encerrado";
                        }
                        ja = 1;
                    }

                    if (filtro.CRM1_DT_CRIACAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Data Início: " + filtro.CRM1_DT_CRIACAO.Value.ToShortDateString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Data Início: " + filtro.CRM1_DT_CRIACAO.Value.ToShortDateString();
                        }
                    }

                    if (filtro.CRM1_DT_CANCELAMENTO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Data Final: " + filtro.CRM1_DT_CANCELAMENTO.Value.ToShortDateString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Data Final: " + filtro.CRM1_DT_CANCELAMENTO.Value.ToShortDateString();
                        }
                    }

                    if (filtro.ORIG_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Origem: " + filtro.CRM_ORIGEM.CROR_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Origem: " + filtro.CRM_ORIGEM.CROR_NM_NOME;
                        }
                    }

                    if (filtro.CRM1_IN_ATIVO > 0)
                    {
                        if (ja == 0)
                        {
                            if (filtro.CRM1_IN_ATIVO == 1)
                            {
                                parametros += "Situação: Ativo";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 2)
                            {
                                parametros += "Situação: Arquivado";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 3)
                            {
                                parametros += "Situação: Cancelado";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 4)
                            {
                                parametros += "Situação: Falhado";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 5)
                            {
                                parametros += "Situação: Sucesso";
                            }
                            ja = 1;
                        }
                        else
                        {
                            if (filtro.CRM1_IN_ATIVO == 1)
                            {
                                parametros += "e Situação: Ativo";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 2)
                            {
                                parametros += "e Situação: Arquivado";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 3)
                            {
                                parametros += "e Situação: Cancelado";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 4)
                            {
                                parametros += "e Situação: Falhado";
                            }
                            else if (filtro.CRM1_IN_ATIVO == 5)
                            {
                                parametros += "e Situação: Sucesso";
                            }
                        }
                    }

                    if (filtro.CRM1_IN_ESTRELA > 0)
                    {
                        if (ja == 0)
                        {
                            if (filtro.CRM1_IN_ESTRELA == 1)
                            {
                                parametros += "Favorito: Sim";
                            }
                            else if (filtro.CRM1_IN_ESTRELA == 0)
                            {
                                parametros += "Favorito: Não";
                            }
                            ja = 1;
                        }
                        else
                        {
                            if (filtro.CRM1_IN_ESTRELA == 1)
                            {
                                parametros += "e Favorito: Sim";
                            }
                            else if (filtro.CRM1_IN_ESTRELA == 0)
                            {
                                parametros += "e Favorito: Não";
                            }
                        }
                    }

                    if (filtro.CRM1_NM_NOME != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Título: " + filtro.CRM1_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Título: " + filtro.CRM1_NM_NOME;
                        }
                    }

                    if (filtro.CRM1_DS_DESCRICAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Geral: " + filtro.CRM1_DS_DESCRICAO;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Geral: " + filtro.CRM1_DS_DESCRICAO;
                        }
                    }
                    if (ja == 0)
                    {
                        parametros = "Nenhum filtro definido.";
                    }
                }
                else
                {
                    parametros = "Nenhum filtro definido.";
                }
                Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirProcessoCRM()
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
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade
                List<CRM> lista = CarregaCRM();
                lista = lista.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
                Int32 num = lista.Count;
                Int32 procBase = (Int32)Session["NumCRM"];
                if ((Int32)Session["NumCRM"] <= num)
                {
                    Session["MensCRM"] = 50;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }

                // Prepara listas
                List<USUARIO> listaTotal = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
                List<SelectListItem> fav = new List<SelectListItem>();
                fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Favorito = new SelectList(fav, "Value", "Text");
                List<SelectListItem> temp = new List<SelectListItem>();
                temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
                temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
                temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
                temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
                ViewBag.Temp = new SelectList(temp, "Value", "Text");
                List<SelectListItem> envio = new List<SelectListItem>();
                envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Envio = new SelectList(fav, "Value", "Text");
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Mensagem
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 22)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0141", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                Session["CRMNovo"] = 0;
                Session["VoltaCliente"] = 8;
                CRM item = new CRM();
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.CRM1_DT_CRIACAO = DateTime.Today.Date;
                vm.CRM1_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.CRM1_IN_STATUS = 1;
                vm.CRM1_IN_ENCERRADO = 0;
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> IncluirProcessoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<USUARIO> listaTotal = CarregaUsuario();
            ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");
            List<SelectListItem> envio = new List<SelectListItem>();
            envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Envio = new SelectList(fav, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRM1_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_NM_NOME);
                    vm.CRM1_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_DS_DESCRICAO);
                    vm.CRM1_NM_CAMPANHA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_NM_CAMPANHA);
                    vm.CRM1_TX_INFORMACOES_GERAIS = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_TX_INFORMACOES_GERAIS);

                    // Verifica Funil
                    if (vm.FUNI_CD_ID == null || vm.FUNI_CD_ID == 0)
                    {
                        FUNIL funil = funApp.GetAllItens(idAss).Where(p => p.FUNI_IN_FIXO == 1).FirstOrDefault();
                        vm.FUNI_CD_ID = funil.FUNI_CD_ID;
                    }                  
                    
                    // Verifica cliente
                    if (vm.CLIE_CD_ID == null || vm.CLIE_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0141", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Recupera etapa
                    FUNIL fun = funApp.GetItemById(vm.FUNI_CD_ID.Value);
                    List<FUNIL_ETAPA> etapas = fun.FUNIL_ETAPA.OrderBy(p => p.FUET_IN_ORDEM).ToList();
                    FUNIL_ETAPA etapa = etapas.First();
                    vm.CRM1_IN_STATUS = etapa.FUET_CD_ID;

                    // Carrega foto e processa alteracao
                    if (vm.CRM1_AQ_IMAGEM == null)
                    {
                        vm.CRM1_AQ_IMAGEM = "~/Images/icone_imagem.jpg";
                    }

                    // Executa a operação
                    vm.CRM1_GU_GUID = Guid.NewGuid().ToString();
                    CLIENTE cli = cliApp.GetItemById(vm.CLIE_CD_ID);
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Cria pasta
                    String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Listas
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;
                    Session["LinhaAlterada"] = item.CRM1_CD_ID;

                    // Processa Anexos
                    if (Session["FileQueueCRM"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCRM"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueCRM(file);
                            }
                            else
                            {
                                UploadFotoQueueCRM(file);
                            }
                        }
                        Session["FileQueueCRM"] = null;
                    }

                    // Recupera responsavel
                    USUARIO usuResp = usuApp.GetItemById(item.USUA_CD_ID.Value);

                    // Monta Texto
                    String info = String.Empty;
                    info = info + "<br />A processo abaixo foi colocado sob sua responsabilidade em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + item.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + cli.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRM1_GU_GUID + "</b><br />";

                    // Gera e-mail
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = usuario.USUA_NM_NOME;
                    mens.ID = usuario.USUA_CD_ID;
                    mens.MODELO = usuResp.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = info;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = "Atribuição de Processo CRM";
                    mens.MENS_NM_CAMPANHA = usuResp.USUA_NM_EMAIL;

                    // Envia mensagem
                    EnvioEMailGeralBase envio1 = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    Int32 voltaX = await envio1.ProcessaEnvioEMailGeral(mens, usuario);

                    // Retorno
                    if ((Int32)Session["VoltaCRM"] == 3)
                    {
                        Session["VoltaCRM"] = 0;
                        Session["CRMAtendimento"] = 0;
                        return RedirectToAction("IncluirProcessoCRM", "CRM");
                    }

                    Session["CRMAtendimento"] = 0;
                    Session["PontoProposta"] = 0;
                    Session["CRMAlterada"] = 1;
                    Session["VoltaTela"] = 0;
                    Session["FlagCRM"] = 1;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
            Session["FileQueueCRM"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueCRM(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdCRM"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 10;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                CRM item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 11;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                CRM_ANEXO foto = new CRM_ANEXO();
                foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.CRAN_DT_ANEXO = DateTime.Today;
                foto.CRAN_IN_ATIVO = 1;
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
                foto.CRAN_IN_TIPO = tipo;
                foto.CRAN_NM_TITULO = fileName;
                foto.CRM1_CD_ID = item.CRM1_CD_ID;

                item.CRM_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "aneCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileCRM(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdCRM"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 10;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                CRM item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 11;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                CRM_ANEXO foto = new CRM_ANEXO();
                foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.CRAN_DT_ANEXO = DateTime.Today;
                foto.CRAN_IN_ATIVO = 1;
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
                foto.CRAN_IN_TIPO = tipo;
                foto.CRAN_NM_TITULO = fileName;
                foto.CRM1_CD_ID = item.CRM1_CD_ID;

                item.CRM_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "aneCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["VoltaTela"] = 4;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoCRMProposta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            return RedirectToAction("EditarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        [HttpPost]
        public ActionResult UploadFileQueueCRMPedido(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdCRMPedido"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 10;
                    if ((Int32)Session["FlagAnexo"] == 2)
                    {
                        return RedirectToAction("VoltarEditarVenda");
                    }
                    return RedirectToAction("VoltarEditarPedidoCRM");
                }

                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 11;
                    if ((Int32)Session["FlagAnexo"] == 2)
                    {
                        return RedirectToAction("VoltarEditarVenda");
                    }
                    return RedirectToAction("VoltarEditarPedidoCRM");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Pedido/" + item.CRPV_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                CRM_PEDIDO_VENDA_ANEXO foto = new CRM_PEDIDO_VENDA_ANEXO();
                foto.CRPA_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.CRPA_DT_ANEXO = DateTime.Today;
                foto.CRPA_IN_ATIVO = 1;
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
                foto.CRPA_IN_TIPO = tipo;
                foto.CRPA_NM_TITULO = fileName;
                foto.CRPV_CD_ID = item.CRPV_CD_ID;

                item.CRM_PEDIDO_VENDA_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEditPedido(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "aneCRPV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarEditarPedidoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileCRMPedido(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdCRMPedido"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 10;
                    if ((Int32)Session["FlagAnexo"] == 2)
                    {
                        return RedirectToAction("VoltarEditarVenda");
                    }
                    return RedirectToAction("VoltarEditarPedidoCRM");
                }

                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 11;
                    if ((Int32)Session["FlagAnexo"] == 2)
                    {
                        return RedirectToAction("VoltarEditarVenda");
                    }
                    return RedirectToAction("VoltarEditarPedidoCRM");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Pedido/" + item.CRPV_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                CRM_PEDIDO_VENDA_ANEXO foto = new CRM_PEDIDO_VENDA_ANEXO();
                foto.CRPA_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.CRPA_DT_ANEXO = DateTime.Today;
                foto.CRPA_IN_ATIVO = 1;
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
                foto.CRPA_IN_TIPO = tipo;
                foto.CRPA_NM_TITULO = fileName;
                foto.CRPV_CD_ID = item.CRPV_CD_ID;

                item.CRM_PEDIDO_VENDA_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEditPedido(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "aneCRPV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarEditarPedidoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            return RedirectToAction("EditarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult MontarTelaVoltaKanban()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaKB"] == 0)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo");
            }
            if ((Int32)Session["VoltaKB"] == 1)
            {
                return RedirectToAction("MontarTelaCRM");
            }
            return RedirectToAction("EditarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarAcompanhamentoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PontoProposta"] == 85)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            if ((Int32)Session["PontoProposta"] == 99)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["VoltaPedido"] == 10)
            {
                return RedirectToAction("VerPedidosUsuarioCRMPrevia", "CRM");
            }
            if ((Int32)Session["VoltaPedido"] == 22)
            {
                return RedirectToAction("MontarTelaHistorico", "CRM");
            }
            Session["VoltaHistorico"] = 0;
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarAcompanhamentoCRMBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarPedidoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 x = (Int32)Session["VoltaPedido"];
            if ((Int32)Session["VoltaPedido"] == 200)
            {
                return RedirectToAction("VerPedidosUsuarioCRM", "CRM");
            }
            if ((Int32)Session["VoltaPedido"] == 1)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            if ((Int32)Session["VoltaPedido"] == 2)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            Session["SegueInclusao"] = 0;
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarPedidoCRM1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 x = (Int32)Session["VoltaPedidoView"];
            if ((Int32)Session["VoltaPedidoView"] == 200)
            {
                return RedirectToAction("VerPedidosUsuarioCRM", "CRM");
            }
            return RedirectToAction("VerPedidosUsuarioCRM", "CRM");
        }

        public ActionResult VoltarAcaoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 x = (Int32)Session["PontoAcao"];
            if ((Int32)Session["PontoAcao"] == 100)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 101)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 91)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["PontoAcao"] == 1)
                {
                return RedirectToAction("VerAcoesUsuarioCRMPrevia");
            }
            if ((Int32)Session["PontoAcao"] == 2)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            if ((Int32)Session["PontoAcao"] == 22)
            {
                return RedirectToAction("MontarTelaHistorico", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 55)
            {
                return RedirectToAction("VerAcoesUsuarioCRM", "CRM");
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarPropostaCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PontoProposta"] == 1)
            {
                return RedirectToAction("VerPropostasUsuarioCRMPrevia");
            }
            if ((Int32)Session["PontoProposta"] == 2)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarEditarPropostaCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaComentProposta"] == 1)
            {
                return RedirectToAction("EditarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 2)
            {
                return RedirectToAction("CancelarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 3)
            {
                return RedirectToAction("ReprovarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 4)
            {
                return RedirectToAction("AprovarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 5)
            {
                return RedirectToAction("EnviarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            return RedirectToAction("EditarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult VoltarEditarPedidoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaComentPedido"] == 1)
            {
                return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 2)
            {
                return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 3)
            {
                return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 4)
            {
                return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 5)
            {
                return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEditarVenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaComentPedido"] == 1)
            {
                return RedirectToAction("EditarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 2)
            {
                return RedirectToAction("CancelarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 3)
            {
                return RedirectToAction("ReprovarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 4)
            {
                return RedirectToAction("AprovarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 5)
            {
                return RedirectToAction("EnviarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            return RedirectToAction("EditarVenda", new { id = (Int32)Session["IdVenda"] });
        }

        public ActionResult VoltarEditarPedidoCRMCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEncerrarProcessoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EncerrarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarCancelarProcessoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CancelarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarCancelarPedido()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEditarPedidoCRMDireto()
        {
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEditarVendaDireto()
        {
            return RedirectToAction("EditarVenda", new { id = (Int32)Session["IdVenda"] });
        }

        [HttpGet]
        public ActionResult VerAnexoCRM(Int32 id)
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
                    if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                    {
                        Session["MensCRM"] = 2;
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                CRM_ANEXO item = baseApp.GetAnexoById(id);
                Session["VoltaTela"] = 4;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoCRMAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                CRM_ANEXO item = baseApp.GetAnexoById(id);
                Session["VoltaTela"] = 4;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoCRMPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                    {
                        Session["MensCRM"] = 2;
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                CRM_PEDIDO_VENDA_ANEXO item = baseApp.GetAnexoPedidoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoCRMPedidoAudio(Int32 id)
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
                    if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                    {
                        Session["MensCRM"] = 2;
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                CRM_PEDIDO_VENDA_ANEXO item = baseApp.GetAnexoPedidoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoQueueCRM(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idUsu = (Int32)Session["IdCRM"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 10;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                CRM item = baseApp.GetItemById(idUsu);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 11;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Checa extensão
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    // Salva arquivo
                    System.IO.File.WriteAllBytes(path, file.Contents);

                    // Gravar registro
                    item.CRM1_AQ_IMAGEM = "~" + caminho + fileName;
                    objeto = item;
                    Int32 volta = baseApp.ValidateEdit(item, objeto);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "fotCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoCRM(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdCRM"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 10;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                CRM item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensCRM"] = 11;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Checa extensão
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    // Salva arquivo
                    file.SaveAs(path);

                    // Gravar registro
                    item.CRM1_AQ_IMAGEM = "~" + caminho + fileName;
                    objeto = item;
                    Int32 volta = baseApp.ValidateEdit(item, objeto);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "fotCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["VoltaTela"] = 4;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public FileResult DownloadCRM(Int32 id)
        {
            try
            {
                CRM_ANEXO item = baseApp.GetAnexoById(id);
                String arquivo = item.CRAN_AQ_ARQUIVO;
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
                Session["VoltaTela"] = 4;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return File(arquivo, contentType, nomeDownload);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public FileResult DownloadCRMPedido(Int32 id)
        {
            try
            {
                CRM_PEDIDO_VENDA_ANEXO item = baseApp.GetAnexoPedidoById(id);
                String arquivo = item.CRPA_AQ_ARQUIVO;
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult CancelarProcessoCRM(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_CANCELA_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                ViewBag.Motivos = new SelectList(CarregaMotivoCancelamento().Where(p => p.MOCA_IN_TIPO == 1).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM item = baseApp.GetItemById(id);
                Session["IdCRM"] = item.CRM1_CD_ID;

                // Checa ações
                Session["TemAcao"] = 0;
                if (item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
                {
                    Session["TemAcao"] = 1;
                }

                // Prepara view
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
                vm.CRM1_DT_CANCELAMENTO = DateTime.Today.Date;
                vm.CRM1_IN_ATIVO = 3;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CancelarProcessoCRM(CRMViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Motivos = new SelectList(CarregaMotivoCancelamento().Where(p => p.MOCA_IN_TIPO == 1).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
                try
                {

                    // Sanitização
                    vm.CRM1_DS_MOTIVO_CANCELAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_DS_MOTIVO_CANCELAMENTO);

                    // Executa a operação
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 30;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 31;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 5)
                    {
                        Session["MensCRM"] = 32;
                        return RedirectToAction("MontarTelaCRM");
                    }

                    // Gera diario
                    CLIENTE cli = cliApp.GetItemById(item.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Cancelamento de Processo";
                    dia.DIPR_DS_DESCRICAO = "Cancelamento de Processo " + item.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Texto
                    MOTIVO_CANCELAMENTO can = mcApp.GetItemById(item.MOCA_CD_ID.Value);
                    String info = String.Empty;
                    info = info + "<br />A processo abaixo foi cancelado pelo responsável em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                    info = info + "Motivo do Cancelamento: <b>" + can.MOCA_NM_NOME + "</b><br />";
                    info = info + "Justificativa do Cancelamento: <b>" + item.CRM1_DS_MOTIVO_CANCELAMENTO + "</b><br />";
                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + item.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + cli.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRM1_GU_GUID + "</b><br />";

                    // Gera e-mail
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = usuario.USUA_NM_NOME;
                    mens.ID = usuario.USUA_CD_ID;
                    mens.MODELO = usuario.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = info;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = "Cancelamento de Processo CRM";
                    mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                    mens.CLIE_CD_ID = item.CLIE_CD_ID;

                    // Envia e-mail
                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    Int32 voltaX = await envio.ProcessaEnvioEMailGeral(mens, usuario);

                    // Retorno
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;
                    Session["CRMAlterada"] = 1;
                    Session["FlagCRM"] = 1;
                    return RedirectToAction("MontarTelaCRM");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EncerrarProcessoChamada()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EncerrarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        [HttpGet]
        public ActionResult ConfirmarContato()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                crm.CRM1_IN_STATUS = 2;
                Int32 volta = baseApp.ValidateEdit(crm, crm);
                Session["CRMAlterada"] = 1;
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ConfirmarElaboracao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                crm.CRM1_IN_STATUS = 1;
                Int32 volta = baseApp.ValidateEdit(crm, crm);
                Session["CRMAlterada"] = 1;
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmarEtapaAnterior()
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
                    if (usuario.PERFIL.PERF_IN_ETAPA_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Volta etapa anterior
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                Int32 etapaAtual = crm.CRM1_IN_STATUS;
                FUNIL funil = funApp.GetItemById(crm.FUNI_CD_ID.Value);
                FUNIL_ETAPA etapa = funil.FUNIL_ETAPA.Where(p => p.FUET_CD_ID == etapaAtual & p.FUET_IN_ATIVO == 1).FirstOrDefault();
                Int32? ordem = etapa.FUET_IN_ORDEM;
                Int32 etapas = (Int32)Session["NumEtapas"];
                if (ordem == 1 || crm.CRM1_DT_ENCERRAMENTO != null)
                {
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }

                Int32 roda = 1;
                Int32? novaEtapa = ordem;
                FUNIL_ETAPA etapaNova = new FUNIL_ETAPA();
                while (roda == 1)
                {
                    novaEtapa = novaEtapa - 1;
                    etapaNova = funil.FUNIL_ETAPA.Where(p => p.FUNI_CD_ID == funil.FUNI_CD_ID & p.FUET_IN_ORDEM == novaEtapa & p.FUET_IN_ATIVO == 1).FirstOrDefault();
                    if (etapaNova == null)
                    {
                        continue;
                    }
                    break;              
                }           
                crm.CRM1_IN_STATUS = etapaNova.FUET_CD_ID;
                Int32 volta = baseApp.ValidateEdit(crm, crm, usuario);

                // Processa 
                if (etapa.FUET_IN_EMAIL == 1)
                {
                    // Monta Texto
                    String info = String.Empty;
                    info = info + "<br />O processo abaixo teve seu status alterado:<br />";
                    info = info + "Status Atual: <b style='color: darkblue'>" + etapaNova.FUET_NM_NOME + "</b><br />";
                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + crm.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + crm.CLIENTE.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + crm.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + crm.CRM1_GU_GUID + "</b><br />";
                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);

                    // envia para cliente
                    if (funil.FUNI_IN_CLIENTE == 1)
                    {
                        CLIENTE clie = (CLIENTE)Session["ClienteBase"];
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.MENS_NM_NOME = "Processo - Mudança de Etapa";
                        vm.ID = crm.CLIE_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 1;
                        vm.MENS_TX_TEXTO = info;
                        vm.CLIE_CD_ID = crm.CLIE_CD_ID;
                        vm.MENS_NM_CAMPANHA = clie.CLIE_NM_EMAIL;

                        Int32 voltaX = await envio.ProcessaEnvioEMailGeral(vm, usuario);
                    }

                    // Envia para responsável
                    if (funil.FUNI_IN_RESPONSAVEL == 1)
                    {
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.MENS_NM_NOME = "Processo - Mudança de Etapa";
                        vm.ID = crm.USUA_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 1;
                        vm.MENS_TX_TEXTO = info;
                        vm.CLIE_CD_ID = crm.CLIE_CD_ID;
                        vm.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;

                        Int32 voltaX = await envio.ProcessaEnvioEMailGeral(vm, usuario);
                    }
                }

                // Gera diario
                CLIENTE cli = cliApp.GetItemById(crm.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = crm.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Mudança de Etapa";
                dia.DIPR_DS_DESCRICAO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME + ". Para " + etapaNova.FUET_NM_NOME;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "etaCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME + ". Para " + etapaNova.FUET_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Retorno
                Session["CRMAlterada"] = 1;
                Session["FlagCRM"] = 1;
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmarEtapaProxima()
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
                    if (usuario.PERFIL.PERF_IN_ETAPA_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa etapa
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                Int32? idFunil = crm.FUNI_CD_ID;
                Int32 etapaAtual = crm.CRM1_IN_STATUS;
                FUNIL funil = funApp.GetItemById(crm.FUNI_CD_ID.Value);
                FUNIL_ETAPA etapa = funil.FUNIL_ETAPA.Where(p => p.FUET_CD_ID == etapaAtual).FirstOrDefault();
                Int32? ordem = etapa.FUET_IN_ORDEM;
                Int32 etapas = (Int32)Session["NumEtapas"];
                if (ordem == etapas)
                {
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }

                Int32 roda = 1;
                Int32? novaEtapa = ordem;
                FUNIL_ETAPA etapaNova = new FUNIL_ETAPA();
                while (roda == 1)
                {
                    novaEtapa = novaEtapa + 1;
                    etapaNova = funil.FUNIL_ETAPA.Where(p => p.FUNI_CD_ID == funil.FUNI_CD_ID & p.FUET_IN_ORDEM == novaEtapa & p.FUET_IN_ATIVO == 1).FirstOrDefault();
                    if (etapaNova == null)
                    {
                        continue;
                    }
                    break;
                }
                crm.CRM1_IN_STATUS = etapaNova.FUET_CD_ID;
                Int32 volta = baseApp.ValidateEdit(crm, crm, usuario);

                // Processa mensagem
                if (etapa.FUET_IN_EMAIL == 1)
                {
                    // Monta Texto
                    String info = String.Empty;
                    info = info + "<br />O processo abaixo teve seu status alterado:<br />";
                    info = info + "Status Atual: <b style='color: darkblue'>" + etapaNova.FUET_NM_NOME + "</b><br />";
                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + crm.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + crm.CLIENTE.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + crm.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + crm.CRM1_GU_GUID + "</b><br />";
                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);

                    // Envia para cliente
                    if (funil.FUNI_IN_CLIENTE == 1)
                    {
                        CLIENTE clie = (CLIENTE)Session["ClienteBase"];
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.MENS_NM_NOME = "Processo - Mudança de Etapa";
                        vm.ID = crm.CLIE_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 1;
                        vm.MENS_TX_TEXTO = info;
                        vm.CLIE_CD_ID = crm.CLIE_CD_ID;
                        vm.MENS_NM_CAMPANHA = clie.CLIE_NM_EMAIL;

                        Int32 voltaX = await envio.ProcessaEnvioEMailGeral(vm, usuario);
                    }

                    // Envia para responsável
                    if (funil.FUNI_IN_RESPONSAVEL == 1)
                    {
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.MENS_NM_NOME = "Processo - Mudança de Etapa";
                        vm.ID = crm.USUA_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 1;
                        vm.MENS_TX_TEXTO = info;
                        vm.CLIE_CD_ID = crm.CLIE_CD_ID;
                        vm.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;

                        Int32 voltaX = await envio.ProcessaEnvioEMailGeral(vm, usuario);
                    }
                }

                // Gera diario
                CLIENTE cli = cliApp.GetItemById(crm.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = crm.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Mudança de Etapa";
                dia.DIPR_DS_DESCRICAO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME + ". Para " + etapaNova.FUET_NM_NOME;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "petCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME + ". Para " + etapaNova.FUET_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Retorno
                Session["CRMAlterada"] = 1;
                Session["VoltaTela"] = 0;
                Session["FlagCRM"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult EditarCR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCRMCR"] = 10;
            return RedirectToAction("EditarCR", "ContaReceber", new { id = id });
        }

        public ActionResult VerCR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCRMCR"] = 10;
            return RedirectToAction("VerCR", "ContaReceber", new { id = id });
        }

        [HttpGet]
        public ActionResult EncerrarProcessoCRM(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ENCERRA_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                ViewBag.Motivos = new SelectList(CarregaMotivoEncerramento().Where(p => p.MOEN_IN_TIPO == 1).OrderBy(p => p.MOEN_NM_NOME), "MOEN_CD_ID", "MOEN_NM_NOME");
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM item = baseApp.GetItemById(id);
                Session["IdCRM"] = item.CRM1_CD_ID;

                // Checa ações
                Session["TemAcao"] = 0;
                if (item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
                {
                    Session["TemAcao"] = 1;
                }

                // Prepara view
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                List<SelectListItem> adic = new List<SelectListItem>();
                adic.Add(new SelectListItem() { Text = "Falhado", Value = "4" });
                adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
                ViewBag.Adic = new SelectList(adic, "Value", "Text");
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
                vm.CRM1_DT_ENCERRAMENTO = DateTime.Today.Date;
                vm.CRM1_IN_ENCERRADO = 1;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EncerrarProcessoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Motivos = new SelectList(CarregaMotivoEncerramento().Where(p => p.MOEN_IN_TIPO == 1).OrderBy(p => p.MOEN_NM_NOME), "MOEN_CD_ID", "MOEN_NM_NOME");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Falhado", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRM1_DS_INFORMACOES_ENCERRAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_DS_INFORMACOES_ENCERRAMENTO);

                    // Executa a operação
                    vm.CRM1_IN_ENCERRADO = 1;
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("MontarTelaCRM");
                    }

                    // Gera diario
                    CLIENTE cli = cliApp.GetItemById(item.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Encerramento de Processo";
                    dia.DIPR_DS_DESCRICAO = "Encerramento de Processo " + item.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Texto
                    MOTIVO_ENCERRAMENTO me = encApp.GetItemById(item.MOEN_CD_ID.Value);
                    String info = String.Empty;
                    info = info + "<br />A processo abaixo foi encerrado pelo responsável em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                    info = info + "Motivo do Encerramento: <b>" + me.MOEN_NM_NOME + "</b><br />";
                    info = info + "<br />Informações do Processo:<br /  >";
                    info = info + "Processo: <b style='color: darkblue'>" + item.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + cli.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + item.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRM1_GU_GUID + "</b><br />";

                    // Gera e-mail
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = usuario.USUA_NM_NOME;
                    mens.ID = usuario.USUA_CD_ID;
                    mens.MODELO = usuario.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = info; 
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = "Encerramento de Processo CRM";
                    mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                    mens.CLIE_CD_ID = item.CLIE_CD_ID;

                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    Int32 voltaX = await envio.ProcessaEnvioEMailGeral(mens, usuario);

                    // Verifica se tem pedido aprovado
                    if (item.CRM1_IN_ENCERRADO == 1)
                    {
                        CRM crm = baseApp.GetItemById(item.CRM1_CD_ID);
                        CRM_PEDIDO_VENDA pedAprov = crm.CRM_PEDIDO_VENDA.Where(p => p.CRPV_IN_STATUS == 5).FirstOrDefault();
                        Int32 idCRM = crm.CRM1_CD_ID;
                        Session["PedAprov"] = pedAprov;
                        Session["Tipo"] = 2;
                    }

                    // Retorno
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;
                    Session["CRMAlterada"] = 1;
                    Session["VoltaTela"] = 0;
                    Session["FlagCRM"] = 1;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("MontarTelaCRM");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        //[HttpGet]
        //public ActionResult EditarProcessoCRM(Int32 id)
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Logout", "ControleAcesso");
        //    }
        //    if ((USUARIO)Session["UserCredentials"] != null)
        //    {
        //        usuario = (USUARIO)Session["UserCredentials"];

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
        //        {
        //            Session["MensCRM"] = 2;
        //            return RedirectToAction("MontarTelaCRM", "CRM");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Logout", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];

        //    // Monta listas
        //    ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
        //    List<SelectListItem> adic = new List<SelectListItem>();
        //    adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
        //    adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
        //    adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
        //    adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
        //    adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
        //    //adic.Add(new SelectListItem() { Text = "Faturamento", Value = "6" });
        //    //adic.Add(new SelectListItem() { Text = "Expedição", Value = "7" });
        //    ViewBag.Adic = new SelectList(adic, "Value", "Text");
        //    List<SelectListItem> fav = new List<SelectListItem>();
        //    fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
        //    fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
        //    ViewBag.Favorito = new SelectList(fav, "Value", "Text");
        //    List<SelectListItem> temp = new List<SelectListItem>();
        //    temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
        //    temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
        //    temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
        //    temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
        //    ViewBag.Temp = new SelectList(temp, "Value", "Text");
        //    Session["VoltaTela"] = 0;
        //    ViewBag.Incluir = (Int32)Session["VoltaTela"];

        //    // Recupera
        //    CRM item = baseApp.GetItemById(id);
        //    Session["CRM"] = item;
        //    ViewBag.Incluir = (Int32)Session["IncluirCRM"];

        //    // Mensagens
        //    if (Session["MensCliente"] != null)
        //    {
        //        if ((Int32)Session["MensCRM"] == 10)
        //        {
        //            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
        //        }
        //        if ((Int32)Session["MensCRM"] == 11)
        //        {
        //            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
        //        }
        //        if ((Int32)Session["MensCRM"] == 50)
        //        {
        //            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0039", CultureInfo.CurrentCulture));
        //        }
        //    }

        //    // Monta view
        //    Session["VoltaCRM1"] = 1;
        //    objetoAntes = item;
        //    Session["IdCRM"] = id;
        //    CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
        //    return View(vm);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult EditarProcessoCRM(CRMViewModel vm)
        //{
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
        //    List<SelectListItem> adic = new List<SelectListItem>();
        //    adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
        //    adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
        //    adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
        //    adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
        //    adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
        //    ViewBag.Adic = new SelectList(adic, "Value", "Text");
        //    List<SelectListItem> fav = new List<SelectListItem>();
        //    fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
        //    fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
        //    ViewBag.Favorito = new SelectList(fav, "Value", "Text");
        //    List<SelectListItem> temp = new List<SelectListItem>();
        //    temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
        //    temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
        //    temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
        //    temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
        //    ViewBag.Temp = new SelectList(temp, "Value", "Text");

        //    // Indicadores
        //    ViewBag.Incluir = (Int32)Session["IncluirCRM"];
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Executa a operação
        //            USUARIO usuario = (USUARIO)Session["UserCredentials"];
        //            CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
        //            Int32 volta = baseApp.ValidateEdit(item, (CRM)Session["CRM"], usuario);

        //            // Verifica retorno
        //            if (volta == 1)
        //            {
        //                Session["MensCRM"] = 60;
        //                return RedirectToAction("MontarTelaCRM");
        //            }
        //            if (volta == 2)
        //            {
        //                Session["MensCRM"] = 61;
        //                return RedirectToAction("MontarTelaCRM");
        //            }
        //            if (volta == 3)
        //            {
        //                Session["MensCRM"] = 62;
        //                return RedirectToAction("MontarTelaCRM");
        //            }
        //            if (volta == 4)
        //            {
        //                Session["MensCRM"] = 63;
        //                return RedirectToAction("MontarTelaCRM");
        //            }

        //            // Sucesso
        //            listaMaster = new List<CRM>();
        //            Session["ListaCRM"] = null;
        //            Session["IncluirCRM"] = 0;
        //            Session["VoltaTela"] = 0;
        //            ViewBag.Incluir = (Int32)Session["VoltaTela"];

        //            if (Session["FiltroCRM"] != null)
        //            {
        //                FiltrarCRM((CRM)Session["FiltroCRM"]);
        //            }
        //            return RedirectToAction("VoltarBaseCRM");
        //        }
        //        catch (Exception ex)
        //        {
        //            LogError(ex.Message);
        //            ViewBag.Message = ex.Message;
        //            Session["TipoVolta"] = 2;
        //            Session["VoltaExcecao"] = "CRM";
        //            Session["Excecao"] = ex;
        //            Session["ExcecaoTipo"] = ex.GetType().ToString();
        //            GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //            Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
        //            return RedirectToAction("TrataExcecao", "BaseAdmin");
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        [HttpGet]
        public ActionResult VisualizarProcessoCRM(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<SelectListItem> temp = new List<SelectListItem>();
                temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
                temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
                temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
                temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
                ViewBag.Temp = new SelectList(temp, "Value", "Text");

                Session["IdCRM"] = id;
                CRM item = baseApp.GetItemById(id);
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);

                List<CRM_ACAO> acoes = item.CRM_ACAO.ToList().OrderByDescending(p => p.CRAC_DT_CRIACAO).ToList();
                CRM_ACAO acao = acoes.Where(p => p.CRAC_IN_STATUS == 1).FirstOrDefault();
                Session["ClienteCRM"] = item.CLIENTE.CLIE_NM_NOME;
                CLIENTE clie = cliApp.GetItemById(item.CRM1_CD_ID);
                Session["ClienteBase"] = clie;
                ViewBag.Acoes = acoes;

                List<CRM_PEDIDO_VENDA> peds = CarregaPedidoVenda().Where(p => p.CRM1_CD_ID == item.CRM1_CD_ID).ToList();
                CRM_PEDIDO_VENDA ped = peds.Where(p => p.CRPV_IN_STATUS == 2 || p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 5).FirstOrDefault();
                CRM_PEDIDO_VENDA pedAprov = peds.Where(p => p.CRPV_IN_STATUS == 5).FirstOrDefault();
                ViewBag.Peds = peds;

                // Recupera dados do funil
                FUNIL funil = funApp.GetItemById(item.FUNI_CD_ID.Value);
                Session["TemProposta"] = funil.FUNI_IN_PROPOSTA;
                Session["Funil"] = funil.FUNI_NM_NOME;
                List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.ToList();
                ViewBag.Etapas = etapas.Count;
                Session["NumEtapas"] = etapas.Count;

                Int32 atual = item.CRM1_IN_STATUS;
                FUNIL_ETAPA etapaAtual = etapas.Where(p => p.FUET_IN_ORDEM == atual).FirstOrDefault();
                String nomeEtapa = etapaAtual.FUET_NM_NOME;
                ViewBag.NomeEtapa = nomeEtapa;
                Session["EtapaAtual"] = atual;
                ViewBag.EtapaProposta = etapaAtual.FUET_IN_PROPOSTA;

                Int32 encerra = etapaAtual.FUET_IN_ENCERRA;
                ViewBag.Encerra = encerra;
                Int32? etapaEncerra = etapas.Where(p => p.FUET_IN_ENCERRA == 1).FirstOrDefault().FUET_IN_ORDEM;
                Session["EtapaEncerra"] = etapaEncerra;
                ViewBag.EtapaEncerra = etapaEncerra;

                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_CONTATO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                List<SelectListItem> princ = new List<SelectListItem>();
                princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Principal = new SelectList(princ, "Value", "Text");
                Session["VoltaTela"] = 5;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                CRM_CONTATO item = baseApp.GetContatoById(id);
                objetoAntes = (CRM)Session["CRM"];
                Session["Contato"] = item;
                CRMContatoViewModel vm = Mapper.Map<CRM_CONTATO, CRMContatoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(CRMContatoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    List<SelectListItem> princ = new List<SelectListItem>();
                    princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                    princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
                    ViewBag.Principal = new SelectList(princ, "Value", "Text");

                    // Sanitização
                    vm.CRCO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRCO_NM_NOME);
                    vm.CRCO_NM_CARGO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRCO_NM_CARGO);
                    vm.CRCO_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.CRCO_NM_EMAIL);
                    vm.CRCO_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.CRCO_NR_TELEFONE);
                    vm.CRCO_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.CRCO_NR_CELULAR);

                    // Checa principal
                    CRM_CONTATO cont = (CRM_CONTATO)Session["Contato"];
                    if (cont.CRCO_IN_PRINCIPAL == 0)
                    {
                        if (((CRM)Session["CRM"]).CRM_CONTATO.Where(p => p.CRCO_IN_PRINCIPAL == 1).ToList().Count > 0 & vm.CRCO_IN_PRINCIPAL == 1)
                        {
                            Session["MensCRM"] = 50;
                            return RedirectToAction("VoltarAcompanhamentoCRM");
                        }
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_CONTATO item = Mapper.Map<CRMContatoViewModel, CRM_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateEditContato(item);


                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ecoCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.CRCO_NM_NOME,
                        LOG_TX_REGISTRO_ANTES = null,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Retorno
                    Session["CRMAlterada"] = 1;
                    Session["VoltaTela"] = 5;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_CONTATO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_CONTATO item = baseApp.GetContatoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRCO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditContato(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xcoCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = item.CRCO_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Retorno
                Session["CRMAlterada"] = 1;
                Session["VoltaTela"] = 5;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_CONTATO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_CONTATO item = baseApp.GetContatoById(id);

                // Checa principal
                CRM crm = (CRM)Session["CRM"];
                if (crm.CRM_CONTATO != null)
                {
                    if (((CRM)Session["CRM"]).CRM_CONTATO.Where(p => p.CRCO_IN_PRINCIPAL == 1 & p.CRCO_IN_ATIVO == 1).ToList().Count > 0 & item.CRCO_IN_PRINCIPAL == 1)
                    {
                        Session["MensCRM"] = 50;
                        Session["VoltaTela"] = 5;
                        ViewBag.Incluir = (Int32)Session["VoltaTela"];
                        return RedirectToAction("VoltarAcompanhamentoCRMBase");
                    }
                }

                objetoAntes = (CRM)Session["CRM"];
                item.CRCO_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditContato(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "rcoCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = item.CRCO_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Retorno
                Session["CRMAlterada"] = 1;
                Session["VoltaTela"] = 5;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirContato()
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
                    if (usuario.PERFIL.PERF_IN_CONTATO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                List<SelectListItem> princ = new List<SelectListItem>();
                princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Principal = new SelectList(princ, "Value", "Text");

                CRM_CONTATO item = new CRM_CONTATO();
                CRMContatoViewModel vm = Mapper.Map<CRM_CONTATO, CRMContatoViewModel>(item);
                vm.CRM1_CD_ID = (Int32)Session["IdCRM"];
                vm.CRCO_IN_ATIVO = 1;
                Session["VoltaTela"] = 5;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(CRMContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRCO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRCO_NM_NOME);
                    vm.CRCO_NM_CARGO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRCO_NM_CARGO);
                    vm.CRCO_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.CRCO_NM_EMAIL);
                    vm.CRCO_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.CRCO_NR_TELEFONE);
                    vm.CRCO_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.CRCO_NR_CELULAR);

                    // Checa principal
                    CRM crm = (CRM)Session["CRM"];
                    if (crm.CRM_CONTATO != null)
                    {
                        if (((CRM)Session["CRM"]).CRM_CONTATO.Where(p => p.CRCO_IN_PRINCIPAL == 1 & p.CRCO_IN_ATIVO == 1).ToList().Count > 0 & vm.CRCO_IN_PRINCIPAL == 1)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0187", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Executa a operação
                    CRM_CONTATO item = Mapper.Map<CRMContatoViewModel, CRM_CONTATO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateContato(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "icoCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_CONTATO>(item),
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Retorno
                    Session["CRMAlterada"] = 1;
                    Session["VoltaTela"] = 5;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult AcompanhamentoProcessoCRM(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ACOMPANHA_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Mensagens
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 42)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0203", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 43)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 44)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 52)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0122", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 53)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0123", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 82)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0140", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 91)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0146", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 92)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0147", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 93)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0148", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0187", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0256", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensCRM"] == 101)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                }

                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }

                // Processa...
                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                List<SelectListItem> status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
                status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
                status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
                status.Add(new SelectListItem() { Text = "Em Negociação", Value = "4" });
                status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
                List<SelectListItem> adic = new List<SelectListItem>();
                adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
                adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
                adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
                adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
                adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
                ViewBag.Adic = new SelectList(adic, "Value", "Text");
                List<SelectListItem> fav = new List<SelectListItem>();
                fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Favorito = new SelectList(fav, "Value", "Text");
                List<SelectListItem> temp = new List<SelectListItem>();
                temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
                temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
                temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
                temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
                ViewBag.Temp = new SelectList(temp, "Value", "Text");
                List<SelectListItem> envio = new List<SelectListItem>();
                envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Envio = new SelectList(fav, "Value", "Text");
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                Session["IdCRM"] = id;
                CRM item = baseApp.GetItemById(id);
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
                List<CRM_ACAO> acoes = item.CRM_ACAO.ToList().OrderByDescending(p => p.CRAC_DT_CRIACAO).ToList();
                CRM_ACAO acao = acoes.Where(p => p.CRAC_IN_STATUS == 1).FirstOrDefault();
                Session["ClienteCRM"] = item.CLIENTE.CLIE_NM_NOME;
                CLIENTE clie = cliApp.GetItemById(item.CRM1_CD_ID);
                Session["ClienteBase"] = clie;

                List<CRM_PEDIDO_VENDA> peds = CarregaPedidoVenda().Where(p => p.CRM1_CD_ID == item.CRM1_CD_ID).ToList();
                CRM_PEDIDO_VENDA ped = peds.Where(p => p.CRPV_IN_STATUS == 2 || p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 5).FirstOrDefault();
                CRM_PEDIDO_VENDA pedAprov = peds.Where(p => p.CRPV_IN_STATUS == 5).FirstOrDefault();
                Session["PedAprov"] = pedAprov;
                ViewBag.PedAprov = pedAprov;
                Session["SegueInclusao"] = 0;
                Session["Tipo"] = 0;
                Session["TipoHistorico"] = 1;
                Session["VerDia"] = 1;
                Session["NivelCliente"] = 1;
                Session["VoltaCRM"] = 11;

                // Recupera dados do funil
                FUNIL funil = funApp.GetItemById(item.FUNI_CD_ID.Value);
                Session["TemProposta"] = funil.FUNI_IN_PROPOSTA;
                Session["Funil"] = funil.FUNI_NM_NOME;
                List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.OrderBy(p => p.FUET_IN_ORDEM).ToList();
                Int32? ordemInicial = etapas.First().FUET_IN_ORDEM;

                ViewBag.Etapas = etapas.Count;
                Session["NumEtapas"] = etapas.Count;
                Session["Inicial"] = etapas.First().FUET_CD_ID;

                Int32 atual = item.CRM1_IN_STATUS;
                FUNIL_ETAPA etapaAtual = etapas.Where(p => p.FUET_CD_ID == atual).FirstOrDefault();
                String nomeEtapa = etapaAtual.FUET_NM_NOME;
                ViewBag.NomeEtapa = nomeEtapa;
                Session["EtapaAtual"] = atual;
                ViewBag.EtapaProposta = etapaAtual.FUET_IN_PROPOSTA;

                Int32? ordemAtual = etapaAtual.FUET_IN_ORDEM;
                if (ordemAtual == ordemInicial)
                {
                    ViewBag.Ant = 1;
                }
                else
                {
                    ViewBag.Ant = 2;     
                }
                if (ordemAtual == etapas.Count)
                {
                    ViewBag.Prox = 1;
                }
                else
                {
                    ViewBag.Prox = 2;
                }

                Int32 encerra = etapaAtual.FUET_IN_ENCERRA;
                ViewBag.Encerra = encerra;
                Int32? etapaEncerra = etapas.Where(p => p.FUET_IN_ENCERRA == 1).FirstOrDefault().FUET_IN_ORDEM;
                Session["EtapaEncerra"] = etapaEncerra;
                ViewBag.EtapaEncerra = etapaEncerra;

                // Sessões
                Session["Acoes"] = acoes;
                Session["Peds"] = peds;
                Session["CRM"] = item;
                Session["VoltaCRM"] = 11;
                Session["VoltaAgendaCRMCalend"] = 10;
                Session["ClienteCRM"] = item.CLIENTE;
                Session["VoltaAgenda"] = 22;
                ViewBag.Acoes = acoes;
                ViewBag.Acao = acao;
                ViewBag.Peds = peds;
                ViewBag.Ped = ped;
                Session["PontoAcao"] = 2;
                Session["PontoProposta"] = 2;
                Session["VoltaPedido"] = 2;
                Session["SegueInclusao"] = 0;
                Session["FlagMensagensEnviadas"] = 8;
                Session["FlagMensagensEnviadas"] = 6;
                Session["MensCRM"] = 0;
                Session["TipoHistorico"] = 1;
                Session["ListaDiario"] = null;
                Session["CatAgendas"] = null;
                Session["Usuarios"] = null;
                Session["AbaAgenda"] = 1;
                Session["NaoFezNada"] = 5;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AcompanhamentoProcessoCRM(CRMViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Em Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");
            List<SelectListItem> envio = new List<SelectListItem>();
            envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Envio = new SelectList(fav, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Indicadores
                    ViewBag.Incluir = (Int32)Session["IncluirCRM"];

                    // Sanitização
                    vm.CRM1_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_DS_DESCRICAO);
                    vm.CRM1_NM_CAMPANHA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRM1_NM_CAMPANHA);
                    vm.CRM1_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringMail(vm.CRM1_NM_NOME);
                    vm.CRM1_TX_INFORMACOES_GERAIS = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.CRM1_TX_INFORMACOES_GERAIS);

                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (CRM)Session["CRM"], usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }

                    // Retorno
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 0;
                    Session["CRMAlterada"] = 1;
                    Session["FlagCRM"] = 1;
                    Session["LinhaAlterada"] = item.CRM1_CD_ID;
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }
            }
            else
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
        }

        public ActionResult GerarRelatorioDetalheCRM()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EnviarEMailContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_MENSAGEM_CONTATO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensCRM"] = 200;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara
                Int32 crm = (Int32)Session["IdCRM"];
                CRM item = baseApp.GetItemById(crm);
                CRM_CONTATO cont = baseApp.GetContatoById(id);
                Session["Contato"] = cont;
                ViewBag.Contato = cont;
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.CRCO_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.CRCO_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarEMailContato(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idNot = (Int32)Session["IdCRM"];

                    // Sanitização
                    CRM_CONTATO cont = (CRM_CONTATO)Session["Contato"];
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Mensagem para " + cont.CRCO_NM_NOME;
                    vm.MENS_NM_CAMPANHA = cont.CRCO_NM_EMAIL;

                    vm.MENS_TX_TEXTO = HtmlToPlainText(vm.MENS_TX_TEXTO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    Int32 voltaX = await envio.ProcessaEnvioEMailGeral(vm, usuarioLogado);

                    // Grava envio
                    String guid = new Guid().ToString();
                    Int32 volta1 = envio.GravarMensagemEnviada(vm, usuarioLogado, vm.MENS_TX_TEXTO, "Success", guid, null, "Mensagem de E-Mail para Contatos");

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "mcoCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.CRCO_NM_NOME + " - E-Mail: " + cont.CRCO_NM_EMAIL,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Retorno
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EnviarSMSContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_MENSAGEM_CONTATO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensCRM"] = 201;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara
                Int32 crm = (Int32)Session["IdCRM"];
                CRM item = baseApp.GetItemById(crm);
                CRM_CONTATO cont = baseApp.GetContatoById(id);
                Session["Contato"] = cont;
                ViewBag.Contato = cont;
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.CRCO_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.CRCO_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EnviarSMSContato(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idNot = (Int32)Session["IdCRM"];

                    // Sanitização
                    CRM_CONTATO cont = (CRM_CONTATO)Session["Contato"];
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Mensagem para " + cont.CRCO_NM_NOME;

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSContato(vm, usuarioLogado);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "scoCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.CRCO_NM_NOME + " - Celular: " + cont.CRCO_NR_CELULAR,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Retorno
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
                }
                catch (Exception ex)
                {
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

        [HttpGet]
        public ActionResult EnviarSMSCliente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_MENSAGEM_CLIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensCRM"] = 201;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara
                Int32 crm = (Int32)Session["IdCRM"];
                CRM item = baseApp.GetItemById(crm);
                CLIENTE cont = cliApp.GetItemById(id);
                Session["Cliente"] = cont;
                ViewBag.Cliente = cont;
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.CLIE_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.CLIE_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                return View(mens);
            }
            catch (Exception ex)
            {
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

        [HttpPost]
        public ActionResult EnviarSMSCliente(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idNot = (Int32)Session["IdCRM"];

                    // Sanitização
                    CLIENTE cont = (CLIENTE)Session["Cliente"];
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Mensagem para " + cont.CLIE_NM_NOME;

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSCliente(vm, usuarioLogado);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "smsCLIE",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.CLIE_NM_NOME + " - Celular: " + cont.CLIE_NR_CELULAR,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Retorno
                    Session["VoltaTela"] = 0;
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
                }
                catch (Exception ex)
                {
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

        [HttpGet]
        public ActionResult IncluirComentarioCRM()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 id = (Int32)Session["IdCRM"];

                // Prepara
                CRM item = baseApp.GetItemById(id);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CRM_COMENTARIO coment = new CRM_COMENTARIO();
                CRMComentarioViewModel vm = Mapper.Map<CRM_COMENTARIO, CRMComentarioViewModel>(coment);
                vm.CRCM_DT_COMENTARIO = DateTime.Now;
                vm.CRCM_IN_ATIVO = 1;
                vm.CRCM_CD_ID = item.CRM1_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                vm.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                Session["PontoProposta"] = 85;
                return View(vm);
            }
            catch (Exception ex)
            {
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

        [HttpPost]
        public ActionResult IncluirComentarioCRM(CRMComentarioViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idNot = (Int32)Session["IdCRM"];

                    // Sanitização
                    vm.CRCM_DS_COMENTARIO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRCM_DS_COMENTARIO);

                    // Executa a operação
                    CRM_COMENTARIO item = Mapper.Map<CRMComentarioViewModel, CRM_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.CRM_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Gera diario
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRCM_CD_ID = item.CRCM_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Comentário de Processo";
                    dia.DIPR_DS_DESCRICAO = "Comentário de Processo " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "icmCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Processo: " + item.CRM.CRM1_NM_NOME + " - Anotação: " + item.CRCM_DS_COMENTARIO,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["VoltaTela"] = 3;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
                }
                catch (Exception ex)
                {
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

        [HttpGet]
        public ActionResult EditarAnotacaoCRM(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                CRM_COMENTARIO item = baseApp.GetComentarioById(id);
                CRMComentarioViewModel vm = Mapper.Map<CRM_COMENTARIO, CRMComentarioViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoCRM(CRMComentarioViewModel vm)
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
                    vm.CRCM_DS_COMENTARIO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRCM_DS_COMENTARIO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_COMENTARIO item = Mapper.Map<CRMComentarioViewModel, CRM_COMENTARIO>(vm);
                    Int32 volta = baseApp.ValidateEditAnotacao(item);
                    CRM crm = (CRM)Session["CRM"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ecmCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Processo: " + crm.CRM1_NM_NOME + " - Anotação: " + item.CRCM_DS_COMENTARIO,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["CRMAlterada"] = 1;
                    Session["VoltaTela"] = 3;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoCRM(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                CRM_COMENTARIO item = baseApp.GetComentarioById(id);
                item.CRCM_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnotacao(item);
                CRM crm = (CRM)Session["CRM"];

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xcmCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + crm.CRM1_NM_NOME + " - Anotação: " + item.CRCM_DS_COMENTARIO,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                Session["CRMAlterada"] = 1;
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarAcao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERACAO_ACAO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica se pode editar ação
                CRM_ACAO item = baseApp.GetAcaoById(id);
                if (item.CRAC_IN_STATUS > 2)
                {
                    Session["MensCRM"] = 43;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara view
                ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
                List<USUARIO> listaTotal = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

                // Monta Status
                List<SelectListItem> status = new List<SelectListItem>();
                if (item.CRAC_IN_STATUS == 1)
                {
                    status.Add(new SelectListItem() { Text = "Pendente", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                    ViewBag.Status = new SelectList(status, "Value", "Text");
                }
                else if (item.CRAC_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Ativa", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                    ViewBag.Status = new SelectList(status, "Value", "Text");
                }

                // Processa
                Session["Acao"] = item;
                objetoAntes = (CRM)Session["CRM"];
                CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAcao(CRMAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario();
            ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {

                    // Sanitização
                    vm.CRAC_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRAC_DS_DESCRICAO);
                    vm.CRAC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRAC_NM_TITULO);

                    // Executa a operação
                    CRM_ACAO item = Mapper.Map<CRMAcaoViewModel, CRM_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEditAcao(item);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRAC_CD_ID = item.CRAC_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Alteração de Ação";
                    dia.DIPR_DS_DESCRICAO = "Alteração de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Log
                    CRM_ACAO antes = (CRM_ACAO)Session["Acao"];
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "edtCRAC",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Processo: " + not.CRM1_NM_NOME + " - Ação: " + item.CRAC_NM_TITULO,
                        LOG_TX_REGISTRO_ANTES = null,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["ListaCRM"] = null;
                    Session["CRMAcaoAlterada"] = 1;
                    Session["VoltaTela"] = 1;
                    Session["FlagCRM"] = 1;
                    Session["LinhaAlterada1"] = item.CRAC_CD_ID;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcaoCRM");
                }
                catch (Exception ex)
                {
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

        [HttpGet]
        public ActionResult ExcluirAcao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_ACAO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_ACAO item = baseApp.GetAcaoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRAC_IN_ATIVO = 0;
                item.CRAC_IN_STATUS = 4;
                Int32 volta = baseApp.ValidateEditAcao(item);

                // Exclui agendamentos
                if (item.AGENDA.Count > 0)
                {
                    AGENDA age = item.AGENDA.First();
                    AGENDA nova = new AGENDA();
                    nova.AGEN_CD_ID = age.AGEN_CD_ID;
                    nova.AGEN_CD_USUARIO = age.AGEN_CD_USUARIO;
                    nova.AGEN_DS_DESCRICAO = age.AGEN_DS_DESCRICAO;
                    nova.AGEN_DT_DATA = age.AGEN_DT_DATA;
                    nova.AGEN_HR_FINAL = age.AGEN_HR_FINAL;
                    nova.AGEN_HR_HORA = age.AGEN_HR_HORA;
                    nova.AGEN_IN_ATIVO = age.AGEN_IN_ATIVO;
                    nova.AGEN_IN_CONFIRMADO = age.AGEN_IN_CONFIRMADO;
                    nova.AGEN_IN_CORPORATIVA = age.AGEN_IN_CORPORATIVA;
                    nova.AGEN_IN_STATUS = age.AGEN_IN_STATUS;
                    nova.AGEN_LK_REUNIAO = age.AGEN_LK_REUNIAO;
                    nova.AGEN_NM_TITULO = age.AGEN_NM_TITULO;
                    nova.AGEN_TX_OBSERVACOES = age.AGEN_TX_OBSERVACOES;
                    nova.ASSI_CD_ID = age.ASSI_CD_ID;
                    nova.CAAG_CD_ID = age.CAAG_CD_ID;
                    nova.TARE_CD_ID = age.TARE_CD_ID;
                    nova.USUA_CD_ID = age.USUA_CD_ID;
                    nova.CRM1_CD_ID = age.CRM1_CD_ID;
                    nova.CRAC_CD_ID = age.CRAC_CD_ID;
                    nova.EMPR_CD_ID = age.EMPR_CD_ID;
                    nova.AGEN_IN_ATIVO = 0;
                    Int32 volta1 = ageApp.ValidateEdit(nova, usuario);
                }

                // Gera diario
                CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.CRAC_CD_ID = item.CRAC_CD_ID;
                dia.DIPR_NM_OPERACAO = "Exclusão de Ação";
                dia.DIPR_DS_DESCRICAO = "Exclusão de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                CRM_ACAO antes = (CRM_ACAO)Session["Acao"];
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "delCRAC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + not.CRM1_NM_NOME + " - Ação: " + item.CRAC_NM_TITULO,
                    LOG_TX_REGISTRO_ANTES = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                Session["CRMAcaoAlterada"] = 1;
                Session["VoltaTela"] = 1;
                Session["FlagCRM"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            catch (Exception ex)
            {
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

        [HttpGet]
        public ActionResult ReativarAcao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVA_ACAO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica se pode reativar ação
                List<CRM_ACAO> acoes = (List<CRM_ACAO>)Session["Acoes"];
                if (acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
                {
                    Session["MensCRM"] = 44;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Processa
                CRM_ACAO item = baseApp.GetAcaoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRAC_IN_ATIVO = 1;
                item.CRAC_IN_STATUS = 1;
                Int32 volta = baseApp.ValidateEditAcao(item);

                // Gera diario
                CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.CRAC_CD_ID = item.CRAC_CD_ID;
                dia.DIPR_NM_OPERACAO = "Reativação de Ação";
                dia.DIPR_DS_DESCRICAO = "Reativação de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                CRM_ACAO antes = (CRM_ACAO)Session["Acao"];
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "reaCRAC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + not.CRM1_NM_NOME + " - Ação: " + item.CRAC_NM_TITULO,
                    LOG_TX_REGISTRO_ANTES = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                Session["FlagCRM"] = 1;
                Session["CRMAlterada"] = 1;
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            catch (Exception ex)
            {
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

        [HttpGet]
        public ActionResult ConsultarEstoque()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaClienteCRM"] = 1;
            return RedirectToAction("MontarTelaEstoqueProduto", "Estoque");
        }

        [HttpGet]
        public ActionResult ConsultarCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaClienteCRM"] = 5;
            return RedirectToAction("EditarCliente", new { id = id });
        }

        [HttpGet]
        public ActionResult EncerrarAcaoNova(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ENCERRA_ACAO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_ACAO item = baseApp.GetAcaoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRAC_IN_ATIVO = 0;
                item.CRAC_IN_STATUS = 3;
                Int32 volta = baseApp.ValidateEditAcao(item);

                // Gera diario
                CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.CRAC_CD_ID = item.CRAC_CD_ID;
                dia.DIPR_NM_OPERACAO = "Encerramento de Ação";
                dia.DIPR_DS_DESCRICAO = "Encerramento de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "encCRAC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + not.CRM1_NM_NOME + " - Ação: " + item.CRAC_NM_TITULO,
                    LOG_TX_REGISTRO_ANTES = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                Session["CRMAcaoAlterada"] = 1;
                Session["VoltaTela"] = 1;
                Session["FlagCRM"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarCliente(Int32 id)
        {
            Session["VoltaCRM"] = 11;
            Session["IdCliente"] = id;
            Session["VoltaTela"] = 0;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("VoltarAnexoCliente", "Cliente");
        }

        [HttpGet]
        public ActionResult EditarAgenda(Int32 id)
        {
            Session["VoltaAgenda"] = 22;
            Session["IdVolta"] = id;
            Session["FiltroAgendaCalendario"] = 0;
            Session["VoltaTela"] = 1;
            return RedirectToAction("VoltarAnexoAgenda", "Agenda");
        }

        public ActionResult VerAcao(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_ACAO item = baseApp.GetAcaoById(id);
                objetoAntes = (CRM)Session["CRM"];
                CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerLembrete(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_FOLLOW item = baseApp.GetFollowById(id);
                objetoAntes = (CRM)Session["CRM"];
                CRMFollowViewModel vm = Mapper.Map<CRM_FOLLOW, CRMFollowViewModel>(item);
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAlerta(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_FOLLOW item = baseApp.GetFollowById(id);
                objetoAntes = (CRM)Session["CRM"];
                CRMFollowViewModel vm = Mapper.Map<CRM_FOLLOW, CRMFollowViewModel>(item);
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAcoesUsuarioCRM()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                List<CRM_ACAO> lista = baseApp.GetAllAcoes(idAss).Where(p => p.USUA_CD_ID2 == usuario.USUA_CD_ID & p.CRM.CRM1_IN_ATIVO != 2).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();
                List<CRM_ACAO> totalPendente = lista.Where(p => p.CRAC_IN_STATUS == 2).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();
                List<CRM_ACAO> totalEncerrada = lista.Where(p => p.CRAC_IN_STATUS == 3).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();
                List<CRM_ACAO> totalAtrasada = lista.Where(p => p.CRAC_IN_STATUS != 3 & p.CRAC_DT_PREVISTA < DateTime.Today.Date).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();

                if ((Int32)Session["AcoesUsuario"] == 1)
                {
                    ViewBag.Lista = lista;
                    ViewBag.TotalAcoes = lista.Count;
                }
                if ((Int32)Session["AcoesUsuario"] == 2)
                {
                    ViewBag.Lista = totalPendente;
                    ViewBag.TotalAcoes = totalPendente.Count;
                }
                if ((Int32)Session["AcoesUsuario"] == 3)
                {
                    ViewBag.Lista = totalEncerrada;
                    ViewBag.TotalAcoes = totalEncerrada.Count;
                }
                if ((Int32)Session["AcoesUsuario"] == 4)
                {
                    ViewBag.Lista = totalAtrasada;
                    ViewBag.TotalAcoes = totalAtrasada.Count;
                }

                ViewBag.TotalPendentes = totalPendente.Count;
                ViewBag.TotalEncerradas = totalEncerrada.Count;
                ViewBag.TotalAtrasadas = totalAtrasada.Count;

                ViewBag.Nome = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
                ViewBag.Foto = usuario.USUA_AQ_FOTO;
                ViewBag.Cargo = usuario.CARGO.CARG_NM_NOME;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAcoesUsuarioCRMPrevia()
        {
            Session["AcoesUsuario"] = 1;
            Session["PontoAcao"] = 100;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMPendentePrevia()
        {
            Session["AcoesUsuario"] = 2;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMPrevia1()
        {
            Session["AcoesUsuario"] = 1;
            Session["PontoAcao"] = 101;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMEncerradaPrevia()
        {
            Session["AcoesUsuario"] = 3;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMAtrasadaPrevia()
        {
            Session["AcoesUsuario"] = 4;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMPrevia()
        {
            Session["PropostasUsuario"] = 1;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMPrevia()
        {
            Session["PedidosUsuario"] = 1;
            Session["SegueInclusao"] = 0;
            Session["PontoProposta"] = 0;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult EditarClienteForm(Int32 id)
        {
            Session["VoltaClienteCRM"] = 2;
            Session["VoltaMsg"] = 0;
            Session["VoltaCliente"] = 0;
            Session["VoltaCRM"] = 0;
            Session["NivelCliente"] = 1;
            return RedirectToAction("EditarCliente", new { id = id });
        }

        public ActionResult VerPropostasUsuarioCRMElaboracaoPrevia()
        {
            Session["PropostasUsuario"] = 2;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMElaboracaoPrevia()
        {
            Session["PedidosUsuario"] = 2;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMEnviadaPrevia()
        {
            Session["PropostasUsuario"] = 3;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMEnviadaPrevia()
        {
            Session["PedidosUsuario"] = 3;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMAprovadaPrevia()
        {
            Session["PropostasUsuario"] = 6;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMAprovadaPrevia()
        {
            Session["PedidosUsuario"] = 6;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMReprovadaPrevia()
        {
            Session["PropostasUsuario"] = 5;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMCanceladaPrevia()
        {
            Session["PropostasUsuario"] = 4;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMencerradaPrevia()
        {
            Session["PropostasUsuario"] = 7;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        [HttpGet]
        public ActionResult IncluirAcao()
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
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_ACAO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica se pode incluir ação
                List<CRM_ACAO> acoes = (List<CRM_ACAO>)Session["Acoes"];
                if (acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
                {
                    Session["MensCRM"] = 42;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara view
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
                ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
                List<USUARIO> listaTotal = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                List<SelectListItem> agenda = new List<SelectListItem>();
                agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
                ViewBag.Agenda = new SelectList(agenda, "Value", "Text");

                CRM_ACAO item = new CRM_ACAO();
                CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
                vm.CRM1_CD_ID = (Int32)Session["IdCRM"];
                vm.CRAC_IN_ATIVO = 1;
                vm.ASSI_CD_ID = idAss;
                vm.CRAC_DT_CRIACAO = DateTime.Now;
                vm.CRAC_IN_STATUS = 1;
                vm.USUA_CD_ID1 = usuario.USUA_CD_ID;
                vm.CRAC_DT_PREVISTA = DateTime.Now.AddDays(Convert.ToDouble(conf.CONF_NR_DIAS_ACAO));
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                vm.CRIA_AGENDA = 2;
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcao(CRMAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];


            ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario();
            ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> agenda = new List<SelectListItem>();
            agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Agenda = new SelectList(agenda, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRAC_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRAC_DS_DESCRICAO);
                    vm.CRAC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRAC_NM_TITULO);

                    // Verifica tipo de ação
                    if (vm.TIAC_CD_ID == null || vm.TIAC_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0142", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.USUA_CD_ID2 == null || vm.USUA_CD_ID2 == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0143", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    CRM_ACAO item = Mapper.Map<CRMAcaoViewModel, CRM_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateAcao(item, usuarioLogado);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRAC_CD_ID = item.CRAC_CD_ID;
                    dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Criação de Ação";
                    dia.DIPR_DS_DESCRICAO = "Criação de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Processa agenda
                    if (vm.CRIA_AGENDA == 1)
                    {
                        AGENDA ag = new AGENDA();
                        ag.AGEN_DS_DESCRICAO = "Ação: " + vm.CRAC_DS_DESCRICAO;
                        ag.AGEN_DT_DATA = item.CRAC_DT_PREVISTA.Value;
                        ag.AGEN_HR_HORA = item.CRAC_DT_PREVISTA.Value.TimeOfDay;
                        ag.AGEN_IN_ATIVO = 1;
                        ag.AGEN_IN_STATUS = 1;
                        ag.AGEN_NM_TITULO = item.CRAC_NM_TITULO;
                        ag.ASSI_CD_ID = idAss;
                        ag.CAAG_CD_ID = 1;
                        ag.AGEN_CD_USUARIO = item.USUA_CD_ID2;
                        ag.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                        ag.CRM1_CD_ID = item.CRM1_CD_ID;
                        ag.CRAC_CD_ID = item.CRAC_CD_ID;
                        Int32 voltaAg = ageApp.ValidateCreate(ag, usuarioLogado);

                        // Cria pastas
                        String caminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Agenda/" + ag.AGEN_CD_ID.ToString() + "/Anexos/";
                        Directory.CreateDirectory(Server.MapPath(caminho));

                        // Gera diario
                        dia = new DIARIO_PROCESSO();
                        dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                        dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                        dia.DIPR_DT_DATA = DateTime.Today.Date;
                        dia.CRM1_CD_ID = item.CRM1_CD_ID;
                        dia.CRAC_CD_ID = item.CRAC_CD_ID;
                        dia.AGEN_CD_ID = ag.AGEN_CD_ID;
                        dia.DIPR_NM_OPERACAO = "Agendamento de Ação";
                        dia.DIPR_DS_DESCRICAO = "Agendamento de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME + ". Data: " + ag.AGEN_DT_DATA.ToLongDateString();
                        dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                        Int32 volta4 = diaApp.ValidateCreate(dia);
                    }

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "addCRAC",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_ACAO>(item),
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["CRMAcaoAlterada"] = 1;
                    Session["FlagCRM"] = 1;
                    Session["VoltaTela"] = 1;
                    Session["LinhaAlterada1"] = item.CRAC_CD_ID;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSContato(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                // Recupera contatos
                Int32 idAss = (Int32)Session["IdAssinante"];
                CRM_CONTATO cont = (CRM_CONTATO)Session["Contato"];

                // Processa SMS
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

                // Recupera CRM
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);

                // Decriptografa chaves
                String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
                String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

                // Monta token
                String text = login + ":" + senha;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = vm.MENS_TX_SMS;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(vm.MENS_TX_SMS);
                if (!String.IsNullOrEmpty(vm.LINK))
                {
                    if (!vm.LINK.Contains("www."))
                    {
                        vm.LINK = "www." + vm.LINK;
                    }
                    if (!vm.LINK.Contains("http://"))
                    {
                        vm.LINK = "http://" + vm.LINK;
                    }
                    str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                    texto += "  " + vm.LINK;
                }
                String body = str.ToString();
                String smsBody = body;
                String erro = null;

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                String listaDest = "55" + Regex.Replace(cont.CRCO_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;
                    
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }

                // Grava envio
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                env.CRCO_CD_ID = cont.CRCO_CD_ID;
                env.CRM1_CD_ID = crm.CRM1_CD_ID;
                env.MEEN_IN_TIPO = 2;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN__CELULAR_DESTINO = cont.CRCO_NR_CELULAR;
                env.MEEN_NM_ORIGEM = "Mensagem SMS para Contato do Processo";
                env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
                env.MEEN_TX_CORPO_COMPLETO = texto;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 5;
                env.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                if (erro == null)
                {
                    env.MEEN_IN_ENTREGUE = 1;
                }
                else
                {
                    env.MEEN_IN_ENTREGUE = 0;
                    env.MEEN_TX_RETORNO = erro;
                }
                Int32 volta5 = meApp.ValidateCreate(env);
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }
            return 0;
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSCliente(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                // Recupera contatos
                Int32 idAss = (Int32)Session["IdAssinante"];
                CLIENTE cont = (CLIENTE)Session["Cliente"];

                // Processa SMS
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

                // Recupera CRM
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);

                // Decriptografa chaves
                String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
                String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

                // Monta token
                String text = login + ":" + senha;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = vm.MENS_TX_SMS;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(vm.MENS_TX_SMS);
                if (!String.IsNullOrEmpty(vm.LINK))
                {
                    if (!vm.LINK.Contains("www."))
                    {
                        vm.LINK = "www." + vm.LINK;
                    }
                    if (!vm.LINK.Contains("http://"))
                    {
                        vm.LINK = "http://" + vm.LINK;
                    }
                    str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                    texto += "  " + vm.LINK;
                }
                String body = str.ToString();
                String smsBody = body;
                String erro = null;

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                String listaDest = "55" + Regex.Replace(cont.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }

                // Grava envio
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                env.CLIE_CD_ID = cont.CLIE_CD_ID;
                env.CRM1_CD_ID = crm.CRM1_CD_ID;
                env.MEEN_IN_TIPO = 2;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN__CELULAR_DESTINO = cont.CLIE_NR_CELULAR;
                env.MEEN_NM_ORIGEM = "Mensagem SMS para Cliente do Processo";
                env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
                env.MEEN_TX_CORPO_COMPLETO = texto;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 5;
                env.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                if (erro == null)
                {
                    env.MEEN_IN_ENTREGUE = 1;
                }
                else
                {
                    env.MEEN_IN_ENTREGUE = 0;
                    env.MEEN_TX_RETORNO = erro;
                }
                Int32 volta5 = meApp.ValidateCreate(env);
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Cliente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }
            return 0;
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSGeral(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                // Recupera contatos
                Int32 idAss = (Int32)Session["IdAssinante"];
                CLIENTE cont = null;
                USUARIO usu = null;
                if (vm.MENS_IN_TIPO == 1)
                {
                    cont = cliApp.GetItemById(vm.ID.Value);
                }
                if (vm.MENS_IN_TIPO == 2)
                {
                    usu = usuApp.GetItemById(vm.ID.Value);
                }

                // Processa SMS
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

                // Recupera CRM
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);

                // Decriptografa chaves
                String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
                String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

                // Monta token
                String text = login + ":" + senha;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = vm.MENS_TX_SMS;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(vm.MENS_TX_SMS);
                if (!String.IsNullOrEmpty(vm.LINK))
                {
                    if (!vm.LINK.Contains("www."))
                    {
                        vm.LINK = "www." + vm.LINK;
                    }
                    if (!vm.LINK.Contains("http://"))
                    {
                        vm.LINK = "http://" + vm.LINK;
                    }
                    str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                    texto += "  " + vm.LINK;
                }
                String body = str.ToString();
                String smsBody = body;
                String erro = null;

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                String listaDest = null;
                if (vm.MENS_IN_TIPO == 1)
                {
                    listaDest = "55" + Regex.Replace(cont.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                }
                if (vm.MENS_IN_TIPO == 2)
                {
                    listaDest = "55" + Regex.Replace(usu.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                }
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"CRMSys\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }

                // Grava envio
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                if (vm.MENS_IN_TIPO == 1)
                {
                    env.CLIE_CD_ID = cont.CLIE_CD_ID;
                }
                else
                {
                    env.MEEN_IN_USUARIO = usu.USUA_CD_ID;
                }
                env.CRM1_CD_ID = crm.CRM1_CD_ID;
                env.MEEN_IN_TIPO = 2;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                if (vm.MENS_IN_TIPO == 1)
                {
                    env.MEEN_EM_EMAIL_DESTINO = cont.CLIE_NM_EMAIL;
                }
                else
                {
                    env.MEEN_EM_EMAIL_DESTINO = usu.USUA_NM_EMAIL;
                }
                env.MEEN_NM_ORIGEM = "Mensagem de Processo - Mudança de Status";
                env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                if (vm.MENS_IN_TIPO == 1)
                {
                    env.MEEN_IN_ESCOPO = 5;
                }
                else
                {
                    env.MEEN_IN_ESCOPO = 5;
                }
                env.MEEN_TX_CORPO_COMPLETO = texto;
                if (erro == null)
                {
                    env.MEEN_IN_ENTREGUE = 1;
                }
                else
                {
                    env.MEEN_IN_ENTREGUE = 0;
                    env.MEEN_TX_RETORNO = erro;
                }
                Int32 volta5 = meApp.ValidateCreate(env);
                return 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }
            return 0;
        }

        [HttpPost]
        public JsonResult GetEtapaFunil(int idFunil)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                // Consulta as etpas do funil

                var listaEtapaFunil = funApp.GetById(idFunil).FUNIL_ETAPA;
                List<Hashtable> etapas = new List<Hashtable>();
                foreach (var item in listaEtapaFunil.OrderBy(x => x.FUET_IN_ORDEM))
                {
                    etapas.Add(new Hashtable()
                {
                    {"FUET_NM_NOME",item.FUET_NM_NOME }
                    ,{"FUET_DS_DESCRICAO",item.FUET_DS_DESCRICAO }
                    ,{"FUET_IN_ENCERRA",item.FUET_IN_ENCERRA }
                    ,{"FUET_CD_ID",item.FUET_CD_ID }
                    ,{"FUNI_CD_ID",item.FUNI_CD_ID }
                });
                }

                //Consultar os processos
                var listaProcessos = CarregaCRMSemCache().Where(p => p.FUNI_CD_ID == idFunil).ToList();
                var processos = new List<Hashtable>();
                foreach (var item in listaProcessos)
                {
                    var hash = new Hashtable();
                    hash.Add("CRM1_IN_STATUS", item.CRM1_IN_STATUS);
                    hash.Add("CRM1_CD_ID", item.CRM1_CD_ID);
                    hash.Add("CRM1_NM_NOME", item.CRM1_NM_NOME);
                    hash.Add("CRM1_NR_TEMPERATURA", item.CRM1_NR_TEMPERATURA);
                    hash.Add("CLIE_NM_NOME", item.CLIENTE.CLIE_NM_NOME);
                    hash.Add("CLIE_IN_ATIVO", item.CLIENTE.CLIE_IN_ATIVO);
                    hash.Add("CROR_NM_NOME", item.CRM_ORIGEM.CROR_NM_NOME);
                    hash.Add("CRM1_DT_CRIACAO", item.CRM1_DT_CRIACAO.Value.ToString("dd/MM/yyyy"));
                    if (item.CRM1_DT_ENCERRAMENTO != null)
                    {
                        hash.Add("CRM1_DT_ENCERRAMENTO", item.CRM1_DT_ENCERRAMENTO.Value.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        hash.Add("CRM1_DT_ENCERRAMENTO", "-");
                    }
                    hash.Add("CRM1_NM_CLIENTE", item.CLIENTE.CLIE_NM_NOME);
                    processos.Add(hash);
                }
                return Json(new
                {
                    etapas = etapas
                    ,
                    processos = processos
                });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Cliente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return Json("FAIL");
            }
        }

        [HttpPost]
        public JsonResult GetProcessos()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                listaMaster = baseApp.GetAllItens(idAss).Where(p => p.FUNI_CD_ID == (Int32)Session["IdFunil"]).ToList();
                var listaHash = new List<Hashtable>();
                foreach (var item in listaMaster)
                {
                    var hash = new Hashtable();
                    hash.Add("CRM1_IN_STATUS", item.CRM1_IN_STATUS);
                    hash.Add("CRM1_CD_ID", item.CRM1_CD_ID);
                    hash.Add("CRM1_NM_NOME", item.CRM1_NM_NOME);
                    hash.Add("CRM1_DT_CRIACAO", item.CRM1_DT_CRIACAO.Value.ToString("dd/MM/yyyy"));
                    if (item.CRM1_DT_ENCERRAMENTO != null)
                    {
                        hash.Add("CRM1_DT_ENCERRAMENTO", item.CRM1_DT_ENCERRAMENTO.Value.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        hash.Add("CRM1_DT_ENCERRAMENTO", "-");
                    }
                    hash.Add("CRM1_NM_CLIENTE", item.CLIENTE.CLIE_NM_NOME);
                    listaHash.Add(hash);
                }
                return Json(listaHash);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Cliente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return Json("FAIL");
            }
        }

        [HttpPost]
        public JsonResult EditarStatusCRM(Int32 id, Int32 status, DateTime? dtEnc)
        {
            try
            {
                CRM crm = baseApp.GetById(id);
                crm.CRM1_IN_STATUS = status;
                crm.CRM1_DT_ENCERRAMENTO = dtEnc;
                crm.MOEN_CD_ID = 1;
                crm.CRM1_DS_INFORMACOES_ENCERRAMENTO = "Processo Encerrado";

                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateEdit(crm, crm, usuarioLogado);

                // Gera diario
                CLIENTE cli = cliApp.GetItemById(crm.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = crm.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Alteração de Status de Processo";
                dia.DIPR_DS_DESCRICAO = "Alteração de Status de processo " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Verifica retorno
                if (volta == 1)
                {
                    return Json(CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }

                Session["ListaCRM"] = null;
                Session["CRMAlterada"] = 1;
                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Cliente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return Json("FAIL");
            }
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                CRM aten = baseApp.GetItemById((Int32)Session["IdCRM"]);
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "CRM" + aten.CRM1_CD_ID.ToString() + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontVerde = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
                Font meuFontAzul= FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Font meuFontVermelho = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.RED);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Cabeçalho
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Processo CRM - Detalhes", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 4;
                table.AddCell(cell);

                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do Cliente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados do Cliente", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Empresa/Filial: " + aten.EMPRESA.EMPR_NM_NOME, meuFontVerde));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome: " + aten.CLIENTE.CLIE_NM_NOME, meuFontVerde));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.CLIENTE.CLIE_NM_ENDERECO != null)
                {
                    cell = new PdfPCell(new Paragraph("Endereço: " + aten.CLIENTE.CLIE_NM_ENDERECO + " " + aten.CLIENTE.CLIE_NR_NUMERO + " " + aten.CLIENTE.CLIE_NM_COMPLEMENTO, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    if (aten.CLIENTE.UF != null)
                    {
                        cell = new PdfPCell(new Paragraph("          " + aten.CLIENTE.CLIE_NM_BAIRRO + " - " + aten.CLIENTE.CLIE_NM_CIDADE + " - " + aten.CLIENTE.UF.UF_SG_SIGLA + " - " + aten.CLIENTE.CLIE_NR_CEP, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("          " + aten.CLIENTE.CLIE_NM_BAIRRO + " - " + aten.CLIENTE.CLIE_NM_CIDADE + " - " + aten.CLIENTE.CLIE_NR_CEP, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                    }
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Endereço: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                cell = new PdfPCell(new Paragraph("Telefone: " + aten.CLIENTE.CLIE_NR_TELEFONE, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular: " + aten.CLIENTE.CLIE_NR_CELULAR, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail: " + aten.CLIENTE.CLIE_NM_EMAIL, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Dados do Processo
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados do Processo", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome: " + aten.CRM1_NM_NOME, meuFontVerde));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.CRM1_IN_STATUS == 1)
                {
                    cell = new PdfPCell(new Paragraph("Status: Prospecção", meuFontAzul));
                }
                else if (aten.CRM1_IN_STATUS == 2)
                {
                    cell = new PdfPCell(new Paragraph("Status: Contato Realizado", meuFontAzul));
                }
                else if (aten.CRM1_IN_STATUS == 3)
                {
                    cell = new PdfPCell(new Paragraph("Status: Proposta Enviada", meuFontAzul));
                }
                else if (aten.CRM1_IN_STATUS == 4)
                {
                    cell = new PdfPCell(new Paragraph("Status: Em Negociação", meuFontAzul));
                }
                else if (aten.CRM1_IN_STATUS == 5)
                {
                    cell = new PdfPCell(new Paragraph("Status: Encerrado", meuFontAzul));
                }
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Descrição: " + aten.CRM1_DS_DESCRICAO, meuFontVerde));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Informações: " + aten.CRM1_TX_INFORMACOES_GERAIS, meuFontVerde));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Criação: " + aten.CRM1_DT_CRIACAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável: " + aten.USUARIO.USUA_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Origem: " + aten.CRM_ORIGEM.CROR_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.CRM1_IN_ATIVO == 3)
                {
                    cell = new PdfPCell(new Paragraph("Data Cancelamento: " + aten.CRM1_DT_CANCELAMENTO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Motivo: " + aten.MOTIVO_CANCELAMENTO.MOCA_NM_NOME, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Descrição: " + aten.CRM1_DS_MOTIVO_CANCELAMENTO, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                if (aten.CRM1_IN_STATUS == 5)
                {
                    cell = new PdfPCell(new Paragraph("Data Encerramento: " + aten.CRM1_DT_ENCERRAMENTO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Motivo: " + aten.MOTIVO_ENCERRAMENTO.MOEN_NM_NOME, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Descrição: " + aten.CRM1_DS_INFORMACOES_ENCERRAMENTO, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Contatos
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Contatos", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.CRM_CONTATO.Count > 0)
                {
                    table = new PdfPTable(new float[] { 130f, 100f, 100f, 80f, 80f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Nome", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Cargo", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Telefone", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Celular", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    foreach (CRM_CONTATO item in aten.CRM_CONTATO)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRCO_NM_NOME, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(item.CRCO_NM_CARGO, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(item.CRCO_NM_EMAIL, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(item.CRCO_NR_TELEFONE, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell = new PdfPCell(new Paragraph(item.CRCO_NR_CELULAR, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    pdfDoc.Add(table);
                }

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Dados Ações
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Ações", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.CRM_ACAO.Count > 0)
                {
                    table = new PdfPTable(new float[] { 120f, 80f, 80f, 100f, 80f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Título", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Criação", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Previsão", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Dias (Prevista)", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Status", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    foreach (CRM_ACAO item in aten.CRM_ACAO)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRAC_NM_TITULO, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(item.CRAC_DT_CRIACAO.Value.ToShortDateString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        if (item.CRAC_DT_PREVISTA > DateTime.Today.Date)
                        {
                            cell = new PdfPCell(new Paragraph(item.CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontVerde))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else if (item.CRAC_DT_PREVISTA == DateTime.Today.Date)
                        {
                            cell = new PdfPCell(new Paragraph(item.CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontAzul))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph(item.CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontAzul))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        table.AddCell(cell);

                        if ((item.CRAC_DT_PREVISTA.Value.Date - DateTime.Today.Date).Days > 0)
                        {
                            cell = new PdfPCell(new Paragraph((item.CRAC_DT_PREVISTA.Value.Date - DateTime.Today.Date).Days.ToString(), meuFontVerde))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph((item.CRAC_DT_PREVISTA.Value.Date - DateTime.Today.Date).Days.ToString(), meuFontVermelho))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        table.AddCell(cell);

                        if (item.CRAC_IN_STATUS == 1)
                        {
                            cell = new PdfPCell(new Paragraph("Ativa", meuFontVerde))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else if (item.CRAC_IN_STATUS == 2)
                        {
                            cell = new PdfPCell(new Paragraph("Pendente", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else if (item.CRAC_IN_STATUS == 3)
                        {
                            cell = new PdfPCell(new Paragraph("Encerrada", meuFontAzul))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else if (item.CRAC_IN_STATUS == 4)
                        {
                            cell = new PdfPCell(new Paragraph("Excluída", meuFontVermelho))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        table.AddCell(cell);
                    }
                    pdfDoc.Add(table);
                }

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "relCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarPropostaLine(Int32 id)
        {
            Session["IdCRMProposta"] = id;
            return RedirectToAction("GerarRelatorioProposta");
        }

        public ActionResult GerarPropostaLinePedido(Int32 id)
        {
            Session["IdCRMPedido"] = id;
            return RedirectToAction("GerarRelatorioPedido");
        }

        public ActionResult GerarRelatorioPedido()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                CRM_PEDIDO_VENDA aten = baseApp.GetPedidoById((Int32)Session["IdCRMPedido"]);
                CRM crm = baseApp.GetItemById(aten.CRM1_CD_ID.Value);
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "CRM_Proposta_" + aten.CRPV_IN_NUMERO_GERADO.ToString() + "_" + data + ".pdf";
                CLIENTE cliente = cliApp.GetItemById(crm.CLIE_CD_ID);
                Session["VoltaCRM"] = 0;
                ASSINANTE assi = (ASSINANTE)Session["Assinante"];

                // Define fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontVerde = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
                Font meuFontAzul= FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Font meuFontVermelho = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.RED);
                Font meuFontOrange = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.ORANGE);
                Font meuFontTitulo = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontGreen = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY);

                // Preparar campos de texto HTML
                String intro = HtmlToPlainText(aten.CRPV_TX_INTRODUCAO);
                String corpo = HtmlToPlainText(aten.CRPV_TX_INFORMACOES_GERAIS);
                String rodape = HtmlToPlainText(aten.CRPV_TX_OUTROS_ITENS);
                String comercial = HtmlToPlainText(aten.CRPV_TX_CONDICOES_COMERCIAIS);

                String intro1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_INTRODUCAO);
                String corpo1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_INFORMACOES_GERAIS);
                String rodape1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_OUTROS_ITENS);
                String comercial1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_CONDICOES_COMERCIAIS);

                intro1 = intro1.Replace("\r\n\r\n", "\r\n");
                intro1 = intro1.Replace("<p>", "");
                intro1 = intro1.Replace("</p>", "<br />");
                corpo1 = corpo1.Replace("\r\n\r\n", "\r\n");
                corpo1 = corpo1.Replace("<p>", "");
                corpo1 = corpo1.Replace("</p>", "<br />");
                rodape1 = rodape1.Replace("\r\n\r\n", "\r\n");
                rodape1 = rodape1.Replace("<p>", "");
                rodape1 = rodape1.Replace("</p>", "<br />");
                comercial1 = comercial1.Replace("\r\n\r\n", "\r\n");
                comercial1 = comercial1.Replace("<p>", "");
                comercial1 = comercial1.Replace("</p>", "<br />");

                intro1 = intro1.Replace("{Nome}", cliente.CLIE_NM_NOME);
                rodape1 = rodape1.Replace("{Assinatura}", assi.ASSI_NM_NOME);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Cabeçalho
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Proposta - Especificações", meuFontTitulo))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 4;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Introdução
                Chunk chunk1 = new Chunk(intro1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);
                Chunk chunk21 = new Chunk(corpo1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk21);
                Chunk chunk22 = new Chunk(rodape1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk22);

                // Dados do Cliente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados do Cliente", meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("    ", meuFontOrange));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome: " + cliente.CLIE_NM_NOME, meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (cliente.CLIE_NM_ENDERECO != null)
                {
                    cell = new PdfPCell(new Paragraph("Endereço: " + cliente.CLIE_NM_ENDERECO + " " + cliente.CLIE_NR_NUMERO + " " + cliente.CLIE_NM_COMPLEMENTO, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    if (cliente.UF != null)
                    {
                        cell = new PdfPCell(new Paragraph("                " + cliente.CLIE_NM_BAIRRO + " - " + cliente.CLIE_NM_CIDADE + " - " + cliente.UF.UF_SG_SIGLA + " - " + cliente.CLIE_NR_CEP, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("                " + cliente.CLIE_NM_BAIRRO + " - " + cliente.CLIE_NM_CIDADE + " - " + cliente.CLIE_NR_CEP, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                    }
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Endereço: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                cell = new PdfPCell(new Paragraph("Telefone: " + cliente.CLIE_NR_TELEFONE, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular: " + cliente.CLIE_NR_CELULAR, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail: " + cliente.CLIE_NM_EMAIL, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.WHITE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Dados da Proposta
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados da Proposta", meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("    ", meuFontOrange));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Identificação: " + aten.CRPV_NM_NOME, meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número: " + aten.CRPV_IN_NUMERO_GERADO.ToString(), meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Emissão: " + aten.CRPV_DT_PEDIDO.ToShortDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Validade: " + aten.CRPV_DT_VALIDADE.ToShortDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável: " + aten.USUARIO.USUA_NM_NOME, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.WHITE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados Comerciais", meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                Chunk chunk31 = new Chunk(comercial1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk31);

                // Dados da Financeiros
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados Financeiros", meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("    ", meuFontOrange));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Valor Proposta (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_VALOR.Value), meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Desconto (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_DESCONTO.Value), meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Frete (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_FRETE.Value), meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Taxas (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_ICMS.Value), meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_TOTAL.Value), meuFontAzul));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "relCRPV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);
            text = text.Replace("<p>", "");
            text = text.Replace("</p>", "");
            return text;
        }

        public ActionResult VerCRMExpansao()
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
                    Session["MensCRM"] = 2;
                    return RedirectToAction("Voltar");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            DateTime inicio = Convert.ToDateTime("01/" + DateTime.Today.Date.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Today.Date.Year.ToString());
            DateTime hoje = DateTime.Today.Date;
            Session["Hoje"] = hoje;
            if (Session["ListaCRMCheia"] == null)
            {
                List<CRM> listaCRMCheia = CarregaCRMGeral();
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            if (Session["ListaCRM"] == null)
            {
                List<CRM> listaCRM = CarregaCRM();
                Session["ListaCRM"] = listaCRM;
            }
            // Retorna
            return View();
        }

        public JsonResult GetDadosProcessosStatus()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            List<CRM> listaCRMCheia = new List<CRM>();

            if (Session["ListaCRMCheia"] == null)
            {
                listaCRMCheia = CarregaCRMGeral();
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            else
            {
                listaCRMCheia = (List<CRM>)Session["ListaCRMCheia"];
            }

            // Prepara
            List<CRMDTOViewModel> lista = new List<CRMDTOViewModel>();
            CRMDTOViewModel dto = new CRMDTOViewModel();

            // Carrega vetores
            Int32 prosp = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 1).ToList().Count;
            desc.Add("Prospecção");
            quant.Add(prosp);
            dto.DESCRICAO = "Prospecção";
            dto.QUANTIDADE = prosp;
            lista.Add(dto);

            Int32 cont = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 2).ToList().Count;
            desc.Add("Contato Realizado");
            quant.Add(cont);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Contato Realizado";
            dto.QUANTIDADE = cont;
            lista.Add(dto);

            Int32 prop = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 3).ToList().Count;
            desc.Add("Proposta Enviada");
            quant.Add(prop);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Proposta Enviada";
            dto.QUANTIDADE = prop;
            lista.Add(dto);

            Int32 neg = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 4).ToList().Count;
            desc.Add("Em Negociação");
            quant.Add(neg);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Em Negociação";
            dto.QUANTIDADE = neg;
            lista.Add(dto);

            Int32 enc = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 5).ToList().Count;
            desc.Add("Encerrado");
            quant.Add(enc);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Encerrado";
            dto.QUANTIDADE = enc;
            lista.Add(dto);
            Session["ListaProcessosStatus"] = lista;

            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#D63131");
            cor.Add("#27A1C6");

            // retorna
            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosProcessosSituacao()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            List<CRM> listaCRMCheia = new List<CRM>();

            if (Session["ListaCRMCheia"] == null)
            {
                listaCRMCheia = CarregaCRMGeral();
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            else
            {
                listaCRMCheia = (List<CRM>)Session["ListaCRMCheia"];
            }

            // Prepara
            List<CRMDTOViewModel> listaSit = new List<CRMDTOViewModel>();
            CRMDTOViewModel dto = new CRMDTOViewModel();

            // Carrega vetores
            Int32 prosp = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 1).ToList().Count;
            desc.Add("Ativos");
            quant.Add(prosp);
            dto.DESCRICAO = "Ativos";
            dto.QUANTIDADE = prosp;
            listaSit.Add(dto);

            Int32 cont = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 2).ToList().Count;
            desc.Add("Arquivados");
            quant.Add(cont);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Arquivados";
            dto.QUANTIDADE = cont;
            listaSit.Add(dto);

            Int32 prop = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 3).ToList().Count;
            desc.Add("Cancelados");
            quant.Add(prop);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Cancelados";
            dto.QUANTIDADE = prop;
            listaSit.Add(dto);

            Int32 neg = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 4).ToList().Count;
            desc.Add("Falhados");
            quant.Add(neg);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Falhados";
            dto.QUANTIDADE = neg;
            listaSit.Add(dto);

            Int32 enc = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 5).ToList().Count;
            desc.Add("Sucesso");
            quant.Add(enc);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Sucesso";
            dto.QUANTIDADE = enc;
            listaSit.Add(dto);
            Session["ListaProcessosSituacao"] = listaSit;

            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#D63131");
            cor.Add("#27A1C6");

            // retorna
            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public ActionResult VerProcessosStatusExpansao()
        {
            // Prepara view
            List<CRMDTOViewModel> lista = (List<CRMDTOViewModel>)Session["ListaProcessosStatus"];
            ViewBag.Lista = lista;
            return View();
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult EnviarEMailCliente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_MENSAGEM_CLIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensCRM"] = 200;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                if (Session["MensMensagem"] != null)
                {
                    if ((Int32)Session["MensMensagem"] == 66)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                    }
                }

                // recupera cliente e assinante
                Session["PontoProposta"] = 85;
                CLIENTE cli = cliApp.GetItemById(id);
                ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                Session["Cliente"] = cli;

                // Prepara mensagem
                String header = "Prezado <b>" + cli.CLIE_NM_NOME + "</b>";
                String body = String.Empty;
                String footer = "<b>" + assi.ASSI_NM_NOME + "</b>";

                // Monta vm
                MensagemViewModel vm = new MensagemViewModel();
                vm.ASSI_CD_ID = idAss;
                vm.MENS_DT_CRIACAO = DateTime.Now;
                vm.MENS_IN_ATIVO = 1;
                vm.NOME = cli.CLIE_NM_NOME;
                vm.ID = id;
                vm.MODELO = cli.CLIE_NM_EMAIL;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.MENS_NM_CABECALHO = header;
                vm.MENS_NM_RODAPE = footer;
                vm.MENS_IN_TIPO = 1;
                vm.ID = cli.CLIE_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EnviarEMailCliente(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Int32 idNot = (Int32)Session["IdCRM"];
                try
                {
                    // Checa corpo da mensagem
                    if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO))
                    {
                        Session["MensMensagem"] = 66;
                        return RedirectToAction("EnviarEMailCliente");
                    }

                    // Sanitização
                    CLIENTE cont = (CLIENTE)Session["Cliente"];
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Mensagem para " + cont.CLIE_NM_NOME;
                    vm.MENS_NM_CAMPANHA = cont.CLIE_NM_EMAIL;

                    // Prepara HTML
                    vm.MENS_TX_TEXTO = HtmlToPlainText(vm.MENS_TX_TEXTO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    Int32 voltaX = await envio.ProcessaEnvioEMailGeral(vm, usuarioLogado);

                    // Grava envio
                    String guid = new Guid().ToString();
                    Int32 volta1 = envio.GravarMensagemEnviada(vm, usuarioLogado, vm.MENS_TX_TEXTO, "Success", guid, null, "Mensagem de E-Mail para Cliente");

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "emaCLIE",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.CLIE_NM_NOME + " - E-Mail: " + cont.CLIE_NM_EMAIL,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["VoltaTela"] = 0;
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardCRMNovo()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["VoltaPedido"] = 1;

                // Recupera listas
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                List<CRM> lt = CarregaCRM().Where(p => p.CRM1_DT_CRIACAO > limite).ToList();
                List<CRM> lm = lt.Where(p => p.CRM1_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.CRM1_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
                List<CRM> la = lt.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
                List<CRM> lq = lt.Where(p => p.CRM1_IN_ATIVO == 2).ToList();
                List<CRM> ls = lt.Where(p => p.CRM1_IN_ATIVO == 5).ToList();
                List<CRM> lc = lt.Where(p => p.CRM1_IN_ATIVO == 3).ToList();
                List<CRM> lf = lt.Where(p => p.CRM1_IN_ATIVO == 4).ToList();

                List<CRM_ACAO> acoes = baseApp.GetAllAcoes(idAss).Where(p => p.CRM.CRM1_IN_ATIVO != 2 & p.CRAC_DT_CRIACAO > limite).ToList();
                List<CRM_ACAO> acoesPend = acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList();
                List<CRM_ACAO> acoesAtraso = acoes.Where(p => p.CRAC_IN_STATUS == 1 & p.CRAC_DT_PREVISTA.Value.Date < DateTime.Today.Date).ToList();

                List<CRM_FOLLOW> follows = baseApp.GetAllFollow(idAss).Where(p => p.CRM.CRM1_IN_ATIVO != 2 & p.CRFL_DT_FOLLOW > limite).ToList();
                List<CRM_FOLLOW> follows_Lembra = follows.Where(p => p.TIFL_CD_ID == 3 & p.CRFL_DT_PREVISTA.Value.Date >= DateTime.Today.Date).ToList();
                List<CRM_FOLLOW> follows_Alerta = follows.Where(p => p.TIFL_CD_ID == 2 & p.CRFL_DT_PREVISTA.Value.Date >= DateTime.Today.Date).ToList();

                List<CLIENTE> cli = CarregaCliente();
                List<CRM_PEDIDO_VENDA> peds = CarregaPedidoVenda().Where(p => p.CRM.CRM1_IN_ATIVO != 2).ToList();
                List<CRM_PEDIDO_VENDA> lmp1 = peds.Where(p => p.CRPV_DT_PEDIDO.Month == DateTime.Today.Date.Month & p.CRPV_DT_PEDIDO.Year == DateTime.Today.Date.Year).ToList();

                List<FUNIL> funs = CarregaFunil();

                // Estatisticas 
                ViewBag.Total = lt.Count;
                ViewBag.TotalAtivo = la.Count;
                ViewBag.TotalSucesso = ls.Count;
                ViewBag.TotalCancelado = lc.Count;
                ViewBag.Acoes = acoes.Count;
                ViewBag.AcoesPend = acoesPend.Count;
                ViewBag.Clientes = cli.Count;

                ViewBag.TotalPes = lt.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
                ViewBag.TotalAtivoPes = la.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
                ViewBag.TotalSucessoPes = ls.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
                ViewBag.TotalCanceladoPes = lc.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
                ViewBag.AcoesPes = acoes.Where(p => p.CRM.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
                ViewBag.AcoesPendPes = acoesPend.Where(p => p.CRM.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;

                Session["ListaCRM"] = lt;
                Session["ListaCRMMes"] = lm;
                Session["ListaCRMAtivo"] = la;
                Session["ListaCRMSucesso"] = ls;
                Session["ListaCRMCanc"] = lc;
                Session["ListaCRMAcoes"] = acoes;
                Session["ListaCRMAcoesPend"] = acoesPend;
                Session["ListaPedidosMes"] = lmp1;

                Session["CRMAtivos"] = la.Count;
                Session["CRMArquivados"] = lq.Count;
                Session["CRMCancelados"] = lc.Count;
                Session["CRMFalhados"] = lf.Count;
                Session["CRMSucessos"] = ls.Count;

                Session["CRMProsp"] = lt.Where(p => p.CRM1_IN_STATUS == 1).ToList().Count;
                Session["CRMCont"] =  lt.Where(p => p.CRM1_IN_STATUS == 2).ToList().Count;
                Session["CRMProp"] =  lt.Where(p => p.CRM1_IN_STATUS == 3).ToList().Count;
                Session["CRMNego"] =  lt.Where(p => p.CRM1_IN_STATUS == 4).ToList().Count;
                Session["CRMEnc"] =  lt.Where(p => p.CRM1_IN_STATUS == 5).ToList().Count;
                Session["IdCRM"] = null;

                Session["AcaoAtiva"] = acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count;
                Session["AcaoPendente"] = acoes.Where(p => p.CRAC_IN_STATUS == 2).ToList().Count;
                Session["AcaoEncerrada"] = acoes.Where(p => p.CRAC_IN_STATUS == 3).ToList().Count;
                Int32 x = acoes.Where(p => p.CRAC_IN_STATUS == 3).ToList().Count;
                Int32 y = acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count;

                Session["PedElaboracao"] = peds.Where(p => p.CRPV_IN_STATUS == 1).ToList().Count;
                Session["PedEnviada"] = peds.Where(p => p.CRPV_IN_STATUS == 2).ToList().Count;
                Session["PedCancelada"] = peds.Where(p => p.CRPV_IN_STATUS == 3).ToList().Count;
                Session["PedReprovada"] = peds.Where(p => p.CRPV_IN_STATUS == 4).ToList().Count;
                Session["PedAprovada"] = peds.Where(p => p.CRPV_IN_STATUS == 5).ToList().Count;

                // Resumo Diario CRM
                List<DateTime> datas = lm.Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMMesResumo"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        if (item.Date > limite)
                        {
                            Int32 conta = lm.Where(p => p.CRM1_DT_CRIACAO.Value.Date == item).Count();
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.DataEmissao = item;
                            mod.Valor = conta;
                            lista.Add(mod);
                        }
                    }
                    ViewBag.ListaCRMMes = lista;
                    ViewBag.ContaCRMMes = lm.Count;
                    Session["ListaDatasCRM"] = datas;
                    Session["ListaCRMMesResumo"] = lista;
                    Session["ListaCRMMesResumoConta"] = lista.Count;
                }
                else
                {
                    ViewBag.ListaCRMMes = (List<ModeloViewModel>)Session["ListaCRMMesResumo"];
                    ViewBag.ContaCRMMes = (Int32)Session["ListaCRMMesResumoConta"];

                }
                Session["ListaDatasCRM"] = datas;
                Session["VoltaKB"] = 0;

                // Resumo Mes CRM
                String mes = null;
                String mesFeito = null;
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMMeses"] == null)
                {
                    datas = lt.Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));

                    List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        if (item.Date > limite)
                        {
                            mes = item.Month.ToString() + "/" + item.Year.ToString();
                            if (mes != mesFeito)
                            {
                                Int32 conta = lt.Where(p => p.CRM1_DT_CRIACAO.Value.Date.Month == item.Month & p.CRM1_DT_CRIACAO.Value.Date.Year == item.Year & p.CRM1_DT_CRIACAO > limite).Count();
                                ModeloViewModel mod = new ModeloViewModel();
                                mod.Nome = mes;
                                mod.Valor = conta;
                                listaMes.Add(mod);
                                mesFeito = item.Month.ToString() + "/" + item.Year.ToString();
                            }
                        }
                    }
                    ViewBag.ListaCRMMeses = listaMes;
                    ViewBag.ContaCRMMeses = listaMes.Count;
                    Session["ListaDatasCRM"] = datas;
                    Session["ListaCRMMeses"] = listaMes;
                    Session["ListaCRMMesesConta"] = listaMes.Count;
                }
                else
                {
                    ViewBag.ListaCRMMeses = (List<ModeloViewModel>)Session["ListaCRMMeses"];
                    ViewBag.ContaCRMMeses = (Int32)Session["ContaCRMMeses"];
                }

                // Resumo Diario Pedidos
                List<DateTime> datasPed = lmp1.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
                if ((Int32)Session["CRMPedidoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaPedidosMesResumo"] == null)
                {
                    datasPed.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> listaPed = new List<ModeloViewModel>();
                    foreach (DateTime item in datasPed)
                    {
                        if (item.Date > limite)
                        {
                            Int32 conta = lmp1.Where(p => p.CRPV_DT_PEDIDO.Date == item).Count();
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.DataEmissao = item;
                            mod.Valor = conta;
                            listaPed.Add(mod);
                        }
                    }
                    ViewBag.ListaPedidosMes = listaPed;
                    ViewBag.ContaPedidosMes = lmp1.Count;
                    Session["ListaDatasPed"] = datasPed;
                    Session["ListaPedidosMesResumo"] = listaPed;
                    Session["ListaPedidosMesResumoConta"] = lmp1.Count;
                }
                else
                {
                    ViewBag.ListaPedidosMes = (List<ModeloViewModel>)Session["ListaPedidosMesResumo"];
                    ViewBag.ContaPedidosMes = (Int32)Session["ListaPedidosMesResumoConta"];
                }
                Session["ListaDatasPed"] = datasPed;

                // Resumo Mes Pedidos
                datasPed = peds.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
                datasPed.Sort((i, j) => i.Date.CompareTo(j.Date));
                Session["ListaMesesPed"] = datasPed;

                if ((Int32)Session["CRMPedidoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaPedidosMeses"] == null)
                {
                    List<ModeloViewModel> listaMesPed = new List<ModeloViewModel>();
                    mes = null;
                    mesFeito = null;
                    foreach (DateTime item in datasPed)
                    {
                        if (item.Date > limite)
                        {
                            mes = item.Month.ToString() + "/" + item.Year.ToString();
                            if (mes != mesFeito)
                            {
                                Int32 conta = peds.Where(p => p.CRPV_DT_PEDIDO.Date.Month == item.Month & p.CRPV_DT_PEDIDO.Date.Year == item.Year & p.CRPV_DT_PEDIDO > limite).Count();
                                ModeloViewModel mod = new ModeloViewModel();
                                mod.Nome = mes;
                                mod.Valor = conta;
                                listaMesPed.Add(mod);
                                mesFeito = item.Month.ToString() + "/" + item.Year.ToString();
                            }
                        }
                    }
                    ViewBag.ListaMesesPed = listaMesPed;
                    ViewBag.ContaMesesPed = listaMesPed.Count;
                    Session["ListaPedidosMeses"] = listaMesPed;
                    Session["ListaPedidosMesesConta"] = listaMesPed.Count;
                }
                else
                {
                    ViewBag.ListaMesesPed = (List<ModeloViewModel>)Session["ListaPedidosMeses"];
                    ViewBag.ContaMesesPed = (Int32)Session["ListaPedidosMesesConta"];
                }

                // Resumo Situacao CRM 
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMSituacao"] == null)
                {
                    List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
                    for (int i = 1; i < 6; i++)
                    {
                        Int32 conta = lt.Where(p => p.CRM1_IN_ATIVO == i & p.CRM1_DT_CRIACAO > limite).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Data = i == 1 ? "Ativo" : (i == 2 ? "Arquivados" : (i == 3 ? "Cancelados" : (i == 4 ? "Falhados" : (i == 5 ? "Sucesso" : (i == 6 ? "Faturamento" : (i == 7 ? "Expedição" : "Entregue"))))));
                        mod.Valor = conta;
                        lista1.Add(mod);
                    }
                    ViewBag.ListaCRMSituacao = lista1;
                    Session["ListaCRMSituacao"] = lista1;
                }
                else
                {
                    ViewBag.ListaCRMSituacao = (List<ModeloViewModel>)Session["ListaCRMSituacao"];
                }

                // Resumo Status CRM 
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMStatus"] == null)
                {
                    List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
                    for (int i = 1; i < 6; i++)
                    {
                        Int32 conta = lt.Where(p => p.CRM1_IN_STATUS == i & p.CRM1_DT_CRIACAO > limite).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Data = i == 1 ? "Prospecção" : (i == 2 ? "Contato Realizado" : (i == 3 ? "Proposta Apresentada" : (i == 4 ? "Negociação" : "Encerrado")));
                        mod.Valor = conta;
                        lista2.Add(mod);
                    }
                    ViewBag.ListaCRMStatus = lista2;
                    Session["ListaCRMStatus"] = lista2;
                }
                else
                {
                    ViewBag.ListaCRMStatus = (List<ModeloViewModel>)Session["ListaCRMStatus"];
                }


                // Resumo ações
                if ((Int32)Session["CRMAcaoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMAcao"] == null)
                {
                    List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
                    for (int i = 1; i < 4; i++)
                    {
                        Int32 conta = acoes.Where(p => p.CRAC_IN_STATUS == i).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = i == 1 ? "Ativa" : (i == 2 ? "Pendente" : "Encerrada");
                        mod.Valor = conta;
                        lista3.Add(mod);
                    }
                    ViewBag.ListaCRMAcao = lista3;
                    Session["ListaCRMAcao"] = lista3;
                }
                else
                {
                    ViewBag.ListaCRMAcao = (List<ModeloViewModel>)Session["ListaCRMAcao"];
                }

                // Resumo funil
                List<Int32> funis = lt.Select(p => p.FUNI_CD_ID.Value).Distinct().ToList();
                Session["ListaFunil1"] = funis;
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaFunilResumo"] == null)
                {
                    List<ModeloViewModel> listaFunil = new List<ModeloViewModel>();
                    foreach (Int32 item in funis)
                    {
                        Int32 conta = lt.Where(p => p.FUNI_CD_ID == item & p.CRM1_DT_CRIACAO > limite).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Valor1 = item;
                        mod.Valor = conta;
                        mod.Nome = funs.Where(p => p.FUNI_CD_ID == item).First().FUNI_NM_NOME;
                        listaFunil.Add(mod);
                    }
                    ViewBag.ListaFunil = listaFunil;
                    ViewBag.ContaFunil = listaFunil.Count;
                    Session["ListaFunilResumo"] = listaFunil;
                    Session["ContaFunil"] = listaFunil.Count;
                }
                else
                {
                    ViewBag.ListaFunil = (List<ModeloViewModel>)Session["ListaFunilResumo"];
                    ViewBag.ContaFunil = (Int32)Session["ContaFunil"];
                }

                // Resumo Pedidos
                if ((Int32)Session["CRMPedidoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMPed"] == null)
                {
                    List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
                    for (int i = 1; i < 6; i++)
                    {
                        Int32 conta = peds.Where(p => p.CRPV_IN_STATUS == i).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviado" : (i == 3 ? "Cancelado" : (i == 4 ? "Reprovado" : "Aprovado")));
                        mod.Valor = conta;
                        lista5.Add(mod);
                    }
                    ViewBag.ListaCRMPed = lista5;
                    Session["ListaCRMPed"] = lista5;
                }
                else
                {
                    ViewBag.ListaCRMPed = (List<ModeloViewModel>)Session["ListaCRMPed"];
                }

                // Recupera processos por etapa
                limite = DateTime.Today.Date.AddMonths(-12);
                funis = lt.Select(p => p.FUNI_CD_ID.Value).Distinct().ToList();
                Session["ListaEtapa"] = funis;
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaEtapaResumo"] == null)
                {
                    List<ModeloViewModel> lista12 = new List<ModeloViewModel>();
                    foreach (Int32 item in funis)
                    {
                        FUNIL funil = funs.Where(p => p.FUNI_CD_ID == item).First();
                        List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.ToList();
                        foreach (FUNIL_ETAPA etapa in etapas)
                        {
                            Int32 conta1 = lt.Where(p => p.FUNI_CD_ID.Value == item & p.CRM1_IN_STATUS == etapa.FUET_CD_ID & p.CRM1_DT_CRIACAO > limite).ToList().Count;
                            if (conta1 > 0)
                            {
                                ModeloViewModel modX = new ModeloViewModel();
                                modX.Valor = conta1;
                                modX.Nome = funs.Where(p => p.FUNI_CD_ID == item).First().FUNI_NM_NOME;
                                modX.Nome1 = etapa.FUET_NM_NOME;
                                lista12.Add(modX);
                            }
                        }
                    }
                    ViewBag.ListaEtapa = lista12;
                    ViewBag.ContaEtapa = lista12.Count;
                    Session["ListaEtapa"] = funis;
                    Session["ListaEtapaResumo"] = lista12;
                    Session["ContaEtapa"] = lista12.Count;
                }
                else
                {
                    ViewBag.ListaEtapa = (List<ModeloViewModel>)Session["ListaEtapaResumo"];
                    ViewBag.ContaEtapa = (Int32)Session["ContaEtapa"];
                }

                // Recupera ações pendentes
                if ((Int32)Session["CRMAcaoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaPendencia"] == null)
                {
                    List<ModeloViewModel> lista15 = new List<ModeloViewModel>();
                    foreach (CRM_ACAO item in acoesPend)
                    {
                        ModeloViewModel modZ = new ModeloViewModel();
                        modZ.Nome = item.CRAC_NM_TITULO;
                        modZ.DataEmissao = item.CRAC_DT_PREVISTA.Value;
                        modZ.Valor = item.CRAC_CD_ID;
                        lista15.Add(modZ);
                    }
                    ViewBag.ListaPend = lista15;
                    ViewBag.ContaPend = lista15.Count;
                    Session["ListaPendencia"] = lista15;
                    Session["ContaPend"] = lista15.Count;
                }
                else
                {
                    ViewBag.ListaPend = (List<ModeloViewModel>)Session["ListaPendencia"];
                    ViewBag.ContaPend = (Int32)Session["ContaPend"];
                }

                // Recupera ações atrasadas
                if ((Int32)Session["CRMAcaoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaAtraso"] == null)
                {
                    List<ModeloViewModel> lista16 = new List<ModeloViewModel>();
                    foreach (CRM_ACAO item in acoesAtraso)
                    {
                        ModeloViewModel modZ = new ModeloViewModel();
                        modZ.Nome = item.CRAC_NM_TITULO;
                        modZ.DataEmissao = item.CRAC_DT_PREVISTA.Value;
                        modZ.Valor = item.CRAC_CD_ID;
                        lista16.Add(modZ);
                    }
                    ViewBag.ListaAtraso = lista16;
                    ViewBag.ContaAtraso = lista16.Count;
                    Session["ListaAtraso"] = lista16;
                    Session["ContaAtraso"] = lista16.Count;
                }
                else
                {
                    ViewBag.ListaAtraso = (List<ModeloViewModel>)Session["ListaAtraso"];
                    ViewBag.ContaAtraso = (Int32)Session["ContaAtraso"];
                }

                // Recupera lembretes
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaLemb"] == null)
                {
                    List<ModeloViewModel> lista17 = new List<ModeloViewModel>();
                    foreach (CRM_FOLLOW item in follows_Lembra)
                    {
                        ModeloViewModel modZ = new ModeloViewModel();
                        modZ.Nome = item.CRFL_NM_TITULO;
                        modZ.DataEmissao = item.CRFL_DT_PREVISTA.Value;
                        modZ.Valor = item.CRFL_CD_ID;
                        lista17.Add(modZ);
                    }
                    ViewBag.ListaLemb = lista17;
                    ViewBag.ContaLemb = lista17.Count;
                    Session["ListaLemb"] = lista17;
                    Session["ContaLemb"] = lista17.Count;
                }
                else
                {
                    ViewBag.ListaLemb = (List<ModeloViewModel>)Session["ListaLemb"];
                    ViewBag.ContaLemb = (Int32)Session["ContaLemb"];
                }

                // Recupera alertas
                if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaLemb"] == null)
                {
                    List<ModeloViewModel> lista19 = new List<ModeloViewModel>();
                    foreach (CRM_FOLLOW item in follows_Alerta)
                    {
                        ModeloViewModel modZ = new ModeloViewModel();
                        modZ.Nome = item.CRFL_NM_TITULO;
                        modZ.DataEmissao = item.CRFL_DT_PREVISTA.Value;
                        modZ.Valor = item.CRFL_CD_ID;
                        lista19.Add(modZ);
                    }
                    ViewBag.ListaAlerta = lista19;
                    ViewBag.ContaAlerta = lista19.Count;
                    Session["ListaAlerta"] = lista19;
                    Session["ContaAlerta"] = lista19.Count;
                }
                else
                {
                    ViewBag.ListaAlerta = (List<ModeloViewModel>)Session["ListaAlerta"];
                    ViewBag.ContaAlerta = (Int32)Session["ContaAlerta"];
                }

                // Retorno
                Session["PontoAcao"] = 100;
                Session["VoltaPedido"] = 1;
                Session["VoltaProdutoDash"] = 1;
                Session["FlagMensagensEnviadas"] = 6;
                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardVendas()
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
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
                if ((Int32)Session["PermCRM"] == 0)
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas
            List<CRM_PEDIDO_VENDA> peds = baseApp.GetAllPedidosVenda(idAss);
            List<CRM_PEDIDO_VENDA> lmp1 = peds.Where(p => p.CRPV_DT_PEDIDO.Month == DateTime.Today.Date.Month & p.CRPV_DT_PEDIDO.Year == DateTime.Today.Date.Year).ToList();

            List<CRM_PEDIDO_VENDA> pedsPes = peds.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            List<CRM_PEDIDO_VENDA> lmp1Pes = pedsPes.Where(p => p.CRPV_DT_PEDIDO.Month == DateTime.Today.Date.Month & p.CRPV_DT_PEDIDO.Year == DateTime.Today.Date.Year).ToList();

            // Estatisticas 
            Session["ListaPedidosMes"] = lmp1;
            Session["PedElaboracao"] = peds.Where(p => p.CRPV_IN_STATUS == 1).ToList().Count;
            Session["PedCancelada"] = peds.Where(p => p.CRPV_IN_STATUS == 3).ToList().Count;
            Session["PedFatura"] = peds.Where(p => p.CRPV_IN_STATUS == 6).ToList().Count;
            Session["PedExpedicao"] = peds.Where(p => p.CRPV_IN_STATUS == 7).ToList().Count;
            Session["PedEntregue"] = peds.Where(p => p.CRPV_IN_STATUS == 8).ToList().Count;

            Session["ListaPedidosMesPes"] = lmp1Pes;
            Session["PedElaboracaoPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 1).ToList().Count;
            Session["PedCanceladaPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 3).ToList().Count;
            Session["PedFaturaPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 6).ToList().Count;
            Session["PedExpedicaoPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 7).ToList().Count;
            Session["PedEntreguePes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 8).ToList().Count;

            ViewBag.Elaboracao = (Int32)Session["PedElaboracao"];
            ViewBag.Cancelado = (Int32)Session["PedCancelada"];
            ViewBag.Fatura = (Int32)Session["PedFatura"];
            ViewBag.Expedicao = (Int32)Session["PedExpedicao"];
            ViewBag.Entregue = (Int32)Session["PedEntregue"];

            ViewBag.ElaboracaoPes = (Int32)Session["PedElaboracaoPes"];
            ViewBag.CanceladoPes = (Int32)Session["PedCanceladaPes"];
            ViewBag.FaturaPes = (Int32)Session["PedFaturaPes"];
            ViewBag.ExpedicaoPes = (Int32)Session["PedExpedicaoPes"];
            ViewBag.EntreguePes = (Int32)Session["PedEntreguePes"];

            // Resumo Mes Pedidos
            List<DateTime> datasPed = lmp1.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
            List<ModeloViewModel> listaPed = new List<ModeloViewModel>();
            foreach (DateTime item in datasPed)
            {
                Int32 conta = lmp1.Where(p => p.CRPV_DT_PEDIDO.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                listaPed.Add(mod);
            }
            ViewBag.ListaPedidosMes = listaPed;
            ViewBag.ContaPedidosMes = lmp1.Count;
            Session["ListaDatasPed"] = datasPed;
            Session["ListaPedidosMesResumo"] = listaPed;

            // Resumo Mes Pedidos Pessoal
            List<DateTime> datasPedPes = lmp1Pes.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
            List<ModeloViewModel> listaPedPes = new List<ModeloViewModel>();
            foreach (DateTime item in datasPedPes)
            {
                Int32 conta = lmp1Pes.Where(p => p.CRPV_DT_PEDIDO.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                listaPedPes.Add(mod);
            }
            ViewBag.ListaPedidosMesPes = listaPedPes;
            ViewBag.ContaPedidosMesPes = lmp1Pes.Count;
            Session["ListaDatasPedPes"] = datasPedPes;
            Session["ListaPedidosMesResumoPes"] = listaPedPes;

            // Resumo Pedidos
            List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = peds.Where(p => p.CRPV_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviado" : (i == 3 ? "Cancelado" : (i == 4 ? "Reprovado" : (i == 5 ? "Aprovado" : (i == 6 ? "Faturamento" : (i == 7 ? "Expedição" : "Entregue" ))))));
                mod.Valor = conta;
                lista5.Add(mod);
            }
            ViewBag.ListaCRMPed = lista5;
            Session["ListaCRMPed"] = lista5;

            // Resumo PedidosPessoal
            List<ModeloViewModel> lista5Pes = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = pedsPes.Where(p => p.CRPV_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviado" : (i == 3 ? "Cancelado" : (i == 4 ? "Reprovado" : (i == 5 ? "Aprovado" : (i == 6 ? "Faturamento" : (i == 7 ? "Expedição" : "Entregue"))))));
                mod.Valor = conta;
                lista5Pes.Add(mod);
            }
            ViewBag.ListaCRMPedPes = lista5Pes;
            Session["ListaCRMPedPes"] = lista5Pes;
            Session["VoltaProdutoDash"] = 1;
            return View(vm);
        }

        public JsonResult GetDadosGraficoCRMSituacao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["CRMAtivos"];
            Int32 q2 = (Int32)Session["CRMArquivados"];
            Int32 q3 = (Int32)Session["CRMCancelados"];
            Int32 q4 = (Int32)Session["CRMFalhados"];
            Int32 q5 = (Int32)Session["CRMSucessos"];
            Int32 q6 = (Int32)Session["CRMFatura"];
            Int32 q7 = (Int32)Session["CRMExpedicao"];
            Int32 q8 = (Int32)Session["CRMEntregue"];

            desc.Add("Ativos");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Arquivados");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Falhados");
            quant.Add(q4);
            cor.Add("#a5d3d4");
            desc.Add("Sucesso");
            quant.Add(q5);
            cor.Add("#bda5d4");
            desc.Add("Faturamento");
            quant.Add(q6);
            cor.Add("##ffffd8");
            desc.Add("Expedição");
            quant.Add(q7);
            cor.Add("#a5d3d4");
            desc.Add("Entregues");
            quant.Add(q8);
            cor.Add("#a5d3d4");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);

        }

        public JsonResult GetDadosGraficoCRMFunil()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            List<Int32> funis = (List<Int32>)Session["ListaFunil1"];
            List<ModeloViewModel> listaFunil = (List<ModeloViewModel>)Session["ListaFunilResumo"];
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in listaFunil)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                cor.Add(cores[i]);
                i++;
                if (i > 10)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);

        }

        public JsonResult GetDadosGraficoCRMStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["CRMProsp"];
            Int32 q2 = (Int32)Session["CRMCont"];
            Int32 q3 = (Int32)Session["CRMProp"];
            Int32 q4 = (Int32)Session["CRMNego"];
            Int32 q5 = (Int32)Session["CRMEnc"];

            desc.Add("Prospecção");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Contato Realizado");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Proposta Enviada");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Em Negociação");
            quant.Add(q4);
            cor.Add("#bda5d4");
            desc.Add("Encerrado");
            quant.Add(q5);
            cor.Add("#ffffd8");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoAcaoStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaCRMAcao"];
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                cor.Add(cores[i]);
                i++;
                if (i > 10)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoPropostaSituacao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PedElaboracao"];
            Int32 q2 = (Int32)Session["PedEnviada"];
            Int32 q3 = (Int32)Session["PedCancelada"];
            Int32 q4 = (Int32)Session["PedReprovada"];
            Int32 q5 = (Int32)Session["PedAprovada"];

            desc.Add("Em Elaboração");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Enviados");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Reprovados");
            quant.Add(q4);
            cor.Add("#bda5d4");
            desc.Add("Aprovados");
            quant.Add(q5);
            cor.Add("#ffffd8");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoPropostaStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PedElaboracao"];
            Int32 q2 = (Int32)Session["PedEnviada"];
            Int32 q3 = (Int32)Session["PedCancelada"];
            Int32 q4 = (Int32)Session["PedReprovada"];
            Int32 q5 = (Int32)Session["PedAprovada"];

            desc.Add("Em Elaboração");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Enviados");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Reprovados");
            quant.Add(q4);
            cor.Add("#bda5d4");
            desc.Add("Aprovados");
            quant.Add(q5);
            cor.Add("#ffffd8");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendasStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PedElaboracao"];
            Int32 q3 = (Int32)Session["PedCancelada"];
            Int32 q6 = (Int32)Session["PedFatura"];
            Int32 q7 = (Int32)Session["PedExpedicao"];
            Int32 q8 = (Int32)Session["PedEntregue"];

            desc.Add("Em Elaboração");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Faturamento");
            quant.Add(q6);
            cor.Add("#359E18");
            desc.Add("Expedição");
            quant.Add(q7);
            cor.Add("#FFAE00");
            desc.Add("Entregues");
            quant.Add(q8);
            cor.Add("#FF7F00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendasStatusPessoal()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PedElaboracaoPes"];
            Int32 q3 = (Int32)Session["PedCanceladaPes"];
            Int32 q6 = (Int32)Session["PedFaturaPes"];
            Int32 q7 = (Int32)Session["PedExpedicaoPes"];
            Int32 q8 = (Int32)Session["PedEntreguePes"];

            desc.Add("Em Elaboração");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Faturamento");
            quant.Add(q6);
            cor.Add("#359E18");
            desc.Add("Expedição");
            quant.Add(q7);
            cor.Add("#FFAE00");
            desc.Add("Entregues");
            quant.Add(q8);
            cor.Add("#FF7F00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRM()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaCRMMesResumo"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRMMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaCRMMeses"];
            List<String> meses = new List<String>();
            List<Int32> valor = new List<Int32>();
            meses.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                meses.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("meses", meses);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoPropostaMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPedidosMeses"];
            List<String> meses = new List<String>();
            List<Int32> valor = new List<Int32>();
            meses.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                meses.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("meses", meses);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoProposta()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPedidosMesResumo"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendas()
        {
            List<CRM_PEDIDO_VENDA> listaCP1 = (List<CRM_PEDIDO_VENDA>)Session["ListaPedidosMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasPed"];
            List<CRM_PEDIDO_VENDA> listaDia = new List<CRM_PEDIDO_VENDA>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.CRPV_DT_PEDIDO.Date == item).ToList();
                Int32 contaDia = listaDia.Count;
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendasPessoal()
        {
            List<CRM_PEDIDO_VENDA> listaCP1 = (List<CRM_PEDIDO_VENDA>)Session["ListaPedidosMesPes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasPedPes"];
            List<CRM_PEDIDO_VENDA> listaDia = new List<CRM_PEDIDO_VENDA>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.CRPV_DT_PEDIDO.Date == item).ToList();
                Int32 contaDia = listaDia.Count;
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult MostrarClientes()
        {
            // Prepara grid
            Session["VoltaMsg"] = 40;
            Session["VoltaTela"] = 0;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("MontarTelaCliente", "Cliente");
        }

        public ActionResult MontarTelaCRMKanbaChama()
        {
            // Prepara grid
            Session["VoltaKB"] = 1;
            return RedirectToAction("MontarTelaKanbanCRM");
        }

        public ActionResult MostrarTelaAgendaCalendario()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 50;
            Session["VoltaTela"] = 1;
            return RedirectToAction("MontarTelaAgendaCalendario", "Agenda");
        }
        
        public ActionResult MostrarIncluirAgenda()
        {
            // Prepara grid
            Session["VoltaAgenda"] = 23;
            Session["VoltaTela"] = 2;
            Session["TipoAgenda"] = 2;
            return RedirectToAction("IncluirAgenda", "Agenda");
        }

        public ActionResult MostrarClientesVenda()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 50;
            return RedirectToAction("MontarTelaCliente", "Cliente");
        }

        public ActionResult MostrarTransportadoras()
        {
            // Prepara grid
            Session["VoltaTransportadora"] = 40;
            return RedirectToAction("MontarTelaTransportadora", "Transportadora");
        }

        public ActionResult MostrarTransportadorasVenda()
        {
            // Prepara grid
            Session["VoltaTransportadora"] = 50;
            return RedirectToAction("MontarTelaTransportadora", "Transportadora");
        }

        public ActionResult MostrarServicos()
        {
            // Prepara grid
            Session["VoltaServico"] = 40;
            return RedirectToAction("MontarTelaServico", "Servico");
        }

        public ActionResult MostrarServicosVenda()
        {
            // Prepara grid
            Session["VoltaServico"] = 50;
            return RedirectToAction("MontarTelaServico", "Servico");
        }

        public ActionResult MostrarProdutos()
        {
            // Prepara grid
            Session["VoltaProduto"] = 40;
            return RedirectToAction("MontarTelaProduto", "Produto");
        }

        public ActionResult MostrarProdutosVenda()
        {
            // Prepara grid
            Session["VoltaProduto"] = 50;
            return RedirectToAction("MontarTelaProduto", "Produto");
        }

        public ActionResult IncluirClienteRapido()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            Session["VoltaTela"] = 0;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("IncluirClienteRapido", "Cliente");
        }

        public ActionResult VerPedidosUsuarioCRM()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                List<CRM_PEDIDO_VENDA> lista = CarregaPedidoVenda().Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.CRM.CRM1_IN_ATIVO != 2).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
                List<CRM_PEDIDO_VENDA> totalElaboracao = lista.Where(p => p.CRPV_IN_STATUS == 1).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
                List<CRM_PEDIDO_VENDA> totalEnviado = lista.Where(p => p.CRPV_IN_STATUS == 2).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
                List<CRM_PEDIDO_VENDA> totalCancelado = lista.Where(p => p.CRPV_IN_STATUS == 3).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
                List<CRM_PEDIDO_VENDA> totalAprovado = lista.Where(p => p.CRPV_IN_STATUS == 5).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
                List<CRM_PEDIDO_VENDA> totalReprovado = lista.Where(p => p.CRPV_IN_STATUS == 4).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
                List<CRM_PEDIDO_VENDA> totalEncerrado = lista.Where(p => p.CRPV_IN_STATUS == 6).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
                List<CRM_PEDIDO_VENDA> totalValidade = lista.Where(p => p.CRPV_IN_STATUS < 3 & p.CRPV_DT_VALIDADE < DateTime.Today.Date).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();

                if ((Int32)Session["PedidosUsuario"] == 1)
                {
                    ViewBag.Lista = lista;
                    ViewBag.TotalPedidos = lista.Count;
                }
                if ((Int32)Session["PedidosUsuario"] == 2)
                {
                    ViewBag.Lista = totalElaboracao;
                    ViewBag.TotalPedidos = totalElaboracao.Count;
                }
                if ((Int32)Session["PedidosUsuario"] == 3)
                {
                    ViewBag.Lista = totalEnviado;
                    ViewBag.TotalPedidos = totalEnviado.Count;
                }
                if ((Int32)Session["PedidosUsuario"] == 4)
                {
                    ViewBag.Lista = totalCancelado;
                    ViewBag.TotalPedidos = totalCancelado.Count;
                }
                if ((Int32)Session["PedidosUsuario"] == 5)
                {
                    ViewBag.Lista = totalReprovado;
                    ViewBag.TotalPedidos = totalReprovado.Count;
                }
                if ((Int32)Session["PedidosUsuario"] == 6)
                {
                    ViewBag.Lista = totalAprovado;
                    ViewBag.TotalPedidos = totalAprovado.Count;
                }
                if ((Int32)Session["PedidosUsuario"] == 7)
                {
                    ViewBag.Lista = totalEncerrado;
                    ViewBag.TotalPedidos = totalEncerrado.Count;
                }

                ViewBag.TotalElaboracao = totalElaboracao.Count;
                ViewBag.TotalEnviado = totalEnviado.Count;
                ViewBag.TotalAprovado = totalAprovado.Count;
                ViewBag.TotalReprovado = totalReprovado.Count;
                ViewBag.TotalEncerrado = totalEncerrado.Count;
                ViewBag.TotalCancelado = totalCancelado.Count;

                ViewBag.Nome = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
                ViewBag.Foto = usuario.USUA_AQ_FOTO;
                ViewBag.Cargo = usuario.CARGO.CARG_NM_NOME;
                Session["PontoPedido"] = 1;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult EnviarPropostaEdicao()
        {
            return RedirectToAction("EnviarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult EnviarPedidoEdicao()
        {
            return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult CancelarPedidoEdicao()
        {
            return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult CancelarPropostaEdicao()
        {
            return RedirectToAction("CancelarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult VerPropostaEdita()
        {
            Session["PontoProposta"] = 77;
            return RedirectToAction("VerProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult VerPedidoEdita()
        {
            Session["PontoPedido"] = 77;
            return RedirectToAction("VerPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessarEnvioPedidoEMail(MensagemViewModel vm, CRM_PEDIDO_VENDA item, USUARIO usuario)
        {
            try
            {
                // Inicialização
                Int32? tem = 0;
                Int32? em = 0;
                item = baseApp.GetPedidoById(item.CRPV_CD_ID);
                TEMPLATE_PROPOSTA temp = null;
                TEMPLATE_EMAIL email = null;
                String sigla = String.Empty;
                String siglaEM = String.Empty;
                Int32 idAss = (Int32)Session["IdAssinante"];
                CLIENTE cliente = null;
                String erro = null;
                Int32 volta = 0;
                CRMSysDBEntities Db = new CRMSysDBEntities();
                String status = "Succeeded";
                String iD = "xyz";

                // Recupera templates
                if (item.TEPR_CD_ID != null)
                {
                    tem = item.TEPR_CD_ID.Value;
                    temp = baseApp.GetTemplateById(tem.Value);
                    sigla = temp.TEPR_SG_SIGLA;
                }
                if (item.CRPC_IN_EMAIL != null)
                {
                    em = item.CRPC_IN_EMAIL.Value;
                    email = teApp.GetItemById(em.Value);
                    siglaEM = email.TEEM_SG_SIGLA;
                }

                // Recupera Cliente
                cliente = cliApp.GetItemById(vm.ID.Value);

                // Configuração
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

                // Recupera CRM
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);

                // Prepara mensagem
                String body = String.Empty;
                String header = String.Empty;
                String footer = String.Empty;
                String comercial = String.Empty;

                String cab = String.Empty;
                String corpo = String.Empty;
                String dados = String.Empty;

                String link = String.Empty;
                String emailBody = String.Empty;
                String propBody = String.Empty;
                String tail = String.Empty;
                String final = String.Empty;

                //Prepara proposta
                body = item.CRPV_TX_INFORMACOES_GERAIS;
                header = item.CRPV_TX_INTRODUCAO;
                footer = item.CRPV_TX_OUTROS_ITENS;
                comercial = item.CRPV_TX_CONDICOES_COMERCIAIS;

                // Prepara mail
                if (siglaEM != "NADA")
                {
                    cab = email.TEEM_TX_CABECALHO;
                    corpo = email.TEEM_TX_CORPO;
                    dados = email.TEEM_TX_DADOS;
                }
                else
                {
                    cab = "{Nome}";
                    corpo = item.CRPV_DS_ENVIO;
                    dados = "{Assinatura}";
                }

                // Formata mensagem 
                comercial = comercial.Replace("\r\n", "<br />");
                comercial = comercial.Replace("<p>", "");
                comercial = comercial.Replace("</p>", "<br />");
                body = body.Replace("\r\n", "<br />");
                body = body.Replace("<p>", "");
                body = body.Replace("</p>", "<br />");
                header = header.Replace("\r\n", "<br />");
                header = header.Replace("<p>", "");
                header = header.Replace("</p>", "<br />");
                footer = footer.Replace("\r\n", "<br />");
                footer = footer.Replace("<p>", "");
                footer = footer.Replace("</p>", "<br />");

                cab = cab.Replace("\r\n", "<br />");
                cab = cab.Replace("<p>", "");
                cab = cab.Replace("</p>", "<br />");
                corpo = corpo.Replace("\r\n", "<br />");
                corpo = corpo.Replace("<p>", "");
                corpo = corpo.Replace("</p>", "<br />");
                dados = dados.Replace("\r\n", "<br />");
                dados = dados.Replace("<p>", "");
                dados = dados.Replace("</p>", "<br />");

                link = vm.MENS_NM_LINK;

                // Prepara cabeçalho mail
                cab = cab.Replace("{Nome}", cliente.CLIE_NM_NOME);

                // Prepara rodape mail
                ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                dados = dados.Replace("{Assinatura}", assi.ASSI_NM_NOME);

                // Monta mail
                emailBody = cab + "<br /><br />" + corpo + "<br /><br />" + dados + "<br />" ;

                // Prepara cabeçalho proposta
                header = header.Replace("{Nome}", cliente.CLIE_NM_NOME);

                // Prepara rodape proposta
                footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);

                // Trata corpo proposta
                StringBuilder str = new StringBuilder();
                str.AppendLine(body);

                // Trata links
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
                    str.AppendLine("<br /><a href='" + link + "'>Clique aqui para maiores informações</a>");
                }
                body = str.ToString();

                // Prepara dados da proposta
                String info = String.Empty;
                CRM_PEDIDO_VENDA prop = baseApp.GetPedidoById((Int32)Session["IdCRMPedido"]);
                if (corpo.Length > 6)
                {
                    tail = CrossCutting.StringLibrary.GetLast(corpo, 6);
                }
                if (tail != String.Empty)
                {
                    if (tail == "<br />")
                    {
                        corpo = CrossCutting.StringLibrary.RemoveLast(corpo, 6);
                    }
                }

                // Acrescenta dados comerciais da proposta
                info = info + "<br /><b>---- Dados da Proposta ----</b> <br />";
                info = info + "<br/><b>Prazos</b> <br />";
                info = info + "<b>Data de Emissão:</b> " + prop.CRPV_DT_PEDIDO.ToShortDateString() + "<br />";
                info = info + "<b>Data de Validade:</b> " + prop.CRPV_DT_VALIDADE.ToShortDateString() + "<br /><br />";

                info = info + "<b>Dados Financeiros</b> <br />";
                info = info + "<b>Valor (R$):</b> " + CrossCutting.Formatters.DecimalFormatter(prop.CRPV_VL_VALOR.Value) + "<br />";
                info = info + "<b>Desconto (R$):</b> " + CrossCutting.Formatters.DecimalFormatter(prop.CRPV_VL_DESCONTO.Value) + "<br />";
                info = info + "<b>Frete (R$):</b> " + CrossCutting.Formatters.DecimalFormatter(prop.CRPV_VL_FRETE.Value) + "<br />";
                info = info + "<b>Taxas (R$):</b> " + CrossCutting.Formatters.DecimalFormatter(prop.CRPV_VL_ICMS.Value) + "<br />";
                info = info + "<b>Total (R$):</b> " + CrossCutting.Formatters.DecimalFormatter(prop.CRPV_TOTAL_PEDIDO.Value) + "<br /><br />";

                info = info + "<b>Condições Comerciais</b> <br />";
                info = info + comercial;

                // Monta proposta final
                propBody = header + "<br />" + body + "<br /><br />" + footer + "<br />" + info;

                // Monta mensagem final
                final = emailBody + "<br />" + propBody;

                // Checa e monta anexos
                List<CRM_PEDIDO_VENDA_ANEXO> anexos = item.CRM_PEDIDO_VENDA_ANEXO.ToList();
                List<AttachmentModel> models = new List<AttachmentModel>();
                if (anexos.Count > 0)
                {
                    String caminho = "/Imagens/" + idAss.ToString() + "/Pedido/" + item.CRPV_CD_ID.ToString() + "/Anexos/";
                    foreach (CRM_PEDIDO_VENDA_ANEXO anexo in anexos)
                    {
                        String path = Path.Combine(Server.MapPath(caminho), anexo.CRPA_NM_TITULO);

                        AttachmentModel model = new AttachmentModel();
                        model.PATH = path;
                        model.ATTACHMENT_NAME = anexo.CRPA_NM_TITULO;
                        if (anexo.CRPA_IN_TIPO == 1)
                        {
                            model.CONTENT_TYPE = MediaTypeNames.Image.Jpeg;
                        }
                        else if (anexo.CRPA_IN_TIPO == 3)
                        {
                            model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
                        }
                        else
                        {
                            model.CONTENT_TYPE = MediaTypeNames.Application.Octet;
                        }
                        models.Add(model);
                    }
                }
                else
                {
                    models = null;
                }

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Proposta #" + item.CRPV_IN_NUMERO_GERADO + " - " + cliente.CLIE_NM_NOME;
                mensagem.CORPO = final;
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
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, models);

                // Grava envio
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                env.CLIE_CD_ID = cliente.CLIE_CD_ID;
                env.CRM1_CD_ID = crm.CRM1_CD_ID;
                env.MEEN_IN_TIPO = 1;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN_EM_EMAIL_DESTINO = cliente.CLIE_NM_EMAIL;
                env.MEEN_NM_ORIGEM = "Mensagem para Cliente - Envio de Proposta";
                env.MEEN_TX_CORPO = emailBody;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 5;
                env.MEEN_TX_CORPO_COMPLETO = emailBody;
                env.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                if (erro == null)
                {
                    env.MEEN_IN_ENTREGUE = 1;
                }
                else
                {
                    env.MEEN_IN_ENTREGUE = 0;
                    env.MEEN_TX_RETORNO = erro;
                }
                Int32 volta5 = meApp.ValidateCreate(env);
                Session["VoltaTela"] = 6;
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
        }

        [HttpGet]
        public ActionResult IncluirComentarioPedidoCRM()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 id = (Int32)Session["IdCRMPedido"];
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CRM_PEDIDO_VENDA_ACOMPANHAMENTO coment = new CRM_PEDIDO_VENDA_ACOMPANHAMENTO();
                CRMPedidoComentarioViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA_ACOMPANHAMENTO, CRMPedidoComentarioViewModel>(coment);
                vm.CRPC_DT_ACOMPANHAMENTO = DateTime.Now;
                vm.CRPC_IN_ATIVO = 1;
                vm.CRPV_CD_ID = item.CRPV_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
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

        [HttpPost]
        public ActionResult IncluirComentarioPedidoCRM(CRMPedidoComentarioViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idNot = (Int32)Session["IdCRMPedido"];

                    // Sanitização
                    vm.CRPC_TX_ACOMPANHAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRPC_TX_ACOMPANHAMENTO);

                    // Executa a operação
                    CRM_PEDIDO_VENDA_ACOMPANHAMENTO item = Mapper.Map<CRMPedidoComentarioViewModel, CRM_PEDIDO_VENDA_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_PEDIDO_VENDA not = baseApp.GetPedidoById(idNot);

                    item.USUARIO = null;
                    not.CRM_PEDIDO_VENDA_ACOMPANHAMENTO.Add(item);
                    Int32 volta = baseApp.ValidateEditPedido(not);

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "icoCRPV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Inclusão de Acompanhamento - Proposta: " + not.CRPV_NM_NOME,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["VoltaTela"] = 6;
                    return RedirectToAction("VoltarEditarPedidoCRM");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirPedido()
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
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica se pode incluir pedido
                List<CRM_PEDIDO_VENDA> peds = CarregaPedidoVenda().Where(p => p.CRM1_CD_ID == (Int32)Session["IdCRM"]).ToList();
                if (peds.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
                {
                    Session["MensCRM"] = 82;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara listas
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
                List<TEMPLATE_PROPOSTA> listaTotal = CarregaTemplateProposta();
                ViewBag.Templates = new SelectList(listaTotal.Where(p => p.TEPR_IN_TIPO != 2).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
                List<USUARIO> listaTotal1 = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaTotal1.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

                // Recupera número
                Int32 num = 0;
                Session["PedidoVendaAlterada"] = 1;
                List<CRM_PEDIDO_VENDA> ped1 = CarregaPedidoVendaGeral();
                if (ped1.Count == 0)
                {
                    num = conf.CONF_IN_NUMERO_INICIAL_PROPOSTA.Value;
                }
                else
                {
                    num = ped1.OrderByDescending(p => p.CRPV_IN_NUMERO_GERADO).ToList().First().CRPV_IN_NUMERO_GERADO.Value;
                    num++;
                }
                Session["UsuarioAlterada"] = 0;

                // Prepara objeto
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                Session["IdCliente"] = crm.CLIE_CD_ID;
                CRM_PEDIDO_VENDA item = new CRM_PEDIDO_VENDA();
                CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
                vm.CRM1_CD_ID = (Int32)Session["IdCRM"];
                vm.CRM = crm;
                vm.CRPV_IN_ATIVO = 1;
                vm.ASSI_CD_ID = idAss;
                vm.CRPV_DT_PEDIDO = DateTime.Now;
                vm.CRPV_IN_STATUS = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.CRPV_VL_VALOR = 0;
                vm.CRPV_VL_TOTAL_ITENS = 0;
                vm.CRPV_VL_DESCONTO = 0;
                vm.CRPV_VL_FRETE = 0;
                vm.CRPV_VL_ICMS = 0;
                vm.CRPV_VL_IPI = 0;
                vm.CRPV_IN_PRAZO_ENTREGA = 0;
                vm.CRPV_VL_PESO_BRUTO = 0;
                vm.CRPV_VL_PESO_LIQUIDO = 0;
                vm.CRPV_IN_GERAR_CR = 0;
                vm.CRPV_DT_VALIDADE = DateTime.Now.AddDays(Convert.ToDouble(conf.CONF_NR_DIAS_PROPOSTA));
                vm.CRPV_IN_NUMERO_GERADO = num;
                vm.CLIE_CD_ID = crm.CLIE_CD_ID;
                vm.FILI_CD_ID = 1;
                vm.CLIENTE_NOME = (CLIENTE)Session["ClienteCRM"];
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_PROPOSTA> listaTotal = CarregaTemplateProposta();
            ViewBag.Templates = new SelectList(listaTotal.Where(p => p.TEPR_IN_TIPO != 2).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
            List<USUARIO> listaTotal1 = CarregaUsuario();
            ViewBag.Usuarios = new SelectList(listaTotal1.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            vm.CLIENTE_NOME = (CLIENTE)Session["ClienteCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRPV_LK_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.CRPV_LK_LINK);
                    vm.CRPV_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_NM_NOME);
                    vm.CRPV_NR_NUMERO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRPV_NR_NUMERO);
                    vm.CRPV_TX_CONDICOES_COMERCIAIS = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_CONDICOES_COMERCIAIS);
                    vm.CRPV_TX_INFORMACOES_GERAIS = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INFORMACOES_GERAIS);
                    vm.CRPV_TX_INFORMACOES_GERAIS_1 = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INFORMACOES_GERAIS_1);
                    vm.CRPV_TX_INTRODUCAO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INTRODUCAO);
                    vm.CRPV_TX_INTRODUCAO_1 = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INTRODUCAO_1);
                    vm.CRPV_TX_OBSERVACAO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_OBSERVACAO);
                    vm.CRPV_TX_OUTROS_ITENS = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_OUTROS_ITENS);
                    vm.CRPV_TX_OUTROS_ITENS_1 = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_OUTROS_ITENS_1);

                    // Criticas 
                    if (vm.CRPV_NM_NOME == null)
                    {
                        vm.CRPV_NM_NOME = "Proposta - " + vm.CLIENTE_NOME.CLIE_NM_NOME;
                    }

                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreatePedido(item);

                    // Cria pasta
                    String caminho = "/Imagens/" + idAss.ToString() + "/Pedido/" + item.CRPV_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Carrega arquivos
                    Session["IdCRMPedido"] = item.CRPV_CD_ID;
                    Session["FlagAnexo"] = 1;
                    if (Session["FileQueueCRM"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCRM"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueCRMPedido(file);
                        }

                        Session["FileQueueCRM"] = null;
                    }

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Criação de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Criação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "addCRPV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_PEDIDO_VENDA>(item),
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["SegueInclusao"] = 1;
                    Session["PedidoVendaAlterada"] = 1;
                    Session["VoltaTela"] = 6;
                    Session["FlagCRMPedido"] = 1;
                    Session["CRMPedidoAlterada"] = 1;
                    Session["LinhaAlterada2"] = item.CRPV_CD_ID;
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult CancelarPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_CANCELAR_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                ViewBag.Motivos = new SelectList(CarregaMotivoCancelamento().Where(p => p.MOCA_IN_TIPO == 1).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                List<CRM_PEDIDO_VENDA> lp = CarregaPedidoVenda();
                Session["IdCRMPedido"] = item.CRPV_CD_ID;

                // Checa pedidos
                Session["TemPed"] = 0;
                if (lp.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
                {
                    Session["TemPed"] = 1;
                }

                // Mensagens
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 30)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0124", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 31)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0125", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 32)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 33)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 80)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0145", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 81)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0149", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                Session["MensCRM"] = 0;
                Session["VoltaComentPedido"] = 2;
                CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
                vm.CRPV_DT_CANCELAMENTO = DateTime.Today.Date;
                vm.CRPV_IN_STATUS = 3;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CancelarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Motivos = new SelectList(CarregaMotivoCancelamento().Where(p => p.MOCA_IN_TIPO == 1).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");

            if (ModelState.IsValid)
            {

                try
                {
                    // Sanitização
                    vm.CRPV_DS_CANCELAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRPV_DS_CANCELAMENTO);

                    // Verifica tipo de ação
                    if (vm.MOCA_CD_ID == null || vm.MOCA_CD_ID == 0)
                    {
                        Session["MensCRM"] = 80;
                        return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (vm.CRPV_DS_CANCELAMENTO == null)
                    {
                        Session["MensCRM"] = 81;
                        return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }

                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 statusAnt = item.CRPV_IN_STATUS;
                    Int32 volta = baseApp.ValidateCancelarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 30;
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 31;
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 32;
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 33;
                        return View(vm);
                    }

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Cancelamento de Proposta";
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    dia.DIPR_DS_DESCRICAO = "Cancelamento de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Texto
                    MOTIVO_CANCELAMENTO mc = mcApp.GetItemById(item.MOCA_CD_ID.Value);
                    String info = String.Empty;
                    info = info + "<br />A proposta abaixo foi cancelada pelo responsável em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                    info = info + "Motivo do Cancelamento: <b>" + mc.MOCA_NM_NOME + "</b><br />";
                    info = info + "Justificativa do Cancelamento: <b>" + item.CRPV_DS_CANCELAMENTO + "</b><br />";
                    info = info + "<br />Informações da Proposta:<br />";
                    info = info + "Número: <b style='color: darkblue'>" + item.CRPV_IN_NUMERO_GERADO.ToString() + "</b><br />";
                    info = info + "Nome: <b style='color: grenn'>" + item.CRPV_NM_NOME + "</b><br />";
                    info = info + "Data de Criação: <b>" + item.CRPV_DT_PEDIDO.ToShortDateString() + "</b><br />";
                    info = info + "Validade: <b>" + item.CRPV_DT_VALIDADE.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRPV_GU_GUID + "</b><br />";

                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + not.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + not.CLIENTE.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + not.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + not.CRM1_GU_GUID + "</b><br />";

                    // Gera e-mail
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = usuario.USUA_NM_NOME;
                    mens.ID = usuario.USUA_CD_ID;
                    mens.MODELO = usuario.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = info;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = "Cancelamento Proposta";
                    mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                    mens.CLIE_CD_ID = item.CLIE_CD_ID;

                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    await envio.ProcessaEnvioEMailGeral(mens, usuario);

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "canCRPV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Caancelamento de Proposta - Proposta: " + item.CRPV_NM_NOME,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Listas
                    Session["VoltaTela"] = 6;
                    Session["FlagCRMPedido"] = 1;
                    Session["CRMPedidoAlterada"] = 1;
                    Session["PedidoVendaAlterada"] = 1;
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReprovarPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_APROVAR_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                List<CRM_PEDIDO_VENDA> lp = baseApp.GetAllPedidos(idAss);
                Session["IdCRMPedido"] = item.CRPV_CD_ID;

                // Checa pedidos
                Session["TemPed"] = 0;
                if (lp.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
                {
                    Session["TemPed"] = 1;
                }

                // Mensagens
                Session["VoltaComentPedido"] = 3;
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0128", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0129", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 52)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0126", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 53)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0127", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
                vm.CRPV_DT_REPROVACAO = DateTime.Today.Date;
                vm.CRPV_IN_STATUS = 4;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ReprovarPedido(CRMPedidoViewModel vm)
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

                    // Sanitização
                    vm.CRPV_DS_REPROVACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRPV_DS_REPROVACAO);

                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateReprovarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 50;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 51;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 52;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 53;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }

                    // Atualiza status do processo
                    CRM crm = baseApp.GetItemById(item.CRM1_CD_ID.Value);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Reprovação de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Reprovação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Texto
                    String info = String.Empty;
                    info = info + "<br />A proposta abaixo foi reprovada pelo cliente em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                    info = info + "<br />Informações da Repreovação: " + item.CRPV_DS_REPROVACAO + "<br />";
                    info = info + "<br />Informações da Proposta:<br />";
                    info = info + "Número: <b style='color: darkblue'>" + item.CRPV_IN_NUMERO_GERADO.ToString() + "</b><br />";
                    info = info + "Nome: <b style='color: grenn'>" + item.CRPV_NM_NOME + "</b><br />";
                    info = info + "Data de Criação: <b>" + item.CRPV_DT_PEDIDO.ToShortDateString() + "</b><br />";
                    info = info + "Validade: <b>" + item.CRPV_DT_VALIDADE.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRPV_GU_GUID + "</b><br />";

                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + not.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + not.CLIENTE.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + not.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + not.CRM1_GU_GUID + "</b><br />";

                    // Gera e-mail
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = usuario.USUA_NM_NOME;
                    mens.ID = usuario.USUA_CD_ID;
                    mens.MODELO = usuario.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = info;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = "Reprovação Proposta";
                    mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                    mens.CLIE_CD_ID = not.CLIE_CD_ID;

                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    await envio.ProcessaEnvioEMailGeral(mens, usuario);

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "repCRPV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Reprovação de Proposta - Proposta: " + item.CRPV_NM_NOME,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Listas
                    Session["FlagCRMPedido"] = 1;
                    Session["CRMPedidoAlterada"] = 1;
                    Session["PedidoVendaAlterada"] = 1;
                    Session["VoltaTela"] = 6;
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult AprovarPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_APROVAR_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                List<CRM_PEDIDO_VENDA> lp = baseApp.GetAllPedidos(idAss);
                Session["IdCRMPedido"] = item.CRPV_CD_ID;

                // Checa pedidos
                Session["TemPed"] = 0;
                if (lp.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
                {
                    Session["TemPed"] = 1;
                }

                // Mensagens
                Session["VoltaComentPedido"] = 4;
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 60)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0132", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 61)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0133", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 62)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0130", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 63)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0131", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
                vm.CRPV_DT_APROVACAO = DateTime.Today.Date;
                vm.CRPV_IN_STATUS = 5;
                vm.FlagVenda = 0;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AprovarPedido(CRMPedidoViewModel vm)
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

                    // Sanitização
                    vm.CRPV_DS_APROVACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRPV_DS_APROVACAO);

                    // Executa a operação
                    Boolean venda = vm.GeraVenda;
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateAprovarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Aprovação de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Aprovação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Texto
                    String info = String.Empty;
                    info = info + "<br />A proposta abaixo foi aprovada pelo cliente em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                    info = info + "Informações da Aprovação: " + item.CRPV_DS_APROVACAO + "<br />";
                    info = info + "<br />Informações da Proposta:<br />";
                    info = info + "Número: <b style='color: darkblue'>" + item.CRPV_IN_NUMERO_GERADO.ToString() + "</b><br />";
                    info = info + "Nome: <b style='color: grenn'>" + item.CRPV_NM_NOME + "</b><br />";
                    info = info + "Data de Criação: <b>" + item.CRPV_DT_PEDIDO.ToShortDateString() + "</b><br />";
                    info = info + "Validade: <b>" + item.CRPV_DT_VALIDADE.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRPV_GU_GUID + "</b><br />";

                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + not.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + not.CLIENTE.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + not.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + not.CRM1_GU_GUID + "</b><br />";

                    // Gera e-mail
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = usuario.USUA_NM_NOME;
                    mens.ID = usuario.USUA_CD_ID;
                    mens.MODELO = usuario.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = info;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = "Aprovação de Proposta";
                    mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                    mens.CLIE_CD_ID = item.CLIE_CD_ID;

                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    await envio.ProcessaEnvioEMailGeral(mens, usuario);

                    // Recupera configuracao
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();

                    // Calcula proximo numero
                    Int32 num = 0;
                    List<PEDIDO_VENDA> ped1 = CarregaVenda();
                    if (ped1.Count == 0)
                    {
                        num = conf.CONF_IN_NUMERO_INICIAL_PEDIDO.Value;
                    }
                    else
                    {
                        num = ped1.OrderByDescending(p => p.PEVE_NR_NUMERO_GERADO).ToList().First().PEVE_NR_NUMERO_GERADO.Value;
                        num++;
                    }

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "aprCRPV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Aprovação de Proposta - Proposta: " + item.CRPV_NM_NOME,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Retorno
                    Session["PedidoVendaAlterada"] = 1;
                    Session["FlagCRMPedido"] = 1;
                    Session["CRMPedidoAlterada"] = 1;
                    Session["VoltaTela"] = 6;
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EnviarPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ENVIO_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                List<CRM_PEDIDO_VENDA> lp = baseApp.GetAllPedidos(idAss);
                Session["IdCRMPedido"] = item.CRPV_CD_ID;

                // Checa propostas
                Session["TemPed"] = 0;
                //ViewBag.Templates = new SelectList(teApp.GetAllItens(idAss).OrderByDescending(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");

                // Mensagens
                Session["VoltaComentPedido"] = 5;
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 70)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0134", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 71)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0135", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 72)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0136", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 75)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0137", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 76)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0138", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 77)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0139", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 90)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0139", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
                vm.CRPV_DT_ENVIO = DateTime.Today.Date;
                vm.CRPV_IN_STATUS = 2;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EnviarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            //ViewBag.Templates = new SelectList(teApp.GetAllItens(idAss).OrderByDescending(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRPV_DS_ENVIO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_DS_ENVIO);

                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Monta anexo
                    Session["LinkAprova"] = null;

                    // Processa envio
                    CRM_PEDIDO_VENDA pro = baseApp.GetPedidoById(item.CRPV_CD_ID);
                    Int32 volta = baseApp.ValidateEnviarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 70;
                        return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 71;
                        return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 72;
                        return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }

                    // Atualiza status do processo
                    CRM crm = baseApp.GetItemById(item.CRM1_CD_ID.Value);

                    // Monta mensagem responsavel
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.ASSI_CD_ID = idAss;
                    mens.MENS_DT_CRIACAO = DateTime.Now;
                    mens.MENS_IN_ATIVO = 1;
                    mens.MENS_IN_TIPO = 1;
                    mens.ID = crm.CLIE_CD_ID;

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Envio de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Envio de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Texto responsavel
                    String info = String.Empty;
                    info = info + "A proposta abaixo foi enviada para o cliente em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                    info = info + "<br />Informações da Proposta:<br />";
                    info = info + "Número: <b style='color: darkblue'>" + item.CRPV_IN_NUMERO_GERADO.ToString() + "</b><br />";
                    info = info + "Nome: <b style='color: grenn'>" + item.CRPV_NM_NOME + "</b><br />";
                    info = info + "Data de Criação: <b>" + item.CRPV_DT_PEDIDO.ToShortDateString() + "</b><br />";
                    info = info + "Validade: <b>" + item.CRPV_DT_VALIDADE.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRPV_GU_GUID + "</b><br />";

                    info = info + "<br />Informações do Processo:<br />";
                    info = info + "Processo: <b style='color: darkblue'>" + not.CRM1_NM_NOME + "</b><br />";
                    info = info + "Cliente: <b style='color: grenn'>" + not.CLIENTE.CLIE_NM_NOME + "</b><br />";
                    info = info + "Data de Início: <b>" + not.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + not.CRM1_GU_GUID + "</b>";

                    // Envia e-mail responsavel
                    mens = new MensagemViewModel();
                    mens.NOME = not.USUARIO.USUA_NM_NOME;
                    mens.ID = crm.CLIE_CD_ID;
                    mens.TEPR_CD_ID = item.CRPC_IN_EMAIL;
                    mens.MENS_NM_LINK = item.CRPV_LK_LINK;
                    mens.MODELO = not.USUARIO.USUA_NM_EMAIL;
                    mens.ASSI_CD_ID = idAss;
                    mens.MENS_DT_CRIACAO = DateTime.Now;
                    mens.MENS_IN_ATIVO = 1;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = info;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = "Envio de Proposta";
                    mens.MENS_NM_CAMPANHA = not.USUARIO.USUA_NM_EMAIL;
                    mens.CLIE_CD_ID = not.CLIE_CD_ID;
                    mens.MENS_NM_ASSINATURA = null;

                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    await envio.ProcessaEnvioEMailGeral(mens, usuario);

                    // Monta texto base cliente
                    info = String.Empty;
                    if (item.CRPV_DS_ENVIO != null & item.CRPV_DS_ENVIO != String.Empty)
                    {
                        info = info + item.CRPV_DS_ENVIO + "<br /><br />";
                        info = info.Replace(" p ", " ");
                    }

                    // Monta resumo da proposta
                    info = info + item.CRPV_TX_INTRODUCAO + "<br /><br />"; 
                    info = info + item.CRPV_TX_INFORMACOES_GERAIS + "<br /><br />";
                    info = info + item.CRPV_TX_OUTROS_ITENS + "<br />";

                    info = info + "<br />-------------------------------------------------------------------------------";
                    info = info + "<br /><b>Informações da Proposta:</b><br />";
                    info = info + "Número: <b style='color: darkblue'>" + item.CRPV_IN_NUMERO_GERADO.ToString() + "</b><br />";
                    info = info + "Nome: <b style='color: grenn'>" + item.CRPV_NM_NOME + "</b><br />";
                    info = info + "Data de Emissão: <b>" + item.CRPV_DT_PEDIDO.ToShortDateString() + "</b><br />";
                    info = info + "Validade: <b>" + item.CRPV_DT_VALIDADE.ToShortDateString() + "</b><br />";
                    info = info + "Identificador: <b>" + item.CRPV_GU_GUID + "</b><br /><br />";

                    info = info + "Valor (R$): <b>" + CrossCutting.Formatters.DecimalFormatter(item.CRPV_VL_VALOR.Value) + "</b><br />";
                    info = info + "Desconto (R$): <b>" + CrossCutting.Formatters.DecimalFormatter(item.CRPV_VL_DESCONTO.Value) + "</b><br />";
                    info = info + "Frete (R$): <b>" + CrossCutting.Formatters.DecimalFormatter(item.CRPV_VL_FRETE.Value) + "</b><br />";
                    info = info + "Taxas (R$): <b>" + CrossCutting.Formatters.DecimalFormatter(item.CRPV_VL_ICMS.Value) + "</b><br />";
                    info = info + "Total (R$): <b style: color: green;>" + CrossCutting.Formatters.DecimalFormatter(item.CRPV_TOTAL_PEDIDO.Value) + "</b><br /><br />";

                    info = info + "<br />Condições Comerciais:<br />";
                    info = info + item.CRPV_TX_CONDICOES_COMERCIAIS + "<br />";
                    info = info.Replace("\r\n", "<br />");
                    info = info.Replace(" p ", " ");
                    info = info.Replace(" br ", " ");

                    // Monta mensagem cliente
                    MensagemViewModel mens1 = new MensagemViewModel();
                    mens1.ASSI_CD_ID = idAss;
                    mens1.MENS_DT_CRIACAO = DateTime.Now;
                    mens1.MENS_IN_ATIVO = 1;
                    mens1.MENS_IN_TIPO = 1;
                    mens1.ID = cli.CLIE_CD_ID;
                    mens1.TEPR_CD_ID = item.CRPC_IN_EMAIL;
                    mens1.MENS_NM_LINK = item.CRPV_LK_LINK;
                    mens1.NOME = cli.CLIE_NM_NOME;
                    mens1.ID = cli.CLIE_CD_ID;
                    mens1.TEPR_CD_ID = item.CRPC_IN_EMAIL;
                    mens1.MENS_NM_LINK = item.CRPV_LK_LINK;
                    mens1.MODELO = cli.CLIE_NM_EMAIL;
                    mens1.ASSI_CD_ID = idAss;
                    mens1.MENS_DT_CRIACAO = DateTime.Now;
                    mens1.MENS_IN_ATIVO = 1;
                    mens1.MENS_IN_TIPO = 1;
                    mens1.MENS_TX_TEXTO = info;
                    mens1.MENS_NM_LINK = null;
                    mens1.MENS_NM_NOME = "Envio de Proposta";
                    mens1.MENS_NM_CAMPANHA = cli.CLIE_NM_EMAIL;
                    mens1.CLIE_CD_ID = not.CLIE_CD_ID;
                    mens1.MENS_NM_ASSINATURA = (String)Session["NomeEmpresaAssina"];

                    EnvioEMailGeralBase envioCliente = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    await envioCliente.ProcessaEnvioEMailGeral(mens1, usuario);

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "envCRPV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Envio de Proposta - Proposta: " + item.CRPV_NM_NOME,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Retorno
                    Session["PedidoVendaAlterada"] = 1;
                    Session["VoltaTela"] = 6;
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERACAO_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica se pode editar ação
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                if (item.CRPV_IN_STATUS > 2)
                {
                    Session["MensCRM"] = 53;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara view
                Session["VoltaComentPedido"] = 1;
                Session["IdCRMPedido"] = item.CRPV_CD_ID;
                List<TEMPLATE_PROPOSTA> listaTotal = CarregaTemplateProposta();
                ViewBag.Templates = new SelectList(listaTotal.Where(p => p.TEPR_IN_TIPO == 1).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
                List<USUARIO> listaTotal1 = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaTotal1.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

                // Mensagens
                if ((Int32)Session["MensCRM"] == 99)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0150", CultureInfo.CurrentCulture));
                }

                // Processa
                ViewBag.Template = item.TEMPLATE_PROPOSTA.TEPR_SG_SIGLA;
                Session["MensCRM"] = 0;
                Session["PedidoAntes"] = item;
                objetoAntes = (CRM)Session["CRM"];
                CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
                CRM proc = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                CLIENTE cli = cliApp.GetItemById(proc.CLIE_CD_ID);
                vm.CLIENTE_NOME = cli;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_PROPOSTA> listaTotal = CarregaTemplateProposta();
            ViewBag.Templates = new SelectList(listaTotal.Where(p => p.TEPR_IN_TIPO == 1).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
            List<USUARIO> listaTotal1 = CarregaUsuario();
            ViewBag.Usuarios = new SelectList(listaTotal1.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRPV_LK_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.CRPV_LK_LINK);
                    vm.CRPV_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_NM_NOME);
                    vm.CRPV_NR_NUMERO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRPV_NR_NUMERO);
                    vm.CRPV_TX_CONDICOES_COMERCIAIS = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_CONDICOES_COMERCIAIS);
                    vm.CRPV_TX_INFORMACOES_GERAIS = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INFORMACOES_GERAIS);
                    vm.CRPV_TX_INFORMACOES_GERAIS_1 = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INFORMACOES_GERAIS_1);
                    vm.CRPV_TX_INTRODUCAO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INTRODUCAO);
                    vm.CRPV_TX_INTRODUCAO_1 = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_INTRODUCAO_1);
                    vm.CRPV_TX_OBSERVACAO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_OBSERVACAO);
                    vm.CRPV_TX_OUTROS_ITENS = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_OUTROS_ITENS);
                    vm.CRPV_TX_OUTROS_ITENS_1 = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.CRPV_TX_OUTROS_ITENS_1);

                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                    // Acerta valores
                    Decimal total = item.CRPV_VL_VALOR.Value;
                    if (item.CRPV_VL_DESCONTO != null & item.CRPV_VL_DESCONTO > 0)
                    {
                        total -= item.CRPV_VL_DESCONTO.Value;
                    }
                    if (item.CRPV_VL_FRETE != null & item.CRPV_VL_FRETE > 0)
                    {
                        total += item.CRPV_VL_FRETE.Value;
                    }
                    item.CRPV_TOTAL_PEDIDO = total;
                    Int32 volta = baseApp.ValidateEditPedido(item);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Alteração de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Alteração de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "edtCRPV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Edição de Proposta - " + item.CRPV_NM_NOME,
                        LOG_TX_REGISTRO_ANTES = null,
                        LOG_IN_SISTEMA = 1
                    };  
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["PedidoVendaAlterada"] = 1;
                    Session["FlagCRMPedido"] = 1;
                    Session["CRMPedidoAlterada"] = 1;
                    Session["ListaCRM"] = null;
                    Session["VoltaTela"] = 6;
                    Session["LinhaAlterada2"] = item.CRPV_CD_ID;
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirPedido(Int32 id)
        {
            // Processa
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRPV_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditPedido(item);

                // Gera diario
                CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = not.CRM1_CD_ID;
                dia.CRPV_CD_ID = item.CRPV_CD_ID;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                dia.DIPR_NM_OPERACAO = "Exclusão de Proposta";
                dia.DIPR_DS_DESCRICAO = "Exclusão de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log                    
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "delCRPV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Exclusão de Proposta - Proposta: " + item.CRPV_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                // Verifica retorno
                Session["PedidoVendaAlterada"] = 1;
                Session["FlagCRMPedido"] = 1;
                Session["CRMPedidoAlterada"] = 1;
                Session["VoltaTela"] = 6;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRPV_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditPedido(item);

                // Gera diario
                CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = not.CRM1_CD_ID;
                dia.CRPV_CD_ID = item.CRPV_CD_ID;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                dia.DIPR_NM_OPERACAO = "Reativação de Proposta";
                dia.DIPR_DS_DESCRICAO = "Reativação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log                    
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "reaCRPV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Reativação de Proposta - Proposta: " + item.CRPV_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                // Verifica retorno
                Session["PedidoVendaAlterada"] = 1;
                Session["VoltaTela"] = 6;
                Session["FlagCRMPedido"] = 1;
                Session["CRMPedidoAlterada"] = 1;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public async Task<ActionResult> ElaborarPedido(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERACAO_PROPOSTA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                Session["IdCRMPedido"] = item.CRPV_CD_ID;

                // Processa
                item.CRPV_IN_STATUS = 1;
                item.CRPV_DT_ENVIO = null;
                item.CRPV_DT_APROVACAO = null;
                item.CRPV_DT_CANCELAMENTO = null;
                item.CRPV_DT_REPROVACAO = null;
                item.CRPV_DS_APROVACAO = null;
                item.CRPV_DS_CANCELAMENTO = null;
                item.CRPV_DS_REPROVACAO = null;
                Int32 volta = baseApp.ValidateEditPedido(item);

                // Gera diario
                CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = not.CRM1_CD_ID;
                dia.CRPV_CD_ID = item.CRPV_CD_ID;
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                dia.DIPR_NM_OPERACAO = "Alteração de Status de Proposta";
                dia.DIPR_DS_DESCRICAO = "Alteração de Status de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Texto
                String info = String.Empty;
                info = info + "<br />A proposta retornou para sua etapa incial em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                info = info + "<br />Informações da Proposta:<br />";
                info = info + "Número: <b style='color: darkblue'>" + item.CRPV_IN_NUMERO_GERADO.ToString() + "</b><br />";
                info = info + "Nome: <b style='color: grenn'>" + item.CRPV_NM_NOME + "</b><br />";
                info = info + "Data de Criação: <b>" + item.CRPV_DT_PEDIDO.ToShortDateString() + "</b><br />";
                info = info + "Validade: <b>" + item.CRPV_DT_VALIDADE.ToShortDateString() + "</b><br />";
                info = info + "Identificador: <b>" + item.CRPV_GU_GUID + "</b><br />";

                info = info + "<br />Informações do Processo:<br />";
                info = info + "Processo: <b style='color: darkblue'>" + not.CRM1_NM_NOME + "</b><br />";
                info = info + "Cliente: <b style='color: grenn'>" + not.CLIENTE.CLIE_NM_NOME + "</b><br />";
                info = info + "Data de Início: <b>" + not.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                info = info + "Identificador: <b>" + not.CRM1_GU_GUID + "</b><br />";

                // Gera e-mail
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = not.USUARIO.USUA_NM_NOME;
                mens.ID = not.USUARIO.USUA_CD_ID;
                mens.MODELO = not.USUARIO.USUA_NM_EMAIL;
                mens.MODELO = not.USUARIO.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_TX_TEXTO = info;
                mens.MENS_NM_LINK = null;
                mens.MENS_NM_NOME = "Proposta - Retorno à Etapa Inicial";
                mens.MENS_NM_CAMPANHA = not.USUARIO.USUA_NM_EMAIL;
                mens.CLIE_CD_ID = not.CLIE_CD_ID;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                await envio.ProcessaEnvioEMailGeral(mens, usuario);

                Session["PedidoVendaAlterada"] = 1;
                Session["VoltaTela"] = 6;
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerPedido(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;
                Session["PontoPedido"] = 0;
                Session["VoltaTela"] = 6;
                Session["VoltaPedidoView"] = 200;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
                Session["IdCRMPedido"] = item.CRPV_CD_ID;
                CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
                CRM proc = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                ViewBag.Template = item.TEMPLATE_PROPOSTA.TEPR_SG_SIGLA;
                CLIENTE cli = cliApp.GetItemById(proc.CLIE_CD_ID);
                vm.CLIENTE_NOME = cli;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirMotivoCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMotCancelamento"] = 2;
            return RedirectToAction("IncluirMotCancelamento", "TabelaAuxiliar");
        }

        public ActionResult IncluirMotivoCancelamento1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMotCancelamento"] = 3;
            return RedirectToAction("IncluirMotCancelamento", "TabelaAuxiliar");
        }

        public ActionResult IncluirMotivoEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMotEncerramento"] = 2;
            return RedirectToAction("IncluirMotEncerramento", "TabelaAuxiliar");
        }

        [HttpGet]
        public ActionResult MontarTelaHistorico()
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
                    if (usuario.PERFIL.PERF_IN_ACOMPANHA_CRM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "CRM";
                        return RedirectToAction("VoltarAnexoAcompanhamento", "CRM");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                Int32 flag = (Int32)Session["TipoHistorico"];
                if ((List<DIARIO_PROCESSO>)Session["ListaDiario"] == null)
                {
                    if ((Int32)Session["TipoHistorico"] == 1)
                    {
                        CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                        Session["IdCRM"] = crm.CRM1_CD_ID;
                        listaMasterDiario = crm.DIARIO_PROCESSO.ToList();
                        Session["ListaDiario"] = listaMasterDiario;
                        Session["VoltaHistorico"] = 1;
                    }
                    else
                    {
                        listaMasterDiario = diaApp.GetAllItens(idAss).Where(p => p.CRM.CRM1_IN_ATIVO != 2).ToList();
                        Session["ListaDiario"] = listaMasterDiario;
                        Session["VoltaHistorico"] = 2;
                    }
                }
                else
                {
                    listaMasterDiario = ((List<DIARIO_PROCESSO>)Session["ListaDiario"]).ToList();
                }

                // Prepara lista
                Session["CRM"] = null;
                List<DIARIO_PROCESSO> list = (List<DIARIO_PROCESSO>)Session["ListaDiario"];
                list = list.OrderByDescending(p => p.DIPR_DT_DATA).ToList();
                ViewBag.Listas = list;
                List<USUARIO> listaTotal = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                List<CRM> listaTotal1 = CarregaCRM();
                ViewBag.CRM = new SelectList(listaTotal1.OrderBy(p => p.CRM1_NM_NOME), "CRM1_CD_ID", "CRM1_NM_NOME");

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["VoltaCRM"] = 22;
                Session["VoltaPedido"] = 22;
                Session["PontoAcao"] = 22;
                Session["VoltaAgenda"] = 44;
                Session["MensCRM"] = null;

                // Abre view
                objetoDiario = new DIARIO_PROCESSO();
                if (Session["FiltroDiario"] != null)
                {
                    objetoDiario = (DIARIO_PROCESSO)Session["FiltroDiario"];
                }
                return View(objetoDiario);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroDiario()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaDiario"] = null;
                Session["FiltroDiario"] = null;
                Session["VerDia"] = 0;
                return RedirectToAction("MontarTelaHistorico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarDiario(DIARIO_PROCESSO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Sanitização
                item.DIPR_NM_OPERACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.DIPR_NM_OPERACAO);
                item.DIPR_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.DIPR_DS_DESCRICAO); 
                
                // Prepara processo
                if ((Int32)Session["TipoHistorico"] == 1)
                {
                    item.CRM1_CD_ID = (Int32)Session["IdCRM"];
                }

                // Executa a operação
                List<DIARIO_PROCESSO> listaObj = new List<DIARIO_PROCESSO>();
                Session["FiltroDiario"] = item;
                Int32 volta = diaApp.ExecuteFilter(item.CRM1_CD_ID, item.DIPR_DT_DUMMY_1, item.DIPR_DT_DUMMY, item.USUA_CD_ID, item.DIPR_NM_OPERACAO, item.DIPR_DS_DESCRICAO, idAss, out listaObj);

                // Sucesso
                listaMasterDiario = listaObj;
                Session["ListaDiario"] = listaObj;
                return RedirectToAction("MontarTelaHistorico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MontarTelaHistoricoGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaDiario"] = null;
            Session["TipoHistorico"] = 2;
            return RedirectToAction("MontarTelaHistorico");
        }

        [HttpPost]
        public JsonResult GetAcoesAtraso()
        {
            try
            {
                var usu = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CRM_ACAO> lista = baseApp.GetAllAcoes(idAss).Where(p => p.USUA_CD_ID1 == usu.USUA_CD_ID & p.CRAC_IN_ATIVO == 1 & p.CRAC_IN_STATUS == 1 & p.CRM.CRM1_IN_ATIVO == 1 & p.CRAC_DT_PREVISTA.Value.Date < DateTime.Today.Date).ToList();

                if (lista.Count == 1)
                {
                    var hash = new Hashtable();
                    hash.Add("msg", "Você tem 1 ação em atraso");

                    return Json(hash);
                }
                else if (lista.Count > 1)
                {
                    var hash = new Hashtable();
                    hash.Add("msg", "Você tem " + lista.Count + " ações em atraso");
                    return Json(hash);
                }
                else
                {
                    return null; // Sem atrasos
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public ActionResult AcoesAtrasoClick()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                var usu = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CRM_ACAO> lista = baseApp.GetAllAcoes(idAss).Where(p => p.USUA_CD_ID1 == usu.USUA_CD_ID & p.CRAC_IN_ATIVO == 1 & p.CRAC_IN_STATUS == 1 & p.CRM.CRM1_IN_ATIVO == 1 & p.CRAC_DT_PREVISTA.Value.Date < DateTime.Today.Date).ToList();

                if (lista.Count == 1)
                {
                    return Json(lista.FirstOrDefault().CRAC_CD_ID);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public JsonResult GetModeloProposta(Int32 id)
        {
            try
            {
                // Recupera modelo
                TEMPLATE_PROPOSTA forn = tpApp.GetItemById(id);

                // Acerta HTML
                String cabecalho = forn.TEPR_TX_CABECALHO;
                String corpo = forn.TEPR_TX_TEXTO;
                String rodape = forn.TEPR_TX_RODAPE;

                if (cabecalho != null)
                {
                    cabecalho = cabecalho.Replace("\r\n", "<br />");
                    cabecalho = cabecalho.Replace("<p>", "");
                    cabecalho = cabecalho.Replace("</p>", "<br />");
                }
                if (corpo != null)
                {
                    corpo = corpo.Replace("\r\n", "<br />");
                    corpo = corpo.Replace("<p>", "");
                    corpo = corpo.Replace("</p>", "<br />");
                }
                if (rodape != null)
                {
                    rodape = rodape.Replace("\r\n", "<br />");
                    rodape = rodape.Replace("<p>", "");
                    rodape = rodape.Replace("</p>", "<br />");
                }

                // Retorno
                var hash = new Hashtable();
                hash.Add("intro", cabecalho);
                hash.Add("corpo", corpo);
                hash.Add("rodape", rodape);
                hash.Add("fixo", forn.TEPR_IN_FIXO);
                return Json(hash);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public JsonResult GetModeloEMail(Int32 id)
        {
            try
            {
                // Recupera modelo
                TEMPLATE_EMAIL forn = teApp.GetItemById(id);

                // Acerta HTML
                String mail = forn.TEEM_TX_CORPO;
                if (mail != null)
                {
                    mail = mail.Replace("\r\n", "<br />");
                    mail = mail.Replace("<p>", "");
                    mail = mail.Replace("</p>", "<br />");
                }

                // Retorno
                var hash = new Hashtable();
                hash.Add("mail", mail);
                hash.Add("fixo", forn.TEEM_IN_FIXO);
                return Json(hash);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM> CarregaCRMSemCache()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CRM> conf = baseApp.GetAllItens(idAss);

                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM> CarregaCRM()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM> conf = new List<CRM>();
                if (Session["CRMs"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["CRMAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<CRM>)Session["CRMs"];
                    }
                }
                Session["CRMs"] = conf;
                Session["CRMAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM> CarregaCRMGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM> conf = new List<CRM>();
                if (Session["CRMsGeral"] == null)
                {
                    conf = baseApp.GetAllItensAdm(idAss);
                }
                else
                {
                    if ((Int32)Session["CRMAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItensAdm(idAss);
                    }
                    else
                    {
                        conf = (List<CRM>)Session["CRMsGeral"];
                    }
                }
                Session["CRMsGeral"] = conf;
                Session["CRMAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<FUNIL> CarregaFunil()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<FUNIL> conf = new List<FUNIL>();
                if (Session["Funis"] == null)
                {
                    conf = funApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["FunilAlterada"] == 1)
                    {
                        conf = funApp.GetAllItens(idAss);
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
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM_ORIGEM> CarregaOrigem()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_ORIGEM> conf = new List<CRM_ORIGEM>();
                if (Session["Origens"] == null)
                {
                    conf = baseApp.GetAllOrigens(idAss);
                }
                else
                {
                    if ((Int32)Session["OrigemAlterada"] == 1)
                    {
                        conf = baseApp.GetAllOrigens(idAss);
                    }
                    else
                    {
                        conf = (List<CRM_ORIGEM>)Session["Origens"];
                    }
                }
                Session["Origens"] = conf;
                Session["OrigemAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<USUARIO> CarregaUsuario()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> conf = new List<USUARIO>();
                if (Session["Usuarios"] == null)
                {
                    conf = usuApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = usuApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<USUARIO>)Session["Usuarios"];
                    }
                }
                Session["UsuarioAlterada"] = 0;
                Session["Usuarios"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<MOTIVO_CANCELAMENTO> CarregaMotivoCancelamento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MOTIVO_CANCELAMENTO> conf = new List<MOTIVO_CANCELAMENTO>();
                if (Session["MotCancelamentos"] == null)
                {
                    conf = baseApp.GetAllMotivoCancelamento(idAss);
                }
                else
                {
                    if ((Int32)Session["MotCancelamentoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllMotivoCancelamento(idAss);
                    }
                    else
                    {
                        conf = (List<MOTIVO_CANCELAMENTO>)Session["MotCancelamentos"];
                    }
                }
                Session["MotCancelamentos"] = conf;
                Session["MotCancelamentoAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<MOTIVO_ENCERRAMENTO> CarregaMotivoEncerramento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MOTIVO_ENCERRAMENTO> conf = new List<MOTIVO_ENCERRAMENTO>();
                if (Session["MotEncerramentos"] == null)
                {
                    conf = baseApp.GetAllMotivoEncerramento(idAss);
                }
                else
                {
                    if ((Int32)Session["MotEncerramentoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllMotivoEncerramento(idAss);
                    }
                    else
                    {
                        conf = (List<MOTIVO_ENCERRAMENTO>)Session["MotEncerramentos"];
                    }
                }
                Session["MotEncerramentos"] = conf;
                Session["MotEncerramentoAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM_PEDIDO_VENDA> CarregaPedidoVenda()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_PEDIDO_VENDA> conf = new List<CRM_PEDIDO_VENDA>();
                if (Session["PedidoVendas"] == null)
                {
                    conf = baseApp.GetAllPedidos(idAss);
                }
                else
                {
                    if ((Int32)Session["PedidoVendaAlterada"] == 1)
                    {
                        conf = baseApp.GetAllPedidos(idAss);
                    }
                    else
                    {
                        conf = (List<CRM_PEDIDO_VENDA>)Session["PedidoVendas"];
                    }
                }
                Session["PedidoVendas"] = conf;
                Session["PedidoVendaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM_PEDIDO_VENDA> CarregaPedidoVendaGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_PEDIDO_VENDA> conf = new List<CRM_PEDIDO_VENDA>();
                if (Session["PedidoVendasGeral"] == null)
                {
                    conf = baseApp.GetAllPedidosGeral(idAss);
                }
                else
                {
                    if ((Int32)Session["PedidoVendaAlterada"] == 1)
                    {
                        conf = baseApp.GetAllPedidosGeral(idAss);
                    }
                    else
                    {
                        conf = (List<CRM_PEDIDO_VENDA>)Session["PedidoVendasGeral"];
                    }
                }
                Session["PedidoVendasGeral"] = conf;
                Session["PedidoVendaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PEDIDO_VENDA> CarregaVenda()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PEDIDO_VENDA> conf = new List<PEDIDO_VENDA>();
            conf = baseApp.GetAllVendas(idAss);
            return conf;
        }

        public List<TIPO_ACAO> CarregaTipoAcao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_ACAO> conf = new List<TIPO_ACAO>();
                if (Session["TipoAcoes"] == null)
                {
                    conf = baseApp.GetAllTipoAcao(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoAcaoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTipoAcao(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_ACAO>)Session["TipoAcoes"];
                    }
                }
                Session["TipoAcoes"] = conf;
                Session["TipoAcaoAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TIPO_FOLLOW> CarregaTipoFollow()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_FOLLOW> conf = new List<TIPO_FOLLOW>();
                if (Session["TipoFollows"] == null)
                {
                    conf = baseApp.GetAllTipoFollow(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoFollowAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTipoFollow(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_FOLLOW>)Session["TipoFollows"];
                    }
                }
                Session["TipoFollows"] = conf;
                Session["TipoFollowAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CLIENTE> CarregaCliente()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CLIENTE> conf = new List<CLIENTE>();
                if (Session["Clientes"] == null)
                {
                    conf = cliApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ClienteAlterada"] == 1)
                    {
                        conf = cliApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<CLIENTE>)Session["Clientes"];
                    }
                }
                Session["Clientes"] = conf;
                Session["ClienteAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<FILIAL> CarregaFilial()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<FILIAL> conf = new List<FILIAL>();
            if (Session["Filiais"] == null)
            {
                conf = baseApp.GetAllFilial(idAss);
            }
            else
            {
                if ((Int32)Session["FilialAlterada"] == 1)
                {
                    conf = baseApp.GetAllFilial(idAss);
                }
                else
                {
                    conf = (List<FILIAL>)Session["Filiais"];
                }
            }
            Session["Filiais"] = conf;
            Session["FilialAlterada"] = 0;
            return conf;
        }

        public List<TEMPLATE_PROPOSTA> CarregaTemplateProposta()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_PROPOSTA> conf = new List<TEMPLATE_PROPOSTA>();
                if (Session["TemplatePropostas"] == null)
                {
                    conf = baseApp.GetAllTemplateProposta(idAss);
                }
                else
                {
                    if ((Int32)Session["TemplatePropostaAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTemplateProposta(idAss);
                    }
                    else
                    {
                        conf = (List<TEMPLATE_PROPOSTA>)Session["TemplatePropostas"];
                    }
                }
                Session["TemplatePropostas"] = conf;
                Session["TemplatePropostaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult VerMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMensagensEnviadas"] = 7;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

        public ActionResult VerMensagensEnviadas1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMensagensEnviadas"] = 8;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

        public ActionResult VerMensagensEnviadas3()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMensagensEnviadas"] = 6;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

        public CONFIGURACAO CarregaConfiguracaoGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                CONFIGURACAO conf = new CONFIGURACAO();
                if (Session["Configuracao"] == null)
                {
                    conf = confApp.GetAllItems(idAss).FirstOrDefault();
                }
                else
                {
                    if ((Int32)Session["ConfAlterada"] == 1)
                    {
                        conf = confApp.GetAllItems(idAss).FirstOrDefault();
                    }
                    else
                    {
                        conf = (CONFIGURACAO)Session["Configuracao"];
                    }
                }
                Session["ConfAlterada"] = 0;
                Session["Configuracao"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult IncluirFollowCRM()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 id = (Int32)Session["IdCRM"];
                CRM item = baseApp.GetItemById(id);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                ViewBag.Follow = new SelectList(CarregaTipoFollow(), "TIFL_CD_ID", "TIFL_NM_NOME");

                CRM_FOLLOW coment = new CRM_FOLLOW();
                CRMFollowViewModel vm = Mapper.Map<CRM_FOLLOW, CRMFollowViewModel>(coment);
                vm.CRFL_DT_FOLLOW = DateTime.Now;
                vm.CRFL_IN_ATIVO = 1;
                vm.CRM1_CD_ID = item.CRM1_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                vm.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                vm.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                Session["PontoProposta"] = 85;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public ActionResult IncluirFollowCRM(CRMFollowViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            ViewBag.Follow = new SelectList(CarregaTipoFollow(), "TIFL_CD_ID", "TIFL_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRFL_DS_FOLLOW = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRFL_DS_FOLLOW);

                    // Executa a operação
                    CRM_FOLLOW item = Mapper.Map<CRMFollowViewModel, CRM_FOLLOW>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.CRM_FOLLOW.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Gera diario
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRFL_CD_ID = item.CRFL_CD_ID;
                    dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Follow-Up de Processo";
                    dia.DIPR_DS_DESCRICAO = "Follow-Up de Processo " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ifoCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Processo: " + item.CRM.CRM1_NM_NOME + " - Acompanhamento: " + item.CRFL_DS_FOLLOW,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["VoltaTela"] = 3;
                    Session["FlagCRM"] = 1;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
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

        [HttpGet]
        public ActionResult EditarFollowCRM(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                CRM_FOLLOW item = baseApp.GetFollowById(id);
                CRMFollowViewModel vm = Mapper.Map<CRM_FOLLOW, CRMFollowViewModel>(item);
                Session["FollowAntes"] = item;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFollowCRM(CRMFollowViewModel vm)
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
                    vm.CRFL_DS_FOLLOW = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CRFL_DS_FOLLOW);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_FOLLOW item = Mapper.Map<CRMFollowViewModel, CRM_FOLLOW>(vm);
                    Int32 volta = baseApp.ValidateEditFollow(item);
                    CRM crm = (CRM)Session["CRM"];

                    // Monta Log                    
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "efoCRM1",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Processo: " + crm.CRM1_NM_NOME + " - Acompanhamento: " + item.CRFL_DS_FOLLOW,
                        LOG_TX_REGISTRO_ANTES = null,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["CRMAlterada"] = 1;
                    Session["VoltaTela"] = 3;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    Session["FlagCRM"] = 1;
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
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

        [HttpGet]
        public ActionResult ExcluirFollowCRM(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                CRM_FOLLOW item = baseApp.GetFollowById(id);
                item.CRFL_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditFollow(item);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CRM crm = (CRM)Session["CRM"];

                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xfoCRM1",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + crm.CRM1_NM_NOME + " - Acompanhamento: " + item.CRFL_DS_FOLLOW,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                Session["CRMAlterada"] = 1;
                Session["FlagCRM"] = 1;
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerFollow(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_FOLLOW item = baseApp.GetFollowById(id);
                objetoAntes = (CRM)Session["CRM"];
                CRMFollowViewModel vm = Mapper.Map<CRM_FOLLOW, CRMFollowViewModel>(item);
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public JsonResult GetLembreteHoje()
        {
            try
            {
                var usu = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_FOLLOW> follows = CarregaCRMFollow().Where(p => p.TIFL_CD_ID == 3 & p.CRFL_DT_PREVISTA.Value.Date == DateTime.Today.Date).ToList();
                if (follows.Count == 1)
                {
                    var hash = new Hashtable();
                    hash.Add("msg", "Você possui 1 lembrete para hoje");
                    return Json(hash);
                }
                else if (follows.Count > 1)
                {
                    var hash = new Hashtable();
                    hash.Add("msg", "Você possui " + follows.Count + " lembretes para hoje");

                    return Json(hash);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetAlertaHoje()
        {
            try
            {
                var usu = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_FOLLOW> follows = CarregaCRMFollow().Where(p => p.TIFL_CD_ID == 2 & p.CRFL_DT_PREVISTA.Value.Date == DateTime.Today.Date).ToList();
                if (follows.Count == 1)
                {
                    var hash = new Hashtable();
                    hash.Add("msg", "Você possui 1 alerta para hoje");
                    return Json(hash);
                }
                else if (follows.Count > 1)
                {
                    var hash = new Hashtable();
                    hash.Add("msg", "Você possui " + follows.Count + " alertas para hoje");

                    return Json(hash);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM_FOLLOW> CarregaCRMFollow()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_FOLLOW> conf = new List<CRM_FOLLOW>();
                conf = baseApp.GetAllFollow(idAss);
                Session["CRMFollow"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetLembreteHojeClick()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                var usu = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CRM_FOLLOW> follows = CarregaCRMFollow().Where(p => p.TIFL_CD_ID == 3 & p.CRFL_DT_PREVISTA.Value.Date == DateTime.Today.Date).ToList();

                if (follows.Count == 1)
                {
                    return Json(follows.FirstOrDefault().CRFL_CD_ID);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult GetAlertaHojeClick()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                var usu = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CRM_FOLLOW> follows = CarregaCRMFollow().Where(p => p.TIFL_CD_ID == 2 & p.CRFL_DT_PREVISTA.Value.Date == DateTime.Today.Date).ToList();

                if (follows.Count == 1)
                {
                    return Json(follows.FirstOrDefault().CRFL_CD_ID);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAnotacaoAcompanhamento()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                List<CRM_COMENTARIO> anots = baseApp.GetAllAnotacao(idAss);
                List<CRM_FOLLOW> follows = baseApp.GetAllFollow(idAss);

                ViewBag.ListaAnot = anots;
                ViewBag.ListaFollow = follows;
                Session["VoltaTela"] = 3;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAnotacao(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_COMENTARIO item = baseApp.GetComentarioById(id);
                objetoAntes = (CRM)Session["CRM"];
                CRMComentarioViewModel vm = Mapper.Map<CRM_COMENTARIO, CRMComentarioViewModel>(item);
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagemEnviada()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                if (Session["MensagensEnviadas"] == null)
                {
                    conf = meApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["MensagensEnviadaAlterada"] == 1)
                    {
                        conf = meApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<MENSAGENS_ENVIADAS_SISTEMA>)Session["MensagensEnviadas"];
                    }
                }
                Session["CatAgendas"] = conf;
                Session["MensagensEnviadaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexoCRM(Int32 id)
        {
            try
            {
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                CRM_ANEXO item = baseApp.GetAnexoById(id);
                item.CRAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnexo(item);
                Session["AgendaAlterada"] = 1;
                MontarLogGeral log = new MontarLogGeral();
                String mensagem = log.MontarLog(3, 3, 0, usu.USUA_NM_NOME);
                Trace.WriteLine(mensagem);

                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

    }
}