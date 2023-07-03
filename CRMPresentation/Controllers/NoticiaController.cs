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
using EntitiesServices.WorkClasses;
using ERP_Condominios_Solution.Classes;


namespace ERP_Condominios_Solution.Controllers
{
    public class NoticiaController : Controller
    {
        private readonly INoticiaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        NOTICIA objeto = new NOTICIA();
        NOTICIA objetoAntes = new NOTICIA();
        List<NOTICIA> listaMaster = new List<NOTICIA>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;
        BaseAdminController contBase = DependencyResolver.Current.GetService<BaseAdminController>();

        public NoticiaController(INoticiaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        public ActionResult VerNoticia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdVolta"] = id;
            NOTICIA item = baseApp.GetItemById(id);
            item.NOTC_NR_ACESSO = ++item.NOTC_NR_ACESSO;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult IncluirComentario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdVolta"];
            NOTICIA item = baseApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            NOTICIA_COMENTARIO coment = new NOTICIA_COMENTARIO();
            NoticiaComentarioViewModel vm = Mapper.Map<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>(coment);
            vm.NOCO_DT_COMENTARIO = DateTime.Now;
            vm.NOCO_IN_ATIVO = 1;
            vm.NOCO_CD_ID = item.NOTC_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentario(NoticiaComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    NOTICIA_COMENTARIO item = Mapper.Map<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTICIA not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.NOTICIA_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VerNoticia", new { id = idNot});
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
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

        public ActionResult MontarTelaUsuario()
        {
            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            usuario = (USUARIO)Session["UserCredentials"];

            if ((List<NOTICIA>)Session["ListaNoticia"] == null)
            {
                listaMaster = CarregaNoticia();
                Session["ListaNoticia"] = listaMaster;
            }
            ViewBag.Listas = (List<NOTICIA>)Session["ListaNoticia"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Notícias";

            // Indicadores
            ViewBag.Noticias = ((List<NOTICIA>)Session["ListaNoticia"]).Count;

            // Mensagem
            if ((Int32)Session["MensNoticia"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensNoticia"] = 0;
            Session["VoltaNoticia"] = 1;
            objeto = new NOTICIA();
            return View(objeto);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaNoticia"] == 1)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["VoltaNoticia"] == 2)
            {
                return RedirectToAction("MontarTelaUsuario", "Noticia");
            }
            return RedirectToAction("MontarTelaNoticia", "Noticia");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("VerNoticia", new { id = idNot });
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpPost]
        public ActionResult FiltrarNoticia(NOTICIA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Tuple<Int32, List<NOTICIA>, Boolean> volta = baseApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensNoticia"] = 1;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Sucesso
                Session["MensNoticia"] = 0;
                listaMaster = volta.Item2;
                Session["ListaNoticia"] = volta.Item2;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaNoticia"] = null;
            return RedirectToAction("MontarTelaUsuario");
        }

        public ActionResult MostrarTudoNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaNoticia"] = listaMaster;
            return RedirectToAction("MontarTelaUsuario");
        }

       [HttpGet]
        public ActionResult MontarTelaNoticia()
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

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["IdAssinante"] = idAss;
            Session["NoticiaGeral"] = null;

            if ((List<NOTICIA>)Session["ListaNoticia"] == null)
            {
                listaMaster = CarregaNoticiaGeral();
                Session["ListaNoticia"] = listaMaster;
            }
            ViewBag.Listas = (List<NOTICIA>)Session["ListaNoticia"];
            ViewBag.Title = "Notícias";

            // Indicadores
            ViewBag.Noticias = ((List<NOTICIA>)Session["ListaNoticia"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagem
            if ((Int32)Session["MensPermissao"] == 2)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensNoticia"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            objeto = new NOTICIA();
            Session["VoltaNoticia"] = 1;
            Session["MensNoticia"] = 0;
            return View(objeto);
        }

        public ActionResult RetirarFiltroNoticiaGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaNoticia"] = null;
            if ((Int32)Session["VoltaNoticia"] == 2)
            {
                return RedirectToAction("VerCardsNoticias");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        public ActionResult MostrarTudoNoticiaGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaNoticia"] = listaMaster;
            return RedirectToAction("MontarTelaNoticia");
        }

        [HttpPost]
        public ActionResult FiltrarNoticiaGeral(NOTICIA item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Tuple<Int32, List<NOTICIA>, Boolean> volta = baseApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensNoticia"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }

                // Sucesso
                Session["MensNoticia"] = 0;
                listaMaster = volta.Item2;
                Session["ListaNoticia"] = volta.Item2;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        [HttpGet]
        public ActionResult IncluirNoticia()
        {
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
                    return RedirectToAction("MontarTelaNoticia", "Noticia");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Prepara view
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            vm.ASSI_CD_ID = (Int32)Session["IdAssinante"];
            vm.NOTC_DT_EMISSAO = DateTime.Today.Date;
            vm.NOTC_IN_ATIVO = 1;
            vm.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTC_NR_ACESSO = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirNoticia(NoticiaViewModel vm)
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
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Carrega foto e processa alteracao
                    item.NOTC_AQ_FOTO = "~/Images/p_big2.jpg";
                    volta = baseApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    if (Session["FileQueueNoticia"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueNoticia"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                            }
                            else
                            {
                                UploadFotoQueueNoticia(file);
                            }
                        }

                        Session["FileQueueNoticia"] = null;
                    }

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["VoltaNoticia"] = 1;
                    Session["IdNoticiaVolta"] = item.NOTC_CD_ID;
                    Session["Noticia"] = item;
                    Session["IdVolta"] = item.NOTC_CD_ID;
                    Session["MensNoticia"] = 0;
                    Session["NoticiaAlterada"] = 1;
                    return RedirectToAction("VoltarAnexoNoticia");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notícia";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarNoticia(Int32 id)
        {
            // Prepara view
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
                    return RedirectToAction("MontarTelaNoticia", "Noticia");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Mensagens
            if ((Int32)Session["MensNoticia"] == 10)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensNoticia"] == 11)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            NOTICIA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Noticia"] = item;
            Session["IdVolta"] = id;
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarNoticia(NoticiaViewModel vm)
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
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["MensNoticia"] = 0;
                    Session["NoticiaAlterada"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notícia";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirNoticia(Int32 id)
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

            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                NOTICIA item = baseApp.GetItemById(id);
                objetoAntes = (NOTICIA)Session["Noticia"];
                item.NOTC_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                listaMaster = new List<NOTICIA>();
                Session["ListaNoticia"] = null;
                Session["FiltroNoticia"] = null;
                Session["NoticiaAlterada"] = 1;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarNoticia(Int32 id)
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

            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                NOTICIA item = baseApp.GetItemById(id);
                objetoAntes = (NOTICIA)Session["Noticia"];
                item.NOTC_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                listaMaster = new List<NOTICIA>();
                Session["ListaNoticia"] = null;
                Session["FiltroNoticia"] = null;
                Session["NoticiaAlterada"] = 1;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoNoticia(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensNoticia"] = 10;
                return RedirectToAction("VoltarAnexoNoticia");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 idNot = (Int32)Session["IdVolta"];

            NOTICIA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensNoticia"] = 11;
                return RedirectToAction("VoltarAnexoNoticia");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                item.NOTC_AQ_FOTO = "~" + caminho + fileName;
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                Session["NoticiaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoNoticia");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoQueueNoticia(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensNoticia"] = 10;
                return RedirectToAction("VoltarAnexoNoticia");
            }

            NOTICIA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensNoticia"] = 11;
                return RedirectToAction("VoltarAnexoNoticia");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Noticia/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
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
                item.NOTC_AQ_FOTO = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            return RedirectToAction("VoltarAnexoNoticia");
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
            Session["FileQueueNoticia"] = queue;
        }

        public ActionResult VoltarAnexoNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("EditarNoticia", new { id = idNot });
        }

        public List<NOTICIA> CarregaNoticia()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<NOTICIA> conf = new List<NOTICIA>();
            if (Session["Noticias"] == null)
            {
                conf = baseApp.GetAllItensValidos(idAss);
            }
            else
            {
                if ((Int32)Session["NoticiaAlterada"] == 1)
                {
                    conf = baseApp.GetAllItensValidos(idAss);
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
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["NoticiaAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
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


    }
}