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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class GrupoController : Controller
    {
        private readonly IGrupoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IClienteAppService cliApp;
        private readonly IMensagemAppService menApp;

        private String msg;
        private Exception exception;
        GRUPO objeto = new GRUPO();
        GRUPO objetoAntes = new GRUPO();
        List<GRUPO> listaMaster = new List<GRUPO>();
        String extensao;

        public GrupoController(IGrupoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IClienteAppService cliApps, IMensagemAppService menApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            cliApp = cliApps;
            menApp = menApps;
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

            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaGrupo()
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
            if ((List<GRUPO>)Session["ListaGrupo"] == null || ((List<GRUPO>)Session["ListaGrupo"]).Count == 0)
            {
                listaMaster = CarregaGrupo();
                Session["ListaGrupo"] = listaMaster;
            }
            ViewBag.Listas = (List<GRUPO>)Session["ListaGrupo"];
            ViewBag.Title = "Grupos";
            Session["Grupo"] = null;
            Session["IncluirGrupo"] = 0;
            Session["ListaClienteGrupo"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Trata mensagens
            if (Session["MensGrupo"] != null)
            {
                if ((Int32)Session["MensGrupo"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0025", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0062", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 12)
                {
                    String frase = "Foram incluídos um total de " + (String)Session["TotalGrupo"] + " contatos após o processamento do grupo";
                    ModelState.AddModelError("", frase);
                }
                if ((Int32)Session["MensGrupo"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0266", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaGrupo"] = 1;
            objeto = new GRUPO();
            return View(objeto);
        }

        public ActionResult RetirarFiltroGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult MostrarTudoGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaGrupo"] = listaMaster;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarBaseGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaGrupo"] == 11)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaMensagem"] == 40)
            {
                return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult IncluirGrupo()
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Clientes = new SelectList(CarregaCliente().OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Cats = new SelectList(CarregaCatCliente().OrderBy(p => p.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            List<SelectListItem> dias = new List<SelectListItem>();
            for (int i = 1; i < 32; i++)
            {
                dias.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.Dias = new SelectList(dias, "Value", "Text");
            List<SelectListItem> meses = new List<SelectListItem>();
            meses.Add(new SelectListItem() { Text = "Janeiro", Value = "1" });
            meses.Add(new SelectListItem() { Text = "Fevereiro", Value = "2" });
            meses.Add(new SelectListItem() { Text = "Março", Value = "3" });
            meses.Add(new SelectListItem() { Text = "Abril", Value = "4" });
            meses.Add(new SelectListItem() { Text = "Maio", Value = "5" });
            meses.Add(new SelectListItem() { Text = "Junho", Value = "6" });
            meses.Add(new SelectListItem() { Text = "Julho", Value = "7" });
            meses.Add(new SelectListItem() { Text = "Agosto", Value = "8" });
            meses.Add(new SelectListItem() { Text = "Setembro", Value = "9" });
            meses.Add(new SelectListItem() { Text = "Outubro", Value = "10" });
            meses.Add(new SelectListItem() { Text = "Novembro", Value = "11" });
            meses.Add(new SelectListItem() { Text = "Dezembro", Value = "12" });
            ViewBag.Meses = new SelectList(meses, "Value", "Text");

            // Prepara view
            Session["GrupoNovo"] = 0;
            GRUPO item = new GRUPO();
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.GRUP_DT_CADASTRO = DateTime.Today.Date;
            vm.GRUP_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.EMPR_CD_ID = 3;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirGrupo(GrupoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Clientes = new SelectList(CarregaCliente().OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Cats = new SelectList(CarregaCatCliente().OrderBy(p => p.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            List<SelectListItem> dias = new List<SelectListItem>();
            for (int i = 1; i < 32; i++)
            {
                dias.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.Dias = new SelectList(dias, "Value", "Text");
            List<SelectListItem> meses = new List<SelectListItem>();
            meses.Add(new SelectListItem() { Text = "Janeiro", Value = "1" });
            meses.Add(new SelectListItem() { Text = "Fevereiro", Value = "2" });
            meses.Add(new SelectListItem() { Text = "Março", Value = "3" });
            meses.Add(new SelectListItem() { Text = "Abril", Value = "4" });
            meses.Add(new SelectListItem() { Text = "Maio", Value = "5" });
            meses.Add(new SelectListItem() { Text = "Junho", Value = "6" });
            meses.Add(new SelectListItem() { Text = "Julho", Value = "7" });
            meses.Add(new SelectListItem() { Text = "Agosto", Value = "8" });
            meses.Add(new SelectListItem() { Text = "Setembro", Value = "9" });
            meses.Add(new SelectListItem() { Text = "Outubro", Value = "10" });
            meses.Add(new SelectListItem() { Text = "Novembro", Value = "11" });
            meses.Add(new SelectListItem() { Text = "Dezembro", Value = "12" });
            ViewBag.Meses = new SelectList(meses, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Crítica
                    if (vm.SEXO_CD_ID == null & vm.CACL_CD_ID == null & vm.GRUP_NM_CIDADE == null & vm.UF_CD_ID == null & vm.GRUP_DT_NASCIMENTO == null & vm.GRUP_NR_DIA == null & vm.GRUP_NR_MES == null & vm.GRUP_NR_ANO == null)
                    {
                        Session["MensGrupo"] = 10;
                        return RedirectToAction("MontarTelaGrupo");
                    }

                    // Executa a operação
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    MontagemGrupo grupo = new MontagemGrupo();
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, grupo, usuario);

                    // Validações
                    Session["MensGrupo"] = null;
                    if (volta == 3456)
                    {
                        Session["MensGrupo"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0025", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 6543)
                    {
                        Session["MensGrupo"] = 11;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Atualiza cache
                    listaMaster = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    Session["IncluirGrupo"] = 1;
                    Session["GrupoNovo"] = item.GRUP_CD_ID;
                    Session["IdGrupo"] = item.GRUP_CD_ID;
                    Session["GrupoAlterada"] = 1;
                    Session["TotalGrupo"] = volta.ToString();

                    // Trata retorno
                    Session["MensGrupo"] = 12;
                    if ((Int32)Session["VoltaGrupo"] == 11)
                    {
                        return RedirectToAction("VoltarAnexoCliente", "Cliente");
                    }
                    if ((Int32)Session["VoltaGrupo"] == 1)
                    {
                        return RedirectToAction("VoltarBaseGrupo");
                    }
                    if ((Int32)Session["VoltaGrupo"] == 10)
                    {
                        return RedirectToAction("IncluirMensagemAutomacao", "MensagemAutomacao");
                    }
                    return RedirectToAction("VoltarBaseGrupo");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Grupo";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult RemontarGrupo(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                GRUPO item = baseApp.GetItemById(id);
                MontagemGrupo grupo = new MontagemGrupo();
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateRemontar(item, grupo, usuario);

                // Validações
                if (volta == 2)
                {
                    Session["MensGrupo"] = 11;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                    return RedirectToAction("VoltarBaseGrupo");
                }

                // Atualiza cache
                listaMaster = new List<GRUPO>();
                Session["ListaGrupo"] = null;
                Session["IncluirGrupo"] = 1;
                Session["GrupoNovo"] = item.GRUP_CD_ID;
                Session["IdGrupo"] = item.GRUP_CD_ID;
                Session["GrupoAlterada"] = 1;

                // Trata retorno
                if ((Int32)Session["VoltaGrupo"] == 2)
                {
                    return RedirectToAction("VoltarAnexoGrupo");
                }
                return RedirectToAction("VoltarBaseGrupo");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupo";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Recupera grupo
            GRUPO item = baseApp.GetItemById(id);
            Session["Grupo"] = item;

            // Indicadores
            ViewBag.Incluir = (Int32)Session["IncluirGrupo"];

            // Monta view
            Session["VoltaGrupo"] = 2;
            objetoAntes = item;
            Session["IdGrupo"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarGrupo(GrupoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Verifica retorno

                    // Atualiza cache
                    listaMaster = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    Session["GrupoAlterada"] = 1;

                    // Trata retorno
                    if ((Int32)Session["VoltaGrupo"] == 10)
                    {
                        return RedirectToAction("VoltarAnexoCliente", "Cliente");
                    }
                    if ((Int32)Session["VoltaCliGrupo"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoCliente", "Cliente");
                    }
                    return RedirectToAction("MontarTelaGrupo");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Grupo";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Recupera grupo
            GRUPO item = baseApp.GetItemById(id);
            Session["Grupo"] = item;

            // Indicadores
            objetoAntes = item;
            Session["IdGrupo"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarAnexoGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarGrupo", new { id = (Int32)Session["IdGrupo"] });
        }

        public ActionResult RemontarGrupoForn()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("RemontarGrupo", new { id = (Int32)Session["IdGrupo"] });
        }

        [HttpGet]
        public ActionResult ExcluirGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                GRUPO item = baseApp.GetItemById(id);
                objetoAntes = (GRUPO)Session["Grupo"];
                item.GRUP_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta > 0)
                {
                    Session["MensGrupo"] = 22;
                    return RedirectToAction("MontarTelaGrupo");
                }

                Session["ListaGrupo"] = null;
                Session["GrupoAlterada"] = 1;
                return RedirectToAction("MontarTelaGrupo");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupo";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                GRUPO item = baseApp.GetItemById(id);
                objetoAntes = (GRUPO)Session["Grupo"];
                item.GRUP_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                Session["ListaGrupo"] = null;
                Session["GrupoAlterada"] = 1;
                return RedirectToAction("MontarTelaGrupo");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupo";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirContatoGrupo()
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("VoltarAnexoCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagens
            if (Session["MensGrupo"] != null)
            {
                if ((Int32)Session["MensGrupo"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0027", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0268", CultureInfo.CurrentCulture));
                }
            }
            Session["MensGrupo"] = 0;

            // Prepara view
            List<CLIENTE> lista = null;
            if (Session["ListaClienteGrupo"] == null)
            {
                lista = CarregaCliente();
                Session["ListaClienteGrupo"] = lista;
            }
            else
            {
                lista = (List<CLIENTE>)Session["ListaClienteGrupo"];
            }
            ViewBag.Lista = new SelectList(lista.OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            GRUPO_CLIENTE item = new GRUPO_CLIENTE();
            GrupoContatoViewModel vm = Mapper.Map<GRUPO_CLIENTE, GrupoContatoViewModel>(item);
            vm.GRCL_IN_ATIVO = 1;
            vm.GRUP_CD_ID = (Int32)Session["IdGrupo"];
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirContatoGrupo(GrupoContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Critica
                    if (vm.CLIE_CD_ID == null || vm.CLIE_CD_ID == 0)
                    {
                        Session["MensGrupo"] = 14;
                        return RedirectToAction("IncluirContatoGrupo");
                    }

                    // Executa a operação
                    GRUPO_CLIENTE item = Mapper.Map<GrupoContatoViewModel, GRUPO_CLIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    
                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupo"] = 4;
                        return RedirectToAction("IncluirContatoGrupo");
                    }

                    // Verifica retorno
                    return RedirectToAction("IncluirContatoGrupo");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Grupo";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirContatoGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("VoltarAnexoGrupo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                GRUPO_CLIENTE item = baseApp.GetContatoById(id);
                objetoAntes = (GRUPO)Session["Grupo"];
                item.GRCL_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditContato(item);
                return RedirectToAction("VoltarAnexoGrupo");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupo";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarContatoGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("VoltarAnexoGrupo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                GRUPO_CLIENTE item = baseApp.GetContatoById(id);
                objetoAntes = (GRUPO)Session["Grupo"];
                item.GRCL_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditContato(item);
                return RedirectToAction("VoltarAnexoGrupo");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupo";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<GRUPO> CarregaGrupo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<GRUPO> conf = new List<GRUPO>();
            if (Session["Grupos"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
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
    }
}