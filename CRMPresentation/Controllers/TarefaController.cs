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
    public class TarefaController : Controller
    {
        private readonly ITarefaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAgendaAppService agenApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        TAREFA objeto = new TAREFA();
        TAREFA objetoAntes = new TAREFA();
        List<TAREFA> listaMaster = new List<TAREFA>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public TarefaController(ITarefaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IAgendaAppService agenApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            agenApp = agenApps;
            confApp = confApps;
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

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        [HttpPost]
        public JsonResult GetTarefaNaoExecutada()
        {
            var usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregaTarefa(usu.USUA_CD_ID).Where(x => x.TARE_DT_REALIZADA == null & x.TARE_IN_ATIVO == 1).ToList();
            if (listaMaster.Count == 1)
            {
                var hash = new Hashtable();
                hash.Add("msg", "Você possui 1 tarefa não executada");

                return Json(hash);
            } 
            else if (listaMaster.Count > 1) 
            {
                var hash = new Hashtable();
                hash.Add("msg", "Você possui " + listaMaster.Count + " tarefas não executadas");

                return Json(hash);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult TarefaNaoRealizadaClick()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            var usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            listaMaster = CarregaTarefa(usu.USUA_CD_ID).Where(x => x.TARE_DT_REALIZADA == null).ToList();

            if (listaMaster.Count == 1)
            {
                return Json(listaMaster.FirstOrDefault().TARE_CD_ID);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTarefaKanban(Int32? id)
        {
            Session["VoltaKanban"] = 1;

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

            // Carrega listas
            if (Session["ListaTarefa"] == null)
            {
                listaMaster = CarregaTarefa(usuario.USUA_CD_ID);
                Session["ListaTarefa"] = listaMaster;
            }

            if (id == null)
            {
                ViewBag.Listas = (List<TAREFA>)Session["ListaTarefa"];
            }
            else
            {
                ViewBag.Listas = baseApp.GetByUser(usuario.USUA_CD_ID).Where(x => x.TARE_DT_REALIZADA == null).ToList();
            }
            ViewBag.Title = "Tarefas";

            // Listas
            ViewBag.Tarefas = ((List<TAREFA>)Session["ListaTarefa"]).Count;
            ViewBag.Tipos = new SelectList(CarregaTipoTarefa(), "TITR_CD_ID", "TITR_NM_NOME");
            ViewBag.TarefasPendentes = baseApp.GetTarefaStatus(usuario.USUA_CD_ID, 1).Count;
            ViewBag.TarefasEncerradas = baseApp.GetTarefaStatus(usuario.USUA_CD_ID, 2).Count;

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Pendente", Value = "1" });
            status.Add(new SelectListItem() { Text = "Suspensa", Value = "2" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "4" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            // Mensagem
            if ((Int32)Session["MensTarefa"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensTarefa"] = 0;
            objeto = new TAREFA();
            objeto.TARE_DT_CADASTRO = DateTime.Today.Date;
            return View(objeto);
        }

        [HttpPost]
        public JsonResult GetTarefas()
        {
            var usuario = (USUARIO)Session["UserCredentials"];
            listaMaster = baseApp.GetByUser(usuario.USUA_CD_ID);
            var listaHash = new List<Hashtable>();
            foreach (var item in listaMaster)
            {
                var hash = new Hashtable();
                hash.Add("TARE_IN_STATUS", item.TARE_IN_STATUS);
                hash.Add("TARE_CD_ID", item.TARE_CD_ID);
                hash.Add("TARE_NM_TITULO", item.TARE_NM_TITULO);
                hash.Add("TARE_DT_CADASTRO", item.TARE_DT_CADASTRO.ToString("dd/MM/yyyy"));
                hash.Add("TARE_DT_ESTIMADA", item.TARE_DT_ESTIMADA.Value.ToString("dd/MM/yyyy"));

                listaHash.Add(hash);
            }
            return Json(listaHash);
        }

        [HttpGet]
        public ActionResult MontarTelaTarefa(Int32? id)
        {
            Session["VoltaKanban"] = 0;

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

            // Carrega listas
            if (Session["ListaTarefa"] == null)
            {
                listaMaster = CarregaTarefa(usuario.USUA_CD_ID);
                Session["ListaTarefa"] = listaMaster;
            }

            if (id == null)
            {
                ViewBag.Listas = ((List<TAREFA>)Session["ListaTarefa"]).OrderByDescending(x => x.TARE_DT_CADASTRO).ToList<TAREFA>();
            }
            else
            {
                ViewBag.Listas = CarregaTarefa(usuario.USUA_CD_ID).Where(x => x.TARE_DT_REALIZADA == null).OrderByDescending(x => x.TARE_DT_CADASTRO).ToList<TAREFA>();
            }

            ViewBag.Title = "Tarefas";

            // Indicadores
            ViewBag.Tarefas = ((List<TAREFA>)Session["ListaTarefa"]).Count;
            ViewBag.Tipos = new SelectList(CarregaTipoTarefa(), "TITR_CD_ID", "TITR_NM_NOME");
            ViewBag.TarefasPendentes = baseApp.GetTarefaStatus(usuario.USUA_CD_ID, 1).Count;
            ViewBag.TarefasEncerradas = baseApp.GetTarefaStatus(usuario.USUA_CD_ID, 2).Count;
            ViewBag.TarefasSuspensas = baseApp.GetTarefaStatus(usuario.USUA_CD_ID, 3).Count;
            ViewBag.TarefasCanceladas= baseApp.GetTarefaStatus(usuario.USUA_CD_ID, 4).Count;

            List<USUARIO> listaTotal = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(listaTotal, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Pendente", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Andamento", Value = "2" });
            status.Add(new SelectListItem() { Text = "Suspensa", Value = "3" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            // Mensagem
            if ((Int32)Session["MensTarefa"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensTarefa"] = 0;
            Session["VoltaTarefa"] = 1;
            objeto = new TAREFA();
            objeto.TARE_DT_CADASTRO = DateTime.Today.Date;
            objeto.TARE_DT_ESTIMADA = DateTime.Today.Date;
            return View(objeto);
        }

        public ActionResult RetirarFiltroTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTarefa"] = null;
            return RedirectToAction("MontarTelaTarefa");
        }

        public ActionResult MostrarTudoTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            listaMaster = baseApp.GetAllItensAdm(usuario.USUA_CD_ID);
            Session["ListaTarefa"] = listaMaster;
            return RedirectToAction("MontarTelaTarefa");
        }

        [HttpPost]
        public ActionResult FiltrarTarefa(TAREFA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                USUARIO user = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                if (user.PERF_CD_ID != 1)
                {
                    item.USUA_CD_ID = user.USUA_CD_ID;
                }

                // Executa a operação
                List<TAREFA> listaObj = new List<TAREFA>();
                Tuple<Int32, List<TAREFA>, Boolean> volta = baseApp.ExecuteFilter(item.TITR_CD_ID, item.TARE_NM_TITULO, item.TARE_DT_CADASTRO, item.TARE_DT_ESTIMADA, item.TARE_IN_STATUS, item.TARE_IN_PRIORIDADE, item.USUA_CD_ID, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensTarefa"] = 1;
                    return RedirectToAction("MontarTelaTarefa");
                }

                // Sucesso
                Session["MensTarefa"] = 0;
                listaMaster = volta.Item2;
                Session["ListaTarefa"] = volta.Item2;
                return RedirectToAction("MontarTelaTarefa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tarefas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult VoltarBaseTarefa()
        {
            if (Session["UserCredentials"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaTarefa"] == 10)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["VoltaKanban"] == 1)
            {
                Session["VoltaKanban"] = 0;
                return RedirectToAction("MontarTelaTarefaKanban");
            }
            return RedirectToAction("MontarTelaTarefa");
        }

        public ActionResult VoltarKanbanTarefa()
        {
            if (Session["UserCredentials"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTarefa"] = null;
            Session["TarefaAlterada"] = 1;
            return RedirectToAction("MontarTelaTarefa");
        }

        [HttpGet]
        public ActionResult IncluirTarefa()
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

            // Prepara listas
            ViewBag.Tipos = new SelectList(CarregaTipoTarefa(), "TITR_CD_ID", "TITR_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(listaTotal, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(baseApp.GetAllPeriodicidade(), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Pendente", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Andamento", Value = "2" });
            status.Add(new SelectListItem() { Text = "Suspensa", Value = "3" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");

            // Prepara view
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            TAREFA item = new TAREFA();
            TarefaViewModel vm = Mapper.Map<TAREFA, TarefaViewModel>(item);
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.TARE_IN_ATIVO = 1;
            vm.TARE_DT_CADASTRO = DateTime.Today.Date;
            vm.TARE_DT_ESTIMADA = DateTime.Today.Date.AddDays(5);
            vm.TARE_IN_PRIORIDADE = 1;
            vm.TARE_IN_STATUS = 1;
            vm.TARE_IN_AVISA = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirTarefa(TarefaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaAgenda"] = null;
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Tipos = new SelectList(CarregaTipoTarefa(), "TITR_CD_ID", "TITR_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(listaTotal, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(baseApp.GetAllPeriodicidade(), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Pendente", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Andamento", Value = "2" });
            status.Add(new SelectListItem() { Text = "Suspensa", Value = "3" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Preparação
                    TAREFA item = Mapper.Map<TarefaViewModel, TAREFA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                    // Verificação
                    Int32 recursividade = 0;
                    if (item.PETA_CD_ID != null)
                    {
                        if (item.TARE_NR_PERIODICIDADE_QUANTIDADE == null || item.TARE_NR_PERIODICIDADE_QUANTIDADE == 0)
                        {
                            item.TARE_NR_PERIODICIDADE_QUANTIDADE = 1;
                        }
                        recursividade = 1;
                    }

                    // Processa inclusão
                    if (recursividade == 1)
                    {
                        DateTime dtAgenda = (DateTime)item.TARE_DT_ESTIMADA;
                        DateTime dtTarefa = (DateTime)item.TARE_DT_CADASTRO;
                        TAREFA tarefa = new TAREFA();
                        Int32 tare = 0;
                        Int32 pai = 0;
                        Int32? repete = item.TARE_NR_PERIODICIDADE_QUANTIDADE + 1;

                        for (var i = 1; i <= repete; i++)
                        {
                            if (i == 1)
                            {
                                item.TARE_NM_ORDEM = "1/" + repete.ToString();
                                Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);
                                Session["PeriTarefa"] = item.PERIODICIDADE_TAREFA;
                                tare = item.TARE_CD_ID;
                                pai = item.TARE_CD_ID;

                                // Verifica retorno
                                if (volta == 1)
                                {
                                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0058", CultureInfo.CurrentCulture));
                                    return View(vm);
                                }
                                if (volta == 2)
                                {
                                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0094", CultureInfo.CurrentCulture));
                                    return View(vm);
                                }
                            }
                            else
                            {
                                tarefa = new TAREFA();
                                tarefa.USUA_CD_ID = item.USUA_CD_ID;
                                tarefa.TITR_CD_ID = item.TITR_CD_ID;
                                tarefa.TARE_DS_DESCRICAO = item.TARE_DS_DESCRICAO;
                                tarefa.TARE_IN_STATUS = item.TARE_IN_STATUS;
                                tarefa.TARE_IN_PRIORIDADE = item.TARE_IN_PRIORIDADE;
                                tarefa.TARE_IN_ATIVO = item.TARE_IN_ATIVO;
                                tarefa.TARE_DT_REALIZADA = item.TARE_DT_REALIZADA;
                                tarefa.TARE_TX_OBSERVACOES = item.TARE_TX_OBSERVACOES;
                                tarefa.TARE_NM_LOCAL = item.TARE_NM_LOCAL;
                                tarefa.TARE_IN_AVISA = item.TARE_IN_AVISA;
                                tarefa.ASSI_CD_ID = item.ASSI_CD_ID;
                                tarefa.PETA_CD_ID = item.PETA_CD_ID;
                                tarefa.TARE_NR_PERIODICIDADE_QUANTIDADE = item.TARE_NR_PERIODICIDADE_QUANTIDADE;
                                tarefa.TARE_DT_CADASTRO = dtTarefa;
                                tarefa.TARE_DT_ESTIMADA = dtAgenda;
                                tarefa.TARE_IN_TAREFA_PAI = pai;
                                tarefa.TARE_NM_TITULO = $"{item.TARE_NM_TITULO} #{i}";
                                tarefa.TARE_NM_ORDEM = i.ToString() + "/" + repete.ToString();

                                Int32 volta = baseApp.ValidateCreate(tarefa, usuarioLogado);
                                tare = tarefa.TARE_CD_ID;
                            }

                            AGENDA ag = new AGENDA();
                            ag.USUA_CD_ID = item.USUA_CD_ID;
                            ag.AGEN_CD_USUARIO = item.USUA_CD_ID;
                            ag.AGEN_DT_DATA = dtAgenda;
                            ag.AGEN_HR_HORA = dtAgenda.AddHours(12).TimeOfDay;
                            ag.AGEN_IN_ATIVO = 1;
                            ag.AGEN_IN_STATUS = 1;
                            ag.AGEN_NM_TITULO = item.TARE_NM_TITULO + (i == 0 ? String.Empty : "#" + i.ToString());
                            ag.AGEN_DS_DESCRICAO = item.TARE_NM_TITULO;
                            ag.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                            ag.TARE_CD_ID = tare;
                            ag.CAAG_CD_ID = 1;
                            Int32 voltaAg = agenApp.ValidateCreate(ag, usuarioLogado);

                            // Cria pastas Tarefa
                            String caminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Tarefa/" + item.TARE_CD_ID.ToString() + "/Anexos/";
                            Directory.CreateDirectory(Server.MapPath(caminho));

                            // Cria pastas Agenda
                            String agCaminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Agenda/" + ag.AGEN_CD_ID.ToString() + "/Anexos/";
                            Directory.CreateDirectory(Server.MapPath(agCaminho));

                            Int32? dias = ((PERIODICIDADE_TAREFA)Session["PeriTarefa"]).PETA_NR_DIAS;
                            Double dia = Convert.ToDouble(dias);

                            if (((PERIODICIDADE_TAREFA)Session["PeriTarefa"]).PETA_CD_ID == 4) //Mensal
                            {
                                dtAgenda = dtAgenda.AddMonths(1);
                                dtTarefa = dtTarefa.AddMonths(1);
                            }
                            else if (((PERIODICIDADE_TAREFA)Session["PeriTarefa"]).PETA_CD_ID == 5) //Semestral
                            {
                                dtAgenda = dtAgenda.AddMonths(6);
                                dtTarefa = dtTarefa.AddMonths(6);
                            }
                            else if (((PERIODICIDADE_TAREFA)Session["PeriTarefa"]).PETA_CD_ID == 6) //Anual
                            {
                                dtAgenda = dtAgenda.AddYears(1);
                                dtTarefa = dtTarefa.AddYears(1);
                            }
                            else // Outras
                            {
                                dtAgenda = dtAgenda.AddDays(dia);
                                dtTarefa = dtTarefa.AddDays(dia);
                            }
                        }
                    }
                    else
                    {
                        Int32 volta1 = baseApp.ValidateCreate(item, usuarioLogado);

                        AGENDA ag = new AGENDA();
                        ag.USUA_CD_ID = item.USUA_CD_ID;
                        ag.AGEN_CD_USUARIO = item.USUA_CD_ID;
                        ag.AGEN_DT_DATA = item.TARE_DT_ESTIMADA.Value;
                        ag.AGEN_HR_HORA = item.TARE_DT_ESTIMADA.Value.AddHours(12).TimeOfDay;
                        ag.AGEN_IN_ATIVO = 1;
                        ag.AGEN_IN_STATUS = 1;
                        ag.AGEN_NM_TITULO = item.TARE_NM_TITULO;
                        ag.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                        ag.AGEN_DS_DESCRICAO = item.TARE_NM_TITULO;
                        ag.TARE_CD_ID = item.TARE_CD_ID;
                        ag.CAAG_CD_ID = 1;


                        // Verifica retorno
                        if (volta1 == 1)
                        {
                            ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0058", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        Int32 voltaAg = agenApp.ValidateCreate(ag, usuarioLogado);

                        // Cria pastas Tarefa
                        String caminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Tarefa/" + item.TARE_CD_ID.ToString() + "/Anexos/";
                        Directory.CreateDirectory(Server.MapPath(caminho));

                        // Cria pastas Agenda
                        String agCaminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Agenda/" + ag.AGEN_CD_ID.ToString() + "/Anexos/";
                        Directory.CreateDirectory(Server.MapPath(agCaminho));
                    }

                    // Sucesso
                    listaMaster = new List<TAREFA>();
                    Session["ListaTarefa"] = null;
                    Session["IdVolta"] = item.TARE_CD_ID;
                    Session["PeriTarefa"] = null;
                    Session["TarefaAlterada"] = 1;

                    if (Session["FileQueueTarefa"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueTarefa"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueTarefa(file);
                        }

                        Session["FileQueueTarefa"] = null;
                    }

                    if ((Int32)Session["VoltaKanban"] == 1)
                    {
                        return RedirectToAction("MontarTelaTarefaKanban");
                    }

                    return RedirectToAction("MontarTelaTarefa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tarefas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTarefa(Int32 id)
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

            // Prepara view
            ViewBag.Tipos = new SelectList(CarregaTipoTarefa(), "TITR_CD_ID", "TITR_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(listaTotal, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(baseApp.GetAllPeriodicidade(), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Pendente", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Andamento", Value = "2" });
            status.Add(new SelectListItem() { Text = "Suspensa", Value = "3" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            ViewBag.StatusX = new SelectList(status, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");

            // Mensagens
            if ((Int32)Session["MensTarefa"] == 10)
            {
                ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensTarefa"] == 11)
            {
                ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            TAREFA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Tarefa"] = item;
            Session["IdVolta"] = id;
            Session["StatusAnterior"] = item.TARE_IN_STATUS;
            ViewBag.Status = (item.TARE_IN_STATUS == 1 ? "Pendente" : (item.TARE_IN_STATUS == 2 ? "Em Andamento" : (item.TARE_IN_STATUS == 3 ? "Suspensa" : (item.TARE_IN_STATUS == 4 ? "Cancelada" : "Encerrada"))));
            ViewBag.StatusCor = (item.TARE_IN_STATUS == 1 ? "red-bg" : (item.TARE_IN_STATUS == 2 ? "blue-bg" : (item.TARE_IN_STATUS == 2 ? "blue-bg" : (item.TARE_IN_STATUS == 3 ? "yellow-bg" : "navy-bg"))));
            ViewBag.Prior = (item.TARE_IN_PRIORIDADE == 1 ? "Normal" : (item.TARE_IN_PRIORIDADE == 2 ? "Baixa" : (item.TARE_IN_PRIORIDADE == 3 ? "Alta" : "Urgente")));
            ViewBag.PriorCor = item.TARE_IN_PRIORIDADE == 1 ? "navy-bg" : "red-bg";
            TarefaViewModel vm = Mapper.Map<TAREFA, TarefaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTarefa(TarefaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(CarregaTipoTarefa(), "TITR_CD_ID", "TITR_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(listaTotal, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(baseApp.GetAllPeriodicidade(), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Pendente", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Andamento", Value = "2" });
            status.Add(new SelectListItem() { Text = "Suspensa", Value = "3" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            ViewBag.StatusX = new SelectList(status, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TAREFA item = Mapper.Map<TarefaViewModel, TAREFA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0013", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0014", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Acerta agenda
                    if (item.TARE_IN_STATUS == 1 || item.TARE_IN_STATUS == 2)
                    {
                        if ((Int32)Session["StatusAnterior"] == 3)
                        {
                            List<AGENDA> ags = agenApp.GetAllItens(idAss);
                            ags = ags.Where(p => p.TARE_CD_ID == item.TARE_CD_ID).ToList();
                            foreach (AGENDA ag in ags)
                            {
                                ag.AGEN_IN_STATUS = 1;
                                Int32 volta1 = agenApp.ValidateEdit(ag, ag, usuarioLogado);
                            }
                        }
                    }
                    if (item.TARE_IN_STATUS == 3)
                    {
                        List<AGENDA> ags = agenApp.GetAllItens(idAss);
                        ags = ags.Where(p => p.TARE_CD_ID == item.TARE_CD_ID).ToList();
                        foreach (AGENDA ag in ags)
                        {
                            ag.AGEN_IN_STATUS = 2;
                            Int32 volta1 = agenApp.ValidateEdit(ag, ag, usuarioLogado);
                        }
                    }
                    if (item.TARE_IN_STATUS == 4 || item.TARE_IN_STATUS == 5)
                    {
                        List<AGENDA> ags = agenApp.GetAllItens(idAss);
                        ags = ags.Where(p => p.TARE_CD_ID == item.TARE_CD_ID).ToList();
                        foreach (AGENDA ag in ags)
                        {
                            ag.AGEN_IN_STATUS = 3;
                            Int32 volta1 = agenApp.ValidateEdit(ag, ag, usuarioLogado);
                        }
                    }

                    // Acerta alteração da periodicidade/repetições
                    TAREFA antes = (TAREFA)Session["Tarefa"];
                    Session["PeriTarefa"] = item.PERIODICIDADE_TAREFA;
                    if (antes.PETA_CD_ID != item.PETA_CD_ID || antes.TARE_NR_PERIODICIDADE_QUANTIDADE != item.TARE_NR_PERIODICIDADE_QUANTIDADE)
                    {
                        // Exclui tarefas e agendas derivadas
                        List<TAREFA> tfs = baseApp.GetByUser(usuarioLogado.USUA_CD_ID);
                        List<AGENDA> agendas = agenApp.GetAllItens(idAss);
                        tfs = tfs.Where(p => p.TARE_IN_TAREFA_PAI == item.TARE_CD_ID).ToList();
                        foreach (TAREFA tf in tfs)
                        {
                            tf.TARE_IN_ATIVO = 0;
                            Int32 volta1 = baseApp.ValidateEdit(tf, tf);

                            List<AGENDA> ags = agendas.Where(p => p.TARE_CD_ID == tf.TARE_CD_ID).ToList();
                            foreach (AGENDA ag in ags)
                            {
                                ag.AGEN_IN_ATIVO = 0;
                                volta1 = agenApp.ValidateEdit(ag, ag, usuarioLogado);
                            }
                        }

                        //Recria tarefas e agendas derivadas
                        if (item.TARE_NR_PERIODICIDADE_QUANTIDADE != null || item.TARE_NR_PERIODICIDADE_QUANTIDADE > 0)
                        {
                            Int32? dias = ((PERIODICIDADE_TAREFA)Session["PeriTarefa"]).PETA_NR_DIAS;
                            Double dia = Convert.ToDouble(dias);
                            DateTime dtAgenda = item.TARE_DT_ESTIMADA.Value.AddDays(dia);
                            DateTime dtTarefa = item.TARE_DT_CADASTRO.AddDays(dia);

                            TAREFA tarefa = new TAREFA();
                            Int32 tare = 0;
                            Int32 pai = item.TARE_CD_ID;

                            for (var i = 1; i < item.TARE_NR_PERIODICIDADE_QUANTIDADE; i++)
                            {
                                tarefa = new TAREFA();
                                tarefa.USUA_CD_ID = item.USUA_CD_ID;
                                tarefa.TITR_CD_ID = item.TITR_CD_ID;
                                tarefa.TARE_DS_DESCRICAO = item.TARE_DS_DESCRICAO;
                                tarefa.TARE_IN_STATUS = item.TARE_IN_STATUS;
                                tarefa.TARE_IN_PRIORIDADE = item.TARE_IN_PRIORIDADE;
                                tarefa.TARE_IN_ATIVO = item.TARE_IN_ATIVO;
                                tarefa.TARE_DT_REALIZADA = item.TARE_DT_REALIZADA;
                                tarefa.TARE_TX_OBSERVACOES = item.TARE_TX_OBSERVACOES;
                                tarefa.TARE_NM_LOCAL = item.TARE_NM_LOCAL;
                                tarefa.TARE_IN_AVISA = item.TARE_IN_AVISA;
                                tarefa.ASSI_CD_ID = item.ASSI_CD_ID;
                                tarefa.PETA_CD_ID = item.PETA_CD_ID;
                                tarefa.TARE_NR_PERIODICIDADE_QUANTIDADE = item.TARE_NR_PERIODICIDADE_QUANTIDADE;
                                tarefa.TARE_DT_CADASTRO = dtTarefa;
                                tarefa.TARE_DT_ESTIMADA = dtAgenda;
                                tarefa.TARE_IN_TAREFA_PAI = pai;
                                tarefa.TARE_NM_TITULO = $"{item.TARE_NM_TITULO} #{i}";

                                Int32 volta6 = baseApp.ValidateCreate(tarefa, usuarioLogado);
                                tare = tarefa.TARE_CD_ID;

                                AGENDA ag = new AGENDA();
                                ag.USUA_CD_ID = item.USUA_CD_ID;
                                ag.AGEN_CD_USUARIO = item.USUA_CD_ID;
                                ag.AGEN_DT_DATA = dtAgenda;
                                ag.AGEN_HR_HORA = dtAgenda.AddHours(12).TimeOfDay;
                                ag.AGEN_IN_ATIVO = 1;
                                ag.AGEN_IN_STATUS = 1;
                                ag.AGEN_NM_TITULO = item.TARE_NM_TITULO + (i == 0 ? String.Empty : "#" + i.ToString());
                                ag.AGEN_DS_DESCRICAO = item.TARE_NM_TITULO;
                                ag.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                                ag.TARE_CD_ID = tare;
                                ag.CAAG_CD_ID = 1;
                                Int32 voltaAg = agenApp.ValidateCreate(ag, usuarioLogado);

                                // Cria pastas Tarefa
                                String caminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Tarefa/" + tare.ToString() + "/Anexos/";
                                Directory.CreateDirectory(Server.MapPath(caminho));

                                // Cria pastas Agenda
                                String agCaminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Agenda/" + ag.AGEN_CD_ID.ToString() + "/Anexos/";
                                Directory.CreateDirectory(Server.MapPath(agCaminho));

                                dias = ((PERIODICIDADE_TAREFA)Session["PeriTarefa"]).PETA_NR_DIAS;
                                dia = Convert.ToDouble(dias);
                                dtAgenda = dtAgenda.AddDays(dia);
                                dtTarefa = dtTarefa.AddDays(dia);
                            }
                        }
                        else
                        {
                            AGENDA ag = new AGENDA();
                            ag.USUA_CD_ID = item.USUA_CD_ID;
                            ag.AGEN_CD_USUARIO = item.USUA_CD_ID;
                            ag.AGEN_DT_DATA = item.TARE_DT_ESTIMADA.Value;
                            ag.AGEN_HR_HORA = item.TARE_DT_ESTIMADA.Value.AddHours(12).TimeOfDay;
                            ag.AGEN_IN_ATIVO = 1;
                            ag.AGEN_IN_STATUS = 1;
                            ag.AGEN_NM_TITULO = item.TARE_NM_TITULO;
                            ag.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                            ag.AGEN_DS_DESCRICAO = item.TARE_NM_TITULO;
                            ag.TARE_CD_ID = item.TARE_CD_ID;
                            ag.CAAG_CD_ID = 1;

                            Int32 voltaAg = agenApp.ValidateCreate(ag, usuarioLogado);

                            // Cria pastas Agenda
                            String agCaminho = "/Imagens/" + usuarioLogado.ASSI_CD_ID.ToString() + "/Agenda/" + ag.AGEN_CD_ID.ToString() + "/Anexos/";
                            Directory.CreateDirectory(Server.MapPath(agCaminho));
                        }
                    }

                    // Sucesso
                    listaMaster = new List<TAREFA>();
                    Session["TarefaAlterada"] = 1;
                    Session["ListaTarefa"] = null;
                    Session["ListaAgenda"] = null;
                    if ((Int32)Session["VoltaKanban"] == 1)
                    {
                        return RedirectToAction("MontarTelaTarefaKanban");
                    }
                    return RedirectToAction("MontarTelaTarefa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tarefas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public JsonResult EditarStatusTarefa(Int32 id, Int32 status, DateTime? dtEnc)
        {
            var tarefa = baseApp.GetById(id);
            tarefa.TARE_IN_STATUS = status;

            var item = new TAREFA();
            item.TARE_CD_ID = tarefa.TARE_CD_ID;
            item.TARE_DS_DESCRICAO = tarefa.TARE_DS_DESCRICAO;
            item.TARE_DT_CADASTRO = tarefa.TARE_DT_CADASTRO;
            item.TARE_DT_ESTIMADA = tarefa.TARE_DT_ESTIMADA;
            item.TARE_DT_REALIZADA = dtEnc;
            item.TARE_IN_ATIVO = tarefa.TARE_IN_ATIVO;
            item.TARE_IN_AVISA = tarefa.TARE_IN_AVISA;
            item.TARE_IN_PRIORIDADE = tarefa.TARE_IN_PRIORIDADE;
            item.TARE_IN_STATUS = tarefa.TARE_IN_STATUS;
            item.TARE_NM_LOCAL = tarefa.TARE_NM_LOCAL;
            item.TARE_NM_TITULO = tarefa.TARE_NM_TITULO;
            item.TARE_TX_OBSERVACOES = tarefa.TARE_TX_OBSERVACOES;
            item.TITR_CD_ID = tarefa.TITR_CD_ID;
            item.USUA_CD_ID = tarefa.USUA_CD_ID;
            item.ASSI_CD_ID = tarefa.ASSI_CD_ID;

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    return Json(CRMSys_Import.ResourceManager.GetString("M0013", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(CRMSys_Import.ResourceManager.GetString("M0014", CultureInfo.CurrentCulture));
                }

                Session["ListaTarefa"] = null;
                Session["TarefaAlterada"] = 1;
                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tarefas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTarefa(Int32 id)
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

            // Processa
            try
            {
                TAREFA tarefa = baseApp.GetItemById(id);
                TAREFA item = new TAREFA();
                item.TARE_CD_ID = tarefa.TARE_CD_ID;
                item.USUA_CD_ID = tarefa.USUA_CD_ID;
                item.TITR_CD_ID = tarefa.TITR_CD_ID;
                item.TARE_DT_CADASTRO = tarefa.TARE_DT_CADASTRO;
                item.TARE_NM_TITULO = tarefa.TARE_NM_TITULO;
                item.TARE_DS_DESCRICAO = tarefa.TARE_DS_DESCRICAO;
                item.TARE_DT_ESTIMADA = tarefa.TARE_DT_ESTIMADA;
                item.TARE_IN_STATUS = tarefa.TARE_IN_STATUS;
                item.TARE_IN_PRIORIDADE = tarefa.TARE_IN_PRIORIDADE;
                item.TARE_DT_REALIZADA = tarefa.TARE_DT_REALIZADA;
                item.TARE_TX_OBSERVACOES = tarefa.TARE_TX_OBSERVACOES;
                item.TARE_NM_LOCAL = tarefa.TARE_NM_LOCAL;
                item.TARE_IN_AVISA = tarefa.TARE_IN_AVISA;
                item.ASSI_CD_ID = tarefa.ASSI_CD_ID;
                objetoAntes = tarefa;
                item.TARE_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                listaMaster = new List<TAREFA>();
                Session["ListaTarefa"] = null;
                Session["TarefaAlterada"] = 1;
                return RedirectToAction("MontarTelaTarefa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tarefas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTarefa(Int32 id)
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

            // processa
            try
            {
                TAREFA tarefa = baseApp.GetItemById(id);
                TAREFA item = new TAREFA();
                item.TARE_CD_ID = tarefa.TARE_CD_ID;
                item.USUA_CD_ID = tarefa.USUA_CD_ID;
                item.TITR_CD_ID = tarefa.TITR_CD_ID;
                item.TARE_DT_CADASTRO = tarefa.TARE_DT_CADASTRO;
                item.TARE_NM_TITULO = tarefa.TARE_NM_TITULO;
                item.TARE_DS_DESCRICAO = tarefa.TARE_DS_DESCRICAO;
                item.TARE_DT_ESTIMADA = tarefa.TARE_DT_ESTIMADA;
                item.TARE_IN_STATUS = tarefa.TARE_IN_STATUS;
                item.TARE_IN_PRIORIDADE = tarefa.TARE_IN_PRIORIDADE;
                item.TARE_DT_REALIZADA = tarefa.TARE_DT_REALIZADA;
                item.TARE_TX_OBSERVACOES = tarefa.TARE_TX_OBSERVACOES;
                item.TARE_NM_LOCAL = tarefa.TARE_NM_LOCAL;
                item.TARE_IN_AVISA = tarefa.TARE_IN_AVISA;
                item.ASSI_CD_ID = tarefa.ASSI_CD_ID;
                objetoAntes = tarefa;
                item.TARE_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                listaMaster = new List<TAREFA>();
                Session["ListaTarefa"] = null;
                Session["TarefaAlterada"] = 1;
                return RedirectToAction("MontarTelaTarefa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tarefas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files)
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
            Session["FileQueueTarefa"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueTarefa(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensTarefa"] = 10;
                return RedirectToAction("VoltarAnexoTarefa");
            }

            TAREFA item = baseApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;

            if (fileName.Length > 250)
            {
                Session["MensTarefa"] = 11;
                return RedirectToAction("VoltarAnexoTarefa");
            }

            String caminho = "/Imagens/" + usu.ASSI_CD_ID.ToString() + "/Tarefa/" + item.TARE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                TAREFA_ANEXO foto = new TAREFA_ANEXO();
                foto.TAAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.TAAN_DT_ANEXO = DateTime.Today;
                foto.TAAN_IN_ATIVO = 1;
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
                foto.TAAN_IN_TIPO = tipo;
                foto.TAAN_NM_TITULO = fileName;
                foto.TARE_CD_ID = item.TARE_CD_ID;

                item.TAREFA_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                return RedirectToAction("VoltarAnexoTarefa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tarefas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileTarefa(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensTarefa"] = 10;
                return RedirectToAction("VoltarAnexoTarefa");
            }

            TAREFA item = baseApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);

            if (fileName.Length > 100)
            {
                Session["MensTarefa"] = 11;
                return RedirectToAction("VoltarAnexoTarefa");
            }

            String caminho = "/Imagens/" + usu.ASSI_CD_ID.ToString() + "/Tarefa/" + item.TARE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                TAREFA_ANEXO foto = new TAREFA_ANEXO();
                foto.TAAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.TAAN_DT_ANEXO = DateTime.Today;
                foto.TAAN_IN_ATIVO = 1;
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
                foto.TAAN_IN_TIPO = tipo;
                foto.TAAN_NM_TITULO = fileName;
                foto.TARE_CD_ID = item.TARE_CD_ID;

                item.TAREFA_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                return RedirectToAction("VoltarAnexoTarefa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tarefas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public JsonResult UploadFileTarefa_Inclusao(IEnumerable<HttpPostedFileBase> files, Int32 perfil)
        {
            var count = 0;

            if (perfil == 0)
            {
                count++;
            }

            foreach (var file in files)
            {
                if (count == 0)
                {
                }
                else
                {
                    UploadFileTarefa(file);
                }
            }

            return Json("1"); 
        }

        public ActionResult VoltarAnexoTarefa()
        {
            return RedirectToAction("EditarTarefa", new { id = (Int32)Session["IdVolta"] });
        }

        public ActionResult VerKanbanTarefa()
        {
            return RedirectToAction("CarregarDesenvolvimento", "BaseAdmin");
        }

        public ActionResult GerarRelatorioLista()
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
            String nomeRel = "TarefaLista" + "_" + data + ".pdf";
            List<TAREFA> lista = (List<TAREFA>)Session["ListaTarefa"];
            TAREFA filtro = (TAREFA)Session["FiltroTarefa"];
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
            image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Tarefas - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 80f, 60f, 120f, 60f, 80f, 60f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Tarefas selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
            cell = new PdfPCell(new Paragraph("Título", meuFont))
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
            cell = new PdfPCell(new Paragraph("Status", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Realizada", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (TAREFA item in lista)
            {
                if (item.TIPO_TAREFA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TIPO_TAREFA.TITR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.TARE_DT_CADASTRO.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TARE_NM_TITULO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.TARE_DT_ESTIMADA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TARE_DT_ESTIMADA.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Sem estimativa", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.TARE_IN_STATUS == 1)
                {
                    cell = new PdfPCell(new Paragraph("Pendente", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.TARE_IN_STATUS == 2)
                {
                    cell = new PdfPCell(new Paragraph("Em Andamento", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.TARE_IN_STATUS == 3)
                {
                    cell = new PdfPCell(new Paragraph("Suspensa", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.TARE_IN_STATUS == 4)
                {
                    cell = new PdfPCell(new Paragraph("Cancelada", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.TARE_IN_STATUS == 5)
                {
                    cell = new PdfPCell(new Paragraph("Encerrada", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.TARE_DT_REALIZADA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TARE_DT_REALIZADA.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
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
                if (filtro.TITR_CD_ID != null)
                {
                    parametros += "Tipo: " + filtro.TIPO_TAREFA.TITR_NM_NOME;
                    ja = 1;
                }
                if (filtro.TARE_NM_TITULO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Título: " + filtro.TARE_NM_TITULO;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Título: " + filtro.TARE_NM_TITULO;
                    }
                }
                if (filtro.TARE_DT_CADASTRO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data: " + filtro.TARE_DT_CADASTRO.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data: " + filtro.TARE_DT_CADASTRO.ToShortDateString();
                    }
                }
                if (filtro.TARE_IN_STATUS > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Status: " + (filtro.TARE_IN_STATUS == 1 ? "Pendente" : filtro.TARE_IN_STATUS == 2 ? "Em Andamento" : filtro.TARE_IN_STATUS == 3 ? "Suspensa" : filtro.TARE_IN_STATUS == 4 ? "Cancelada" : "Realizada");
                        ja = 1;
                    }
                    else
                    {
                        parametros += "e Status: " + (filtro.TARE_IN_STATUS == 1 ? "Pendente" : filtro.TARE_IN_STATUS == 2 ? "Em Andamento" : filtro.TARE_IN_STATUS == 3 ? "Suspensa" : filtro.TARE_IN_STATUS == 4 ? "Cancelada" : "Realizada");
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

            return RedirectToAction("MontarTelaTarefa");
        }

        public FileResult DownloadTarefa(Int32 id)
        {
            TAREFA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.TAAN_AQ_ARQUIVO;
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
        public ActionResult VerAnexoTarefa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            TAREFA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VerAnexoTarefaAudio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            TAREFA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult IncluirAcompanhamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            TAREFA item = baseApp.GetItemById((Int32)Session["IdVolta"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            TAREFA_ACOMPANHAMENTO coment = new TAREFA_ACOMPANHAMENTO();
            TarefaAcompanhamentoViewModel vm = Mapper.Map<TAREFA_ACOMPANHAMENTO, TarefaAcompanhamentoViewModel>(coment);
            vm.TAAC_DT_ACOMPANHAMENTO = DateTime.Now;
            vm.TAAC_IN_ATIVO = 1;
            vm.TARE_CD_ID = item.TARE_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcompanhamento(TarefaAcompanhamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TAREFA_ACOMPANHAMENTO item = Mapper.Map<TarefaAcompanhamentoViewModel, TAREFA_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TAREFA not = baseApp.GetItemById((Int32)Session["IdVolta"]);

                    item.USUARIO = null;
                    not.TAREFA_ACOMPANHAMENTO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarTarefa", new { id = (Int32)Session["IdVolta"] });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tarefas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tarefas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public List<TAREFA> CarregaTarefa(Int32 usuario)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TAREFA> conf = new List<TAREFA>();
            if (Session["TarefasLista"] == null)
            {
                conf = baseApp.GetByUser(usuario);
            }
            else
            {
                if ((Int32)Session["TarefaAlterada"] == 1)
                {
                    conf = baseApp.GetByUser(usuario);
                }
                else
                {
                    conf = (List<TAREFA>)Session["TarefasLista"];
                }
            }
            Session["TarefasLista"] = conf;
            Session["TarefaAlterada"] = 0;
            return conf;
        }

        public List<TIPO_TAREFA> CarregaTipoTarefa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_TAREFA> conf = new List<TIPO_TAREFA>();
            if (Session["TipoTarefas"] == null)
            {
                conf = baseApp.GetAllTipos(idAss);
            }
            else
            {
                if ((Int32)Session["TipoTarefaAlterada"] == 1)
                {
                    conf = baseApp.GetAllTipos(idAss);
                }
                else
                {
                    conf = (List<TIPO_TAREFA>)Session["TipoTarefas"];
                }
            }
            Session["TipoTarefas"] = conf;
            Session["TipoTarefaAlterada"] = 0;
            return conf;
        }

        public List<USUARIO> CarregaUsuario()
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

        public CONFIGURACAO CarregaConfiguracaoGeral()
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

        public List<PERIODICIDADE_TAREFA> CarregaPeriodicidade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PERIODICIDADE_TAREFA> conf = new List<PERIODICIDADE_TAREFA>();
            if (Session["Periodicidades"] == null)
            {
                conf = baseApp.GetAllPeriodicidade();
            }
            else
            {
                if ((Int32)Session["PeriodicidadeAlterada"] == 1)
                {
                    conf = baseApp.GetAllPeriodicidade();
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

    }
}