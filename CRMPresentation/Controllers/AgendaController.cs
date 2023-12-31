﻿using System;
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
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using static System.Net.Mime.MediaTypeNames;
using Azure.Communication.Email;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class AgendaController : Controller
    {
        private readonly IAgendaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly ICategoriaAgendaAppService caApp;
        private readonly ITemplateAppService temApp;
        private readonly IRecursividadeAppService recApp;

        private String msg;
        private Exception exception;
        private AGENDA objeto = new AGENDA();
        private AGENDA objetoAntes = new AGENDA();
        private List<AGENDA> listaMaster = new List<AGENDA>();
        private List<AGENDA> listaMasterCalendario = new List<AGENDA>();
        private List<Hashtable> listaCalendario = new List<Hashtable>();
        private String extensao;

        public AgendaController(IAgendaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IMensagemEnviadaSistemaAppService meApps, ICategoriaAgendaAppService caApps, ITemplateAppService temApps, IRecursividadeAppService recApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
            meApp = meApps;
            caApp = caApps;
            temApp = temApps;
            recApp = recApps;
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

        private int div(int x)
        {
            return x / 0;
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            listaMaster = new List<AGENDA>();
            Session["Agenda"] = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaAgendaCalendario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                Session["VoltaAgenda"] = 3;
                Session["VoltaDashAgenda"] = 2;
                Session["FiltroAgendaCalendario"] = 2;
                Session["VoltaAgendaCRM"] = 0;
                var usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                ViewBag.Tipos = new SelectList(CarregaCatAgenda(), "CAAG_CD_ID", "CAAG_NM_NOME");
                ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

                AGENDA item = new AGENDA();
                AgendaViewModel vm = Mapper.Map<AGENDA, AgendaViewModel>(item);
                vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
                vm.AGEN_DT_DATA = DateTime.Today.Date;
                vm.AGEN_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.AGEN_IN_STATUS = 1;

                listaMasterCalendario = baseApp.GetByUser(usuario.USUA_CD_ID, idAss).Where(p => p.AGEN_DT_DATA.Month == DateTime.Today.Date.Month).ToList();
                Session["ListaAgenda"] = listaMasterCalendario;
                Session["EnviaLink"] = 0;
                Session["EditaLink"] = 0;
                Session["NaoFezNada"] = 0;
                Session["TipoAgenda"] = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AccAGEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);
                return View(vm);
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

        [HttpPost]
        public JsonResult GetEventosCalendario()
        {
            try
            {
                var usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<Hashtable> listaCalendario = new List<Hashtable>();

                listaMaster = baseApp.GetByUser(usuario.USUA_CD_ID, idAss).Where(p => p.AGEN_DT_DATA.Month == DateTime.Today.Date.Month).ToList();
                Session["ListaAgenda"] = listaMaster;

                foreach (var item in (List<AGENDA>)Session["ListaAgenda"])
                {
                    var hash = new Hashtable();

                    hash.Add("id", item.AGEN_CD_ID);
                    hash.Add("title", item.AGEN_NM_TITULO);
                    hash.Add("start", (item.AGEN_DT_DATA + item.AGEN_HR_HORA).ToString("yyyy-MM-dd HH:mm:ss"));
                    hash.Add("description", (new DateTime() + item.AGEN_HR_HORA).ToString("HH:mm"));

                    listaCalendario.Add(hash);
                }
                return Json(listaCalendario);
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

        [HttpPost]
        public JsonResult GetDetalhesEvento(Int32 id)
        {
            try
            {
                var evento = baseApp.GetById(id);

                var hash = new Hashtable();
                hash.Add("data", evento.AGEN_DT_DATA.ToShortDateString());
                hash.Add("hora", evento.AGEN_HR_HORA.ToString());
                hash.Add("final", evento.AGEN_HR_FINAL.ToString());
                hash.Add("categoria", evento.CATEGORIA_AGENDA.CAAG_NM_NOME);
                hash.Add("titulo", evento.AGEN_NM_TITULO);
                hash.Add("contato", evento.USUARIO == null ? "-" : evento.USUARIO.USUA_NM_NOME);
                if (evento.AGEN_IN_STATUS == 1)
                {
                    hash.Add("status", "Ativo");
                }
                else if (evento.AGEN_IN_STATUS == 2)
                {
                    hash.Add("status", "Suspenso");
                }
                else
                {
                    hash.Add("status", "Encerrado");
                }
                hash.Add("anexo", evento.AGENDA_ANEXO.Count);

                return Json(hash);
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
        public ActionResult MontarTelaAgenda()
        {
            // Verifica se tem usuario logado
            try
            {
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["FiltroAgendaCalendario"] = 1;

                // Carrega listas
                if (Session["ListaAgenda"] == null)
                {
                    listaMaster = baseApp.GetByUser(usuario.USUA_CD_ID, idAss).Where(p => p.AGEN_DT_DATA.Month == DateTime.Today.Date.Month).ToList();
                    Session["ListaAgenda"] = listaMaster;
                }
                ViewBag.Listas = ((List<AGENDA>)Session["ListaAgenda"]).OrderByDescending(x => x.AGEN_DT_DATA).ThenByDescending(x => x.AGEN_HR_HORA).ToList<AGENDA>();
                ViewBag.Itens = ((List<AGENDA>)Session["ListaAgenda"]).Count;
                ViewBag.Tipos = new SelectList(CarregaCatAgenda(), "CAAG_CD_ID", "CAAG_NM_NOME");

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                Session["VoltaAgendaCRM"] = 0;
                if ((Int32)Session["MensAgenda"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAgenda"] == 11)
                {
                    String texto = CRMSys_Base.ResourceManager.GetString("M0317", CultureInfo.CurrentCulture) + " " + ((Int32)Session["TotalEnvios"]).ToString();
                    ModelState.AddModelError("", texto);
                }
                if ((Int32)Session["MensAgenda"] == 200)
                {
                    String texto = CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture);
                    ModelState.AddModelError("", texto);
                }

                // Abre view
                Session["ListaAgendaTimeLine"] = null;
                Session["MensAgenda"] = 0;
                objeto = new AGENDA();
                objeto.AGEN_DT_DATA = DateTime.Today.Date;
                Session["VoltaAgenda"] = 1;
                Session["VoltaDashAgenda"] = 1;
                Session["FiltroAgendaCalendario"] = 1;
                Session["VoltaMensagem"] = 99;
                Session["AbaAgenda"] = 1;
                Session["EnviaLink"] = 0;
                Session["EditaLink"] = 0;
                Session["EditaUnico"] = 0;
                Session["IncluiuContato"] = 0;
                Session["NaoFezNada"] = 0;
                Session["TipoAgenda"] = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AccAGEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);
                return View(objeto);
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

        public ActionResult RetirarFiltroAgenda()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaAgenda"] = null;
                if ((Int32)Session["VoltaAgenda"] == 2)
                {
                    return RedirectToAction("VerTimelineAgenda");
                }
                return RedirectToAction("MontarTelaAgenda");
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

        public ActionResult MostrarTudoAgenda()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                Session["ListaAgenda"] = listaMaster;
                if ((Int32)Session["VoltaAgenda"] == 2)
                {
                    return RedirectToAction("VerTimelineAgenda");
                }
                return RedirectToAction("MontarTelaAgenda");
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

        [HttpPost]
        public ActionResult FiltrarAgenda(AGENDA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Sanitização
                item.AGEN_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.AGEN_NM_TITULO);
                item.AGEN_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.AGEN_DS_DESCRICAO);

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<AGENDA> listaObj = new List<AGENDA>();
                Session["FiltroAgenda"] = item;
                Tuple<Int32, List<AGENDA>, Boolean> volta = baseApp.ExecuteFilterTuple(item.AGEN_DT_DATA, item.CAAG_CD_ID, item.AGEN_NM_TITULO, item.AGEN_DS_DESCRICAO, idAss, usuario.USUA_CD_ID, 1);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensAgenda"] = 1;
                    return RedirectToAction("MontarTelaAgenda");
                }

                // Sucesso
                listaMaster = volta.Item2;
                Session["ListaAgenda"] = volta.Item2;

                // Sucesso
                Session["MensAgenda"] = 0;
                if ((Int32)Session["VoltaAgenda"] == 2)
                {
                    return RedirectToAction("VerTimelineAgenda");
                }
                return RedirectToAction("MontarTelaAgenda");
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

        public ActionResult VoltarBaseAgenda()
        {
            try
            {
                if ((USUARIO)Session["UserCredentials"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Envia link
                if ((Int32)Session["VoltaCRM"] == 11)
                {
                    return RedirectToAction("VoltarAcompanhamentoCRMBase", "CRM");
                }
                if ((Int32)Session["VoltaAgenda"] == 50)
                {
                    return RedirectToAction("EnviarLinkReuniaoFormNovoLista");
                }
                if ((Int32)Session["VoltaAgenda"] == 2)
                {
                    return RedirectToAction("VerTimelineAgenda");
                }
                if ((Int32)Session["VoltaAgenda"] == 3)
                {
                    return RedirectToAction("MontarTelaAgendaCalendario");
                }

                if ((Int32)Session["VoltaAgenda"] == 11)
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
                if ((Int32)Session["VoltaAgenda"] == 44)
                {
                    return RedirectToAction("MontarTelaHistorico", "CRM");
                }
                if ((Int32)Session["VoltaAgenda"] == 22)
                {
                    return RedirectToAction("VoltarAcompanhamentoCRMBase", "CRM");
                }
                if ((Int32)Session["VoltaAgenda"] == 10)
                {
                    return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
                }
                if ((Int32)Session["VoltaAgenda"] == 2)
                {
                    return RedirectToAction("VerTimelineAgenda");
                }
                else if ((Int32)Session["FiltroAgendaCalendario"] == 1)
                {
                    return RedirectToAction("MontarTelaAgenda");
                }
                else if ((Int32)Session["FiltroAgendaCalendario"] == 2)
                {
                    return RedirectToAction("MontarTelaAgendaCalendario");
                }
                return RedirectToAction("MontarTelaAgenda");
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

        [HttpGet]
        public ActionResult IncluirAgenda()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                ViewBag.Tipos = new SelectList(CarregaCatAgenda(), "CAAG_CD_ID", "CAAG_NM_NOME");
                ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                AGENDA item = new AGENDA();
                AgendaViewModel vm = Mapper.Map<AGENDA, AgendaViewModel>(item);
                vm.AGEN_DT_DATA = DateTime.Today.Date;
                vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
                vm.AGEN_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.AGEN_CD_USUARIO = usuario.USUA_CD_ID;
                vm.AGEN_IN_STATUS = 1;
                return View(vm);
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

        [HttpPost]
        public ActionResult IncluirAgenda(AgendaViewModel vm)
        {
            ViewBag.Tipos = new SelectList(CarregaCatAgenda(), "CAAG_CD_ID", "CAAG_NM_NOME");
            ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    var result = new Hashtable();
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Sanitização
                    vm.AGEN_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGEN_NM_TITULO);
                    vm.AGEN_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGEN_DS_DESCRICAO);
                    vm.AGEN_LK_REUNIAO = CrossCutting.UtilitariosGeral.CleanStringLink(vm.AGEN_LK_REUNIAO);
                    vm.AGEN_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGEN_TX_OBSERVACOES);

                    // Executa a operação
                    AGENDA item = Mapper.Map<AgendaViewModel, AGENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    if (Session["IdCRM"] != null)
                    {
                        item.CRM1_CD_ID = (Int32)Session["IdCRM"];
                    }
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAgenda"] = 10;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0229", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensAgenda"] = 20;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0321", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensAgenda"] = 33;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0335", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensAgenda"] = 34;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0336", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    TimeSpan dif = (TimeSpan)item.AGEN_HR_FINAL - item.AGEN_HR_HORA;

                    // Cria pastas
                    String caminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Agenda/" + item.AGEN_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<AGENDA>();
                    Session["ListaAgenda"] = null;
                    Session["IdVolta"] = item.AGEN_CD_ID;
                    Session["IdAgenda"] = item.AGEN_CD_ID;

                    // Trata anexo
                    if (Session["FileQueueAgenda"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueAgenda"];
                        foreach (var file in fq)
                        {
                            UploadFileQueueAgenda(file);
                        }
                        Session["FileQueueAgenda"] = null;
                    }

                    // Retornos
                    Session["AbaAgenda"] = 2;
                    return RedirectToAction("VoltarAnexoAgenda");
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
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public JsonResult EditarAgendaOnChange(Int32 id, DateTime data)
        {
            try
            {
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                AGENDA obj = baseApp.GetById(id);
                AGENDA item = new AGENDA();
                item.AGEN_CD_ID = id;
                item.USUA_CD_ID = obj.USUA_CD_ID;
                item.ASSI_CD_ID = obj.ASSI_CD_ID;
                item.CAAG_CD_ID = obj.CAAG_CD_ID;
                item.AGEN_HR_HORA = data.TimeOfDay;
                item.AGEN_DT_DATA = data.Date;
                item.AGEN_IN_ATIVO = 1;
                item.AGEN_NM_TITULO = obj.AGEN_NM_TITULO;
                item.AGEN_DS_DESCRICAO = obj.AGEN_DS_DESCRICAO;
                item.AGEN_TX_OBSERVACOES = obj.AGEN_TX_OBSERVACOES;
                item.AGEN_IN_STATUS = obj.AGEN_IN_STATUS;

                Int32 volta = baseApp.ValidateEdit(item, obj, usu);

                if (volta == 0)
                {
                    ((List<AGENDA>)Session["ListaAgenda"]).Where(x => x.AGEN_CD_ID == id).First().AGEN_DT_DATA = data;
                }
                return Json(volta);
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
                return Json(0);
            }
        }

        [HttpGet]
        public ActionResult EditarAgenda(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Listas
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Tipos = new SelectList(CarregaCatAgenda(), "CAAG_CD_ID", "CAAG_NM_NOME");
                ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
                List<SelectListItem> status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
                status.Add(new SelectListItem() { Text = "Suspenso", Value = "2" });
                status.Add(new SelectListItem() { Text = "Encerrado", Value = "3" });
                ViewBag.Status = new SelectList(status, "Value", "Text");

                // Mensagens
                if ((Int32)Session["MensAgenda"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAgenda"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAgenda"] == 20)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0321", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAgenda"] == 21)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0333", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAgenda"] == 23)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0336", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAgenda"] == 24)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0335", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAgenda"] == 90)
                {
                    String texto = CRMSys_Base.ResourceManager.GetString("M0317", CultureInfo.CurrentCulture) + " " + ((Int32)Session["TotalEnvios"]).ToString();
                    ModelState.AddModelError("", texto);
                }

                // Prepara view
                AGENDA item = baseApp.GetItemById(id);
                Session["AgendaData"] = item.AGEN_DT_DATA.Date;
                Session["AgendaInicio"] = item.AGEN_HR_HORA.ToString();
                Session["AgendaFinal"] = item.AGEN_HR_FINAL.ToString();
                Session["AgendaNome"] = item.AGEN_NM_TITULO;
                Session["AgendaDesc"] = item.AGEN_DS_DESCRICAO;
                Session["AgendaLink"] = item.AGEN_LK_REUNIAO;

                Session["MensAgenda"] = 0;
                Session["objetoAntes"] = item;
                Session["Agenda"] = item;
                Session["IdVolta"] = id;
                Session["IdAgenda"] = id;
                Session["VoltaAgenda"] = 1;
                Session["VoltaTela"] = 1;
                AgendaViewModel vm = Mapper.Map<AGENDA, AgendaViewModel>(item);
                Session["AgendaVm"] = vm;
                return View(vm);
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

        [HttpPost]
        public ActionResult EditarAgenda(AgendaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Monta listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(CarregaCatAgenda(), "CAAG_CD_ID", "CAAG_NM_NOME");
            ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            status.Add(new SelectListItem() { Text = "Suspenso", Value = "2" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "3" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.AGEN_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGEN_NM_TITULO);
                    vm.AGEN_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGEN_DS_DESCRICAO);
                    vm.AGEN_LK_REUNIAO = CrossCutting.UtilitariosGeral.CleanStringLink(vm.AGEN_LK_REUNIAO);
                    vm.AGEN_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGEN_TX_OBSERVACOES);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    AGENDA item = Mapper.Map<AgendaViewModel, AGENDA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (AGENDA)Session["objetoAntes"], usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAgenda"] = 21;
                        return RedirectToAction("VoltarAnexoAgenda");
                    }
                    if (volta == 2)
                    {
                        Session["MensAgenda"] = 20;
                        return RedirectToAction("VoltarAnexoAgenda");
                    }
                    if (volta == 3)
                    {
                        Session["MensAgenda"] = 24;
                        return RedirectToAction("VoltarAnexoAgenda");
                    }
                    if (volta == 4)
                    {
                        Session["MensAgenda"] = 23;
                        return RedirectToAction("VoltarAnexoAgenda");
                    }

                    // Sucesso
                    Session["AgendaAlterada"] = 1;
                    listaMaster = new List<AGENDA>();
                    Session["ListaAgenda"] = null;
                    Session["AbaAgenda"] = 1;

                    // Envia link
                    if (item.AGEN_DT_DATA.Date != (DateTime)Session["AgendaData"] || item.AGEN_HR_HORA.ToString() != (String)Session["AgendaInicio"] || item.AGEN_HR_FINAL.ToString() != (String)Session["AgendaFinal"] || item.AGEN_NM_TITULO != (String)Session["AgendaNome"] || item.AGEN_DS_DESCRICAO != (String)Session["AgendaDesc"] || item.AGEN_LK_REUNIAO != (String)Session["AgendaLink"])
                    {
                        if (item.AGEN_LK_REUNIAO != null)
                        {
                            Session["EditaLink"] = 1;
                            Session["VoltaAgenda"] = 50;
                            Session["NaoFezNada"] = 1;
                            return RedirectToAction("VoltarBaseAgenda");
                        }
                    }

                    // retorno
                    if ((Int32)Session["VoltaAgenda"] == 2)
                    {
                        return RedirectToAction("VerTimelineAgenda");
                    }
                    if ((Int32)Session["VoltaAgenda"] == 3)
                    {
                        return RedirectToAction("MontarTelaAgendaCalendario");
                    }
                    if ((Int32)Session["VoltaAgenda"] == 22)
                    {
                        return RedirectToAction("VoltarAcompanhamentoCRM" , "CRM");
                    }
                    return RedirectToAction("VoltarBaseAgenda");
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
            else
            {
                Int32 id = (Int32)Session["IdVolta"];
                AGENDA item = baseApp.GetItemById(id);
                objetoAntes = item;
                AgendaViewModel vm1 = (AgendaViewModel)Session["AgendaVm"];
                return View(vm1);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAgenda(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se tem usuario logado
            USUARIO usu = (USUARIO)Session["UserCredentials"];

            // Executar
            try
            {
                //int x = div(50);
                AGENDA item = baseApp.GetItemById(id);
                objetoAntes = (AGENDA)Session["Agenda"];
                item.AGEN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usu);
                listaMaster = new List<AGENDA>();
                Session["ListaAgenda"] = null;
                Session["AgendaAlterada"] = 1;

                if ((Int32)Session["VoltaAgenda"] == 3)
                {
                    return RedirectToAction("MontarTelaAgendaCalendario");
                }
                if ((Int32)Session["VoltaAgenda"] == 3)
                {
                    return RedirectToAction("MontarTelaAgendaCalendario");
                }
                return RedirectToAction("MontarTelaAgenda");
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

        [HttpGet]
        public ActionResult ReativarAgenda(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica se tem usuario logado
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Executar
                AGENDA item = baseApp.GetItemById(id);
                objetoAntes = (AGENDA)Session["Agenda"];
                item.AGEN_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usu);
                listaMaster = new List<AGENDA>();
                Session["ListaAgenda"] = null;
                Session["AgendaAlterada"] = 1;

                return RedirectToAction("MontarTelaAgenda");
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

        [HttpGet]
        public ActionResult VerAnexoAgenda(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                AGENDA_ANEXO item = baseApp.GetAnexoById(id);
                return View(item);
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

        public ActionResult VoltarAnexoAgenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaAgenda"] == 51)
            {
                return RedirectToAction("EnviarLinkReuniaoFormNovoLista");
            }
            return RedirectToAction("EditarAgenda", new { id = (Int32)Session["IdVolta"] });
        }

        public ActionResult VoltarGrava()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["NaoFezNada"] == 1)
            {
                return RedirectToAction("EnviarLinkReuniaoFormNovoLista");
            }
            if ((Int32)Session["NaoFezNada"] == 5)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaAgenda");
        }

        public ActionResult AlteraReuniao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarTemplate", "Template", new { codigo = "Reuniao" });
        }

        [HttpGet]
        public ActionResult VerAnexoAgendaAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                AGENDA_ANEXO item = baseApp.GetAnexoById(id);
                return View(item);
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

        public FileResult DownloadAgenda(Int32 id)
        {
            try
            {
                AGENDA_ANEXO item = baseApp.GetAnexoById(id);
                String arquivo = item.AGAN_AQ_ARQUIVO;
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
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files)
        {
            try
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
                    queue.Add(f);
                }
                Session["FileQueueAgenda"] = queue;
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
            }
        }

        [HttpPost]
        public ActionResult UploadFileQueueAgenda(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if (file == null)
                {
                    Session["MensAgenda"] = 10;
                    return RedirectToAction("VoltarAnexoAgenda");
                }

                AGENDA item = baseApp.GetById((Int32)Session["IdVolta"]);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;

                if (fileName.Length > 250)
                {
                    Session["MensAgenda"] = 11;
                    return RedirectToAction("VoltarAnexoAgenda");
                }

                String caminho = "/Imagens/" + ((Int32)Session["IdAssinante"]).ToString() + "/Agenda/" + item.AGEN_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                AGENDA_ANEXO foto = new AGENDA_ANEXO();
                foto.AGAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.AGAN_DT_ANEXO = DateTime.Today;
                foto.AGAN_IN_ATIVO = 1;
                Int32 tipo = 7;
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
                foto.AGAN_IN_TIPO = tipo;
                foto.AGAN_NM_TITULO = fileName;
                foto.AGEN_CD_ID = item.AGEN_CD_ID;

                item.AGENDA_ANEXO.Add(foto);
                objetoAntes = item;
                Session["ListaAgenda"] = null;
                Int32 volta = baseApp.ValidateEdit(item, usu);
                return RedirectToAction("VoltarAnexoAgenda");
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

        [HttpPost]
        public ActionResult UploadFileAgenda(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if (file == null)
                {
                    Session["MensAgenda"] = 10;
                    return RedirectToAction("VoltarAnexoAgenda");
                }

                AGENDA item = baseApp.GetById((Int32)Session["IdVolta"]);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);

                if (fileName.Length > 250)
                {
                    Session["MensAgenda"] = 11;
                    return RedirectToAction("VoltarAnexoAgenda");
                }

                String caminho = "/Imagens/" + ((Int32)Session["IdAssinante"]).ToString() + "/Agenda/" + item.AGEN_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                AGENDA_ANEXO foto = new AGENDA_ANEXO();
                foto.AGAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.AGAN_DT_ANEXO = DateTime.Today;
                foto.AGAN_IN_ATIVO = 1;
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
                foto.AGAN_IN_TIPO = tipo;
                foto.AGAN_NM_TITULO = fileName;
                foto.AGEN_CD_ID = item.AGEN_CD_ID;

                item.AGENDA_ANEXO.Add(foto);
                objetoAntes = item;
                Session["ListaAgenda"] = null;
                Int32 volta = baseApp.ValidateEdit(item, usu);
                return RedirectToAction("VoltarAnexoAgenda");
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

        [HttpGet]
        public ActionResult VerTimelineAgenda()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Monta listas
                ViewBag.Tipos = new SelectList(CarregaCatAgenda(), "CAAG_CD_ID", "CAAG_NM_NOME");

                // Carrega listas
                List<AGENDA> lista = ((List<AGENDA>)Session["ListaAgenda"]).Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).OrderByDescending(x => x.AGEN_HR_HORA).ToList<AGENDA>();
                lista = lista.Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList();
                ViewBag.Listas = lista;
                ViewBag.Agenda = ((List<AGENDA>)Session["ListaAgenda"]).Count;

                objeto = new AGENDA();
                return View(objeto);
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

        public ActionResult VoltarTimeline()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaAgenda"] == 3)
            {
                return RedirectToAction("MontarTelaAgendaCalendario");
            }
            return RedirectToAction("VoltarBaseAgenda");
        }

        [HttpPost]
        public ActionResult FiltrarTimelineAgenda(AGENDA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Sanitização
                item.AGEN_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.AGEN_NM_TITULO);
                item.AGEN_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.AGEN_DS_DESCRICAO);

                // Executa a operação
                List<AGENDA> listaObj = new List<AGENDA>();
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["FiltroAgenda"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.AGEN_DT_DATA, item.CAAG_CD_ID, item.AGEN_NM_TITULO, item.AGEN_DS_DESCRICAO, idAss, usuario.USUA_CD_ID, 1, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAgendaTimeline"] = 1;
                    return RedirectToAction("VerTimelineAgenda");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaAgendaTimeLine"] = listaObj;
                return RedirectToAction("VerTimelineAgenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCrdentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioLista()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "AgendaLista" + "_" + data + ".pdf";
                List<AGENDA> lista = (List<AGENDA>)Session["ListaAgenda"];
                AGENDA filtro = (AGENDA)Session["FiltroAgenda"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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
                    image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Agenda - Listagem", meuFont2))
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
                table = new PdfPTable(new float[] { 60f, 60f, 60f, 90f, 200f, 120f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Itens de Agenda selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 8;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Final", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Categoria", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título", meuFont))
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
                cell = new PdfPCell(new Paragraph("Anexos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (AGENDA item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.AGEN_DT_DATA.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.AGEN_HR_HORA.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.AGEN_HR_FINAL.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_AGENDA.CAAG_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.AGEN_NM_TITULO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.AGEN_IN_STATUS == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.AGEN_IN_STATUS == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Suspenso", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Encerrado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.AGENDA_ANEXO.Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph(item.AGENDA_ANEXO.Count.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
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
                    if (filtro.CAAG_CD_ID > 0)
                    {
                        parametros += "Categoria: " + filtro.CAAG_CD_ID;
                        ja = 1;
                    }
                    if (filtro.AGEN_DT_DATA != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Data: " + filtro.AGEN_DT_DATA.ToShortDateString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Data: " + filtro.AGEN_DT_DATA.ToShortDateString();
                        }
                    }
                    if (filtro.AGEN_NM_TITULO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Título: " + filtro.AGEN_NM_TITULO;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Título: " + filtro.AGEN_NM_TITULO;
                        }
                    }
                    if (filtro.AGEN_DS_DESCRICAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Descrição: " + filtro.AGEN_DS_DESCRICAO;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Descrição: " + filtro.AGEN_DS_DESCRICAO;
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

                return RedirectToAction("MontarTelaAgenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMsys", 1, (USUARIO)Session["UserCrdentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
        }

        public async Task<ActionResult> EnviarLinkReuniao(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera agenda
                AGENDA age = baseApp.GetItemById(id);
                USUARIO usuario = usuApp.GetItemById(age.AGEN_CD_USUARIO.Value);

                // Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensAgenda"] = 200;
                    return RedirectToAction("MontarTelaAgenda");
                }

                // Monta mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = usuario.USUA_NM_NOME;
                mens.ID = usuario.USUA_CD_ID;
                mens.MODELO = usuario.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_TX_TEXTO = "Foi agendada uma reunião para o Sr(a). em " + age.AGEN_DT_DATA.ToShortDateString() + " às " + age.AGEN_HR_HORA.ToString() + " horas, com assunto " + age.AGEN_NM_TITULO + ". Favor acessar o link em anexo e tomar as providências necessárias.";
                mens.MENS_NM_LINK = age.AGEN_LK_REUNIAO;
                mens.MENS_NM_NOME = "Link para Reunião";
                mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                mens.MENS_IN_USUARIO = age.USUARIO1.USUA_CD_ID;

                // Executa a operação
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = await envio.ProcessaEnvioEMailGeral(mens, usuario);

                // Grava envio
                String guid = new Guid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Success", guid, null, "Envio de link de Reunião");

                return RedirectToAction("MontarTelaAgenda");
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

        public async Task<ActionResult> EnviarLinkReuniaoForm()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                //Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensAgenda"] = 200;
                    return RedirectToAction("MontarTelaAgenda");
                }

                // Recupera agenda
                AGENDA age = baseApp.GetItemById((Int32)Session["IdAgenda"]);
                USUARIO usuario = usuApp.GetItemById(age.AGEN_CD_USUARIO.Value);

                // Monta mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.MENS_NM_NOME = "Agendamento - Reunião";
                mens.ID = usuario.USUA_CD_ID;
                mens.MENS_NM_CAMPANHA = age.USUARIO1.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_TX_TEXTO = "Foi agendada uma reunião para o Sr(a). em " + age.AGEN_DT_DATA.ToShortDateString() + " às " + age.AGEN_HR_HORA.ToString() + " horas, com assunto " + age.AGEN_NM_TITULO + ". Favor acessar o link em anexo e tomar as providências necessárias.";
                mens.MENS_NM_LINK = age.AGEN_LK_REUNIAO;
                mens.MENS_IN_USUARIO = age.USUARIO1.USUA_CD_ID;

                // Executa a operação
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = await envio.ProcessaEnvioEMailGeral(mens, usuario);

                // Grava envio
                String guid = new Guid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Success", guid, null, "Envio de link de Reunião");

                Session["AgendaAlterada"] = 1;
                if ((Int32)Session["VoltaAgenda"] == 3)
                {
                    return RedirectToAction("MontarTelaAgendaCalendario");
                }
                Session["MensAgenda"] = 11;
                return RedirectToAction("MontarTelaAgenda");
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

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailReuniao(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                // Recupera usuario
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO cont = (USUARIO)Session["Usuario"];

                // Processa e-mail
                CONFIGURACAO conf = CarregaConfiguracaoGeral();

                // Prepara cabeçalho
                String cab = "Prezado Sr(a). <b>" + vm.NOME + "</b>";

                // Prepara rodape
                ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                String rod = "<b>" + usuario.USUA_NM_NOME + "</b>";

                // Prepara corpo do e-mail e trata link
                String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
                StringBuilder str = new StringBuilder();
                str.AppendLine(corpo);
                if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
                {
                    if (!vm.MENS_NM_LINK.Contains("www."))
                    {
                        vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                    }
                    if (!vm.MENS_NM_LINK.Contains("http://"))
                    {
                        vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                    }
                    str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui acessar a reunião</a>");
                }
                String body = str.ToString();
                body = body.Replace("\r\n", "<br />");
                String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;
                String status = "Succeeded";
                String iD = "xyz";
                String erro = null;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Envio de Link para Reunião";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = cont.USUA_NM_EMAIL;
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
                    await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
                    Session["MensAgenda"] = 10;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Cliente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }

                return 0;
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
                return 0;
            }
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaAgenda"] == 23)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
            }
            if ((Int32)Session["VoltaMensagem"] == 50)
            {
                return RedirectToAction("VoltarAcompanhamentoCRMBase", "CRM");
            }
            if ((Int32)Session["VoltaMensagem"] == 51)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
            }
            if ((Int32)Session["VoltaMensagem"] == 99)
            {
                return RedirectToAction("MontarTelaAgenda", "Agenda");
            }

            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult ExcluirAnexoAgenda(Int32 id)
        {
            try
            {
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                AGENDA_ANEXO item = baseApp.GetAnexoById(id);
                item.AGAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnexo(item);
                Session["AgendaAlterada"] = 1;

                return RedirectToAction("VoltarAnexoAgenda");
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

        public List<CATEGORIA_AGENDA> CarregaCatAgenda()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CATEGORIA_AGENDA> conf = new List<CATEGORIA_AGENDA>();
                if (Session["CatAgendas"] == null)
                {
                    conf = baseApp.GetAllTipos(idAss);
                }
                else
                {
                    if ((Int32)Session["CatAgendaAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTipos(idAss);
                    }
                    else
                    {
                        conf = (List<CATEGORIA_AGENDA>)Session["CatAgendas"];
                    }
                }
                Session["CatAgendas"] = conf;
                Session["CatAgendaAlterada"] = 0;
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
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
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
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public async Task<ActionResult> EnviarLinkReuniaoNovo(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Recupera agenda
                AGENDA age = baseApp.GetItemById(id);
                USUARIO usuario = usuApp.GetItemById(age.AGEN_CD_USUARIO.Value);

                // Recupera modelo
                TEMPLATE temp = temApp.GetByCode("Reuniao");
                String cabecalho = temp.TEMP_TX_CABECALHO;
                String corpo = temp.TEMP_TX_CORPO;
                String rodape = temp.TEMP_TX_DADOS;

                // Monta texto
                cabecalho = cabecalho.Replace("{Nome}", age.USUARIO1.USUA_NM_NOME);
                corpo = corpo.Replace("{Data}", age.AGEN_DT_DATA.ToShortDateString());
                corpo = corpo.Replace("{Hora}", age.AGEN_HR_HORA.ToString());
                corpo = corpo.Replace("{Assunto}", age.AGEN_NM_TITULO);
                if (rodape.Contains("Assinatura"))
                {
                    rodape = rodape.Replace("{Assinatura}", usuario.ASSINANTE.ASSI_NM_NOME);
                }
                String body = cabecalho + "<br />" + corpo + "<br />" + rodape;

                // Ajusta texto
                body = body.Replace("\r\n", "<br />");
                body = body.Replace("<p>", "");
                body = body.Replace("</p>", "<br />");

                // Monta mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = usuario.USUA_NM_NOME;
                mens.ID = usuario.USUA_CD_ID;
                mens.MODELO = usuario.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_TX_TEXTO = body;
                mens.MENS_NM_LINK = age.AGEN_LK_REUNIAO;
                mens.MENS_NM_NOME = "Link para Reunião";
                mens.MENS_NM_CAMPANHA = age.USUARIO1.USUA_NM_EMAIL;
                mens.MENS_IN_USUARIO = age.USUARIO1.USUA_CD_ID;

                // Executa a operação
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = await envio.ProcessaEnvioEMailGeralReuniao(mens, usuario);

                // Retorno
                Session["MensAgenda"] = 11;
                return RedirectToAction("MontarTelaAgenda");
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

        public ActionResult EnviarLinkReuniaoGrid(Int32 id)
        {
            Session["IdAgenda"] = id;
            Session["EditaLink"] = 1;
            return RedirectToAction("EnviarLinkReuniaoFormNovo", "Agenda");
        }

        public ActionResult EnviarLinkReuniaoUnico(Int32 id)
        {
            Session["EditaLink"] = 1;
            Session["EditaUnico"] = 1;
            Session["UnicoLink"] = id;
            return RedirectToAction("EnviarLinkReuniaoFormNovo", "Agenda");
        }

        public ActionResult EnviarLinkReuniaoFormNovo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera agenda
                AGENDA age = baseApp.GetItemById((Int32)Session["IdAgenda"]);
                USUARIO usuario = usuApp.GetItemById(age.USUA_CD_ID);
                String link = age.AGEN_LK_REUNIAO;
                Int32 total = 0;

                // Recupera modelo
                TEMPLATE temp = temApp.GetByCode("Reuniao");
                String cabecalho = temp.TEMP_TX_CABECALHO;
                String corpo = temp.TEMP_TX_CORPO;
                String rodape = temp.TEMP_TX_DADOS;

                // Monta texto
                corpo = corpo.Replace("{Data}", age.AGEN_DT_DATA.ToShortDateString());
                corpo = corpo.Replace("{Hora}", age.AGEN_HR_HORA.ToString());
                corpo = corpo.Replace("{Assunto}", age.AGEN_NM_TITULO);
                if (rodape.Contains("Assinatura"))
                {
                    rodape = rodape.Replace("{Assinatura}", usuario.ASSINANTE.ASSI_NM_NOME);
                }

                // Percorre contatos
                foreach (AGENDA_CONTATO item in age.AGENDA_CONTATO)
                {
                    if (item.AGCO_IN_ENVIO == 0 || (Int32)Session["EditaLink"] == 1)
                    {
                        // Verfica se é envio único
                        if ((Int32)Session["EditaUnico"] == 1)
                        {
                            if ((Int32)Session["UnicoLink"] != item.AGCO_CD_ID)
                            {
                                continue;
                            }
                        }

                        // Monta corpo
                        Int32 id = item.AGCO_CD_ID;
                        if (item.AGCO_IN_TIPO == 1)
                        {
                            // Contato interno
                            cabecalho = cabecalho.Replace("{Nome}", item.USUARIO.USUA_NM_NOME);
                        }
                        else
                        {
                            // Contato Externo
                            cabecalho = cabecalho.Replace("{Nome}", item.AGCO_NM_NOME);
                        }
                        String body = cabecalho + "<br />" + corpo + "<br />" + rodape;

                        // Ajusta texto
                        body = body.Replace("\r\n", "<br />");
                        body = body.Replace("<p>", "");
                        body = body.Replace("</p>", "<br />");

                        // Prepara  link
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(body);
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
                            body += "  " + link;
                        }
                        body = str.ToString();
                        String emailBody = body;
                        Session["BodyEMail"] = body;

                        // Monta registro de recursividade
                        RECURSIVIDADE rec = new RECURSIVIDADE();
                        rec.ASSI_CD_ID = idAss;
                        rec.MENS_CD_ID = null;
                        rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                        rec.RECU_IN_TIPO_MENSAGEM = 1;
                        rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                        rec.RECU_IN_TIPO_SMS = 0;
                        if (item.AGCO_IN_TIPO == 1)
                        {
                            // Contato interno
                            rec.RECU_NM_NOME = item.USUARIO.USUA_NM_NOME;
                        }
                        else
                        {
                            // Contato Externo
                            rec.RECU_NM_NOME = item.AGCO_NM_NOME;
                        }
                        rec.RECU_LK_LINK = link;
                        rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                        rec.RECU_IN_ATIVO = 1;

                        // Monta destinos
                        RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                        dest.CLIE_CD_ID = 2;
                        if (item.AGCO_IN_TIPO == 1)
                        {
                            // Contato interno
                            dest.REDE_EM_EMAIL = item.USUARIO.USUA_NM_EMAIL;
                            dest.REDE_NM_NOME = item.USUARIO.USUA_NM_NOME;
                        }
                        else
                        {
                            // Contato Externo
                            dest.REDE_EM_EMAIL = item.AGCO_NM_EMAIL;
                            dest.REDE_NM_NOME = item.AGCO_NM_NOME;
                        }
                        dest.REDE_TX_CORPO = emailBody;
                        dest.REDE_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DESTINO.Add(dest);

                        // Monta Datas
                        RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                        data1.REDA_DT_PROGRAMADA = DateTime.Today.Date;
                        data1.REDA_IN_PROCESSADA = 0;
                        data1.REDA_IN_ATIVO = 1;
                        rec.RECURSIVIDADE_DATA.Add(data1);

                        // Grava recursividade
                        total++;
                        Int32 voltaRec = recApp.ValidateCreate(rec, usuario);

                        // Atualiza contato
                        AGENDA_CONTATO cont = baseApp.GetContatoById(id);
                        cont.AGCO_IN_ENVIO = 1;
                        Int32 x = baseApp.ValidateEditContato(cont);
                    }
                }

                // Retorno
                if (total > 0)
                {
                    Session["TotalEnvios"] = total;
                    Session["MensAgenda"] = 11;
                }
                Session["EnviaLink"] = 0;
                Session["EditaLink"] = 0;
                if ((Int32)Session["VoltaAgenda"] == 51)
                {
                    Session["MensAgenda"] = 90;
                    Session["VoltaAgenda"] = 0;
                    return RedirectToAction("VoltarAnexoAgenda");
                }
                return RedirectToAction("MontarTelaAgenda");
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

        public async Task<ActionResult> EnviarLinkReuniaoFormNovoLista()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera agenda
                AGENDA age = baseApp.GetItemById((Int32)Session["IdAgenda"]);
                USUARIO usuario = usuApp.GetItemById(age.USUA_CD_ID);
                String link = age.AGEN_LK_REUNIAO;
                Int32 total = 0;
                String emailBody = String.Empty;
                String status = String.Empty;
                String iD = String.Empty;
                String erro = String.Empty;
                List<EmailAddress> toRecipients = new List<EmailAddress>();

                //Carrega configuracao
                CONFIGURACAO conf = CarregaConfiguracaoGeral();

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

                // Recupera modelo
                TEMPLATE temp = temApp.GetByCode("Reuniao");
                String cabecalho = temp.TEMP_TX_CABECALHO;
                String corpo = temp.TEMP_TX_CORPO;
                String rodape = temp.TEMP_TX_DADOS;

                // Monta texto
                corpo = corpo.Replace("{Data}", age.AGEN_DT_DATA.ToShortDateString());
                corpo = corpo.Replace("{Hora}", age.AGEN_HR_HORA.ToString());
                corpo = corpo.Replace("{Assunto}", age.AGEN_NM_TITULO);
                if (rodape.Contains("Assinatura"))
                {
                    rodape = rodape.Replace("{Assinatura}", usuario.ASSINANTE.ASSI_NM_NOME);
                }

                // Percorre contatos
                foreach (AGENDA_CONTATO item in age.AGENDA_CONTATO)
                {
                    if (item.AGCO_IN_ENVIO == 0 || (Int32)Session["EditaLink"] == 1 || (Int32)Session["IncluiuContato"] == 1)
                    {
                        // Verfica se é envio único
                        if ((Int32)Session["EditaUnico"] == 1)
                        {
                            if ((Int32)Session["UnicoLink"] != item.AGCO_CD_ID)
                            {
                                continue;
                            }
                        }

                        // Monta corpo
                        cabecalho = temp.TEMP_TX_CABECALHO;
                        Int32 id = item.AGCO_CD_ID;
                        if (item.AGCO_IN_TIPO == 1)
                        {
                            // Contato interno
                            cabecalho = cabecalho.Replace("{Nome}", item.USUARIO.USUA_NM_NOME);
                        }
                        else
                        {
                            // Contato Externo
                            cabecalho = cabecalho.Replace("{Nome}", item.AGCO_NM_NOME);
                        }
                        cabecalho = "Prezado Sr(a)<br />";
                        String body = cabecalho + "<br />" + corpo + "<br />" + rodape;

                        // Ajusta texto
                        body = body.Replace("\r\n", "<br />");
                        body = body.Replace("<p>", "");
                        body = body.Replace("</p>", "<br />");

                        // Prepara  link
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(body);
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
                            body += "  " + link;
                        }
                        body = str.ToString();
                        emailBody = body;
                        Session["BodyEMail"] = body;

                        // Inclui na lista de envio
                        String mail = String.Empty;
                        String nome = String.Empty;
                        if (item.AGCO_IN_TIPO == 1)
                        {
                            // Contato interno
                            mail = item.USUARIO.USUA_NM_EMAIL;
                            nome = item.USUARIO.USUA_NM_NOME;
                        }
                        else
                        {
                            // Contato Externo
                            mail = item.AGCO_NM_EMAIL;
                            nome = item.AGCO_NM_NOME;
                        }
                        EmailAddress adress = new EmailAddress(address: mail, displayName: nome);
                        toRecipients.Add(adress);

                        // Atualiza contato
                        total++;
                        AGENDA_CONTATO cont = baseApp.GetContatoById(id);
                        cont.AGCO_IN_ENVIO = 1;
                        Int32 x = baseApp.ValidateEditContato(cont);
                    }
                }

                // Monta envio
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Envio de Link para Reunião";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = null;
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

                // Envia mensagens
                try
                {
                    await CrossCutting.CommunicationAzurePackage.SendMailListAsync(mensagem, null, toRecipients);
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Agenda";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }

                // Grava envios
                foreach (AGENDA_CONTATO item in age.AGENDA_CONTATO)
                {
                    MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                    env.ASSI_CD_ID = idAss;
                    env.CLIE_CD_ID = null;
                    env.MEEN_IN_USUARIO = usuario.USUA_CD_ID;
                    env.MEEN_IN_TIPO = 1;
                    env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                    if (item.AGCO_IN_TIPO == 1)
                    {
                        // Contato interno
                        env.MEEN_EM_EMAIL_DESTINO = item.USUARIO.USUA_NM_EMAIL;
                        env.MEEN_NM_ORIGEM = item.USUARIO.USUA_NM_NOME;
                    }
                    else
                    {
                        // Contato Externo
                        env.MEEN_EM_EMAIL_DESTINO = item.AGCO_NM_EMAIL;
                        env.MEEN_NM_ORIGEM = item.AGCO_NM_NOME;
                    }
                    env.MEEN_TX_CORPO = emailBody;
                    env.MEEN_IN_ANEXOS = 0;
                    env.MEEN_IN_ATIVO = 1;
                    env.MEEN_IN_ESCOPO = 1;
                    env.MEEN_TX_CORPO_COMPLETO = emailBody;
                    env.USUA_CD_ID = usuario.USUA_CD_ID;
                    if (erro == String.Empty)
                    {
                        env.MEEN_IN_ENTREGUE = 1;
                    }
                    else
                    {
                        env.MEEN_IN_ENTREGUE = 0;
                        env.MEEN_TX_RETORNO = erro;
                    }
                    env.MEEN_SG_STATUS = status;
                    env.MEEN_GU_ID_MENSAGEM = iD;
                    env.EMPR_CD_ID = usuario.EMPR_CD_ID;

                    Int32 voltaMens = meApp.ValidateCreate(env);
                }

                // Retorno
                if (total > 0)
                {
                    Session["TotalEnvios"] = total;
                    Session["MensAgenda"] = 11;
                }
                Session["EnviaLink"] = 0;
                Session["EditaLink"] = 0;
                if ((Int32)Session["VoltaAgenda"] == 51)
                {
                    Session["MensAgenda"] = 90;
                    Session["VoltaAgenda"] = 0;
                    return RedirectToAction("VoltarAnexoAgenda");
                }
                return RedirectToAction("MontarTelaAgenda");
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

        [HttpGet]
        public ActionResult IncluirContatoAgenda()
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Listas
                ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

                // Prepara view
                AGENDA_CONTATO item = new AGENDA_CONTATO();
                AgendaContatoViewModel vm = Mapper.Map<AGENDA_CONTATO, AgendaContatoViewModel>(item);
                vm.AGEN_CD_ID = (Int32)Session["IdVolta"];
                vm.AGCO_IN_ATIVO = 1;
                vm.AGCO_IN_ENVIO = 0;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Agenda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agendaa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
            // Prepara view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContatoAgenda(AgendaContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.AGCO_DS_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGCO_DS_ANOTACAO);
                    vm.AGCO_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.AGCO_NM_EMAIL);
                    vm.AGCO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGCO_NM_NOME);

                    // Executa a operação
                    AGENDA_CONTATO item = Mapper.Map<AgendaContatoViewModel, AGENDA_CONTATO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    Session["AbaAgenda"] = 2;
                    Session["AgendaAlterada"] = 1;
                    AGENDA age = baseApp.GetItemById(item.AGEN_CD_ID);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "icoAGEN",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA_CONTATO>(item),
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["VoltaAgenda"] = 50;
                    Session["IncluiuContato"] = 1;
                    Session["NaoFezNada"] = 1;
                    return RedirectToAction("VoltarAnexoAgenda");
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
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarContatoAgenda(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Listas
                ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

                // Prepara view
                AGENDA_CONTATO item = baseApp.GetContatoById(id);
                AgendaContatoViewModel vm = Mapper.Map<AGENDA_CONTATO, AgendaContatoViewModel>(item);
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContatoAgenda(AgendaContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.AGCO_DS_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGCO_DS_ANOTACAO);
                    vm.AGCO_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.AGCO_NM_EMAIL);
                    vm.AGCO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.AGCO_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    AGENDA_CONTATO item = Mapper.Map<AgendaContatoViewModel, AGENDA_CONTATO>(vm);
                    item.AGCO_IN_ENVIO = 0;
                    Int32 volta = baseApp.ValidateEditContato(item);

                    // Verifica retorno
                    Session["AbaAgenda"] = 2;
                    Session["AgendaAlterada"] = 1;

                    AGENDA age = baseApp.GetItemById(item.AGEN_CD_ID);
                    if (age.AGEN_LK_REUNIAO != null)
                    {
                        Session["EditarLink"] = 1;
                        Session["VoltaAgenda"] = 50;
                    }
                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ecoAGEN",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA_CONTATO>(item),
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    return RedirectToAction("VoltarAnexoAgenda");
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
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirContatoAgenda(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                AGENDA_CONTATO item = baseApp.GetContatoById(id);
                item.AGCO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditContato(item);
                Session["AbaAgenda"] = 2;
                Session["AgendaAlterada"] = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xcoAGEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA_CONTATO>(item),
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);
                return RedirectToAction("VoltarAnexoAgenda");
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

        [HttpGet]
        public ActionResult ReativarContatoAgenda(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                AGENDA_CONTATO item = baseApp.GetContatoById(id);
                item.AGCO_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditContato(item);
                Session["AbaAgenda"] = 2;
                Session["AgendaAlterada"] = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "rcoAGEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA_CONTATO>(item),
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);
                return RedirectToAction("VoltarAnexoAgenda");
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