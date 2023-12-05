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
using Azure.Communication.Email;
using System.Net.Mime;
using ERP_Condominios_Solution.Classes;
using iText.IO.Util;
using System.Threading;
using System.Web.ModelBinding;
using Microsoft.Office.Interop.Word;

namespace ERP_Condominios_Solution.Controllers
{
    public class PrecatorioController : Controller
    {
        private readonly IPrecatorioAppService tranApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IBeneficiarioAppService beneApp;
        private readonly IHonorarioAppService honoApp;
        private readonly IContatoAppService conApp;
        private readonly ICRMAppService crmApp;
        private readonly INaturezaAppService natApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IClienteAppService cliApp;

        private String msg;
        private Exception exception;
        private String extensao;
        PRECATORIO objetoTran = new PRECATORIO();
        PRECATORIO objetoTranAntes = new PRECATORIO();
        List<PRECATORIO> listaMasterTran = new List<PRECATORIO>();
        CONTATO objetoContato = new CONTATO();
        CONTATO objetoContatoAntes = new CONTATO();
        List<CONTATO> listaMasterContato= new List<CONTATO>();

        public PrecatorioController(IPrecatorioAppService tranApps, IConfiguracaoAppService confApps, IBeneficiarioAppService beneApps, IHonorarioAppService honoApps, IContatoAppService conApps, ICRMAppService crmApps, INaturezaAppService natApps, IUsuarioAppService usuApps, IMensagemEnviadaSistemaAppService meApps, IClienteAppService cliApps)
        {
            tranApp = tranApps;
            confApp = confApps;
            beneApp = beneApps;
            honoApp = honoApps;
            conApp = conApps;
            crmApp = crmApps;
            natApp = natApps;
            usuApp = usuApps;
            meApp = meApps;
            cliApp = cliApps;
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
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }


        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaPrecatorio"] == 40)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult BuscaNumero(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<PRECATORIO> clientes = CarregaPrecatorio();
            Session["Precatorios"] = clientes;

            if (nome != null)
            {
                List<PRECATORIO> lstCliente = clientes.Where(x => x.PREC_NM_PRECATORIO != null && x.PREC_NM_PRECATORIO.ToLower().Contains(nome.ToLower())).ToList<PRECATORIO>();
                lstCliente = lstCliente.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.PREC_CD_ID);
                        result.Add("text", item.PREC_NM_PRECATORIO + " - " + item.PREC_NM_REQUERENTE);
                        listResult.Add(result);
                    }
                }
            }
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaPrecatorio()
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
                    Session["MensPrecatorio"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if ((List<PRECATORIO>)Session["ListaPrecatorio"] == null)
            {
                listaMasterTran = CarregaPrecatorio();
                Session["ListaPrecatorio"] = listaMasterTran;
            }

            List<PRECATORIO> lista = (List<PRECATORIO>)Session["ListaPrecatorio"];
            ViewBag.Listas = lista;
            ViewBag.Title = "Precatorio";
            ViewBag.Natureza = new SelectList(CarregaNatureza().OrderBy(p => p.NATU_NM_NOME), "NATU_CD_ID", "NATU_NM_NOME");
            ViewBag.TRF = new SelectList(CarregaTRF().OrderBy(p => p.TRF1_NM_NOME), "TRF1_CD_ID", "TRF1_NM_NOME");
            ViewBag.Estado = new SelectList(CarregaEstado().OrderBy(p => p.PRES_NM_NOME), "PRES_CD_ID", "PRES_NM_NOME");
            ViewBag.Beneficiario = new SelectList(CarregaBeneficiario().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Advogado = new SelectList(CarregaHonorario().OrderBy(p => p.HONO_NM_NOME), "HONO_CD_ID", "HONO_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> crm = new List<SelectListItem>();
            crm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            crm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CRM = new SelectList(crm, "Value", "Text");
            List<SelectListItem> pesquisa = new List<SelectListItem>();
            pesquisa.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pesquisa.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Pesquisa = new SelectList(pesquisa, "Value", "Text");
            List<SelectListItem> sit = new List<SelectListItem>();
            sit.Add(new SelectListItem() { Text = "Pago", Value = "1" });
            sit.Add(new SelectListItem() { Text = "Não Pago", Value = "2" });
            sit.Add(new SelectListItem() { Text = "Pago Parcial", Value = "3" });
            ViewBag.Situacao = new SelectList(sit, "Value", "Text");

            // Contagem
            ViewBag.ContagemFiltro = lista.Count;

            // Totais
            ViewBag.ValorInscProp = lista.Sum(p => p.PREC_VL_VALOR_INSCRITO_PROPOSTA);
            ViewBag.ValorPrincipal = lista.Sum(p => p.PREC_VL_BEN_VALOR_PRINCIPAL);
            ViewBag.Juros = lista.Sum(p => p.PREC_VL_JUROS);
            ViewBag.Valor_PSS = lista.Sum(p => p.PREC_VL_VALOR_INICIAL_PSS);
            ViewBag.Valor_Req = lista.Sum(p => p.PREC_VL_BEN_VALOR_REQUISITADO);

            if (Session["MensPrecatorio"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPrecatorio"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0145", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPrecatorio"] == 10)
                {
                    ModelState.AddModelError("", "Foram processados e incluídos " + ((Int32)Session["Conta"]).ToString() + " precatórios");
                    ModelState.AddModelError("", "Foram processados e ajustados " + ((Int32)Session["Ajuste"]).ToString() + " precatórios");
                    ModelState.AddModelError("", "Foram processados e rejeitados " + ((Int32)Session["Falha"]).ToString() + " precatórios");
                }
                if ((Int32)Session["MensPrecatorio"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPrecatorio"] == 200)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0278", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPrecatorio"] == 999)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0279", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoTran = new PRECATORIO();
            Session["MensPrecatorio"] = 0;
            Session["VoltaPrecatorio"] = 1;
            Session["VoltaPrecCRM"] = 0;
            if (Session["FiltroPrecatorio"] != null)
            {
                objetoTran = (PRECATORIO)Session["FiltroPrecatorio"];
            }
            return View(objetoTran);
        }

        public ActionResult RetirarFiltroPrecatorio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaPrecatorio"] = null;
            Session["FiltroPrecatorio"] = null;
            return RedirectToAction("MontarTelaPrecatorio");
        }

        public ActionResult MostrarTudoPrecatorio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMasterTran = tranApp.GetAllItensAdm();
            Session["FiltroPrecatorio"] = null;
            Session["ListaPrecatorio"] = listaMasterTran;
            return RedirectToAction("MontarTelaPrecatorio");
        }

        [HttpPost]
        public ActionResult FiltrarPrecatorio(PRECATORIO item)
        {            
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                // Executa a operação
                List<PRECATORIO> listaObj = new List<PRECATORIO>();
                Tuple<Int32, List<PRECATORIO>, Boolean> volta = tranApp.ExecuteFilterTuple(item.TRF1_CD_ID, item.BENE_CD_ID, item.HONO_CD_ID, item.NATU_CD_ID, item.PRES_CD_ID, item.PREC_NM_REQUERENTE, item.PREC_NR_ANO, item.PREC_IN_FOI_IMPORTADO_PIPE, item.PREC_IN_FOI_PESQUISADO, item.PREC_VL_BEN_VALOR_REQUISITADO, item.PREC_VL_HON_VALOR_REQUISITADO, item.PREC_IN_SITUACAO_ATUAL, 1);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPrecatorio"] = 1;
                    return RedirectToAction("MontarTelaPrecatorio");
                }

                // Sucesso
                listaMasterTran = volta.Item2;
                Session["ListaPrecatorio"] = listaMasterTran;
                return RedirectToAction("MontarTelaPrecatorio");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Precatório";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBasePrecatorio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaPrecCRM"] == 1)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["VoltaMensagem"] == 40)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            if ((Int32)Session["VoltaPrecatorio"] == 10)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            return RedirectToAction("MontarTelaPrecatorio");
        }

        [HttpGet]
        public ActionResult IncluirPrecatorio()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPrecatorio", "Precatorio");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Natureza = new SelectList(CarregaNatureza().OrderBy(p => p.NATU_NM_NOME), "NATU_CD_ID", "NATU_NM_NOME");
            ViewBag.TRF = new SelectList(CarregaTRF().OrderBy(p => p.TRF1_NM_NOME), "TRF1_CD_ID", "TRF1_NM_NOME");
            ViewBag.Estado = new SelectList(CarregaEstado().OrderBy(p => p.PRES_NM_NOME), "PRES_CD_ID", "PRES_NM_NOME");
            ViewBag.Beneficiario = new SelectList(CarregaBeneficiario().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Advogado = new SelectList(CarregaHonorario().OrderBy(p => p.HONO_NM_NOME), "HONO_CD_ID", "HONO_NM_NOME");
            ViewBag.Usuario = new SelectList(CarregaUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            List<SelectListItem> proc = new List<SelectListItem>();
            proc.Add(new SelectListItem() { Text = "PRC", Value = "1" });
            proc.Add(new SelectListItem() { Text = "RPV", Value = "2" });
            ViewBag.Procedimento = new SelectList(proc, "Value", "Text");
            List<SelectListItem> IRBen = new List<SelectListItem>();
            IRBen.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRBen.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRBeneficiario = new SelectList(IRBen, "Value", "Text");
            List<SelectListItem> IRHon = new List<SelectListItem>();
            IRHon.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRHon.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRAdvogado = new SelectList(IRHon, "Value", "Text");
            List<SelectListItem> pesq = new List<SelectListItem>();
            pesq.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pesq.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Pesquisado = new SelectList(pesq, "Value", "Text");
            List<SelectListItem> crm = new List<SelectListItem>();
            crm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            crm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CRM = new SelectList(crm, "Value", "Text");
            List<SelectListItem> sit = new List<SelectListItem>();
            sit.Add(new SelectListItem() { Text = "Pago", Value = "1" });
            sit.Add(new SelectListItem() { Text = "Não Pago", Value = "2" });
            sit.Add(new SelectListItem() { Text = "Pago Parcial", Value = "3" });
            ViewBag.Situacao = new SelectList(sit, "Value", "Text");
            List<SelectListItem> bloc = new List<SelectListItem>();
            bloc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            bloc.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Bloqueada = new SelectList(bloc, "Value", "Text");
            List<SelectListItem> loa = new List<SelectListItem>();
            loa.Add(new SelectListItem() { Text = "Bot", Value = "1" });
            loa.Add(new SelectListItem() { Text = "LOA", Value = "2" });
            ViewBag.LOA = new SelectList(loa, "Value", "Text");
            // Prepara view
            PRECATORIO item = new PRECATORIO();
            PrecatorioViewModel vm = Mapper.Map<PRECATORIO, PrecatorioViewModel>(item);
            vm.PREC_DT_CADASTRO = DateTime.Today.Date;
            vm.PREC_IN_ATIVO = 1;
            vm.PREC_VL_VALOR_INSCRITO_PROPOSTA = 0;
            vm.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
            vm.PREC_VL_JUROS = 0;
            vm.PREC_VL_VALOR_INICIAL_PSS = 0;
            vm.PREC_VL_BEN_VALOR_REQUISITADO = 0;
            vm.PREC_VL_HON_VALOR_PRINCIPAL = 0;
            vm.PREC_VL_HON_JUROS = 0;
            vm.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
            vm.PREC_VL_HON_VALOR_REQUISITADO = 0;
            vm.PREC_VL_RRA = 0;
            vm.PREC_PC_RRA = 0;
            vm.PREC_NM_NOME = "-";
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirPrecatorio(PrecatorioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ViewBag.Natureza = new SelectList(CarregaNatureza().OrderBy(p => p.NATU_NM_NOME), "NATU_CD_ID", "NATU_NM_NOME");
            ViewBag.TRF = new SelectList(CarregaTRF().OrderBy(p => p.TRF1_NM_NOME), "TRF1_CD_ID", "TRF1_NM_NOME");
            ViewBag.Estado = new SelectList(CarregaEstado().OrderBy(p => p.PRES_NM_NOME), "PRES_CD_ID", "PRES_NM_NOME");
            ViewBag.Beneficiario = new SelectList(CarregaBeneficiario().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Advogado = new SelectList(CarregaHonorario().OrderBy(p => p.HONO_NM_NOME), "HONO_CD_ID", "HONO_NM_NOME");
            ViewBag.Usuario = new SelectList(CarregaUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> proc = new List<SelectListItem>();
            proc.Add(new SelectListItem() { Text = "PRC", Value = "1" });
            proc.Add(new SelectListItem() { Text = "RPV", Value = "2" });
            ViewBag.Procedimento = new SelectList(proc, "Value", "Text");
            List<SelectListItem> IRBen = new List<SelectListItem>();
            IRBen.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRBen.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRBeneficiario = new SelectList(IRBen, "Value", "Text");
            List<SelectListItem> IRHon = new List<SelectListItem>();
            IRHon.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRHon.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRAdvogado = new SelectList(IRHon, "Value", "Text");
            List<SelectListItem> pesq = new List<SelectListItem>();
            pesq.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pesq.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Pesquisado = new SelectList(pesq, "Value", "Text");
            List<SelectListItem> crm = new List<SelectListItem>();
            crm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            crm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CRM = new SelectList(crm, "Value", "Text");
            List<SelectListItem> sit = new List<SelectListItem>();
            sit.Add(new SelectListItem() { Text = "Pago", Value = "1" });
            sit.Add(new SelectListItem() { Text = "Não Pago", Value = "2" });
            sit.Add(new SelectListItem() { Text = "Pago Parcial", Value = "3" });
            ViewBag.Situacao = new SelectList(sit, "Value", "Text");
            List<SelectListItem> bloc = new List<SelectListItem>();
            bloc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            bloc.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Bloqueada = new SelectList(bloc, "Value", "Text");
            List<SelectListItem> loa = new List<SelectListItem>();
            loa.Add(new SelectListItem() { Text = "Bot", Value = "1" });
            loa.Add(new SelectListItem() { Text = "LOA", Value = "2" });
            ViewBag.LOA = new SelectList(loa, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRECATORIO item = Mapper.Map<PrecatorioViewModel, PRECATORIO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = tranApp.ValidateCreate(item, usuario);

                    // Verifica retorno

                    // Cria pastas
                    String caminho = "/Imagens/1" + "/Precatorios/" + item.PREC_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterTran = new List<PRECATORIO>();
                    Session["ListaPrecatorio"] = null;
                    Session["IdPrecatorio"] = item.PREC_CD_ID;

                    if (Session["FileQueueTrans"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueTrans"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueuePrecatorio(file);
                            }
                        }
                        Session["FileQueueTrans"] = null;
                    }
                    return RedirectToAction("IncluirPrecatorio");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Precatório";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerPrecatorio(Int32 id)
        {

            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            PRECATORIO item = tranApp.GetItemById(id);
            objetoTranAntes = item;
            Session["Precatorio"] = item;
            Session["IdPrecatorio"] = id;
            Session["VoltaComent"] = 2;

            ViewBag.Natureza = new SelectList(CarregaNatureza().OrderBy(p => p.NATU_NM_NOME), "NATU_CD_ID", "NATU_NM_NOME");
            ViewBag.TRF = new SelectList(CarregaTRF().OrderBy(p => p.TRF1_NM_NOME), "TRF1_CD_ID", "TRF1_NM_NOME");
            ViewBag.Estado = new SelectList(CarregaEstado().OrderBy(p => p.PRES_NM_NOME), "PRES_CD_ID", "PRES_NM_NOME");
            ViewBag.Beneficiario = new SelectList(CarregaBeneficiario().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Advogado = new SelectList(CarregaHonorario().OrderBy(p => p.HONO_NM_NOME), "HONO_CD_ID", "HONO_NM_NOME");
            ViewBag.Usuario = new SelectList(CarregaUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> proc = new List<SelectListItem>();
            proc.Add(new SelectListItem() { Text = "PRC", Value = "1" });
            proc.Add(new SelectListItem() { Text = "RPV", Value = "2" });
            ViewBag.Procedimento = new SelectList(proc, "Value", "Text");
            List<SelectListItem> IRBen = new List<SelectListItem>();
            IRBen.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRBen.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRBeneficiario = new SelectList(IRBen, "Value", "Text");
            List<SelectListItem> IRHon = new List<SelectListItem>();
            IRHon.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRHon.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRAdvogado = new SelectList(IRHon, "Value", "Text");
            List<SelectListItem> pesq = new List<SelectListItem>();
            pesq.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pesq.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Pesquisado = new SelectList(pesq, "Value", "Text");
            List<SelectListItem> crm = new List<SelectListItem>();
            crm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            crm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CRM = new SelectList(crm, "Value", "Text");
            List<SelectListItem> sit = new List<SelectListItem>();
            sit.Add(new SelectListItem() { Text = "Pago", Value = "1" });
            sit.Add(new SelectListItem() { Text = "Não Pago", Value = "2" });
            sit.Add(new SelectListItem() { Text = "Pago Parcial", Value = "3" });
            ViewBag.Situacao = new SelectList(sit, "Value", "Text");
            List<SelectListItem> bloc = new List<SelectListItem>();
            bloc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            bloc.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Bloqueada = new SelectList(bloc, "Value", "Text");
            List<SelectListItem> loa = new List<SelectListItem>();
            loa.Add(new SelectListItem() { Text = "Bot", Value = "1" });
            loa.Add(new SelectListItem() { Text = "LOA", Value = "2" });
            ViewBag.LOA = new SelectList(loa, "Value", "Text");
            PrecatorioViewModel vm = Mapper.Map<PRECATORIO, PrecatorioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExportarCRM(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Recupera precatorio
            PRECATORIO item = tranApp.GetItemById(id);
            objetoTranAntes = item;
            Session["Precatorio"] = item;
            Session["IdPrecatorio"] = id;
            Session["VoltaComent"] = 2;

            // Monta CRM
            CRM crm = new CRM();
            crm.ASSI_CD_ID = 1;
            crm.CRM1_AQ_IMAGEM = "~/Images/icone_imagem.jpg";
            crm.CRM1_DS_DESCRICAO = "Importado do precatório " + item.PREC_NM_PRECATORIO + " em " + DateTime.Today.Date.ToLongDateString();
            crm.CRM1_DT_CRIACAO = DateTime.Today.Date;
            crm.CRM1_IN_ATIVO = 1;
            crm.CRM1_IN_ESTRELA = 1;
            crm.CRM1_IN_STATUS = 1;
            crm.CRM1_NM_NOME = "Precatório " + item.PREC_NM_PRECATORIO;
            crm.CRM1_NR_TEMPERATURA = 1;
            crm.ORIG_CD_ID = 1;
            crm.PREC_CD_ID = item.PREC_CD_ID;
            crm.USUA_CD_ID = usuario.USUA_CD_ID;
            crm.EMPR_CD_ID = 3;
            crm.CRM1_NM_CAMPANHA = "-";
            crm.CRM1_VL_VALOR_INICIAL = item.PREC_VL_BEN_VALOR_REQUISITADO;
            Int32 volta = crmApp.ValidateCreate(crm, usuario);

            // Acerta precatorio
            item.PREC_IN_FOI_IMPORTADO_PIPE = 1;
            Int32 volta1 = tranApp.ValidateEdit(item, item);
            Session["ListaPrecatorio"] = null;
            return RedirectToAction("VoltarBasePrecatorio");
        }

        [HttpGet]
        public ActionResult EditarPrecatorio(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPrecatorio", "Precatorio");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Natureza = new SelectList(CarregaNatureza().OrderBy(p => p.NATU_NM_NOME), "NATU_CD_ID", "NATU_NM_NOME");
            ViewBag.TRF = new SelectList(CarregaTRF().OrderBy(p => p.TRF1_NM_NOME), "TRF1_CD_ID", "TRF1_NM_NOME");
            ViewBag.Estado = new SelectList(CarregaEstado().OrderBy(p => p.PRES_NM_NOME), "PRES_CD_ID", "PRES_NM_NOME");
            ViewBag.Beneficiario = new SelectList(CarregaBeneficiario().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Advogado = new SelectList(CarregaHonorario().OrderBy(p => p.HONO_NM_NOME), "HONO_CD_ID", "HONO_NM_NOME");
            ViewBag.Usuario = new SelectList(CarregaUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> proc = new List<SelectListItem>();
            proc.Add(new SelectListItem() { Text = "PRC", Value = "1" });
            proc.Add(new SelectListItem() { Text = "RPV", Value = "2" });
            ViewBag.Procedimento = new SelectList(proc, "Value", "Text");
            List<SelectListItem> IRBen = new List<SelectListItem>();
            IRBen.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRBen.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRBeneficiario = new SelectList(IRBen, "Value", "Text");
            List<SelectListItem> IRHon = new List<SelectListItem>();
            IRHon.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRHon.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRAdvogado = new SelectList(IRHon, "Value", "Text");
            List<SelectListItem> pesq = new List<SelectListItem>();
            pesq.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pesq.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Pesquisado = new SelectList(pesq, "Value", "Text");
            List<SelectListItem> crm = new List<SelectListItem>();
            crm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            crm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CRM = new SelectList(crm, "Value", "Text");
            List<SelectListItem> sit = new List<SelectListItem>();
            sit.Add(new SelectListItem() { Text = "Pago", Value = "1" });
            sit.Add(new SelectListItem() { Text = "Não Pago", Value = "2" });
            sit.Add(new SelectListItem() { Text = "Pago Parcial", Value = "3" });
            ViewBag.Situacao = new SelectList(sit, "Value", "Text");
            List<SelectListItem> bloc = new List<SelectListItem>();
            bloc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            bloc.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Bloqueada = new SelectList(bloc, "Value", "Text");
            List<SelectListItem> loa = new List<SelectListItem>();
            loa.Add(new SelectListItem() { Text = "Bot", Value = "1" });
            loa.Add(new SelectListItem() { Text = "LOA", Value = "2" });
            ViewBag.LOA = new SelectList(loa, "Value", "Text");

            PRECATORIO item = tranApp.GetItemById(id);
            objetoTranAntes = item;
            Session["Precatorio"] = item;
            Session["IdPrecatorio"] = id;
            Session["VoltaComent"] = 1;
            PrecatorioViewModel vm = Mapper.Map<PRECATORIO, PrecatorioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarPrecatorio(PrecatorioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Natureza = new SelectList(CarregaNatureza().OrderBy(p => p.NATU_NM_NOME), "NATU_CD_ID", "NATU_NM_NOME");
            ViewBag.TRF = new SelectList(CarregaTRF().OrderBy(p => p.TRF1_NM_NOME), "TRF1_CD_ID", "TRF1_NM_NOME");
            ViewBag.Estado = new SelectList(CarregaEstado().OrderBy(p => p.PRES_NM_NOME), "PRES_CD_ID", "PRES_NM_NOME");
            ViewBag.Beneficiario = new SelectList(CarregaBeneficiario().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Advogado = new SelectList(CarregaHonorario().OrderBy(p => p.HONO_NM_NOME), "HONO_CD_ID", "HONO_NM_NOME");
            ViewBag.Usuario = new SelectList(CarregaUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> proc = new List<SelectListItem>();
            proc.Add(new SelectListItem() { Text = "PRC", Value = "1" });
            proc.Add(new SelectListItem() { Text = "RPV", Value = "2" });
            ViewBag.Procedimento = new SelectList(proc, "Value", "Text");
            List<SelectListItem> IRBen = new List<SelectListItem>();
            IRBen.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRBen.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRBeneficiario = new SelectList(IRBen, "Value", "Text");
            List<SelectListItem> IRHon = new List<SelectListItem>();
            IRHon.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRHon.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRAdvogado = new SelectList(IRHon, "Value", "Text");
            List<SelectListItem> pesq = new List<SelectListItem>();
            pesq.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pesq.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Pesquisado = new SelectList(pesq, "Value", "Text");
            List<SelectListItem> crm = new List<SelectListItem>();
            crm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            crm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CRM = new SelectList(crm, "Value", "Text");
            List<SelectListItem> sit = new List<SelectListItem>();
            sit.Add(new SelectListItem() { Text = "Pago", Value = "1" });
            sit.Add(new SelectListItem() { Text = "Não Pago", Value = "2" });
            sit.Add(new SelectListItem() { Text = "Pago Parcial", Value = "3" });
            ViewBag.Situacao = new SelectList(sit, "Value", "Text");
            List<SelectListItem> bloc = new List<SelectListItem>();
            bloc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            bloc.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Bloqueada = new SelectList(bloc, "Value", "Text");
            List<SelectListItem> loa = new List<SelectListItem>();
            loa.Add(new SelectListItem() { Text = "Bot", Value = "1" });
            loa.Add(new SelectListItem() { Text = "LOA", Value = "2" });
            ViewBag.LOA = new SelectList(loa, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    PRECATORIO item = Mapper.Map<PrecatorioViewModel, PRECATORIO>(vm);
                    Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTran = new List<PRECATORIO>();
                    Session["ListaPrecatorio"] = null;
                    if ((Int32)Session["VoltaClienteCRM"] == 11)
                    {
                        Session["VoltaClienteCRM"] = 0;
                        return RedirectToAction("IncluirProcessoCRM", "CRM");
                    }

                    // Retorno
                    if ((Int32)Session["VoltaPrecCRM"] == 1)                 
                    {
                        return RedirectToAction("MontarTelaCRM", "CRM");
                    }
                    return RedirectToAction("VoltarBasePrecatorio");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Precatório";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirPrecatorio(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPrecatorio", "Precatorio");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                PRECATORIO item = tranApp.GetItemById(id);
                Session["Precatorio"] = item;
                item.PREC_IN_ATIVO = 0;
                Int32 volta = tranApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensPrecatorio"] = 2;
                    return RedirectToAction("MontarTelaPrecatorio", "Precatorio");
                }
                listaMasterTran = new List<PRECATORIO>();
                Session["ListaPrecatorio"] = null;
                return RedirectToAction("MontarTelaPrecatorio");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Precatório";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarPrecatorio(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPrecatorio", "Precatorio");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                PRECATORIO item = tranApp.GetItemById(id);
                Session["Precatorio"] = item;
                item.PREC_IN_ATIVO = 1;
                Int32 volta = tranApp.ValidateReativar(item, usuario);
                listaMasterTran = new List<PRECATORIO>();
                Session["ListaPrecatorio"] = null;
                return RedirectToAction("MontarTelaPrecatorio");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Precatório";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPrecatorio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            PRECATORIO_ANEXO item = tranApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoPrecatorioAudio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            PRECATORIO_ANEXO item = tranApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoPrecatorio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            return RedirectToAction("EditarPrecatorio", new { id = (Int32)Session["IdPrecatorio"] });
        }

        public FileResult DownloadPrecatorio(Int32 id)
        {
            PRECATORIO_ANEXO item = tranApp.GetAnexoById(id);
            String arquivo = item.PRAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg") || arquivo.Contains(".jpeg"))
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

            Session["FileQueueTrans"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueuePrecatorio(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdPrecatorio"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensPrecatorio"] = 5;
                return RedirectToAction("VoltarAnexoPrecatorio");
            }

            PRECATORIO item = tranApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensPrecatorio"] = 6;
                return RedirectToAction("VoltarAnexoPrecatorio");
            }
            String caminho = "/Imagens/1" + "/Precatorios/" + item.PREC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PRECATORIO_ANEXO foto = new PRECATORIO_ANEXO();
            foto.PRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PRAN_DT_ANEXO = DateTime.Today;
            foto.PRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.PRAN_IN_TIPO = tipo;
            foto.PRAN_NM_TITULO = fileName;
            foto.PREC_CD_ID = item.PREC_CD_ID;
            item.PRECATORIO_ANEXO.Add(foto);
            objetoTranAntes = item;
            
            Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes);
            return RedirectToAction("VoltarAnexoPrecatorio");
        }

        [HttpPost]
        public ActionResult UploadFilePrecatorio(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdPrecatorio"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensPrecatorio"] = 5;
                return RedirectToAction("VoltarAnexoPrecatorio");
            }

            PRECATORIO item = tranApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensPrecatorio"] = 6;
                return RedirectToAction("VoltarAnexoPrecatorio");
            }
            String caminho = "/Imagens/1" + "/Precatorios/" + item.PREC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PRECATORIO_ANEXO foto = new PRECATORIO_ANEXO();
            foto.PRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PRAN_DT_ANEXO = DateTime.Today;
            foto.PRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.PRAN_IN_TIPO = tipo;
            foto.PRAN_NM_TITULO = fileName;
            foto.PREC_CD_ID = item.PREC_CD_ID;

            item.PRECATORIO_ANEXO.Add(foto);
            objetoTranAntes = item;
            Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes);
            return RedirectToAction("VoltarAnexoPrecatorio");
        }

        [HttpGet]
        public ActionResult IncluirComentario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdPrecatorio"];
            PRECATORIO item = tranApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            PRECATORIO_ANOTACAO coment = new PRECATORIO_ANOTACAO();
            PrecatorioComentarioViewModel vm = Mapper.Map<PRECATORIO_ANOTACAO, PrecatorioComentarioViewModel>(coment);
            vm.PRAT_DT_ANOTACAO = DateTime.Now;
            vm.PRAT_IN_ATIVO = 1;
            vm.PREC_CD_ID = item.PREC_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentario(PrecatorioComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdPrecatorio"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRECATORIO_ANOTACAO item = Mapper.Map<PrecatorioComentarioViewModel, PRECATORIO_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRECATORIO not = tranApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.PRECATORIO_ANOTACAO.Add(item);
                    objetoTranAntes = not;
                    Int32 volta = tranApp.ValidateEdit(not, objetoTranAntes);

                    // Verifica retorno

                    // Sucesso
                    if ((Int32)Session["VoltaComent"] == 1)
                    {
                        return RedirectToAction("EditarPrecatorio", new { id = idNot });
                    }
                    Session["VoltaComent"] = 0;
                    return RedirectToAction("VerPrecatorio", new { id = idNot });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Precatório";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarComentario(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            Session["VoltaTela"] = 2;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            PRECATORIO_ANOTACAO item = tranApp.GetComentarioById(id);
            PRECATORIO cli = tranApp.GetItemById(item.PREC_CD_ID);
            PrecatorioComentarioViewModel vm = Mapper.Map<PRECATORIO_ANOTACAO, PrecatorioComentarioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarComentario(PrecatorioComentarioViewModel vm)
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
                    PRECATORIO_ANOTACAO item = Mapper.Map<PrecatorioComentarioViewModel, PRECATORIO_ANOTACAO>(vm);
                    Int32 volta = tranApp.ValidateEditAnotacao(item);

                    // Verifica retorno
                    Session["PrecatorioAlterada"] = 1;
                    Session["NivelCliente"] = 5;
                    return RedirectToAction("VoltarAnexoPrecatorio");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Precatorio";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirComentario(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                Session["VoltaTela"] = 2;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PRECATORIO_ANOTACAO item = tranApp.GetComentarioById(id);
                item.PRAT_IN_ATIVO = 0;
                Int32 volta = tranApp.ValidateEditAnotacao(item);
                Session["PrecatorioAlterada"] = 1;
                Session["NivelBeneficiario"] = 5;
                return RedirectToAction("VoltarAnexoPrecatorio");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Precatorio";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Precatorio", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioLista()
        {            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "PrecatorioLista" + "_" + data + ".pdf";
            List<PRECATORIO> lista = (List<PRECATORIO>)Session["ListaPrecatorio"];
            PRECATORIO filtro = (PRECATORIO)Session["FiltroPrecatorio"];
            iTextSharp.text.Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            iTextSharp.text.Paragraph line = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/Base/LogoRidolfi.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Precatórios - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            iTextSharp.text.Paragraph line1 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.GREEN, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new iTextSharp.text.Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 50f, 80f, 80f, 80f, 50f, 80f, 80f, 80f, 30f, 60f, 70f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Precatórios selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 11;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Tribunal", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Precatório", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Proc.Execução", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Proc.Original", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Procedimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Requerente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Requerido", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Beneficiário", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Ano", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor (R$)", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Situação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (PRECATORIO item in lista)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.TRF.TRF1_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PREC_NM_PRECATORIO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PREC_NM_PROC_EXECUCAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PREC_NM_PROCESSO_ORIGEM, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PREC_SG_PROCEDIMENTO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PREC_NM_REQUERENTE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PREC_NM_REQUERIDO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.BENEFICIARIO != null)
                {
                    cell = new PdfPCell(new iTextSharp.text.Paragraph(item.BENEFICIARIO.BENE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new iTextSharp.text.Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PREC_NR_ANO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.PREC_VL_BEN_VALOR_REQUISITADO != null)
                {
                    cell = new PdfPCell(new iTextSharp.text.Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PREC_VL_BEN_VALOR_REQUISITADO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else
                {
                    cell = new PdfPCell(new iTextSharp.text.Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);
                if (item.PREC_IN_SITUACAO_ATUAL == 1)
                {
                    cell = new PdfPCell(new iTextSharp.text.Paragraph("Pago", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.PREC_IN_SITUACAO_ATUAL == 3)
                {
                    cell = new PdfPCell(new iTextSharp.text.Paragraph("Parcial", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else 
                {
                    cell = new PdfPCell(new iTextSharp.text.Paragraph("Não Pago", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            iTextSharp.text.Paragraph line2 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.TRF1_CD_ID != null)
                {
                    parametros += "Tribunal: " + filtro.TRF1_CD_ID;
                    ja = 1;
                }
                if (filtro.BENE_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Beneficiário: " + filtro.BENE_CD_ID;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Beneficiário: " + filtro.BENE_CD_ID;
                    }
                }
                if (filtro.HONO_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Advogado: " + filtro.HONO_CD_ID;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Advogado: " + filtro.HONO_CD_ID;
                    }
                }
                if (filtro.NATU_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Natureza: " + filtro.NATU_CD_ID;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Natureza: " + filtro.NATU_CD_ID;
                    }
                }
                if (filtro.PRES_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Estado: " + filtro.PRES_CD_ID;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Estado: " + filtro.PRES_CD_ID;
                    }
                }
                if (filtro.PREC_NM_REQUERENTE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Requerente: " + filtro.PREC_NM_REQUERENTE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Requerente: " + filtro.PREC_NM_REQUERENTE;
                    }
                }
                if (filtro.PREC_NR_ANO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Ano: " + filtro.PREC_NR_ANO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Ano: " + filtro.PREC_NR_ANO;
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
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            iTextSharp.text.Paragraph line3 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
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

            return RedirectToAction("MontarTelaPrecatorio");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            PRECATORIO tran = tranApp.GetById((Int32)Session["IdPrecatorio"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Precatorio_" + tran.PREC_CD_ID.ToString() + "_" + data + ".pdf";
            iTextSharp.text.Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
            iTextSharp.text.Font meuFont4 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);
            iTextSharp.text.Font meuFont5 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);

            // Cria documento
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            iTextSharp.text.Paragraph line1 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/Base/LogoRidolfi.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Precatório - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new iTextSharp.text.Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 6;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.PREC_IN_SITUACAO_ATUAL == 1)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Situação Atual: Pago", meuFont3));
                cell.Border = 0;
                cell.Colspan = 6;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (tran.PREC_IN_SITUACAO_ATUAL == 3)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Situação Atual: Pago Parcial", meuFont5));
                cell.Border = 0;
                cell.Colspan = 6;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Situação Atual: Não Pago", meuFont4));
                cell.Border = 0;
                cell.Colspan = 6;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Tribunal: " + tran.TRF.TRF1_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Número: " + tran.PREC_NM_PRECATORIO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Processo Origem: " + tran.PREC_NM_PROCESSO_ORIGEM, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Ano: " + tran.PREC_NR_ANO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (tran.NATUREZA != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Natureza: " + tran.NATUREZA.NATU_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Natureza: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Requerente: " + tran.PREC_NM_REQUERENTE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Requerido: " + tran.PREC_NM_REQUERIDO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Advogado: " + tran.PREC_NM_ADVOGADO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Deprecante: " + tran.PREC_NM_DEPRECANTE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Situação Requisição: " + tran.PREC_NM_SITUACAO_REQUISICAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Procedimento: " + tran.PREC_SG_PROCEDIMENTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.PREC_VL_VALOR_INSCRITO_PROPOSTA != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Insc.Proposta (R$): " + CrossCutting.Formatters.DecimalFormatter(tran.PREC_VL_VALOR_INSCRITO_PROPOSTA.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Insc.Proposta (R$): -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Processo Execução: " + tran.PREC_NM_PROC_EXECUCAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Ofício Requisitório: " + tran.PREC_NM_OFICIO_REQUISITORIO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Assunto: " + tran.PREC_NM_ASSUNTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 6;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Beneficiário", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 6;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.BENEFICIARIO != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Nome: " + tran.BENEFICIARIO.BENE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CPF: " + tran.BENEFICIARIO.BENE_NR_CPF, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CNPJ: " + tran.BENEFICIARIO.BENE_NR_CNPJ, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("E-Mail: " + tran.BENEFICIARIO.BENE_EM_EMAIL, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Nome: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CPF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CNPJ: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("E-Mail: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (tran.PREC_DT_BEN_DATABASE != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Data: " + tran.PREC_DT_BEN_DATABASE.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Data: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (tran.PREC_VL_BEN_VALOR_PRINCIPAL != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Principal (R$): " + CrossCutting.Formatters.DecimalFormatter(tran.PREC_VL_BEN_VALOR_PRINCIPAL.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Principal (R$): -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (tran.PREC_VL_JUROS != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Juros (R$): " + CrossCutting.Formatters.DecimalFormatter(tran.PREC_VL_JUROS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Juros (R$): -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new iTextSharp.text.Paragraph("IR RRA: " + tran.PREC_SG_BEN_IR_RRA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.PREC_VL_VALOR_INICIAL_PSS != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Inic. PSS   (R$): " + CrossCutting.Formatters.DecimalFormatter(tran.PREC_VL_VALOR_INICIAL_PSS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Inic. PSS (R$): -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (tran.PREC_BEN_MESES_EXE_ANTERIOR != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Meses Execução Anterior: " + tran.PREC_BEN_MESES_EXE_ANTERIOR.Value.ToString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Meses Execução Anterior: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Advogado - Honorários", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 6;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.HONORARIO != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Nome: " + tran.HONORARIO.HONO_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CPF: " + tran.HONORARIO.HONO_NR_CPF, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CNPJ: " + tran.HONORARIO.HONO_NR_CNPJ, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("OAB: " + tran.HONORARIO.HONO_NR_OAB, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Nome: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CPF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("CNPJ: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph("OAB: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (tran.PREC_DT_HON_DATABASE != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Data: " + tran.PREC_DT_HON_DATABASE.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Data: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (tran.PREC_VL_HON_VALOR_PRINCIPAL != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Principal (R$): " + CrossCutting.Formatters.DecimalFormatter(tran.PREC_VL_HON_VALOR_PRINCIPAL.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Principal (R$): -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (tran.PREC_VL_HON_JUROS != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Juros (R$): " + CrossCutting.Formatters.DecimalFormatter(tran.PREC_VL_HON_JUROS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Juros (R$): -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new iTextSharp.text.Paragraph("IR RRA: " + tran.PREC_SG_HON_IR_RRA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.PREC_VL_HON_VALOR_INICIAL_PSS != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Inic. PSS   (R$): " + CrossCutting.Formatters.DecimalFormatter(tran.PREC_VL_HON_VALOR_INICIAL_PSS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Valor Inic. PSS (R$): -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (tran.PREC_IN_HON_MESES_EXE_ANTERIOR != null)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Meses Execução Anterior: " + tran.PREC_IN_HON_MESES_EXE_ANTERIOR .Value.ToString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph("Meses Execução Anterior: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
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

            return RedirectToAction("VoltarAnexoPrecatorio");
        }

        [HttpGet]
        public ActionResult IncluirPrecatorioTRFExcel()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPrecatorio", "Precatorio");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return View();
        }

        [HttpPost]
        public System.Threading.Tasks.Task ProcessaOperacaoPlanilha(HttpPostedFileBase file)
        {
            USUARIO user = (USUARIO)Session["UserCredentials"];
            Int32 conta = 0;
            Int32 falha = 0;
            Int32 trf = 0;
            Int32 ajuste = 0;

            // Recupera configuracao
            CONFIGURACAO conf = confApp.GetItemById(1);
            String caminhoAudio = conf.CONF_NM_LOCAL_AUDIO;

            using (var pkg = new ExcelPackage(file.InputStream))
            {
                // Inicialização
                ExcelWorksheet wsGeral = pkg.Workbook.Worksheets[0];
                var wsFinalRow = wsGeral.Dimension.End;
                Int32 idTRF = 0;

                // Listas de pesquisas
                List<TRF> trfs = CarregaTRF();
                List<PRECATORIO> precs = CarregaPrecatorio();
                List<NATUREZA> nats = CarregaNatureza();
                PRECATORIO precCheca = new PRECATORIO();
                NATUREZA natCheca = new NATUREZA();
                BENEFICIARIO beneCheca = new BENEFICIARIO();
                HONORARIO honoCheca = new HONORARIO();
                TRF trfx = new TRF();
                CONTATO contCheca = new CONTATO();
                QUALIFICACAO qualiCheca = new QUALIFICACAO();
                QUEM_DESLIGOU quemCheca = new QUEM_DESLIGOU();
                List<BENEFICIARIO> listaBene = CarregaBeneficiario();
                List<CLIENTE> listaCli = CarregaCliente();
                USUARIO usuCheca = new USUARIO();

                // Checa planilha
                if (wsGeral.Cells[2, 1].Value.ToString() == "TRF3")
                {
                    // Prcessa TRF
                    trf = 3;
                    trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-3").FirstOrDefault();
                    idTRF = trfx.TRF1_CD_ID;
                }
                if (wsGeral.Cells[2, 1].Value.ToString() == "TRF5")
                {
                    // Prcessa TRF
                    trf = 5;
                    trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-5").FirstOrDefault();
                    idTRF = trfx.TRF1_CD_ID;
                }
                if (wsGeral.Cells[2, 1].Value.ToString() == "TRF2")
                {
                    // Prcessa TRF
                    trf = 2;
                    trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-2").FirstOrDefault();
                    idTRF = trfx.TRF1_CD_ID;
                }
                if (wsGeral.Cells[2, 1].Value.ToString() == "TRF4")
                {
                    // Prcessa TRF
                    trf = 4;
                    trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-4").FirstOrDefault();
                    idTRF = trfx.TRF1_CD_ID;
                }
                if (wsGeral.Cells[2, 1].Value.ToString() == "DISC")
                {
                    // Processa Callix
                    trf = 0;
                }

                // Processa TRF3
                if (trf == 3)
                {
                    for (int row = 2; row <= wsFinalRow.Row; row++)
                    {
                        try
                        {
                            Int32 check = 0;
                            PRECATORIO prec = new PRECATORIO();
                            String colTRF = null;

                            // Verifica saida
                            if (wsGeral.Cells[row, 1].Value == null)
                            {
                                break;
                            }
                            else
                            {
                                colTRF = wsGeral.Cells[row, 1].Value.ToString();
                            }

                            // Verifica existencia
                            String numPrec = null;
                            if (wsGeral.Cells[row, 2].Value != null)
                            {
                                numPrec = wsGeral.Cells[row, 2].Value.ToString();
                                numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
                                precCheca = tranApp.CheckExist(numPrec);
                                if (precCheca != null)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 1;
                                    fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Precatório já incluído para o " + trfx.TRF1_NM_NOME;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;
                                }
                            }
                            else
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = "Não informado";
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 1;
                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Número não informado";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }

                            // Monta objeto
                            prec.TRF1_CD_ID = idTRF;
                            prec.PREC_NM_PRECATORIO = numPrec;
                            if (wsGeral.Cells[row, 3].Value != null)
                            {
                                prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 3].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_PROC_EXECUCAO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 4].Value != null)
                            {
                                prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 4].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_PROCESSO_ORIGEM = String.Empty;
                            }

                            if (wsGeral.Cells[row, 6].Value != null)
                            {
                                prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 6].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_REQUERENTE = String.Empty;
                            }

                            if (wsGeral.Cells[row, 7].Value != null)
                            {
                                prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 7].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_ADVOGADO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 8].Value != null)
                            {
                                prec.PREC_NM_DEPRECANTE = wsGeral.Cells[row, 8].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_DEPRECANTE = String.Empty;
                            }

                            if (wsGeral.Cells[row, 9].Value != null)
                            {
                                prec.PREC_NM_ASSUNTO = wsGeral.Cells[row, 9].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_ASSUNTO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 10].Value != null)
                            {
                                prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 10].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_REQUERIDO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 13].Value != null)
                            {
                                String valPrinc = wsGeral.Cells[row, 13].Value.ToString();
                                if (valPrinc != null)
                                {
                                    if (Regex.IsMatch(valPrinc, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 13].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO VALOR PRINCIPAL" + valPrinc;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
                            }

                            if ( wsGeral.Cells[row, 14].Value != null)
                            {
                                String juroBen = wsGeral.Cells[row, 14].Value.ToString();
                                if (juroBen != null)
                                {
                                    if (Regex.IsMatch(juroBen, @"\d"))
                                    {
                                        prec.PREC_VL_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 14].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO JUROS " + juroBen;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_JUROS = 0;
                                }

                            }
                            else
                            {
                                prec.PREC_VL_JUROS = 0;
                            }

                            if (wsGeral.Cells[row, 15].Value != null)
                            {
                                if (wsGeral.Cells[row, 15].Value.ToString() == "Sim" || wsGeral.Cells[row, 15].Value.ToString() == "S")
                                {
                                    prec.PREC_SG_BEN_IR_RRA = "1";
                                }
                                else
                                {
                                    prec.PREC_SG_BEN_IR_RRA = "0";
                                }
                            }
                            else
                            {
                                prec.PREC_SG_BEN_IR_RRA = "0";
                            }

                            if (wsGeral.Cells[row, 16].Value != null)
                            {
                                String mesAnt = wsGeral.Cells[row, 16].Value.ToString();
                                if (mesAnt != null & mesAnt != "0" & mesAnt != "-")
                                {
                                    if (Regex.IsMatch(mesAnt, @"\d"))
                                    {
                                        prec.PREC_BEN_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 16].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO MESES EXE ANTERIOR " + mesAnt;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
                                }

                            }
                            else
                            {
                                prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
                            }

                            if (wsGeral.Cells[row, 17].Value != null)
                            {
                                if (wsGeral.Cells[row, 17].Value.ToString() != null)
                                {
                                    String datx = wsGeral.Cells[row, 17].Value.ToString();
                                    DateTime outData = new DateTime();
                                    if (DateTime.TryParse(datx, out outData))
                                    {
                                        prec.PREC_DT_BEN_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 17].Value.ToString());
                                    }
                                    else
                                    {
                                        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 17].Value));
                                        if (dat == DateTime.MinValue)
                                        {
                                            prec.PREC_DT_BEN_DATABASE = null;
                                        }
                                        else
                                        {
                                            prec.PREC_DT_BEN_DATABASE = dat;
                                        }
                                    }
                                }
                                else
                                {
                                    prec.PREC_DT_BEN_DATABASE = null;
                                }
                            }
                            else
                            {
                                prec.PREC_DT_BEN_DATABASE = null;
                            }

                            if (wsGeral.Cells[row, 18].Value != null)
                            {
                                String iniPSS = wsGeral.Cells[row, 18].Value.ToString();
                                if (iniPSS != null)
                                {
                                    if (Regex.IsMatch(iniPSS, @"\d"))
                                    {
                                        prec.PREC_VL_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 18].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR INICIAL PSS" + iniPSS;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_VALOR_INICIAL_PSS = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_VALOR_INICIAL_PSS = 0;
                            }

                            if (wsGeral.Cells[row, 19].Value != null)
                            {
                                String valReq = wsGeral.Cells[row, 19].Value.ToString();
                                if (valReq != null)
                                {
                                    if (Regex.IsMatch(valReq, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 19].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR REQUISITADO" + valReq;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                            }

                            Int32 honoFlag = 1;
                            String honoNome = null;
                            String honoCPF = null;

                            if ( wsGeral.Cells[row, 20].Value != null)
                            {
                                honoNome = wsGeral.Cells[row, 20].Value.ToString();
                            }
                            else
                            {
                                honoNome = null;
                            }
                            if (wsGeral.Cells[row, 21].Value != null)
                            {
                                honoCPF = wsGeral.Cells[row, 21].Value.ToString();
                                String novoCPF = String.Empty;
                                if (honoCPF.Length == 11)
                                {
                                    if (!honoCPF.Contains("."))
                                    {
                                        novoCPF = honoCPF.Substring(0, 3) + "." + honoCPF.Substring(3, 3) + "." + honoCPF.Substring(6, 3) + "-" + honoCPF.Substring(9, 2);
                                        honoCPF = novoCPF;
                                    }
                                    else
                                    {
                                        honoCPF = null;
                                        honoFlag = 0;
                                    }
                                }
                                if (honoCPF.Length == 14)
                                {
                                    if (!honoCPF.Contains("."))
                                    {
                                        novoCPF = honoCPF.Substring(0, 2) + "." + honoCPF.Substring(2, 3) + "." + honoCPF.Substring(5, 3) + "/" + honoCPF.Substring(8, 4) + "-" + honoCPF.Substring(12, 2);
                                        honoCPF = novoCPF;
                                    }
                                }

                            }
                            else
                            {
                                honoCPF = null;
                                honoFlag = 0;
                            }

                            if ((honoCPF != null & honoNome == null) || (honoCPF == null & honoNome != null))
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Informações incompletas do ADVOGADO. Precatório: " + numPrec + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                honoFlag = 0;
                            }
                            if (honoCPF != null)
                            {
                                if (honoCPF.Length == 14)
                                {
                                    if (!CrossCutting.ValidarCPF.IsCFPValid(honoCPF))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "CPF do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        honoFlag = 0;
                                    }
                                }
                                else if (honoCPF.Length == 18)
                                {
                                    if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(honoCPF))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "CNPJ do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        honoFlag = 0;
                                    }
                                }
                                else
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    fal.PRFA_DS_MOTIVO = "CPF ou CNPJ do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                    honoFlag = 0;
                                }
                            }

                            if (honoFlag == 1)
                            {
                                honoCheca = honoApp.CheckExist(honoNome, honoCPF);
                                if (honoCheca != null)
                                {
                                    prec.HONO_CD_ID = honoCheca.HONO_CD_ID;
                                    prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
                                    prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());
                                }
                                else
                                {
                                    HONORARIO hono = new HONORARIO();
                                    hono.HONO_DT_CADASTRO = DateTime.Today.Date;
                                    hono.HONO_IN_ATIVO = 1;
                                    hono.HONO_NM_NOME = honoNome;
                                    String cpfCNPJ = honoCPF;
                                    if (cpfCNPJ.Length == 14)
                                    {
                                        hono.HONO_NR_CPF = cpfCNPJ;
                                        hono.TIPE_CD_ID = 1;
                                    }
                                    else
                                    {
                                        hono.HONO_NR_CNPJ = cpfCNPJ;
                                        hono.TIPE_CD_ID = 2;
                                    }
                                    Int32 volta3 = honoApp.ValidateCreate(hono, user);
                                    if (volta3 > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do ADVOGADO " + honoNome + ". Prossegue a importação sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        prec.HONO_CD_ID = hono.HONO_CD_ID;
                                        prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
                                        prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());

                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Advogados/" + hono.HONO_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }

                            if (wsGeral.Cells[row, 24].Value != null)
                            {
                                if (wsGeral.Cells[row, 24].Value.ToString() == "Sim" || wsGeral.Cells[row, 24].Value.ToString() == "S")
                                {
                                    prec.PREC_SG_HON_IR_RRA = "1";
                                }
                                else
                                {
                                    prec.PREC_SG_HON_IR_RRA = "0";
                                }
                            }
                            else
                            {
                                prec.PREC_SG_HON_IR_RRA = "0";
                            }

                            if (wsGeral.Cells[row, 25].Value != null)
                            {
                                String honoMes = wsGeral.Cells[row, 25].Value.ToString();
                                if (honoMes != null)
                                {
                                    if (Regex.IsMatch(honoMes, @"\d"))
                                    {
                                        prec.PREC_IN_HON_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 25].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para HONORARIO MES ANTERIOR" + honoMes;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
                            }

                            if (wsGeral.Cells[row, 26].Value != null)
                            {
                                if (wsGeral.Cells[row, 26].Value.ToString() != null)
                                {
                                    String datx = wsGeral.Cells[row, 26].Value.ToString();
                                    DateTime outData = new DateTime();
                                    if (DateTime.TryParse(datx, out outData))
                                    {
                                        prec.PREC_DT_HON_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 26].Value.ToString());
                                    }
                                    else
                                    {
                                        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 26].Value));
                                        if (dat == DateTime.MinValue)
                                        {
                                            prec.PREC_DT_HON_DATABASE = null;
                                        }
                                        else
                                        {
                                            prec.PREC_DT_HON_DATABASE = dat;
                                        }
                                    }
                                }
                                else
                                {
                                    prec.PREC_DT_HON_DATABASE = null;
                                }
                            }
                            else
                            {
                                prec.PREC_DT_HON_DATABASE = null;
                            }

                            if (wsGeral.Cells[row, 27].Value != null)
                            {
                                String valPSS = wsGeral.Cells[row, 27].Value.ToString();
                                if (valPSS != null)
                                {
                                    if (Regex.IsMatch(valPSS, @"\d"))
                                    {
                                        prec.PREC_VL_HON_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 27].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO INICIAL PSS" + valPSS;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
                            }

                            if (wsGeral.Cells[row, 28].Value != null)
                            {
                                String honReq = wsGeral.Cells[row, 28].Value.ToString();
                                if (honReq != null)
                                {
                                    if (Regex.IsMatch(honReq, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 28].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO REQUISITADO" + honReq;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                            }

                            if (wsGeral.Cells[row, 36].Value != null)
                            {
                                String anoProp = wsGeral.Cells[row, 36].Value.ToString();
                                if (anoProp != null)
                                {
                                    if (Regex.IsMatch(anoProp, @"\d{4}"))
                                    {
                                        prec.PREC_NR_ANO = anoProp;
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor inválido informado para ANO DA PROPOSTA " + anoProp;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        prec.PREC_NR_ANO = String.Empty;
                                    }
                                }
                                else
                                {
                                    prec.PREC_NR_ANO = String.Empty;
                                }
                            }
                            else
                            {
                                prec.PREC_NR_ANO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 37].Value != null)
                            {
                                prec.PREC_NM_TIPO_DESPESA = wsGeral.Cells[row, 37].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_TIPO_DESPESA = String.Empty;
                            }

                            if (wsGeral.Cells[row, 38].Value != null)
                            {
                                String valRRA = wsGeral.Cells[row, 38].Value.ToString();
                                if (valRRA != null)
                                {
                                    if (Regex.IsMatch(valRRA, @"\d"))
                                    {
                                        prec.PREC_VL_RRA = Convert.ToDecimal(wsGeral.Cells[row, 38].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR RRA. Assumido 0. " + valRRA;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        prec.PREC_VL_RRA = 0;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_RRA = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_RRA = 0;
                            }

                            if (wsGeral.Cells[row, 39].Value != null)
                            {
                                String percRRA = wsGeral.Cells[row, 39].Value.ToString();
                                if (percRRA != null)
                                {
                                    if (Regex.IsMatch(percRRA, @"\d"))
                                    {
                                        prec.PREC_PC_RRA = Convert.ToDecimal(wsGeral.Cells[row, 39].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para PERCENTAGEM RRA. Assumido 0. " + percRRA;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        prec.PREC_PC_RRA = 0;
                                    }
                                }
                                else
                                {
                                    prec.PREC_PC_RRA = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_PC_RRA = 0;
                            }

                            //if (wsGeral.Cells[row, 40].Value != null)
                            //{
                            //    prec.PREC_NM_PREFERENCIA = wsGeral.Cells[row, 40].Value.ToString();
                            //}
                            //else
                            //{
                            //    prec.PREC_NM_PREFERENCIA = String.Empty;
                            //}
                            
                            Int32 flagBene = 1;
                            String nome = null;
                            String cpf = null;
                            String sexo = null;
                            String nasc = null;
                            String cel1 = null;
                            String cel2 = null;

                            if ( wsGeral.Cells[row, 29].Value != null)
                            {
                                nome = wsGeral.Cells[row, 29].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 30].Value != null)
                            {
                                cpf = wsGeral.Cells[row, 30].Value.ToString();
                                String novoCPF = String.Empty;
                                if (cpf.Length != 14)
                                {
                                    if (cpf.Length == 11)
                                    {
                                        if (cpf.Length == 11)
                                        {
                                            if (!cpf.Contains("."))
                                            {
                                                novoCPF = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);
                                                cpf = novoCPF;
                                            }
                                            else
                                            {
                                                cpf = null;
                                                flagBene = 0;
                                            }
                                        }
                                    }
                                }
                                if (cpf != null)
                                {
                                    if (!CrossCutting.ValidarCPF.IsCFPValid(cpf))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Beneficiário: CPF inválido. Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        cpf = null;
                                        flagBene = 0;
                                    }
                                }

                            }
                            if (wsGeral.Cells[row, 31].Value != null)
                            {
                                sexo = wsGeral.Cells[row, 31].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 32].Value != null)
                            {
                                nasc = wsGeral.Cells[row, 32].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 33].Value != null)
                            {
                                cel1 = wsGeral.Cells[row, 33].Value.ToString();
                                String novoCel = String.Empty;
                                if (cel1.Length != 14)
                                {
                                    if (cel1.Length == 11)
                                    {
                                        novoCel = "(" + cel1.Substring(0, 2) + ")" + cel1.Substring(2, 5) + "-" + cel1.Substring(7, 4);
                                        cel1 = novoCel;
                                    }
                                    else
                                    {
                                        cel1 = "-";
                                    }
                                }
                            }
                            if (wsGeral.Cells[row, 34].Value != null)
                            {
                                cel2 = wsGeral.Cells[row, 34].Value.ToString();
                                String novoCel = String.Empty;
                                if (cel2.Length != 14)
                                {
                                    if (cel1.Length == 11)
                                    {
                                        novoCel = "(" + cel2.Substring(0, 2) + ")" + cel2.Substring(2, 5) + "-" + cel2.Substring(7, 4);
                                        cel2 = novoCel;
                                    }
                                    else
                                    {
                                        cel2 = "-";
                                    }
                                }
                            }

                            if (nome == null || cpf == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Beneficiário: Nome e CPF não informados. Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                flagBene = 0;
                            }

                            DateTime checaNasc;
                            DateTime? dataNasc = null;
                            if (nasc != null)
                            {
                                Boolean data = DateTime.TryParse(nasc, out checaNasc);
                                if (!data)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_DS_MOTIVO = "Data de nascimento inválida. Beneficiário: " + nome + ". " + nasc + ". Prossegue a importação sem essa informação";
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                }
                                String x = nasc.ToString();
                                dataNasc = Convert.ToDateTime(x);
                            }

                            if (flagBene == 1)
                            {
                                BENEFICIARIO beneNovo = new BENEFICIARIO();
                                BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
                                if (bene == null)
                                {
                                    beneNovo.TIPE_CD_ID = 1;
                                    if (sexo == "M" || sexo == "F")
                                    {
                                        beneNovo.SEXO_CD_ID = sexo == "M" ? 1 : 2;
                                    }
                                    beneNovo.BENE_NM_NOME = nome;
                                    beneNovo.BENE_NR_CPF = cpf;
                                    beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
                                    beneNovo.BENE_NR_CELULAR = cel1;
                                    beneNovo.BENE_NR_CELULAR2 = cel2;
                                    beneNovo.BENE_DT_NASCIMENTO = dataNasc;
                                    beneNovo.BENE_IN_ATIVO = 1;
                                    Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
                                    if (voltaBene > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome + ". Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        prec.BENE_CD_ID = beneNovo.BENE_CD_ID;

                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Beneficiarios/" + beneNovo.BENE_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }
                            else
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome + ". BENEFICIÁRIO não criado por inconsistencia de dados";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                            }

                            if (flagBene == 1)
                            {
                                CLIENTE cliNovo = new CLIENTE();
                                CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
                                if (cli == null)
                                {
                                    cliNovo.TIPE_CD_ID = 1;
                                    if (sexo == "M" || sexo == "F")
                                    {
                                        cliNovo.SEXO_CD_ID = sexo == "M" ? 1 : 2;
                                    }
                                    cliNovo.CLIE_NM_NOME = nome;
                                    cliNovo.CLIE_NR_CPF = cpf;
                                    cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
                                    cliNovo.CLIE_NR_CELULAR = cel1;
                                    cliNovo.CLIE_NR_WHATSAPP = cel2;
                                    cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
                                    cliNovo.CLIE_IN_ATIVO = 1;
                                    cliNovo.CLIE_NM_EMAIL = String.Empty;
                                    Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
                                    if (voltaCli > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome + ". Prossegue a importação sem inclusão de CLIENTE";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Clientes/" + cliNovo.CLIE_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                        caminho = "/Imagens/1" + "/Clientes/" + cliNovo.CLIE_CD_ID.ToString() + "/Fotos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }

                            // Processa usuário
                            Int32 flagUsu = 1;
                            String usu = null;
                            if (wsGeral.Cells[row, 40].Value != null)
                            {
                                usu = wsGeral.Cells[row, 40].Value.ToString();
                            }

                            if (usu == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Usuário responsável não definido. Precatório " + numPrec + ". Prossegue a importação sem associação de RESPONSÁVEL";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                flagUsu = 0;
                            }

                            if (flagUsu == 1)
                            {
                                usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
                                if (usuCheca == null)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado. Precatório " + numPrec + ". Prossegue a importação sem associação de RESPONSÁVEL";
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                    prec.USUA_CD_ID = null;
                                }
                                else
                                {
                                    prec.USUA_CD_ID = Convert.ToInt32(usu);
                                }
                            }

                            // Grava objeto
                            if (colTRF != null)
                            {
                                prec.PREC_NM_NOME = nome;
                                Int32 volta = tranApp.ValidateCreate(prec, user);
                                if (volta > 0)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 1;
                                    fal.PRFA_DS_MOTIVO = "Erro na gravação do PRECATÓRIO. Precatório " + numPrec;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;
                                }
                                else
                                {
                                    conta++;

                                    // Cria pastas
                                    String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
                                    Directory.CreateDirectory(Server.MapPath(caminho));
                                }
                            }
                            else
                            {
                                Session["Conta"] = conta;
                                Session["Falha"] = falha;
                                Session["MensPrecatorio"] = 50;
                                Session["ListaPrecatorio"] = null;
                                Session["VoltaPrecatorio"] = 0;
                                return System.Threading.Tasks.Task.Delay(5);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return System.Threading.Tasks.Task.Delay(5);
                        }
                    }
                }

                // Processa TRF-5
                if (trf == 5)
                {
                    for (int row = 2; row <= wsFinalRow.Row; row++)
                    {
                        try
                        {
                            Int32 check = 0;
                            PRECATORIO prec = new PRECATORIO();

                            // Verifica existencia
                            String numPrec = wsGeral.Cells[row, 2].Value.ToString();
                            numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
                            precCheca = tranApp.CheckExist(numPrec);
                            if (precCheca != null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_DS_MOTIVO = "Precatório já incluído para o " + trfx.TRF1_NM_NOME;
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }

                            // Monta objeto
                            prec.TRF1_CD_ID = idTRF;
                            prec.PREC_NM_PRECATORIO = numPrec;
                            prec.PREC_NR_ANO = wsGeral.Cells[row, 3].Value.ToString();
                            prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 11].Value.ToString();
                            prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 12].Value.ToString();
                            prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 13].Value.ToString();
                            prec.PREC_NM_DEPRECANTE = wsGeral.Cells[row, 14].Value.ToString();
                            prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 15].Value.ToString();
                            prec.PREC_NM_OFICIO_REQUISITORIO = wsGeral.Cells[row, 16].Value.ToString();
                            prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 17].Value.ToString();
                            prec.PREC_NM_ASSUNTO = wsGeral.Cells[row, 21].Value.ToString();
                            prec.PREC_NM_SITUACAO_REQUISICAO = wsGeral.Cells[row, 22].Value.ToString();
                            String nat = wsGeral.Cells[row, 20].Value.ToString();
                            natCheca = tranApp.CheckExistNatureza(nat);
                            if (natCheca != null)
                            {
                                prec.NATU_CD_ID = natCheca.NATU_CD_ID;
                            }
                            else
                            {
                                prec.NATU_CD_ID = null;
                            }

                            String nome = wsGeral.Cells[row, 25].Value.ToString();
                            String cpf = wsGeral.Cells[row, 26].Value.ToString();
                            String sexo = wsGeral.Cells[row, 27].Value.ToString();
                            String nasc = wsGeral.Cells[row, 28].Value.ToString();
                            String cel1 = wsGeral.Cells[row, 29].Value.ToString();
                            String cel2 = wsGeral.Cells[row, 30].Value.ToString();
                            String tel = wsGeral.Cells[row, 31].Value.ToString();
                            DateTime? dataNasc = null;
                            if (nasc != null)
                            {
                                String x = nasc.ToString();
                                dataNasc = Convert.ToDateTime(x);
                            }

                            BENEFICIARIO beneNovo = new BENEFICIARIO();
                            if (nome != null && cpf != null)
                            {
                                BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
                                if (bene == null)
                                {
                                    beneNovo.TIPE_CD_ID = 1;
                                    if (sexo == "1" || sexo == "2")
                                    {
                                        beneNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
                                    }
                                    beneNovo.BENE_NM_NOME = nome;
                                    beneNovo.BENE_NR_CPF = cpf;
                                    beneNovo.BENE_NR_TELEFONE2 = tel;
                                    beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
                                    beneNovo.BENE_NR_CELULAR = cel1;
                                    beneNovo.BENE_NR_CELULAR2 = cel2;
                                    beneNovo.BENE_DT_NASCIMENTO = dataNasc;
                                    beneNovo.BENE_IN_ATIVO = 1;
                                    Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
                                    if (voltaBene > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                    }
                                    prec.BENE_CD_ID = beneNovo.BENE_CD_ID;
                                }
                            }

                            CLIENTE cliNovo = new CLIENTE();
                            if (nome != null && cpf != null)
                            {
                                CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
                                if (cli == null)
                                {
                                    cliNovo.TIPE_CD_ID = 1;
                                    if (sexo == "1" || sexo == "2")
                                    {
                                        cliNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
                                    }
                                    cliNovo.CLIE_NM_NOME = nome;
                                    cliNovo.CLIE_NR_CPF = cpf;
                                    cliNovo.CLIE_NR_TELEFONE = tel;
                                    cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
                                    cliNovo.CLIE_NR_CELULAR = cel1;
                                    cliNovo.CLIE_NR_WHATSAPP = cel2;
                                    cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
                                    cliNovo.CLIE_IN_ATIVO = 1;
                                    Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
                                    if (voltaCli > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                    }
                                }
                            }

                            // Processa usuário
                            String usu = wsGeral.Cells[row, 32].Value.ToString();
                            if (usu == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_DS_MOTIVO = "Usuário responsável não definido " + trfx.TRF1_NM_NOME;
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }

                            usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
                            if (usuCheca == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado " + trfx.TRF1_NM_NOME;
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }
                            prec.USUA_CD_ID = Convert.ToInt32(usu);

                            // Grava objeto
                            prec.PREC_NM_NOME = nome;
                            Int32 volta = tranApp.ValidateCreate(prec, user);
                            conta++;

                            // Cria pastas
                            String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
                            Directory.CreateDirectory(Server.MapPath(caminho));
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return System.Threading.Tasks.Task.Delay(5);
                        }
                    }
                }

                // Processa TRF-2
                if (trf == 2)
                {
                    for (int row = 2; row <= wsFinalRow.Row; row++)
                    {
                        try
                        {
                            Int32 check = 0;
                            PRECATORIO prec = new PRECATORIO();
                            String colTRF = null;
                            if (wsGeral.Cells[row, 1].Value == null)
                            {
                                break;
                            }
                            else
                            {
                                colTRF = wsGeral.Cells[row, 1].Value.ToString();
                            }

                            // Verifica existencia
                            String numPrec = null;
                            if (wsGeral.Cells[row, 2].Value != null)
                            {
                                numPrec = wsGeral.Cells[row, 2].Value.ToString();
                                numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
                                precCheca = tranApp.CheckExist(numPrec);
                                if (precCheca != null)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 1;
                                    fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Precatório já incluído para o " + trfx.TRF1_NM_NOME;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;
                                }
                            }
                            else
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = "Não informado";
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 1;
                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Número não informado";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }

                            // Monta objeto
                            prec.TRF1_CD_ID = idTRF;
                            prec.PREC_NM_PRECATORIO = numPrec;
                            if (wsGeral.Cells[row, 3].Value != null)
                            {
                                prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 3].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_PROC_EXECUCAO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 4].Value != null)
                            {
                                prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 4].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_PROCESSO_ORIGEM = String.Empty;
                            }

                            if (wsGeral.Cells[row, 6].Value != null)
                            {
                                prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 6].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_REQUERENTE = String.Empty;
                            }

                            if (wsGeral.Cells[row, 7].Value != null)
                            {
                                prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 7].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_ADVOGADO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 8].Value != null)
                            {
                                prec.PREC_NM_DEPRECANTE = wsGeral.Cells[row, 8].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_DEPRECANTE = String.Empty;
                            }

                            if (wsGeral.Cells[row, 9].Value != null)
                            {
                                prec.PREC_NM_ASSUNTO = wsGeral.Cells[row, 9].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_ASSUNTO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 10].Value != null)
                            {
                                prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 10].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_REQUERIDO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 13].Value != null)
                            {
                                String valPrinc = wsGeral.Cells[row, 13].Value.ToString();
                                if (valPrinc != null)
                                {
                                    if (Regex.IsMatch(valPrinc, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 13].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO VALOR PRINCIPAL" + valPrinc;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
                            }

                            if ( wsGeral.Cells[row, 14].Value != null)
                            {
                                String juroBen = wsGeral.Cells[row, 14].Value.ToString();
                                if (juroBen != null)
                                {
                                    if (Regex.IsMatch(juroBen, @"\d"))
                                    {
                                        prec.PREC_VL_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 14].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO JUROS " + juroBen;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_JUROS = 0;
                                }

                            }
                            else
                            {
                                prec.PREC_VL_JUROS = 0;
                            }

                            if (wsGeral.Cells[row, 15].Value != null)
                            {
                                if (wsGeral.Cells[row, 15].Value.ToString() == "Sim" || wsGeral.Cells[row, 15].Value.ToString() == "S")
                                {
                                    prec.PREC_SG_BEN_IR_RRA = "1";
                                }
                                else
                                {
                                    prec.PREC_SG_BEN_IR_RRA = "0";
                                }
                            }
                            else
                            {
                                prec.PREC_SG_BEN_IR_RRA = "0";
                            }

                            if (wsGeral.Cells[row, 16].Value != null)
                            {
                                String mesAnt = wsGeral.Cells[row, 16].Value.ToString();
                                if (mesAnt != null & mesAnt != "0" & mesAnt != "-")
                                {
                                    if (Regex.IsMatch(mesAnt, @"\d"))
                                    {
                                        prec.PREC_BEN_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 16].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO MESES EXE ANTERIOR " + mesAnt;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
                                }

                            }
                            else
                            {
                                prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
                            }

                            if (wsGeral.Cells[row, 17].Value != null)
                            {
                                if (wsGeral.Cells[row, 17].Value.ToString() != null)
                                {
                                    String datx = wsGeral.Cells[row, 17].Value.ToString();
                                    DateTime outData = new DateTime();
                                    if (DateTime.TryParse(datx, out outData))
                                    {
                                        prec.PREC_DT_BEN_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 17].Value.ToString());
                                    }
                                    else
                                    {
                                        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 17].Value));
                                        if (dat == DateTime.MinValue)
                                        {
                                            prec.PREC_DT_BEN_DATABASE = null;
                                        }
                                        else
                                        {
                                            prec.PREC_DT_BEN_DATABASE = dat;
                                        }
                                    }
                                }
                                else
                                {
                                    prec.PREC_DT_BEN_DATABASE = null;
                                }
                            }
                            else
                            {
                                prec.PREC_DT_BEN_DATABASE = null;
                            }

                            if (wsGeral.Cells[row, 18].Value != null)
                            {
                                String iniPSS = wsGeral.Cells[row, 18].Value.ToString();
                                if (iniPSS != null)
                                {
                                    if (Regex.IsMatch(iniPSS, @"\d"))
                                    {
                                        prec.PREC_VL_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 18].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR INICIAL PSS" + iniPSS;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_VALOR_INICIAL_PSS = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_VALOR_INICIAL_PSS = 0;
                            }

                            if (wsGeral.Cells[row, 19].Value != null)
                            {
                                String valReq = wsGeral.Cells[row, 19].Value.ToString();
                                if (valReq != null)
                                {
                                    if (Regex.IsMatch(valReq, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 19].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR REQUISITADO" + valReq;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                            }

                            Int32 honoFlag = 1;
                            String honoNome = null;
                            String honoCPF = null;

                            if ( wsGeral.Cells[row, 20].Value != null)
                            {
                                honoNome = wsGeral.Cells[row, 20].Value.ToString();
                            }
                            else
                            {
                                honoNome = null;
                            }
                            if (wsGeral.Cells[row, 21].Value != null)
                            {
                                honoCPF = wsGeral.Cells[row, 21].Value.ToString();
                                String novoCPF = String.Empty;
                                if (honoCPF.Length == 11)
                                {
                                    if (!honoCPF.Contains("."))
                                    {
                                        novoCPF = honoCPF.Substring(0, 3) + "." + honoCPF.Substring(3, 3) + "." + honoCPF.Substring(6, 3) + "-" + honoCPF.Substring(9, 2);
                                        honoCPF = novoCPF;
                                    }
                                    else
                                    {
                                        honoCPF = null;
                                        honoFlag = 0;
                                    }
                                }
                                if (honoCPF.Length == 14)
                                {
                                    if (!honoCPF.Contains("."))
                                    {
                                        novoCPF = honoCPF.Substring(0, 2) + "." + honoCPF.Substring(2, 3) + "." + honoCPF.Substring(5, 3) + "/" + honoCPF.Substring(8, 4) + "-" + honoCPF.Substring(12, 2);
                                        honoCPF = novoCPF;
                                    }
                                }

                            }
                            else
                            {
                                honoCPF = null;
                                honoFlag = 0;
                            }

                            if ((honoCPF != null & honoNome == null) || (honoCPF == null & honoNome != null))
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Informações incompletas do ADVOGADO. Precatório: " + numPrec + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                honoFlag = 0;
                            }
                            if (honoCPF != null)
                            {
                                if (honoCPF.Length == 14)
                                {
                                    if (!CrossCutting.ValidarCPF.IsCFPValid(honoCPF))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "CPF do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        honoFlag = 0;
                                    }
                                }
                                else if (honoCPF.Length == 18)
                                {
                                    if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(honoCPF))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "CNPJ do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        honoFlag = 0;
                                    }
                                }
                                else
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    fal.PRFA_DS_MOTIVO = "CPF ou CNPJ do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                    honoFlag = 0;
                                }
                            }

                            if (honoFlag == 1)
                            {
                                honoCheca = honoApp.CheckExist(honoNome, honoCPF);
                                if (honoCheca != null)
                                {
                                    prec.HONO_CD_ID = honoCheca.HONO_CD_ID;
                                    prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
                                    prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());
                                }
                                else
                                {
                                    HONORARIO hono = new HONORARIO();
                                    hono.HONO_DT_CADASTRO = DateTime.Today.Date;
                                    hono.HONO_IN_ATIVO = 1;
                                    hono.HONO_NM_NOME = honoNome;
                                    String cpfCNPJ = honoCPF;
                                    if (cpfCNPJ.Length == 14)
                                    {
                                        hono.HONO_NR_CPF = cpfCNPJ;
                                        hono.TIPE_CD_ID = 1;
                                    }
                                    else
                                    {
                                        hono.HONO_NR_CNPJ = cpfCNPJ;
                                        hono.TIPE_CD_ID = 2;
                                    }
                                    Int32 volta3 = honoApp.ValidateCreate(hono, user);
                                    if (volta3 > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do ADVOGADO " + honoNome + ". Prossegue a importação sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        prec.HONO_CD_ID = hono.HONO_CD_ID;
                                        prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
                                        prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());

                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Advogados/" + hono.HONO_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }

                            if (wsGeral.Cells[row, 24].Value != null)
                            {
                                if (wsGeral.Cells[row, 24].Value.ToString() == "Sim" || wsGeral.Cells[row, 24].Value.ToString() == "S")
                                {
                                    prec.PREC_SG_HON_IR_RRA = "1";
                                }
                                else
                                {
                                    prec.PREC_SG_HON_IR_RRA = "0";
                                }
                            }
                            else
                            {
                                prec.PREC_SG_HON_IR_RRA = "0";
                            }

                            if (wsGeral.Cells[row, 25].Value != null)
                            {
                                String honoMes = wsGeral.Cells[row, 25].Value.ToString();
                                if (honoMes != null)
                                {
                                    if (Regex.IsMatch(honoMes, @"\d"))
                                    {
                                        prec.PREC_IN_HON_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 25].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para HONORARIO MES ANTERIOR" + honoMes;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
                            }

                            if (wsGeral.Cells[row, 26].Value != null)
                            {
                                if (wsGeral.Cells[row, 26].Value.ToString() != null)
                                {
                                    String datx = wsGeral.Cells[row, 26].Value.ToString();
                                    DateTime outData = new DateTime();
                                    if (DateTime.TryParse(datx, out outData))
                                    {
                                        prec.PREC_DT_HON_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 26].Value.ToString());
                                    }
                                    else
                                    {
                                        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 26].Value));
                                        if (dat == DateTime.MinValue)
                                        {
                                            prec.PREC_DT_HON_DATABASE = null;
                                        }
                                        else
                                        {
                                            prec.PREC_DT_HON_DATABASE = dat;
                                        }
                                    }
                                }
                                else
                                {
                                    prec.PREC_DT_HON_DATABASE = null;
                                }
                            }
                            else
                            {
                                prec.PREC_DT_HON_DATABASE = null;
                            }

                            if (wsGeral.Cells[row, 27].Value != null)
                            {
                                String valPSS = wsGeral.Cells[row, 27].Value.ToString();
                                if (valPSS != null)
                                {
                                    if (Regex.IsMatch(valPSS, @"\d"))
                                    {
                                        prec.PREC_VL_HON_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 27].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO INICIAL PSS" + valPSS;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
                            }

                            if (wsGeral.Cells[row, 28].Value != null)
                            {
                                String honReq = wsGeral.Cells[row, 28].Value.ToString();
                                if (honReq != null)
                                {
                                    if (Regex.IsMatch(honReq, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 28].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO REQUISITADO" + honReq;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                            }

                            if (wsGeral.Cells[row, 36].Value != null)
                            {
                                String anoProp = wsGeral.Cells[row, 36].Value.ToString();
                                if (anoProp != null)
                                {
                                    if (Regex.IsMatch(anoProp, @"\d{4}"))
                                    {
                                        prec.PREC_NR_ANO = anoProp;
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor inválido informado para ANO DA PROPOSTA " + anoProp;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        prec.PREC_NR_ANO = String.Empty;
                                    }
                                }
                                else
                                {
                                    prec.PREC_NR_ANO = String.Empty;
                                }
                            }
                            else
                            {
                                prec.PREC_NR_ANO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 37].Value != null)
                            {
                                prec.PREC_NM_TIPO_DESPESA = wsGeral.Cells[row, 37].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_TIPO_DESPESA = String.Empty;
                            }

                            if (wsGeral.Cells[row, 38].Value != null)
                            {
                                String valRRA = wsGeral.Cells[row, 38].Value.ToString();
                                if (valRRA != null)
                                {
                                    if (Regex.IsMatch(valRRA, @"\d"))
                                    {
                                        prec.PREC_VL_RRA = Convert.ToDecimal(wsGeral.Cells[row, 38].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR RRA. Assumido 0. " + valRRA;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        prec.PREC_VL_RRA = 0;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_RRA = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_RRA = 0;
                            }

                            if (wsGeral.Cells[row, 39].Value != null)
                            {
                                String percRRA = wsGeral.Cells[row, 39].Value.ToString();
                                if (percRRA != null)
                                {
                                    if (Regex.IsMatch(percRRA, @"\d"))
                                    {
                                        prec.PREC_PC_RRA = Convert.ToDecimal(wsGeral.Cells[row, 39].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para PERCENTAGEM RRA. Assumido 0. " + percRRA;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        prec.PREC_PC_RRA = 0;
                                    }
                                }
                                else
                                {
                                    prec.PREC_PC_RRA = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_PC_RRA = 0;
                            }

                            //if (wsGeral.Cells[row, 40].Value != null)
                            //{
                            //    prec.PREC_NM_PREFERENCIA = wsGeral.Cells[row, 40].Value.ToString();
                            //}
                            //else
                            //{
                            //    prec.PREC_NM_PREFERENCIA = String.Empty;
                            //}
                            
                            Int32 flagBene = 1;
                            String nome = null;
                            String cpf = null;
                            String sexo = null;
                            String nasc = null;
                            String cel1 = null;
                            String cel2 = null;

                            if ( wsGeral.Cells[row, 29].Value != null)
                            {
                                nome = wsGeral.Cells[row, 29].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 30].Value != null)
                            {
                                cpf = wsGeral.Cells[row, 30].Value.ToString();
                                String novoCPF = String.Empty;
                                if (cpf.Length != 14)
                                {
                                    if (cpf.Length == 11)
                                    {
                                        if (cpf.Length == 11)
                                        {
                                            if (!cpf.Contains("."))
                                            {
                                                novoCPF = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);
                                                cpf = novoCPF;
                                            }
                                            else
                                            {
                                                cpf = null;
                                                flagBene = 0;
                                            }
                                        }
                                    }
                                }
                                if (cpf != null)
                                {
                                    if (!CrossCutting.ValidarCPF.IsCFPValid(cpf))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Beneficiário: CPF inválido. Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        cpf = null;
                                        flagBene = 0;
                                    }
                                }

                            }
                            if (wsGeral.Cells[row, 31].Value != null)
                            {
                                sexo = wsGeral.Cells[row, 31].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 32].Value != null)
                            {
                                nasc = wsGeral.Cells[row, 32].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 33].Value != null)
                            {
                                cel1 = wsGeral.Cells[row, 33].Value.ToString();
                                String novoCel = String.Empty;
                                if (cel1.Length != 14)
                                {
                                    if (cel1.Length == 11)
                                    {
                                        novoCel = "(" + cel1.Substring(0, 2) + ")" + cel1.Substring(2, 5) + "-" + cel1.Substring(7, 4);
                                        cel1 = novoCel;
                                    }
                                    else
                                    {
                                        cel1 = "-";
                                    }
                                }
                            }
                            if (wsGeral.Cells[row, 34].Value != null)
                            {
                                cel2 = wsGeral.Cells[row, 34].Value.ToString();
                                String novoCel = String.Empty;
                                if (cel2.Length != 14)
                                {
                                    if (cel1.Length == 11)
                                    {
                                        novoCel = "(" + cel2.Substring(0, 2) + ")" + cel2.Substring(2, 5) + "-" + cel2.Substring(7, 4);
                                        cel2 = novoCel;
                                    }
                                    else
                                    {
                                        cel2 = "-";
                                    }
                                }
                            }

                            if (nome == null || cpf == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Beneficiário: Nome e CPF não informados. Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                flagBene = 0;
                            }

                            DateTime checaNasc;
                            DateTime? dataNasc = null;
                            if (nasc != null)
                            {
                                Boolean data = DateTime.TryParse(nasc, out checaNasc);
                                if (!data)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_DS_MOTIVO = "Data de nascimento inválida. Beneficiário: " + nome + ". " + nasc + ". Prossegue a importação sem essa informação";
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                }
                                String x = nasc.ToString();
                                dataNasc = Convert.ToDateTime(x);
                            }

                            if (flagBene == 1)
                            {
                                BENEFICIARIO beneNovo = new BENEFICIARIO();
                                BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
                                if (bene == null)
                                {
                                    beneNovo.TIPE_CD_ID = 1;
                                    if (sexo == "M" || sexo == "F")
                                    {
                                        beneNovo.SEXO_CD_ID = sexo == "M" ? 1 : 2;
                                    }
                                    beneNovo.BENE_NM_NOME = nome;
                                    beneNovo.BENE_NR_CPF = cpf;
                                    beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
                                    beneNovo.BENE_NR_CELULAR = cel1;
                                    beneNovo.BENE_NR_CELULAR2 = cel2;
                                    beneNovo.BENE_DT_NASCIMENTO = dataNasc;
                                    beneNovo.BENE_IN_ATIVO = 1;
                                    Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
                                    if (voltaBene > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome + ". Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        prec.BENE_CD_ID = beneNovo.BENE_CD_ID;

                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Beneficiarios/" + beneNovo.BENE_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }
                            else
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome + ". BENEFICIÁRIO não criado por inconsistencia de dados";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                            }

                            if (flagBene == 1)
                            {
                                CLIENTE cliNovo = new CLIENTE();
                                CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
                                if (cli == null)
                                {
                                    cliNovo.TIPE_CD_ID = 1;
                                    if (sexo == "M" || sexo == "F")
                                    {
                                        cliNovo.SEXO_CD_ID = sexo == "M" ? 1 : 2;
                                    }
                                    cliNovo.CLIE_NM_NOME = nome;
                                    cliNovo.CLIE_NR_CPF = cpf;
                                    cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
                                    cliNovo.CLIE_NR_CELULAR = cel1;
                                    cliNovo.CLIE_NR_WHATSAPP = cel2;
                                    cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
                                    cliNovo.CLIE_IN_ATIVO = 1;
                                    cliNovo.CLIE_NM_EMAIL = String.Empty;
                                    Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
                                    if (voltaCli > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome + ". Prossegue a importação sem inclusão de CLIENTE";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Clientes/" + cliNovo.CLIE_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                        caminho = "/Imagens/1" + "/Clientes/" + cliNovo.CLIE_CD_ID.ToString() + "/Fotos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }

                            // Processa usuário
                            Int32 flagUsu = 1;
                            String usu = null;
                            if (wsGeral.Cells[row, 40].Value != null)
                            {
                                usu = wsGeral.Cells[row, 40].Value.ToString();
                            }

                            if (usu == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Usuário responsável não definido. Precatório " + numPrec + ". Prossegue a importação sem associação de RESPONSÁVEL";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                flagUsu = 0;
                            }

                            if (flagUsu == 1)
                            {
                                usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
                                if (usuCheca == null)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado. Precatório " + numPrec + ". Prossegue a importação sem associação de RESPONSÁVEL";
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                    prec.USUA_CD_ID = null;
                                }
                                else
                                {
                                    prec.USUA_CD_ID = Convert.ToInt32(usu);
                                }
                            }

                            // Grava objeto
                            if (colTRF != null)
                            {
                                prec.PREC_NM_NOME = nome;
                                Int32 volta = tranApp.ValidateCreate(prec, user);
                                if (volta > 0)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 1;
                                    fal.PRFA_DS_MOTIVO = "Erro na gravação do PRECATÓRIO. Precatório " + numPrec;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;
                                }
                                else
                                {
                                    conta++;

                                    // Cria pastas
                                    String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
                                    Directory.CreateDirectory(Server.MapPath(caminho));
                                }
                            }
                            else
                            {
                                Session["Conta"] = conta;
                                Session["Falha"] = falha;
                                Session["MensPrecatorio"] = 50;
                                Session["ListaPrecatorio"] = null;
                                Session["VoltaPrecatorio"] = 0;
                                return System.Threading.Tasks.Task.Delay(5);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return System.Threading.Tasks.Task.Delay(5);
                        }
                    }
                }
            
                // Processa TRF-4
                if (trf == 4)
                {
                    for (int row = 2; row <= wsFinalRow.Row; row++)
                    {
                        try
                        {
                            Int32 check = 0;
                            PRECATORIO prec = new PRECATORIO();
                            String colTRF = null;
                            if (wsGeral.Cells[row, 1].Value == null)
                            {
                                break;
                            }
                            else
                            {
                                colTRF = wsGeral.Cells[row, 1].Value.ToString();
                            }

                            // Verifica existencia
                            String numPrec = null;
                            if (wsGeral.Cells[row, 2].Value != null)
                            {
                                numPrec = wsGeral.Cells[row, 2].Value.ToString();
                                numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
                                precCheca = tranApp.CheckExist(numPrec);
                                if (precCheca != null)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 1;
                                    fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Precatório já incluído para o " + trfx.TRF1_NM_NOME;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;
                                }
                            }
                            else
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = "Não informado";
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 1;
                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Número não informado";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }

                            // Monta objeto
                            prec.TRF1_CD_ID = idTRF;
                            prec.PREC_NM_PRECATORIO = numPrec;
                            if (wsGeral.Cells[row, 3].Value != null)
                            {
                                prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 3].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_PROC_EXECUCAO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 4].Value != null)
                            {
                                prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 4].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_PROCESSO_ORIGEM = String.Empty;
                            }

                            if (wsGeral.Cells[row, 6].Value != null)
                            {
                                prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 6].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_REQUERENTE = String.Empty;
                            }

                            if (wsGeral.Cells[row, 7].Value != null)
                            {
                                prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 7].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_ADVOGADO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 5].Value != null)
                            {
                                prec.PREC_NM_DEPRECANTE = wsGeral.Cells[row, 8].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_DEPRECANTE = String.Empty;
                            }

                            if (wsGeral.Cells[row, 9].Value != null)
                            {
                                prec.PREC_NM_ASSUNTO = wsGeral.Cells[row, 9].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_ASSUNTO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 10].Value != null)
                            {
                                prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 10].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_REQUERIDO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 13].Value != null)
                            {
                                String valPrinc = wsGeral.Cells[row, 13].Value.ToString();
                                if (valPrinc != null)
                                {
                                    if (Regex.IsMatch(valPrinc, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 13].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO VALOR PRINCIPAL" + valPrinc;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
                            }

                            if ( wsGeral.Cells[row, 14].Value != null)
                            {
                                String juroBen = wsGeral.Cells[row, 14].Value.ToString();
                                if (juroBen != null)
                                {
                                    if (Regex.IsMatch(juroBen, @"\d"))
                                    {
                                        prec.PREC_VL_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 14].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO JUROS " + juroBen;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_JUROS = 0;
                                }

                            }
                            else
                            {
                                prec.PREC_VL_JUROS = 0;
                            }

                            if (wsGeral.Cells[row, 15].Value != null)
                            {
                                if (wsGeral.Cells[row, 15].Value.ToString() == "Sim" || wsGeral.Cells[row, 15].Value.ToString() == "S")
                                {
                                    prec.PREC_SG_BEN_IR_RRA = "1";
                                }
                                else
                                {
                                    prec.PREC_SG_BEN_IR_RRA = "0";
                                }
                            }
                            else
                            {
                                prec.PREC_SG_BEN_IR_RRA = "0";
                            }

                            if (wsGeral.Cells[row, 16].Value != null)
                            {
                                String mesAnt = wsGeral.Cells[row, 16].Value.ToString();
                                if (mesAnt != null & mesAnt != "0" & mesAnt != "-")
                                {
                                    if (Regex.IsMatch(mesAnt, @"\d"))
                                    {
                                        prec.PREC_BEN_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 16].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO MESES EXE ANTERIOR " + mesAnt;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
                                }

                            }
                            else
                            {
                                prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
                            }

                            if (wsGeral.Cells[row, 17].Value != null)
                            {
                                if (wsGeral.Cells[row, 17].Value.ToString() != null)
                                {
                                    String datx = wsGeral.Cells[row, 17].Value.ToString();
                                    DateTime outData = new DateTime();
                                    if (DateTime.TryParse(datx, out outData))
                                    {
                                        prec.PREC_DT_BEN_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 17].Value.ToString());
                                    }
                                    else
                                    {
                                        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 17].Value));
                                        if (dat == DateTime.MinValue)
                                        {
                                            prec.PREC_DT_BEN_DATABASE = null;
                                        }
                                        else
                                        {
                                            prec.PREC_DT_BEN_DATABASE = dat;
                                        }
                                    }
                                }
                                else
                                {
                                    prec.PREC_DT_BEN_DATABASE = null;
                                }
                            }
                            else
                            {
                                prec.PREC_DT_BEN_DATABASE = null;
                            }

                            if (wsGeral.Cells[row, 18].Value != null)
                            {
                                String iniPSS = wsGeral.Cells[row, 18].Value.ToString();
                                if (iniPSS != null)
                                {
                                    if (Regex.IsMatch(iniPSS, @"\d"))
                                    {
                                        prec.PREC_VL_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 18].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR INICIAL PSS" + iniPSS;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_VALOR_INICIAL_PSS = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_VALOR_INICIAL_PSS = 0;
                            }

                            if (wsGeral.Cells[row, 19].Value != null)
                            {
                                String valReq = wsGeral.Cells[row, 19].Value.ToString();
                                if (valReq != null)
                                {
                                    if (Regex.IsMatch(valReq, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 19].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR REQUISITADO" + valReq;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                            }

                            Int32 honoFlag = 1;
                            String honoNome = null;
                            String honoCPF = null;

                            if ( wsGeral.Cells[row, 20].Value != null)
                            {
                                honoNome = wsGeral.Cells[row, 20].Value.ToString();
                            }
                            else
                            {
                                honoNome = null;
                            }
                            if (wsGeral.Cells[row, 21].Value != null)
                            {
                                honoCPF = wsGeral.Cells[row, 21].Value.ToString();
                                String novoCPF = String.Empty;
                                if (honoCPF.Length == 11)
                                {
                                    if (!honoCPF.Contains("."))
                                    {
                                        novoCPF = honoCPF.Substring(0, 3) + "." + honoCPF.Substring(3, 3) + "." + honoCPF.Substring(6, 3) + "-" + honoCPF.Substring(9, 2);
                                        honoCPF = novoCPF;
                                    }
                                    else
                                    {
                                        honoCPF = null;
                                        honoFlag = 0;
                                    }
                                }
                                if (honoCPF.Length == 14)
                                {
                                    if (!honoCPF.Contains("."))
                                    {
                                        novoCPF = honoCPF.Substring(0, 2) + "." + honoCPF.Substring(2, 3) + "." + honoCPF.Substring(5, 3) + "/" + honoCPF.Substring(8, 4) + "-" + honoCPF.Substring(12, 2);
                                        honoCPF = novoCPF;
                                    }
                                }

                            }
                            else
                            {
                                honoCPF = null;
                                honoFlag = 0;
                            }

                            if ((honoCPF != null & honoNome == null) || (honoCPF == null & honoNome != null))
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Informações incompletas do ADVOGADO. Precatório: " + numPrec + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                honoFlag = 0;
                            }
                            if (honoCPF != null)
                            {
                                if (honoCPF.Length == 14)
                                {
                                    if (!CrossCutting.ValidarCPF.IsCFPValid(honoCPF))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "CPF do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        honoFlag = 0;
                                    }
                                }
                                else if (honoCPF.Length == 18)
                                {
                                    if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(honoCPF))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "CNPJ do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        honoFlag = 0;
                                    }
                                }
                                else
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    fal.PRFA_DS_MOTIVO = "CPF ou CNPJ do ADVOGADO inválido. " + honoCPF + ". Importação prosseguiu sem inclusão de ADVOGADO";
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                    honoFlag = 0;
                                }
                            }

                            if (honoFlag == 1)
                            {
                                honoCheca = honoApp.CheckExist(honoNome, honoCPF);
                                if (honoCheca != null)
                                {
                                    prec.HONO_CD_ID = honoCheca.HONO_CD_ID;
                                    prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
                                    prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());
                                }
                                else
                                {
                                    HONORARIO hono = new HONORARIO();
                                    hono.HONO_DT_CADASTRO = DateTime.Today.Date;
                                    hono.HONO_IN_ATIVO = 1;
                                    hono.HONO_NM_NOME = honoNome;
                                    String cpfCNPJ = honoCPF;
                                    if (cpfCNPJ.Length == 14)
                                    {
                                        hono.HONO_NR_CPF = cpfCNPJ;
                                        hono.TIPE_CD_ID = 1;
                                    }
                                    else
                                    {
                                        hono.HONO_NR_CNPJ = cpfCNPJ;
                                        hono.TIPE_CD_ID = 2;
                                    }
                                    Int32 volta3 = honoApp.ValidateCreate(hono, user);
                                    if (volta3 > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do ADVOGADO " + honoNome + ". Prossegue a importação sem inclusão de ADVOGADO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        prec.HONO_CD_ID = hono.HONO_CD_ID;
                                        prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
                                        prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());

                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Advogados/" + hono.HONO_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }

                            if (wsGeral.Cells[row, 24].Value != null)
                            {
                                if (wsGeral.Cells[row, 24].Value.ToString() == "Sim" || wsGeral.Cells[row, 24].Value.ToString() == "S")
                                {
                                    prec.PREC_SG_HON_IR_RRA = "1";
                                }
                                else
                                {
                                    prec.PREC_SG_HON_IR_RRA = "0";
                                }
                            }
                            else
                            {
                                prec.PREC_SG_HON_IR_RRA = "0";
                            }

                            if (wsGeral.Cells[row, 25].Value != null)
                            {
                                String honoMes = wsGeral.Cells[row, 25].Value.ToString();
                                if (honoMes != null)
                                {
                                    if (Regex.IsMatch(honoMes, @"\d"))
                                    {
                                        prec.PREC_IN_HON_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 25].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para HONORARIO MES ANTERIOR" + honoMes;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
                            }

                            if (wsGeral.Cells[row, 26].Value != null)
                            {
                                if (wsGeral.Cells[row, 26].Value.ToString() != null)
                                {
                                    String datx = wsGeral.Cells[row, 26].Value.ToString();
                                    DateTime outData = new DateTime();
                                    if (DateTime.TryParse(datx, out outData))
                                    {
                                        prec.PREC_DT_HON_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 26].Value.ToString());
                                    }
                                    else
                                    {
                                        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 26].Value));
                                        if (dat == DateTime.MinValue)
                                        {
                                            prec.PREC_DT_HON_DATABASE = null;
                                        }
                                        else
                                        {
                                            prec.PREC_DT_HON_DATABASE = dat;
                                        }
                                    }
                                }
                                else
                                {
                                    prec.PREC_DT_HON_DATABASE = null;
                                }
                            }
                            else
                            {
                                prec.PREC_DT_HON_DATABASE = null;
                            }

                            if (wsGeral.Cells[row, 27].Value != null)
                            {
                                String valPSS = wsGeral.Cells[row, 27].Value.ToString();
                                if (valPSS != null)
                                {
                                    if (Regex.IsMatch(valPSS, @"\d"))
                                    {
                                        prec.PREC_VL_HON_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 27].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO INICIAL PSS" + valPSS;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
                            }

                            if (wsGeral.Cells[row, 28].Value != null)
                            {
                                String honReq = wsGeral.Cells[row, 28].Value.ToString();
                                if (honReq != null)
                                {
                                    if (Regex.IsMatch(honReq, @"\d"))
                                    {
                                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 28].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO REQUISITADO" + honReq;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
                            }

                            if (wsGeral.Cells[row, 35].Value != null)
                            {
                                String anoProp = wsGeral.Cells[row, 35].Value.ToString();
                                if (anoProp != null)
                                {
                                    if (Regex.IsMatch(anoProp, @"\d{4}"))
                                    {
                                        prec.PREC_NR_ANO = anoProp;
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor inválido informado para ANO DA PROPOSTA " + anoProp;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        prec.PREC_NR_ANO = String.Empty;
                                    }
                                }
                                else
                                {
                                    prec.PREC_NR_ANO = String.Empty;
                                }
                            }
                            else
                            {
                                prec.PREC_NR_ANO = String.Empty;
                            }

                            if (wsGeral.Cells[row, 36].Value != null)
                            {
                                prec.PREC_NM_TIPO_DESPESA = wsGeral.Cells[row, 36].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_TIPO_DESPESA = String.Empty;
                            }

                            if (wsGeral.Cells[row, 37].Value != null)
                            {
                                String valRRA = wsGeral.Cells[row, 37].Value.ToString();
                                if (valRRA != null)
                                {
                                    if (Regex.IsMatch(valRRA, @"\d"))
                                    {
                                        prec.PREC_VL_RRA = Convert.ToDecimal(wsGeral.Cells[row, 37].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 1;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR RRA. Assumido 0. " + valRRA;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                        prec.PREC_VL_RRA = 0;
                                    }
                                }
                                else
                                {
                                    prec.PREC_VL_RRA = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_VL_RRA = 0;
                            }

                            if (wsGeral.Cells[row, 38].Value != null)
                            {
                                String percRRA = wsGeral.Cells[row, 38].Value.ToString();
                                if (percRRA != null)
                                {
                                    if (Regex.IsMatch(percRRA, @"\d"))
                                    {
                                        prec.PREC_PC_RRA = Convert.ToDecimal(wsGeral.Cells[row, 38].Value.ToString());
                                    }
                                    else
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Valor não numérico informado para PERCENTAGEM RRA. Assumido 0. " + percRRA;
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        prec.PREC_PC_RRA = 0;
                                    }
                                }
                                else
                                {
                                    prec.PREC_PC_RRA = 0;
                                }
                            }
                            else
                            {
                                prec.PREC_PC_RRA = 0;
                            }

                            if (wsGeral.Cells[row, 40].Value != null)
                            {
                                prec.PREC_NM_PREFERENCIA = wsGeral.Cells[row, 40].Value.ToString();
                            }
                            else
                            {
                                prec.PREC_NM_PREFERENCIA = String.Empty;
                            }
                            
                            Int32 flagBene = 1;
                            String nome = null;
                            String cpf = null;
                            String sexo = null;
                            String nasc = null;
                            String cel1 = null;
                            String cel2 = null;

                            if ( wsGeral.Cells[row, 29].Value != null)
                            {
                                nome = wsGeral.Cells[row, 29].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 30].Value != null)
                            {
                                cpf = wsGeral.Cells[row, 30].Value.ToString();
                                String novoCPF = String.Empty;
                                if (cpf.Length != 14)
                                {
                                    if (cpf.Length == 11)
                                    {
                                        if (cpf.Length == 11)
                                        {
                                            if (!cpf.Contains("."))
                                            {
                                                novoCPF = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);
                                                cpf = novoCPF;
                                            }
                                            else
                                            {
                                                cpf = null;
                                                flagBene = 0;
                                            }
                                        }
                                    }
                                }
                                if (cpf != null)
                                {
                                    if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(cpf))
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Beneficiário: CPF inválido. Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                        flagBene = 0;
                                        cpf = null;
                                    }
                                }

                            }
                            if (wsGeral.Cells[row, 31].Value != null)
                            {
                                sexo = wsGeral.Cells[row, 31].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 32].Value != null)
                            {
                                nasc = wsGeral.Cells[row, 32].Value.ToString();
                            }
                            if (wsGeral.Cells[row, 33].Value != null)
                            {
                                cel1 = wsGeral.Cells[row, 33].Value.ToString();
                                String novoCel = String.Empty;
                                if (cel1.Length != 14)
                                {
                                    if (cel1.Length == 11)
                                    {
                                        novoCel = "(" + cel1.Substring(0, 2) + ")" + cel1.Substring(2, 5) + "-" + cel1.Substring(7, 4);
                                        cel1 = novoCel;
                                    }
                                    else
                                    {
                                        cel1 = "-";
                                    }
                                }
                            }
                            if (wsGeral.Cells[row, 34].Value != null)
                            {
                                cel2 = wsGeral.Cells[row, 34].Value.ToString();
                                String novoCel = String.Empty;
                                if (cel2.Length != 14)
                                {
                                    if (cel1.Length == 11)
                                    {
                                        novoCel = "(" + cel2.Substring(0, 2) + ")" + cel2.Substring(2, 5) + "-" + cel2.Substring(7, 4);
                                        cel2 = novoCel;
                                    }
                                    else
                                    {
                                        cel2 = "-";
                                    }
                                }
                            }

                            if (nome == null || cpf == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Beneficiário: Nome e CPF não informados. Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                                flagBene = 0;
                            }

                            DateTime checaNasc;
                            DateTime? dataNasc = null;
                            if (nasc != null)
                            {
                                Boolean data = DateTime.TryParse(nasc, out checaNasc);
                                if (!data)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_DS_MOTIVO = "Data de nascimento inválida. Beneficiário: " + nome + ". " + nasc + ". Prossegue a importação sem essa informação";
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    falha++;
                                }
                                String x = nasc.ToString();
                                dataNasc = Convert.ToDateTime(x);
                            }

                            if (flagBene == 1)
                            {
                                BENEFICIARIO beneNovo = new BENEFICIARIO();
                                BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
                                if (bene == null)
                                {
                                    beneNovo.TIPE_CD_ID = 1;
                                    if (sexo == "M" || sexo == "F")
                                    {
                                        beneNovo.SEXO_CD_ID = sexo == "M" ? 1 : 2;
                                    }
                                    beneNovo.BENE_NM_NOME = nome;
                                    beneNovo.BENE_NR_CPF = cpf;
                                    beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
                                    beneNovo.BENE_NR_CELULAR = cel1;
                                    beneNovo.BENE_NR_CELULAR2 = cel2;
                                    beneNovo.BENE_DT_NASCIMENTO = dataNasc;
                                    beneNovo.BENE_IN_ATIVO = 1;
                                    Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
                                    if (voltaBene > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome + ". Prossegue a importação sem inclusão de BENEFICIÁRIO";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        falha++;
                                    }
                                    else
                                    {
                                        prec.BENE_CD_ID = beneNovo.BENE_CD_ID;

                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Beneficiarios/" + beneNovo.BENE_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }
                            else
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome + ". BENEFICIÁRIO não criado por inconsistencia de dados";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                falha++;
                            }

                            if (flagBene == 1)
                            {
                                CLIENTE cliNovo = new CLIENTE();
                                CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
                                if (cli == null)
                                {
                                    cliNovo.TIPE_CD_ID = 1;
                                    if (sexo == "M" || sexo == "F")
                                    {
                                        cliNovo.SEXO_CD_ID = sexo == "M" ? 1 : 2;
                                    }
                                    cliNovo.CLIE_NM_NOME = nome;
                                    cliNovo.CLIE_NR_CPF = cpf;
                                    cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
                                    cliNovo.CLIE_NR_CELULAR = cel1;
                                    cliNovo.CLIE_NR_WHATSAPP = cel2;
                                    cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
                                    cliNovo.CLIE_IN_ATIVO = 1;
                                    cliNovo.CLIE_NM_EMAIL = String.Empty;
                                    Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
                                    if (voltaCli > 0)
                                    {
                                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                        fal.PRFA_NM_PRECATORIO = numPrec;
                                        fal.PRFA_DT_DATA = DateTime.Now;
                                        fal.PRFA_IN_TIPO = 2;
                                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome + ". Prossegue a importação sem inclusão de CLIENTE";
                                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                        ajuste++;
                                    }
                                    else
                                    {
                                        // Cria pastas
                                        String caminho = "/Imagens/1" + "/Clientes/" + cliNovo.CLIE_CD_ID.ToString() + "/Anexos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                        caminho = "/Imagens/1" + "/Clientes/" + cliNovo.CLIE_CD_ID.ToString() + "/Fotos/";
                                        Directory.CreateDirectory(Server.MapPath(caminho));
                                    }
                                }
                            }

                            // Processa usuário
                            Int32 flagUsu = 1;
                            String usu = null;
                            if (wsGeral.Cells[row, 39].Value != null)
                            {
                                usu = wsGeral.Cells[row, 39].Value.ToString();
                            }

                            if (usu == null)
                            {
                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                fal.PRFA_NM_PRECATORIO = numPrec;
                                fal.PRFA_DT_DATA = DateTime.Now;
                                fal.PRFA_IN_TIPO = 2;
                                fal.PRFA_DS_MOTIVO = "Usuário responsável não definido. Precatório " + numPrec + ". Prossegue a importação sem associação de RESPONSÁVEL";
                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                ajuste++;
                                flagUsu = 0;
                            }

                            if (flagUsu == 1)
                            {
                                usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
                                if (usuCheca == null)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 2;
                                    fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado. Precatório " + numPrec + ". Prossegue a importação sem associação de RESPONSÁVEL";
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    ajuste++;
                                    prec.USUA_CD_ID = null;
                                }
                                else
                                {
                                    prec.USUA_CD_ID = Convert.ToInt32(usu);
                                }
                            }

                            // Grava objeto
                            if (colTRF != null)
                            {
                                prec.PREC_NM_NOME = nome;
                                Int32 volta = tranApp.ValidateCreate(prec, user);
                                if (volta > 0)
                                {
                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
                                    fal.PRFA_NM_PRECATORIO = numPrec;
                                    fal.PRFA_DT_DATA = DateTime.Now;
                                    fal.PRFA_IN_TIPO = 1;
                                    fal.PRFA_DS_MOTIVO = "Erro na gravação do PRECATÓRIO. Precatório " + numPrec;
                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;
                                }
                                else
                                {
                                    conta++;

                                    // Cria pastas
                                    String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
                                    Directory.CreateDirectory(Server.MapPath(caminho));
                                }
                            }
                            else
                            {
                                Session["Conta"] = conta;
                                Session["Falha"] = falha;
                                Session["MensPrecatorio"] = 50;
                                Session["ListaPrecatorio"] = null;
                                Session["VoltaPrecatorio"] = 0;
                                return System.Threading.Tasks.Task.Delay(5);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return System.Threading.Tasks.Task.Delay(5);
                        }
                    }
                }

                // Processa Callix
                if (trf == 0)
                {
                    for (int row = 2; row < wsFinalRow.Row; row++)
                    {
                        try
                        {
                            Int32 check = 0;
                            CONTATO prec = new CONTATO();

                            // Verifica existencia
                            String prot = wsGeral.Cells[row, 4].Value.ToString();
                            prot = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(prot);
                            contCheca = tranApp.CheckExistContato(prot);
                            if (contCheca != null)
                            {
                                CONTATO_FALHA fal = new CONTATO_FALHA();
                                fal.COFA_NR_PROTOCOLO = wsGeral.Cells[2, 4].Value.ToString();
                                fal.COFA_NR_PRECATORIO = wsGeral.Cells[2, 3].Value.ToString();
                                fal.COFA_NM_CONTATO = wsGeral.Cells[2, 7].Value.ToString();
                                fal.COFA_NM_OPERADOR = wsGeral.Cells[2, 9].Value.ToString();
                                fal.COFA_DT_DATA = DateTime.Now;
                                String datx = wsGeral.Cells[row, 2].Value.ToString();
                                DateTime outData = new DateTime();
                                if (DateTime.TryParse(datx, out outData))
                                {
                                    fal.COFA_DT_DATA_FALHA = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
                                }
                                else
                                {
                                    DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
                                    if (dat == DateTime.MinValue)
                                    {
                                        fal.COFA_DT_DATA_FALHA = null;
                                    }
                                    else
                                    {
                                        fal.COFA_DT_DATA_FALHA = dat;
                                    }
                                }
                                fal.COFA_DS_MOTIVO = "Contato já incluído para " + wsGeral.Cells[2, 4].Value.ToString();
                                Int32 volta2 = tranApp.ValidateCreateFalhaContato(fal);
                                falha++;
                                continue;
                            }

                            // Verifica precatorio
                            String numPrec = wsGeral.Cells[row, 3].Value.ToString();
                            numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
                            precCheca = tranApp.CheckExist(numPrec);
                            if (precCheca == null)
                            {
                                CONTATO_FALHA fal = new CONTATO_FALHA();
                                fal.COFA_NR_PROTOCOLO = wsGeral.Cells[2, 4].Value.ToString();
                                fal.COFA_NR_PRECATORIO = wsGeral.Cells[2, 3].Value.ToString();
                                fal.COFA_NM_CONTATO = wsGeral.Cells[2, 7].Value.ToString();
                                fal.COFA_NM_OPERADOR = wsGeral.Cells[2, 9].Value.ToString();
                                fal.COFA_DT_DATA = DateTime.Now;
                                String datx = wsGeral.Cells[row, 2].Value.ToString();
                                DateTime outData = new DateTime();
                                if (DateTime.TryParse(datx, out outData))
                                {
                                    fal.COFA_DT_DATA_FALHA = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
                                }
                                else
                                {
                                    DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
                                    if (dat == DateTime.MinValue)
                                    {
                                        fal.COFA_DT_DATA_FALHA = null;
                                    }
                                    else
                                    {
                                        fal.COFA_DT_DATA_FALHA = dat;
                                    }
                                }
                                fal.COFA_DS_MOTIVO = "Precatório inexistente para " + wsGeral.Cells[2, 4].Value.ToString();
                                Int32 volta2 = tranApp.ValidateCreateFalhaContato(fal);
                                falha++;
                                continue;
                            }

                            // Verifica beneficiario
                            //String nome = wsGeral.Cells[row, 7].Value.ToString();
                            //String cpf = wsGeral.Cells[row, 19].Value.ToString();
                            //beneCheca = beneApp.CheckExist(nome, cpf);
                            //if (beneCheca == null)
                            //{
                            //    CONTATO_FALHA fal = new CONTATO_FALHA();
                            //    fal.COFA_NR_PROTOCOLO = wsGeral.Cells[2, 4].Value.ToString();
                            //    fal.COFA_NR_PRECATORIO = wsGeral.Cells[2, 3].Value.ToString();
                            //    fal.COFA_NM_CONTATO = wsGeral.Cells[2, 7].Value.ToString();
                            //    fal.COFA_NM_OPERADOR = wsGeral.Cells[2, 9].Value.ToString();
                            //    fal.COFA_DT_DATA = DateTime.Now;
                            //    String datx = wsGeral.Cells[row, 2].Value.ToString();
                            //    DateTime outData = new DateTime();
                            //    if (DateTime.TryParse(datx, out outData))
                            //    {
                            //        fal.COFA_DT_DATA_FALHA = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
                            //    }
                            //    else
                            //    {
                            //        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
                            //        if (dat == DateTime.MinValue)
                            //        {
                            //            fal.COFA_DT_DATA_FALHA = null;
                            //        }
                            //        else
                            //        {
                            //            fal.COFA_DT_DATA_FALHA = dat;
                            //        }
                            //    }
                            //    fal.COFA_DS_MOTIVO = "Beneficiário inexistente para " + wsGeral.Cells[2, 4].Value.ToString();
                            //    Int32 volta2 = tranApp.ValidateCreateFalhaContato(fal);
                            //    falha++;
                            //    continue;
                            //}

                            // Monta objeto
                            if (wsGeral.Cells[row, 2].Value.ToString() != null)
                            {
                                String datx = wsGeral.Cells[row, 2].Value.ToString();
                                DateTime outData = new DateTime();
                                if (DateTime.TryParse(datx, out outData))
                                {
                                    prec.CONT_DT_CONTATO = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
                                }
                                else
                                {
                                    DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
                                    if (dat == DateTime.MinValue)
                                    {
                                        prec.CONT_DT_CONTATO = null;
                                    }
                                    else
                                    {
                                        prec.CONT_DT_CONTATO = dat;
                                    }
                                }
                            }
                            else
                            {
                                prec.CONT_DT_CONTATO = null;
                            }

                            String precNum = wsGeral.Cells[row, 3].Value.ToString();
                            precCheca = tranApp.CheckExist(precNum);
                            if (precCheca != null)
                            {
                                prec.PREC_CD_ID = precCheca.PREC_CD_ID;
                            }
                            else
                            {
                                PRECATORIO bene = new PRECATORIO();
                                bene.PREC_DT_CADASTRO = DateTime.Today.Date;
                                bene.PREC_IN_ATIVO = 1;
                                bene.PREC_NM_PRECATORIO = wsGeral.Cells[row, 3].Value.ToString();
                                bene.TRF1_CD_ID = 1;
                                Int32 volta3 = tranApp.ValidateCreate(bene, user);
                                prec.PREC_CD_ID = bene.PREC_CD_ID;
                            }

                            prec.CONT_NR_PROTOCOLO = wsGeral.Cells[row, 4].Value.ToString();
                            prec.CONT_NM_CAMPANHA = wsGeral.Cells[row, 5].Value.ToString();
                            prec.CONT_NM_LISTA = wsGeral.Cells[row, 6].Value.ToString();

                            String beneNome = wsGeral.Cells[row, 7].Value.ToString();
                            String beneCPF = wsGeral.Cells[row, 19].Value.ToString();
                            beneCheca = beneApp.CheckExist(beneNome, beneCPF);
                            if (beneCheca != null)
                            {
                                prec.BENE_CD_ID = beneCheca.BENE_CD_ID;
                            }
                            else
                            {
                                BENEFICIARIO bene = new BENEFICIARIO();
                                bene.BENE_DT_CADASTRO = DateTime.Today.Date;
                                bene.BENE_IN_ATIVO = 1;
                                bene.BENE_NM_NOME = wsGeral.Cells[row, 7].Value.ToString();
                                bene.BENE_NR_CPF = wsGeral.Cells[row, 19].Value.ToString();
                                bene.TIPE_CD_ID = 1;
                                Int32 volta3 = beneApp.ValidateCreate(bene, user);
                                prec.BENE_CD_ID = bene.BENE_CD_ID;

                                // Cria pastas
                                String caminho1 = "/Imagens/1" + "/Beneficiarios/" + prec.BENE_CD_ID.ToString() + "/Anexos/";
                                Directory.CreateDirectory(Server.MapPath(caminho1));
                            }

                            prec.CONT_NR_TELEFONE = wsGeral.Cells[row, 8].Value.ToString();
                            prec.CONT_NM_AGENTE = wsGeral.Cells[row, 9].Value.ToString();

                            String quali = wsGeral.Cells[row, 10].Value.ToString();
                            qualiCheca = conApp.CheckExistQualificacao(quali);
                            if (qualiCheca != null)
                            {
                                prec.QUAL_CD_ID = qualiCheca.QUAL_CD_ID;
                            }
                            else
                            {
                                QUALIFICACAO qualif = new QUALIFICACAO();
                                qualif.QUAL_IN_ATIVO = 1;
                                qualif.QUAL_NM_NOME = wsGeral.Cells[row, 10].Value.ToString();
                                Int32 volta3 = conApp.ValidateCreateQualificacao(qualif);
                                prec.QUAL_CD_ID = qualif.QUAL_CD_ID;
                            }

                            String quem = wsGeral.Cells[row, 11].Value.ToString();
                            quemCheca = conApp.CheckExistDesligamento(quali);
                            if (quemCheca != null)
                            {
                                prec.QUDE_CD_ID = quemCheca.QUDE_CD_ID;
                            }
                            else
                            {
                                QUEM_DESLIGOU desl = new QUEM_DESLIGOU();
                                desl.QUDE_IN_ATIVO = 1;
                                desl.QUDE_NM_NOME = wsGeral.Cells[row, 11].Value.ToString();
                                Int32 volta3 = conApp.ValidateCreateDesligamento(desl);
                                prec.QUDE_CD_ID = desl.QUDE_CD_ID;
                            }

                            prec.CONT_TM_DURACAO = wsGeral.Cells[row, 13].Value.ToString();
                            prec.CONT_TM_DURACAO_ATENDIMENTO = wsGeral.Cells[row, 14].Value.ToString();
                            
                            String audio = wsGeral.Cells[row, 15].Value.ToString();
                            prec.CONT_AQ_AUDIO = "/Audios/Ligacoes/" + audio;
                            String arquivo = caminhoAudio + "/" + audio;
                            String caminho = "/Audios/Ligacoes/";
                            String path = Path.Combine(Server.MapPath(caminho), audio);
                            if (System.IO.File.Exists(arquivo))
                            {
                                System.IO.File.Copy(arquivo, path);
                            }

                            prec.CONT_NM_ASSUNTO = wsGeral.Cells[row, 16].Value.ToString();
                            if (wsGeral.Cells[row, 17].Value != null)
                            {
                                prec.CONT_NR_CELULAR_1 = wsGeral.Cells[row, 17].Value.ToString();
                            }
                            else
                            {
                                prec.CONT_NR_CELULAR_1 = "-";
                            }
                            if (wsGeral.Cells[row, 18].Value != null)
                            {
                                prec.CONT_NR_CELULAR_2 = wsGeral.Cells[row, 18].Value.ToString();
                            }
                            else
                            {
                                prec.CONT_NR_CELULAR_2 = "-";
                            }
                            if (wsGeral.Cells[row, 22].Value != null)
                            {
                                prec.CONT_NR_TELEFONE_1 = wsGeral.Cells[row, 22].Value.ToString();
                            }
                            else
                            {
                                prec.CONT_NR_TELEFONE_1 = "-";
                            }
                            if (wsGeral.Cells[row, 23].Value != null)
                            {
                                prec.CONT_NR_TELEFONE_2 = wsGeral.Cells[row, 23].Value.ToString();
                            }
                            else
                            {
                                prec.CONT_NR_TELEFONE_2 = "-";
                            }
                            if (wsGeral.Cells[row, 25].Value != null)
                            {
                                prec.CONT_VL_RIDOLFI = wsGeral.Cells[row, 25].Value.ToString();
                            }
                            else
                            {
                                prec.CONT_VL_RIDOLFI = "-";
                            }

                            // Grava objeto
                            Int32 volta = conApp.ValidateCreate(prec, user);
                            conta++;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return System.Threading.Tasks.Task.Delay(5);
                        }
                    }
                }
            }

            // Finaliza
            if (trf != 0)
            {
                Session["Conta"] = conta;
                Session["Falha"] = falha;
                Session["Ajuste"] = ajuste;
                Session["MensPrecatorio"] = 10;
                Session["ListaPrecatorio"] = null;
                Session["VoltaPrecatorio"] = 0;
                return System.Threading.Tasks.Task.Delay(5);
            }
            else
            {
                Session["Conta"] = conta;
                Session["Falha"] = falha;
                Session["Ajuste"] = ajuste;
                Session["MensPrecatorio"] = 11;
                Session["ListaContato"] = null;
                Session["VoltaContato"] = 0;
                return System.Threading.Tasks.Task.Delay(5);
            }
        }

        [HttpPost]
        public async Task<ActionResult> IncluirPrecatorioTRFExcel(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            using (var pkg = new ExcelPackage(file.InputStream))
            {
                // Inicialização
                ExcelWorksheet wsGeral = pkg.Workbook.Worksheets[0];
                var wsFinalRow = wsGeral.Dimension.End;
                if (wsFinalRow.Row > 500)
                {
                    Session["MensPrecatorio"] = 200;
                    return RedirectToAction("MontarTelaPrecatorio");
                }
                if (wsGeral.Cells[2, 1].Value == null)
                {
                    Session["MensPrecatorio"] = 999;
                    return RedirectToAction("MontarTelaPrecatorio");
                }
            }

            await ProcessaOperacaoPlanilha(file);

            // Finaliza
            Session["ListaPrecatorio"] = null;
            Session["PrecatorioAlterada"] = 1;
            return RedirectToAction("MontarTelaPrecatorio");
        }


        //[HttpPost]
        //public ActionResult IncluirPrecatorioTRFExcel(HttpPostedFileBase file)
        //{
        //    USUARIO user = (USUARIO)Session["UserCredentials"];
        //    Int32 conta = 0;
        //    Int32 falha = 0;
        //    Int32 trf = 0;

        //    // Recupera configuracao
        //    CONFIGURACAO conf = confApp.GetItemById(1);
        //    String caminhoAudio = conf.CONF_NM_LOCAL_AUDIO;

        //    using (var pkg = new ExcelPackage(file.InputStream))
        //    {
        //        // Inicialização
        //        ExcelWorksheet wsGeral = pkg.Workbook.Worksheets[0];
        //        var wsFinalRow = wsGeral.Dimension.End;
        //        Int32 idTRF = 0;

        //        // Listas de pesquisas
        //        List<TRF> trfs = CarregaTRF();
        //        List<PRECATORIO> precs = CarregaPrecatorio();
        //        List<NATUREZA> nats = CarregaNatureza();
        //        PRECATORIO precCheca = new PRECATORIO();
        //        NATUREZA natCheca = new NATUREZA();
        //        BENEFICIARIO beneCheca = new BENEFICIARIO();
        //        HONORARIO honoCheca = new HONORARIO();
        //        TRF trfx = new TRF();
        //        CONTATO contCheca = new CONTATO();
        //        QUALIFICACAO qualiCheca = new QUALIFICACAO();
        //        QUEM_DESLIGOU quemCheca = new QUEM_DESLIGOU();
        //        List<BENEFICIARIO> listaBene = CarregaBeneficiario();
        //        List<CLIENTE> listaCli = CarregaCliente();
        //        USUARIO usuCheca = new USUARIO();

        //        // Checa planilha
        //        if (wsGeral.Cells[2, 1].Value.ToString() == null)
        //        {
        //            Session["MensPrecatorio"] = 999;
        //            return RedirectToAction("MontarTelaPrecatorio");
        //        }
        //        if (wsGeral.Cells[2, 1].Value.ToString() == "TRF3")
        //        {
        //            // Prcessa TRF
        //            trf = 3;
        //            trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-3").FirstOrDefault();
        //            idTRF = trfx.TRF1_CD_ID;
        //        }
        //        if (wsGeral.Cells[2, 1].Value.ToString() == "TRF5")
        //        {
        //            // Prcessa TRF
        //            trf = 5;
        //            trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-5").FirstOrDefault();
        //            idTRF = trfx.TRF1_CD_ID;
        //        }
        //        if (wsGeral.Cells[2, 1].Value.ToString() == "TRF2")
        //        {
        //            // Prcessa TRF
        //            trf = 2;
        //            trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-2").FirstOrDefault();
        //            idTRF = trfx.TRF1_CD_ID;
        //        }
        //        if (wsGeral.Cells[2, 1].Value.ToString() == "TRF4")
        //        {
        //            // Prcessa TRF
        //            trf = 4;
        //            trfx = trfs.Where(m => m.TRF1_NM_NOME == "TRF-4").FirstOrDefault();
        //            idTRF = trfx.TRF1_CD_ID;
        //        }
        //        if (wsGeral.Cells[2, 1].Value.ToString() == "DISC")
        //        {
        //            // Processa Callix
        //            trf = 0;
        //        }

        //        // Processa TRF3
        //        if (trf == 3)
        //        {
        //            for (int row = 2; row <= wsFinalRow.Row; row++)
        //            {
        //                try
        //                {
        //                    Int32 check = 0;
        //                    PRECATORIO prec = new PRECATORIO();

        //                    // Verifica existencia
        //                    String numPrec = wsGeral.Cells[row, 2].Value.ToString();
        //                    numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
        //                    precCheca = tranApp.CheckExist(numPrec);
        //                    if (precCheca != null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Precatório já incluído para o " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    // Monta objeto
        //                    if (true)
        //                    {

        //                    }
        //                    prec.TRF1_CD_ID = idTRF;
        //                    prec.PREC_NM_PRECATORIO = numPrec;
        //                    prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 3].Value.ToString();
        //                    prec.PREC_SG_PROCEDIMENTO = wsGeral.Cells[row, 4].Value.ToString();

        //                    if (wsGeral.Cells[row, 6].Value != null)
        //                    {
        //                        String x = wsGeral.Cells[row, 6].Value.ToString();
        //                        DateTime data = Convert.ToDateTime(x);
        //                        prec.PREC_DT_PROTOCOLO_TRF = data;
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_DT_PROTOCOLO_TRF = null;
        //                    }

        //                    prec.PREC_NM_OFICIO_REQUISITORIO = wsGeral.Cells[row, 8].Value.ToString();
        //                    prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 10].Value.ToString();
        //                    prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 11].Value.ToString();
        //                    prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 12].Value.ToString();
        //                    prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 13].Value.ToString();
        //                    prec.PREC_NR_ANO = wsGeral.Cells[row, 14].Value.ToString();
        //                    prec.PREC_VL_VALOR_INSCRITO_PROPOSTA = Convert.ToDecimal(wsGeral.Cells[row, 17].Value);
        //                    if (wsGeral.Cells[row, 18].Value.ToString() == "SIM")
        //                    {
        //                        prec.PREC_IN_SITUACAO_ATUAL = 1;
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_IN_SITUACAO_ATUAL = 2;
        //                    }
        //                    prec.PREC_NM_SITUACAO_REQUISICAO = wsGeral.Cells[row, 19].Value.ToString();
        //                    String nat = wsGeral.Cells[row, 21].Value.ToString();
        //                    natCheca = tranApp.CheckExistNatureza(nat);
        //                    if (natCheca != null)
        //                    {
        //                        prec.NATU_CD_ID = natCheca.NATU_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        prec.NATU_CD_ID = null;
        //                    }

        //                    String nome = wsGeral.Cells[row, 22].Value.ToString();
        //                    String cpf = wsGeral.Cells[row, 23].Value.ToString();
        //                    String sexo = wsGeral.Cells[row, 24].Value.ToString();
        //                    String nasc = wsGeral.Cells[row, 25].Value.ToString();
        //                    String cel1 = wsGeral.Cells[row, 26].Value.ToString();
        //                    String cel2 = wsGeral.Cells[row, 27].Value.ToString();
        //                    String tel = wsGeral.Cells[row, 28].Value.ToString();

        //                    DateTime? dataNasc = null;
        //                    if (nasc != null)
        //                    {
        //                        String x = nasc.ToString();
        //                        dataNasc = Convert.ToDateTime(x);
        //                    }

        //                    BENEFICIARIO beneNovo = new BENEFICIARIO();
        //                    if (nome != null && cpf != null)
        //                    {
        //                        BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
        //                        if (bene == null)
        //                        {
        //                            beneNovo.TIPE_CD_ID = 1;
        //                            if (sexo == "1" || sexo == "2")
        //                            {
        //                                beneNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
        //                            }
        //                            beneNovo.BENE_NM_NOME = nome;
        //                            beneNovo.BENE_NR_CPF = cpf;
        //                            beneNovo.BENE_NR_TELEFONE2 = tel;
        //                            beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
        //                            beneNovo.BENE_NR_CELULAR = cel1;
        //                            beneNovo.BENE_NR_CELULAR2 = cel2;
        //                            beneNovo.BENE_DT_NASCIMENTO = dataNasc;
        //                            beneNovo.BENE_IN_ATIVO = 1;
        //                            Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
        //                            if (voltaBene > 0)
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                            }
        //                            prec.BENE_CD_ID = beneNovo.BENE_CD_ID;
        //                        }
        //                    }

        //                    CLIENTE cliNovo = new CLIENTE();
        //                    if (nome != null && cpf != null)
        //                    {
        //                        CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
        //                        if (cli == null)
        //                        {
        //                            cliNovo.TIPE_CD_ID = 1;
        //                            if (sexo == "1" || sexo == "2")
        //                            {
        //                                cliNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
        //                            }
        //                            cliNovo.CLIE_NM_NOME = nome;
        //                            cliNovo.CLIE_NR_CPF = cpf;
        //                            cliNovo.CLIE_NR_TELEFONE = tel;
        //                            cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
        //                            cliNovo.CLIE_NR_CELULAR = cel1;
        //                            cliNovo.CLIE_NR_WHATSAPP = cel2;
        //                            cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
        //                            cliNovo.CLIE_IN_ATIVO = 1;
        //                            Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
        //                            if (voltaCli > 0)
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                            }
        //                        }
        //                    }

        //                    // Processa usuário
        //                    String usu = wsGeral.Cells[row, 29].Value.ToString();
        //                    if (usu == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Usuário responsável não definido " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
        //                    if (usuCheca == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }
        //                    prec.USUA_CD_ID = Convert.ToInt32(usu);

        //                    // Grava objeto
        //                    prec.PREC_NM_NOME = nome;
        //                    Int32 volta = tranApp.ValidateCreate(prec, user);
        //                    conta++;

        //                    // Cria pastas
        //                    String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
        //                    Directory.CreateDirectory(Server.MapPath(caminho));
        //                }
        //                catch (Exception ex)
        //                {
        //                    ModelState.AddModelError("", ex.Message);
        //                    return View();
        //                }
        //            }
        //        }

        //        // Processa TRF-5
        //        if (trf == 5)
        //        {
        //            for (int row = 2; row <= wsFinalRow.Row; row++)
        //            {
        //                try
        //                {
        //                    Int32 check = 0;
        //                    PRECATORIO prec = new PRECATORIO();

        //                    // Verifica existencia
        //                    String numPrec = wsGeral.Cells[row, 2].Value.ToString();
        //                    numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
        //                    precCheca = tranApp.CheckExist(numPrec);
        //                    if (precCheca != null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Precatório já incluído para o " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    // Monta objeto
        //                    prec.TRF1_CD_ID = idTRF;
        //                    prec.PREC_NM_PRECATORIO = numPrec;
        //                    prec.PREC_NR_ANO = wsGeral.Cells[row, 3].Value.ToString();
        //                    prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 11].Value.ToString();
        //                    prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 12].Value.ToString();
        //                    prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 13].Value.ToString();
        //                    prec.PREC_NM_DEPRECANTE = wsGeral.Cells[row, 14].Value.ToString();
        //                    prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 15].Value.ToString();
        //                    prec.PREC_NM_OFICIO_REQUISITORIO = wsGeral.Cells[row, 16].Value.ToString();
        //                    prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 17].Value.ToString();
        //                    prec.PREC_NM_ASSUNTO = wsGeral.Cells[row, 21].Value.ToString();
        //                    prec.PREC_NM_SITUACAO_REQUISICAO = wsGeral.Cells[row, 22].Value.ToString();
        //                    String nat = wsGeral.Cells[row, 20].Value.ToString();
        //                    natCheca = tranApp.CheckExistNatureza(nat);
        //                    if (natCheca != null)
        //                    {
        //                        prec.NATU_CD_ID = natCheca.NATU_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        prec.NATU_CD_ID = null;
        //                    }

        //                    String nome = wsGeral.Cells[row, 25].Value.ToString();
        //                    String cpf = wsGeral.Cells[row, 26].Value.ToString();
        //                    String sexo = wsGeral.Cells[row, 27].Value.ToString();
        //                    String nasc = wsGeral.Cells[row, 28].Value.ToString();
        //                    String cel1 = wsGeral.Cells[row, 29].Value.ToString();
        //                    String cel2 = wsGeral.Cells[row, 30].Value.ToString();
        //                    String tel = wsGeral.Cells[row, 31].Value.ToString();
        //                    DateTime? dataNasc = null;
        //                    if (nasc != null)
        //                    {
        //                        String x = nasc.ToString();
        //                        dataNasc = Convert.ToDateTime(x);
        //                    }

        //                    BENEFICIARIO beneNovo = new BENEFICIARIO();
        //                    if (nome != null && cpf != null)
        //                    {
        //                        BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
        //                        if (bene == null)
        //                        {
        //                            beneNovo.TIPE_CD_ID = 1;
        //                            if (sexo == "1" || sexo == "2")
        //                            {
        //                                beneNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
        //                            }
        //                            beneNovo.BENE_NM_NOME = nome;
        //                            beneNovo.BENE_NR_CPF = cpf;
        //                            beneNovo.BENE_NR_TELEFONE2 = tel;
        //                            beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
        //                            beneNovo.BENE_NR_CELULAR = cel1;
        //                            beneNovo.BENE_NR_CELULAR2 = cel2;
        //                            beneNovo.BENE_DT_NASCIMENTO = dataNasc;
        //                            beneNovo.BENE_IN_ATIVO = 1;
        //                            Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
        //                            if (voltaBene > 0)
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                            }
        //                            prec.BENE_CD_ID = beneNovo.BENE_CD_ID;
        //                        }
        //                    }

        //                    CLIENTE cliNovo = new CLIENTE();
        //                    if (nome != null && cpf != null)
        //                    {
        //                        CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
        //                        if (cli == null)
        //                        {
        //                            cliNovo.TIPE_CD_ID = 1;
        //                            if (sexo == "1" || sexo == "2")
        //                            {
        //                                cliNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
        //                            }
        //                            cliNovo.CLIE_NM_NOME = nome;
        //                            cliNovo.CLIE_NR_CPF = cpf;
        //                            cliNovo.CLIE_NR_TELEFONE = tel;
        //                            cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
        //                            cliNovo.CLIE_NR_CELULAR = cel1;
        //                            cliNovo.CLIE_NR_WHATSAPP = cel2;
        //                            cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
        //                            cliNovo.CLIE_IN_ATIVO = 1;
        //                            Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
        //                            if (voltaCli > 0)
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                            }
        //                        }
        //                    }

        //                    // Processa usuário
        //                    String usu = wsGeral.Cells[row, 32].Value.ToString();
        //                    if (usu == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Usuário responsável não definido " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
        //                    if (usuCheca == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }
        //                    prec.USUA_CD_ID = Convert.ToInt32(usu);

        //                    // Grava objeto
        //                    prec.PREC_NM_NOME = nome;
        //                    Int32 volta = tranApp.ValidateCreate(prec, user);
        //                    conta++;

        //                    // Cria pastas
        //                    String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
        //                    Directory.CreateDirectory(Server.MapPath(caminho));
        //                }
        //                catch (Exception ex)
        //                {
        //                    ModelState.AddModelError("", ex.Message);
        //                    return View();
        //                }
        //            }
        //        }

        //        // Processa TRF-2
        //        if (trf == 2)
        //        {
        //            for (int row = 2; row <= wsFinalRow.Row; row++)
        //            {
        //                try
        //                {
        //                    if (wsGeral.Cells[row, 1].Value == null)
        //                    {
        //                        break;
        //                    }                          

        //                    Int32 check = 0;
        //                    PRECATORIO prec = new PRECATORIO();

        //                    // Verifica existencia
        //                    String numPrec = String.Empty;
        //                    if (wsGeral.Cells[row, 2].Value != null)
        //                    {
        //                        numPrec = wsGeral.Cells[row, 2].Value.ToString();
        //                        numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
        //                        precCheca = tranApp.CheckExist(numPrec);
        //                        if (precCheca != null)
        //                        {
        //                            PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                            fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                            fal.PRFA_NM_PRECATORIO = numPrec;
        //                            fal.PRFA_DT_DATA = DateTime.Now;
        //                            fal.PRFA_DS_MOTIVO = "Precatório já incluído para o " + trfx.TRF1_NM_NOME;
        //                            Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                            falha++;
        //                            continue;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Precatório não informado " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    // Monta objeto
        //                    prec.TRF1_CD_ID = idTRF;
        //                    prec.PREC_NM_PRECATORIO = numPrec;
        //                    if (wsGeral.Cells[row, 3].Value != null)
        //                    {
        //                        prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 3].Value.ToString();

        //                    }
        //                    if (wsGeral.Cells[row, 4].Value != null)
        //                    {
        //                        prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 4].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 6].Value != null)
        //                    {
        //                        prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 6].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 7].Value != null)
        //                    {
        //                        prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 7].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 8].Value != null)
        //                    {
        //                        prec.PREC_NM_DEPRECANTE = wsGeral.Cells[row, 8].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 9].Value != null)
        //                    {
        //                        prec.PREC_NM_ASSUNTO = wsGeral.Cells[row, 9].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 10].Value != null)
        //                    {
        //                        prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 10].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 13].Value != null)
        //                    {
        //                        prec.PREC_VL_BEN_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 13].Value.ToString());

        //                    }

        //                    if (wsGeral.Cells[row, 14].Value != null)
        //                    {
        //                        prec.PREC_VL_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 14].Value.ToString());

        //                    }

        //                    if (wsGeral.Cells[row, 15].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 15].Value.ToString() == "Sim")
        //                        {
        //                            prec.PREC_SG_BEN_IR_RRA = "1";
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_SG_BEN_IR_RRA = "0";
        //                        }

        //                    }

        //                    if (wsGeral.Cells[row, 16].Value == null)
        //                    {
        //                        prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_BEN_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 16].Value.ToString());
        //                    }

        //                    if (wsGeral.Cells[row, 17].Value != null)
        //                    {
        //                        String datx = wsGeral.Cells[row, 17].Value.ToString();
        //                        DateTime outData = new DateTime();
        //                        if (DateTime.TryParse(datx, out outData))
        //                        {
        //                            prec.PREC_DT_BEN_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 17].Value.ToString());
        //                        }
        //                        else
        //                        {
        //                            DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 17].Value));
        //                            if (dat == DateTime.MinValue)
        //                            {
        //                                prec.PREC_DT_BEN_DATABASE = null;
        //                            }
        //                            else
        //                            {
        //                                prec.PREC_DT_BEN_DATABASE = dat;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_DT_BEN_DATABASE = null;
        //                    }

        //                    if (wsGeral.Cells[row, 18].Value == null)
        //                    {
        //                        prec.PREC_VL_VALOR_INICIAL_PSS = 0;
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 18].Value.ToString());
        //                    }

        //                    if (wsGeral.Cells[row, 19].Value != null)
        //                    {
        //                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 19].Value.ToString());

        //                    }

        //                    if (wsGeral.Cells[row, 20].Value != null & wsGeral.Cells[row, 21].Value != null)
        //                    {
        //                        String honoNome = wsGeral.Cells[row, 20].Value.ToString();
        //                        String honoCPF = wsGeral.Cells[row, 21].Value.ToString();
        //                        honoCheca = honoApp.CheckExist(honoNome, honoCPF);
        //                        if (honoCheca != null)
        //                        {
        //                            prec.HONO_CD_ID = honoCheca.HONO_CD_ID;
        //                        }
        //                        else
        //                        {
        //                            HONORARIO hono = new HONORARIO();
        //                            hono.HONO_DT_CADASTRO = DateTime.Today.Date;
        //                            hono.HONO_IN_ATIVO = 1;
        //                            hono.HONO_NM_NOME = wsGeral.Cells[row, 20].Value.ToString();
        //                            if (CrossCutting.ValidarNumerosDocumentos.IsCFPValid(honoCPF))
        //                            {
        //                                hono.HONO_NR_CPF = wsGeral.Cells[row, 21].Value.ToString();
        //                                hono.TIPE_CD_ID = 1;
        //                            }
        //                            else if (CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(honoCPF))
        //                            {
        //                                hono.HONO_NR_CNPJ = wsGeral.Cells[row, 21].Value.ToString();
        //                                hono.TIPE_CD_ID = 2;
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "CPF/CNPJ Advogado - Inválido " + trfx.TRF1_NM_NOME;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                            Int32 volta3 = honoApp.ValidateCreate(hono, user);
        //                            prec.HONO_CD_ID = hono.HONO_CD_ID;
        //                        }
        //                    }

        //                    if (wsGeral.Cells[row, 22].Value != null)
        //                    {
        //                        prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());

        //                    }

        //                    if (wsGeral.Cells[row, 23].Value != null)
        //                    {
        //                        prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());

        //                    }

        //                    if (wsGeral.Cells[row, 24].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 24].Value.ToString() == "Sim")
        //                        {
        //                            prec.PREC_SG_HON_IR_RRA = "1";
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_SG_HON_IR_RRA = "0";
        //                        }

        //                    }

        //                    if (wsGeral.Cells[row, 25].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 25].Value.ToString() == "-")
        //                        {
        //                            prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_IN_HON_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 25].Value.ToString());
        //                        }

        //                    }

        //                    if (wsGeral.Cells[row, 26].Value != null)
        //                    {
        //                        String datx = wsGeral.Cells[row, 26].Value.ToString();
        //                        DateTime outData = new DateTime();
        //                        if (DateTime.TryParse(datx, out outData))
        //                        {
        //                            prec.PREC_DT_HON_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 26].Value.ToString());
        //                        }
        //                        else
        //                        {
        //                            DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 26].Value));
        //                            if (dat == DateTime.MinValue)
        //                            {
        //                                prec.PREC_DT_HON_DATABASE = null;
        //                            }
        //                            else
        //                            {
        //                                prec.PREC_DT_HON_DATABASE = dat;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_DT_HON_DATABASE = null;
        //                    }

        //                    if (wsGeral.Cells[row, 27].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 27].Value.ToString() == "-")
        //                        {
        //                            prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_HON_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 27].Value.ToString());
        //                        }

        //                    }

        //                    if (wsGeral.Cells[row, 28].Value != null)
        //                    {
        //                        prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 28].Value.ToString());

        //                    }

        //                    String nome = String.Empty;
        //                    String cpf= String.Empty;
        //                    String nasc = String.Empty;
        //                    String sexo = String.Empty;
        //                    String cel1 = String.Empty;
        //                    String cel2 = String.Empty;
        //                    String tel = String.Empty;
        //                    String nat = String.Empty;
        //                    String usu = String.Empty;

        //                    if (wsGeral.Cells[row, 29].Value != null)
        //                    {
        //                        nome = wsGeral.Cells[row, 29].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 30].Value != null)
        //                    {
        //                        cpf = wsGeral.Cells[row, 30].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 31].Value != null)
        //                    {
        //                        sexo = wsGeral.Cells[row, 31].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 32].Value != null)
        //                    {
        //                        nasc = wsGeral.Cells[row, 32].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 33].Value != null)
        //                    {
        //                        cel1 = wsGeral.Cells[row, 33].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 34].Value != null)
        //                    {
        //                        cel2 = wsGeral.Cells[row, 34].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 35].Value != null)
        //                    {
        //                        tel = wsGeral.Cells[row, 35].Value.ToString();

        //                    }

        //                    DateTime? dataNasc = null;
        //                    if (nasc != String.Empty)
        //                    {
        //                        String x = nasc.ToString();
        //                        dataNasc = Convert.ToDateTime(x);
        //                    }

        //                    BENEFICIARIO beneNovo = new BENEFICIARIO();
        //                    if (nome != String.Empty && cpf != String.Empty)
        //                    {
        //                        BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
        //                        if (bene == null)
        //                        {
        //                            beneNovo.TIPE_CD_ID = 1;
        //                            if (sexo == "1" || sexo == "2")
        //                            {
        //                                beneNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
        //                            }
        //                            beneNovo.BENE_NM_NOME = nome;
        //                            beneNovo.BENE_NR_CPF = cpf;
        //                            beneNovo.BENE_NR_TELEFONE2 = tel;
        //                            beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
        //                            beneNovo.BENE_NR_CELULAR = cel1;
        //                            beneNovo.BENE_NR_CELULAR2 = cel2;
        //                            beneNovo.BENE_DT_NASCIMENTO = dataNasc;
        //                            beneNovo.BENE_IN_ATIVO = 1;
        //                            Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
        //                            if (voltaBene > 0)
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                            }
        //                            prec.BENE_CD_ID = beneNovo.BENE_CD_ID;
        //                        }
        //                        else
        //                        {
        //                            prec.BENE_CD_ID = beneNovo.BENE_CD_ID;
        //                        }

        //                        CLIENTE cliNovo = new CLIENTE();
        //                        if (nome != null && cpf != null)
        //                        {
        //                            CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
        //                            if (cli == null)
        //                            {
        //                                cliNovo.TIPE_CD_ID = 1;
        //                                if (sexo == "1" || sexo == "2")
        //                                {
        //                                    cliNovo.SEXO_CD_ID = Convert.ToInt32(sexo);
        //                                }
        //                                cliNovo.CLIE_NM_NOME = nome;
        //                                cliNovo.CLIE_NR_CPF = cpf;
        //                                cliNovo.CLIE_NR_TELEFONE = tel;
        //                                cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
        //                                cliNovo.CLIE_NR_CELULAR = cel1;
        //                                cliNovo.CLIE_NR_WHATSAPP = cel2;
        //                                cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
        //                                cliNovo.CLIE_IN_ATIVO = 1;
        //                                Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
        //                                if (voltaCli > 0)
        //                                {
        //                                    PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                    fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                    fal.PRFA_NM_PRECATORIO = numPrec;
        //                                    fal.PRFA_DT_DATA = DateTime.Now;
        //                                    fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome;
        //                                    Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                    falha++;
        //                                }
        //                            }
        //                        }

        //                    }

        //                    if (wsGeral.Cells[row, 37].Value != null)
        //                    {
        //                        nat = wsGeral.Cells[row, 37].Value.ToString();

        //                    }
        //                    natCheca = tranApp.CheckExistNatureza(nat);
        //                    if (natCheca != null)
        //                    {
        //                        prec.NATU_CD_ID = natCheca.NATU_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        NATUREZA natNova = new NATUREZA();
        //                        natNova.NATU_IN_ATIVO = 1;
        //                        natNova.NATU_NM_NOME = nat;
        //                        Int32 voltaNat = natApp.ValidateCreate(natNova, user);
        //                        if (voltaNat > 0)
        //                        {
        //                            PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                            fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                            fal.PRFA_NM_PRECATORIO = numPrec;
        //                            fal.PRFA_DT_DATA = DateTime.Now;
        //                            fal.PRFA_DS_MOTIVO = "Erro na inclusão da natureza " + nat;
        //                            Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                            falha++;
        //                            prec.NATU_CD_ID = null;
        //                        }
        //                        else
        //                        {
        //                            prec.NATU_CD_ID = natNova.NATU_CD_ID;
        //                        }
        //                    }

        //                    if (wsGeral.Cells[row, 36].Value != null)
        //                    {
        //                        prec.PREC_NR_ANO = wsGeral.Cells[row, 36].Value.ToString();

        //                    }

        //                    if (wsGeral.Cells[row, 38].Value != null)
        //                    {
        //                        prec.PREC_VL_RRA = Convert.ToDecimal(wsGeral.Cells[row, 38].Value.ToString());

        //                    }

        //                    if (wsGeral.Cells[row, 39].Value != null)
        //                    {
        //                        prec.PREC_PC_RRA = Convert.ToDecimal(wsGeral.Cells[row, 39].Value.ToString());

        //                    }

        //                    // Processa usuário
        //                    if (wsGeral.Cells[row, 40].Value != null)
        //                    {
        //                        usu = wsGeral.Cells[row, 40].Value.ToString();

        //                    }

        //                    if (usu == String.Empty)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Usuário responsável não definido " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
        //                    if (usuCheca == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado " + trfx.TRF1_NM_NOME;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }
        //                    prec.USUA_CD_ID = Convert.ToInt32(usu);

        //                    // Grava objeto
        //                    prec.PREC_NM_NOME = nome;
        //                    Int32 volta = tranApp.ValidateCreate(prec, user);
        //                    conta++;

        //                    // Cria pastas
        //                    String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
        //                    Directory.CreateDirectory(Server.MapPath(caminho));
        //                }
        //                catch (Exception ex)
        //                {
        //                    ModelState.AddModelError("", ex.Message);
        //                    return View();
        //                }
        //            }
        //        }

        //        // Processa TRF-4
        //        if (trf == 4)
        //        {
        //            for (int row = 2; row <= wsFinalRow.Row; row++)
        //            {
        //                try
        //                {
        //                    Int32 check = 0;
        //                    PRECATORIO prec = new PRECATORIO();
        //                    String colTRF = null;
        //                    if (wsGeral.Cells[row, 1].Value == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = "Não informado";
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Tribunal não informado. Importação Encerrada";
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        colTRF = wsGeral.Cells[row, 1].Value.ToString();
        //                    }

        //                    // Verifica existencia
        //                    String numPrec = null;
        //                    if (wsGeral.Cells[row, 2].Value != null)
        //                    {
        //                        numPrec = wsGeral.Cells[row, 2].Value.ToString();
        //                        numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
        //                        precCheca = tranApp.CheckExist(numPrec);
        //                        if (precCheca != null)
        //                        {
        //                            PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                            fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                            fal.PRFA_NM_PRECATORIO = numPrec;
        //                            fal.PRFA_DT_DATA = DateTime.Now;
        //                            fal.PRFA_DS_MOTIVO = "Precatório já incluído para o " + trfx.TRF1_NM_NOME;
        //                            Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                            falha++;
        //                            continue;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = "Não informado";
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Erro na inclusão do Precatório. Número não informado";
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    // Monta objeto
        //                    prec.TRF1_CD_ID = idTRF;
        //                    prec.PREC_NM_PRECATORIO = numPrec;
        //                    if (wsGeral.Cells[row, 3].Value != null)
        //                    {
        //                        prec.PREC_NM_PROC_EXECUCAO = wsGeral.Cells[row, 3].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_PROC_EXECUCAO = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 4].Value != null)
        //                    {
        //                        prec.PREC_NM_PROCESSO_ORIGEM = wsGeral.Cells[row, 4].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_PROCESSO_ORIGEM = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 6].Value != null)
        //                    {
        //                        prec.PREC_NM_REQUERENTE = wsGeral.Cells[row, 6].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_REQUERENTE = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 7].Value != null)
        //                    {
        //                        prec.PREC_NM_ADVOGADO = wsGeral.Cells[row, 7].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_ADVOGADO = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 5].Value != null)
        //                    {
        //                        prec.PREC_NM_DEPRECANTE = wsGeral.Cells[row, 8].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_DEPRECANTE = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 9].Value != null)
        //                    {
        //                        prec.PREC_NM_ASSUNTO = wsGeral.Cells[row, 9].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_ASSUNTO = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 10].Value != null)
        //                    {
        //                        prec.PREC_NM_REQUERIDO = wsGeral.Cells[row, 10].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_REQUERIDO = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 13].Value != null)
        //                    {
        //                        String valPrinc = wsGeral.Cells[row, 13].Value.ToString();
        //                        if (valPrinc != null)
        //                        {
        //                            if (Regex.IsMatch(valPrinc, @"\d"))
        //                            {
        //                                prec.PREC_VL_BEN_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 13].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO VALOR PRINCIPAL" + valPrinc;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_BEN_VALOR_PRINCIPAL = 0;
        //                    }

        //                    if ( wsGeral.Cells[row, 14].Value != null)
        //                    {
        //                        String juroBen = wsGeral.Cells[row, 14].Value.ToString();
        //                        if (juroBen != null)
        //                        {
        //                            if (Regex.IsMatch(juroBen, @"\d"))
        //                            {
        //                                prec.PREC_VL_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 14].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO JUROS " + juroBen;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_JUROS = 0;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_JUROS = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 15].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 15].Value.ToString() == "Sim" || wsGeral.Cells[row, 15].Value.ToString() == "S")
        //                        {
        //                            prec.PREC_SG_BEN_IR_RRA = "1";
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_SG_BEN_IR_RRA = "0";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_SG_BEN_IR_RRA = "0";
        //                    }

        //                    if (wsGeral.Cells[row, 16].Value != null)
        //                    {
        //                        String mesAnt = wsGeral.Cells[row, 16].Value.ToString();
        //                        if (mesAnt != null)
        //                        {
        //                            if (Regex.IsMatch(mesAnt, @"\d"))
        //                            {
        //                                prec.PREC_BEN_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 16].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para BENEFICARIO MESES EXE ANTERIOR " + mesAnt;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        prec.PREC_BEN_MESES_EXE_ANTERIOR = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 17].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 17].Value.ToString() != null)
        //                        {
        //                            String datx = wsGeral.Cells[row, 17].Value.ToString();
        //                            DateTime outData = new DateTime();
        //                            if (DateTime.TryParse(datx, out outData))
        //                            {
        //                                prec.PREC_DT_BEN_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 17].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 17].Value));
        //                                if (dat == DateTime.MinValue)
        //                                {
        //                                    prec.PREC_DT_BEN_DATABASE = null;
        //                                }
        //                                else
        //                                {
        //                                    prec.PREC_DT_BEN_DATABASE = dat;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_DT_BEN_DATABASE = null;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_DT_BEN_DATABASE = null;
        //                    }

        //                    if (wsGeral.Cells[row, 18].Value != null)
        //                    {
        //                        String iniPSS = wsGeral.Cells[row, 18].Value.ToString();
        //                        if (iniPSS != null)
        //                        {
        //                            if (Regex.IsMatch(iniPSS, @"\d"))
        //                            {
        //                                prec.PREC_VL_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 18].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR INICIAL PSS" + iniPSS;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_VALOR_INICIAL_PSS = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_VALOR_INICIAL_PSS = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 19].Value != null)
        //                    {
        //                        String valReq = wsGeral.Cells[row, 19].Value.ToString();
        //                        if (valReq != null)
        //                        {
        //                            if (Regex.IsMatch(valReq, @"\d"))
        //                            {
        //                                prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 19].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR REQUISITADO" + valReq;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
        //                    }

        //                    Int32 honoFlag = 1;
        //                    String honoNome = null;
        //                    String honoCPF = null;

        //                    if ( wsGeral.Cells[row, 20].Value != null)
        //                    {
        //                        honoNome = wsGeral.Cells[row, 20].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        honoNome = null;
        //                    }
        //                    if (wsGeral.Cells[row, 21].Value != null)
        //                    {
        //                        honoCPF = wsGeral.Cells[row, 21].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        honoCPF = null;
        //                        honoFlag = 0;
        //                    }

        //                    if ((honoCPF != null & honoNome == null) || (honoCPF == null & honoNome != null))
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Informações incompletas do ADVOGADO. Precatório: " + numPrec;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        honoFlag = 0;
        //                    }
        //                    if (honoCPF != null)
        //                    {
        //                        if (honoCPF.Length == 14)
        //                        {
        //                            if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(honoCPF))
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "CPF do ADVOGADO inválido. " + honoCPF;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                honoFlag = 0;
        //                            }
        //                        }
        //                        else if (honoCPF.Length == 18)
        //                        {
        //                            if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(honoCPF))
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "CNPJ do ADVOGADO inválido. " + honoCPF;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                honoFlag = 0;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                            fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                            fal.PRFA_NM_PRECATORIO = numPrec;
        //                            fal.PRFA_DT_DATA = DateTime.Now;
        //                            fal.PRFA_DS_MOTIVO = "CPF ou CNPJ do ADVOGADO inválido. " + honoCPF;
        //                            Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                            falha++;
        //                            honoFlag = 0;
        //                        }
        //                    }

        //                    if (honoFlag == 1)
        //                    {
        //                        honoCheca = honoApp.CheckExist(honoNome, honoCPF);
        //                        if (honoCheca != null)
        //                        {
        //                            prec.HONO_CD_ID = honoCheca.HONO_CD_ID;
        //                            prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
        //                            prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());
        //                        }
        //                        else
        //                        {
        //                            HONORARIO hono = new HONORARIO();
        //                            hono.HONO_DT_CADASTRO = DateTime.Today.Date;
        //                            hono.HONO_IN_ATIVO = 1;
        //                            hono.HONO_NM_NOME = honoNome;
        //                            String cpfCNPJ = honoCPF;
        //                            if (cpfCNPJ.Length == 14)
        //                            {
        //                                hono.HONO_NR_CPF = cpfCNPJ;
        //                                hono.TIPE_CD_ID = 1;
        //                            }
        //                            else
        //                            {
        //                                hono.HONO_NR_CNPJ = cpfCNPJ;
        //                                hono.TIPE_CD_ID = 2;
        //                            }
        //                            Int32 volta3 = honoApp.ValidateCreate(hono, user);
        //                            prec.HONO_CD_ID = hono.HONO_CD_ID;
        //                            prec.PREC_VL_HON_VALOR_PRINCIPAL = Convert.ToDecimal(wsGeral.Cells[row, 22].Value.ToString());
        //                            prec.PREC_VL_HON_JUROS = Convert.ToDecimal(wsGeral.Cells[row, 23].Value.ToString());
        //                        }
        //                    }

        //                    if (wsGeral.Cells[row, 24].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 24].Value.ToString() == "Sim" || wsGeral.Cells[row, 24].Value.ToString() == "S")
        //                        {
        //                            prec.PREC_SG_HON_IR_RRA = "1";
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_SG_HON_IR_RRA = "0";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_SG_HON_IR_RRA = "0";
        //                    }

        //                    if (wsGeral.Cells[row, 25].Value != null)
        //                    {
        //                        String honoMes = wsGeral.Cells[row, 25].Value.ToString();
        //                        if (honoMes != null)
        //                        {
        //                            if (Regex.IsMatch(honoMes, @"\d"))
        //                            {
        //                                prec.PREC_IN_HON_MESES_EXE_ANTERIOR = Convert.ToInt32(wsGeral.Cells[row, 25].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para HONORARIO MES ANTERIOR" + honoMes;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_IN_HON_MESES_EXE_ANTERIOR = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 26].Value != null)
        //                    {
        //                        if (wsGeral.Cells[row, 26].Value.ToString() != null)
        //                        {
        //                            String datx = wsGeral.Cells[row, 26].Value.ToString();
        //                            DateTime outData = new DateTime();
        //                            if (DateTime.TryParse(datx, out outData))
        //                            {
        //                                prec.PREC_DT_HON_DATABASE = Convert.ToDateTime(wsGeral.Cells[row, 26].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 26].Value));
        //                                if (dat == DateTime.MinValue)
        //                                {
        //                                    prec.PREC_DT_HON_DATABASE = null;
        //                                }
        //                                else
        //                                {
        //                                    prec.PREC_DT_HON_DATABASE = dat;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_DT_HON_DATABASE = null;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_DT_HON_DATABASE = null;
        //                    }

        //                    if (wsGeral.Cells[row, 27].Value != null)
        //                    {
        //                        String valPSS = wsGeral.Cells[row, 27].Value.ToString();
        //                        if (valPSS != null)
        //                        {
        //                            if (Regex.IsMatch(valPSS, @"\d"))
        //                            {
        //                                prec.PREC_VL_HON_VALOR_INICIAL_PSS = Convert.ToDecimal(wsGeral.Cells[row, 27].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO INICIAL PSS" + valPSS;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_HON_VALOR_INICIAL_PSS = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 28].Value != null)
        //                    {
        //                        String honReq = wsGeral.Cells[row, 28].Value.ToString();
        //                        if (honReq != null)
        //                        {
        //                            if (Regex.IsMatch(honReq, @"\d"))
        //                            {
        //                                prec.PREC_VL_BEN_VALOR_REQUISITADO = Convert.ToDecimal(wsGeral.Cells[row, 28].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR HONORARIO REQUISITADO" + honReq;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_BEN_VALOR_REQUISITADO = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 35].Value != null)
        //                    {
        //                        String anoProp = wsGeral.Cells[row, 35].Value.ToString();
        //                        if (anoProp != null)
        //                        {
        //                            if (Regex.IsMatch(anoProp, @"\d{4}"))
        //                            {
        //                                prec.PREC_NR_ANO = anoProp;
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não inválido informado para ANO DA PROPOSTA " + anoProp;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                prec.PREC_NR_ANO = String.Empty;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_NR_ANO = String.Empty;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NR_ANO = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 36].Value != null)
        //                    {
        //                        prec.PREC_NM_TIPO_DESPESA = wsGeral.Cells[row, 36].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_TIPO_DESPESA = String.Empty;
        //                    }

        //                    if (wsGeral.Cells[row, 37].Value != null)
        //                    {
        //                        String valRRA = wsGeral.Cells[row, 37].Value.ToString();
        //                        if (valRRA != null)
        //                        {
        //                            if (Regex.IsMatch(valRRA, @"\d"))
        //                            {
        //                                prec.PREC_VL_RRA = Convert.ToDecimal(wsGeral.Cells[row, 37].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para VALOR RRA. Assumido 0. " + valRRA;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                prec.PREC_VL_RRA = 0;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_VL_RRA = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_VL_RRA = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 38].Value != null)
        //                    {
        //                        String percRRA = wsGeral.Cells[row, 38].Value.ToString();
        //                        if (percRRA != null)
        //                        {
        //                            if (Regex.IsMatch(percRRA, @"\d"))
        //                            {
        //                                prec.PREC_PC_RRA = Convert.ToDecimal(wsGeral.Cells[row, 38].Value.ToString());
        //                            }
        //                            else
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Valor não numérico informado para PERCENTAGEM RRA. Assumido 0. " + percRRA;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                                prec.PREC_PC_RRA = 0;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            prec.PREC_PC_RRA = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_PC_RRA = 0;
        //                    }

        //                    if (wsGeral.Cells[row, 40].Value != null)
        //                    {
        //                        prec.PREC_NM_PREFERENCIA = wsGeral.Cells[row, 40].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.PREC_NM_PREFERENCIA = String.Empty;
        //                    }

        //                    Int32 flagBene = 1;
        //                    String nome = null;
        //                    String cpf = null;
        //                    String sexo = null;
        //                    String nasc = null;
        //                    String cel1 = null;
        //                    String cel2 = null;

        //                    if ( wsGeral.Cells[row, 29].Value != null)
        //                    {
        //                        nome = wsGeral.Cells[row, 29].Value.ToString();
        //                    }
        //                    if (wsGeral.Cells[row, 30].Value != null)
        //                    {
        //                        cpf = wsGeral.Cells[row, 30].Value.ToString();
        //                    }
        //                    if (wsGeral.Cells[row, 31].Value != null)
        //                    {
        //                        sexo = wsGeral.Cells[row, 31].Value.ToString();
        //                    }
        //                    if (wsGeral.Cells[row, 32].Value != null)
        //                    {
        //                        nasc = wsGeral.Cells[row, 32].Value.ToString();
        //                    }
        //                    if (wsGeral.Cells[row, 33].Value != null)
        //                    {
        //                        cel1 = wsGeral.Cells[row, 33].Value.ToString();
        //                    }
        //                    if (wsGeral.Cells[row, 34].Value != null)
        //                    {
        //                        cel2 = wsGeral.Cells[row, 34].Value.ToString();
        //                    }

        //                    if (nome == null || cpf == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Beneficiário: Nome e CPF não informados";
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        flagBene = 0;
        //                    }

        //                    DateTime checaNasc;
        //                    DateTime? dataNasc = null;
        //                    if (nasc != null)
        //                    {
        //                        Boolean data = DateTime.TryParse(nasc, out checaNasc);
        //                        if (!data)
        //                        {
        //                            PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                            fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                            fal.PRFA_NM_PRECATORIO = numPrec;
        //                            fal.PRFA_DT_DATA = DateTime.Now;
        //                            fal.PRFA_DS_MOTIVO = "Data de nascimento inválida. Beneficiário: " + nome + ". " + nasc;
        //                            Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                            falha++;
        //                        }
        //                        String x = nasc.ToString();
        //                        dataNasc = Convert.ToDateTime(x);
        //                    }

        //                    if (flagBene == 1)
        //                    {
        //                        BENEFICIARIO beneNovo = new BENEFICIARIO();
        //                        BENEFICIARIO bene = listaBene.Where(p => p.BENE_NM_NOME == nome & p.BENE_NR_CPF == cpf).FirstOrDefault();
        //                        if (bene == null)
        //                        {
        //                            beneNovo.TIPE_CD_ID = 1;
        //                            if (sexo == "M" || sexo == "F")
        //                            {
        //                                beneNovo.SEXO_CD_ID = sexo == "M" ? 1 : 0;
        //                            }
        //                            beneNovo.BENE_NM_NOME = nome;
        //                            beneNovo.BENE_NR_CPF = cpf;
        //                            beneNovo.BENE_DT_CADASTRO = DateTime.Today.Date;
        //                            beneNovo.BENE_NR_CELULAR = cel1;
        //                            beneNovo.BENE_NR_CELULAR2 = cel2;
        //                            beneNovo.BENE_DT_NASCIMENTO = dataNasc;
        //                            beneNovo.BENE_IN_ATIVO = 1;
        //                            Int32 voltaBene = beneApp.ValidateCreate(beneNovo, user);
        //                            if (voltaBene > 0)
        //                            {
        //                                PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                                fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                                fal.PRFA_NM_PRECATORIO = numPrec;
        //                                fal.PRFA_DT_DATA = DateTime.Now;
        //                                fal.PRFA_DS_MOTIVO = "Erro na inclusão do beneficiário " + nome;
        //                                Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                                falha++;
        //                            }
        //                            prec.BENE_CD_ID = beneNovo.BENE_CD_ID;
        //                        }
        //                    }

        //                    CLIENTE cliNovo = new CLIENTE();
        //                    CLIENTE cli = listaCli.Where(p => p.CLIE_NM_NOME == nome & p.CLIE_NR_CPF == cpf).FirstOrDefault();
        //                    if (cli == null)
        //                    {
        //                        cliNovo.TIPE_CD_ID = 1;
        //                        if (sexo == "M" || sexo == "F")
        //                        {
        //                            cliNovo.SEXO_CD_ID = sexo == "M" ? 1 : 0;
        //                        }
        //                        cliNovo.CLIE_NM_NOME = nome;
        //                        cliNovo.CLIE_NR_CPF = cpf;
        //                        cliNovo.CLIE_DT_CADASTRO = DateTime.Today.Date;
        //                        cliNovo.CLIE_NR_CELULAR = cel1;
        //                        cliNovo.CLIE_NR_WHATSAPP = cel2;
        //                        cliNovo.CLIE_DT_NASCIMENTO = dataNasc;
        //                        cliNovo.CLIE_IN_ATIVO = 1;
        //                        cliNovo.CLIE_NM_EMAIL = String.Empty;
        //                        Int32 voltaCli = cliApp.ValidateCreate(cliNovo, user);
        //                        if (voltaCli > 0)
        //                        {
        //                            PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                            fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                            fal.PRFA_NM_PRECATORIO = numPrec;
        //                            fal.PRFA_DT_DATA = DateTime.Now;
        //                            fal.PRFA_DS_MOTIVO = "Erro na inclusão do cliente " + nome;
        //                            Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                            falha++;
        //                        }
        //                    }

        //                    // Processa usuário
        //                    Int32 flagUsu = 1;
        //                    String usu = null;
        //                    if (wsGeral.Cells[row, 39].Value != null)
        //                    {
        //                        usu = wsGeral.Cells[row, 39].Value.ToString();
        //                    }

        //                    if (usu == null)
        //                    {
        //                        PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                        fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                        fal.PRFA_NM_PRECATORIO = numPrec;
        //                        fal.PRFA_DT_DATA = DateTime.Now;
        //                        fal.PRFA_DS_MOTIVO = "Usuário responsável não definido. Precatório " + numPrec;
        //                        Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                        falha++;
        //                        flagUsu = 0;
        //                    }

        //                    if (flagUsu == 1)
        //                    {
        //                        usuCheca = usuApp.GetItemById(Convert.ToInt32(usu));
        //                        if (usuCheca == null)
        //                        {
        //                            PRECATORIO_FALHA fal = new PRECATORIO_FALHA();
        //                            fal.PRFA_NM_TRF = wsGeral.Cells[2, 1].Value.ToString();
        //                            fal.PRFA_NM_PRECATORIO = numPrec;
        //                            fal.PRFA_DT_DATA = DateTime.Now;
        //                            fal.PRFA_DS_MOTIVO = "Usuário responsável não cadastrado. Precatório " + numPrec;
        //                            Int32 volta2 = tranApp.ValidateCreateFalha(fal);
        //                            falha++;
        //                            prec.USUA_CD_ID = null;
        //                        }
        //                        else
        //                        {
        //                            prec.USUA_CD_ID = Convert.ToInt32(usu);
        //                        }
        //                    }

        //                    // Grava objeto
        //                    if (colTRF != null)
        //                    {
        //                        prec.PREC_NM_NOME = nome;
        //                        Int32 volta = tranApp.ValidateCreate(prec, user);
        //                        conta++;

        //                        // Cria pastas
        //                        String caminho = "/Imagens/1" + "/Precatorios/" + prec.PREC_CD_ID.ToString() + "/Anexos/";
        //                        Directory.CreateDirectory(Server.MapPath(caminho));
        //                    }
        //                    else
        //                    {
        //                        Session["Conta"] = conta;
        //                        Session["Falha"] = falha;
        //                        Session["MensPrecatorio"] = 10;
        //                        Session["ListaPrecatorio"] = null;
        //                        Session["VoltaPrecatorio"] = 0;
        //                        return RedirectToAction("MontarTelaPrecatorio");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    ModelState.AddModelError("", ex.Message);
        //                    return View();
        //                }
        //            }
        //        }

        //        // Processa Callix
        //        if (trf == 0)
        //        {
        //            for (int row = 2; row < wsFinalRow.Row; row++)
        //            {
        //                try
        //                {
        //                    Int32 check = 0;
        //                    CONTATO prec = new CONTATO();

        //                    // Verifica existencia
        //                    String prot = wsGeral.Cells[row, 4].Value.ToString();
        //                    prot = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(prot);
        //                    contCheca = tranApp.CheckExistContato(prot);
        //                    if (contCheca != null)
        //                    {
        //                        CONTATO_FALHA fal = new CONTATO_FALHA();
        //                        fal.COFA_NR_PROTOCOLO = wsGeral.Cells[2, 4].Value.ToString();
        //                        fal.COFA_NR_PRECATORIO = wsGeral.Cells[2, 3].Value.ToString();
        //                        fal.COFA_NM_CONTATO = wsGeral.Cells[2, 7].Value.ToString();
        //                        fal.COFA_NM_OPERADOR = wsGeral.Cells[2, 9].Value.ToString();
        //                        fal.COFA_DT_DATA = DateTime.Now;
        //                        String datx = wsGeral.Cells[row, 2].Value.ToString();
        //                        DateTime outData = new DateTime();
        //                        if (DateTime.TryParse(datx, out outData))
        //                        {
        //                            fal.COFA_DT_DATA_FALHA = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
        //                        }
        //                        else
        //                        {
        //                            DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
        //                            if (dat == DateTime.MinValue)
        //                            {
        //                                fal.COFA_DT_DATA_FALHA = null;
        //                            }
        //                            else
        //                            {
        //                                fal.COFA_DT_DATA_FALHA = dat;
        //                            }
        //                        }
        //                        fal.COFA_DS_MOTIVO = "Contato já incluído para " + wsGeral.Cells[2, 4].Value.ToString();
        //                        Int32 volta2 = tranApp.ValidateCreateFalhaContato(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    // Verifica precatorio
        //                    String numPrec = wsGeral.Cells[row, 3].Value.ToString();
        //                    numPrec = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(numPrec);
        //                    precCheca = tranApp.CheckExist(numPrec);
        //                    if (precCheca == null)
        //                    {
        //                        CONTATO_FALHA fal = new CONTATO_FALHA();
        //                        fal.COFA_NR_PROTOCOLO = wsGeral.Cells[2, 4].Value.ToString();
        //                        fal.COFA_NR_PRECATORIO = wsGeral.Cells[2, 3].Value.ToString();
        //                        fal.COFA_NM_CONTATO = wsGeral.Cells[2, 7].Value.ToString();
        //                        fal.COFA_NM_OPERADOR = wsGeral.Cells[2, 9].Value.ToString();
        //                        fal.COFA_DT_DATA = DateTime.Now;
        //                        String datx = wsGeral.Cells[row, 2].Value.ToString();
        //                        DateTime outData = new DateTime();
        //                        if (DateTime.TryParse(datx, out outData))
        //                        {
        //                            fal.COFA_DT_DATA_FALHA = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
        //                        }
        //                        else
        //                        {
        //                            DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
        //                            if (dat == DateTime.MinValue)
        //                            {
        //                                fal.COFA_DT_DATA_FALHA = null;
        //                            }
        //                            else
        //                            {
        //                                fal.COFA_DT_DATA_FALHA = dat;
        //                            }
        //                        }
        //                        fal.COFA_DS_MOTIVO = "Precatório inexistente para " + wsGeral.Cells[2, 4].Value.ToString();
        //                        Int32 volta2 = tranApp.ValidateCreateFalhaContato(fal);
        //                        falha++;
        //                        continue;
        //                    }

        //                    // Verifica beneficiario
        //                    //String nome = wsGeral.Cells[row, 7].Value.ToString();
        //                    //String cpf = wsGeral.Cells[row, 19].Value.ToString();
        //                    //beneCheca = beneApp.CheckExist(nome, cpf);
        //                    //if (beneCheca == null)
        //                    //{
        //                    //    CONTATO_FALHA fal = new CONTATO_FALHA();
        //                    //    fal.COFA_NR_PROTOCOLO = wsGeral.Cells[2, 4].Value.ToString();
        //                    //    fal.COFA_NR_PRECATORIO = wsGeral.Cells[2, 3].Value.ToString();
        //                    //    fal.COFA_NM_CONTATO = wsGeral.Cells[2, 7].Value.ToString();
        //                    //    fal.COFA_NM_OPERADOR = wsGeral.Cells[2, 9].Value.ToString();
        //                    //    fal.COFA_DT_DATA = DateTime.Now;
        //                    //    String datx = wsGeral.Cells[row, 2].Value.ToString();
        //                    //    DateTime outData = new DateTime();
        //                    //    if (DateTime.TryParse(datx, out outData))
        //                    //    {
        //                    //        fal.COFA_DT_DATA_FALHA = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
        //                    //        if (dat == DateTime.MinValue)
        //                    //        {
        //                    //            fal.COFA_DT_DATA_FALHA = null;
        //                    //        }
        //                    //        else
        //                    //        {
        //                    //            fal.COFA_DT_DATA_FALHA = dat;
        //                    //        }
        //                    //    }
        //                    //    fal.COFA_DS_MOTIVO = "Beneficiário inexistente para " + wsGeral.Cells[2, 4].Value.ToString();
        //                    //    Int32 volta2 = tranApp.ValidateCreateFalhaContato(fal);
        //                    //    falha++;
        //                    //    continue;
        //                    //}

        //                    // Monta objeto
        //                    if (wsGeral.Cells[row, 2].Value.ToString() != null)
        //                    {
        //                        String datx = wsGeral.Cells[row, 2].Value.ToString();
        //                        DateTime outData = new DateTime();
        //                        if (DateTime.TryParse(datx, out outData))
        //                        {
        //                            prec.CONT_DT_CONTATO = Convert.ToDateTime(wsGeral.Cells[row, 2].Value.ToString());
        //                        }
        //                        else
        //                        {
        //                            DateTime dat = DateTime.FromOADate(Convert.ToDouble(wsGeral.Cells[row, 2].Value));
        //                            if (dat == DateTime.MinValue)
        //                            {
        //                                prec.CONT_DT_CONTATO = null;
        //                            }
        //                            else
        //                            {
        //                                prec.CONT_DT_CONTATO = dat;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        prec.CONT_DT_CONTATO = null;
        //                    }

        //                    String precNum = wsGeral.Cells[row, 3].Value.ToString();
        //                    precCheca = tranApp.CheckExist(precNum);
        //                    if (precCheca != null)
        //                    {
        //                        prec.PREC_CD_ID = precCheca.PREC_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        PRECATORIO bene = new PRECATORIO();
        //                        bene.PREC_DT_CADASTRO = DateTime.Today.Date;
        //                        bene.PREC_IN_ATIVO = 1;
        //                        bene.PREC_NM_PRECATORIO = wsGeral.Cells[row, 3].Value.ToString();
        //                        bene.TRF1_CD_ID = 1;
        //                        Int32 volta3 = tranApp.ValidateCreate(bene, user);
        //                        prec.PREC_CD_ID = bene.PREC_CD_ID;
        //                    }

        //                    prec.CONT_NR_PROTOCOLO = wsGeral.Cells[row, 4].Value.ToString();
        //                    prec.CONT_NM_CAMPANHA = wsGeral.Cells[row, 5].Value.ToString();
        //                    prec.CONT_NM_LISTA = wsGeral.Cells[row, 6].Value.ToString();

        //                    String beneNome = wsGeral.Cells[row, 7].Value.ToString();
        //                    String beneCPF = wsGeral.Cells[row, 19].Value.ToString();
        //                    beneCheca = beneApp.CheckExist(beneNome, beneCPF);
        //                    if (beneCheca != null)
        //                    {
        //                        prec.BENE_CD_ID = beneCheca.BENE_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        BENEFICIARIO bene = new BENEFICIARIO();
        //                        bene.BENE_DT_CADASTRO = DateTime.Today.Date;
        //                        bene.BENE_IN_ATIVO = 1;
        //                        bene.BENE_NM_NOME = wsGeral.Cells[row, 7].Value.ToString();
        //                        bene.BENE_NR_CPF = wsGeral.Cells[row, 19].Value.ToString();
        //                        bene.TIPE_CD_ID = 1;
        //                        Int32 volta3 = beneApp.ValidateCreate(bene, user);
        //                        prec.BENE_CD_ID = bene.BENE_CD_ID;

        //                        // Cria pastas
        //                        String caminho1 = "/Imagens/1" + "/Beneficiarios/" + prec.BENE_CD_ID.ToString() + "/Anexos/";
        //                        Directory.CreateDirectory(Server.MapPath(caminho1));
        //                    }

        //                    prec.CONT_NR_TELEFONE = wsGeral.Cells[row, 8].Value.ToString();
        //                    prec.CONT_NM_AGENTE = wsGeral.Cells[row, 9].Value.ToString();

        //                    String quali = wsGeral.Cells[row, 10].Value.ToString();
        //                    qualiCheca = conApp.CheckExistQualificacao(quali);
        //                    if (qualiCheca != null)
        //                    {
        //                        prec.QUAL_CD_ID = qualiCheca.QUAL_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        QUALIFICACAO qualif = new QUALIFICACAO();
        //                        qualif.QUAL_IN_ATIVO = 1;
        //                        qualif.QUAL_NM_NOME = wsGeral.Cells[row, 10].Value.ToString();
        //                        Int32 volta3 = conApp.ValidateCreateQualificacao(qualif);
        //                        prec.QUAL_CD_ID = qualif.QUAL_CD_ID;
        //                    }

        //                    String quem = wsGeral.Cells[row, 11].Value.ToString();
        //                    quemCheca = conApp.CheckExistDesligamento(quali);
        //                    if (quemCheca != null)
        //                    {
        //                        prec.QUDE_CD_ID = quemCheca.QUDE_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        QUEM_DESLIGOU desl = new QUEM_DESLIGOU();
        //                        desl.QUDE_IN_ATIVO = 1;
        //                        desl.QUDE_NM_NOME = wsGeral.Cells[row, 11].Value.ToString();
        //                        Int32 volta3 = conApp.ValidateCreateDesligamento(desl);
        //                        prec.QUDE_CD_ID = desl.QUDE_CD_ID;
        //                    }

        //                    prec.CONT_TM_DURACAO = wsGeral.Cells[row, 13].Value.ToString();
        //                    prec.CONT_TM_DURACAO_ATENDIMENTO = wsGeral.Cells[row, 14].Value.ToString();

        //                    String audio = wsGeral.Cells[row, 15].Value.ToString();
        //                    prec.CONT_AQ_AUDIO = "/Audios/Ligacoes/" + audio;
        //                    String arquivo = caminhoAudio + "/" + audio;
        //                    String caminho = "/Audios/Ligacoes/";
        //                    String path = Path.Combine(Server.MapPath(caminho), audio);
        //                    if (System.IO.File.Exists(arquivo))
        //                    {
        //                        System.IO.File.Copy(arquivo, path);
        //                    }

        //                    prec.CONT_NM_ASSUNTO = wsGeral.Cells[row, 16].Value.ToString();
        //                    if (wsGeral.Cells[row, 17].Value != null)
        //                    {
        //                        prec.CONT_NR_CELULAR_1 = wsGeral.Cells[row, 17].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.CONT_NR_CELULAR_1 = "-";
        //                    }
        //                    if (wsGeral.Cells[row, 18].Value != null)
        //                    {
        //                        prec.CONT_NR_CELULAR_2 = wsGeral.Cells[row, 18].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.CONT_NR_CELULAR_2 = "-";
        //                    }
        //                    if (wsGeral.Cells[row, 22].Value != null)
        //                    {
        //                        prec.CONT_NR_TELEFONE_1 = wsGeral.Cells[row, 22].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.CONT_NR_TELEFONE_1 = "-";
        //                    }
        //                    if (wsGeral.Cells[row, 23].Value != null)
        //                    {
        //                        prec.CONT_NR_TELEFONE_2 = wsGeral.Cells[row, 23].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.CONT_NR_TELEFONE_2 = "-";
        //                    }
        //                    if (wsGeral.Cells[row, 25].Value != null)
        //                    {
        //                        prec.CONT_VL_RIDOLFI = wsGeral.Cells[row, 25].Value.ToString();
        //                    }
        //                    else
        //                    {
        //                        prec.CONT_VL_RIDOLFI = "-";
        //                    }

        //                    // Grava objeto
        //                    Int32 volta = conApp.ValidateCreate(prec, user);
        //                    conta++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    ModelState.AddModelError("", ex.Message);
        //                    return View();
        //                }
        //            }
        //        }
        //    }

        //    // Finaliza
        //    if (trf != 0)
        //    {
        //        Session["Conta"] = conta;
        //        Session["Falha"] = falha;
        //        Session["MensPrecatorio"] = 10;
        //        Session["ListaPrecatorio"] = null;
        //        Session["VoltaPrecatorio"] = 0;
        //        return RedirectToAction("MontarTelaPrecatorio");
        //    }
        //    else
        //    {
        //        Session["Conta"] = conta;
        //        Session["Falha"] = falha;
        //        Session["MensPrecatorio"] = 11;
        //        Session["ListaContato"] = null;
        //        Session["VoltaContato"] = 0;
        //        return RedirectToAction("MontarTelaContato");
        //    }
        //}

        [HttpGet]
        public ActionResult OuvirAudio()
        {
            return View();
        }

        public DateTime ConverteData(String data)
        {
            String dia = data.Substring(0, 2);
            String mes = data.Substring(3, 3);
            String ano = data.Substring(7, 4);
            String mesCerto = null;

            if (mes == "jan")
            {
                mesCerto = "01";

            }
            if (mes == "fev")
            {
                mesCerto = "02";

            }
            if (mes == "mar")
            {
                mesCerto = "03";

            }
            if (mes == "abr")
            {
                mesCerto = "04";

            }
            if (mes == "mai")
            {
                mesCerto = "05";

            }
            if (mes == "jun")
            {
                mesCerto = "06";

            }
            if (mes == "jul")
            {
                mesCerto = "07";

            }
            if (mes == "ago")
            {
                mesCerto = "08";

            }
            if (mes == "set")
            {
                mesCerto = "09";

            }
            if (mes == "out")
            {
                mesCerto = "10";

            }
            if (mes == "nov")
            {
                mesCerto = "11";

            }
            if (mes == "dez")
            {
                mesCerto = "12";

            }

            String dataCerta = dia + "/" + mesCerto + "/" + ano;
            DateTime dataVolta = Convert.ToDateTime(dataCerta); 
            return dataVolta;
        }

        [HttpGet]
        public ActionResult MontarTelaContato()
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
            if ((List<CONTATO>)Session["ListaContato"] == null || ((List<CONTATO>)Session["ListaContato"]).Count == 0)
            {
                listaMasterContato = CarregaContato();
                Session["ListaContato"] = listaMasterContato;
            }

            ViewBag.Listas = (List<CONTATO>)Session["ListaContato"];
            ViewBag.Title = "Contato";
            ViewBag.Precatorio = new SelectList(CarregaPrecatorio().OrderBy(p => p.PREC_NM_PRECATORIO), "PREC_CD_ID", "PREC_NM_PRECATORIO");
            ViewBag.Qualificacao = new SelectList(CarregaQualificacao().OrderBy(p => p.QUAL_NM_NOME), "QUAL_CD_ID", "QUAL_NM_NOME");
            ViewBag.Desligamento = new SelectList(CarregaDesligamento().OrderBy(p => p.QUDE_NM_NOME), "QUDE_CD_ID", "QUDE_NM_NOME");
            ViewBag.Beneficiario = new SelectList(CarregaBeneficiario().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensPrecatorio"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPrecatorio"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPrecatorio"] == 2)  
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0145", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPrecatorio"] == 11)
                {
                    ModelState.AddModelError("", "Foram processados e incluídos " + ((Int32)Session["Conta"]).ToString() + " contatos telefônicos");
                    ModelState.AddModelError("", "Foram processados e rejeitados " + ((Int32)Session["Falha"]).ToString() + " contatos telefônicos");
                }
            }

            // Abre view
            objetoContato = new CONTATO();
            objetoContato.CONT_DT_CONTATO = DateTime.Today.Date.AddDays(-30);
            objetoContato.CONT_DT_FINAL = DateTime.Today.Date;
            Session["MensPrecatorio"] = 0;
            Session["VoltaPrecatorio"] = 2;
            if (Session["FiltroContato"] != null)
            {
                objetoContato = (CONTATO)Session["FiltroContato"];
            }
            return View(objetoContato);
        }

        public ActionResult RetirarFiltroContato()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaContato"] = null;
            Session["FiltroContato"] = null;
            return RedirectToAction("MontarTelaContato");
        }

        public ActionResult MostrarTudoContato()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMasterContato = conApp.GetAllItensAdm();
            Session["FiltroContato"] = null;
            Session["ListaContato"] = listaMasterContato;
            return RedirectToAction("MontarTelaContato");
        }

        [HttpPost]
        public ActionResult FiltrarContato(CONTATO item)
        {            
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                // Executa a operação
                List<CONTATO> listaObj = new List<CONTATO>();
                Session["FiltroContato"] = item;
                Int32 volta = conApp.ExecuteFilter(item.PREC_CD_ID, item.BENE_CD_ID, item.QUAL_CD_ID, item.QUDE_CD_ID, item.CONT_NR_PROTOCOLO, item.CONT_DT_CONTATO, item.CONT_DT_FINAL, item.CONT_NR_TELEFONE, item.CONT_NM_AGENTE, item.CONT_NM_CAMPANHA, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensPrecatorio"] = 1;
                }

                // Sucesso
                listaMasterContato = listaObj;
                Session["ListaContato"] = listaObj;
                return RedirectToAction("MontarTelaContato");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaContato");
            }
        }

        public ActionResult VoltarBaseContato()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaContato");
        }

        [HttpGet]
        public ActionResult VerContato(Int32 id)
        {

            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CONTATO item = conApp.GetItemById(id);
            objetoContatoAntes = item;
            Session["Contato"] = item;
            Session["IdContatoPrecatorio"] = id;
            return View(item);
        }

       public ActionResult GerarRelatorioListaContato()
        {            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ContatoLista" + "_" + data + ".pdf";
            List<CONTATO> lista = (List<CONTATO>)Session["ListaContato"];
            CONTATO filtro = (CONTATO)Session["FiltroContato"];
            iTextSharp.text.Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            iTextSharp.text.Paragraph line = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/Base/LogoRidolfi.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Contatos Telefônicos - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            iTextSharp.text.Paragraph line1 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.GREEN, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new iTextSharp.text.Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 50f, 60f, 100f, 80f, 70f, 50f, 50f, 80f, 100f, 100f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Contatos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 11;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new iTextSharp.text.Paragraph("Data", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Precatório", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Beneficiário", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Qualificação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Quem Desligou", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Protocolo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Telefone", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Agente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Campanha", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new iTextSharp.text.Paragraph("Assunto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTATO item in lista)
            {
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.CONT_DT_CONTATO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.PRECATORIO.PREC_NM_PRECATORIO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.BENEFICIARIO.BENE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.QUALIFICACAO.QUAL_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.QUEM_DESLIGOU.QUDE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.CONT_NR_PROTOCOLO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.CONT_NR_TELEFONE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.CONT_NM_AGENTE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.CONT_NM_CAMPANHA, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new iTextSharp.text.Paragraph(item.CONT_NM_ASSUNTO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            iTextSharp.text.Paragraph line2 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.PREC_CD_ID != null)
                {
                    parametros += "Precatório: " + filtro.PREC_CD_ID;
                    ja = 1;
                }
                if (filtro.BENE_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Beneficiário: " + filtro.BENE_CD_ID;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Beneficiário: " + filtro.BENE_CD_ID;
                    }
                }
                if (filtro.QUAL_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Qualificação: " + filtro.QUAL_CD_ID;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Qualificação: " + filtro.QUAL_CD_ID;
                    }
                }
                if (filtro.QUDE_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Quem Desligou: " + filtro.QUDE_CD_ID;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Quem Desligou: " + filtro.QUDE_CD_ID;
                    }
                }
                if (filtro.CONT_NR_PROTOCOLO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Protocolo: " + filtro.CONT_NR_PROTOCOLO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Protocolo: " + filtro.CONT_NR_PROTOCOLO;
                    }
                }
                if (filtro.CONT_DT_CONTATO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data Inicial: " + filtro.CONT_DT_CONTATO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data Inicial: " + filtro.CONT_DT_CONTATO;
                    }
                }
                if (filtro.CONT_DT_FINAL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data Final: " + filtro.CONT_DT_FINAL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data Final: " + filtro.CONT_DT_FINAL;
                    }
                }
                if (filtro.CONT_NR_TELEFONE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Telefone: " + filtro.CONT_NR_TELEFONE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Telefone: " + filtro.CONT_NR_TELEFONE;
                    }
                }
                if (filtro.CONT_NM_AGENTE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Agente: " + filtro.CONT_NM_AGENTE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Agente: " + filtro.CONT_NM_AGENTE;
                    }
                }
                if (filtro.CONT_NM_CAMPANHA != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Campanha: " + filtro.CONT_NM_CAMPANHA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Campanha: " + filtro.CONT_NM_CAMPANHA;
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
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            iTextSharp.text.Paragraph line3 = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
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

            return RedirectToAction("MontarTelaContato");
        }

        [HttpGet]
        public ActionResult MontarTelaFiltroGenericoPrecatorio()
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
            if ((List<PRECATORIO>)Session["ListaPrecatorioGeral"] == null || ((List<PRECATORIO>)Session["ListaPrecatorioGeral"]).Count == 0)
            {
                listaMasterTran = tranApp.GetAllItens();
                Session["ListaPrecatorioGeral"] = listaMasterTran;
            }
            ViewBag.Listas = (List<PRECATORIO>)Session["ListaPrecatorioGeral"];
            ViewBag.Title = "Precatorio";
            ViewBag.TRFs = new SelectList(tranApp.GetAllTRF().OrderBy(p => p.TRF1_NM_NOME), "TRF1_CD_ID", "TRF1_NM_NOME");
            ViewBag.Benefs = new SelectList(tranApp.GetAllBeneficiarios().OrderBy(p => p.BENE_NM_NOME), "BENE_CD_ID", "BENE_NM_NOME");
            ViewBag.Honos = new SelectList(tranApp.GetAllAdvogados().OrderBy(p => p.HONO_NM_NOME), "HONO_CD_ID", "HONO_NM_NOME");
            ViewBag.Natus = new SelectList(tranApp.GetAllNaturezas().OrderBy(p => p.NATU_NM_NOME), "NATU_CD_ID", "NATU_NM_NOME");
            ViewBag.Pres = new SelectList(tranApp.GetAllEstados().OrderBy(p => p.PRES_NM_NOME), "PRES_CD_ID", "PRES_NM_NOME");
            Session["Precatorio"] = null;

            List<SelectListItem> SigProc = new List<SelectListItem>();
            SigProc.Add(new SelectListItem() { Text = "PRC", Value = "1" });
            SigProc.Add(new SelectListItem() { Text = "RPV", Value = "2" });
            ViewBag.SigProc = new SelectList(SigProc, "Value", "Text");

            List<SelectListItem> IRBen = new List<SelectListItem>();
            IRBen.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            IRBen.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.IRBeneficiario = new SelectList(IRBen, "Value", "Text");           
            List<SelectListItem> pesq = new List<SelectListItem>();
            pesq.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pesq.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Pesquisado = new SelectList(pesq, "Value", "Text");
            List<SelectListItem> crm = new List<SelectListItem>();
            crm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            crm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CRM = new SelectList(crm, "Value", "Text");
            List<SelectListItem> sit = new List<SelectListItem>();
            sit.Add(new SelectListItem() { Text = "Pago", Value = "1" });
            sit.Add(new SelectListItem() { Text = "Não Pago", Value = "2" });
            sit.Add(new SelectListItem() { Text = "Pago Parcial", Value = "3" });
            ViewBag.Situacao = new SelectList(sit, "Value", "Text");
            List<SelectListItem> bloc = new List<SelectListItem>();
            bloc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            bloc.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Bloqueada = new SelectList(bloc, "Value", "Text");
            List<SelectListItem> loa = new List<SelectListItem>();
            loa.Add(new SelectListItem() { Text = "Bot", Value = "1" });
            loa.Add(new SelectListItem() { Text = "LOA", Value = "2" });
            ViewBag.LOA = new SelectList(loa, "Value", "Text");

            // Mensagens
            if (Session["MensPrecatorio"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPrecatorio"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
            }

            // Carrega Operadores
            List<SelectListItem> operaTRF= new List<SelectListItem>();
            operaTRF.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaTRF.Add(new SelectListItem() { Text = "Diferente", Value = "2" });
            ViewBag.OperaTRF = new SelectList(operaTRF, "Value", "Text");
            
            List<SelectListItem> operaBene = new List<SelectListItem>();
            operaBene.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaBene.Add(new SelectListItem() { Text = "Diferente", Value = "2" });
            ViewBag.OperaBene = new SelectList(operaBene, "Value", "Text");

            List<SelectListItem> operaAdv = new List<SelectListItem>();
            operaAdv.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaAdv.Add(new SelectListItem() { Text = "Diferente", Value = "2" });
            ViewBag.OperaAdv = new SelectList(operaAdv, "Value", "Text");

            List<SelectListItem> operaNat = new List<SelectListItem>();
            operaNat.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaNat.Add(new SelectListItem() { Text = "Diferente", Value = "2" });
            ViewBag.OperaNat = new SelectList(operaNat, "Value", "Text");

            List<SelectListItem> operaPres = new List<SelectListItem>();
            operaPres.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaPres.Add(new SelectListItem() { Text = "Diferente", Value = "2" });
            ViewBag.OperaPres = new SelectList(operaPres, "Value", "Text");

            List<SelectListItem> operaPrec = new List<SelectListItem>();
            operaPrec.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaPrec.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaPrec = new SelectList(operaPrec, "Value", "Text");

            List<SelectListItem> operaAno = new List<SelectListItem>();
            operaAno.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaAno.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaAno = new SelectList(operaAno, "Value", "Text");

            List<SelectListItem> operaProcOrig = new List<SelectListItem>();
            operaProcOrig.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaProcOrig.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaProcOrig = new SelectList(operaProcOrig, "Value", "Text");

            List<SelectListItem> operaReq = new List<SelectListItem>();
            operaReq.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaReq.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaReq = new SelectList(operaReq, "Value", "Text");

            List<SelectListItem> operaDepr = new List<SelectListItem>();
            operaDepr.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaDepr.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaDepr = new SelectList(operaDepr, "Value", "Text");

            List<SelectListItem> operaAss = new List<SelectListItem>();
            operaAss.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaAss.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaAss = new SelectList(operaAss, "Value", "Text");

            List<SelectListItem> operaReqd = new List<SelectListItem>();
            operaReqd.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaReqd.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaReqd = new SelectList(operaReqd, "Value", "Text");

            List<SelectListItem> operaSgProc = new List<SelectListItem>();
            operaSgProc.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaSgProc = new SelectList(operaSgProc, "Value", "Text");

            List<SelectListItem> operaSitReq = new List<SelectListItem>();
            operaSitReq.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaSitReq.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaSitReq = new SelectList(operaSitReq, "Value", "Text");

            List<SelectListItem> operaValorInsProp = new List<SelectListItem>();
            operaValorInsProp.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaValorInsProp = new SelectList(operaValorInsProp, "Value", "Text");

            List<SelectListItem> operaProcExec = new List<SelectListItem>();
            operaProcExec.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaProcExec.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaProcExec = new SelectList(operaProcExec, "Value", "Text");

            List<SelectListItem> operaBenData = new List<SelectListItem>();
            operaBenData.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaBenData = new SelectList(operaBenData, "Value", "Text");

            List<SelectListItem> operaValorPrin = new List<SelectListItem>();
            operaValorPrin.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaValorPrin = new SelectList(operaValorPrin, "Value", "Text");

            List<SelectListItem> operaJuros = new List<SelectListItem>();
            operaJuros.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaJuros = new SelectList(operaJuros, "Value", "Text");

            List<SelectListItem> operaValorIniPSS = new List<SelectListItem>();
            operaValorIniPSS.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaValorIniPSS = new SelectList(operaValorIniPSS, "Value", "Text");

            List<SelectListItem> operaBenValorReq = new List<SelectListItem>();
            operaBenValorReq.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaBenValorReq = new SelectList(operaBenValorReq, "Value", "Text");

            List<SelectListItem> operaPesq = new List<SelectListItem>();
            operaPesq.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaPesq = new SelectList(operaPesq, "Value", "Text");

            List<SelectListItem> operaSitAtual = new List<SelectListItem>();
            operaSitAtual.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaSitAtual = new SelectList(operaSitAtual, "Value", "Text");

            List<SelectListItem> operaImpCRM = new List<SelectListItem>();
            operaImpCRM.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaImpCRM = new SelectList(operaImpCRM, "Value", "Text");

            List<SelectListItem> operaDataProtTRF = new List<SelectListItem>();
            operaDataProtTRF.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaDataProtTRF = new SelectList(operaDataProtTRF, "Value", "Text");

            List<SelectListItem> operaOfiReq = new List<SelectListItem>();
            operaOfiReq.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaOfiReq.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaOfiReq = new SelectList(operaOfiReq, "Value", "Text");

            List<SelectListItem> operaReqBloq = new List<SelectListItem>();
            operaReqBloq.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaReqBloq.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaReqBloq = new SelectList(operaReqBloq, "Value", "Text");

            List<SelectListItem> operaDataExp = new List<SelectListItem>();
            operaDataExp.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaDataExp.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaDataExp = new SelectList(operaDataExp, "Value", "Text");

            List<SelectListItem> operaNomLOA = new List<SelectListItem>();
            operaNomLOA.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaNomLOA.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaNomLOA = new SelectList(operaNomLOA, "Value", "Text");

            // Abre view
            Session["MensPrecatorio"] = 0;
            Session["VoltaPrecatorio"] = 1;
            Session["VoltaPrecatorioCRM"] = 0;
            Session["VoltaCRM"] = 0;
            PrecatorioViewModel vm = new PrecatorioViewModel();
            return View(vm);
        }

        public ActionResult RetirarFiltroGenerico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaClienteGeral"] = null;
            return RedirectToAction("MontarTelaFiltroGenericoCliente");
        }

        public ActionResult FiltrarGenerico(PrecatorioViewModel item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Inicia a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();
                List<PRECATORIO> lista = new List<PRECATORIO>();
                IQueryable<PRECATORIO> query = Db.PRECATORIO;

                // TRF
                if (item.TRF1_CD_ID != null & item.TRF1_CD_ID > 0)
                {
                    if (item.OperaTRF != null & item.OperaTRF > 0)
                    {
                        if (item.OperaTRF == 1)
                        {
                            query = query.Where(p => p.TRF1_CD_ID == item.TRF1_CD_ID);
                        }
                        if (item.OperaTRF == 2)
                        {
                            query = query.Where(p => p.TRF1_CD_ID != item.TRF1_CD_ID);
                        }
                    }
                }

                // Beneficiário
                if (item.BENE_CD_ID != null & item.BENE_CD_ID > 0)
                {
                    if (item.OperaBene != null & item.OperaBene > 0)
                    {
                        if (item.OperaBene == 1)
                        {
                            query = query.Where(p => p.BENE_CD_ID == item.BENE_CD_ID);
                        }
                        if (item.OperaBene == 2)
                        {
                            query = query.Where(p => p.BENE_CD_ID != item.BENE_CD_ID);
                        }
                    }
                }

                // Advogado
                if (item.HONO_CD_ID != null & item.HONO_CD_ID > 0)
                {
                    if (item.OperaAdv != null & item.OperaAdv > 0)
                    {
                        if (item.OperaAdv == 1)
                        {
                            query = query.Where(p => p.HONO_CD_ID == item.HONO_CD_ID);
                        }
                        if (item.OperaAdv == 2)
                        {
                            query = query.Where(p => p.HONO_CD_ID != item.HONO_CD_ID);
                        }
                    }
                }

                // Natureza
                if (item.NATU_CD_ID != null & item.NATU_CD_ID > 0)
                {
                    if (item.OperaNat != null & item.OperaNat > 0)
                    {
                        if (item.OperaNat == 1)
                        {
                            query = query.Where(p => p.NATU_CD_ID == item.NATU_CD_ID);
                        }
                        if (item.OperaNat == 2)
                        {
                            query = query.Where(p => p.NATU_CD_ID != item.NATU_CD_ID);
                        }
                    }
                }

                // Estado
                if (item.PRES_CD_ID != null & item.PRES_CD_ID > 0)
                {
                    if (item.OperaPres != null & item.OperaPres > 0)
                    {
                        if (item.OperaPres == 1)
                        {
                            query = query.Where(p => p.PRES_CD_ID == item.PRES_CD_ID);
                        }
                        if (item.OperaPres == 2)
                        {
                            query = query.Where(p => p.PRES_CD_ID != item.PRES_CD_ID);
                        }
                    }
                }

                // Precatorio
                if (item.PREC_NM_PRECATORIO != null)
                {
                    if (item.OperaPrec != null & item.OperaPrec > 0)
                    {
                        if (item.OperaPrec == 1)
                        {
                            query = query.Where(p => p.PREC_NM_PRECATORIO == item.PREC_NM_PRECATORIO);
                        }
                        if (item.OperaPrec == 3)
                        {
                            query = query.Where(p => p.PREC_NM_PRECATORIO.Contains(item.PREC_NM_PRECATORIO));
                        }
                    }
                }

                // Ano
                if (item.PREC_NR_ANO != null & item.ANO_FINAL == null)
                {
                    query = query.Where(p => Convert.ToInt32(p.PREC_NR_ANO) >= Convert.ToInt32(item.PREC_NR_ANO));
                }
                if (item.PREC_NR_ANO == null & item.ANO_FINAL != null)
                {
                    query = query.Where(p => Convert.ToInt32(p.PREC_NR_ANO) <= Convert.ToInt32(item.PREC_NR_ANO));
                }
                if (item.PREC_NR_ANO != null & item.ANO_FINAL != null)
                {
                    query = query.Where(p => Convert.ToInt32(p.PREC_NR_ANO) >= Convert.ToInt32(item.PREC_NR_ANO) & Convert.ToInt32(p.PREC_NR_ANO) <= Convert.ToInt32(item.ANO_FINAL));
                }

                // Processo Origem
                if (item.PREC_NM_PROCESSO_ORIGEM != null)
                {
                    if (item.OperaProcOrig != null & item.OperaProcOrig > 0)
                    {
                        if (item.OperaProcOrig == 1)
                        {
                            query = query.Where(p => p.PREC_NM_PROCESSO_ORIGEM == item.PREC_NM_PROCESSO_ORIGEM);
                        }
                        if (item.OperaProcOrig == 3)
                        {
                            query = query.Where(p => p.PREC_NM_PROCESSO_ORIGEM.Contains(item.PREC_NM_PROCESSO_ORIGEM));
                        }
                    }
                }

                // Requerente
                if (item.PREC_NM_REQUERENTE != null)
                {
                    if (item.OperaReq != null & item.OperaReq > 0)
                    {
                        if (item.OperaReq == 1)
                        {
                            query = query.Where(p => p.PREC_NM_REQUERENTE == item.PREC_NM_REQUERENTE);
                        }
                        if (item.OperaReq == 3)
                        {
                            query = query.Where(p => p.PREC_NM_REQUERENTE.Contains(item.PREC_NM_REQUERENTE));
                        }
                    }
                }

                // Deprecante
                if (item.PREC_NM_DEPRECANTE != null)
                {
                    if (item.OperaDepr != null & item.OperaDepr > 0)
                    {
                        if (item.OperaDepr == 1)
                        {
                            query = query.Where(p => p.PREC_NM_DEPRECANTE == item.PREC_NM_DEPRECANTE);
                        }
                        if (item.OperaDepr == 3)
                        {
                            query = query.Where(p => p.PREC_NM_DEPRECANTE.Contains(item.PREC_NM_DEPRECANTE));
                        }
                    }
                }

                // Assunto
                if (item.PREC_NM_ASSUNTO != null)
                {
                    if (item.OperaAss != null & item.OperaAss > 0)
                    {
                        if (item.OperaAss == 1)
                        {
                            query = query.Where(p => p.PREC_NM_ASSUNTO == item.PREC_NM_ASSUNTO);
                        }
                        if (item.OperaAss == 3)
                        {
                            query = query.Where(p => p.PREC_NM_ASSUNTO.Contains(item.PREC_NM_ASSUNTO));
                        }
                    }
                }

                // Requerido
                if (item.PREC_NM_REQUERIDO != null)
                {
                    if (item.OperaReqd != null & item.OperaReqd > 0)
                    {
                        if (item.OperaReqd == 1)
                        {
                            query = query.Where(p => p.PREC_NM_REQUERIDO == item.PREC_NM_REQUERIDO);
                        }
                        if (item.OperaReqd == 3)
                        {
                            query = query.Where(p => p.PREC_NM_REQUERIDO.Contains(item.PREC_NM_REQUERIDO));
                        }
                    }
                }

                // Sigla do Procedimento
                if (item.PREC_SG_PROCEDIMENTO != null)
                {
                    if (item.OperaSgProc != null & item.OperaSgProc > 0)
                    {
                        if (item.OperaSgProc == 1)
                        {
                            query = query.Where(p => p.PREC_SG_PROCEDIMENTO == item.PREC_SG_PROCEDIMENTO);
                        }
                    }
                }

                // Situação da Requisição
                if (item.PREC_NM_SITUACAO_REQUISICAO != null)
                {
                    if (item.OperaSitReq != null & item.OperaSitReq > 0)
                    {
                        if (item.OperaSitReq == 1)
                        {
                            query = query.Where(p => p.PREC_NM_SITUACAO_REQUISICAO == item.PREC_NM_SITUACAO_REQUISICAO);
                        }
                        if (item.OperaSitReq == 3)
                        {
                            query = query.Where(p => p.PREC_NM_SITUACAO_REQUISICAO.Contains(item.PREC_NM_SITUACAO_REQUISICAO));
                        }
                    }
                }

                // Valor Inscrito Proposta
                if (item.PREC_VL_VALOR_INSCRITO_PROPOSTA != null & item.VALOR_INSC_PROP_FINAL == null)
                {
                    query = query.Where(p => p.PREC_VL_VALOR_INSCRITO_PROPOSTA >= item.PREC_VL_VALOR_INSCRITO_PROPOSTA);
                }
                if (item.PREC_VL_VALOR_INSCRITO_PROPOSTA == null & item.VALOR_INSC_PROP_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_VALOR_INSCRITO_PROPOSTA <= item.VALOR_INSC_PROP_FINAL);
                }
                if (item.PREC_VL_VALOR_INSCRITO_PROPOSTA != null & item.VALOR_INSC_PROP_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_VALOR_INSCRITO_PROPOSTA >= item.PREC_VL_VALOR_INSCRITO_PROPOSTA & p.PREC_VL_VALOR_INSCRITO_PROPOSTA <= item.VALOR_INSC_PROP_FINAL);
                }

                // Processo Execucao
                if (item.PREC_NM_PROC_EXECUCAO != null)
                {
                    if (item.OperaProcExec != null & item.OperaProcExec > 0)
                    {
                        if (item.OperaProcExec == 1)
                        {
                            query = query.Where(p => p.PREC_NM_PROC_EXECUCAO == item.PREC_NM_PROC_EXECUCAO);
                        }
                        if (item.OperaProcExec == 3)
                        {
                            query = query.Where(p => p.PREC_NM_PROC_EXECUCAO.Contains(item.PREC_NM_PROC_EXECUCAO));
                        }
                    }
                }

                // BEN DATA
                if (item.PREC_DT_BEN_DATABASE != null & item.BEN_DATA_FINAL == null)
                {
                    query = query.Where(p => p.PREC_DT_BEN_DATABASE >= item.PREC_DT_BEN_DATABASE);
                }
                if (item.PREC_DT_BEN_DATABASE == null & item.BEN_DATA_FINAL != null)
                {
                    query = query.Where(p => p.PREC_DT_BEN_DATABASE <= item.BEN_DATA_FINAL);
                }
                if (item.PREC_DT_BEN_DATABASE != null & item.BEN_DATA_FINAL != null)
                {
                    query = query.Where(p => p.PREC_DT_BEN_DATABASE >= item.PREC_DT_BEN_DATABASE & p.PREC_DT_BEN_DATABASE <= item.BEN_DATA_FINAL);
                }

                // Valor Principal
                if (item.PREC_VL_BEN_VALOR_PRINCIPAL != null & item.VALOR__PRINCIPAL_FINAL == null)
                {
                    query = query.Where(p => p.PREC_VL_BEN_VALOR_PRINCIPAL >= item.PREC_VL_BEN_VALOR_PRINCIPAL);
                }
                if (item.PREC_VL_BEN_VALOR_PRINCIPAL == null & item.VALOR__PRINCIPAL_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_BEN_VALOR_PRINCIPAL <= item.VALOR__PRINCIPAL_FINAL);
                }
                if (item.PREC_VL_BEN_VALOR_PRINCIPAL != null & item.VALOR__PRINCIPAL_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_BEN_VALOR_PRINCIPAL >= item.PREC_VL_BEN_VALOR_PRINCIPAL & p.PREC_VL_BEN_VALOR_PRINCIPAL <= item.VALOR__PRINCIPAL_FINAL);
                }

                // Juros
                if (item.PREC_VL_JUROS != null & item.JUROS_FINAL == null)
                {
                    query = query.Where(p => p.PREC_VL_JUROS >= item.PREC_VL_JUROS);
                }
                if (item.PREC_VL_JUROS == null & item.JUROS_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_JUROS <= item.JUROS_FINAL);
                }
                if (item.PREC_VL_JUROS != null & item.JUROS_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_JUROS >= item.PREC_VL_JUROS & p.PREC_VL_JUROS <= item.JUROS_FINAL);
                }

                // Valor PSS
                if (item.PREC_VL_VALOR_INICIAL_PSS != null & item.VALOR_PSS_FINAL == null)
                {
                    query = query.Where(p => p.PREC_VL_VALOR_INICIAL_PSS >= item.PREC_VL_VALOR_INICIAL_PSS);
                }
                if (item.PREC_VL_VALOR_INICIAL_PSS == null & item.VALOR_PSS_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_VALOR_INICIAL_PSS <= item.VALOR_PSS_FINAL);
                }
                if (item.PREC_VL_VALOR_INICIAL_PSS != null & item.VALOR_PSS_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_VALOR_INICIAL_PSS >= item.PREC_VL_VALOR_INICIAL_PSS & p.PREC_VL_VALOR_INICIAL_PSS <= item.VALOR_PSS_FINAL);
                }

                // Valor REQ
                if (item.PREC_VL_BEN_VALOR_REQUISITADO != null & item.VALOR_REQ_FINAL == null)
                {
                    query = query.Where(p => p.PREC_VL_BEN_VALOR_REQUISITADO >= item.PREC_VL_BEN_VALOR_REQUISITADO);
                }
                if (item.PREC_VL_BEN_VALOR_REQUISITADO == null & item.VALOR_REQ_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_BEN_VALOR_REQUISITADO <= item.VALOR_REQ_FINAL);
                }
                if (item.PREC_VL_BEN_VALOR_REQUISITADO != null & item.VALOR_REQ_FINAL != null)
                {
                    query = query.Where(p => p.PREC_VL_BEN_VALOR_REQUISITADO >= item.PREC_VL_BEN_VALOR_REQUISITADO & p.PREC_VL_BEN_VALOR_REQUISITADO <= item.VALOR_REQ_FINAL);
                }

                // Pesquisado
                if (item.PREC_IN_FOI_PESQUISADO != null & item.PREC_IN_FOI_PESQUISADO > 0)
                {
                    if (item.OperaPesq != null & item.OperaPesq > 0)
                    {
                        if (item.OperaPesq == 1)
                        {
                            query = query.Where(p => p.PREC_IN_FOI_PESQUISADO == item.PREC_IN_FOI_PESQUISADO);
                        }
                    }
                }

                // Situação Atual
                if (item.PREC_IN_SITUACAO_ATUAL != null & item.PREC_IN_SITUACAO_ATUAL > 0)
                {
                    if (item.OperaSitAtual != null & item.OperaSitAtual > 0)
                    {
                        if (item.OperaSitAtual == 1)
                        {
                            query = query.Where(p => p.PREC_IN_SITUACAO_ATUAL == item.PREC_IN_SITUACAO_ATUAL);
                        }
                    }
                }

                // CRM
                if (item.PREC_IN_FOI_IMPORTADO_PIPE != null & item.PREC_IN_FOI_IMPORTADO_PIPE > 0)
                {
                    if (item.OperaImpCRM != null & item.OperaImpCRM > 0)
                    {
                        if (item.OperaImpCRM == 1)
                        {
                            query = query.Where(p => p.PREC_IN_FOI_IMPORTADO_PIPE == item.PREC_IN_FOI_IMPORTADO_PIPE);
                        }
                    }
                }

                // Oficio Frequisitório
                if (item.PREC_NM_OFICIO_REQUISITORIO != null)
                {
                    if (item.OperaOfiReq != null & item.OperaOfiReq > 0)
                    {
                        if (item.OperaOfiReq == 1)
                        {
                            query = query.Where(p => p.PREC_NM_OFICIO_REQUISITORIO == item.PREC_NM_OFICIO_REQUISITORIO);
                        }
                        if (item.OperaOfiReq == 3)
                        {
                            query = query.Where(p => p.PREC_NM_OFICIO_REQUISITORIO.Contains(item.PREC_NM_OFICIO_REQUISITORIO));
                        }
                    }
                }

                // Req. Bloqueada
                if (item.PREC_NM_REQUISICAO_BLOQUEADA != null)
                {
                    if (item.OperaReqBloq != null & item.OperaReqBloq > 0)
                    {
                        if (item.OperaReqBloq == 1)
                        {
                            query = query.Where(p => p.PREC_NM_REQUISICAO_BLOQUEADA == item.PREC_NM_REQUISICAO_BLOQUEADA);
                        }
                        if (item.OperaReqBloq == 3)
                        {
                            query = query.Where(p => p.PREC_NM_REQUISICAO_BLOQUEADA.Contains(item.PREC_NM_REQUISICAO_BLOQUEADA));
                        }
                    }
                }

                // Data Expedicao
                if (item.PREC_DT_EXPEDICAO != null & item.DATA_EXP_FINAL == null)
                {
                    query = query.Where(p => p.PREC_DT_EXPEDICAO >= item.PREC_DT_EXPEDICAO);
                }
                if (item.PREC_DT_EXPEDICAO == null & item.DATA_EXP_FINAL != null)
                {
                    query = query.Where(p => p.PREC_DT_EXPEDICAO <= item.DATA_EXP_FINAL);
                }
                if (item.PREC_DT_EXPEDICAO != null & item.DATA_EXP_FINAL != null)
                {
                    query = query.Where(p => p.PREC_DT_EXPEDICAO >= item.PREC_DT_EXPEDICAO & p.PREC_DT_EXPEDICAO <= item.DATA_EXP_FINAL);
                }

                // Nome LOA
                if (item.PREC_NM_PRC_LOA != null)
                {
                    if (item.OperaNomLOA != null & item.OperaNomLOA > 0)
                    {
                        if (item.OperaNomLOA == 1)
                        {
                            query = query.Where(p => p.PREC_NM_PRC_LOA == item.PREC_NM_PRC_LOA);
                        }
                        if (item.OperaNomLOA == 3)
                        {
                            query = query.Where(p => p.PREC_NM_PRC_LOA.Contains(item.PREC_NM_PRC_LOA));
                        }
                    }
                }

                // Finaliza
                if (query != null)
                {
                    query = query.OrderBy(a => a.PREC_NM_PRECATORIO);
                    lista = query.ToList<PRECATORIO>();
                }

                // Sucesso
                listaMasterTran = lista;
                Session["ListaPrecatorio"] = lista;
                return RedirectToAction("MontarTelaPrecatorio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPrecatorio");
            }
        }

        public List<PRECATORIO> CarregaPrecatorio()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PRECATORIO> conf = new List<PRECATORIO>();
            if (Session["Precatorios"] == null)
            {
                conf = tranApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["PrecatorioAlterada"] == 1)
                {
                    conf = tranApp.GetAllItens();
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

        public List<BENEFICIARIO> CarregaBeneficiario()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<BENEFICIARIO> conf = new List<BENEFICIARIO>();
            if (Session["Beneficiarios"] == null)
            {
                conf = tranApp.GetAllBeneficiarios();
            }
            else
            {
                if ((Int32)Session["BeneficiarioAlterada"] == 1)
                {
                    conf = tranApp.GetAllBeneficiarios();
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

        public List<NATUREZA> CarregaNatureza()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<NATUREZA> conf = new List<NATUREZA>();
            if (Session["Naturezas"] == null)
            {
                conf = tranApp.GetAllNaturezas();
            }
            else
            {
                conf = (List<NATUREZA>)Session["Naturezas"];
            }
            Session["Naturezas"] = conf;
            return conf;
        }

        public List<TRF> CarregaTRF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TRF> conf = new List<TRF>();
            if (Session["TRFs"] == null)
            {
                conf = tranApp.GetAllTRF();
            }
            else
            {
                conf = (List<TRF>)Session["TRFs"];
            }
            Session["TRFs"] = conf;
            return conf;
        }

        public List<PRECATORIO_ESTADO> CarregaEstado()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PRECATORIO_ESTADO> conf = new List<PRECATORIO_ESTADO>();
            if (Session["Estados"] == null)
            {
                conf = tranApp.GetAllEstados();
            }
            else
            {
                conf = (List<PRECATORIO_ESTADO>)Session["Estados"];
            }
            Session["Estados"] = conf;
            return conf;
        }

        public List<HONORARIO> CarregaHonorario()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<HONORARIO> conf = new List<HONORARIO>();
            if (Session["Honorarios"] == null)
            {
                conf = tranApp.GetAllAdvogados();
            }
            else
            {
                if ((Int32)Session["HonorarioAlterada"] == 1)
                {
                    conf = tranApp.GetAllAdvogados();
                }
                else
                {
                    conf = (List<HONORARIO>)Session["Honorarios"];
                }
            }
            Session["Honorarios"] = conf;
            Session["HonorarioAlterada"] = 0;
            return conf;
        }

        public List<USUARIO> CarregaUsuario()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<USUARIO> conf = new List<USUARIO>();
            if (Session["Usuarios"] == null)
            {
                conf = usuApp.GetAllItens(1);
            }
            else
            {
                if ((Int32)Session["UsuarioAlterada"] == 1)
                {
                    conf = usuApp.GetAllItens(1);
                }
                else
                {
                    conf = (List<USUARIO>)Session["Usuarios"];
                }
            }
            Session["Usuarios"] = conf;
            Session["UsuarioAlterada"] = 0;
            return conf;
        }

        public List<CONTATO> CarregaContato()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CONTATO> conf = new List<CONTATO>();
            if (Session["Contatos"] == null)
            {
                conf = conApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["ContatoAlterada"] == 1)
                {
                    conf = conApp.GetAllItens();
                }
                else
                {
                    conf = (List<CONTATO>)Session["Contatos"];
                }
            }
            Session["Contatos"] = conf;
            Session["ContatoAlterada"] = 0;
            return conf;
        }

        public List<QUALIFICACAO> CarregaQualificacao()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<QUALIFICACAO> conf = new List<QUALIFICACAO>();
            if (Session["Qualificacoes"] == null)
            {
                conf = conApp.GetAllQualificacao();
            }
            else
            {
                if ((Int32)Session["QualificacaoAlterada"] == 1)
                {
                    conf = conApp.GetAllQualificacao();
                }
                else
                {
                    conf = (List<QUALIFICACAO>)Session["Qualificacoes"];
                }
            }
            Session["Qualificacoes"] = conf;
            Session["QualificacaoAlterada"] = 0;
            return conf;
        }

        public List<QUEM_DESLIGOU> CarregaDesligamento()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<QUEM_DESLIGOU> conf = new List<QUEM_DESLIGOU>();
            if (Session["Desligas"] == null)
            {
                conf = conApp.GetAllDesligamento();
            }
            else
            {
                if ((Int32)Session["DesligaAlterada"] == 1)
                {
                    conf = conApp.GetAllDesligamento();
                }
                else
                {
                    conf = (List<QUEM_DESLIGOU>)Session["Desligas"];
                }
            }
            Session["Desligas"] = conf;
            Session["DesligaAlterada"] = 0;
            return conf;
        }

        public List<CLIENTE> CarregaCliente()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> conf = new List<CLIENTE>();
            if (Session["Clientes"] == null)
            {
                conf = cliApp.GetAllItens(1);
            }
            else
            {
                if ((Int32)Session["ClienteAlterada"] == 1)
                {
                    conf = cliApp.GetAllItens(1);
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

    }
}