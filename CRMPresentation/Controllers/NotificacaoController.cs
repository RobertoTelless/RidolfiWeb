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
using EntitiesServices.WorkClasses;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class NotificacaoController : Controller
    {
        private readonly INotificacaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;

        private String msg;
        private Exception exception;
        NOTIFICACAO objeto = new NOTIFICACAO();
        NOTIFICACAO objetoAntes = new NOTIFICACAO();
        List<NOTIFICACAO> listaMaster = new List<NOTIFICACAO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public NotificacaoController(INotificacaoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
        }

        /// <summary>Indexes this instance.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            NOTIFICACAO item = new NOTIFICACAO();
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        [HttpPost]
        public JsonResult GetNotificacaoRefreshTime()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var refresh = confApp.GetItemById(idAss).CONF_NR_REFRESH_NOTIFICACAO;

            if (refresh == null)
            {
                refresh = 60;
            }

            return Json(refresh);
        }

        [HttpPost]
        public JsonResult GetNotificacaoNaoLida()
        {
            var usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            listaMaster = baseApp.GetNotificacaoNovas(usu.USUA_CD_ID, idAss).Where(x => x.NOTI_DT_VISTA == null & x.NOTI_IN_ATIVO == 1).ToList();

            if (listaMaster.Count == 1)
            {
                var hash = new Hashtable();
                hash.Add("msg", "Você possui 1 notificação não lida");

                return Json(hash);
            }
            else if (listaMaster.Count > 1)
            {
                var hash = new Hashtable();
                hash.Add("msg", "Você possui " + listaMaster.Count + " notificações não lidas");

                return Json(hash);
            }
            else
            {
                return null; // Sem notificacoes
            }
        }

        [HttpPost]
        public ActionResult NovaNotificacaoClick()
        {
            var usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            listaMaster = baseApp.GetNotificacaoNovas(usu.USUA_CD_ID, idAss).Where(x => x.NOTI_DT_VISTA == null).ToList();
            if (listaMaster.Count == 1)
            {
                return Json(listaMaster.FirstOrDefault().NOTI_CD_ID);
            }
            else
            {
                return Json(0);
            }
        }

        public ActionResult VerNotificacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdVolta"] = id;
            NOTIFICACAO item = baseApp.GetItemById(id);
            item.NOTI_IN_VISTA = 1;
            item.NOTI_DT_VISTA = DateTime.Now;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);

            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult VerNotificacao(NotificacaoViewModel vm)
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
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    item.NOTI_IN_VISTA = 1;
                    item.NOTI_DT_VISTA = DateTime.Now;
                    objetoAntes = item;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes);

                    // Verifica retorno
                    Session["MensNotificacao"] = 0;
                    Session["NotificacaoAlterada"] = 1;

                    // Sucesso
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notificação";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerNotificacao", new { id = (Int32)Session["IdVolta"] });
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public ActionResult MontarTelaNotificacao()
        {
            // Carrega listas
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

            if ((List<NOTIFICACAO>)Session["ListaNotificacao"] == null)
            {
                listaMaster = baseApp.GetAllItensUser(usuario.USUA_CD_ID, idAss);
                Session["ListaNotificacao"] = listaMaster;
                Session["FiltroNotificacao"] = null;
            }
            ViewBag.Listas = (List<NOTIFICACAO>)Session["ListaNotificacao"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Notificações";
            ViewBag.Cats = new SelectList(baseApp.GetAllCategorias(idAss), "CANO_CD_ID", "CANO_NM_NOME");

            // Indicadores
            ViewBag.Notificacoes = baseApp.GetNotificacaoNovas(usuario.USUA_CD_ID, idAss).Count;

            // Mensagem
            if ((Int32)Session["MensNotificacao"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensNotificacao"] = 0;
            Session["VoltaNotificacao"] = 1;
            objeto = new NOTIFICACAO();
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarNotificacao(NOTIFICACAO item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTIFICACAO> listaObj = new List<NOTIFICACAO>();
                Tuple<Int32, List<NOTIFICACAO>, Boolean> volta = baseApp.ExecuteFilter(item.NOTI_NM_TITULO, item.NOTI_DT_EMISSAO, item.NOTI_TX_TEXTO, idAss);
                Session["FiltroNotificacao"] = item;

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensNotificacao"] = 1;
                    return RedirectToAction("MontarTelaNotificacao");
                }

                // Sucesso
                Session["MensNotificacao"] = 0;
                listaMaster = volta.Item2;
                Session["ListaNotificacao"] = volta.Item2;
                return RedirectToAction("MontarTelaNotificacao");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notificação";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroNotificacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaNotificacao"] = null;
            return RedirectToAction("MontarTelaNotificacao");
        }

        public ActionResult MostrarTudoNotificacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            listaMaster = baseApp.GetAllItensUser(usuario.USUA_CD_ID, idAss);
            Session["ListaNotificacao"] = listaMaster;
            return RedirectToAction("MontarTelaNotificacao");
        }

        [HttpPost]
        public ActionResult UploadFileNotificacao()
        {
            return RedirectToAction("MontarTelaNotificacao");
        }

        [HttpGet]
        public ActionResult VerAnexoNotificacao(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            NOTIFICACAO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoNotificacaoAudio(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            NOTIFICACAO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadNotificacao(Int32 id)
        {
            NOTIFICACAO_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.NOAN_AQ_ARQUIVO;
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

        public ActionResult VoltarAnexoNotificacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("VerNotificacao", new { id = idNot });
        }

        [HttpGet]
        public ActionResult MontarTelaNotificacaoGeral()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "OPR" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if ((List<NOTIFICACAO>)Session["ListaNotificacao"] == null)
            {
                listaMaster = CarregaNotificacao().Where(p => p.NOTI_DT_EMISSAO.Value.Month == DateTime.Today.Date.Month & p.NOTI_DT_EMISSAO.Value.Year == DateTime.Today.Date.Year).OrderByDescending(p => p.NOTI_DT_EMISSAO.Value).ToList();
                Session["ListaNotificacao"] = listaMaster;
                Session["FiltroNotificacao"] = null;
            }
            ViewBag.Listas = (List<NOTIFICACAO>)Session["ListaNotificacao"];
            ViewBag.Title = "Notificações";
            ViewBag.Cats = new SelectList(CarregaCatNotificacao(), "CANO_CD_ID", "CANO_NM_NOME");

            // Indicadores
            ViewBag.Nots = ((List<NOTIFICACAO>)Session["ListaNotificacao"]).Count;

            // Mensagem
            if ((Int32)Session["MensNotificacao"] == 2)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensNotificacao"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensNotificacao"] = 0;
            Session["VoltaNotificacao"] = 2;
            objeto = new NOTIFICACAO();
            return View(objeto);
        }


        public ActionResult RetirarFiltroNotificacaoGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaNotificacao"] = null;
            listaMaster = new List<NOTIFICACAO>();
            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        public ActionResult MostrarTudoNotificacaoGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaNotificacao"] = listaMaster;
            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        [HttpPost]
        public ActionResult FiltrarNotificacaoGeral(NOTIFICACAO item)
        {
            try
            {
                try
                {
                    // Executa a operação
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    List<NOTIFICACAO> listaObj = new List<NOTIFICACAO>();
                    Tuple<Int32, List<NOTIFICACAO>, Boolean> volta = baseApp.ExecuteFilter(item.NOTI_NM_TITULO, item.NOTI_DT_EMISSAO, item.NOTI_TX_TEXTO, idAss);
                    Session["FiltroNotificacao"] = item;

                    // Verifica retorno
                    if (volta.Item1 == 1)
                    {
                        Session["MensNotificacao"] = 1;
                        return RedirectToAction("MontarTelaNotificacaoGeral");
                    }

                    // Sucesso
                    Session["MensNotificacao"] = 0;
                    listaMaster = volta.Item2;
                    Session["ListaNotificacao"] = volta.Item2;
                    return RedirectToAction("MontarTelaNotificacaoGeral");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notificação";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaNotificacaoGeral");
            }
        }


        public ActionResult VoltarBaseNotificacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaNotificacao"] == 1)
            {
                return RedirectToAction("MontarTelaNotificacao");
            }
            if ((Int32)Session["VoltaNotificacao"] == 3)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["VoltaNotificacao"] == 10)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        [HttpGet]
        public ActionResult IncluirNotificacao()
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
                    return RedirectToAction("MontarTelaNotificacaoGeral", "Notificacao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(CarregaCatNotificacao(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

            // Prepara view
            NOTIFICACAO item = new NOTIFICACAO();
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            vm.NOTI_DT_EMISSAO = DateTime.Today;
            vm.NOTI_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTI_IN_VISTA = 0;
            vm.NOTI_IN_NIVEL = 0;
            vm.NOTI_IN_STATUS = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirNotificacao(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(CarregaCatNotificacao(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno


                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Notificacao/" + item.NOTI_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Trata anexos
                    Session["IdVolta"] = item.NOTI_CD_ID;
                    Session["IdNoti"] = item.NOTI_CD_ID;
                    if (Session["FileQueueNotificacao"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueNotificacao"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueNotificacao(file);
                            }
                        }

                        Session["FileQueueNotificacao"] = null;
                    }

                    // Sucesso
                    listaMaster = new List<NOTIFICACAO>();
                    Session["ListaNotificacao"] = null;
                    Session["VoltaNotificacao"] = 1;
                    Session["IdNotificacaoVolta"] = item.NOTI_CD_ID;
                    Session["Notificacao"] = item;
                    Session["MensNotificacao"] = 0;
                    Session["NotificacaoAlterada"] = 1;
                    return RedirectToAction("MontarTelaNotificacaoGeral");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notificação";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarNotificacao(Int32 id)
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
                    Session["MensNotificacao"] = 2;
                    return RedirectToAction("MontarTelaNotificacaoGeral", "Notificacao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Listas
            ViewBag.Cats = new SelectList(CarregaCatNotificacao(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

            // Mensagens
            if ((Int32)Session["MensNotificacao"] == 10)
            {
                ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensNotificacao"] == 11)
            {
                ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            // Prepara view
            NOTIFICACAO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Notificacao"] = item;
            Session["IdVolta"] = id;
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarNotificacao(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(CarregaCatNotificacao(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<NOTIFICACAO>();
                    Session["ListaNotificacao"] = null;
                    Session["MensNotificacao"] = 0;
                    Session["NotificacaoAlterada"] = 1;
                    return RedirectToAction("MontarTelaNotificacaoGeral");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notificação";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirNotificacao(Int32 id)
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
                NOTIFICACAO item = baseApp.GetItemById(id);
                objetoAntes = (NOTIFICACAO)Session["Notificacao"];
                item.NOTI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                listaMaster = new List<NOTIFICACAO>();
                Session["ListaNotificacao"] = null;
                Session["FiltroNotificacao"] = null;
                Session["NotificacaoAlterada"] = 1;
                return RedirectToAction("MontarTelaNotificacaoGeral");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notificação";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarNotificacao(Int32 id)
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
                NOTIFICACAO item = baseApp.GetItemById(id);
                objetoAntes = (NOTIFICACAO)Session["Notificacao"];
                item.NOTI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                listaMaster = new List<NOTIFICACAO>();
                Session["ListaNotificacao"] = null;
                Session["FiltroNotificacao"] = null;
                Session["NotificacaoAlterada"] = 1;
                return RedirectToAction("MontarTelaNotificacaoGeral");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notificação";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoNotificacaoGeral(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            NOTIFICACAO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoNotificacaoGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("EditarNotificacao", new { id = idNot});
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

            Session["FileQueueNotificacao"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileNotificacaoGeral(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensNotificacao"] = 10;
                return RedirectToAction("VoltarAnexoNotificacaoGeral");
            }

            NOTIFICACAO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensNotificacao"] = 11;
                return RedirectToAction("VoltarAnexoNotificacaoGeral");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Notificacao/" + item.NOTI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                NOTIFICACAO_ANEXO foto = new NOTIFICACAO_ANEXO();
                foto.NOAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.NOAN_DT_ANEXO = DateTime.Today;
                foto.NOAN_IN_ATIVO = 1;
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
                foto.NOAN_IN_TIPO = tipo;
                foto.NOAN_NM_TITULO = fileName;
                foto.NOTI_CD_ID = item.NOTI_CD_ID;

                item.NOTIFICACAO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                return RedirectToAction("VoltarAnexoNotificacaoGeral");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notificação";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileQueueNotificacao(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensNotificacao"] = 5;
                return RedirectToAction("VoltarAnexoNotificacao");
            }

            NOTIFICACAO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 50)
            {
                Session["MensNotificacao"] = 6;
                return RedirectToAction("VoltarAnexoNotificacao");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Notificacao/" + item.NOTI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                NOTIFICACAO_ANEXO foto = new NOTIFICACAO_ANEXO();
                foto.NOAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.NOAN_DT_ANEXO = DateTime.Today;
                foto.NOAN_IN_ATIVO = 1;
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
                foto.NOAN_IN_TIPO = tipo;
                foto.NOAN_NM_TITULO = fileName;
                foto.NOTI_CD_ID = item.NOTI_CD_ID;

                item.NOTIFICACAO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                return RedirectToAction("VoltarAnexoNotificacao");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notificação";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notificação", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioLista()
        {
            // Prepara geração
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            Int32 idAss = (Int32)Session["IdAssinante"];
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "NotificacaoLista" + "_" + data + ".pdf";
            List<NOTIFICACAO> lista = (List<NOTIFICACAO>)Session["ListaNotificacao"];
            NOTIFICACAO filtro = (NOTIFICACAO)Session["FiltroNotificacao"];
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
                image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            }
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Notificações - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 50f, 120f, 120f, 50f, 50f, 40f, 40f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Notificações selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Destinatário", meuFont))
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
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Validade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Lida?", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Foto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (NOTIFICACAO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_NOTIFICACAO.CANO_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.USUARIO.USUA_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.NOTI_NM_TITULO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.NOTI_DT_EMISSAO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.NOTI_DT_VALIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.NOTI_DT_VALIDADE.Value.ToShortDateString(), meuFont))
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
                if (item.NOTI_IN_VISTA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Sim", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Não", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (System.IO.File.Exists(Server.MapPath(item.USUARIO.USUA_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.USUARIO.USUA_AQ_FOTO));
                    image.ScaleAbsolute(20, 20);
                    cell.AddElement(image);
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
                if (filtro.NOTI_NM_TITULO != null)
                {
                    parametros += "Título: " + filtro.NOTI_NM_TITULO;
                    ja = 1;
                }
                if (filtro.NOTI_DT_EMISSAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data: " + filtro.NOTI_DT_EMISSAO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data: " + filtro.NOTI_DT_EMISSAO.Value.ToShortDateString();
                    }
                }
                if (filtro.NOTI_TX_TEXTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Conteúdo: " + filtro.NOTI_TX_TEXTO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Conteúdo: " + filtro.NOTI_TX_TEXTO;
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

            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 idNot = (Int32)Session["IdVolta"];
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            NOTIFICACAO aten = baseApp.GetItemById(idNot);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Notificacao_" + aten.NOTI_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

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

            cell = new PdfPCell(new Paragraph("Notificação - Detalhes", meuFont2))
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

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            
            cell = new PdfPCell(new Paragraph("Destinatário: " + aten.USUARIO.USUA_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_NOTIFICACAO.CANO_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Título: " + aten.NOTI_NM_TITULO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);


            if (aten.NOTI_DT_EMISSAO != null)
            {
                cell = new PdfPCell(new Paragraph("Emissão: " + aten.NOTI_DT_EMISSAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Emissão: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.NOTI_DT_VALIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Validade: " + aten.NOTI_DT_VALIDADE.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Validade: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.NOTI_DT_VISTA != null)
            {
                cell = new PdfPCell(new Paragraph("Data de Visualização: " + aten.NOTI_DT_VISTA.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data de Visualização: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Conteúdo: " + aten.NOTI_TX_TEXTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoNotificacao");
        }

        public List<NOTIFICACAO> CarregaNotificacao()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<NOTIFICACAO> conf = new List<NOTIFICACAO>();
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            if (Session["Notificacoes"] == null)
            {
                conf = baseApp.GetAllItensUser(usuario.USUA_CD_ID, idAss);
            }
            else
            {
                if ((Int32)Session["NotificacaoAlterada"] == 1)
                {
                    conf = baseApp.GetAllItensUser(usuario.USUA_CD_ID, idAss);
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

        public List<CATEGORIA_NOTIFICACAO> CarregaCatNotificacao()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CATEGORIA_NOTIFICACAO> conf = new List<CATEGORIA_NOTIFICACAO>();
            if (Session["CatNotificacoes"] == null)
            {
                conf = baseApp.GetAllCategorias(idAss);
            }
            else
            {
                if ((Int32)Session["CatNotificacaoAlterada"] == 1)
                {
                    conf = baseApp.GetAllCategorias(idAss);
                }
                else
                {
                    conf = (List<CATEGORIA_NOTIFICACAO>)Session["CatNotificacoes"];
                }
            }
            Session["CatNotificacoes"] = conf;
            Session["CatNotificacaoAlterada"] = 0;
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

    }
}