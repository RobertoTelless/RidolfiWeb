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
using Canducci.Zip;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using System.Security.Cryptography.Xml;
using ERP_Condominios_Solution.Classes;
using DataServices.Repositories;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IEmpresaAppService baseApp;
        private readonly IPlataformaEntregaAppService platApp;
        private readonly ITicketAppService tkApp;
        private readonly IAntecipacaoAppService antApp;
        private readonly IMaquinaAppService maqApp;
        private readonly IVendaMensalAppService venApp;
        private readonly IComissaoAppService comApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        EMPRESA objeto = new EMPRESA();
        EMPRESA objetoAntes = new EMPRESA();
        List<EMPRESA> listaMaster = new List<EMPRESA>();
        String extensao;

        public EmpresaController(IEmpresaAppService baseApps, IPlataformaEntregaAppService platApps, ITicketAppService tkApps, IAntecipacaoAppService antApps, IMaquinaAppService maqApps, IVendaMensalAppService venApps, IComissaoAppService comApps, IUsuarioAppService usuApps)
        {
            baseApp = baseApps;
            platApp = platApps;
            tkApp = tkApps;
            antApp = antApps;
            maqApp = maqApps;
            venApp = venApps;
            comApp = comApps;
            usuApp = usuApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
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

        [HttpGet]
        public ActionResult MontarTelaEmpresa(Int32? id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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

            // Carrega Dados
            objeto = baseApp.GetItemById(usuario.EMPR_CD_ID.Value);
            Session["Empresa"] = objeto;
            ViewBag.Empresa = objeto;
            Session["IdEmpresa"] = objeto.EMPR_CD_ID;

            // Indicadores
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Maquinas = new SelectList(CarregaMaquina(), "MAQN_CD_ID", "MAQN_NM_NOME");
            List<SelectListItem> opera = new List<SelectListItem>();
            opera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            opera.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Opera = new SelectList(opera, "Value", "Text");
            List<SelectListItem> comissao = new List<SelectListItem>();
            comissao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            comissao.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Comissao = new SelectList(comissao, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Calculo = objeto.EMPR_IN_CALCULADO;

            // Verifica virada de mes
            ViewBag.Virada = 0;
            ViewBag.Flag = (Int32)Session["FlagCalculoCustos"];
            DateTime hoje = DateTime.Today.Date;
            CUSTO_VARIAVEL_CALCULO calc = baseApp.GetAllCalculo(idAss).OrderByDescending(p => p.CVCA_DT_REFERENCIA).ToList().FirstOrDefault();
            if (calc != null)
            {
                if (calc.CVCA_DT_REFERENCIA.Value.Month != hoje.Month & calc.CVCA_DT_REFERENCIA.Value.Year != hoje.Year)
                {
                    ViewBag.Virada = 1;
                    Session["FlagCalculoCustos"] = 0;
                    ViewBag.Flag = 0;

                    // Acerta custos
                    EMPRESA empresa = (EMPRESA)Session["Empresa"];
                    RecalculoCustos calculo = new RecalculoCustos(baseApp, platApp, tkApp, antApp, maqApp, venApp, comApp);
                    Int32 voltaX = calculo.AtualizarCustos(empresa, usuario, idAss);
                    Session["FlagCalculoCustos"] = 1;
                }
            }

            // Mensagem
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0020", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0112", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0113", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 45)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0272", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 46)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0272", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 100)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0294", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["FlagMensagensEnviadas"] = 1;
            Session["MensEmpresa"] = 0;
            Session["VoltarEmpresa"] = 44;
            EmpresaViewModel vm = Mapper.Map<EMPRESA, EmpresaViewModel>(objeto);
            objetoAntes = objeto;
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaEmpresa(EmpresaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Maquinas = new SelectList(CarregaMaquina(), "MAQN_CD_ID", "MAQN_NM_NOME");
            List<SelectListItem> opera = new List<SelectListItem>();
            opera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            opera.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Opera = new SelectList(opera, "Value", "Text");
            List<SelectListItem> comissao = new List<SelectListItem>();
            comissao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            comissao.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Comissao = new SelectList(comissao, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    EMPRESA item = Mapper.Map<EmpresaViewModel, EMPRESA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Acerta custos de vendas
                    listaMaster = new List<EMPRESA>();
                    Session["Empresa"] = null;
                    Session["MensEmpresa"] = 0;
                    Session["VoltaAtualiza"] = 1;
                    Session["FlagCalculoCustos"] = 1;

                    // Encerra
                    return RedirectToAction("MontarTelaEmpresa", "Empresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VerAssociacaoMaquina()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["NivelEmpresa"] = 2;
            return RedirectToAction("MontarTelaEmpresa");
        }

        public ActionResult VerAssociacaoPlataforma()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["NivelEmpresa"] = 3;
            return RedirectToAction("MontarTelaEmpresa");
        }

        public ActionResult VerAssociacaoTicket()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["NivelEmpresa"] = 4;
            return RedirectToAction("MontarTelaEmpresa");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEmpresa");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltarEmpresa"] == 2)
            {
                return RedirectToAction("MontarTelaRoteiro_1", "Precificacao");
            }
            if ((Int32)Session["VoltarEmpresa"] == 3)
            {
                return RedirectToAction("MontarTelaRoteiro_2", "Precificacao");
            }
            if ((Int32)Session["VoltarEmpresa"] == 4)
            {
                return RedirectToAction("MontarTelaRoteiro_3", "Precificacao");
            }
            if ((Int32)Session["VoltarEmpresa"] == 5)
            {
                return RedirectToAction("MontarTelaRoteiro_4", "Precificacao");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult VerAnexoEmpresa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoEmpresaAudio(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoEmpresa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEmpresa");
        }

        public FileResult DownloadEmpresa(Int32 id)
        {
            EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.EMAN_AQ_ARQUIVO;
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

        [HttpPost]
        public ActionResult UploadFileEmpresa(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensEmpresa"] = 1;
                return RedirectToAction("VoltarAnexoEmpresa");
            }

            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            EMPRESA item = baseApp.GetItemByAssinante(usuario.ASSI_CD_ID);
            var fileName = Path.GetFileName(file.FileName);

            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensEmpresa"] = 3;
                return RedirectToAction("VoltarAnexoEmpresa");
            }

            String caminho = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Empresa/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            EMPRESA_ANEXO foto = new EMPRESA_ANEXO();
            foto.EMAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.EMAN_DT_ANEXO = DateTime.Today;
            foto.EMAN_IN_ATIVO = 1;
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
            foto.EMAN_IN_TIPO = tipo;
            foto.EMAN_NM_TITULO = fileName;
            foto.EMPR_CD_ID = item.EMPR_CD_ID;

            item.EMPRESA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoEmpresa");
        }

        [HttpGet]
        public ActionResult ExcluirEmpresaMaquina(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA_MAQUINA item = baseApp.GetMaquinaById(id);

                List<VENDA_MENSAL> vendas = CarregaVenda().Where(p => p.MAQN_CD_ID == item.MAQN_CD_ID).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    vendas = vendas.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                if (vendas.Count > 0)
                {
                    Session["MensEmpresa"] = 100;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                item.EMMA_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditMaquina(item);
                Session["EmpresaAlterada"] = 1;

                // ReCalcula taxas médias
                EMPRESA emp = (EMPRESA)Session["Empresa"];
                List<EMPRESA_MAQUINA> maq = emp.EMPRESA_MAQUINA.ToList();
                Decimal somaC = 0;
                Decimal somaD = 0;
                Decimal taxaC = 0;
                Decimal taxaD = 0;
                Int32 quant = 0;
                foreach (EMPRESA_MAQUINA em in maq)
                {
                    somaC += em.MAQUINA.MAQN_PC_CREDITO;
                    somaD += em.MAQUINA.MAQN_PC_DEBITO;
                    quant++;
                }
                taxaC = somaC / quant;
                taxaD = somaD / quant;
                emp.EMPR_VL_TAXA_MEDIA = taxaC;
                emp.EMPR_VL_TAXA_MEDIA_DEBITO = taxaD;
                volta = baseApp.ValidateEdit(emp, emp);
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEmpresaMaquina(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA_MAQUINA item = baseApp.GetMaquinaById(id);
                item.EMMA_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditMaquina(item);
                Session["EmpresaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirEmpresaPlataforma(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA_PLATAFORMA item = baseApp.GetPlataformaById(id);

                List<VENDA_MENSAL> vendas = CarregaVenda().Where(p => p.PLEN_CD_ID == item.PLEN_CD_ID).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    vendas = vendas.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                if (vendas.Count > 0)
                {
                    Session["MensEmpresa"] = 101;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                item.EMPL_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditPlataforma(item);
                Session["EmpresaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEmpresaPlataforma(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA_PLATAFORMA item = baseApp.GetPlataformaById(id);
                item.EMPL_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditPlataforma(item);
                Session["EmpresaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirEmpresaMaquina()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 20)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0252", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            ViewBag.Maquinas = new SelectList(CarregaMaquina().OrderBy(p => p.MAQN_NM_NOME), "MAQN_CD_ID", "MAQN_NM_EXIBE");
            EMPRESA_MAQUINA item = new EMPRESA_MAQUINA();
            EmpresaMaquinaViewModel vm = Mapper.Map<EMPRESA_MAQUINA, EmpresaMaquinaViewModel>(item);
            vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
            vm.EMMA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEmpresaMaquina(EmpresaMaquinaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Maquinas = new SelectList(CarregaMaquina().OrderBy(p => p.MAQN_NM_NOME), "MAQN_CD_ID", "MAQN_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação                   
                    EMPRESA_MAQUINA item = Mapper.Map<EmpresaMaquinaViewModel, EMPRESA_MAQUINA>(vm);
                    item.EMPRESA = null;
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateMaquina(item, idAss);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 20;
                        return RedirectToAction("IncluirEmpresaMaquina", "Empresa");
                    }

                    // ReCalcula taxas médias
                    EMPRESA emp = baseApp.GetItemByAssinante(idAss);
                    List<EMPRESA_MAQUINA> maq = emp.EMPRESA_MAQUINA.ToList();
                    Decimal somaC = 0;
                    Decimal somaD = 0;
                    Decimal taxaC = 0;
                    Decimal taxaD = 0;
                    Int32 quant = 0;
                    foreach (EMPRESA_MAQUINA em in maq)
                    {
                        somaC += em.MAQUINA.MAQN_PC_CREDITO;
                        somaD += em.MAQUINA.MAQN_PC_DEBITO;
                        quant++;
                    }
                    taxaC = somaC / quant;
                    taxaD = somaD / quant;
                    emp.EMPR_VL_TAXA_MEDIA = taxaC;
                    emp.EMPR_VL_TAXA_MEDIA_DEBITO = taxaD;
                    volta = baseApp.ValidateEdit(emp, emp);

                    // Retorna
                    if ((Int32)Session["VoltarMaquina"] == 2)
                    {
                        return RedirectToAction("MontarTelaRoteiro_2", "Precificacao");
                    }
                     return RedirectToAction("VoltarAnexoEmpresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresas";
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
        public ActionResult IncluirEmpresaPlataforma()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 30)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0254", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            ViewBag.Plataformas = new SelectList(CarregaPlatEntrega().OrderBy(p => p.PLEN_NM_NOME), "PLEN_CD_ID", "PLEN_NM_NOME");
            EMPRESA_PLATAFORMA item = new EMPRESA_PLATAFORMA();
            EmpresaPlataformaViewModel vm = Mapper.Map<EMPRESA_PLATAFORMA, EmpresaPlataformaViewModel>(item);
            vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
            vm.EMPL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEmpresaPlataforma(EmpresaPlataformaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Plataformas = new SelectList(CarregaPlatEntrega().OrderBy(p => p.PLEN_NM_NOME), "PLEN_CD_ID", "PLEN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EMPRESA_PLATAFORMA item = Mapper.Map<EmpresaPlataformaViewModel, EMPRESA_PLATAFORMA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreatePlataforma(item, idAss);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 30;
                        return RedirectToAction("IncluirEmpresaPlataforma", "Empresa");
                    }

                    // Retorna
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresas";
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

        public JsonResult GetRegime(Int32 id)
        {
            var forn = baseApp.GetRegimeById(id);
            var hash = new Hashtable();
            hash.Add("aliquota", forn.RETR_VL_ALIQUOTA);
            return Json(hash);
        }

        [HttpPost]
        public JsonResult PesquisaCEP_Javascript(String cep, int tipoEnd)
        {
            // Chama servico ECT
            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(cep);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza
            var hash = new Hashtable();

            if (tipoEnd == 1)
            {
                hash.Add("EMPR_NM_ENDERECO", end.Address);
                hash.Add("EMPR_NM_UMERO", end.Complement);
                hash.Add("EMPR_NM_BAIRRO", end.District);
                hash.Add("EMPR_NM_CIDADE", end.City);
                hash.Add("UF_CD_ID", baseApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("EMPR_NR_CEP", cep);
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public ActionResult ExcluirEmpresaTicket(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA_TICKET item = baseApp.GetTicketById(id);

                List<VENDA_MENSAL> vendas = CarregaVenda().Where(p => p.TICK_CD_ID == item.TICK_CD_ID).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    vendas = vendas.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                if (vendas.Count > 0)
                {
                    Session["MensEmpresa"] = 102;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                item.EMTI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditTicket(item);
                Session["EmpresaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEmpresaTicket(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA_TICKET item = baseApp.GetTicketById(id);
                item.EMTI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditTicket(item);
                Session["EmpresaAlterada"] = 1;
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirEmpresaTicket()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 30)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0256", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            ViewBag.Tickets = new SelectList(CarregaTicket().OrderBy(p => p.TICK_NM_NOME), "TICK_CD_ID", "TICK_NM_NOME");
            EMPRESA_TICKET item = new EMPRESA_TICKET();
            EmpresaTicketViewModel vm = Mapper.Map<EMPRESA_TICKET, EmpresaTicketViewModel>(item);
            vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
            vm.EMTI_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEmpresaTicket(EmpresaTicketViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tickets = new SelectList(CarregaTicket().OrderBy(p => p.TICK_NM_NOME), "TICK_CD_ID", "TICK_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EMPRESA_TICKET item = Mapper.Map<EmpresaTicketViewModel, EMPRESA_TICKET>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateTicket(item, idAss);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 30;
                        return RedirectToAction("IncluirEmpresaTicket", "Empresa");
                    }

                    // Retorna
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresas";
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

        public ActionResult VerCustosFixos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaFixo"] = 1;
            return RedirectToAction("MontarTelaCustoFixo", "CustoFixo");
        }

        [HttpGet]
        public ActionResult IncluirEmpresaCustoVariavel()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 40)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0267", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 41)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0268", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 42)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0269", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 43)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0270", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 44)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0271", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 47)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0273", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 46)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0272", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            EMPRESA_CUSTO_VARIAVEL item = new EMPRESA_CUSTO_VARIAVEL();
            EmpresaCustoVariavelViewModel vm = Mapper.Map<EMPRESA_CUSTO_VARIAVEL, EmpresaCustoVariavelViewModel>(item);
            vm.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
            vm.EMCV_IN_ATIVO = 1;
            vm.EMCV_VL_VALOR = 0;
            vm.EMCV_PC_PERCENTUAL_VENDA = null;
            vm.EMCV_IN_TIPO = null;
            vm.EMCV_IN_VENDA = 0;
            vm.PLEN_CD_ID = null;
            vm.MAQN_CD_ID = null;
            vm.TICK_CD_ID = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEmpresaCustoVariavel(EmpresaCustoVariavelViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    vm.EMCV_VL_VALOR = vm.EMCV_PC_TAXA;
                    EMPRESA_CUSTO_VARIAVEL item = Mapper.Map<EmpresaCustoVariavelViewModel, EMPRESA_CUSTO_VARIAVEL>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateCustoVariavel(item, idAss);

                    // Verifica retorno
                    Session["MensEmpresa"] = 0;
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 40;
                        return RedirectToAction("IncluirEmpresaCustoVariavel");
                    }
                    if (volta == 2)
                    {
                        Session["MensEmpresa"] = 41;
                        return RedirectToAction("IncluirEmpresaCustoVariavel");
                    }
                    if (volta == 3)
                    {
                        Session["MensEmpresa"] = 42;
                        return RedirectToAction("IncluirEmpresaCustoVariavel");
                    }
                    if (volta == 4)
                    {
                        Session["MensEmpresa"] = 43;
                        return RedirectToAction("IncluirEmpresaCustoVariavel");
                    }
                    if (volta == 5)
                    {
                        Session["MensEmpresa"] = 44;
                        return RedirectToAction("IncluirEmpresaCustoVariavel");
                    }
                    if (volta == 6)
                    {
                        Session["MensEmpresa"] = 46;
                    }
                    if (volta == 7)
                    {
                        Session["MensEmpresa"] = 47;
                        return RedirectToAction("IncluirEmpresaCustoVariavel");
                    }

                    // Acerta custos de vendas
                    Session["VoltaAtualiza"] = 2;
                    EMPRESA empresa = (EMPRESA)Session["Empresa"];
                    RecalculoCustos calculo = new RecalculoCustos(baseApp, platApp, tkApp, antApp, maqApp, venApp, comApp);
                    Int32 voltaX = calculo.AtualizarCustos(empresa, usuarioLogado, idAss);
                    if (voltaX == 1)
                    {
                        Session["MensEmpresa"] = 10;
                        return RedirectToAction("VoltarAnexoEmpresa");
                    }
                    Session["FlagCalculoCustos"] = 1;

                    // Encerra
                    return RedirectToAction("VoltarAnexoEmpresa", "Empresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarEmpresaCustoVariavel(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0271", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 45)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0272", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            EMPRESA_CUSTO_VARIAVEL item = baseApp.GetCustoVariavelById(id);
            EmpresaCustoVariavelViewModel vm = Mapper.Map<EMPRESA_CUSTO_VARIAVEL, EmpresaCustoVariavelViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEmpresaCustoVariavel(EmpresaCustoVariavelViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EMPRESA_CUSTO_VARIAVEL item = Mapper.Map<EmpresaCustoVariavelViewModel, EMPRESA_CUSTO_VARIAVEL>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEditCustoVariavel(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 50;
                        return RedirectToAction("EditarEmpresaCustoVariavel");
                    }
                    if (volta == 2)
                    {
                        Session["MensEmpresa"] = 45;
                        return RedirectToAction("VoltarAnexoEmpresa");
                    }

                    // Recalcula
                    Session["MensEmpresa"] = 0;
                    Session["VoltaAtualiza"] = 2;
                    EMPRESA empresa = (EMPRESA)Session["Empresa"];
                    RecalculoCustos calculo = new RecalculoCustos(baseApp, platApp, tkApp, antApp, maqApp, venApp, comApp);
                    Int32 voltaX = calculo.AtualizarCustos(empresa, usuarioLogado, idAss);
                    if (voltaX == 1)
                    {
                        Session["MensEmpresa"] = 10;
                        return RedirectToAction("VoltarAnexoEmpresa");
                    }
                    Session["FlagCalculoCustos"] = 1;

                    // Encerra
                    return RedirectToAction("VoltarAnexoEmpresa", "Empresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult ExcluirEmpresaCustoVariavel(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa exclusão
            try
            {
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA_CUSTO_VARIAVEL item = baseApp.GetCustoVariavelById(id);
                item.EMCV_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditCustoVariavel(item);

                // Mensagens
                if (volta == 1)
                {
                    Session["MensEmpresa"] = 44;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                if (volta == 2)
                {
                    Session["MensEmpresa"] = 45;
                }

                // Acerta custos de vendas
                Session["VoltaAtualiza"] = 2;
                EMPRESA empresa = (EMPRESA)Session["Empresa"];
                RecalculoCustos calculo = new RecalculoCustos(baseApp, platApp, tkApp, antApp, maqApp, venApp, comApp);
                Int32 voltaX = calculo.AtualizarCustos(empresa, usuario, idAss);
                if (voltaX == 1)
                {
                    Session["MensEmpresa"] = 10;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                Session["FlagCalculoCustos"] = 1;

                // Encerra
                return RedirectToAction("VoltarAnexoEmpresa", "Empresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEmpresaCustoVariavel(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa reativação
            try
            {
                EMPRESA_CUSTO_VARIAVEL item = baseApp.GetCustoVariavelById(id);
                item.EMCV_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditCustoVariavel(item);

                // Mensagens
                if (volta == 1)
                {
                    Session["MensEmpresa"] = 45;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                if (volta == 2)
                {
                    Session["MensEmpresa"] = 46;
                }

                // Acerta custos de vendas
                Session["VoltaAtualiza"] = 2;
                EMPRESA empresa = (EMPRESA)Session["Empresa"];
                RecalculoCustos calculo = new RecalculoCustos(baseApp, platApp, tkApp, antApp, maqApp, venApp, comApp);
                Int32 voltaX = calculo.AtualizarCustos(empresa, usuario, idAss);
                if (voltaX == 1)
                {
                    Session["MensEmpresa"] = 10;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                Session["FlagCalculoCustos"] = 1;

                // Encerra
                return RedirectToAction("VoltarAnexoEmpresa", "Empresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirAntecipacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 60)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0274", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            ViewBag.Maquinas = new SelectList(CarregaMaquina().OrderBy(p => p.MAQN_NM_EXIBE), "MAQN_CD_ID", "MAQN_NM_EXIBE");
            DateTime hoje = DateTime.Today.Date;
            DateTime referencia = Convert.ToDateTime("01/" + hoje.Month.ToString() + "/" + hoje.Year.ToString()); 

            // Prepara view
            ANTECIPACAO item = new ANTECIPACAO();
            AntecipacaoViewModel vm = Mapper.Map<ANTECIPACAO, AntecipacaoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
            vm.ANTE_IN_ATIVO = 1;
            vm.ANTE_DT_INICIO = referencia;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAntecipacao(AntecipacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Maquinas = new SelectList(CarregaMaquina().OrderBy(p => p.MAQN_NM_EXIBE), "MAQN_CD_ID", "MAQN_NM_EXIBE");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ANTECIPACAO item = Mapper.Map<AntecipacaoViewModel, ANTECIPACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = antApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 60;
                        return RedirectToAction("IncluirAntecipacao");
                    }

                    // Retorna
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresas";
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
        public ActionResult EditarAntecipacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens

            // Prepara view
            Session["IdAntecipacao"] = id;
            ANTECIPACAO item = antApp.GetItemById(id);
            AntecipacaoViewModel vm = Mapper.Map<ANTECIPACAO, AntecipacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAntecipacao(AntecipacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ANTECIPACAO item = Mapper.Map<AntecipacaoViewModel, ANTECIPACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = antApp.ValidateEdit(item, item, usuarioLogado);

                    // Verifica retorno

                    // Retorna
                    Session["MensEmpresa"] = 0;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresas";
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

        public ActionResult ExcluirEmpresaAntecipacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            //; Processa exclusão
            try
            {
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                ANTECIPACAO item = antApp.GetItemById(id);
                item.ANTE_IN_ATIVO = 0;
                Int32 volta = antApp.ValidateDelete(item, usuario);
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEmpresaAntecipacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Processa reativação
            try
            {
                ANTECIPACAO item = antApp.GetItemById(id);
                item.ANTE_IN_ATIVO = 1;
                Int32 volta = antApp.ValidateReativar(item, usuario);
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<MAQUINA> CarregaMaquina()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MAQUINA> conf = new List<MAQUINA>();
            if (Session["Maquinas"] == null)
            {
                conf = maqApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["MaquinaAlterada"] == 1)
                {
                    conf = maqApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<MAQUINA>)Session["Maquinas"];
                }
            }
            Session["MaquinaAlterada"] = 0;
            Session["Maquinas"] = conf;
            return conf;
        }

        public List<PLATAFORMA_ENTREGA> CarregaPlatEntrega()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PLATAFORMA_ENTREGA> conf = new List<PLATAFORMA_ENTREGA>();
            if (Session["PlatEntregas"] == null)
            {
                conf = platApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["PlatEntregaAlterada"] == 1)
                {
                    conf = platApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<PLATAFORMA_ENTREGA>)Session["PlatEntregas"];
                }
            }
            Session["PlatEntregaAlterada"] = 0;
            Session["PlatEntregas"] = conf;
            return conf;
        }

        public List<TICKET_ALIMENTACAO> CarregaTicket()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TICKET_ALIMENTACAO> conf = new List<TICKET_ALIMENTACAO>();
            if (Session["Tickets"] == null)
            {
                conf = tkApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["TicketAlterada"] == 1)
                {
                    conf = tkApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TICKET_ALIMENTACAO>)Session["Tickets"];
                }
            }
            Session["TicketAlterada"] = 0;
            Session["Tickets"] = conf;
            return conf;
        }

        public List<UF> CarregaUF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<UF> conf = new List<UF>();
            if (Session["UF"] == null)
            {
                conf = baseApp.GetAllUF();
            }
            else
            {
                conf = (List<UF>)Session["UF"];
            }
            Session["UF"] = conf;
            return conf;
        }

        public List<REGIME_TRIBUTARIO> CarregaRegime()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<REGIME_TRIBUTARIO> conf = new List<REGIME_TRIBUTARIO>();
            if (Session["Regimes"] == null)
            {
                conf = baseApp.GetAllRegimes();
            }
            else
            {
                conf = (List<REGIME_TRIBUTARIO>)Session["Regimes"];
            }
            Session["Regimes"] = conf;
            return conf;
        }

        public List<VENDA_MENSAL> CarregaVenda()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<VENDA_MENSAL> conf = new List<VENDA_MENSAL>();
            if (Session["Vendas"] == null)
            {
                conf = venApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["VendasAlterada"] == 1)
                {
                    conf = venApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<VENDA_MENSAL>)Session["Vendas"];
                }
            }
            Session["VendasAlterada"] = 0;
            Session["Vendas"] = conf;
            return conf;
        }

        public List<EMPRESA_CUSTO_VARIAVEL> CarregaCustoVariavel()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            EMPRESA emp = baseApp.GetItemByAssinante(idAss);
            List<EMPRESA_CUSTO_VARIAVEL> conf = new List<EMPRESA_CUSTO_VARIAVEL>();
            if (Session["CustosVariaveis"] == null)
            {
                conf = baseApp.GetAllCustoVariavel(emp.EMPR_CD_ID);
            }
            else
            {
                if ((Int32)Session["CustoVariavelAlterada"] == 1)
                {
                    conf = baseApp.GetAllCustoVariavel(emp.EMPR_CD_ID);
                }
                else
                {
                    conf = (List<EMPRESA_CUSTO_VARIAVEL>)Session["CustosVariaveis"];
                }
            }
            Session["CustoVariavelAlterada"] = 0;
            Session["CustosVariaveis"] = conf;
            return conf;
        }

        [HttpGet]
        public ActionResult AtualizarCustoVenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            EMPRESA item = (EMPRESA)Session["Empresa"];
            RecalculoCustos calculo = new RecalculoCustos(baseApp, platApp, tkApp, antApp, maqApp, venApp, comApp);
            Int32 voltaX = calculo.AtualizarCustos(item, usuario, idAss);
            if (voltaX == 1)
            {
                Session["MensEmpresa"] = 10;
                return RedirectToAction("MontarTelaEmpresa");
            }
            Session["FlagCalculoCustos"] = 1;

            // Encerra
            return RedirectToAction("MontarTelaEmpresa", "Empresa");
        }

        [HttpGet]
        public Int32 AtualizarCustoDireto()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            EMPRESA item = (EMPRESA)Session["Empresa"];
            RecalculoCustos calculo = new RecalculoCustos(baseApp, platApp, tkApp, antApp, maqApp, venApp, comApp);
            Int32 voltaX = calculo.AtualizarCustos(item, usuario, idAss);
            if (voltaX == 1)
            {
                Session["MensEmpresa"] = 10;
                return voltaX;
            }
            Session["FlagCalculoCustos"] = 1;

            // Encerra
            return voltaX;
        }

        [HttpPost]
        public ActionResult UploadFotoEmpresa(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdEmpresa"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            EMPRESA item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensEmpresa"] = 11;
                return RedirectToAction("MontarTelaEmpresa");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Empresa/" + item.EMPR_CD_ID.ToString() + "/Logo/";
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
                item.EMPR_AQ_LOGO = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            Session["VoltaTela"] = 4;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("MontarTelaEmpresa");
        }

        [HttpGet]
        public ActionResult MontarTelaEmpresaGeral()
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
            if ((List<EMPRESA>)Session["ListaEmpresa"] == null)
            {
                listaMaster = CarregaEmpresa();
                Session["ListaEmpresa"] = listaMaster;
            }
            ViewBag.Listas = (List<EMPRESA>)Session["ListaEmpresa"];

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Trata mensagens
            if (Session["MensEmpresa"] != null)
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
            Session["VoltaEmpresa"] = 1;
            objeto = new EMPRESA();
            return View(objeto);
        }

        public ActionResult RetirarFiltroEmpresa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaEmpresa"] = null;
            return RedirectToAction("MontarTelaEmpresaGeral");
        }

        public ActionResult MostrarTudoEmpresa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaEmpresa"] = listaMaster;
            return RedirectToAction("MontarTelaEmpresaGeral");
        }

        public ActionResult VoltarBaseEmpresa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult IncluirEmpresa()
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
            ViewBag.Regimes = new SelectList(CarregaRegime().OrderBy(p => p.RETR_NM_NOME), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
            List<SelectListItem> matriz = new List<SelectListItem>();
            matriz.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            matriz.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Matriz = new SelectList(matriz, "Value", "Text");

            // Prepara view
            Session["EmpresaNovo"] = 0;
            EMPRESA item = new EMPRESA();
            EmpresaViewModel vm = Mapper.Map<EMPRESA, EmpresaViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.EMPR_DT_CADASTRO = DateTime.Today.Date;
            vm.EMPR_IN_ATIVO = 1;
            vm.EMPR_IN_MATRIZ = 0;
            vm.EMPR_VL_PATRIMONIO_LIQUIDO = 0;
            vm.EMPR_VL_IMPOSTO_MEI = 0;
            vm.EMPR_VL_IMPOSTO_OUTROS = 0;
            vm.EMPR_VL_COMISSAO_GERENTE = 0;
            vm.EMPR_VL_COMISSAO_VENDEDOR = 0;
            vm.EMPR_VL_COMISSAO_OUTROS = 0;
            vm.EMPR_VL_ROYALTIES = 0;
            vm.EMPR_VL_FUNDO_PROPAGANDA = 0;
            vm.EMPR_VL_FUNDO_SEGURANCA = 0;
            vm.EMPR_VL_TAXA_MEDIA = 0;
            vm.EMPR_VL_TAXA_MEDIA_DEBITO = 0;
            vm.EMPR_PC_CUSTO_VARIAVEL_VENDA = 0;
            vm.EMPR_PC_CUSTO_VARIAVEL_TOTAL = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirEmpresa(EmpresaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Regimes = new SelectList(CarregaRegime().OrderBy(p => p.RETR_NM_NOME), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
            List<SelectListItem> matriz = new List<SelectListItem>();
            matriz.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            matriz.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Matriz = new SelectList(matriz, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EMPRESA item = Mapper.Map<EmpresaViewModel, EMPRESA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Validações
                    Session["MensEmpresa"] = null;
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
                    listaMaster = new List<EMPRESA>();
                    Session["ListaEmpresa"] = null;
                    Session["IncluirEmpresa"] = 1;
                    Session["EmpresaNovo"] = item.EMPR_CD_ID;
                    Session["EmpresaAlterada"] = 1;
                    Session["IdEmpr"] = item.EMPR_CD_ID;

                    // Trata retorno
                    return RedirectToAction("VoltarAnexoEmpresaGeral");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarEmpresa(Int32 id)
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
                    Session["MensEmpresa"] = 2;
                    return RedirectToAction("MontarTelaEmpresaGeral", "Empresa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Recupera empresa
            EMPRESA item = baseApp.GetItemById(id);
            Session["Empresa"] = item;

            // Monta view
            Session["VoltaEmpresa"] = 2;
            objetoAntes = item;
            EmpresaViewModel vm = Mapper.Map<EMPRESA, EmpresaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarEmpresa(EmpresaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    EMPRESA item = Mapper.Map<EmpresaViewModel, EMPRESA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Verifica retorno

                    // Atualiza cache
                    listaMaster = new List<EMPRESA>();
                    Session["ListaEmpresa"] = null;
                    Session["EmpresaAlterada"] = 1;

                    // Trata retorno
                    return RedirectToAction("MontarTelaEmpresaGeral");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoEmpresaGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarEmpresa", new { id = (Int32)Session["IdEmpr"] });
        }

        [HttpGet]
        public ActionResult ExcluirEmpresa(Int32 id)
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
                    Session["MensEmpresa"] = 2;
                    return RedirectToAction("MontarTelaEmpresaGeral");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                EMPRESA item = baseApp.GetItemById(id);
                objetoAntes = (EMPRESA)Session["Empresa"];
                item.EMPR_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);

                Session["ListaEmpresa"] = null;
                Session["EmpresaAlterada"] = 1;
                return RedirectToAction("MontarTelaGrupoGeral");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEmpresa(Int32 id)
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupo", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }
























        public List<EMPRESA> CarregaEmpresa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<EMPRESA> conf = new List<EMPRESA>();
            if (Session["Empresas"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["EmpresaAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
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


    }
}