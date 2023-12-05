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
        private readonly IClienteAppService cliApp;
        private readonly IGrupoAppService gruApp;
        private readonly ITemplatePropostaAppService tpApp;
        private readonly IAssinanteAppService assiApp;
        private readonly IPlanoAppService planApp;
        private readonly ICRMAppService crmApp;
        private readonly ITemplateAppService temApp;
        private readonly IMensagemEnviadaSistemaAppService envApp;
        private readonly ITemplateEMailAppService mailApp;
        private readonly ITemplateSMSAppService smsApp;
        private readonly IEmpresaAppService empApp;
        private readonly IFunilAppService funApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        MENSAGENS_ENVIADAS_SISTEMA objetoEnviada = new MENSAGENS_ENVIADAS_SISTEMA();
        MENSAGENS_ENVIADAS_SISTEMA objetEnviadaoAntes = new MENSAGENS_ENVIADAS_SISTEMA();
        List<MENSAGENS_ENVIADAS_SISTEMA> listaMasterEnviada = new List<MENSAGENS_ENVIADAS_SISTEMA>();

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, INoticiaAppService notApps, ITarefaAppService tarApps, INotificacaoAppService notfApps, IUsuarioAppService usuApps, IAgendaAppService ageApps, IConfiguracaoAppService confApps, IClienteAppService cliApps, IGrupoAppService gruApps, ITemplatePropostaAppService tpApps, IAssinanteAppService assiApps, IPlanoAppService planApps, ICRMAppService crmApps, ITemplateAppService temApps, IMensagemEnviadaSistemaAppService envApps, ITemplateEMailAppService mailApps, ITemplateSMSAppService smsApps, IEmpresaAppService empApps, IFunilAppService funApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notApps;
            tarApp = tarApps;
            notfApp = notfApps;
            usuApp = usuApps;
            ageApp = ageApps;
            confApp = confApps;
            cliApp = cliApps;
            gruApp = gruApps;
            tpApp = tpApps;
            assiApp = assiApps; 
            planApp = planApps; 
            crmApp = crmApps;
            temApp = temApps;
            envApp= envApps;
            mailApp = mailApps;
            smsApp = smsApps;
            empApp = empApps;
            funApp = funApps;
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

        public JsonResult GetLembrete()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            bool hasNotf;
            var hash = new Hashtable();
            USUARIO usu = (USUARIO)Session["Usuario"];
            CONFIGURACAO conf = confApp.GetAllItems(idAss).FirstOrDefault();

            List<CRM_FOLLOW> follow = CarregaCRMFollow().Where(p => p.TIFL_CD_ID == 3 & p.CRFL_DT_PREVISTA.Value.Date == DateTime.Today.Date).ToList();
            if (follow.Count > 0)
            {
                hasNotf = true;
            }
            else
            {
                hasNotf = false;
            }

            hash.Add("CONF_NM_ARQUIVO_ALARME", conf.CONF_NM_ARQUIVO_ALARME);
            hash.Add("LEMBRETE", hasNotf);
            return Json(hash);
        }

        public ActionResult CarregarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if ((Int32)Session["MensPermisao"] == 2)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }

            // Carrega listas
            ASSINANTE assi = assiApp.GetItemById(idAss);
            Session["Perfis"] = baseApp.GetAllPerfis();
            Session["Usuarios"] = CarregaUsuario();
            PERFIL perfil = usuario.PERFIL;

            Session["PerfilUsuario"] = perfil;
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
            Session["MensSenha"] = 0;
            Session["MensagemLogin"] = 0;
            Session["MensEnvioLogin"] = 0;
            Session["VoltaNotificacao"] = 3;
            Session["VoltaNoticia"] = 1;
            Session["VoltaMensagem"] = 1;
            Session["VoltarTabs"] = 1;
            Session["VoltarEmpresa"] = 1;
            Session["VoltarConsulta"] = 1;
            Session["InclusaoOK"] = 0;
            Session["NivelEmpresa"] = 1;
            Session["NivelEmpresa"] = 1;
            Session["LinhaAlterada"] = 0;
            Session["TipoCargaMsgInt"] = 2;
            Session["MensConfiguracao"] = 0;
            Session["RetornoPesquisa"] = 0;
            Session["Manual"] = "/Documentacao/CRMSys_Manual_Versao_1.pdf";
            Session["VoltaNotificacao"] = 3;
            Session["VoltaNoticia"] = 1;
            Session["VoltaMensagem"] = 1;
            Session["NivelEmpresa"] = 1;
            Session["TipoCarga"] = 1;
            Session["TipoCargaMsgInt"] = 1;
            Session["ListaEmpresa"] = null;
            Session["MensEmpresa"] = 0;
            Session["LinhaAlterada"] = 0;
            Session["LinhaAlterada1"] = 0;
            Session["LinhaAlterada2"] = 0;
            Session["ListaFunil"] = null;
            Session["MensMensagem"] = 0;
            Session["VoltaAnexos"] = 0;
            Session["VoltaCRM"] = 0;
            Session["MensagensEnviadas"] = null;

            // Permissoes
            ViewBag.PermCRM = (Int32)Session["PermCRM"];
            ViewBag.Empresas = new SelectList(CarregaEmpresa(), "EMPR_CD_ID", "EMPR_NM_NOME");

            // Carrega Cabecalho
            if (Session["PerfilUsuario"] == null)
            {
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["PerfilUsuario"] = usuario.PERFIL;
            }

            if ((Int32)Session["FlagNotificacoes"] == 1)
            {
                List<NOTIFICACAO> noti = new List<NOTIFICACAO>();
                noti = CarregaNotificacao(usuario.USUA_CD_ID).ToList();
                Session["Notificacoes"] = noti;
                Session["ListaNovas"] = noti.Where(p => p.NOTI_IN_VISTA == 0).ToList().Take(5).OrderByDescending(p => p.NOTI_DT_EMISSAO.Value).ToList();
                Session["NovasNotificacoes"] = noti.Where(p => p.NOTI_IN_VISTA == 0).Count();
                Session["Nome"] = usuario.USUA_NM_NOME;
                Session["FlagNotificacoes"] = 0;
            }

            if ((Int32)Session["FlagTarefas"] == 1)
            {
                Session["ListaPendentes"] = tarApp.GetTarefaStatus(usuario.USUA_CD_ID, 1);
                Session["TarefasPendentes"] = ((List<TAREFA>)Session["ListaPendentes"]).Count;
                Session["TarefasLista"] = CarregaTarefa(usuario.USUA_CD_ID);
                Session["Tarefas"] = ((List<TAREFA>)Session["TarefasLista"]).Count;
                Session["FlagTarefas"] = 0;
            }
            ViewBag.TarefaHoje = ((List<TAREFA>)Session["TarefasLista"]).Where(p => p.TARE_DT_ESTIMADA == DateTime.Today.Date).ToList().Count;

            if ((Int32)Session["FlagAgendas"] == 1)
            {
                Session["Agendas"] = CarregaAgenda(usuario.USUA_CD_ID);
                Session["NumAgendas"] = ((List<AGENDA>)Session["Agendas"]).Count;
                Session["AgendasHoje"] = ((List<AGENDA>)Session["Agendas"]).Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList();
                Session["NumAgendasHoje"] = ((List<AGENDA>)Session["AgendasHoje"]).Count;
                Session["Logs"] = logApp.GetAllItensDataCorrente(idAss).Count;
                Session["FlagAgendas"] = 0;
            }
            ViewBag.AgendaHoje = ((List<AGENDA>)Session["Agendas"]).Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList().Count;

            // Carrega numeros
            Int32 volta1 = CarregaNumeroCRM();
            List<CRM> listaCRM = (List<CRM>)Session["ListaCRMBase"];
            List<CRM> lm = listaCRM.Where(p => p.CRM1_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.CRM1_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            DateTime limite = (DateTime)Session["Limite"];
            List<DateTime> datas = lm.Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();

            // Recupera clientes
            List<CLIENTE> cliente = new List<CLIENTE>();
            if ((Int32)Session["ClienteAlterada"] == 1 || (Int32)Session["FlagCliente"] == 1 || Session["ListaCliente"] == null)
            {
                cliente = CarregaCliente().Where(p => p.ASSI_CD_ID == idAss).ToList();
                Session["ListaCliente"] = cliente;
                Int32 clientes = cliente.Count;
            }

            // Recupera clientes por UF
            if ((Int32)Session["ClienteAlterada"] == 1 || (Int32)Session["FlagCliente"] == 1 || Session["ListaClienteUF"] == null)
            {
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
            }
            else
            {
                ViewBag.ListaClienteUF = (List<ModeloViewModel>)Session["ListaClienteUF"];
            }

            // Recupera clientes por Cidade
            if ((Int32)Session["ClienteAlterada"] == 1 || (Int32)Session["FlagCliente"] == 1 || Session["ListaClienteCidade"] == null)
            {
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
            }
            else
            {
                ViewBag.ListaClienteCidade = (List<ModeloViewModel>)Session["ListaClienteCidade"];
            }

            // Recupera clientes por Categoria
            if ((Int32)Session["ClienteAlterada"] == 1 || (Int32)Session["FlagCliente"] == 1 || Session["ListaClienteCats"] == null)
            {
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
                Session["FlagAlteraCliente"] = 0;
            }
            else
            {
                ViewBag.ListaClienteCats = (List<ModeloViewModel>)Session["ListaClienteCats"];
                Session["FlagAlteraCliente"] = 0;
            }

            // Resumo Diario CRM
            if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMMesResumo"] == null)
            {
                datas = lm.Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista15 = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    if (item.Date > limite)
                    {
                        Int32 conta = lm.Where(p => p.CRM1_DT_CRIACAO.Value.Date == item).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        lista15.Add(mod);
                    }
                }
                ViewBag.ListaCRMMes = lista15;
                ViewBag.ContaCRMMes = lm.Count;
                Session["ListaDatasCRM"] = datas;
                Session["ListaCRMMesResumo"] = lista15;
                Session["ListaCRMMesResumoConta"] = lista15.Count;
                Session["VoltaKB"] = 0;
            }
            else
            {
                ViewBag.ContaCRMMes = lm.Count;
                ViewBag.ListaCRMMes = (List<ModeloViewModel>)Session["ListaCRMMesResumo"];
                Session["ListaDatasCRM"] = datas;
                Session["VoltaKB"] = 0;
            }

            // Resumo Mes CRM
            datas = listaCRM.Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();
            datas.Sort((i, j) => i.Date.CompareTo(j.Date));

            if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMMeses"] == null)
            {
                List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                String mes = null;
                String mesFeito = null;
                foreach (DateTime item in datas)
                {
                    if (item.Date > limite)
                    {
                        mes = item.Month.ToString() + "/" + item.Year.ToString();
                        if (mes != mesFeito)
                        {
                            Int32 conta = listaCRM.Where(p => p.CRM1_DT_CRIACAO.Value.Date.Month == item.Month & p.CRM1_DT_CRIACAO.Value.Date.Year == item.Year & p.CRM1_DT_CRIACAO > limite).Count();
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
                Session["ContaCRMMeses"] = listaMes.Count;
                Session["ListaDatasCRM"] = datas;
                Session["ListaCRMMeses"] = listaMes;
            }
            else
            {
                ViewBag.ContaCRMMeses = (Int32)Session["ContaCRMMeses"];
                ViewBag.ListaCRMMeses = (List<ModeloViewModel>)Session["ListaCRMMeses"];
                Session["ListaDatasCRM"] = datas;
            }

            // Resumo Diario Pedidos
            List<CRM_PEDIDO_VENDA> lmp1 = (List<CRM_PEDIDO_VENDA>)Session["ListaPedidosMes"];
            List<DateTime> datasPed = lmp1.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
            datasPed.Sort((i, j) => i.Date.CompareTo(j.Date));
            if ((Int32)Session["CRMPedidoAlterada"] == 1 || (Int32)Session["FlagCRMPedido"] == 1 || Session["ListaPedidosMesResumo"] == null)
            {
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
                Session["ContaPedidoMes"] = lmp1.Count;
                Session["ListaDatasPed"] = datasPed;
                Session["ListaPedidosMesResumo"] = listaPed;
                Session["ListaPedidosMesResumoConta"] = listaPed.Count;
            }
            else
            {
                ViewBag.ContaPedidosMes = (Int32)Session["ContaPedidoMes"];
                ViewBag.ListaPedidosMes = (List<ModeloViewModel>)Session["ListaPedidosMesResumo"];
            }

            // Resumo Mes Pedidos
            List<CRM_PEDIDO_VENDA> peds = (List<CRM_PEDIDO_VENDA>)Session["ListaPedidosBase"];
            if ((Int32)Session["CRMPedidoAlterada"] == 1 || (Int32)Session["FlagCRMPedido"] == 1 || Session["ListaPedidosMeses"] == null)
            {
                String mes = String.Empty;
                String mesFeito = String.Empty;
                List<ModeloViewModel> listaMesPed = new List<ModeloViewModel>();
                if (peds != null)
                {
                    datasPed = peds.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
                    datasPed.Sort((i, j) => i.Date.CompareTo(j.Date));

                    listaMesPed = new List<ModeloViewModel>();
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
                    Session["ListaMesesPed"] = datasPed;
                    Session["ListaPedidosMeses"] = listaMesPed;
                    Session["ListaPedidosMesesConta"] = listaMesPed.Count;
                }
                else
                {
                    Session["ListaPedidosMeses"] = listaMesPed;
                }
            }

            // Resumo Situacao CRM 
            if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaCRMSituacao"] == null)
            {
                List<ModeloViewModel> lista16 = new List<ModeloViewModel>();
                for (int i = 1; i < 6; i++)
                {
                    Int32 conta = listaCRM.Where(p => p.CRM1_IN_ATIVO == i & p.CRM1_DT_CRIACAO > limite).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Data = i == 1 ? "Ativo" : (i == 2 ? "Arquivados" : (i == 3 ? "Cancelados" : (i == 4 ? "Falhados" : (i == 5 ? "Sucesso" : (i == 6 ? "Faturamento" : (i == 7 ? "Expedição" : "Entregue"))))));
                    mod.Valor = conta;
                    lista16.Add(mod);
                }
                ViewBag.ListaCRMSituacao = lista16;
                Session["ListaCRMSituacao"] = lista16;
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
                    Int32 conta = listaCRM.Where(p => p.CRM1_IN_STATUS == i & p.CRM1_DT_CRIACAO > limite).Count();
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
                List<CRM_ACAO> acoes = (List<CRM_ACAO>)Session["ListaAcoesBase"];
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
            List<Int32> funis = listaCRM.Select(p => p.FUNI_CD_ID.Value).Distinct().ToList();
            List<FUNIL> funs = (List<FUNIL>)Session["ListaFunilBase"];
            if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaFunilResumo"] == null)
            {
                List<ModeloViewModel> listaFunil = new List<ModeloViewModel>();
                foreach (Int32 item in funis)
                {
                    Int32 conta = listaCRM.Where(p => p.FUNI_CD_ID == item & p.CRM1_DT_CRIACAO > limite).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Valor1 = item;
                    mod.Valor = conta;
                    mod.Nome = funs.Where(p => p.FUNI_CD_ID == item).First().FUNI_NM_NOME;
                    listaFunil.Add(mod);
                }
                ViewBag.ListaFunil = listaFunil;
                ViewBag.ContaFunil = listaFunil.Count;
                Session["ContaFunil"] = listaFunil.Count;
                Session["ListaFunil"] = funis;
                Session["ListaFunilResumo"] = listaFunil;
            }
            else
            {
                ViewBag.ListaFunil = (List<ModeloViewModel>)Session["ListaFunil"];
                ViewBag.ContaFunil = (Int32)Session["ContaFunil"];
            }

            // Resumo Pedidos
            if ((Int32)Session["CRMPedidoAlterada"] == 1 || (Int32)Session["FlagCRMPedido"] == 1 || Session["ListaCRMPed"] == null)
            {
                List<ModeloViewModel> lista51 = new List<ModeloViewModel>();
                if (peds != null)
                {
                    for (int i = 1; i < 6; i++)
                    {
                        Int32 conta = peds.Where(p => p.CRPV_IN_STATUS == i).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviado" : (i == 3 ? "Cancelado" : (i == 4 ? "Reprovado" : "Aprovado")));
                        mod.Valor = conta;
                        lista51.Add(mod);
                    }
                    ViewBag.ListaCRMPed = lista51;
                    Session["ListaCRMPed"] = lista51;
                }
                else
                {
                    Session["ListaCRMPed"] = lista51;
                }
            }
            else
            {
                ViewBag.ListaCRMPed = (List<ModeloViewModel>)Session["ListaCRMPed"];
            }

            // Recupera processos por etapa
            if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaEtapaResumo"] == null)
            {
                List<CRM> lt = (List<CRM>)Session["ListaCRM"];
                limite = DateTime.Today.Date.AddMonths(-12);
                List<ModeloViewModel> lista12 = new List<ModeloViewModel>();
                funis = lt.Select(p => p.FUNI_CD_ID.Value).Distinct().ToList();
                foreach (Int32 item in funis)
                {
                    FUNIL funil = funs.Where(p => p.FUNI_CD_ID == item).First();
                    List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.ToList();
                    foreach (FUNIL_ETAPA etapa in etapas)
                    {
                        Int32 conta1 = lt.Where(p => p.FUNI_CD_ID.Value == item & p.CRM1_IN_STATUS == etapa.FUET_IN_ORDEM & p.CRM1_DT_CRIACAO > limite).ToList().Count;
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
                Session["ContaEtapa"] = lista12.Count;
                Session["ListaEtapa"] = funis;
                Session["ListaEtapaResumo"] = lista12;
            }
            else
            {
                ViewBag.ListaEtapa = (List<ModeloViewModel>)Session["ListaEtapaResumo"];
                ViewBag.ContaEtapa = (Int32)Session["ContaEtapa"];
            }


            // Recupera ações pendentes
            if ((Int32)Session["CRMAcaoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaPendencia"] == null)
            {
                List<CRM_ACAO> acoesPend = (List<CRM_ACAO>)Session["ListaCRMAcoesPend"];
                List<ModeloViewModel> lista151 = new List<ModeloViewModel>();
                foreach (CRM_ACAO item in acoesPend)
                {
                    ModeloViewModel modZ = new ModeloViewModel();
                    modZ.Nome = item.CRAC_NM_TITULO;
                    modZ.DataEmissao = item.CRAC_DT_PREVISTA.Value;
                    modZ.Valor = item.CRAC_CD_ID;
                    lista151.Add(modZ);
                }
                ViewBag.ListaPend = lista151;
                ViewBag.ContaPend = lista151.Count;
                Session["ContaPend"] = lista151.Count;
                Session["ListaPendencia"] = lista151;
            }
            else
            {
                ViewBag.ListaPend = (List<ModeloViewModel>)Session["ListaPendencia"];
                ViewBag.ContaPend = (Int32)Session["ContaPend"];
            }

            // Recupera ações atrasadas
            if ((Int32)Session["CRMAcaoAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaAtraso"] == null)
            {
                List<CRM_ACAO> acoesAtraso = (List<CRM_ACAO>)Session["ListaCRMAcoesAtraso"];
                List<ModeloViewModel> lista161 = new List<ModeloViewModel>();
                foreach (CRM_ACAO item in acoesAtraso)
                {
                    ModeloViewModel modZ = new ModeloViewModel();
                    modZ.Nome = item.CRAC_NM_TITULO;
                    modZ.DataEmissao = item.CRAC_DT_PREVISTA.Value;
                    modZ.Valor = item.CRAC_CD_ID;
                    lista161.Add(modZ);
                }
                ViewBag.ListaAtraso = lista161;
                ViewBag.ContaAtraso = lista161.Count;
                Session["ContaAtraso"] = lista161.Count;
                Session["ListaAtraso"] = lista161;
            }
            else
            {
                ViewBag.ListaAtraso = (List<ModeloViewModel>)Session["ListaAtraso"];
                ViewBag.ContaAtraso = (Int32)Session["ContaAtraso"];
            }

            // Recupera lembretes
            if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaLemb"] == null)
            {
                List<CRM_FOLLOW> follows_Lembra = (List<CRM_FOLLOW>)Session["ListaCRMFollowsLembra"];
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
                Session["ContaLemb"] = lista17.Count;
                Session["ListaLemb"] = lista17;
            }
            else
            {
                ViewBag.ListaLemb = (List<ModeloViewModel>)Session["ListaLemb"];
                ViewBag.ContaLemb = (Int32)Session["ContaLemb"];
            }

            // Recupera alertas
            if ((Int32)Session["CRMAlterada"] == 1 || (Int32)Session["FlagCRM"] == 1 || Session["ListaAlerta"] == null)
            {
                List<CRM_FOLLOW> follows_Alerta = (List<CRM_FOLLOW>)Session["ListaCRMFollowsAlerta"];
                List<ModeloViewModel> lista22 = new List<ModeloViewModel>();
                foreach (CRM_FOLLOW item in follows_Alerta)
                {
                    ModeloViewModel modZ = new ModeloViewModel();
                    modZ.Nome = item.CRFL_NM_TITULO;
                    modZ.DataEmissao = item.CRFL_DT_PREVISTA.Value;
                    modZ.Valor = item.CRFL_CD_ID;
                    lista22.Add(modZ);
                }
                ViewBag.ListaAlerta = lista22;
                ViewBag.ContaAlerta = lista22.Count;
                Session["ListaAlerta"] = lista22;
                Session["ContaAlerta"] = lista22.Count;
            }
            else
            {
                ViewBag.ListaAlerta = (List<ModeloViewModel>)Session["ListaAlerta"];
                ViewBag.ContaAlerta = (Int32)Session["ContaAlerta"];
            }

            // Reseta flags
            Session["FlagCliente"] = 0;
            Session["FlagCRM"] = 0;
            Session["FlagCRMPedido"] = 0;

            // Planos
            Session["Planos"] = assi.ASSINANTE_PLANO;
            Session["PlanosVencidos"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date).ToList();
            Session["PlanosVencer30"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date.AddDays(30)).ToList();
            Session["NumPlanos"] = assi.ASSINANTE_PLANO.Count;
            Session["NumPlanosVencidos"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date).ToList().Count();
            Session["NumPlanosVencer30"] = assi.ASSINANTE_PLANO.Where(p => p.ASPL_DT_VALIDADE.Value.Date < DateTime.Today.Date.AddDays(30)).ToList().Count();

            // Pagamentos
            List<ASSINANTE_PAGAMENTO> pags = assiApp.GetAllPagamentos();
            pags = pags.Where(p => p.ASPA_IN_PAGO == 0 & p.ASPA_NR_ATRASO > 0 & p.ASSI_CD_ID == idAss).ToList();
            Int32 atraso = pags.Count;
            Session["Atrasos"] = pags;
            Session["NumAtrasos"] = pags.Count;

            // Apresentação
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

            // Mensagens de Planos
            if ((Int32)Session["MensEnvioLogin"] == 0)
            {
                if ((String)Session["Vence30"] == "Sim")
                {
                    String frase1 = "O plano " + (String)Session["NomePlano"] + " vence em menos de 30 dias.";
                    ModelState.AddModelError("", frase);
                    Session["MensEnvioLogin"] = 1;
                }
            }

            // Mensagem de permissão
            if ((Int32)Session["MensPermissao"] == 2)
            {
                String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                ModelState.AddModelError("", mens);
                Session["MensPermissao"] = 0;
            }


            // Finalização
            ViewBag.NomeEmpresa = (String)Session["NomeEmpresa"];
            Session["MensPermissao"] = 0;
            Session["NotificacaoAlterada"] = 0;
            Session["NoticiaAlterada"] = 0;
            Session["FlagMensagensEnviadas"] = 10;
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

        [HttpGet]
        public ActionResult MontarTelaDashboardAdministracao()
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
                    if (usuario.PERFIL.PERF_SG_SIGLA == "ADM")
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
                ViewBag.Perfil = ((PERFIL)Session["PerfilUsuario"]).PERF_SG_SIGLA;

                // Recupera listas usuarios
                List<USUARIO> listaTotal = CarregaUsuarioAdm();
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Administração";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administração", "SysPrec", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
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
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult PesquisarTudo(MensagemWidgetViewModel vm)
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

                    // Verifica preenchimento
                    if (vm.Descrição == null)
                    {
                        Session["MensBase"] = 10;
                        return RedirectToAction("PesquisarTudo");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    List<VOLTA_PESQUISA> voltaPesq = usuApp.PesquisarTudo(vm.Descrição, usuarioLogado.USUA_CD_ID, idAss);
                    Session["VoltaPesquisa"] = voltaPesq;

                    // Mensagens
                    Session["MensCliente"] = 0;
                    Session["MensCRM"] = 0;
                    Session["MensAcao"] = 0;
                    Session["MensProposta"] = 0;
                    Session["AbaCliente"] = 1;
                    Session["AbaCRM"] = 1;
                    Session["RetornoPesquisa"] = 1;
                    Session["VoltaTarefa"] = 11;
                    Session["VoltaAgenda"] = 11;

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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Pesquisa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroCentralMensagens()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaMensagem"] = null;
                return RedirectToAction("MontarCentralMensagens");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarCentralMensagens(MensagemWidgetViewModel item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

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
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
            ViewBag.PermCRM = (Int32)Session["PermCRM"];
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
            List<TEMPLATE_PROPOSTA> temp = CarregaTemplateProposta();
            Int32 temps = temp.Count;

            List<TEMPLATE_EMAIL> tempMail = CarregaTemplateEMail();
            Int32 mails = tempMail.Count;

            Session["TempsConta"] = temps;
            ViewBag.Temps = temps;
            ViewBag.TempMail= mails;

            ViewBag.Permissoes = (Int32[])Session["Permissoes"];
            ViewBag.PermCRM = (Int32)Session["PermCRM"];

            // Encerra
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

        public JsonResult GetDadosClienteCidadeLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteCidade"];
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

        public JsonResult GetDadosClienteCategoriaLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteCats"];
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

        public ActionResult EditarClienteMark(Int32 id)
        {
            Session["NivelCliente"] = 1;
            Session["VoltaMsg"] = 55;
            Session["VoltaClienteCRM"] = 99;
            Session["IncluirCliente"] = 0;
            Session["AbaCliente"] = 1;
            return RedirectToAction("EditarCliente", "Cliente", new { id = id });
        }

        public ActionResult AcompanhamentoProcessoCRMMark(Int32 id)
        {
            Session["VoltaCRMBase"] = 99;
            Session["AbaCRM"] = 1;
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

        public JsonResult GetPlanos(Int32 id)
        {
            PLANO forn = assiApp.GetPlanoBaseById(id);
            var hash = new Hashtable();
            hash.Add("nome", forn.PLAN_NM_NOME);
            hash.Add("periodicidade", forn.PLANO_PERIODICIDADE.PLPE_NM_NOME);
            hash.Add("valor", CrossCutting.Formatters.DecimalFormatter(forn.PLAN_VL_PRECO.Value));
            hash.Add("promo", CrossCutting.Formatters.DecimalFormatter(forn.PLAN_VL_PROMOCAO.Value));
            DateTime data = DateTime.Today.Date.AddDays(Convert.ToDouble(forn.PLANO_PERIODICIDADE.PLPE_NR_DIAS));
            hash.Add("data", data.ToShortDateString());
            hash.Add("duracao", forn.PLAN_IN_DURACAO);
            return Json(hash);
        }

        public async Task<ActionResult> TrataExcecao()
        {
            try
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
                Int32 volta = await ProcessaEnvioEMailSuporte(mens, usuario);

                // Mensagem
                ModelState.AddModelError("", "O processamento do CRMSys detectou uma falha. Uma mensagem urgente já foi enviada ao suporte com as informações abaixo e logo voce receberá a resposta. Se desejar reenvie a mensagem usando os botões disponíveis nesta página." + " ID do envio: " + (String)Session["IdMail"]);
                return View(exc);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Exceção";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EnviarEMailSuporte()
        {
            try
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
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarEMailSuporte(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = await ProcessaEnvioEMailSuporte(vm, usuarioLogado);

                    // Verifica retorno
                    // Sucesso
                    return RedirectToAction("TrataExcecao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Suporte";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailSuporte(MensagemViewModel vm, USUARIO usuario)
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
            String mail1 = conf.CONF_EM_CRMSYS;
            String mail2 = conf.CONF_EM_CRMSYS1;

            // Prepara cabeçalho
            String cab = "Prezado <b>Suporte RTi</b>";

            // Prepara rodape
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara assinante
            String doc = assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ;
            String nome = assi.ASSI_NM_NOME + (doc != null ? " - " + doc : String.Empty);

            // Prepara lista de destinos
            List<EmailAddress> emails = new List<EmailAddress>();
            EmailAddress add = new EmailAddress(address: mail1, displayName: "Suporte 1");
            emails.Add(add);
            EmailAddress add1 = new EmailAddress(address: mail2, displayName: "Suporte 2");
            emails.Add(add1);

            // Prepara corpo do e-mail
            String inner = String.Empty;
            String mens = String.Empty;
            String intro = "Por favor verifiquem a exceção abaixo e as condições em que ela ocorreu.<br />";
            String contato = "Para mais informações entre em contato pelo telefone <b>" + conf.CONF_NR_SUPORTE_ZAP + "</b> ou pelo e-mail <b>" + conf.CONF_EM_CRMSYS + ".</b><br /><br />";
            String final = "<br />Atenciosamente,<br /><br />";
            String aplicacao = "<b>Aplicação: </b> RidolfiNet" + "<br />";
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
            mensagem.NOME_EMISSOR = "RidolfiNet";
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
                await CrossCutting.CommunicationAzurePackage.SendMailListAsync(mensagem, null, emails);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSSuporte()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "SysPrec", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EnviarSMSSuporte(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

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
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Suporte";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "SysPrec", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
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
            String cel1 = conf.CONF_NR_SUPORTE_ZAP;
            String cel2 = conf.CONF_NR_SUPORTE_ZAP;

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
            String contato = "Para mais informações entre em contato pelo telefone " + conf.CONF_NR_SUPORTE_ZAP + " ou pelo e-mail " + conf.CONF_EM_CRMSYS + ". ";
            String aviso = "Veja e-mail enviado para o suporte com maiores detalhes. ";

            String aplicacao = " Aplicação: SysPrec. ";
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
                // Prepara envio
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String json = String.Empty;

                // Monta destinatarios
                String vetor = String.Empty;
                String listaDest = "55" + Regex.Replace(cel1, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                vetor = String.Concat("{\"to\": \"", listaDest, "\", \"text\": \"", smsBody, "\", \"customId\": \"" + customId + "\", \"from\": \"SysPrec\"}");
                String listaDest1 = "55" + Regex.Replace(cel2, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                vetor += String.Concat(",{\"to\": \"", listaDest, "\", \"text\": \"", smsBody, "\", \"customId\": \"" + customId + "\", \"from\": \"SysPrec\"}");

                // Envia mensagem
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [", vetor, "]}");
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
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "SysPrec", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
            return 0;
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
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<USUARIO> CarregaUsuarioAdm()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<AGENDA> CarregaAgenda(Int32 usuario)
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TAREFA> CarregaTarefa(Int32 usuario)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TAREFA> conf = new List<TAREFA>();
                if (Session["TarefasLista"] == null)
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
                        conf = (List<TAREFA>)Session["TarefasLista"];
                    }
                }
                Session["TarefasLista"] = conf;
                Session["TarefaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<UF> CarregaUF()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<UF> conf = new List<UF>();
                if (Session["UF"] == null)
                {
                    conf = empApp.GetAllUF();
                }
                else
                {
                    conf = (List<UF>)Session["UF"];
                }
                Session["UF"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PLANO> CarregaPlano()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<NOTICIA> CarregaNoticia()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<NOTICIA> CarregaNoticiaGeral()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<NOTIFICACAO> CarregaNotificacao()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<GRUPO> CarregaGrupo()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TEMPLATE_PROPOSTA> CarregaTemplateProposta()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TEMPLATE_EMAIL> CarregaTemplateEMail()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TEMPLATE_SMS> CarregaTemplateSMS()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TIPO_PESSOA> CarregaTipoPessoa()
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
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
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult VerNotificacaoBase(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaNotificacao"] = 10;
            return RedirectToAction("VerNotificacao", "Notificacao", new { id = id });
        }

        public ActionResult VerAgendaBase(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaAgenda"] = 10;
            Session["VoltaAgendaCRM"] = 0;
            return RedirectToAction("EditarAgenda", "Agenda", new { id = id });
        }

        public ActionResult VerTarefaBase(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTarefa"] = 10;
            Session["VoltaAgendaCRM"] = 0;
            return RedirectToAction("EditarTarefa", "Tarefa", new { id = id });
        }

        [HttpGet]
        public ActionResult MontarTelaMensagensEnviadas()
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

                // Carrega listas
                listaMasterEnviada = CarregaMensagensEnviadasHoje();
                Session["ListaMensagensEnviadas"] = listaMasterEnviada;

                // Monta demais listas
                List<USUARIO> listaUsu = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaUsu, "USUA_CD_ID", "USUA_NM_NOME");
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "SMS", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> escopo = new List<SelectListItem>();
                escopo.Add(new SelectListItem() { Text = "Cliente", Value = "1" });
                escopo.Add(new SelectListItem() { Text = "Usuário", Value = "2" });
                escopo.Add(new SelectListItem() { Text = "CRM", Value = "3" });
                ViewBag.Escopos = new SelectList(escopo, "Value", "Text");
                ViewBag.TipoCarga = (Int32)Session["TipoCargaMsgInt"];
                Session["VoltaTela"] = 0;

                // Restringe pelo perfil
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                List<MENSAGENS_ENVIADAS_SISTEMA> listaMens = ((List<MENSAGENS_ENVIADAS_SISTEMA>)Session["ListaMensagensEnviadas"]).ToList();
                if((String)Session["PerfilSigla"] == "ADM" || (String)Session["PerfilSigla"] == "GER")
                {
                    listaMens = listaMens.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                ViewBag.Listas = listaMens;

                // Indicadores
                ViewBag.Mensagens = listaMens.Count;
                ViewBag.EMails = listaMens.Where(p => p.MEEN_IN_TIPO == 1).Count();
                ViewBag.SMS = listaMens.Where(p => p.MEEN_IN_TIPO == 2).Count();
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
                    if ((Int32)Session["MensEnviada"] == 3)
                    {
                        String mens = CRMSys_Base.ResourceManager.GetString("M0279", CultureInfo.CurrentCulture) + " - ID: " + (String)Session["GUID"];
                        ModelState.AddModelError("", mens);
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }


        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagensEnviadas()
        {
            try
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
                Session["MensagemEnviadaAlterada"] = 0;
                Session["MensagensEnviadas"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagensEnviadasHoje()
        {
            try
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
                Session["MensagemEnviadaAlterada"] = 0;
                Session["MensagensEnviadas"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult RetirarFiltroMensagensEnviadas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["MensagemEnviadaAlterada"] = 1;
                List<MENSAGENS_ENVIADAS_SISTEMA> lm = CarregaMensagensEnviadasHoje();
                if ((String)Session["PerfilUsuario"] != "GER" & Session["PerfilUsuario"] != "ADM")
                {
                    lm = lm.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagensEnviadas"] = lm;
                Session["FiltroMensagensEnviadas"] = null;
                Session["TipoCargaMsgInt"] = 1;
                return RedirectToAction("MontarTelaMensagensEnviadas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult MostrarTodasMensagensEnviadas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["MensagemEnviadaAlterada"] = 1;
                List<MENSAGENS_ENVIADAS_SISTEMA> lm = CarregaMensagensEnviadas();
                if ((String)Session["PerfilUsuario"] != "GER" & Session["PerfilUsuario"] != "ADM")
                {
                    lm = lm.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagensEnviadas"] = lm;
                Session["FiltroMensagensEnviadas"] = null;
                Session["TipoCargaMsgInt"] = 2;
                return RedirectToAction("MontarTelaMensagensEnviadas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public ActionResult FiltrarMensagensEnviadas(MENSAGENS_ENVIADAS_SISTEMA item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

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
                if ((String)Session["PerfilUsuario"] != "GER" & Session["PerfilUsuario"] != "ADM")
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens Enviadas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMensagemEnviada(Int32 id)
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

                // Prepara view
                MENSAGENS_ENVIADAS_SISTEMA item = envApp.GetItemById(id);
                MensagemEmitidaViewModel vm = Mapper.Map<MENSAGENS_ENVIADAS_SISTEMA, MensagemEmitidaViewModel>(item);
                Session["MensagemEnviada"] = item;
                Session["IdVolta"] = id;
                Session["IdFilial"] = id;
                return View(vm);
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens Enviadas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 flag = (Int32)Session["FlagMensagensEnviadas"];
            
            
            if ((Int32)Session["FlagMensagensEnviadas"] == 11)
            {
                return RedirectToAction("MontarTelaCliente", "Cliente");
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
            if ((Int32)Session["FlagMensagensEnviadas"] == 10)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
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

        public List<EMPRESA> CarregaEmpresa()
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

        public JsonResult GetDadosGraficoEmail()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaEMail"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasEMail"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                Int32 contaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item  & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoSMS()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMS"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMS"];
            List<MENSAGENS_DESTINOS> listaDia = new List<MENSAGENS_DESTINOS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                Int32 contaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoSMSTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMSTudo"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMSTudo"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public Int32 CarregaNumeroCRM()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Recupera listas
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                Session["Limite"] = limite;
                List<CRM> lt = CarregaCRM().Where(p => p.CRM1_DT_CRIACAO > limite).ToList();
                List<CRM> lm = lt.Where(p => p.CRM1_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.CRM1_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
                List<CRM> la = lt.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
                List<CRM> lq = lt.Where(p => p.CRM1_IN_ATIVO == 2).ToList();
                List<CRM> ls = lt.Where(p => p.CRM1_IN_ATIVO == 5).ToList();
                List<CRM> lc = lt.Where(p => p.CRM1_IN_ATIVO == 3).ToList();
                List<CRM> lf = lt.Where(p => p.CRM1_IN_ATIVO == 4).ToList();
                List<CRM> lx = lt.Where(p => p.CRM1_IN_ATIVO == 6).ToList();
                List<CRM> ly = lt.Where(p => p.CRM1_IN_ATIVO == 7).ToList();
                List<CRM> lz = lt.Where(p => p.CRM1_IN_ATIVO == 8).ToList();

                List<CRM_ACAO> acoes = crmApp.GetAllAcoes(idAss).Where(p => p.CRM.CRM1_IN_ATIVO != 2 & p.CRAC_DT_CRIACAO > limite).ToList();
                List<CRM_ACAO> acoesPend = acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList();
                List<CRM_ACAO> acoesAtraso = acoes.Where(p => p.CRAC_IN_STATUS == 1 & p.CRAC_DT_PREVISTA.Value.Date < DateTime.Today.Date).ToList();

                List<CRM_FOLLOW> follows = crmApp.GetAllFollow(idAss).Where(p => p.CRM.CRM1_IN_ATIVO != 2 & p.CRFL_DT_FOLLOW > limite).ToList();
                List<CRM_FOLLOW> follows_Lembra = follows.Where(p => p.TIFL_CD_ID == 3 & p.CRFL_DT_PREVISTA.Value.Date >= DateTime.Today.Date).ToList();
                List<CRM_FOLLOW> follows_Alerta = follows.Where(p => p.TIFL_CD_ID == 2 & p.CRFL_DT_PREVISTA.Value.Date >= DateTime.Today.Date).ToList();

                List<CLIENTE> cli = CarregaCliente();
                List<CRM_PEDIDO_VENDA> peds = CarregaPedidoVenda().Where(p => p.CRM.CRM1_IN_ATIVO != 2).ToList();
                List<CRM_PEDIDO_VENDA> lmp1 = peds.Where(p => p.CRPV_DT_PEDIDO.Month == DateTime.Today.Date.Month & p.CRPV_DT_PEDIDO.Year == DateTime.Today.Date.Year).ToList();
                ViewBag.PropAtiva = peds.Count;

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
                Session["ListaCRMAcoesAtraso"] = acoesAtraso;
                Session["ListaPedidosMes"] = lmp1;
                Session["ListaCRMFollowsLembra"] = follows_Lembra;
                Session["ListaCRMFollowsAlerta"] = follows_Alerta;

                Session["CRMAtivos"] = la.Count;
                Session["CRMArquivados"] = lq.Count;
                Session["CRMCancelados"] = lc.Count;
                Session["CRMFalhados"] = lf.Count;
                Session["CRMSucessos"] = la.Count;
                Session["CRMFatura"] = lx.Count;
                Session["CRMExpedicao"] = ly.Count;
                Session["CRMEntregue"] = lz.Count;

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
                Session["ListaCRMBase"] = lt;
                Session["ListaPedidoBase"] = peds;
                Session["ListaAcoesBase"] = acoes;
                Session["ListaFunilBase"] = funs;
                return 0;
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
                return 0;
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
                    conf = crmApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["CRMAlterada"] == 1)
                    {
                        conf = crmApp.GetAllItens(idAss);
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
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Importação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Importação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                    conf = crmApp.GetAllPedidos(idAss);
                }
                else
                {
                    if ((Int32)Session["PedidoVendaAlterada"] == 1)
                    {
                        conf = crmApp.GetAllPedidos(idAss);
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
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Importação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM_ACAO> CarregaAcao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_ACAO> conf = new List<CRM_ACAO>();
                if (Session["CRMAcoes"] == null)
                {
                    conf = crmApp.GetAllAcoes(idAss);
                }
                else
                {
                    if ((Int32)Session["CRMAcaoAlterada"] == 1)
                    {
                        conf = crmApp.GetAllAcoes(idAss);
                    }
                    else
                    {
                        conf = (List<CRM_ACAO>)Session["CRMAcoes"];
                    }
                }
                Session["CRMAcoes"] = conf;
                Session["CRMAcaoAlterada"] = 0;
                return conf;
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
                return null;
            }
        }

        [HttpGet]
        public ActionResult MontarTelaFaleConosco()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Configuração
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

                // Mensagem
                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0256", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Monta objeto
                Session["MensFC"] = 0;
                FaleConoscoViewModel fale = new FaleConoscoViewModel();
                fale.Telefone = conf.CONF_NR_SUPORTE_ZAP;
                fale.EMail = conf.CONF_EM_CRMSYS;
                fale.Resposta = usuario.USUA_NM_EMAIL;
                fale.Nome = usuario.USUA_NM_NOME;

                List<SelectListItem> assunto = new List<SelectListItem>();
                assunto.Add(new SelectListItem() { Text = "Sugestões", Value = "1" });
                assunto.Add(new SelectListItem() { Text = "Informações", Value = "2" });
                assunto.Add(new SelectListItem() { Text = "Reclamações", Value = "3" });
                assunto.Add(new SelectListItem() { Text = "Suporte Técnico", Value = "4" });
                assunto.Add(new SelectListItem() { Text = "Outros Assuntos", Value = "5" });
                ViewBag.Assunto = new SelectList(assunto, "Value", "Text");
                fale.Assunto = null;
                return View(fale);
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
                return RedirectToAction("MontarTelaFaleConosco");
            }
        }

        [HttpPost]
        public async Task<ActionResult> MontarTelaFaleConosco(FaleConoscoViewModel vm)
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
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    List<SelectListItem> assunto = new List<SelectListItem>();
                    assunto.Add(new SelectListItem() { Text = "Sugestões", Value = "1" });
                    assunto.Add(new SelectListItem() { Text = "Informações", Value = "2" });
                    assunto.Add(new SelectListItem() { Text = "Reclamações", Value = "3" });
                    assunto.Add(new SelectListItem() { Text = "Suporte Técnico", Value = "4" });
                    assunto.Add(new SelectListItem() { Text = "Outros Assuntos", Value = "5" });
                    ViewBag.Assunto = new SelectList(assunto, "Value", "Text");

                    // Executa a operação
                    if (vm.Mensagem != null)
                    {
                        // Valida informações
                        if (vm.Assunto == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0275", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Decodifica assunto
                        String assuntoDesc = vm.Assunto == 1 ? "Sugestões" : (vm.Assunto == 2 ? "Informações" : (vm.Assunto == 3 ? "Reclamações" : (vm.Assunto == 4 ? "Suporte Técnico" : "Outros Assuntos")));

                        // Prepara mensagem
                        MensagemViewModel mens = new MensagemViewModel();
                        mens.NOME = vm.Nome;
                        mens.ID = usuario.USUA_CD_ID;
                        mens.MODELO = vm.EMail;
                        mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                        mens.MENS_IN_TIPO = 1;
                        mens.MENS_TX_TEXTO = vm.Mensagem;
                        mens.MENS_NM_LINK = null;
                        mens.MENS_NM_NOME = "Solicitação Fale Conosco";
                        mens.MENS_NM_CAMPANHA = assuntoDesc;
                        await ProcessaEnvioEMailFaleConosco(mens, usuario);
                    }

                    // Sucesso
                    return RedirectToAction("MontarTelaFaleConosco");
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
                    return RedirectToAction("MontarTelaFaleConosco");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailFaleConosco(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = usuario.ASSI_CD_ID;

            // Prepara corpo do e-mail e trata link
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            corpo = corpo.Replace("\r\n", "<br />");

            // Monta mensagem
            ASSINANTE assi = assiApp.GetItemById(idAss);
            corpo = corpo + "<b style='color:darkblue'>Assinante:</b> " + assi.ASSI_NM_NOME + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Num.Assinante:</b> " + assi.ASSI_CD_ID.ToString() + "<br />";
            corpo = corpo + "<b style='color:darkblue'>CPF/CNPJ:</b> " + (assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ) + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Data Assinatura:</b> " + assi.ASSI_DT_INICIO.Value.ToShortDateString() + "<br />";

            String status = "Succeeded";
            String iD = "xyz";
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA;
            mensagem.CORPO = corpo;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MODELO;
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
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult MontarTelaSobre()
        {
            // Verifica se tem usuario logado
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Configuração
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Monta timeline
            List<ModeloViewModel> mods = new List<ModeloViewModel>();
            ModeloViewModel mod = new ModeloViewModel();
            mod.DataEmissao = Convert.ToDateTime("01/09/2023");
            mod.Nome = "Versão 1.1.1.0";
            mod.Valor = 1;
            mod.Nome1 = "Melhorias Gerais";
            mods.Add(mod);
            mod = new ModeloViewModel();
            mod.DataEmissao = Convert.ToDateTime("10/09/2023");
            mod.Nome = "Versão 1.1.2.0";
            mod.Valor = 1;
            mod.Nome1 = "Melhorias Gerais";
            mods.Add(mod);
            mod = new ModeloViewModel();
            mod.DataEmissao = Convert.ToDateTime("15/09/2023");
            mod.Nome = "Versão 1.1.3.0";
            mod.Valor = 1;
            mod.Nome1 = "Melhorias Gerais";
            mods.Add(mod);
            mod = new ModeloViewModel();
            mod.DataEmissao = Convert.ToDateTime("25/09/2023");
            mod.Nome = "Versão 1.2.0.0";
            mod.Valor = 1;
            mod.Nome1 = "Melhorias Gerais";
            mods.Add(mod);
            ViewBag.Time = mods;

            // Monta objeto
            return View();
        }

        public List<CRM_FOLLOW> CarregaCRMFollow()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CRM_FOLLOW> conf = new List<CRM_FOLLOW>();
            conf = crmApp.GetAllFollow(idAss);
            Session["CRMFollow"] = conf;
            return conf;
        }


        public List<NOTIFICACAO> CarregaNotificacao(Int32 id)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTIFICACAO> conf = new List<NOTIFICACAO>();
                if (Session["NotificacoesUsuario"] == null)
                {
                    conf = notfApp.GetAllItensUser(id, idAss);
                }
                else
                {
                    if ((Int32)Session["MaquinaAlterada"] == 1)
                    {
                        conf = notfApp.GetAllItensUser(id, idAss);
                    }
                    else
                    {
                        conf = (List<NOTIFICACAO>)Session["NotificacoesUsuario"];
                    }
                }
                Session["NotificacoesUsuario"] = conf;
                Session["NotificacaoUsuarioAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens Enviadas", "SysPrec", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

    }
}