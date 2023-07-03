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

namespace ERP_Condominios_Solution.Controllers
{
    public class PesquisaController : Controller
    {
        private readonly IPesquisaAppService baseApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IClienteAppService cliApp;
        private readonly IUsuarioAppService usuApp;
        private readonly ITipoPesquisaAppService tpApp;
        private readonly ITipoItemPesquisaAppService tiApp;
        private readonly ITemplateEMailAppService temaApp;
        private readonly IGrupoAppService gruApp;

        private String msg;
        private Exception exception;
        PESQUISA objeto = new PESQUISA();
        PESQUISA objetoAntes = new PESQUISA();
        List<PESQUISA> listaMaster = new List<PESQUISA>();
        String extensao;

        public PesquisaController(IPesquisaAppService baseApps, IConfiguracaoAppService confApps, IClienteAppService cliApps, IUsuarioAppService usuApps, ITipoPesquisaAppService tpApps, ITipoItemPesquisaAppService tiApps, ITemplateEMailAppService temaApps, IGrupoAppService gruApps)
        {
            baseApp = baseApps;
            confApp = confApps;
            cliApp = cliApps;
            usuApp = usuApps;
            tpApp = tpApps;
            tiApp = tiApps;
            temaApp = temaApps;
            gruApp = gruApps;
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

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        private int div(int x)
        {
            return x / 0;
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 v = (Int32)Session["VoltaMensagem"];
            return RedirectToAction("MontarTelaDashboardPesquisa");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> clientes = CarregaCliente();
            Session["Clientes"] = clientes;

            if (nome != null)
            {
                List<CLIENTE> lstCliente = clientes.Where(x => x.CLIE_NM_NOME != null && x.CLIE_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = clientes.Where(x => x.CLIE_NM_RAZAO != null).ToList<CLIENTE>();
                    lstCliente = lstCliente.Where(x => x.CLIE_NM_RAZAO.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.CLIE_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.CLIE_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.CLIE_NM_NOME + " (" + item.CLIE_NM_RAZAO + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }

            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardPesquisa()
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas
            List<CLIENTE> lc = CarregaCliente();
            List<PESQUISA> lt = CarregaPesquisa();
            List<PESQUISA> lm = lt.Where(p => p.PESQ_DT_CRIACAO.Month == DateTime.Today.Date.Month & p.PESQ_DT_CRIACAO.Year == DateTime.Today.Date.Year).ToList();

            // Estatisticas 
            ViewBag.TotalCliente = lc.Count;
            ViewBag.Total = lt.Count;
            ViewBag.TotalPes = lt.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;

            Session["ListaPesquisa"] = lt;
            Session["ListaPesquisaMes"] = lm;
            Session["IdPesquisa"] = null;

            // Resumo Mes Pesquisas
            List<DateTime> datas = lm.Select(p => p.PESQ_DT_CRIACAO.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = lm.Where(p => p.PESQ_DT_CRIACAO.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaPesquisaMes = lista;
            ViewBag.ContaPesquisaMes = lm.Count;
            Session["ListaDatasPesquisa"] = datas;
            Session["ListaPesquisaMesResumo"] = lista;

            // Recupera pesquisas por Tipo
            List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
            List<TIPO_PESQUISA> catc = CarregaTipoPesquisa();
            foreach (TIPO_PESQUISA item in catc)
            {
                Int32 num = lt.Where(p => p.TIPS_CD_ID == item.TIPS_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.TIPS_NM_NOME;
                    mod.Valor = num;
                    lista7.Add(mod);
                }
            }
            ViewBag.ListaPesquisaCats = lista7;
            Session["ListaPesquisaCats"] = lista7;
            Session["FlagAlteraPesquisa"] = 0;
            return View(vm);
        }

        public JsonResult GetDadosGraficoPesquisa()
        {
            List<PESQUISA> listaCP1 = (List<PESQUISA>)Session["ListaPesquisaMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasPesquisa"];
            List<PESQUISA> listaDia = new List<PESQUISA>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.PESQ_DT_CRIACAO.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosPesquisaTipo()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPesquisaCats"];
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

        [HttpGet]
        public ActionResult MontarTelaPesquisa()
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
            if ((List<PESQUISA>)Session["ListaPesquisa"] == null)
            {
                listaMaster = CarregaPesquisa();
                Session["ListaPesquisa"] = listaMaster;
            }

            // Monta demais listas
            ViewBag.Listas = (List<PESQUISA>)Session["ListaPesquisa"];
            ViewBag.Title = "Pesquisa";
            ViewBag.Tipos = new SelectList(CarregaTipoPesquisa(), "TIPS_CD_ID", "TIPS_NM_NOME");
            Session["VoltaTelaPesq"] = 0;
            ViewBag.Incluir = (Int32)Session["VoltaTelaPesq"];

            // Indicadores
            List<PESQUISA> lp = (List<PESQUISA>)Session["ListaPesquisa"];
            ViewBag.Pesquisas = lp.Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Ativas = lp.Where(p => p.PESQ_DT_VALIDADE.Date >= DateTime.Today.Date).ToList().Count();
            ViewBag.Inativas = lp.Where(p => p.PESQ_DT_VALIDADE.Date < DateTime.Today.Date).ToList().Count();

            List<PESQUISA_RESPOSTA> resp = baseApp.GetAllRespostas(idAss).ToList();
            List<Int32> pesqResp = resp.Select(p => p.PESQ_CD_ID).Distinct().ToList();
            ViewBag.Respostas = resp.Count;
            ViewBag.PesqRespondidas = pesqResp.Count;

            // Mensagem
            if (Session["MensPesquisa"] != null)
            {
                if ((Int32)Session["MensPesquisa"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPesquisa"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPesquisa"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0181", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPesquisa"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0232", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPesquisa"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0230", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPesquisa"] == 55)
                {
                    String mens = "Foram processadas " + ((Int32)Session["ContaRespostas"]).ToString() + " respostas na pesquisa ";
                    ModelState.AddModelError("", mens);
                }
                if ((Int32)Session["MensPesquisa"] == 99)
                {
                    String mens = "A pesquisa " + (String)Session["PesquisaJa"] + " já foi respondida pelo usuário " + usuario.USUA_NM_NOME;
                    ModelState.AddModelError("", mens);
                }
            }

            // Abre view
            Session["EnviaPesquisa"] = 0;
            Session["VoltaTela"] = 0;
            Session["FlagAlteraPesquisa"] = 0;
            Session["MensPesquisa"] = 0;
            Session["VoltaEnvio"] = 0;
            Session["ListaPerguntas"] = null;
            Session["PerguntaAtual"] = null;
            objeto = new PESQUISA();
            if (Session["FiltroPesquisa"] != null)
            {
                objeto = (PESQUISA)Session["FiltroPesquisa"];
            }
            objeto.PESQ_IN_ATIVO = 1;
            return View(objeto);
        }

        public ActionResult RetirarFiltroPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaPesquisa"] = null;
            Session["FiltroPesquisa"] = null;
            return RedirectToAction("MontarTelaPesquisa");
        }

        public ActionResult MostrarTudoPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregaPesquisaAdm();
            Session["FiltroPesquisa"] = null;
            Session["ListaPesquisa"] = listaMaster;
            return RedirectToAction("MontarTelaPesquisa");
        }

        [HttpPost]
        public ActionResult FiltrarPesquisa(PESQUISA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CLIENTE> listaObj = new List<CLIENTE>();
                Session["FiltroPesquisa"] = item;
                Tuple < Int32, List<PESQUISA>, Boolean > volta = baseApp.ExecuteFilterTuple(item.TIPS_CD_ID, item.PESQ_NM_NOME, item.PESQ_DS_DESCRICAO, item.PESQ_NM_CAMPANHA, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPesquisa"] = 1;
                    return RedirectToAction("MontarTelaPesquisa");
                }

                // Sucesso
                listaMaster = volta.Item2;
                Session["ListaPesquisa"]  = volta.Item2;
                return RedirectToAction("MontarTelaPesquisa");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBasePesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPesquisa");
        }

       [HttpGet]
        public ActionResult IncluirPesquisa()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPesquisa"] = 2;
                    return RedirectToAction("MontarTelaPesquisa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = CarregaPesquisa().Count;
            if ((Int32)Session["NumPesquisas"] <= num)
            {
                Session["MensPesquisa"] = 50;
                return RedirectToAction("MontarTelaPesquisa", "Pesquisa");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(CarregaTipoPesquisa(), "TIPS_CD_ID", "TIPS_NM_NOME");

            // Prepara objeto
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            Session["PesquisaNovo"] = 0;
            PESQUISA item = new PESQUISA();
            PesquisaViewModel vm = Mapper.Map<PESQUISA, PesquisaViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.PESQ_DT_CRIACAO = DateTime.Today.Date;
            vm.PESQ_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.PESQ_DT_VALIDADE = DateTime.Today.Date.AddDays(conf.CONF_NR_VALIDADE_PESQUISA.Value);
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirPesquisa(PesquisaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(CarregaTipoPesquisa(), "TIPS_CD_ID", "TIPS_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PESQUISA item = Mapper.Map<PesquisaViewModel, PESQUISA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPesquisa"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0231", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensPesquisa"] = 10;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0237", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Pesquisas/" + item.PESQ_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Atualiza cache
                    listaMaster = new List<PESQUISA>();
                    Session["ListaPesquisa"] = null;
                    Session["IncluirPesquisa"] = 1;
                    Session["PesquisaNovo"] = item.PESQ_CD_ID;
                    Session["Pesquisas"] = CarregaPesquisa();
                    Session["IdVolta"] = item.PESQ_CD_ID;
                    Session["IdPesquisa"] = item.PESQ_CD_ID;
                    Session["PesquisaAlterada"] = 1;

                    // Trata anexos
                    if (Session["FileQueuePesquisa"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueuePesquisa"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueuePesquisa(file);
                            }
                        }
                        Session["FileQueuePesquisa"] = null;
                    }

                    // Trata retorno
                    return RedirectToAction("VoltarAnexoPesquisa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarPesquisa(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPesquisa"] = 2;
                    return RedirectToAction("MontarTelaPesquisa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(CarregaTipoPesquisa(), "TIPS_CD_ID", "TIPS_NM_NOME");

            // Carrega pesquisa
            PESQUISA item = baseApp.GetItemById(id);

            // Indicadores
            List<PESQUISA_ITEM> pergs = item.PESQUISA_ITEM.ToList();
            List<PESQUISA_RESPOSTA> resps = item.PESQUISA_RESPOSTA.ToList();
            Int32 pesqResp = resps.Where(p => p.CLIE_CD_ID != null).Select(p => p.CLIE_CD_ID).Distinct().Count();  

            ViewBag.Pergs = pergs;
            ViewBag.Resps = resps;
            ViewBag.PesqResp = pesqResp;

            List<PESQUISA_RESPOSTA> resp30 = new List<PESQUISA_RESPOSTA>();
            ViewBag.ListaRespostas = resp30;

            // Trata mensagens
            if (Session["MensPesquisa"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPesquisa"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPesquisa"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPesquisa"] == 66)
                {
                    String msg = "A pesquisa " + item.PESQ_NM_NOME + " teve suas informações alteradas.";                 
                    ModelState.AddModelError("", msg);
                }
                if ((Int32)Session["MensPesquisa"] == 90)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0238", CultureInfo.CurrentCulture));
                }
            }

            // Carrega view
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            Session["VoltaPesquisa"] = 1;
            objetoAntes = item;
            Session["Pesquisa"] = item;
            Session["IdPesquisa"] = id;
            Session["IdVolta"] = id;
            Session["EnviaPesquisa"] = 0;
            Session["VoltaEnvio"] = 1;
            PesquisaViewModel vm = Mapper.Map<PESQUISA, PesquisaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarPesquisa(PesquisaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagAlteraPesquisa"] = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PESQUISA item = Mapper.Map<PesquisaViewModel, PESQUISA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Retoeno
                    if (volta == 2)
                    {
                        Session["MensPesquisa"] = 10;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0237", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Verifica se não houve alteração
                    if (item == objetoAntes)
                    {
                        Session["FlagAlteraPesquisa"] = 0;
                    }

                    // Atualiza cache
                    listaMaster = new List<PESQUISA>();
                    Session["ListaPesquisa"] = null;
                    Session["IncluirPesquisa"] = 0;
                    Session["PesquisaAlterada"] = 1;

                    // Trata retorno

                    // Retorno normal
                    Session["MensPesquisa"] = 66;
                    return RedirectToAction("VoltarAnexoPesquisa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirPesquisa(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                //Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPesquisa"] = 2;
                    return RedirectToAction("MontarTelaPesquisa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar exclusão
            try
            {
                PESQUISA item = baseApp.GetItemById(id);
                objetoAntes = (PESQUISA)Session["Pesquisa"];
                item.PESQ_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensPesquisa"] = 4;
                    return RedirectToAction("MontarTelaPesquisa", "Pesquisa");
                }
                listaMaster = new List<PESQUISA>();
                Session["ListaPesquisa"] = null;
                Session["FiltroPesquisa"] = null;
                Session["PesquisaAlterada"] = 1;
                return RedirectToAction("MontarTelaPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarPesquisa(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                //Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPesquisa"] = 2;
                    return RedirectToAction("MontarTelaPesquisa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = CarregaPesquisa().Count;
            if ((Int32)Session["NumPesquisas"] <= num)
            {
                Session["MensPesquisa"] = 50;
                return RedirectToAction("MontarTelaPesquisa", "Pesquisa");
            }

            // Executar reativação
            try
            {
                PESQUISA item = baseApp.GetItemById(id);
                objetoAntes = (PESQUISA)Session["Pesquisa"];
                item.PESQ_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                listaMaster = new List<PESQUISA>();
                Session["ListaPesquisa"] = null;
                Session["FiltroPesquisa"] = null;
                Session["PesquisaAlterada"] = 1;
                return RedirectToAction("MontarTelaPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
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
            Session["FileQueuePesquisa"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueuePesquisa(FileQueue file)
        {
            // Inicializa
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdPesquisa"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensPesquisa"] = 5;
                return RedirectToAction("VoltarAnexoPesquisa");
            }

            // Recupera cliente
            PESQUISA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 50)
            {
                Session["MensPesquisa"] = 6;
                return RedirectToAction("VoltarAnexoPesquisa");
            }

            // Copia arquivo para pasta
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pesquisas/" + item.PESQ_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                PESQUISA_ANEXO foto = new PESQUISA_ANEXO();
                foto.PEAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PEAN_DT_ANEXO = DateTime.Today;
                foto.PEAN_IN_ATIVO = 1;
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
                foto.PEAN_IN_TIPO = tipo;
                foto.PEAN_NM_TITULO = fileName;
                foto.PESQ_CD_ID = item.PESQ_CD_ID;

                item.PESQUISA_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                return RedirectToAction("VoltarAnexoPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFilePesquisa(HttpPostedFileBase file)
        {
            // Inicializa
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdPesquisa"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensPesquisa"] = 5;
                return RedirectToAction("VoltarAnexoPesquisa");
            }

            // Recupera cliente
            PESQUISA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensPesquisa"] = 6;
                return RedirectToAction("VoltarAnexoPesquisa");
            }

            // Copia arquivo
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pesquisas/" + item.PESQ_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                PESQUISA_ANEXO foto = new PESQUISA_ANEXO();
                foto.PEAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PEAN_DT_ANEXO = DateTime.Today;
                foto.PEAN_IN_ATIVO = 1;
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
                foto.PEAN_IN_TIPO = tipo;
                foto.PEAN_NM_TITULO = fileName;
                foto.PESQ_CD_ID = item.PESQ_CD_ID;

                item.PESQUISA_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAnexoPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarPesquisa", new { id = (Int32)Session["IdPesquisa"] });
        }

        public ActionResult VoltarAnexoPerguntaPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarPerguntaPesquisa", new { id = (Int32)Session["IdPergunta"] });
        }

        public ActionResult VoltarAnexoPesquisa1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("EditarPesquisa", new { id = (Int32)Session["IdPesquisa"] });
        }

        public List<PESQUISA> CarregaPesquisa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PESQUISA> conf = new List<PESQUISA>();
            if (Session["Pesquisas"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["PesquisaAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<PESQUISA>)Session["Pesquisas"];
                }
            }
            Session["Pesquisas"] = conf;
            Session["PesquisaAlterada"] = 0;
            return conf;
        }

        public List<PESQUISA_RESPOSTA> CarregaRespostas()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PESQUISA_RESPOSTA> conf = new List<PESQUISA_RESPOSTA>();
            if (Session["Respostas"] == null)
            {
                conf = baseApp.GetAllRespostas(idAss);
            }
            else
            {
                if ((Int32)Session["RespostasAlterada"] == 1)
                {
                    conf = baseApp.GetAllRespostas(idAss);
                }
                else
                {
                    conf = (List<PESQUISA_RESPOSTA>)Session["Respostas"];
                }
            }
            Session["Respostas"] = conf;
            Session["RespostasAlterada"] = 0;
            return conf;
        }

        public List<PESQUISA> CarregaPesquisaAdm()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PESQUISA> conf = new List<PESQUISA>();
            if (Session["PesquisasAdm"] == null)
            {
                conf = baseApp.GetAllItensAdm(idAss);
            }
            else
            {
                if ((Int32)Session["PesquisaAlterada"] == 1)
                {
                    conf = baseApp.GetAllItensAdm(idAss);
                }
                else
                {
                    conf = (List<PESQUISA>)Session["PesquisasAdm"];
                }
            }
            Session["PesquisasAdm"] = conf;
            Session["PesquisaAlterada"] = 0;
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

        public List<TIPO_PESQUISA> CarregaTipoPesquisa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_PESQUISA> conf = new List<TIPO_PESQUISA>();
            if (Session["TipoPesquisas"] == null)
            {
                conf = tpApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["TipoPesquisaAlterada"] == 1)
                {
                    conf = tpApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TIPO_PESQUISA>)Session["TipoPesquisas"];
                }
            }
            Session["TipoPesquisas"] = conf;
            Session["TipoPesquisaAlterada"] = 0;
            return conf;
        }

        public List<TIPO_ITEM_PESQUISA> CarregaTipoItemPesquisa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_ITEM_PESQUISA> conf = new List<TIPO_ITEM_PESQUISA>();
            if (Session["TipoItemPesquisas"] == null)
            {
                conf = tiApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["TipoItemPesquisaAlterada"] == 1)
                {
                    conf = tiApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TIPO_ITEM_PESQUISA>)Session["TipoItemPesquisas"];
                }
            }
            Session["TipoItemPesquisas"] = conf;
            Session["TipoItemPesquisaAlterada"] = 0;
            return conf;
        }

        public FileResult DownloadPesquisa(Int32 id)
        {
            PESQUISA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.PEAN_AQ_ARQUIVO;
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
        public ActionResult VerAnexoPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            PESQUISA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoPesquisaAudio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            PESQUISA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult IncluirAnotacaoPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 3;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            PESQUISA item = baseApp.GetItemById((Int32)Session["IdPesquisa"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            PESQUISA_ANOTACAO coment = new PESQUISA_ANOTACAO();
            PesquisaAnotacaoViewModel vm = Mapper.Map<PESQUISA_ANOTACAO, PesquisaAnotacaoViewModel>(coment);
            vm.PECO_DT_ANOTACAO = DateTime.Now;
            vm.PECO_IN_ATIVO = 1;
            vm.PESQ_CD_ID = item.PESQ_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAnotacaoPesquisa(PesquisaAnotacaoViewModel vm)
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
                    PESQUISA_ANOTACAO item = Mapper.Map<PesquisaAnotacaoViewModel, PESQUISA_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PESQUISA not = baseApp.GetItemById((Int32)Session["IdPesquisa"]);

                    item.USUARIO = null;
                    not.PESQUISA_ANOTACAO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno
                    Session["PesquisaAlterada"] = 1;

                    // Sucesso
                    return RedirectToAction("EditarPesquisa", new { id = (Int32)Session["IdPesquisa"] });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAnotacaoPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            Session["VoltaTela"] = 3;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            PESQUISA_ANOTACAO item = baseApp.GetComentarioById(id);
            PESQUISA cli = baseApp.GetItemById(item.PESQ_CD_ID);
            PesquisaAnotacaoViewModel vm = Mapper.Map<PESQUISA_ANOTACAO, PesquisaAnotacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoPesquisa(PesquisaAnotacaoViewModel vm)
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
                    PESQUISA_ANOTACAO item = Mapper.Map<PesquisaAnotacaoViewModel, PESQUISA_ANOTACAO>(vm);
                    Int32 volta = baseApp.ValidateEditAnotacao(item);

                    // Verifica retorno
                    Session["PesquisaAlterada"] = 1;
                    return RedirectToAction("VoltarAnexoPesquisa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PESQUISA_ANOTACAO item = baseApp.GetComentarioById(id);
                item.PECO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnotacao(item);
                Session["PesquisaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarPerguntaPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(CarregaTipoItemPesquisa(), "TIIT_CD_ID", "TIIT_NM_NOME");
            List<SelectListItem> obrig = new List<SelectListItem>();
            obrig.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            obrig.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Obrigatorio = new SelectList(obrig, "Value", "Text");

            // Prepara view
            Session["VoltaTela"] = 2;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            PESQUISA_ITEM item = baseApp.GetItemPesquisaById(id);
            Session["IdPergunta"] = item.PEIT_CD_ID;
            Session["Pergunta"] = item;
            PesquisaItemViewModel vm = Mapper.Map<PESQUISA_ITEM, PesquisaItemViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPerguntaPesquisa(PesquisaItemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            ViewBag.Tipos = new SelectList(CarregaTipoItemPesquisa(), "TIIT_CD_ID", "TIIT_NM_NOME");
            List<SelectListItem> obrig = new List<SelectListItem>();
            obrig.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            obrig.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Obrigatorio = new SelectList(obrig, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PESQUISA pesq = (PESQUISA)Session["Pesquisa"];
                    PESQUISA_ITEM item = Mapper.Map<PesquisaItemViewModel, PESQUISA_ITEM>(vm);
                    Int32 volta = baseApp.ValidateEditItemPesquisa(item, pesq);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPesquisa"] = 91;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0239", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Encerra
                    Session["PesquisaAlterada"] = 1;
                    return RedirectToAction("EditarPerguntaPesquisa", new { id = (Int32)Session["IdPergunta"] });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirPerguntaPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                Session["VoltaTela"] = 2;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PESQUISA_ITEM item = baseApp.GetItemPesquisaById(id);
                item.PEIT_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDeleteItemPesquisa(item);
                if (volta == 1)
                {
                    Session["MensPesquisa"] = 90;
                    return RedirectToAction("VoltarAnexoPesquisa");
                }
                Session["PesquisaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarPerguntaPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                PESQUISA pesq = (PESQUISA)Session["Pesquisa"];
                Session["VoltaTela"] = 2;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PESQUISA_ITEM item = baseApp.GetItemPesquisaById(id);
                item.PEIT_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditItemPesquisa(item, pesq);
                Session["PesquisaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirPerguntaPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(CarregaTipoItemPesquisa(), "TIIT_CD_ID", "TIIT_NM_NOME");
            List<SelectListItem> obrig = new List<SelectListItem>();
            obrig.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            obrig.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Obrigatorio = new SelectList(obrig, "Value", "Text");

            // Recupera ultima ordem
            Int32 num = 0;
            PESQUISA pesq = baseApp.GetItemById((Int32)Session["IdPesquisa"]);
            Session["Pesquisa"] = pesq;
            Int32 numItem = pesq.PESQUISA_ITEM.Count;
            if (numItem == 0)
            {
                num = 1;
            }
            else
            {
                num = pesq.PESQUISA_ITEM.OrderByDescending(p => p.PEIT_IN_ORDEM).ToList().First().PEIT_IN_ORDEM;
                num++;
            }

            // Prepara view
            Session["VoltaTela"] = 2;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            PESQUISA_ITEM item = new PESQUISA_ITEM();
            PesquisaItemViewModel vm = Mapper.Map<PESQUISA_ITEM, PesquisaItemViewModel>(item);
            vm.PESQ_CD_ID = (Int32)Session["IdPesquisa"];
            vm.PEIT_IN_ATIVO = 1;
            vm.PEIT_IN_ORDEM = num;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirPerguntaPesquisa(PesquisaItemViewModel vm)
        {
            ViewBag.Tipos = new SelectList(CarregaTipoItemPesquisa(), "TIIT_CD_ID", "TIIT_NM_NOME");
            List<SelectListItem> obrig = new List<SelectListItem>();
            obrig.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            obrig.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Obrigatorio = new SelectList(obrig, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PESQUISA pesq = (PESQUISA)Session["Pesquisa"];
                    PESQUISA_ITEM item = Mapper.Map<PesquisaItemViewModel, PESQUISA_ITEM>(vm);
                    Int32 volta = baseApp.ValidateCreateItemPesquisa(item, pesq);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPesquisa"] = 91;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0239", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Encerra                    
                    Session["PesquisaAlterada"] = 1;
                    return RedirectToAction("EditarPerguntaPesquisa", new { id = item.PEIT_CD_ID });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirOpcaoPerguntaPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Recupera ultima ordem
            Int32? num = 0;
            PESQUISA_ITEM pesq = (PESQUISA_ITEM)Session["Pesquisa"];
            Session["Pergunta"] = pesq;
            Int32 numItem = pesq.PESQUISA_ITEM_OPCAO.Count;
            if (numItem == 0)
            {
                num = 1;
            }
            else
            {
                num = pesq.PESQUISA_ITEM_OPCAO.OrderByDescending(p => p.PEIO_IN_ORDEM).ToList().First().PEIO_IN_ORDEM;
                num++;
            }

            // Prepara view
            Session["VoltaTela"] = 2;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            PESQUISA_ITEM_OPCAO item = new PESQUISA_ITEM_OPCAO();
            PesquisaItemOpcaoViewModel vm = Mapper.Map<PESQUISA_ITEM_OPCAO, PesquisaItemOpcaoViewModel>(item);
            vm.PEIO_CD_ID = (Int32)Session["IdPergunta"];
            vm.PEIO_IN_ATIVO = 1;
            vm.PEIO_IN_ORDEM = num;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirOpcaoPerguntaPesquisa(PesquisaItemOpcaoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PESQUISA_ITEM pesq = (PESQUISA_ITEM)Session["Pesquisa"];
                    PESQUISA_ITEM_OPCAO item = Mapper.Map<PesquisaItemOpcaoViewModel, PESQUISA_ITEM_OPCAO>(vm);
                    Int32 volta = baseApp.ValidateCreateItemOpcaoPesquisa(item, pesq);

                    // Encerra                    
                    Session["PesquisaAlterada"] = 1;
                    return RedirectToAction("EditarPerguntaPesquisa", new { id = item.PEIT_CD_ID });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarOpcaoPerguntaPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Prepara view
            Session["VoltaTela"] = 2;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            PESQUISA_ITEM_OPCAO item = baseApp.GetItemOpcaoPesquisaById(id);
            Session["IdPergunta"] = item.PEIT_CD_ID;
            PesquisaItemOpcaoViewModel vm = Mapper.Map<PESQUISA_ITEM_OPCAO, PesquisaItemOpcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarOpcaoPerguntaPesquisa(PesquisaItemOpcaoViewModel vm)
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
                    PESQUISA_ITEM pesq = (PESQUISA_ITEM)Session["Pesquisa"];
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PESQUISA_ITEM_OPCAO item = Mapper.Map<PesquisaItemOpcaoViewModel, PESQUISA_ITEM_OPCAO>(vm);
                    Int32 volta = baseApp.ValidateEditItemOpcaoPesquisa(item, pesq);

                    // Verifica retorno

                    // Encerra
                    Session["PesquisaAlterada"] = 1;
                    return RedirectToAction("EditarPerguntaPesquisa", new { id = (Int32)Session["IdPergunta"] });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirOpcaoPerguntaPesquisa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                Session["VoltaTela"] = 2;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PESQUISA_ITEM_OPCAO item = baseApp.GetItemOpcaoPesquisaById(id);
                item.PEIO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDeleteItemOpcaoPesquisa(item);
                Session["PesquisaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EnviarPesquisa(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPesquisa"] = 2;
                    return RedirectToAction("MontarTelaPesquisa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Grupos = new SelectList(CarregaGrupo(), "GRUP_CD_ID", "GRUP_NM_NOME");
            List<TEMPLATE_EMAIL> mods = CarregaTemplateEMail().Where(p => p.TEEM_IN_PESQUISA == 1).ToList();
            ViewBag.Modelos = new SelectList(mods, "TEEM_CD_ID", "TEEM_NM_NOME");

            // Prepara objeto
            PESQUISA item = baseApp.GetItemById(id);
            PesquisaViewModel vm = Mapper.Map<PESQUISA, PesquisaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EnviarPesquisa(PesquisaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Grupos = new SelectList(CarregaGrupo(), "GRUP_CD_ID", "GRUP_NM_NOME");
            List<TEMPLATE_EMAIL> mods = CarregaTemplateEMail().Where(p => p.TEEM_IN_PESQUISA == 1).ToList();
            ViewBag.Modelos = new SelectList(mods, "TEEM_CD_ID", "TEEM_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PESQUISA item = Mapper.Map<PesquisaViewModel, PESQUISA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    String link = GerarLinkPesquisa(item);
                    item.PESQ_LK_LINK = link;
                    String msgErro = String.Empty;
                    Tuple < Int32, String, Boolean > volta = baseApp.ValidateEnviarPesquisa(item, usuario);

                    // Verifica retorno
                    if (volta.Item1 == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0240", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta.Item1 == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0241", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta.Item1 == 3)
                    {
                        msgErro = CRMSys_Base.ResourceManager.GetString("M0242", CultureInfo.CurrentCulture) + ". Código do Cliente: " + volta.Item2;
                        ModelState.AddModelError("", msgErro);
                        return View(vm);
                    }
                    if (volta.Item1 == 4)
                    {
                        msgErro = CRMSys_Base.ResourceManager.GetString("M0246", CultureInfo.CurrentCulture) + ". Código do Grupo: " + volta.Item2;
                        ModelState.AddModelError("", msgErro);
                        return View(vm);
                    }
                    if (volta.Item1 == 5)
                    {
                        msgErro = CRMSys_Base.ResourceManager.GetString("M0247", CultureInfo.CurrentCulture) + ". Código do Grupo: " + volta.Item2;
                        ModelState.AddModelError("", msgErro);
                        return View(vm);
                    }
                    if (volta.Item1 == 11)
                    {
                        msgErro = CRMSys_Base.ResourceManager.GetString("M0243", CultureInfo.CurrentCulture) + ". Código do Modelo: " + volta.Item2;
                        ModelState.AddModelError("", msgErro);
                        return View(vm);
                    }
                    if (volta.Item1 == 12)
                    {
                        msgErro = CRMSys_Base.ResourceManager.GetString("M0244", CultureInfo.CurrentCulture) + ". Erro : " + volta.Item2;
                        ModelState.AddModelError("", msgErro);
                        return View(vm);
                    }
                    if (volta.Item1 == 13)
                    {
                        msgErro = CRMSys_Base.ResourceManager.GetString("M0245", CultureInfo.CurrentCulture) + ". Erro : " + volta.Item2;
                        ModelState.AddModelError("", msgErro);
                        return View(vm);
                    }

                    // Atualiza cache
                    listaMaster = new List<PESQUISA>();
                    Session["ListaPesquisa"] = null;
                    Session["Pesquisas"] = CarregaPesquisa();
                    Session["IdVolta"] = item.PESQ_CD_ID;
                    Session["IdPesquisa"] = item.PESQ_CD_ID;
                    Session["PesquisaAlterada"] = 1;

                    // Trata retorno
                    if ((Int32)Session["VoltaEnvio"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoPesquisa");
                    }
                    return RedirectToAction("VoltarBasePesquisa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult EnviarPesquisaForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EnviarPesquisa", new { id = (Int32)Session["IdPesquisa"] });
        }

        public JsonResult PesquisaTemplateEMail(String temp)
        {
            // Recupera Template
            TEMPLATE_EMAIL tmp = temaApp.GetItemById(Convert.ToInt32(temp));

            // Atualiza
            var hash = new Hashtable();
            hash.Add("TEEM_TX_CORPO", tmp.TEEM_TX_CORPO);
            hash.Add("TEEM_TX_CABECALHO", tmp.TEEM_TX_CABECALHO);
            hash.Add("TEEM_TX_RODAPE", tmp.TEEM_TX_DADOS);

            // Retorna
            return Json(hash);
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

        [HttpGet]
        public ActionResult EnviarPesquisas(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            Session["EnviaPesquisa"] = 1;
            return RedirectToAction("EnviarPesquisa");
        }

        public List<TEMPLATE_EMAIL> CarregaTemplateEMail()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_EMAIL> conf = new List<TEMPLATE_EMAIL>();
            if (Session["TemplatesEMail"] == null)
            {
                conf = temaApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = temaApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TEMPLATE_EMAIL>)Session["TemplatesEMail"];
                }
            }
            Session["TemplatesEMail"] = conf;
            Session["TemplateEMailAlterada"] = 0;
            return conf;
        }

        [HttpGet]
        public String GerarLinkPesquisa(PESQUISA item)
        {
            // Prepara view
            String link = "www.globo.com";
            return link;
        }

        [HttpGet]
        public ActionResult ResponderPesquisa(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                //Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPesquisa"] = 2;
                    return RedirectToAction("MontarTelaPesquisa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar processo
            try
            {
                // Verifica se já foi respondida
                PESQUISA pesq = baseApp.GetItemById(id);
                Session["IdPesquisa"] = id;
                Session["Pesquisa"] = pesq;
                List<PESQUISA_RESPOSTA> resp = CarregaRespostas();
                resp = resp.Where(p => p.PESQ_CD_ID == id & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                if (resp.Count > 0)
                {
                    Session["PesquisaJa"] = pesq.PESQ_NM_NOME;
                    Session["MensPesquisa"] = 99;
                    RedirectToAction("MontarTelaPesquisa");
                }

                // Recupera Dados
                if (Session["PerguntaAtual"] == null)
                {
                    // Recupera Perguntas
                    List<PESQUISA_ITEM> itens = pesq.PESQUISA_ITEM.OrderBy(p => p.PEIT_IN_ORDEM).ToList();

                    // Pergunta atual
                    PESQUISA_ITEM item = itens.First();
                    Session["ListaPerguntas"] = itens;
                    Session["PerguntaAtual"] = item;
                    Session["OrdemAtual"] = item.PEIT_IN_ORDEM;
                    Session["UltimaOrdem"] = itens.Last().PEIT_IN_ORDEM;
                    ViewBag.Ordem = item.PEIT_IN_ORDEM;

                    // Cria coleção Resposta
                    List<PesquisaItemViewModel> respostas = new List<PesquisaItemViewModel>();
                    Session["Respostas"] = respostas;
                }

                // Monta view
                Session["EncerrouResposta"] = 0;
                PESQUISA_ITEM atual = (PESQUISA_ITEM)Session["PerguntaAtual"];
                PesquisaItemViewModel vm = Mapper.Map<PESQUISA_ITEM, PesquisaItemViewModel>(atual);
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult ResponderPesquisa(PesquisaItemViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica resposta
                    if (vm.PEIT_IN_OBRIGATORIA == 1 & vm.PEIT_DS_RESPOSTA_TEXTO == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0248", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Adiciona na coleção de respostas
                    List<PesquisaItemViewModel> respostas = (List<PesquisaItemViewModel>)Session["Respostas"];
                    respostas.Add(vm);
                    Session["Respostas"] = respostas;

                    // Recupera dados
                    List<PESQUISA_ITEM> itens = (List<PESQUISA_ITEM>)Session["ListaPerguntas"];
                    itens = itens.OrderBy(p => p.PEIT_IN_ORDEM).ToList();
                    Int32 atual = (Int32)Session["OrdemAtual"];
                    Int32 ultima = (Int32)Session["UltimaOrdem"];

                    // Calcula nova pergunta
                    Int32 nova = atual + 1;
                    PESQUISA_ITEM perg = itens.Where(p => p.PEIT_IN_ORDEM == nova).FirstOrDefault();
                    Int32 sai = 0;
                    while (sai == 0)
                    {
                        if (perg == null)
                        {
                            nova = nova++;
                            if (nova > ultima)
                            {
                                sai = 2;
                                continue;
                            }
                            else
                            {
                                perg = itens.Where(p => p.PEIT_IN_ORDEM == nova).FirstOrDefault();
                            }
                        }
                        sai = 1;
                    }

                    // Ultima pergunta
                    if (sai == 1)
                    {
                        Session["PerguntaAtual"] = perg;
                        Session["OrdemAtual"] = nova;
                        Session["PesquisaAlterada"] = 1;
                        return RedirectToAction("ResponderPesquisa", new { id = (Int32)Session["IdPesquisa"] });
                    }
                    else
                    {
                        Session["PerguntaAtual"] = null;
                    }

                    // Trata retorno
                    return RedirectToAction("ResponderPesquisa", new { id = (Int32)Session["IdPesquisa"] });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }
        public ActionResult VoltarBasePesquisaResposta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }         
            return RedirectToAction("MontarTelaPesquisa");
        }

        public ActionResult FecharPesquisa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            try
            {
                // Recupera credenciais
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Recupera pesquisa
                PESQUISA pesq = (PESQUISA)Session["Pesquisa"];

                // Recupera coleção de respostas
                List<PesquisaItemViewModel> respostas = (List<PesquisaItemViewModel>)Session["Respostas"];

                // Recupera cliente
                CLIENTE cliente = new CLIENTE();
                PesquisaItemViewModel linha = respostas.First();
                if (linha.CLIE_CD_ID != null)
                {
                    cliente = cliApp.GetItemById(linha.CLIE_CD_ID);
                }

                // Processa respostas
                Int32 conta = 0;
                foreach (PesquisaItemViewModel item in respostas)
                {
                    // Monta resposta
                    PESQUISA_RESPOSTA respFinal = new PESQUISA_RESPOSTA();
                    respFinal.ASSI_CD_ID = idAss;
                    respFinal.PESQ_CD_ID = item.PESQ_CD_ID;
                    respFinal.PEIT_CD_ID = item.PEIT_CD_ID;
                    respFinal.ASSI_CD_ID = idAss;
                    if (item.CLIE_CD_ID != null)
                    {
                        respFinal.CLIE_CD_ID = item.CLIE_CD_ID;
                        respFinal.USUA_CD_ID = item.USUA_CD_ID;
                        respFinal.PERE_NM_RESPONDENTE = cliente.CLIE_NM_NOME;
                        respFinal.PERE_EM_EMAIL_RESPONDENTE = cliente.CLIE_NM_EMAIL;
                        respFinal.PERE_NR_TELEFONE_RESPONDENTE = cliente.CLIE_NR_CELULAR;
                    }
                    else
                    {
                        respFinal.CLIE_CD_ID = null;
                        respFinal.USUA_CD_ID = item.USUA_CD_ID;
                        respFinal.PERE_NM_RESPONDENTE = usuario.USUA_NM_NOME;
                        respFinal.PERE_EM_EMAIL_RESPONDENTE = usuario.USUA_NM_EMAIL;
                        respFinal.PERE_NR_TELEFONE_RESPONDENTE = usuario.USUA_NR_CELULAR;
                    }
                    respFinal.PERE_DT_RESPOSTA = DateTime.Today.Date;
                    respFinal.PERE_IN_ATIVO = 1;

                    // Grava resposta
                    Int32 volta = baseApp.ValidateCreatePesquisaResposta(respFinal);
                    conta++;
                }

                // Mensagem
                Session["ContaRespostas"] = conta;
                Session["MensPesquisa"] = 55;

                // Retorno
                return RedirectToAction("VoltarBasePesquisa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pesquisa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }





    }
}