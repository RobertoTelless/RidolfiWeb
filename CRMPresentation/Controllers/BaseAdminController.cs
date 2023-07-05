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
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using System.Text;
using System.Net;
using CrossCutting;
using System.Text.RegularExpressions;
using Azure.Communication.Email;
using System.Threading.Tasks;
using OfficeOpenXml;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using Azure.Communication.Email;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class BaseAdminController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly INoticiaAppService notiApp;
        private readonly ILogAppService logApp;
        private readonly ITarefaAppService tarApp;
        private readonly INotificacaoAppService notfApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAgendaAppService ageApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IVideoAppService vidApp;
        private readonly IClienteAppService cliApp;
        private readonly IGrupoAppService gruApp;
        private readonly ITemplatePropostaAppService tpApp;
        private readonly IAssinanteAppService assiApp;
        private readonly IPlanoAppService planApp;
        private readonly ICRMAppService crmApp;
        private readonly IMensagemAppService menApp;
        private readonly ITemplateAppService temApp;
        private readonly IMensagemEnviadaSistemaAppService envApp;
        private readonly ITemplateEMailAppService mailApp;
        private readonly ITemplateSMSAppService smsApp;
        private readonly IPrecatorioAppService precApp;
        private readonly IBeneficiarioAppService beneApp;
        private readonly ITRFAppService trfApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        MENSAGENS_ENVIADAS_SISTEMA objetoEnviada = new MENSAGENS_ENVIADAS_SISTEMA();
        MENSAGENS_ENVIADAS_SISTEMA objetEnviadaoAntes = new MENSAGENS_ENVIADAS_SISTEMA();
        List<MENSAGENS_ENVIADAS_SISTEMA> listaMasterEnviada = new List<MENSAGENS_ENVIADAS_SISTEMA>();

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, INoticiaAppService notApps, ITarefaAppService tarApps, INotificacaoAppService notfApps, IUsuarioAppService usuApps, IAgendaAppService ageApps, IConfiguracaoAppService confApps, IVideoAppService vidApps, IClienteAppService cliApps, IGrupoAppService gruApps, ITemplatePropostaAppService tpApps, IAssinanteAppService assiApps, IPlanoAppService planApps, ICRMAppService crmApps, IMensagemAppService menApps, ITemplateAppService temApps, IMensagemEnviadaSistemaAppService envApps, ITemplateEMailAppService mailApps, ITemplateSMSAppService smsApps, IPrecatorioAppService precApps, IBeneficiarioAppService beneApps, ITRFAppService trfApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notApps;
            tarApp = tarApps;
            notfApp = notfApps;
            usuApp = usuApps;
            ageApp = ageApps;
            confApp = confApps;
            vidApp = vidApps;
            cliApp = cliApps;
            gruApp = gruApps;
            tpApp = tpApps;
            assiApp = assiApps; 
            planApp = planApps; 
            crmApp = crmApps;
            menApp = menApps;   
            temApp = temApps;
            envApp= envApps;
            mailApp = mailApps;
            smsApp = smsApps;
            precApp = precApps;
            beneApp = beneApps;
            trfApp = trfApps;
        }

        public ActionResult CarregarAdmin()
        {
            Int32? idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = baseApp.GetAllUsuarios(idAss.Value).Count;
            ViewBag.Logs = logApp.GetAllItens(idAss.Value).Count;
            ViewBag.UsuariosLista = baseApp.GetAllUsuarios(idAss.Value);
            ViewBag.LogsLista = logApp.GetAllItens(idAss.Value);
            return View();

        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        public ActionResult CarregarLandingPage()
        {
            return View();
        }

        public JsonResult GetRefreshTime()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            return Json(confApp.GetAllItems(idAss).FirstOrDefault().CONF_NR_REFRESH_DASH);
        }

        public JsonResult GetConfigNotificacoes()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            bool hasNotf;
            var hash = new Hashtable();
            USUARIO usu = (USUARIO)Session["Usuario"];
            CONFIGURACAO conf = confApp.GetAllItems(idAss).FirstOrDefault();

            if (baseApp.GetAllItensUser(usu.USUA_CD_ID, idAss).Count > 0)
            {
                hasNotf = true;
            }
            else
            {
                hasNotf = false;
            }

            hash.Add("CONF_NM_ARQUIVO_ALARME", conf.CONF_NM_ARQUIVO_ALARME);
            hash.Add("NOTIFICACAO", hasNotf);
            return Json(hash);
        }

        public ActionResult CarregarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            ASSINANTE assi = assiApp.GetItemById(idAss);
            if ((Int32)Session["Login"] == 1)
            {
                Session["Perfis"] = baseApp.GetAllPerfis();
                Session["Usuarios"] = usuApp.GetAllUsuarios(idAss);
            }

            Session["MensTarefa"] = 0;
            Session["MensNoticia"] = 0;
            Session["MensNotificacao"] = 0;
            Session["MensUsuario"] = 0;
            Session["MensLog"] = 0;
            Session["MensUsuarioAdm"] = 0;
            Session["MensAgenda"] = 0;
            Session["MensTemplate"] = 0;
            Session["MensConfiguracao"] = 0;
            Session["MensCargo"] = 0;
            Session["MensSMSError"] = 0;
            Session["MensVideo"] = 0;
            Session["MensSenha"] = 0;
            Session["MensagemLogin"] = 0;
            Session["FlagCalculoCustos"] = 0;
            Session["VoltaNotificacao"] = 3;
            Session["VoltaNoticia"] = 1;
            Session["VoltaMensagem"] = 1;
            Session["NivelEmpresa"] = 1;
            Session["TipoCarga"] = 1;
            Session["TipoCargaMsgInt"] = 1;
            Session["ListaEmpresa"] = null;

            ViewBag.PermMensagem = (Int32)Session["PermMens"];
            ViewBag.PermCRM = (Int32)Session["PermCRM"];
            ViewBag.PermPesquisa = (Int32)Session["PermPesquisa"];
            ViewBag.PermAtendimento = (Int32)Session["PermAtendimentos"];
            ViewBag.PermComercial= (Int32)Session["PermComercial"];

            List<NOTIFICACAO> noti = new List<NOTIFICACAO>();
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            noti = notfApp.GetAllItensUser(usuario.USUA_CD_ID, usuario.ASSI_CD_ID);
            Session["Notificacoes"] = noti;
            Session["ListaNovas"] = noti.Where(p => p.NOTI_IN_VISTA == 0).ToList().Take(5).OrderByDescending(p => p.NOTI_DT_EMISSAO.Value).ToList();
            Session["NovasNotificacoes"] = noti.Where(p => p.NOTI_IN_VISTA == 0).Count();
            Session["Nome"] = usuario.USUA_NM_NOME;

            List<NOTICIA> not = CarregaNoticia().OrderByDescending(p => p.NOTC_DT_EMISSAO).ToList();
            Session["Noticias"] = not;
            Session["NoticiasNumero"] = not.Count;

            List<VIDEO> vid = CarregaVideo().OrderByDescending(p => p.VIDE_DT_EMISSAO).ToList();
            Session["Videos"] = vid;
            Session["VideosNumero"] = vid.Count;

            Session["ListaPendentes"] = tarApp.GetTarefaStatus(usuario.USUA_CD_ID, 1);
            Session["TarefasPendentes"] = ((List<TAREFA>)Session["ListaPendentes"]).Count;
            Session["TarefasLista"] = CarregaTarefa(usuario.USUA_CD_ID);
            Session["Tarefas"] = ((List<TAREFA>)Session["TarefasLista"]).Count;

            Session["Agendas"] = CarregaAgenda(usuario.USUA_CD_ID);
            Session["NumAgendas"] = ((List<AGENDA>)Session["Agendas"]).Count;
            Session["AgendasHoje"] = ((List<AGENDA>)Session["Agendas"]).Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList();
            Session["NumAgendasHoje"] = ((List<AGENDA>)Session["AgendasHoje"]).Count;
            Session["Logs"] = logApp.GetAllItensDataCorrente(idAss).Count;

            //Session["Planos"] = assi.ASSINANTE_PLANO;
            //Session["PlanosVencidos"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date).ToList();
            //Session["PlanosVencer30"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date.AddDays(30)).ToList();
            //Session["NumPlanos"] = assi.ASSINANTE_PLANO.Count;
            //Session["NumPlanosVencidos"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date).ToList().Count();
            //Session["NumPlanosVencer30"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date.AddDays(30)).ToList().Count();

            //List<ASSINANTE_PAGAMENTO> pags = assiApp.GetAllPagamentos();
            //pags = pags.Where(p => p.ASPA_IN_PAGO == 0 & p.ASPA_NR_ATRASO > 0 & p.ASSI_CD_ID == idAss).ToList();
            //Int32 atraso = pags.Count;
            //Session["Atrasos"] = pags;
            //Session["NumAtrasos"] = pags.Count;

            String frase = String.Empty;
            String nome = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
            Session["NomeGreeting"] = nome;
            if (DateTime.Now.Hour <= 12)
            {
                frase = "Bom dia, " + nome;
            }
            else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
            {
                frase = "Boa tarde, " + nome;
            }
            else
            {
                frase = "Boa noite, " + nome;
            }
            Session["Greeting"] = frase;
            Session["Foto"] = usuario.USUA_AQ_FOTO;

            // Mensagens
            if ((Int32)Session["MensPermissao"] == 2)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }

            // Mensagens de Planos
            //if ((Int32)Session["MensEnvioLogin"] == 0)
            //{
            //    if (((List<PlanoVencidoViewModel>)Session["PlanosVencidosModel"]).Count > 0)
            //    {
            //        List<PlanoVencidoViewModel> plans = (List<PlanoVencidoViewModel>)Session["PlanosVencidosModel"];
            //        foreach (PlanoVencidoViewModel item in plans)
            //        {
            //            PLANO pl = planApp.GetItemById(item.Plano);
            //            DateTime? dv = item.Vencimento;
            //            if (item.Tipo == 1)
            //            {
            //                String frase1 = "O plano " + pl.PLAN_NM_NOME + " venceu em " + dv.Value.ToShortDateString() + ". Por favor reative";
            //                ModelState.AddModelError("", frase);
            //            }
            //            if (item.Tipo == 2)
            //            {
            //                String frase1 = "O plano " + pl.PLAN_NM_NOME + " vence em " + dv.Value.ToShortDateString() + ". Por favor tome as providencias para que ele não expire.";
            //                ModelState.AddModelError("", frase);
            //            }
            //        }
            //        Session["MensEnvioLogin"] = 1;
            //    }
            //}

            Session["MensPermissao"] = 0;
            Session["NotificacaoAlterada"] = 0;
            Session["NoticiaAlterada"] = 0;
            Session["VideoAlterada"] = 0;
            return View(usuario);
        }

        public ActionResult CarregarDesenvolvimento()
        {
            return View();
        }

        public ActionResult VoltarDashboard()
        {
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarDashboardAdministracao()
        {
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public ActionResult MontarFaleConosco()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardAdministracao()
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


            // Carrega valores dos cadastros
            List<NOTICIA> noticia = CarregaNoticiaGeral();
            Int32 noticias = noticia.Count;
            List<VIDEO> video = CarregaVideo();
            Int32 videos = video.Count;

            Session["Noticias1"] = noticias;
            Session["Video"] = videos;

            ViewBag.Noticias = noticias;
            ViewBag.Videos = videos;
            ViewBag.Perfil = (String)Session["PerfilUsuario"];

            // Recupera listas usuarios
            List<USUARIO> listaTotal = CarregaUsuarioAdm();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<USUARIO> bloqueados = listaTotal.Where(p => p.USUA_IN_BLOQUEADO == 1).ToList();

            Int32 numUsuarios = listaTotal.Count;
            Int32 numBloqueados = bloqueados.Count;
            Int32? numAcessos = listaTotal.Sum(p => p.USUA_NR_ACESSOS);
            Int32? numFalhas = listaTotal.Sum(p => p.USUA_NR_FALHAS);

            ViewBag.NumUsuarios = numUsuarios;
            ViewBag.NumBloqueados = numBloqueados;
            ViewBag.NumAcessos = numAcessos;
            ViewBag.NumFalhas = numFalhas;

            Session["TotalUsuarios"] = listaTotal.Count;
            Session["Bloqueados"] = numBloqueados;

            // Recupera listas log
            List<LOG> listaLog = logApp.GetAllItensMesCorrente(idAss);
            if ((String)Session["PerfilUsuario"] == "GER")
            {
                listaLog = listaLog.Where(p => p.USUARIO.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Int32 log = listaLog.Count;
            Int32 logDia = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == DateTime.Today.Date).ToList().Count;
            Int32 logMes = listaLog.Count;
            List<LOG> listaDia = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == DateTime.Today.Date).ToList();
            List<LOG> listaMes = listaLog;
            
            ViewBag.Log = log;
            ViewBag.LogDia = logDia;
            ViewBag.LogMes = logMes;

            Session["TotalLog"] = log;
            Session["LogDia"] = logDia;
            Session["LogMes"] = logMes;

            // Resumo Log Diario
            List<DateTime> datasCR = listaMes.Where(m => m.LOG_DT_DATA.Value != null).OrderBy(m => m.LOG_DT_DATA.Value).Select(p => p.LOG_DT_DATA.Value.Date).Distinct().ToList();
            List<ModeloViewModel> listaLogDia = new List<ModeloViewModel>();
            foreach (DateTime item in datasCR)
            {
                Int32 conta = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.Valor = conta;
                listaLogDia.Add(mod1);
            }
            listaLogDia = listaLogDia.OrderBy(p => p.DataEmissao).ToList();
            ViewBag.ListaLogDia = listaLogDia;
            ViewBag.ContaLogDia = listaLogDia.Count;
            Session["ListaDatasLog"] = datasCR;
            Session["ListaLogResumo"] = listaLogDia;

            // Resumo Log Situacao  
            List<String> opLog = listaLog.Select(p => p.LOG_NM_OPERACAO).Distinct().ToList();
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            foreach (String item in opLog)
            {
                Int32 conta1 = listaLog.Where(p => p.LOG_NM_OPERACAO == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.Nome = item;
                mod1.Valor = conta1;
                lista2.Add(mod1);
            }
            ViewBag.ListaLogOp = lista2;
            ViewBag.ContaLogOp = lista2.Count;
            Session["ListaOpLog"] = opLog;
            Session["ListaLogOp"] = lista2;
            Session["VoltaDash"] = 3;
            Session["VoltaUnidade"] = 1;
            return View(usuario);
        }

        public JsonResult GetDadosGraficoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogResumo"];
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

        public JsonResult GetDadosGraficoCRSituacao()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogOp"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
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

        public JsonResult GetDadosGraficoLogOper()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogOp"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
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

        [HttpGet]
        public ActionResult PesquisarTudo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagem
            if (Session["MensBase"] != null)
            {
                if ((Int32)Session["MensBase"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0200", CultureInfo.CurrentCulture));
                }
            }
            Session["MensBase"] = null;

            if ((List<VOLTA_PESQUISA>)Session["VoltaPesquisa"] != null)
            {
                ViewBag.Listas = (List<VOLTA_PESQUISA>)Session["VoltaPesquisa"];
            }
            else
            {
                ViewBag.Listas = null;
            }

            // Processa
            MensagemWidgetViewModel vm = new MensagemWidgetViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult PesquisarTudo(MensagemWidgetViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica preenchimento
                    if (vm.Descrição == null)
                    {
                        Session["MensBase"] = 10;
                        return RedirectToAction("PesquisarTudo");
                    }

                    // Executa a operação
                    String perfil = (String)Session["PerfilUsuario"];
                    Int32[] permissoes = new Int32[] { (Int32)Session["PermMens"], (Int32)Session["PermCRM"], (Int32)Session["PermPesquisa"], (Int32)Session["PermAtendimentos"] };
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    List<VOLTA_PESQUISA> voltaPesq = cliApp.PesquisarTudo(vm.Descrição, permissoes, perfil, (Int32)Session["IdEmpresa"], idAss);
                    Session["VoltaPesquisa"] = voltaPesq;

                    // Verifica retorno
                    return RedirectToAction("PesquisarTudo");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Pesquisa", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult MontarCentralMensagens()
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
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Cria Base de mensagens
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            List<MensagemWidgetViewModel> listaMensagens = new List<MensagemWidgetViewModel>();
            if (Session["ListaMensagem"] == null)
            {
                // Carrega notificações
                List<NOTIFICACAO> notificacoes = (List<NOTIFICACAO>)Session["Notificacoes"];
                List<NOTIFICACAO> naoLidas = notificacoes.Where(p => p.NOTI_IN_VISTA == 0).OrderByDescending(p => p.NOTI_DT_EMISSAO).ToList();
                foreach (NOTIFICACAO item in naoLidas)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.NOTI_DT_EMISSAO;
                    mens.Descrição = item.NOTI_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.NOTI_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 1;
                    mens.Categoria = item.CATEGORIA_NOTIFICACAO.CANO_NM_NOME;
                    mens.NomeCliente = item.NOTI_IN_VISTA == 1 ? "Lida" : "Não Lida";
                    listaMensagens.Add(mens);
                }

                // Carrega Agenda
                List<AGENDA> agendas = (List<AGENDA>)Session["Agendas"];
                List<AGENDA> hoje = agendas.Where(p => p.AGEN_DT_DATA >= DateTime.Today.Date).OrderByDescending(p => p.AGEN_DT_DATA).ToList();
                foreach (AGENDA item in hoje)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.AGEN_DT_DATA;
                    mens.Descrição = item.AGEN_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.AGEN_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 2;
                    mens.Categoria = item.CATEGORIA_AGENDA.CAAG_NM_NOME;
                    mens.NomeCliente = item.AGEN_IN_STATUS == 1 ? "Ativa" : (item.AGEN_IN_STATUS == 2 ? "Suspensa" : "Encerrada");
                    listaMensagens.Add(mens);
                }

                // Carrega Tarefas
                List<TAREFA> tarefas = (List<TAREFA>)Session["ListaPendentes"];
                tarefas = tarefas.Where(p => p.TARE_DT_ESTIMADA >= DateTime.Today.Date).OrderByDescending(p => p.TARE_DT_ESTIMADA).ToList();
                foreach (TAREFA item in tarefas)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.TARE_DT_ESTIMADA;
                    mens.Descrição = item.TARE_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.TARE_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 3;
                    mens.Categoria = item.TIPO_TAREFA.TITR_NM_NOME;
                    if (item.TARE_IN_STATUS == 1)
                    {
                        mens.NomeCliente = "Ativa";

                    }
                    else if (item.TARE_IN_STATUS == 2)
                    {
                        mens.NomeCliente = "Em Andamento";

                    }
                    else if (item.TARE_IN_STATUS == 3)
                    {
                        mens.NomeCliente = "Suspensa";

                    }
                    else if (item.TARE_IN_STATUS == 4)
                    {
                        mens.NomeCliente = "Cancelada";

                    }
                    else if (item.TARE_IN_STATUS == 5)
                    {
                        mens.NomeCliente = "Encerrada";

                    }
                    listaMensagens.Add(mens);
                }
                Session["ListaMensagem"] = listaMensagens;
            }
            else
            {
                listaMensagens = (List<MensagemWidgetViewModel>)Session["ListaMensagem"];
            }

            // Prepara listas dos filtros
            List<SelectListItem> tipos = new List<SelectListItem>();
            tipos.Add(new SelectListItem() { Text = "Notificações", Value = "1" });
            tipos.Add(new SelectListItem() { Text = "Agenda", Value = "2" });
            tipos.Add(new SelectListItem() { Text = "Tarefas", Value = "3" });
            ViewBag.Tipos = new SelectList(tipos, "Value", "Text");
            List<SelectListItem> urg = new List<SelectListItem>();
            urg.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            urg.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Urgencia = new SelectList(urg, "Value", "Text");

            // Exibe
            ViewBag.ListaMensagem = listaMensagens;
            MensagemWidgetViewModel mod = new MensagemWidgetViewModel();
            return View(mod);
        }

        public ActionResult RetirarFiltroCentralMensagens()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMensagem"] = null;
            return RedirectToAction("MontarCentralMensagens");
        }

        [HttpPost]
        public ActionResult FiltrarCentralMensagens(MensagemWidgetViewModel item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<MensagemWidgetViewModel> listaObj = (List<MensagemWidgetViewModel>)Session["ListaMensagem"];
                if (item.TipoMensagem != null)
                {
                    listaObj = listaObj.Where(p => p.TipoMensagem == item.TipoMensagem).ToList();
                }
                if (item.Descrição != null)
                {
                    listaObj = listaObj.Where(p => p.Descrição.Contains(item.Descrição)).ToList();
                }
                if (item.DataMensagem != null)
                {
                    listaObj = listaObj.Where(p => p.DataMensagem == item.DataMensagem).ToList();
                }
                if (item.FlagUrgencia != null)
                {
                    listaObj = listaObj.Where(p => p.FlagUrgencia == item.FlagUrgencia).ToList();
                }
                Session["ListaMensagem"] = listaObj;

                // Sucesso
                return RedirectToAction("MontarCentralMensagens");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Central de Notificações", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTabelasAuxiliares()
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
            ViewBag.Permissoes = (Int32[])Session["Permissoes"];
            ViewBag.PermMensagem = (Int32)Session["PermMens"];
            ViewBag.PermCRM = (Int32)Session["PermCRM"];
            ViewBag.PermPesquisa = (Int32)Session["PermPesquisa"];
            ViewBag.PermAtendimento = (Int32)Session["PermAtendimentos"];
            return View(usuario);
        }


        public ActionResult MontarTelaDashboardCadastros()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega valores dos cadastros
            List<CLIENTE> cliente = CarregaCliente();
            Session["ListaCliente"] = cliente;
            Int32 clientes = cliente.Count;

            List<GRUPO> grupo = CarregaGrupo();
            Int32 grupos = grupo.Count;

            List<PRECATORIO> prec = CarregaPrecatorios();
            Int32 precs = prec.Count;

            List<BENEFICIARIO> bene = CarregaBeneficiarios();
            Int32 benes = prec.Count;

            List<TEMPLATE_PROPOSTA> temp = CarregaTemplateProposta();
            if ((String)Session["PerfilUsuario"] == "GER")
            {
                temp = temp.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Int32 temps = temp.Count;

            List<TEMPLATE_EMAIL> tempMail = CarregaTemplateEMail();
            if ((String)Session["PerfilUsuario"] == "GER")
            {
                tempMail = tempMail.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Int32 mails = tempMail.Count;

            List<TEMPLATE_SMS> tempSMS = CarregaTemplateSMS();
            if ((String)Session["PerfilUsuario"] == "GER")
            {
                tempSMS = tempSMS.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Int32 smss = tempSMS.Count;

            Session["ClientesConta"] = clientes;
            ViewBag.Clientes = clientes;
            Session["GruposConta"] = grupos;
            ViewBag.Grupos = grupos;
            Session["TempsConta"] = temps;
            ViewBag.Temps = temps;
            ViewBag.TempMail= mails;
            ViewBag.TempSMS = smss;
            ViewBag.Precatorios = precs;
            ViewBag.Beneficiarios = benes;

            ViewBag.Permissoes = (Int32[])Session["Permissoes"];
            ViewBag.PermMensagem = (Int32)Session["PermMens"];
            ViewBag.PermCRM = (Int32)Session["PermCRM"];
            ViewBag.PermPesquisa = (Int32)Session["PermPesquisa"];
            ViewBag.PermAtendimento = (Int32)Session["PermAtendimentos"];

            // Recupera clientes por UF
            List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
            List<UF> ufs = CarregaUF();
            foreach (UF item in ufs)
            {
                Int32 num = cliente.Where(p => p.UF_CD_ID == item.UF_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.UF_NM_NOME;
                    mod.Valor = num;
                    lista5.Add(mod);
                }
            }
            ViewBag.ListaClienteUF = lista5;
            Session["ListaClienteUF"] = lista5;

            // Recupera clientes por Cidade
            List<ModeloViewModel> lista6 = new List<ModeloViewModel>();
            List<String> cids = cliente.Select(p => p.CLIE_NM_CIDADE).Distinct().ToList();
            foreach (String item in cids)
            {
                Int32 num = cliente.Where(p => p.CLIE_NM_CIDADE == item).ToList().Count;
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome = item;
                mod.Valor = num;
                lista6.Add(mod);
            }
            ViewBag.ListaClienteCidade = lista6;
            Session["ListaClienteCidade"] = lista6;

            // Recupera clientes por Categoria
            List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
            List<CATEGORIA_CLIENTE> catc = CarregaCatCliente();
            foreach (CATEGORIA_CLIENTE item in catc)
            {
                Int32 num = cliente.Where(p => p.CACL_CD_ID == item.CACL_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.CACL_NM_NOME;
                    mod.Valor = num;
                    lista7.Add(mod);
                }
            }
            ViewBag.ListaClienteCats = lista7;
            Session["ListaClienteCats"] = lista7;

            // Recupera precatorios por TRF
            List<ModeloViewModel> lista9 = new List<ModeloViewModel>();
            List<TRF> trfs = CarregaTRF();
            foreach (TRF item in trfs)
            {
                Int32 num = prec.Where(p => p.TRF1_CD_ID == item.TRF1_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.TRF1_NM_NOME;
                    mod.Valor = num;
                    lista5.Add(mod);
                }
            }
            ViewBag.ListaPrecTRF = lista9;
            Session["ListaPrecTRF"] = lista9;

            Session["FlagAlteraCliente"] = 0;
            Session["FlagMensagensEnviadas"] = 1;
            return View(usuario);
        }

        public JsonResult GetDadosClienteUF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteUF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosClienteCidade()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteCidade"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosClienteCategoria()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteCats"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosPrecTRF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPrecTRF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosClienteUFLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteUF"];
            List<String> uf = new List<String>();
            List<Int32> valor = new List<Int32>();
            uf.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                uf.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("ufs", uf);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosPrecTRFLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPrecTRF"];
            List<String> trf = new List<String>();
            List<Int32> valor = new List<Int32>();
            trf.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                trf.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", trf);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult EditarClienteMark(Int32 id)
        {
            Session["NivelCliente"] = 1;
            Session["VoltaMsg"] = 55;
            Session["VoltaClienteCRM"] = 99;
            Session["IncluirCliente"] = 0;
            return RedirectToAction("EditarCliente", "Cliente", new { id = id });
        }

        public ActionResult EditarPrecatorioMark(Int32 id)
        {
            return RedirectToAction("EditarPrecatorio", "Precatorio", new { id = id });
        }

        public ActionResult EditarBeneficiarioMark(Int32 id)
        {
            return RedirectToAction("EditarBeneficiario", "Beneficiario", new { id = id });
        }

        public ActionResult AcompanhamentoProcessoCRMMark(Int32 id)
        {
            Session["VoltaCRMBase"] = 99;
            return RedirectToAction("AcompanhamentoProcessoCRM", "CRM", new { id = id });
        }

        public ActionResult EditarAcaoMark(Int32 id)
        {
            return RedirectToAction("EditarAcao", "CRM", new { id = id });
        }

        public ActionResult EditarPedidoMark(Int32 id)
        {
            Session["PontoProposta"] = 99;
            return RedirectToAction("VerPedido", "CRM", new { id = id });
        }

        //[HttpGet]
        //public ActionResult MontarTelaDashboardAssinantes()
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    if ((USUARIO)Session["UserCredentials"] != null)
        //    {
        //        usuario = (USUARIO)Session["UserCredentials"];

        //        // Verfifica permissão
        //        if (usuario.USUA_IN_ESPECIAL != 1)
        //        {
        //            Session["MensPermissao"] = 2;
        //            return RedirectToAction("CarregarBase", "BaseAdmin");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }

        //    // Recupera listas 
        //    List<ASSINANTE> listaTotal = CarregaAssinante();
        //    List<ASSINANTE> bloqueados = listaTotal.Where(p => p.ASSI_IN_BLOQUEADO == 1).ToList();
        //    List<PLANO> planos = CarregaPlano();

        //    Int32 numAssinantes = listaTotal.Count;
        //    Int32 numBloqueados = bloqueados.Count;
        //    ViewBag.NumAssinantes = numAssinantes;
        //    ViewBag.NumBloqueados = numBloqueados;
        //    ViewBag.Planos = planos.Count;

        //    List<ASSINANTE> listaDia = listaTotal.Where(p => p.ASSI_DT_INICIO.Value.Date == DateTime.Today.Date).ToList();
        //    List<ASSINANTE> listaMes = listaTotal.Where(p => p.ASSI_DT_INICIO.Value.Month == DateTime.Today.Month & p.ASSI_DT_INICIO.Value.Year == DateTime.Today.Year).ToList();

        //    // Recupera vencimentos
        //    List<ASSINANTE_PLANO> planosAss = assiApp.GetAllAssPlanos();
        //    List<ASSINANTE_PLANO> planosVencidos = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date).ToList();
        //    List<ASSINANTE_PLANO> planosVencer30 = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30)).ToList();
        //    Int32 vencidos = planosVencidos.Count;
        //    Int32 vencer30 = planosVencer30.Count;

        //    Session["PlanosVencidos"] = planosVencidos;
        //    Session["PlanosVencer30"] = planosVencer30;
        //    ViewBag.Vencidos = vencidos;
        //    ViewBag.Vencer30 = vencer30;
        //    ViewBag.Planos = planos.Count;

        //    // Recupera assinantes por UF
        //    List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
        //    List<UF> ufs = CarregaUF();
        //    foreach (UF item in ufs)
        //    {
        //        Int32 num = listaTotal.Where(p => p.UF_CD_ID == item.UF_CD_ID).ToList().Count;
        //        if (num > 0)
        //        {
        //            ModeloViewModel mod = new ModeloViewModel();
        //            mod.Nome = item.UF_NM_NOME;
        //            mod.Valor = num;
        //            lista5.Add(mod);
        //        }
        //    }
        //    ViewBag.ListaAssUF = lista5;
        //    Session["ListaAssUF"] = lista5;

        //    // Recupera assinantes por Cidade
        //    List<ModeloViewModel> lista6 = new List<ModeloViewModel>();
        //    List<String> cids = listaTotal.Select(p => p.ASSI_NM_CIDADE).Distinct().ToList();
        //    foreach (String item in cids)
        //    {
        //        Int32 num = listaTotal.Where(p => p.ASSI_NM_CIDADE == item).ToList().Count;
        //        ModeloViewModel mod = new ModeloViewModel();
        //        mod.Nome = item;
        //        mod.Valor = num;
        //        lista6.Add(mod);
        //    }
        //    ViewBag.ListaAssCidade = lista6;
        //    Session["ListaAssCidade"] = lista6;

        //    // Recupera assinantes por tipo
        //    List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
        //    List<TIPO_PESSOA> catc = CarregaTipoPessoa();
        //    foreach (TIPO_PESSOA item in catc)
        //    {
        //        Int32 num = listaTotal.Where(p => p.TIPE_CD_ID == item.TIPE_CD_ID).ToList().Count;
        //        if (num > 0)
        //        {
        //            ModeloViewModel mod = new ModeloViewModel();
        //            mod.Nome = item.TIPE_NM_NOME;
        //            mod.Valor = num;
        //            lista7.Add(mod);
        //        }
        //    }
        //    ViewBag.ListaAssCats = lista7;
        //    Session["ListaAssCats"] = lista7;

        //    // Recupera assinantes por data de início
        //    List<DateTime> datasCR = listaMes.Where(m => m.ASSI_DT_INICIO.Value != null).OrderBy(m => m.ASSI_DT_INICIO.Value).Select(p => p.ASSI_DT_INICIO.Value.Date).Distinct().ToList();
        //    List<ModeloViewModel> listaLogDia = new List<ModeloViewModel>();
        //    foreach (DateTime item in datasCR)
        //    {
        //        Int32 conta = listaTotal.Where(p => p.ASSI_DT_INICIO.Value.Date == item).ToList().Count;
        //        ModeloViewModel mod1 = new ModeloViewModel();
        //        mod1.DataEmissao = item;
        //        mod1.Valor = conta;
        //        listaLogDia.Add(mod1);
        //    }
        //    listaLogDia = listaLogDia.OrderBy(p => p.DataEmissao).ToList();
        //    ViewBag.ListaLogDia = listaLogDia;
        //    ViewBag.ContaLogDia = listaLogDia.Count;
        //    Session["ListaDatasLog"] = datasCR;
        //    Session["ListaLogResumo"] = listaLogDia;

        //    // Assinantes em atraso
        //    List<ASSINANTE_PAGAMENTO> pags = assiApp.GetAllPagamentos().ToList();
        //    pags = pags.Where(p => p.ASPA_NR_ATRASO > 0 & p.ASPA_IN_PAGO == 0).ToList();
        //    List<Int32> assi = pags.Select(p => p.ASSI_CD_ID).Distinct().ToList();
        //    Int32 numAtrasos = pags.Count;
        //    ViewBag.NumAtrasos = numAtrasos;
        //    Int32 numAssiAtrasos = assi.Count;
        //    ViewBag.NumAssiAtrasos = numAssiAtrasos;
            
        //    List<ModeloViewModel> lista8 = new List<ModeloViewModel>();
        //    foreach (Int32 item in assi)
        //    {
        //        ASSINANTE ass = listaTotal.Where(p => p.ASSI_CD_ID == item).FirstOrDefault();
        //        if (ass != null)
        //        {
        //            String nome = ass.ASSI_NM_NOME;
        //            Int32 num = pags.Where(p => p.ASSI_CD_ID == item).Count();
        //            Decimal? valor = pags.Where(p => p.ASSI_CD_ID == item).Sum(p => p.ASPA_VL_VALOR);
        //            ModeloViewModel mod = new ModeloViewModel();
        //            mod.Nome = nome;
        //            mod.Valor = num;
        //            mod.ValorDec = valor.Value;
        //            lista8.Add(mod);
        //        }
        //    }
        //    ViewBag.ListaAssAtraso = lista8;
        //    Session["ListaAssAtraso"] = lista8;

        //    // Exibe
        //    UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);
        //    Session["TotalAssinantes"] = listaTotal.Count;
        //    Session["Bloqueados"] = numBloqueados;
        //    Session["VoltaDash"] = 3;
        //    Session["VoltaAssinante"] = 2;
        //    return View(vm);
        //}


        public JsonResult GetDadosUsuario()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaUsuario"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosAssinanteUFLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssUF"];
            List<String> uf = new List<String>();
            List<Int32> valor = new List<Int32>();
            uf.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                uf.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("ufs", uf);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteCidadeLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssCidade"];
            List<String> cidade = new List<String>();
            List<Int32> valor = new List<Int32>();
            cidade.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                cidade.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", cidade);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteAtraso()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssAtraso"];
            List<String> nome = new List<String>();
            List<Int32> valor = new List<Int32>();
            nome.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                nome.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", nome);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteCategoria()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssCats"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosAssinanteUF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssUF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosAssinanteCidade()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssCidade"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public ActionResult TrataExcecao()
        {
            // Recupera Assinante e configuração
            CONFIGURACAO conf = null;
            Int32 idAss = 0;
            ASSINANTE assi = null;
            USUARIO usuario = null;
            if (Session["IdAssinante"] != null)
            {
                idAss = (Int32)Session["IdAssinante"];
                conf = confApp.GetItemById(idAss);
                assi = assiApp.GetItemById(idAss);
                usuario = (USUARIO)Session["UserCredentials"];
            }

            // Monta Exceção
            ExcecaoViewModel exc = new ExcecaoViewModel();
            Exception ex = (Exception)Session["Excecao"];
            exc.DataExcecao = DateTime.Now;
            exc.Gerador = (String)Session["VoltaExcecao"];
            exc.Message = ex.Message;
            exc.Source = ex.Source;
            exc.StackTrace = ex.StackTrace;
            if (ex.InnerException != null)
            {
                exc.Inner = ex.InnerException.Message;
            }
            exc.tipoExcecao = (String)Session["ExcecaoTipo"];
            exc.tipoVolta = (Int32)Session["TipoVolta"];
            if (conf != null)
            {
                exc.SuporteMail = conf.CONF_EM_CRMSYS;
                exc.SuporteZap = conf.CONF_NR_SUPORTE_ZAP;
            }
            else
            {
                exc.SuporteMail = "suporte@rtiltda.net";
                exc.SuporteZap = "(21)97302-4096";
            }
            Session["ExcecaoView"] = exc;

            // Gera mensagem automática para suporte          
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = assi.ASSI_NM_NOME;
            mens.ID = idAss;
            mens.MODELO = assi.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.EXCECAO = exc;
            Int32 volta = ProcessaEnvioEMailSuporte(mens, usuario);
            Int32 volta1 = ProcessaEnvioSMSSuporte(mens, usuario);
            return View(exc);
        }

        [HttpGet]
        public ActionResult EnviarEMailSuporte()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CONFIGURACAO conf = null;
            Int32 idAss = 0;
            ASSINANTE assi = null;
            if (Session["IdAssinante"] != null)
            {
                idAss = (Int32)Session["IdAssinante"];
                conf = confApp.GetItemById(idAss);
                assi = assiApp.GetItemById(idAss);
            }
            Session["Assinante"] = assi;
            ViewBag.Configuracao = conf;

            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = assi.ASSI_NM_NOME;
            mens.ID = idAss;
            mens.MODELO = assi.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.EXCECAO = (ExcecaoViewModel)Session["ExcecaoView"];
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailSuporte(MensagemViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailSuporte(vm, usuarioLogado);

                    // Verifica retorno
                    // Sucesso
                    return RedirectToAction("TrataExcecao");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailSuporte(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = 0;
            ASSINANTE assi = null;
            if (Session["IdAssinante"] != null)
            {
                idAss = (Int32)Session["IdAssinante"];
                assi = assiApp.GetItemById(idAss);
            }

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado <b>Suporte RTi</b>";

            // Prepara rodape
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara assinante
            String doc = assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ;
            String nome = assi.ASSI_NM_NOME + (doc != null ? " - " + doc : String.Empty);

            // Prepara corpo do e-mail
            String inner = String.Empty;
            String mens = String.Empty;
            String intro = "Por favor verifiquem a exceção abaixo e as condições em que ela ocorreu.<br />";
            String contato = "Para mais informações entre em contato pelo telefone <b>" + assi.ASSI_NR_CELULAR + "</b> ou pelo e-mail <b>" + assi.ASSI_NM_EMAIL + ".</b><br /><br />";
            String final = "<br />Atenciosamente,<br /><br />";
            String aplicacao = "<b>Aplicação: </b> CRMSys" + "<br />";
            String assinante = "<b>Assinante: </b>" + nome + "<br />";
            String data = "<b>Data: </b>" + DateTime.Today.Date.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";
            String modulo = "<b>Módulo: </b>" + vm.EXCECAO.Gerador + "<br />";
            String origem = "<b>Origem: </b>" + vm.EXCECAO.Source + "<br />";
            String tipo = "<b>Tipo da Exceção: </b>" + vm.EXCECAO.tipoExcecao + "<br />";
            String message = "<b>Exceção: </b>" + vm.EXCECAO.Message + "<br />";
            if (vm.EXCECAO.Inner != null)
            {
                inner = "<b>Exceção Interna: </b>" + vm.EXCECAO.Inner + "<br /><br />";
            }
            String trace = "<b>Stack Trace: </b>" + vm.EXCECAO.StackTrace + "<br />";
            String body = intro + contato + aplicacao + assinante + data + modulo + origem + tipo + message + inner + trace;

            if (vm.MENS_TX_TEXTO != null)
            {
                mens = vm.MENS_TX_TEXTO + "<br />";
            }
            body = body + mens + final;
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Solicitação de Suporte";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = conf.CONF_EM_CRMSYS;
            mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "RidolfiWeb";
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
            String status = "Succeeded";
            String iD = "xyz";

            // Envia mensagem
            try
            {
                Tuple<EmailSendStatus, String, Boolean> voltaMail = CrossCutting.CommunicationAzurePackage.SendMail(mensagem, null);
                status = voltaMail.Item1.ToString();
                iD = voltaMail.Item2;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
            }
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSSuporte()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CONFIGURACAO conf = null;
            Int32 idAss = 0;
            ASSINANTE assi = null;
            if (Session["IdAssinante"] != null)
            {
                idAss = (Int32)Session["IdAssinante"];
                conf = confApp.GetItemById(idAss);
                assi = assiApp.GetItemById(idAss);
            }
            Session["Assinante"] = assi;
            ViewBag.Configuracao = conf;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = assi.ASSI_NM_NOME;
            mens.ID = idAss;
            mens.MODELO = assi.ASSI_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            mens.EXCECAO = (ExcecaoViewModel)Session["ExcecaoView"];
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSSuporte(MensagemViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSSuporte(vm, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("TrataExcecao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSSuporte(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Suporte RTi. ";

            // Prepara rodape
            String rod = ". "+ assi.ASSI_NM_NOME;

            // Prepara assinante
            String doc = assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ;
            String nome = assi.ASSI_NM_NOME + (doc != null ? " - " + doc : String.Empty);

            // Decriptografa chaves
            String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
            String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

            // Monta token
            String text = login + ":" + senha;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara corpo do SMS
            String mens = String.Empty;
            String intro = "Por favor verifiquem a exceção abaixo e as condições em que ela ocorreu. ";
            String contato = "Para mais informações entre em contato pelo telefone " + assi.ASSI_NR_CELULAR + " ou pelo e-mail " + assi.ASSI_NM_EMAIL + ". ";
            String aviso = "Veja e-mail enviado para o suporte com maiores detalhes. ";

            String aplicacao = " Aplicação: CRMSys. ";
            String assinante = "Assinante: " + nome + ". ";
            String data = "Data: " + DateTime.Today.Date.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ", ";
            String modulo = "Módulo: " + vm.EXCECAO.Gerador + ". ";
            String origem = "Origem: " + vm.EXCECAO.Source + ". ";
            String tipo = "Tipo da Exceção: " + vm.EXCECAO.tipoExcecao + ". ";
            String message = "Exceção: " + vm.EXCECAO.Message + ". ";
            if (vm.MENS_TX_TEXTO != null)
            {
                mens = vm.MENS_TX_TEXTO + ". ";
            }
            String body = intro + contato + aviso + aplicacao + assinante + data + modulo + origem + tipo + message + mens;
            String smsBody = body;
            String erro = null;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(conf.CONF_NR_SUPORTE_ZAP, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String json = String.Empty;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", smsBody, "\", \"customId\": \"" + customId + "\", \"from\": \"RidolfiWeb\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }
            return 0;
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

        public List<USUARIO> CarregaUsuarioAdm()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<USUARIO> conf = new List<USUARIO>();
            if (Session["UsuariosAdm"] == null)
            {
                conf = usuApp.GetAllUsuariosAdm(idAss);
            }
            else
            {
                if ((Int32)Session["UsuarioAlterada"] == 1)
                {
                    conf = usuApp.GetAllUsuariosAdm(idAss);
                }
                else
                {
                    conf = (List<USUARIO>)Session["Usuarios"];
                }
            }
            Session["UsuarioAlterada"] = 0;
            Session["UsuariosAdm"] = conf;
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

        public List<AGENDA> CarregaAgenda(Int32 usuario)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<AGENDA> conf = new List<AGENDA>();
            if (Session["Agendas"] == null)
            {
                conf = ageApp.GetByUser(usuario, idAss);
            }
            else
            {
                if ((Int32)Session["AgendaAlterada"] == 1)
                {
                    conf = ageApp.GetByUser(usuario, idAss);
                }
                else
                {
                    conf = (List<AGENDA>)Session["Agendas"];
                }
            }
            Session["Agendas"] = conf;
            Session["AgendaAlterada"] = 0;
            return conf;
        }

        public List<TAREFA> CarregaTarefa(Int32 usuario)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TAREFA> conf = new List<TAREFA>();
            if (Session["Tarefas1"] == null)
            {
                conf = tarApp.GetByUser(usuario);
            }
            else
            {
                if ((Int32)Session["TarefaAlterada"] == 1)
                {
                    conf = tarApp.GetByUser(usuario);
                }
                else
                {
                    conf = (List<TAREFA>)Session["Tarefas1"];
                }
            }
            Session["Tarefas1"] = conf;
            Session["TarefaAlterada"] = 0;
            return conf;
        }

        public List<PRECATORIO> CarregaPrecatorios()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PRECATORIO> conf = new List<PRECATORIO>();
            if (Session["Precatorios"] == null)
            {
                conf = precApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["PrecatorioAlterada"] == 1)
                {
                    conf = precApp.GetAllItens();
                }
                else
                {
                    conf = (List<PRECATORIO>)Session["Precatorios"];
                }
            }
            Session["Precatorios"] = conf;
            Session["PrecatorioAlterada"] = 0;
            return conf;
        }

        public List<BENEFICIARIO> CarregaBeneficiarios()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<BENEFICIARIO> conf = new List<BENEFICIARIO>();
            if (Session["Beneficiarios"] == null)
            {
                conf = beneApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["BeneficiarioAlterada"] == 1)
                {
                    conf = beneApp.GetAllItens();
                }
                else
                {
                    conf = (List<BENEFICIARIO>)Session["Beneficiarios"];
                }
            }
            Session["Beneficiarios"] = conf;
            Session["BeneficiarioAlterada"] = 0;
            return conf;
        }

        public List<TRF> CarregaTRF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TRF> conf = new List<TRF>();
            if (Session["TRFs"] == null)
            {
                conf = trfApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["TRFAlterada"] == 1)
                {
                    conf = trfApp.GetAllItensAdm();
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

        public List<NOTICIA> CarregaNoticia()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<NOTICIA> conf = new List<NOTICIA>();
            if (Session["Noticias"] == null)
            {
                conf = notiApp.GetAllItensValidos(idAss);
            }
            else
            {
                if ((Int32)Session["NoticiaAlterada"] == 1)
                {
                    conf = notiApp.GetAllItensValidos(idAss);
                }
                else
                {
                    conf = (List<NOTICIA>)Session["Noticias"];
                }
            }
            Session["Noticias"] = conf;
            Session["NoticiaAlterada"] = 0;
            return conf;
        }

        public List<NOTICIA> CarregaNoticiaGeral()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<NOTICIA> conf = new List<NOTICIA>();
            if (Session["NoticiaGeral"] == null)
            {
                conf = notiApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["NoticiaAlterada"] == 1)
                {
                    conf = notiApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<NOTICIA>)Session["NoticiaGeral"];
                }
            }
            Session["NoticiaGeral"] = conf;
            Session["NoticiaAlterada"] = 0;
            return conf;
        }

        public List<NOTIFICACAO> CarregaNotificacao()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<NOTIFICACAO> conf = new List<NOTIFICACAO>();
            if (Session["Notificacoes"] == null)
            {
                conf = notfApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["NotificacaoAlterada"] == 1)
                {
                    conf = notfApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<NOTIFICACAO>)Session["Notificacoes"];
                }
            }
            Session["Notificacoes"] = conf;
            Session["NotificacaoAlterada"] = 0;
            return conf;
        }

        public List<VIDEO> CarregaVideo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<VIDEO> conf = new List<VIDEO>();
            if (Session["Videos"] == null)
            {
                conf = vidApp.GetAllItensValidos(idAss);
            }
            else
            {
                if ((Int32)Session["VideoAlterada"] == 1)
                {
                    conf = vidApp.GetAllItensValidos(idAss);
                }
                else
                {
                    conf = (List<VIDEO>)Session["Videos"];
                }
            }
            Session["Videos"] = conf;
            Session["VideoAlterada"] = 0;
            return conf;
        }

        public List<CLIENTE> CarregaCliente()
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

        public List<GRUPO> CarregaGrupo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<GRUPO> conf = new List<GRUPO>();
            if (Session["Grupos"] == null)
            {
                conf = gruApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = gruApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<GRUPO>)Session["Grupos"];
                }
            }
            Session["Grupos"] = conf;
            Session["GrupoAlterada"] = 0;
            return conf;
        }

        public List<TEMPLATE_PROPOSTA> CarregaTemplateProposta()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_PROPOSTA> conf = new List<TEMPLATE_PROPOSTA>();
            if (Session["TemplatesProposta"] == null)
            {
                conf = tpApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["TemplatesPropostaAlterada"] == 1)
                {
                    conf = tpApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TEMPLATE_PROPOSTA>)Session["TemplatesProposta"];
                }
            }
            Session["TemplatesProposta"] = conf;
            Session["TemplatesPropostaAlterada"] = 0;
            return conf;
        }

        public List<TEMPLATE_EMAIL> CarregaTemplateEMail()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_EMAIL> conf = new List<TEMPLATE_EMAIL>();
            if (Session["TemplatesEMail"] == null)
            {
                conf = mailApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["TemplatesEMailAlterada"] == 1)
                {
                    conf = mailApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TEMPLATE_EMAIL>)Session["TemplatesEMail"];
                }
            }
            Session["TemplatesEMail"] = conf;
            Session["TemplatesEMailAlterada"] = 0;
            return conf;
        }

        public List<TEMPLATE_SMS> CarregaTemplateSMS()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_SMS> conf = new List<TEMPLATE_SMS>();
            if (Session["TemplatesSMS"] == null)
            {
                conf = smsApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["TemplatesSMSAlterada"] == 1)
                {
                    conf = smsApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TEMPLATE_SMS>)Session["TemplatesSMS"];
                }
            }
            Session["TemplatesSMS"] = conf;
            Session["TemplatesSMSAlterada"] = 0;
            return conf;
        }

        public List<UF> CarregaUF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<UF> conf = new List<UF>();
            if (Session["UF"] == null)
            {
                conf = cliApp.GetAllUF();
            }
            else
            {
                    conf = (List<UF>)Session["UF"];
            }
            Session["UF"] = conf;
            return conf;
        }

        public List<TIPO_PESSOA> CarregaTipoPessoa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_PESSOA> conf = new List<TIPO_PESSOA>();
            if (Session["TipoPessoa"] == null)
            {
                conf = cliApp.GetAllTiposPessoa();
            }
            else
            {
                conf = (List<TIPO_PESSOA>)Session["TipoPessoa"];
            }
            Session["TipoPessoa"] = conf;
            return conf;
        }

        public List<SEXO> CarregaSexo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SEXO> conf = new List<SEXO>();
            if (Session["Sexos"] == null)
            {
                conf = cliApp.GetAllSexo();
            }
            else
            {
                conf = (List<SEXO>)Session["Sexos"];
            }
            Session["Sexos"] = conf;
            return conf;
        }

        public List<CATEGORIA_CLIENTE> CarregaCatCliente()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CATEGORIA_CLIENTE> conf = new List<CATEGORIA_CLIENTE>();
            if (Session["CatClientes"] == null)
            {
                conf = cliApp.GetAllTipos(idAss);
            }
            else
            {
                if ((Int32)Session["CatClienteAlterada"] == 1)
                {
                    conf = cliApp.GetAllTipos(idAss);
                }
                else
                {
                    conf = (List<CATEGORIA_CLIENTE>)Session["CatClientes"];
                }
            }
            Session["CatClientes"] = conf;
            Session["CatClienteAlterada"] = 0;
            return conf;
        }

        public List<ASSINANTE> CarregaAssinante()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<ASSINANTE> conf = new List<ASSINANTE>();
            if (Session["Assinantes"] == null)
            {
                conf = assiApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["AssinanteAlterada"] == 1)
                {
                    conf = assiApp.GetAllItens();
                }
                else
                {
                    conf = (List<ASSINANTE>)Session["Assinantes"];
                }
            }
            Session["Assinantes"] = conf;
            Session["AssinanteAlterada"] = 0;
            return conf;
        }

        public List<PLANO> CarregaPlano()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PLANO> conf = new List<PLANO>();
            if (Session["PlanosCarga"] == null)
            {
                conf = planApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["PlanoAlterada"] == 1)
                {
                    conf = planApp.GetAllItens();
                }
                else
                {
                    conf = (List<PLANO>)Session["PlanosCarga"];
                }
            }
            Session["PlanosCarga"] = conf;
            Session["PlanoAlterada"] = 0;
            return conf;
        }

        public ActionResult VerNotificacaoBase(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaNotificacao"] = 10;
            return RedirectToAction("VerNotificacao", "Notificacao", new { id = id });
        }

        public ActionResult VerAgendaBase(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaAgenda"] = 10;
            Session["VoltaAgendaCRM"] = 0;
            return RedirectToAction("EditarAgenda", "Agenda", new { id = id });
        }

        public ActionResult VerTarefaBase(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            Session["VoltaTarefa"] = 10;
            Session["VoltaAgendaCRM"] = 0;
            return RedirectToAction("EditarTarefa", "Tarefa", new { id = id });
        }

        [HttpGet]
        public ActionResult MontarTelaMensagensEnviadas()
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
            if ((List<MENSAGENS_ENVIADAS_SISTEMA>)Session["ListaMensagensEnviadas"] == null)
            {
                listaMasterEnviada = CarregaMensagensEnviadasHoje();
                Session["ListaMensagensEnviadas"] = listaMasterEnviada;
            }

            // Monta demais listas
            List<USUARIO> listaUsu = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            ViewBag.Usuarios = new SelectList(listaUsu, "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "SMS", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "WhatsApp", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> escopo = new List<SelectListItem>();
            escopo.Add(new SelectListItem() { Text = "Cliente", Value = "1" });
            escopo.Add(new SelectListItem() { Text = "Usuário", Value = "3" });
            escopo.Add(new SelectListItem() { Text = "Processos", Value = "4" });
            ViewBag.Escopos = new SelectList(escopo, "Value", "Text");
            ViewBag.TipoCarga = (Int32)Session["TipoCargaMsgInt"];
            Session["VoltaTela"] = 0;

            // Restringe pelo perfil
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<MENSAGENS_ENVIADAS_SISTEMA> listaMens = ((List<MENSAGENS_ENVIADAS_SISTEMA>)Session["ListaMensagensEnviadas"]).ToList();
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
            {
                listaMens = listaMens.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            ViewBag.Listas = listaMens;

            // Indicadores
            ViewBag.Mensagens = listaMens.Count;
            ViewBag.EMails = listaMens.Where(p => p.MEEN_IN_TIPO == 1).Count();
            ViewBag.SMS = listaMens.Where(p => p.MEEN_IN_TIPO == 2).Count();
            ViewBag.WZ = listaMens.Where(p => p.MEEN_IN_TIPO == 3).Count();
            ViewBag.Entregue = listaMens.Where(p => p.MEEN_IN_ENTREGUE == 1).Count();
            ViewBag.Falha = listaMens.Where(p => p.MEEN_IN_ENTREGUE == 0).Count();

            // Mensagem
            if (Session["MensEnviada"] != null)
            {
                if ((Int32)Session["MensEnviada"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEnviada"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensEnviada"] = 0;
            objetoEnviada = new MENSAGENS_ENVIADAS_SISTEMA();
            if (Session["FiltroMensagensEnviadas"] != null)
            {
                objetoEnviada = (MENSAGENS_ENVIADAS_SISTEMA)Session["FiltroMensagensEnviadas"];
            }
            objetoEnviada.MEEN_IN_ATIVO = 1;
            objetoEnviada.MEEN_DT_DATA_ENVIO = DateTime.Today.Date;
            objetoEnviada.MEEN_DT_DUMMY = DateTime.Today.Date;
            objetoEnviada.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(objetoEnviada);
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagensEnviadas()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
            if (Session["MensagensEnviadas"] == null)
            {
                conf = envApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["MensagemEnviadaAlterada"] == 1)
                {
                    conf = envApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<MENSAGENS_ENVIADAS_SISTEMA>)Session["MensagensEnviadas"];
                }
            }
            if ((String)Session["PerfilUsuario"] != "GER")
            {
                conf = conf.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            Session["MensagemEnviadaAlterada"] = 0;
            Session["MensagensEnviadas"] = conf;
            return conf;
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagensEnviadasHoje()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
            DateTime data = DateTime.Today.Date;
            if (Session["MensagensEnviadas"] == null)
            {
                conf = envApp.GetByDate(data, idAss);
            }
            else
            {
                if ((Int32)Session["MensagemEnviadaAlterada"] == 1)
                {
                    conf = envApp.GetByDate(data, idAss);
                }
                else
                {
                    conf = (List<MENSAGENS_ENVIADAS_SISTEMA>)Session["MensagensEnviadas"];
                }
            }
            if ((String)Session["PerfilUsuario"] != "GER")
            {
                conf = conf.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            Session["MensagemEnviadaAlterada"] = 0;
            Session["MensagensEnviadas"] = conf;
            return conf;
        }

        public ActionResult RetirarFiltroMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Session["MensagemEnviadaAlterada"] = 1;
            List<MENSAGENS_ENVIADAS_SISTEMA> lm = CarregaMensagensEnviadasHoje();
            if ((String)Session["PerfilUsuario"] != "GER")
            {
                lm = lm.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            Session["ListaMensagensEnviadas"] = lm;
            Session["FiltroMensagensEnviadas"] = null;
            Session["TipoCargaMsgInt"] = 1;
            return RedirectToAction("MontarTelaMensagensEnviadas");
        }

        public ActionResult MostrarTodasMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Session["MensagemEnviadaAlterada"] = 1;
            List<MENSAGENS_ENVIADAS_SISTEMA> lm = CarregaMensagensEnviadas();
            if ((String)Session["PerfilUsuario"] != "GER")
            {
                lm = lm.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            Session["ListaMensagensEnviadas"] = lm;
            Session["FiltroMensagensEnviadas"] = null;
            Session["TipoCargaMsgInt"] = 2;
            return RedirectToAction("MontarTelaMensagensEnviadas");
        }

        [HttpPost]
        public ActionResult FiltrarMensagensEnviadas(MENSAGENS_ENVIADAS_SISTEMA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MENSAGENS_ENVIADAS_SISTEMA> listaObj = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                Session["FiltroMensagensEnviadas"] = item;
                Tuple<Int32, List<MENSAGENS_ENVIADAS_SISTEMA>, Boolean> volta = envApp.ExecuteFilterTuple(item.MEEN_IN_ESCOPO, item.MEEN_IN_TIPO, item.MEEN_DT_DATA_ENVIO, item.MEEN_DT_DUMMY, item.USUA_CD_ID, item.MEEN_NM_TITULO, item.MEEN_NM_ORIGEM, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensEnviada"] = 1;
                    return RedirectToAction("MontarTelaMensagensEnviadas");
                }

                // Sucesso
                listaMasterEnviada = volta.Item2;
                if ((String)Session["PerfilUsuario"] != "GER")
                {
                    listaMasterEnviada = listaMasterEnviada.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagensEnviadas"] = listaMasterEnviada;
                Session["TipoCargaMsgInt"] = 2;
                return RedirectToAction("MontarTelaMensagensEnviadas");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens Enviadas", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMensagemEnviada(Int32 id)
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
            MENSAGENS_ENVIADAS_SISTEMA item = envApp.GetItemById(id);
            MensagemEmitidaViewModel vm = Mapper.Map<MENSAGENS_ENVIADAS_SISTEMA, MensagemEmitidaViewModel>(item);
            Session["MensagemEnviada"] = item;
            Session["IdVolta"] = id;
            Session["IdFilial"] = id;
            return View(vm);
        }

        public ActionResult VoltarMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 flag = (Int32)Session["FlagMensagensEnviadas"];
            
            
            if ((Int32)Session["FlagMensagensEnviadas"] == 1)
            {
                return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 2)
            {
                return RedirectToAction("MontarTelaCliente", "Cliente");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 3)
            {
                return RedirectToAction("MontarTelaFilial", "Filial");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 4)
            {
                return RedirectToAction("MontarTelaUsuario", "Usuario");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 5)
            {
                return RedirectToAction("MontarTelaAssinante", "Assinante");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 6)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 7)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 8)
            {
                return RedirectToAction("VoltarAcompanhamentoCRMBase", "CRM");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult ImportarPlanilhaMunicipio()
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
            return View();
        }

        public Task ProcessaOperacaoPlanilha(HttpPostedFileBase file)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO user = (USUARIO)Session["UserCredentials"];
            Int32 conta = 0;
            Int32 falha = 0;

            // Recupera configuracao
            CONFIGURACAO conf = confApp.GetItemById(idAss);

            // Processa planilha
            using (var pkg = new ExcelPackage(file.InputStream))
            {
                // Inicialização
                ExcelWorksheet wsGeral = pkg.Workbook.Worksheets[0];
                var wsFinalRow = wsGeral.Dimension.End;

                // Listas de pesquisas
                List<UF> ufs = cliApp.GetAllUF();
                UF uf = new UF();

                // Processa Planilha
                for (int row = 2; row < wsFinalRow.Row; row++)
                {
                    try
                    {
                        // Inicialização
                        Int32 ufMun = 0;
                        Int32 volta = 0;
                        uf = null;

                        // Recupera colunas
                        String x = null;
                        if (wsGeral.Cells[row, 1].Value != null)
                        {
                            x = wsGeral.Cells[row, 1].Value.ToString();
                        }
                        String y = null;
                        if (wsGeral.Cells[row, 2].Value != null)
                        {
                            y = wsGeral.Cells[row, 2].Value.ToString();
                        }
                        String z = null;
                        if (wsGeral.Cells[row, 3].Value != null)
                        {
                            z = wsGeral.Cells[row, 3].Value.ToString();
                        }
                        String sigla = null;
                        if (wsGeral.Cells[row, 4].Value != null)
                        {
                            sigla = wsGeral.Cells[row, 4].Value.ToString();
                        }
                        String nome = null;
                        if (wsGeral.Cells[row, 5].Value != null)
                        {
                            nome = wsGeral.Cells[row, 5].Value.ToString();
                        }
                        String m = null;
                        if (wsGeral.Cells[row, 6].Value != null)
                        {
                            m = wsGeral.Cells[row, 6].Value.ToString();
                        }
                        String n = null;
                        if (wsGeral.Cells[row, 7].Value != null)
                        {
                            n = wsGeral.Cells[row, 7].Value.ToString();
                        }
                        String u = null;
                        if (wsGeral.Cells[row, 8].Value != null)
                        {
                            u = wsGeral.Cells[row, 8].Value.ToString();
                        }

                        // Verifica saida
                        if (sigla == null & nome == null)
                        {
                            break;
                        }

                        // Sigla
                        if (sigla != null)
                        { 
                            // Verifica existencia
                            uf = ufs.Where(p => p.UF_SG_SIGLA.ToUpper() == sigla.ToUpper()).FirstOrDefault();
                            ufMun = uf.UF_CD_ID;
                        }

                        // Monta objeto
                        MUNICIPIO muni = new MUNICIPIO();
                        muni.UF_CD_ID = ufMun;
                        muni.MUNI_NM_NOME = nome;
                        muni.MUNI_IN_ATIVO = 1;

                        // Verifica existencia
                        MUNICIPIO checa = cliApp.CheckExistMunicipio(muni);

                        // Grava objeto
                        if (checa == null)
                        {
                            Int32 volta5 = cliApp.ValidateCreateMunicipio(muni);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message);
                        ViewBag.Message = ex.Message;
                        Session["TipoVolta"] = 2;
                        Session["VoltaExcecao"] = "Pesquisa";
                        Session["Excecao"] = ex;
                        Session["ExcecaoTipo"] = ex.GetType().ToString();
                        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                        Int32 voltaX = grava.GravarLogExcecao(ex, "Importação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    }
                }
                return Task.Delay(5);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ImportarPlanilhaMunicipio(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            await ProcessaOperacaoPlanilha(file);

            // Finaliza
            return RedirectToAction("CarregarBase");
        }
    }
}