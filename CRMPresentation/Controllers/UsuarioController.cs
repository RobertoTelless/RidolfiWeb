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
using System.Text.RegularExpressions;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using Azure.Communication.Email;
using ERP_Condominios_Solution.Classes;
using System.Threading.Tasks;

namespace ERP_Condominios_Solution.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly INotificacaoAppService notiApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IEmpresaAppService empApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        LOG_EXCECAO_NOVO objLogExc = new LOG_EXCECAO_NOVO();
        LOG_EXCECAO_NOVO objLogExcAntes = new LOG_EXCECAO_NOVO();
        List<LOG_EXCECAO_NOVO> listaMasterLogExc = new List<LOG_EXCECAO_NOVO>();
        String extensao;

        public UsuarioController(IUsuarioAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps, IConfiguracaoAppService confApps, IMensagemEnviadaSistemaAppService meApps, IEmpresaAppService empApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notiApps;
            confApp = confApps;
            meApp = meApps;
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

        [HttpGet]
        public ActionResult MontarTelaUsuario()
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
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
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

                // Carrega listas
                ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                ViewBag.Cargos = new SelectList(CarregaCargo(), "CARG_CD_ID", "CARG_NM_NOME");

                // Carrega listas
                if ((List<USUARIO>)Session["ListaUsuario"] == null)
                {
                    listaMaster = CarregaUsuario();
                    Session["ListaUsuario"] = listaMaster;
                    Session["FiltroUsuario"] = null;
                }
                List<USUARIO> listaUsu = (List<USUARIO>)Session["ListaUsuario"];
                ViewBag.Listas = listaUsu;
                ViewBag.Usuarios = listaUsu.Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                ViewBag.UsuariosBloqueados = listaUsu.Where(p => p.USUA_IN_BLOQUEADO == 1).ToList().Count;
                ViewBag.UsuariosHoje = listaUsu.Where(p => p.USUA_IN_BLOQUEADO == 0 && p.USUA_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList().Count;
                ViewBag.Title = "Usuários";

                // Recupera numero de usuarios do assinante
                Session["NumeroUsuarios"] = listaUsu.Count;
                Int32 usuariosPossiveis = (Int32)Session["NumUsuarios"];
                ViewBag.UsuariosPossiveis = usuariosPossiveis;

                // Mensagens
                if (Session["MensUsuario"] != null)
                {
                    if ((Int32)Session["MensUsuario"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0110", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0111", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 9)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0158", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0365", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensUsuario"] == 101)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensUsuario"] == 200)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 201)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "accUSUA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["FlagMensagensEnviadas"] = 4;
                Session["VoltaAnexos"] = 0;
                Session["MensUsuario"] = 0;
                Session["VoltaUsuario"] = 1;
                objeto = new USUARIO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
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
            if ((Int32)Session["VoltaUsuario"] == 2)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaUsuario");
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
        public ActionResult FiltrarUsuario(USUARIO item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.USUA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(item.USUA_NM_NOME);
                item.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringGeral(item.USUA_NM_LOGIN);
                item.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(item.USUA_NM_EMAIL);

                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> listaObj = new List<USUARIO>();
                Tuple<Int32, List<USUARIO>, Boolean> volta = baseApp.ExecuteFilter(item.PERF_CD_ID, item.CARG_CD_ID, item.USUA_NM_NOME, item.USUA_NM_LOGIN, item.USUA_NM_EMAIL, idAss);
                Session["FiltroUsuario"] = item;

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensUsuario"] = 1;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Sucesso
                Session["MensUsuario"] = 0;
                listaMaster = volta.Item2;
                Session["ListaUsuario"] = listaMaster;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroUsuario()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaUsuario"] = null;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoUsuario()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                listaMaster = baseApp.GetAllUsuarios(idAss);
                Session["ListaUsuario"] = listaMaster;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoUsuario(Int32 id)
        {
            try
            {
                // Prepara view
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO_ANEXO item = baseApp.GetAnexoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoUsuarioAudio(Int32 id)
        {
            try
            {
                // Prepara view
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO_ANEXO item = baseApp.GetAnexoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public FileResult DownloadUsuario(Int32 id)
        {
            try
            {
                USUARIO_ANEXO item = baseApp.GetAnexoById(id);
                String arquivo = item.USAN_AQ_ARQUIVO;
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
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult VoltarAnexoUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idUsu = (Int32)Session["IdUsuario"];
            if ((Int32)Session["VoltaAnexos"] == 0)
            {
                return RedirectToAction("MontarTelaUsuario", "Usuario");
            }
            return RedirectToAction("EditarUsuario", new { id = idUsu });
        }

        public ActionResult EditarPerfilUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaUsu"] = 2;
            Int32 idUsu = (Int32)Session["IdUsuario"];
            return RedirectToAction("EditarUsuario", new { id = idUsu });
        }

        [HttpGet]
        public ActionResult VerUsuario(Int32 id)
        {
            try
            {
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["IdUsuario"] = id;
                USUARIO item = baseApp.GetItemById(id);
                UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);

                // Monta Log
                item.ASSINANTE = null;
                item.PERFIL = null;
                item.EMPRESA = null;
                item.CATEGORIA_USUARIO = null;
                item.CARGO = null;
                item.DEPARTAMENTO = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "verUSUA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = item.USUA_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaPerfilUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idUsu = ((USUARIO)Session["UserCredentials"]).USUA_CD_ID;
            Session["VoltaUsuario"] = 2;
            return RedirectToAction("VerUsuario", new { id = idUsu });
        }

        [HttpGet]
        public ActionResult IncluirUsuario()
        {
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

                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
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

                //Verifica possibilidade
                Int32 num = CarregaUsuario().Count;
                Int32 cc = (Int32)Session["NumUsuarios"];
                if ((Int32)Session["NumUsuarios"] <= num)
                {
                    Session["MensUsuario"] = 50;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }

                // Prepara listas
                ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                ViewBag.Cargos = new SelectList(CarregaCargo().OrderBy(p => p.CARG_NM_NOME), "CARG_CD_ID", "CARG_NM_NOME");

                // Prepara view
                USUARIO item = new USUARIO();
                UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
                vm.USUA_DT_CADASTRO = DateTime.Today.Date;
                vm.USUA_IN_ATIVO = 1;
                vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
                vm.USUA_IN_BLOQUEADO = 0;
                vm.USUA_IN_LOGADO = 0;
                vm.USUA_IN_LOGIN_PROVISORIO = 0;
                vm.USUA_IN_PROVISORIO = 0;
                vm.USUA_NR_ACESSOS = 0;
                vm.USUA_NR_FALHAS = 0;
                vm.CAUS_CD_ID = 1;
                vm.USUA_IN_ESPECIAL = 0;
                vm.USUA_IN_PENDENTE_CODIGO = 0;
                vm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirUsuario(UsuarioViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                    ViewBag.Cargos = new SelectList(CarregaCargo().OrderBy(p => p.CARG_NM_NOME), "CARG_CD_ID", "CARG_NM_NOME");

                    // Sanitização
                    vm.USUA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_NOME);
                    vm.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_LOGIN);
                    vm.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_NM_EMAIL);
                    vm.USUA_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_TX_OBSERVACOES);

                    // Executa a operação
                    USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUsuario"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensUsuario"] = 4;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensUsuario"] = 5;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0110", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensUsuario"] = 6;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0111", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        Session["MensUsuario"] = 7;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        Session["MensUsuario"] = 10;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0158", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        Session["MensUsuario"] = 11;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0223", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        Session["MensUsuario"] = 12;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0224", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 11)
                    {
                        Session["MensUsuario"] = 13;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0225", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 12)
                    {
                        Session["MensUsuario"] = 14;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0226", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 13)
                    {
                        Session["MensUsuario"] = 15;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0227", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Carrega foto e processa alteracao
                    if (item.USUA_AQ_FOTO == null)
                    {
                        item.USUA_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                        volta = baseApp.ValidateEdit(item, usuarioLogado);

                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<USUARIO>();
                    Session["ListaUsuario"] = null;
                    Session["IdUsuario"] = item.USUA_CD_ID;
                    Session["UsuarioAlterada"] = 1;

                    if (Session["FileQueueUsuario"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueUsuario"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueUsuario(file);
                            }
                            else
                            {
                                UploadFotoQueueUsuario(file);
                            }
                        }

                        Session["FileQueueUsuario"] = null;
                    }

                    Session["MensUsuario"] = 0;
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarUsuario(Int32 id)
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
                        return RedirectToAction("CarregarBase", "BaseAdmin");
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

                if ((Int32)Session["MensUsuario"] == 100)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0305", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                    ModelState.AddModelError("", frase);
                }
                if ((Int32)Session["MensUsuario"] == 101)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                    ModelState.AddModelError("", frase);
                }

                // Prepara view
                ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                ViewBag.Cargos = new SelectList(CarregaCargo().OrderBy(p => p.CARG_NM_NOME), "CARG_CD_ID", "CARG_NM_NOME");
                ViewBag.UsuarioLogado = usuario;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = item;
                Session["VoltaAnexos"] = 1;
                Session["Usuario"] = item;
                Session["IdUsuario"] = id;
                Session["VoltaUsu"] = 1;
                Session["MensUsuario"] = 0;
                Session["FlagMensagensEnviadas"] = 4;
                UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarUsuario(UsuarioViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                    ViewBag.Cargos = new SelectList(CarregaCargo().OrderBy(p => p.CARG_NM_NOME), "CARG_CD_ID", "CARG_NM_NOME");

                    // Sanitização
                    vm.USUA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_NOME);
                    vm.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_LOGIN);
                    vm.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_NM_EMAIL);
                    vm.USUA_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_TX_OBSERVACOES);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUsuario"] = 4;
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    if (volta == 2)
                    {
                        Session["MensUsuario"] = 5;
                        return RedirectToAction("MontarTelaUsuario");
                    }

                    // Mensagens
                    if (Session["MensUsuario"] != null)
                    {
                        if ((Int32)Session["MensUsuario"] == 10)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                        }
                        if ((Int32)Session["MensUsuario"] == 11)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                        }
                    }

                    // Sucesso
                    listaMaster = new List<USUARIO>();
                    Session["ListaUsuario"] = null;
                    Session["MensUsuario"] = 0;
                    Session["UsuarioAlterada"] = 1;
                    if ((Int32)Session["VoltaUsu"] == 1)
                    {
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    else
                    {
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

       [HttpGet]
        public ActionResult BloquearUsuario(Int32 id)
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

                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
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

                // Executar
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                USUARIO block = new USUARIO();
                block.USUA_CD_ID = item.USUA_CD_ID;
                block.ASSI_CD_ID = item.ASSI_CD_ID;
                block.PERF_CD_ID = item.PERF_CD_ID;
                block.CARG_CD_ID = item.CARG_CD_ID;
                block.USUA_NM_NOME = item.USUA_NM_NOME;
                block.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                block.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                block.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                block.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                block.USUA_NM_SENHA = item.USUA_NM_SENHA;
                block.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                block.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                block.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                block.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                block.USUA_NR_CPF = item.USUA_NR_CPF;
                block.USUA_NR_RG = item.USUA_NR_RG;
                block.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                block.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                block.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                block.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                block.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                block.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                block.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
                block.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
                block.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                block.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                block.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                block.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                block.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                block.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                block.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                block.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                block.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                block.USUA_IN_BLOQUEADO = 1;
                block.USUA_DT_BLOQUEADO = DateTime.Today;
                block.EMPR_CD_ID = item.EMPR_CD_ID;

                Int32 volta = baseApp.ValidateBloqueio(block, usuario);
                listaMaster = new List<USUARIO>();
                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult DesbloquearUsuario(Int32 id)
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
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                USUARIO unblock = new USUARIO();
                unblock.USUA_CD_ID = item.USUA_CD_ID;
                unblock.ASSI_CD_ID = item.ASSI_CD_ID;
                unblock.PERF_CD_ID = item.PERF_CD_ID;
                unblock.CARG_CD_ID = item.CARG_CD_ID;
                unblock.USUA_NM_NOME = item.USUA_NM_NOME;
                unblock.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                unblock.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                unblock.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                unblock.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                unblock.USUA_NM_SENHA = item.USUA_NM_SENHA;
                unblock.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                unblock.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                unblock.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                unblock.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                unblock.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                unblock.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                unblock.USUA_NR_CPF = item.USUA_NR_CPF;
                unblock.USUA_NR_RG = item.USUA_NR_RG;
                unblock.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                unblock.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                unblock.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                unblock.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                unblock.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
                unblock.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
                unblock.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                unblock.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                unblock.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                unblock.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                unblock.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                unblock.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                unblock.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                unblock.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                unblock.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                unblock.USUA_IN_BLOQUEADO = 0;
                unblock.USUA_DT_BLOQUEADO = null;
                unblock.EMPR_CD_ID = item.EMPR_CD_ID;

                Int32 volta = baseApp.ValidateDesbloqueio(unblock, usuario);
                listaMaster = new List<USUARIO>();

                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult DesativarUsuario(Int32 id)
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
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                USUARIO dis = new USUARIO();
                dis.USUA_CD_ID = item.USUA_CD_ID;
                dis.ASSI_CD_ID = item.ASSI_CD_ID;
                dis.PERF_CD_ID = item.PERF_CD_ID;
                dis.CARG_CD_ID = item.CARG_CD_ID;
                dis.USUA_NM_NOME = item.USUA_NM_NOME;
                dis.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                dis.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                dis.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                dis.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                dis.USUA_NM_SENHA = item.USUA_NM_SENHA;
                dis.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                dis.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                dis.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                dis.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                dis.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                dis.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                dis.USUA_NR_CPF = item.USUA_NR_CPF;
                dis.USUA_NR_RG = item.USUA_NR_RG;
                dis.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                dis.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                dis.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                dis.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                dis.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                dis.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                dis.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                dis.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                dis.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                dis.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                dis.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                dis.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                dis.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                dis.USUA_IN_ATIVO = 0;
                dis.USUA_DT_ALTERACAO = DateTime.Today;
                dis.EMPR_CD_ID = item.EMPR_CD_ID;

                Int32 volta = baseApp.ValidateDelete(dis, usuario);
                listaMaster = new List<USUARIO>();

                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarUsuario(Int32 id)
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
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                //Verifica possibilidade
                Int32 num = CarregaUsuario().Count;
                if ((Int32)Session["NumUsuarios"] <= num)
                {
                    Session["MensUsuario"] = 50;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }

                // Executar
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                USUARIO en = new USUARIO();
                en.USUA_CD_ID = item.USUA_CD_ID;
                en.ASSI_CD_ID = item.ASSI_CD_ID;
                en.PERF_CD_ID = item.PERF_CD_ID;
                en.CARG_CD_ID = item.CARG_CD_ID;
                en.USUA_NM_NOME = item.USUA_NM_NOME;
                en.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                en.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                en.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                en.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                en.USUA_NM_SENHA = item.USUA_NM_SENHA;
                en.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                en.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                en.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                en.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                en.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                en.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                en.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                en.USUA_NR_CPF = item.USUA_NR_CPF;
                en.USUA_NR_RG = item.USUA_NR_RG;
                en.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                en.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                en.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                en.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                en.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                en.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                en.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                en.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                en.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                en.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                en.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                en.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                en.USUA_IN_ATIVO = 1;
                en.USUA_DT_ALTERACAO = DateTime.Today;
                en.EMPR_CD_ID = item.EMPR_CD_ID;

                Int32 volta = baseApp.ValidateReativar(en, usuario);
                listaMaster = new List<USUARIO>();

                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
            Session["FileQueueUsuario"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueUsuario(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                USUARIO_ANEXO foto = new USUARIO_ANEXO();
                foto.USAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.USAN_DT_ANEXO = DateTime.Today;
                foto.USAN_IN_ATIVO = 1;
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
                foto.USAN_IN_TIPO = tipo;
                foto.USAN_NM_TITULO = fileName;
                foto.USUA_CD_ID = item.USUA_CD_ID;

                item.USUARIO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);
                Session["UsuarioAlterada"] = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "aneUSUA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

       [HttpPost]
        public ActionResult UploadFileUsuario(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                USUARIO_ANEXO foto = new USUARIO_ANEXO();
                foto.USAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.USAN_DT_ANEXO = DateTime.Today;
                foto.USAN_IN_ATIVO = 1;
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
                foto.USAN_IN_TIPO = tipo;
                foto.USAN_NM_TITULO = fileName;
                foto.USUA_CD_ID = item.USUA_CD_ID;

                item.USUARIO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "aneUSUA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoQueueUsuario(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idUsu = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idUsu);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
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
                    item.USUA_AQ_FOTO = "~" + caminho + fileName;
                    objeto = item;
                    Int32 volta = baseApp.ValidateEdit(item, objeto);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "fotUSUA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        public ActionResult UploadFotoUsuario(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
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
                    item.USUA_AQ_FOTO = "~" + caminho + fileName;
                    objeto = item;
                    Int32 volta = baseApp.ValidateEdit(item, objeto);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "fotUSUA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioLista()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "UsuarioLista" + "_" + data + ".pdf";
                List<USUARIO> lista = (List<USUARIO>)Session["ListaUsuario"];
                USUARIO filtro = (USUARIO)Session["FiltroUsuario"];
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

                cell = new PdfPCell(new Paragraph("Usuários - Listagem", meuFont2))
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
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 60f, 80f, 50f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Usuários selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 8;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
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
                cell = new PdfPCell(new Paragraph("Login", meuFont))
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
                cell = new PdfPCell(new Paragraph("Perfil", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Bloqueado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Acessos", meuFont))
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

                foreach (USUARIO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_LOGIN, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CARGO.CARG_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PERFIL.PERF_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_IN_BLOQUEADO == 1 ? "Sim" : "Não", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NR_ACESSOS.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (System.IO.File.Exists(Server.MapPath(item.USUA_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        image = Image.GetInstance(Server.MapPath(item.USUA_AQ_FOTO));
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
                    if (filtro.USUA_NM_NOME != null)
                    {
                        parametros += "Nome: " + filtro.USUA_NM_NOME;
                        ja = 1;
                    }
                    if (filtro.USUA_NM_LOGIN != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Login: " + filtro.USUA_NM_LOGIN;
                            ja = 1;
                        }
                        else
                        {
                            parametros +=  " e Login: " + filtro.USUA_NM_LOGIN;
                        }
                    }
                    if (filtro.USUA_NM_EMAIL != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "E-Mail: " + filtro.USUA_NM_EMAIL;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e E-Mail: " + filtro.USUA_NM_EMAIL;
                        }
                    }
                    if (filtro.PERF_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Perfil: " + filtro.PERFIL.PERF_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Perfil: " + filtro.PERFIL.PERF_NM_NOME;
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

                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
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
                USUARIO aten = baseApp.GetItemById((Int32)Session["IdUsuario"]);
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "Usuario_" + aten.USUA_CD_ID.ToString() + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontGreen = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.GREEN);

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

                cell = new PdfPCell(new Paragraph("Usuário - Detalhes", meuFont2))
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
            
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = Image.GetInstance(Server.MapPath(aten.USUA_AQ_FOTO));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Empresa/Filial: " + aten.EMPRESA.EMPR_NM_NOME, meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome: " + aten.USUA_NM_NOME, meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("E-Mail: " + aten.USUA_NM_EMAIL, meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Login: " + aten.USUA_NM_LOGIN, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Cargo: " + aten.CARGO.CARG_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Perfil: " + aten.PERFIL.PERF_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Acessos
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados de Acesso", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.USUA_IN_BLOQUEADO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Bloqueado: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Bloqueado: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_DT_BLOQUEADO != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Bloqueio: " + aten.USUA_DT_BLOQUEADO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Bloqueio: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_IN_PROVISORIO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Senha Provisória: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Senha Provisória: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_IN_LOGIN_PROVISORIO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Login Provisório: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Login Provisório: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                if (aten.USUA_DT_ALTERACAO != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração: " + aten.USUA_DT_ALTERACAO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_DT_TROCA_SENHA != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração de Senha: " + aten.USUA_DT_TROCA_SENHA.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração de Senha: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                cell = new PdfPCell(new Paragraph("Acessos: " + CrossCutting.Formatters.DecimalFormatter(aten.USUA_NR_ACESSOS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.USUA_DT_ACESSO != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Último Acesso: " + aten.USUA_DT_ACESSO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Último Acesso: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph("Falhas de Login: " + CrossCutting.Formatters.DecimalFormatter(aten.USUA_NR_FALHAS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.USUA_DT_ULTIMA_FALHA != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Última Falha: " + aten.USUA_DT_ULTIMA_FALHA.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Última Falha: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Observações
                Chunk chunk1 = new Chunk("Observações: " + aten.USUA_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

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
                    LOG_NM_OPERACAO = "relUSUA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = aten.USUA_NM_NOME,
                    LOG_IN_SISTEMA = 1
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenha(Int32 id)
        {
            
            // Prepara view
            return RedirectToAction("TrocarSenha", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult EnviarEMailUsuarioForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EnviarEMailUsuario", new { id = (Int32)Session["IdUsuario"] });
        }

        [HttpGet]
        public ActionResult EnviarSMSUsuarioForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EnviarSMSUsuario", new { id = (Int32)Session["IdUsuario"] });
        }

        [HttpGet]
        public ActionResult EnviarEMailUsuario(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                //Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensUsuario"] = 200;
                    return RedirectToAction("MontarTelaUsuario");
                }

                USUARIO cont = baseApp.GetItemById(id);
                Session["Usuario"] = cont;
                ViewBag.PessoaExterna = cont;
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.USUA_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarEMailUsuario(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = await ProcessaEnvioEMailUsuario(vm, usuarioLogado);
                    USUARIO cont = (USUARIO)Session["Usuario"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "emaUSUA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.USUA_NM_NOME,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailUsuario(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];
            String erro = null;
            String status = "Succeeded";
            String iD = new Guid().ToString();

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.USUA_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

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
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
            }
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Usuário - " + cont.USUA_NM_NOME;
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
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                vm.MENS_NM_NOME = "Usuário - " + cont.USUA_NM_NOME;
                vm.MENS_NM_CAMPANHA = cont.USUA_NM_EMAIL;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null;
                vm.MENS_IN_USUARIO = cont.USUA_CD_ID;
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(baseApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(vm, usuario, emailBody, status, iD, erro, "Mensagem para Usuário");
                Session["MensUsuario"] = 100;
                Session["IdMail"] = new Guid().ToString();
            }
            else
            {
                Session["MensUsuario"] = 110;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }

            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSUsuario(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                //Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    Session["MensUsuario"] = 201;
                    return RedirectToAction("MontarTelaUsuario");
                }

                USUARIO item = baseApp.GetItemById(id);
                Session["Usuario"] = item;
                ViewBag.PessoaExterna = item;
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = item.USUA_NM_NOME;
                mens.ID = id;
                mens.MODELO = item.USUA_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EnviarSMSUsuario(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSUsuario(vm, usuarioLogado);
                    USUARIO cont = (USUARIO)Session["Usuario"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "smsUSUA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.USUA_NM_NOME,
                        LOG_IN_SISTEMA = 1
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSUsuario(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];
            String erro = null;

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.USUA_NM_NOME + ".  ";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = assi.ASSI_NM_NOME;

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Decriptografa chaves
            String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
            String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

            // Monta token
            String text = login + ":" + senha;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = cab + " " + vm.MENS_TX_SMS + "  " + rod;

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

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(cont.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"SysPrec\"}]}");
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
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            vm.MENS_NM_NOME = "Usuário - " + cont.USUA_NM_NOME;
            vm.MENS_NM_CAMPANHA = cont.USUA_NM_EMAIL;
            vm.FORN_CD_ID = null;
            vm.CLIE_CD_ID = null;
            vm.MENS_IN_USUARIO = cont.USUA_CD_ID;
            EnvioEMailGeralBase envio = new EnvioEMailGeralBase(baseApp, confApp, meApp);
            Int32 volta = envio.GravarMensagemEnviadaSMS(vm, usuario, smsBody);
            return 0;
        }

        public ActionResult IncluirCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCargo"] = 2;
            return RedirectToAction("IncluirCargo", "TabelaAuxiliar");
        }

        public ActionResult IncluirCargo1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCargo"] = 3;
            return RedirectToAction("IncluirCargo", "TabelaAuxiliar");
        }

        public List<USUARIO> CarregaUsuario()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> conf = new List<USUARIO>();
                if (Session["Usuarios"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
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
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CARGO> CarregaCargo()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CARGO> conf = new List<CARGO>();
                if (Session["Cargos"] == null)
                {
                    conf = baseApp.GetAllCargos(idAss);
                }
                else
                {
                    if ((Int32)Session["CargoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllCargos(idAss);
                    }
                    else
                    {
                        conf = (List<CARGO>)Session["Cargos"];
                    }
                }
                Session["CargoAlterada"] = 0;
                Session["Cargos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PERFIL> CarregaPerfil()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PERFIL> conf = new List<PERFIL>();
                if (Session["Perfis"] == null)
                {
                    conf = baseApp.GetAllPerfis();
                }
                else
                {
                    conf = (List<PERFIL>)Session["Perfis"];
                }
                Session["Perfis"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
            Session["FlagMensagensEnviadas"] = 4;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

        public ActionResult CriptoSenha()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO item = (USUARIO)Session["UserCredentials"];

            USUARIO block = new USUARIO();
            block.USUA_CD_ID = item.USUA_CD_ID;
            block.ASSI_CD_ID = item.ASSI_CD_ID;
            block.PERF_CD_ID = item.PERF_CD_ID;
            block.CARG_CD_ID = item.CARG_CD_ID;
            block.USUA_NM_NOME = item.USUA_NM_NOME;
            block.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            block.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            block.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            block.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            block.USUA_NM_SENHA = item.USUA_NM_SENHA;
            block.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            block.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            block.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            block.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            block.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
            block.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
            block.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            block.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            block.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            block.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            block.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            block.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            block.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            block.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            block.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            block.USUA_IN_BLOQUEADO = 0;
            block.USUA_DT_BLOQUEADO = null;
            block.EMPR_CD_ID = item.EMPR_CD_ID;

            String senha = block.USUA_NM_SENHA;
            byte[] salt = CrossCutting.Cryptography.GenerateSalt();
            String hashedPassword = CrossCutting.Cryptography.HashPassword(block.USUA_NM_SENHA, salt);
            block.USUA_NM_SENHA = hashedPassword;
            Int32 volta = baseApp.ValidateEdit(block, block);
            Session["UserCredentials"] = block;

            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public ActionResult DecriptoSenha()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO item = (USUARIO)Session["UserCredentials"];

            USUARIO block = new USUARIO();
            block.USUA_CD_ID = item.USUA_CD_ID;
            block.ASSI_CD_ID = item.ASSI_CD_ID;
            block.PERF_CD_ID = item.PERF_CD_ID;
            block.CARG_CD_ID = item.CARG_CD_ID;
            block.USUA_NM_NOME = item.USUA_NM_NOME;
            block.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            block.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            block.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            block.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            block.USUA_NM_SENHA = item.USUA_NM_SENHA;
            block.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            block.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            block.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            block.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            block.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
            block.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
            block.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            block.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            block.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            block.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            block.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            block.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            block.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            block.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            block.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            block.USUA_IN_BLOQUEADO = 0;
            block.USUA_DT_BLOQUEADO = null;
            block.EMPR_CD_ID = item.EMPR_CD_ID;

            String senha = block.USUA_NM_SENHA;
            senha = CrossCutting.Cryptography.Decrypt(senha);
            block.USUA_NM_SENHA = senha;
            Int32 volta = baseApp.ValidateEdit(block, block);
            Session["UserCredentials"] = block;
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaLogExcecao()
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
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                ViewBag.Usuarios = new SelectList(baseApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                if ((List<LOG_EXCECAO_NOVO>)Session["ListaLogExcecao"] == null)
                {
                    listaMasterLogExc = baseApp.GetAllLogExcecao(idAss);
                    Session["ListaLogExcecao"] = listaMasterLogExc;
                }
                ViewBag.Listas = (List<LOG_EXCECAO_NOVO>)Session["ListaLogExcecao"];
                ViewBag.Logs = ((List<LOG_EXCECAO_NOVO>)Session["ListaLogExcecao"]).Count;

                // Mensagens
                if ((Int32)Session["MensLog"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }

                // Abre view
                Session["MensLog"] = 0;
                objLogExc = new LOG_EXCECAO_NOVO();
                objLogExc.LOEX_DT_DATA = DateTime.Today.Date;
                return View(objLogExc);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroLogExcecao()
        {
            Session["ListaLogExcecao"] = null;
            return RedirectToAction("MontarTelaLogExcecao");
        }

        [HttpPost]
        public ActionResult FiltrarLogExcecao(LOG_EXCECAO_NOVO item)
        {
            try
            {
                // Sanitização
                item.LOEX_NM_GERADOR = CrossCutting.UtilitariosGeral.CleanStringGeral(item.LOEX_NM_GERADOR);

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOG_EXCECAO_NOVO> listaObj = new List<LOG_EXCECAO_NOVO>();
                Tuple<Int32, List<LOG_EXCECAO_NOVO>, Boolean> volta = baseApp.ExecuteFilterExcecao(item.USUA_CD_ID, item.LOEX_DT_DATA, item.LOEX_NM_GERADOR, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLog"] = 1;
                    return RedirectToAction("MontarTelaLogExcecao");
                }

                // Sucesso
                listaMasterLogExc = volta.Item2;
                Session["ListaLogExcecao"] = listaMasterLogExc;
                return RedirectToAction("MontarTelaLogExcecao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerLogExcecao(Int32 id)
        {
            try
            {
                // Prepara view
                LOG_EXCECAO_NOVO item = baseApp.GetLogExcecaoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexo(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO_ANEXO item = baseApp.GetAnexoById(id);
                item.USAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnexo(item);
                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Agenda", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

    }
}