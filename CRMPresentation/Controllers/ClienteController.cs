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
using log4net;
using System.Reflection;
using log4net.Config;   
using log4net.Core;
using Azure.Communication.Email;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly ITipoPessoaAppService tpApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IClienteCnpjAppService ccnpjApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ICRMAppService crmApp;
        private readonly IGrupoAppService gruApp;
        private readonly ICategoriaClienteAppService ccApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IAssinanteAppService assApp;

        private String msg;
        private Exception exception;
        CLIENTE objeto = new CLIENTE();
        CLIENTE objetoAntes = new CLIENTE();
        List<CLIENTE> listaMaster = new List<CLIENTE>();
        CLIENTE_FALHA objetoFalha = new CLIENTE_FALHA();
        CLIENTE_FALHA objetoAntesFalha = new CLIENTE_FALHA();
        List<CLIENTE_FALHA> listaMasterFalha = new List<CLIENTE_FALHA>();
        String extensao;

        public ClienteController(IClienteAppService baseApps, ILogAppService logApps, ITipoPessoaAppService tpApps, IUsuarioAppService usuApps, IClienteCnpjAppService ccnpjApps, IConfiguracaoAppService confApps, ICRMAppService crmApps, IGrupoAppService gruApps, ICategoriaClienteAppService ccApps, IMensagemEnviadaSistemaAppService meApps, IAssinanteAppService assApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            tpApp = tpApps;
            usuApp = usuApps;
            ccnpjApp = ccnpjApps;
            confApp = confApps;
            crmApp = crmApps;
            gruApp = gruApps;
            ccApp = ccApps;
            meApp= meApps;
            assApp = assApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
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
            if ((Int32)Session["VoltaMensagem"] == 30)
            {
                Session["VoltaMensagem"] = 0;
                return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
            }
            if ((Int32)Session["VoltaMensagem"] == 40)
            {
                Session["VoltaMensagem"] = 0;
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult EnviarSmsCliente(Int32 id, String mensagem)
        {
            try
            {
                CLIENTE clie = baseApp.GetById(id);
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Verifica existencia prévia
                if (clie == null)
                {
                    Session["MensSMSClie"] = 1;
                    return RedirectToAction("MontarTelaCliente");
                }

                // Criticas
                if (clie.CLIE_NR_CELULAR == null)
                {
                    Session["MensSMSClie"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }

                // Monta token
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = mensagem;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(texto);
                String body = str.ToString();
                String smsBody = body;

                // inicia processo
                String resposta = String.Empty;
                try
                {
                    String listaDest = "55" + Regex.Replace(clie.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String customId = Cryptography.GenerateRandomPassword(8);
                    String data = String.Empty;
                    String json = String.Empty;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
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
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }

                Session["MensSMSClie"] = 200;
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                Session["MensSMSClie"] = 3;
                Session["MensSMSClieErro"] = ex.Message;
                return RedirectToAction("MontarTelaCliente");
            }
        }

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> clientes = baseApp.GetAllItens(idAss);
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                clientes = clientes.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
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

        public ActionResult DashboardAdministracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            listaMaster = new List<CLIENTE>();
            Session["Cliente"] = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        public void FlagContinua()
        {
            Session["VoltaCliente"] = 3;
        }

        [HttpGet]
        public ActionResult IncluirGrupo()
        {
            Session["VoltaCliGrupo"] = 10;
            Session["VoltaGrupo"] = 11;
            return RedirectToAction("IncluirGrupo", "Grupo");
        }

        [HttpGet]
        public ActionResult VerGrupoTela(Int32 id)
        {
            Session["VoltaTela"] = 3;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            Session["VoltaCliGrupo"] = 10;
            Session["VoltaGrupo"] = 11;
            return RedirectToAction("VerGrupo", "Grupo", new { id = id });
        }

        [HttpGet]
        public ActionResult VerGrupoTodos()
        {
            Session["VoltaCliGrupo"] = 10;
            return RedirectToAction("MontarTelaGrupo", "Grupo");
        }

        [HttpGet]
        public ActionResult IncluirGrupoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            Session["VoltaTela"] = 3;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            List<GRUPO> grupos = gruApp.GetAllItens(idAss);
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                grupos = grupos.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Grupos = new SelectList(grupos.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            GRUPO_CLIENTE item = new GRUPO_CLIENTE();
            GrupoContatoViewModel vm = Mapper.Map<GRUPO_CLIENTE, GrupoContatoViewModel>(item);
            vm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            vm.GRCL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirGrupoCliente(GrupoContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<GRUPO> grupos = gruApp.GetAllItens(idAss);
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                grupos = grupos.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Grupos = new SelectList(grupos.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    GRUPO_CLIENTE item = Mapper.Map<GrupoContatoViewModel, GRUPO_CLIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateGrupo(item);

                    // Verifica retorno
                    Session["NivelCliente"] = 4;
                    return RedirectToAction("VoltarAnexoCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirGrupoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaTela"] = 3;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            GRUPO_CLIENTE item = baseApp.GetGrupoById(id);
            item.GRCL_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditGrupo(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult ReativarGrupoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaTela"] = 3;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            GRUPO_CLIENTE item = baseApp.GetGrupoById(id);
            item.GRCL_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditGrupo(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public JsonResult GetValorGrafico(Int32 id, Int32? meses)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (meses == null)
            {
                meses = 3;
            }

            var clie = baseApp.GetById(id);
            List<CRM_PEDIDO_VENDA> peds = crmApp.GetAllPedidos(idAss).Where(p => p.CLIE_CD_ID == id).ToList();
            Int32 m1 = peds.Where(x => x.CRPV_DT_PEDIDO >= DateTime.Now.AddDays(DateTime.Now.Day * -1)).ToList().Count;
            Int32 m2 = peds.Where(x => x.CRPV_DT_PEDIDO >= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-1) && x.CRPV_DT_PEDIDO <= DateTime.Now.AddDays(DateTime.Now.Day * -1)).ToList().Count;
            Int32 m3 = peds.Where(x => x.CRPV_DT_PEDIDO >= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-2) && x.CRPV_DT_PEDIDO <= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-1)).ToList().Count;

            var hash = new Hashtable();
            hash.Add("m1", m1);
            hash.Add("m2", m2);
            hash.Add("m3", m3);
            return Json(hash);
        }

        [HttpPost]
        public JsonResult PesquisaCNPJ(string cnpj)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lstQs = new List<CLIENTE_QUADRO_SOCIETARIO>();

            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(cnpj, "[^0-9]", "");
            String json = String.Empty;

            WebRequest request = WebRequest.Create(url);
            request.Headers["Authorization"] = "df3c411d-bb44-41eb-9304-871c45d72978-cd751b62-ff3d-4421-a9d2-b97e01ca6d2b";

            try
            {
                WebResponse response = request.GetResponse();

                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }

                var jObject = JObject.Parse(json);

                if (jObject["membership"].Count() == 0)
                {
                    CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                    qs.CLIENTE = new CLIENTE();
                    qs.CLIENTE.CLIE_NM_RAZAO = jObject["name"] == null ? String.Empty : jObject["name"].ToString();
                    qs.CLIENTE.CLIE_NM_NOME = jObject["alias"] == null ? jObject["name"].ToString() : jObject["alias"].ToString();
                    qs.CLIENTE.CLIE_NR_CEP = jObject["address"]["zip"].ToString();
                    qs.CLIENTE.CLIE_NM_ENDERECO = jObject["address"]["street"].ToString();
                    qs.CLIENTE.CLIE_NR_NUMERO = jObject["address"]["number"].ToString();
                    qs.CLIENTE.CLIE_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                    qs.CLIENTE.CLIE_NM_CIDADE = jObject["address"]["city"].ToString();
                    qs.CLIENTE.UF_CD_ID = baseApp.GetAllUF().Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                    qs.CLIENTE.CLIE_NR_INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                    qs.CLIENTE.CLIE_NR_TELEFONE = jObject["phone"].ToString();
                    qs.CLIENTE.CLIE_NR_TELEFONE_ADICIONAL = jObject["phone_alt"].ToString();
                    qs.CLIENTE.CLIE_NM_EMAIL = jObject["email"].ToString();
                    qs.CLIENTE.CLIE_NM_SITUACAO = jObject["registration"]["status"].ToString();
                    qs.CLQS_IN_ATIVO = 0;

                    lstQs.Add(qs);
                }
                else
                {
                    foreach (var s in jObject["membership"])
                    {
                        CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                        qs.CLIENTE = new CLIENTE();
                        qs.CLIENTE.CLIE_NM_RAZAO = jObject["name"].ToString() == "" ? String.Empty : jObject["name"].ToString();
                        qs.CLIENTE.CLIE_NM_NOME = jObject["alias"].ToString() == "" ? jObject["name"].ToString() : jObject["alias"].ToString();
                        qs.CLIENTE.CLIE_NR_CEP = jObject["address"]["zip"].ToString();
                        qs.CLIENTE.CLIE_NM_ENDERECO = jObject["address"]["street"].ToString();
                        qs.CLIENTE.CLIE_NR_NUMERO = jObject["address"]["number"].ToString();
                        qs.CLIENTE.CLIE_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                        qs.CLIENTE.CLIE_NM_CIDADE = jObject["address"]["city"].ToString();
                        qs.CLIENTE.UF_CD_ID = baseApp.GetAllUF().Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                        qs.CLIENTE.CLIE_NR_INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                        qs.CLIENTE.CLIE_NR_TELEFONE = jObject["phone"].ToString();
                        qs.CLIENTE.CLIE_NR_TELEFONE_ADICIONAL = jObject["phone_alt"].ToString();
                        qs.CLIENTE.CLIE_NM_EMAIL = jObject["email"].ToString();
                        qs.CLIENTE.CLIE_NM_SITUACAO = jObject["registration"]["status"].ToString();
                        qs.CLQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                        qs.CLQS_NM_NOME = s["name"].ToString();

                        // CNPJá não retorna esses valores
                        qs.CLQS_NM_PAIS_ORIGEM = String.Empty;
                        qs.CLQS_NM_REPRESENTANTE_LEGAL = String.Empty;
                        qs.CLQS_NM_QUALIFICACAO_REP_LEGAL = String.Empty;
                        lstQs.Add(qs);
                    }
                }
                return Json(lstQs);
            }
            catch (WebException ex)
            {
                var hash = new Hashtable();
                hash.Add("status", "ERROR");

                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "BadRequest")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "CNPJ inválido");
                    return Json(hash);
                }
                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "NotFound")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "O CNPJ consultado não está registrado na Receita Federal");
                    return Json(hash);
                }
                else
                {
                    hash.Add("public", 1);
                    hash.Add("message", ex.Message);
                    return Json(hash);
                }
            }
        }

        private List<CLIENTE_QUADRO_SOCIETARIO> PesquisaCNPJ(CLIENTE cliente)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lstQs = new List<CLIENTE_QUADRO_SOCIETARIO>();

            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(cliente.CLIE_NR_CNPJ, "[^0-9]", "");
            String json = String.Empty;

            WebRequest request = WebRequest.Create(url);
            request.Headers["Authorization"] = "df3c411d-bb44-41eb-9304-871c45d72978-cd751b62-ff3d-4421-a9d2-b97e01ca6d2b";

            WebResponse response = request.GetResponse();

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
            {
                json = reader.ReadToEnd();
            }

            var jObject = JObject.Parse(json);
            foreach (var s in jObject["membership"])
            {
                CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                qs.CLQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                qs.CLQS_NM_NOME = s["name"].ToString();
                qs.CLIE_CD_ID = cliente.CLIE_CD_ID;

                // CNPJá não retorna esses valores
                qs.CLQS_NM_PAIS_ORIGEM = String.Empty;
                qs.CLQS_NM_REPRESENTANTE_LEGAL = String.Empty;
                qs.CLQS_NM_QUALIFICACAO_REP_LEGAL = String.Empty;
                lstQs.Add(qs);
            }
            return lstQs;
        }

        [HttpGet]
        public ActionResult MontarTelaCliente()
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
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            if ((List<CLIENTE>)Session["ListaCliente"] == null)
            {
                listaMaster = CarregaClienteUltimos(conf.CONF_NR_GRID_CLIENTE.Value);
                List<GRUPO> grupos = gruApp.GetAllItens(idAss);
                ViewBag.Grupos = new SelectList(grupos.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
                Session["ListaCliente"] = listaMaster;
            }

            // Verifica se houve alteração
            if ((Int32)Session["FlagAlteraCliente"] == 1)
            {
                List<CLIENTE> lista1 = (List<CLIENTE>)Session["ListaCliente"];
                lista1 = lista1.Where(p => p.CLIE_CD_ID == (Int32)Session["IdCliente"]).ToList();
                Session["ListaCliente"] = lista1;
                Session["MensCliente"] = 77;
            }

            // Monta demais listas
            ViewBag.Listas = (List<CLIENTE>)Session["ListaCliente"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(CarregaCatCliente(), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Cliente"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            Session["VoltaTela"] = 0;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];

            // Indicadores
            List<CLIENTE> listaCli = ((List<CLIENTE>)Session["ListaCliente"]).ToList();
            Int32 totalCli = listaCli.Count;
            ViewBag.Clientes = totalCli;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<CLIENTE> inativos = CarregaClienteAdm().Where(p => p.CLIE_IN_ATIVO == 0).ToList();
            ViewBag.Inativos = inativos.Count;
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                ViewBag.Inativos = inativos.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList().Count;
            }

            List<CRM_PEDIDO_VENDA> peds = CarregaPedidoVenda().Where(p => p.CRPV_IN_ATIVO == 1).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                peds = peds.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }

            List<Int32?> clientes = peds.OrderBy(m => m.CLIE_CD_ID).Select(p => p.CLIE_CD_ID).Distinct().ToList();
            Int32 pedidos = totalCli - clientes.Count;
            ViewBag.Pedidos = pedidos;
            ViewBag.CodigoCliente = Session["IdCliente"];
            ViewBag.TipoCarga = (Int32)Session["TipoCarga"];
            ViewBag.NumCarga = conf.CONF_NR_GRID_CLIENTE;

            // Mensagem
            if (Session["MensCliente"] != null)
            {
                if ((Int32)Session["MensCliente"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0181", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0080", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 77)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0222", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 99)
                {
                    ModelState.AddModelError("", "Foram processados e incluídos " + ((Int32)Session["Conta"]).ToString() + " clientes");
                    ModelState.AddModelError("", "Foram processados e rejeitados " + ((Int32)Session["Falha"]).ToString() + " clientes");
                }
                if ((Int32)Session["MensCliente"] == 200)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0253", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 100)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0256", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                    ModelState.AddModelError("", frase);
                }
                if ((Int32)Session["MensCliente"] == 101)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                    ModelState.AddModelError("", frase);
                }
            }

            // Abre view
            Session["FlagMensagensEnviadas"] = 2;
            Session["FlagAlteraCliente"] = 0;
            Session["MensCliente"] = 0;
            Session["VoltaCliente"] = 1;
            Session["VoltaClienteCRM"] = 0;
            Session["VoltaCRM"] = 0;
            Session["NivelCliente"] = 1;
            Session["VoltaMsg"] = 1;
            Session["ListaFalha"] = null;
            objeto = new CLIENTE();
            if (Session["FiltroCliente"] != null)
            {
                objeto = (CLIENTE)Session["FiltroCliente"];
            }
            objeto.CLIE_IN_ATIVO = 1;
            return View(objeto);
        }

        public ActionResult RetirarFiltroCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCliente"] = null;
            Session["FiltroCliente"] = null;
            Session["TipoCarga"] = 1;
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult MostrarTudoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregaClienteAdm();
            Session["FiltroCliente"] = null;
            Session["ListaCliente"] = listaMaster;
            Session["TipoCarga"] = 2;
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpPost]
        public ActionResult FiltrarCliente(CLIENTE item)
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
                Session["FiltroCliente"] = item;
                Tuple < Int32, List<CLIENTE>, Boolean > volta = baseApp.ExecuteFilterTuple(item.CLIE_CD_ID, item.CACL_CD_ID, item.CLIE_NM_RAZAO, item.CLIE_NM_NOME, item.CLIE_NR_CPF, item.CLIE_NR_CNPJ, item.CLIE_NM_EMAIL, item.CLIE_NM_CIDADE, item.UF_CD_ID, item.CLIE_IN_ATIVO, 3, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensCliente"] = 1;
                    return RedirectToAction("MontarTelaCliente");
                }

                // Sucesso
                listaMaster = volta.Item2;
                Session["ListaCliente"] = listaMaster;
                Session["TipoCarga"] = 2;
                if ((Int32)Session["VoltaCliente"] == 2)
                {
                    return RedirectToAction("VerCardsCliente");
                }
                return RedirectToAction("MontarTelaCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseCliente()
        {
            if ((Int32)Session["VoltaMsg"] == 99)
            {
                return RedirectToAction("MontarTelaIndicadoresCliente", "Cliente");
            }
            if ((Int32)Session["VoltaMsg"] == 1)
            {
                return RedirectToAction("MontarTelaCliente", "Cliente");
            }
            if ((Int32)Session["VoltaMsg"] == 55)
            {
                return RedirectToAction("PesquisarTudo", "BaseAdmin");
            }
            if ((Int32)Session["VoltaMsg"] == 2)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            if ((Int32)Session["VoltaClienteCRM"] == 99)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["VoltaClienteCRM"] == 0)
            {
                return RedirectToAction("MontarTelaCliente");
            }
            if ((Int32)Session["VoltaClienteCRM"] == 1)
            {
                return RedirectToAction("IncluirProcessoCRM", "CRM");
            }
            if ((Int32)Session["VoltaClienteCRM"] == 5)
            {
                return RedirectToAction("VoltarEditarPedidoCRMCliente", "CRM");
            }
            if ((Int32)Session["VoltaClienteCRM"] == 2)
            {
                return RedirectToAction("VoltarBaseCRM", "CRM");
            }
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            if ((Int32)Session["VoltaCliente"] == 3)
            {
                return RedirectToAction("VerClientesAtraso");
            }
            if ((Int32)Session["VoltaCliente"] == 4)
            {
                return RedirectToAction("VerClientesSemPedidos");
            }
            if ((Int32)Session["VoltaCliente"] == 5)
            {
                return RedirectToAction("VerClientesInativos");
            }
            if ((Int32)Session["VoltaCliente"] == 6)
            {
                return RedirectToAction("IncluirAtendimento", "Atendimento");
            }
            if ((Int32)Session["VoltaCliente"] == 7)
            {
                return RedirectToAction("IncluirOrdemServico", "OrdemServico");
            }
            if ((Int32)Session["VoltaCRM"] == 11)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
            }
            if ((Int32)Session["VoltaMensagem"] == 30)
            {
                Session["VoltaMensagem"] = 0;
                return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
            }
            return RedirectToAction("MontarTelaCliente");
        }

       [HttpGet]
        public ActionResult IncluirCliente()
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = CarregaCliente().Count;
            if ((Int32)Session["NumClientes"] <= num)
            {
                Session["MensCliente"] = 50;
                return RedirectToAction("MontarTelaCliente", "Cliente");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(CarregaCatCliente(), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");;
            ViewBag.TiposCont = new SelectList(CarregaTipoContribuinte(), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<USUARIO> usuarios = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                usuarios = usuarios.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Nacionalidades = new SelectList(CarregaNacionalidade(), "NACI_CD_ID", "NACI_NM_PAIS");
            ViewBag.Municipios = new SelectList(CarregaMunicipio(), "MUNI_CD_ID", "MUNI_NM_NOME");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");
            ViewBag.Perfil = (String)Session["PerfilUsuario"];

            List<UF> uFs = CarregaUF();
            List<SelectListItem> ufNatur = new List<SelectListItem>();
            foreach (UF uf in uFs)
            {
                ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
            }
            ViewBag.UFNatur = new SelectList(ufNatur, "Value", "Text");

            // Prepara objeto
            Session["ClienteNovo"] = 0;
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CLIE_DT_CADASTRO = DateTime.Today;
            vm.CLIE_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.TIPE_CD_ID = 0;
            vm.EMPR_CD_ID = 3;
            vm.CLIE_DT_ULTIMO_USO = null;
            vm.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCliente(ClienteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(CarregaCatCliente(), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME"); ;
            ViewBag.TiposCont = new SelectList(CarregaTipoContribuinte(), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<USUARIO> usuarios = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                usuarios = usuarios.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Nacionalidades = new SelectList(CarregaNacionalidade(), "NACI_CD_ID", "NACI_NM_PAIS");
            ViewBag.Municipios = new SelectList(CarregaMunicipio(), "MUNI_CD_ID", "MUNI_NM_NOME");
            List<UF> uFs = CarregaUF();
            List<SelectListItem> ufNatur = new List<SelectListItem>();
            foreach (UF uf in uFs)
            {
                ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
            }
            ViewBag.UFNatur = new SelectList(ufNatur, "Value", "Text");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCliente"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0181", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Carrega foto e processa alteracao
                    if (item.CLIE_AQ_FOTO == null)
                    {
                        item.CLIE_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                        volta = baseApp.ValidateEdit(item, item, usuario);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Atualiza cache
                    listaMaster = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirCliente"] = 1;
                    Session["ClienteNovo"] = item.CLIE_CD_ID;
                    Session["Clientes"] = baseApp.GetAllItens(idAss);
                    Session["IdVolta"] = item.CLIE_CD_ID;
                    Session["IdCliente"] = item.CLIE_CD_ID;
                    Session["ClienteAlterada"] = 1;

                    // Atualiza CNPJ
                    if (item.TIPE_CD_ID == 2 & item.CLIE_NR_CNPJ != null)
                    {
                        var lstQs = PesquisaCNPJ(item);

                        foreach (var qs in lstQs)
                        {
                            Int32 voltaQs = ccnpjApp.ValidateCreate(qs, usuario);
                        }
                    }

                    // Trata anexos
                    if (Session["FileQueueCliente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCliente"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueCliente(file);
                            }
                            else
                            {
                                UploadFotoQueueCliente(file);
                            }
                        }
                        Session["FileQueueCliente"] = null;
                    }

                    // Trata retorno
                    if ((Int32)Session["VoltaCliente"] == 6)
                    {
                        return RedirectToAction("IncluirAtendimento", "Atendimento");
                    }
                    if ((Int32)Session["VoltaCliente"] == 7)
                    {
                        return RedirectToAction("IncluirOrdemServico", "OrdemServico");
                    }
                    if ((Int32)Session["VoltaClienteCRM"] == 1)
                    {
                        return RedirectToAction("IncluirProcessoCRM", "CRM");
                    }
                    if ((Int32)Session["VoltaCliente"] == 3)
                    {
                        Session["VoltaCliente"] = 0;
                        return RedirectToAction("IncluirCliente", "Cliente");
                    }
                    Session["NivelCliente"] = 1;
                    return RedirectToAction("MontarTelaCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                vm.TIPE_CD_ID = 0;
                vm.SEXO_CD_ID = 0;
                return View(vm);
            }
        }

        [HttpPost]
        public JsonResult FiltrarMunicipio(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaFiltrada = CarregaMunicipio();

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaFiltrada = listaFiltrada.Where(x => x.UF_CD_ID == id).ToList();
            }
            return Json(listaFiltrada.Select(x => new { x.MUNI_CD_ID, x.MUNI_NM_NOME }));
        }

        [HttpGet]
        public ActionResult EditarClienteGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["NivelCliente"] = 1;
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        [HttpGet]
        public ActionResult EditarClienteEndereco()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["NivelCliente"] = 11;
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }


        [HttpGet]
        public ActionResult EditarClienteContatos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["NivelCliente"] = 4;
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        [HttpGet]
        public ActionResult EditarClientePessoal()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["NivelCliente"] = 13;
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        [HttpGet]
        public ActionResult EditarClienteObservacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["NivelCliente"] = 14;
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        [HttpGet]
        public ActionResult EditarCliente(Int32 id)
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(CarregaCatCliente(), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME"); ;
            ViewBag.TiposCont = new SelectList(CarregaTipoContribuinte(), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<USUARIO> usuarios = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                usuarios = usuarios.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Nacionalidades = new SelectList(CarregaNacionalidade(), "NACI_CD_ID", "NACI_NM_PAIS");
            ViewBag.Municipios = new SelectList(CarregaMunicipio(), "MUNI_CD_ID", "MUNI_NM_NOME");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");

            List<UF> uFs = CarregaUF();
            List<SelectListItem> ufNatur = new List<SelectListItem>();
            List<SelectListItem> ufNatur1 = new List<SelectListItem>();
            foreach (UF uf in uFs)
            {
                ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
                ufNatur1.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
            }
            ViewBag.UFNatur1 = new SelectList(ufNatur, "Value", "Text");
            ViewBag.UFNatur2 = new SelectList(ufNatur1, "Value", "Text");

            // Carrega cliente
            CLIENTE item = baseApp.GetItemById(id);
            ViewBag.QuadroSoci = ccnpjApp.GetByCliente(item);

            // Trata mensagens
            if (Session["MensCliente"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCliente"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 66)
                {
                    String msg = "O cliente " + item.CLIE_NM_NOME + " teve suas informações alteradas.";                 
                    ModelState.AddModelError("", msg);
                }
            }

            // Indicadores
            List<CRM_PEDIDO_VENDA> listaProp = crmApp.GetAllPedidos(idAss).Where(p => p.CLIE_CD_ID == id).ToList();
            listaProp = listaProp.Where(p => p.CRPV_DT_PEDIDO > DateTime.Today.Date.AddDays(-30)).ToList();

            Int32 props = listaProp.Count;
            Int32 envs = listaProp.Where(p => p.CRPV_IN_STATUS == 2).ToList().Count;
            Int32 aprovs = listaProp.Where(p => p.CRPV_IN_STATUS == 5).ToList().Count;
            Int32 reprovs = listaProp.Where(p => p.CRPV_IN_STATUS == 4).ToList().Count;

            Session["ListaVendas"] = listaProp;
            Session["Props"] = props;
            Session["Envs"] = envs;
            Session["Aprovs"] = aprovs;
            Session["Reprovs"] = reprovs;
            Session["Nivel"] = 1;

            ViewBag.Props = props;
            ViewBag.Envs = envs;
            ViewBag.Aprovs = aprovs;
            ViewBag.Reprovs = reprovs;
            ViewBag.ListaVendas = listaProp;
            ViewBag.NivelCliente = (Int32)Session["NivelCliente"];

            // Carrega view
            Session["FlagMensagensEnviadas"] = 2;
            ViewBag.Incluir = 1;
            Session["VoltaCliente"] = 1;
            objetoAntes = item;
            Session["Cliente"] = item;
            Session["IdCliente"] = id;
            Session["IdVolta"] = id;
            Session["VoltaCEP"] = 1;
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCliente(ClienteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(CarregaCatCliente(), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME"); ;
            ViewBag.TiposCont = new SelectList(CarregaTipoContribuinte(), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<USUARIO> usuarios = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                usuarios = usuarios.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Nacionalidades = new SelectList(CarregaNacionalidade(), "NACI_CD_ID", "NACI_NM_PAIS");
            ViewBag.Municipios = new SelectList(CarregaMunicipio(), "MUNI_CD_ID", "MUNI_NM_NOME");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");

            List<UF> uFs = CarregaUF();
            List<SelectListItem> ufNatur = new List<SelectListItem>();
            List<SelectListItem> ufNatur1 = new List<SelectListItem>();
            foreach (UF uf in uFs)
            {
                ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
                ufNatur1.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
            }
            ViewBag.UFNatur1 = new SelectList(ufNatur, "Value", "Text");
            ViewBag.UFNatur2 = new SelectList(ufNatur1, "Value", "Text");

            CLIENTE cli = baseApp.GetItemById(vm.CLIE_CD_ID);
            ViewBag.QuadroSoci = ccnpjApp.GetByCliente(cli);
            Session["FlagAlteraCliente"] = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica se não houve alteração
                    if (item == objetoAntes)
                    {
                        Session["FlagAlteraCliente"] = 0;
                    }

                    // Atualiza cache
                    listaMaster = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirCliente"] = 0;
                    Session["ClienteAlterada"] = 1;

                    // Trata retorno
                    if (Session["FiltroCliente"] != null)
                    {
                        FiltrarCliente((CLIENTE)Session["FiltroCliente"]);
                    }

                    if ((Int32)Session["VoltaCliente"] == 2)
                    {
                        return RedirectToAction("VerCardsCliente");
                    }
                    if ((Int32)Session["VoltaCliente"]  == 5)
                    {
                        return RedirectToAction("VerClientesInativos");
                    }
                    if ((Int32)Session["VoltaCRM"] == 11)
                    {
                        return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
                    }

                    // Retorno normal
                    Session["MensCliente"] = 66;
                    return RedirectToAction("VoltarAnexoCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                vm = Mapper.Map<CLIENTE, ClienteViewModel>(cli);
                ViewBag.Props = (Int32)Session["Props"];
                ViewBag.Envs = (Int32)Session["Envs"];
                ViewBag.Aprovs = (Int32)Session["Aprovs"];
                ViewBag.Reprovs = (Int32)Session["Reprovs"];
                ViewBag.ListaVendas = (List<CRM_PEDIDO_VENDA>)Session["ListaVendas"];
                ViewBag.NivelCliente = (Int32)Session["NivelCliente"];
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCliente(Int32 id)
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
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
                CLIENTE item = baseApp.GetItemById(id);
                objetoAntes = (CLIENTE)Session["Cliente"];
                item.CLIE_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensCliente"] = 4;
                    return RedirectToAction("MontarTelaCliente", "Cliente");
                }
                listaMaster = new List<CLIENTE>();
                Session["ListaCliente"] = null;
                Session["FiltroCliente"] = null;
                Session["ClienteAlterada"] = 1;
                Session["NivelCliente"] = 1;
                return RedirectToAction("MontarTelaCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarCliente(Int32 id)
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = CarregaCliente().Count;
            if ((Int32)Session["NumClientes"] <= num)
            {
                Session["MensCliente"] = 50;
                return RedirectToAction("MontarTelaCliente", "Cliente");
            }

            // Executar reativação
            try
            {
                CLIENTE item = baseApp.GetItemById(id);
                objetoAntes = (CLIENTE)Session["Cliente"];
                item.CLIE_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                listaMaster = new List<CLIENTE>();
                Session["ListaCliente"] = null;
                Session["FiltroCliente"] = null;
                Session["ClienteAlterada"] = 1;
                Session["NivelCliente"] = 1;
                return RedirectToAction("MontarTelaCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerCardsCliente()
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
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            if ((List<CLIENTE>)Session["ListaCliente"] == null || ((List<CLIENTE>)Session["ListaCliente"]).Count == 0)
            {
                listaMaster = CarregaClienteUltimos(conf.CONF_NR_GRID_CLIENTE.Value);
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaCliente"] = listaMaster;
            }
            ViewBag.Listas = (List<CLIENTE>)Session["ListaCliente"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(CarregaCatCliente(), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            Session["Cliente"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            ViewBag.TipoCarga = (Int32)Session["TipoCarga"];
            ViewBag.NumCarga = conf.CONF_NR_GRID_CLIENTE;

            // Indicadores
            ViewBag.Clientes = ((List<CLIENTE>)Session["ListaCliente"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagens
            if (Session["MensCliente"] != null)
            {
            }

            // Abre view
            Session["VoltaCliente"] = 2;
            Session["NivelCliente"] = 1;
            objeto = new CLIENTE();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult VerAnexoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
            Session["NivelCliente"] = 2;
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoClienteAudio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
            Session["NivelCliente"] = 2;
            return View(item);
        }

        public ActionResult VoltarAnexoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        public ActionResult VoltarAnexoCliente1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaMensagem"] == 40)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            if ((Int32)Session["VoltaMensagem"] == 30)
            {
                return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        public FileResult DownloadCliente(Int32 id)
        {
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.CLAN_AQ_ARQUIVO;
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
            Session["FileQueueCliente"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueCliente(FileQueue file)
        {
            // Inicializa
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Recupera cliente
            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 50)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Copia arquivo para pasta
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;


            // Gravar registro
            try
            {
                CLIENTE_ANEXO foto = new CLIENTE_ANEXO();
                foto.CLAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.CLAN_DT_ANEXO = DateTime.Today;
                foto.CLAN_IN_ATIVO = 1;
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
                foto.CLAN_IN_TIPO = tipo;
                foto.CLAN_NM_TITULO = fileName;
                foto.CLIE_CD_ID = item.CLIE_CD_ID;
                item.CLIENTE_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                return RedirectToAction("VoltarAnexoCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileCliente(HttpPostedFileBase file)
        {
            // Inicializa
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Recupera cliente
            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Copia arquivo
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                CLIENTE_ANEXO foto = new CLIENTE_ANEXO();
                foto.CLAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.CLAN_DT_ANEXO = DateTime.Today;
                foto.CLAN_IN_ATIVO = 1;
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
                else if (extensao.ToUpper() == ".DOCX" || extensao.ToUpper() == ".DOC" || extensao.ToUpper() == ".ODT" )
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
                foto.CLAN_IN_TIPO = tipo;
                foto.CLAN_NM_TITULO = fileName;
                foto.CLIE_CD_ID = item.CLIE_CD_ID;

                item.CLIENTE_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                Session["NivelCliente"] = 2;
                return RedirectToAction("VoltarAnexoCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoQueueCliente(FileQueue file)
        {
            // Inicializa
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Recupera cliente
            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Copia imagem
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.CLIE_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            listaMaster = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public ActionResult UploadFotoCliente(HttpPostedFileBase file)
        {
            // Inicializa            
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Recupera cliente
            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }

            // Copia imagem
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.CLIE_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            listaMaster = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            Session["NivelCliente"] = 2;
            return RedirectToAction("VoltarAnexoCliente");
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
                hash.Add("CLIE_NM_ENDERECO", end.Address);
                hash.Add("CLIE_NR_NUMERO", end.Complement);
                hash.Add("CLIE_NM_BAIRRO", end.District);
                hash.Add("CLIE_NM_CIDADE", end.City);
                hash.Add("CLIE_SG_UF", end.Uf);
                hash.Add("UF_CD_ID", baseApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("CLIE_NR_CEP", cep);
            }
            else if (tipoEnd == 2)
            {
                hash.Add("CLIE_NM_ENDERECO_ENTREGA", end.Address);
                hash.Add("CLIE_NR_NUMERO_ENTREGA", end.Complement);
                hash.Add("CLIE_NM_BAIRRO_ENTREGA", end.District);
                hash.Add("CLIE_NM_CIDADE_ENTREGA", end.City);
                hash.Add("CLIE_SG_UF_ENTREGA", end.Uf);
                hash.Add("UF_CD_ID_ENTREGA", baseApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("CLIE_NR_CEP_ENTREGA", cep);
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public ActionResult PesquisaCEPEntrega(ClienteViewModel itemVolta)
        {
            // Chama servico ECT
            CLIENTE cli = baseApp.GetItemById((Int32)Session["IdCliente"]);
            ClienteViewModel item = Mapper.Map<CLIENTE, ClienteViewModel>(cli);

            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            String cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(itemVolta.CLIE_NR_CEP_BUSCA);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza            
            item.CLIE_NM_ENDERECO_ENTREGA = end.Address + "/" + end.Complement;
            item.CLIE_NM_BAIRRO_ENTREGA = end.District;
            item.CLIE_NM_CIDADE_ENTREGA = end.City;
            item.CLIE_SG_UF_ENTREGA = end.Uf;
            item.CLIE_UF_CD_ENTREGA = baseApp.GetUFbySigla(end.Uf).UF_CD_ID;
            item.CLIE_NR_CEP_ENTREGA = itemVolta.CLIE_NR_CEP_BUSCA;

            // Retorna
            Session["VoltaCEP"] = 2;
            Session["Cliente"] = Mapper.Map<ClienteViewModel, CLIENTE>(item);
            return RedirectToAction("BuscarCEPCliente2");
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            Session["NivelCliente"] = 3;
            Session["VoltaTela"] = 1;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            CLIENTE_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(ClienteContatoViewModel vm)
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
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateEditContato(item);

                    // Verifica retorno
                    Session["ClienteAlterada"] = 1;
                    Session["NivelCliente"] = 3;
                    return RedirectToAction("VoltarAnexoCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                CLIENTE_CONTATO item = baseApp.GetContatoById(id);
                objetoAntes = (CLIENTE)Session["Cliente"];
                item.CLCO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditContato(item);
                Session["ClienteAlterada"] = 1;
                Session["NivelCliente"] = 3;
                return RedirectToAction("VoltarAnexoCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                CLIENTE_CONTATO item = baseApp.GetContatoById(id);
                objetoAntes = (CLIENTE)Session["Cliente"];
                item.CLCO_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditContato(item);
                Session["ClienteAlterada"] = 1;
                Session["NivelCliente"] = 3;
                return RedirectToAction("VoltarAnexoCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirContato()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Prepara view
            Session["VoltaTela"] = 1;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            CLIENTE_CONTATO item = new CLIENTE_CONTATO();
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            vm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            vm.CLCO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(ClienteContatoViewModel vm)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    // Verifica retorno
                    Session["ClienteAlterada"] = 1;
                    Session["NivelCliente"] = 3;
                    return RedirectToAction("IncluirContato");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarReferencia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            ClienteReferenciaViewModel vm = Mapper.Map<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarReferencia(ClienteReferenciaViewModel vm)
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
                    CLIENTE_REFERENCIA item = Mapper.Map<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>(vm);
                    Int32 volta = baseApp.ValidateEditReferencia(item);

                    // Verifica retorno
                    Session["ClienteAlterada"] = 1;
                    Session["NivelCliente"] = 4;
                    return RedirectToAction("VoltarAnexoCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirReferencia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
                objetoAntes = (CLIENTE)Session["Cliente"];
                item.CLRE_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditReferencia(item);
                Session["ClienteAlterada"] = 1;
                Session["NivelCliente"] = 4;
                return RedirectToAction("VoltarAnexoCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarReferencia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
                objetoAntes = (CLIENTE)Session["Cliente"];
                item.CLRE_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditReferencia(item);
                Session["ClienteAlterada"] = 1;
                Session["NivelCliente"] = 4;
                return RedirectToAction("VoltarAnexoCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirReferencia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_REFERENCIA item = new CLIENTE_REFERENCIA();
            ClienteReferenciaViewModel vm = Mapper.Map<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>(item);
            vm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            vm.CLRE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirReferencia(ClienteReferenciaViewModel vm)
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
                    CLIENTE_REFERENCIA item = Mapper.Map<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>(vm);
                    Int32 volta = baseApp.ValidateCreateReferencia(item);
                    // Verifica retorno
                    Session["ClienteAlterada"] = 1;
                    Session["NivelCliente"] = 4;
                    return RedirectToAction("VoltarAnexoCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerClientesInativos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCliente"] = 5;
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Monta listas
            if (Session["ListaInativos"] == null || ((List<CLIENTE>)Session["ListaInativos"]).Count == 0)
            {
                List<CLIENTE> listaMaster = CarregaClienteAdm().Where(x => x.CLIE_IN_ATIVO == 0).ToList();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaInativos"] = listaMaster;
            }
            ViewBag.Listas = (List<CLIENTE>)Session["ListaInativos"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(CarregaCatCliente(), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Cliente"] = null;

            // Indicadores
            ViewBag.Clientes = ((List<CLIENTE>)Session["ListaInativos"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagens
            if (Session["MensClienteInativos"] != null)
            {
                if ((Int32)Session["MensCliente"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objeto = new CLIENTE();
            Session["VoltaCliente"] = 1;
            Session["NivelCliente"] = 1;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarInativos(CLIENTE item)
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
                Tuple<Int32, List<CLIENTE>, Boolean> volta = baseApp.ExecuteFilterTuple(item.CLIE_CD_ID, item.CACL_CD_ID, item.CLIE_NM_RAZAO, item.CLIE_NM_NOME, item.CLIE_NR_CPF, item.CLIE_NR_CNPJ, item.CLIE_NM_EMAIL, item.CLIE_NM_CIDADE, item.UF_CD_ID, item.CLIE_IN_ATIVO, null, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensCliente"] = 1;
                    return RedirectToAction("VerClientesInativos");
                }

                // Sucesso
                listaMaster = volta.Item2;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaCliente"] = listaMaster;
                return RedirectToAction("VerClientesInativos");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult VerGrupo(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaGrupo"] = 10;
            return RedirectToAction("EditarGrupo", "Grupo");
        }

        public ActionResult RetirarFiltroInativos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaInativos"] = null;
            return RedirectToAction("VerClientesInativos");
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
            String nomeRel = "ClienteLista" + "_" + data + ".pdf";
            List<CLIENTE> lista = (List<CLIENTE>)Session["ListaCliente"];
            CLIENTE filtro = (CLIENTE)Session["FiltroCliente"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            //PdfPTable table = new PdfPTable(5);
            PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            cell.Colspan = 1;
            Image image = null;
            image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Clientes - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 1;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Clientes selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CPF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CNPJ", meuFont))
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
            cell = new PdfPCell(new Paragraph("Telefone", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("UF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CLIENTE item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_CLIENTE.CACL_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CLIE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CLIE_NR_CPF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_CPF, meuFont))
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
                if (item.CLIE_NR_CNPJ != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_CNPJ, meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CLIE_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CLIE_NR_TELEFONE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_TELEFONE, meuFont))
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
                if (item.CLIE_NM_CIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NM_CIDADE, meuFont))
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
                if (item.UF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.UF.UF_SG_SIGLA, meuFont))
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
                if (filtro.CACL_CD_ID > 0)
                {
                    CATEGORIA_CLIENTE cat = ccApp.GetItemById(filtro.CACL_CD_ID.Value);
                    parametros += "Categoria: " + cat.CACL_NM_NOME;
                    ja = 1;
                }
                if (filtro.CLIE_CD_ID > 0)
                {
                    CLIENTE cli = baseApp.GetItemById(filtro.CLIE_CD_ID);
                    if (ja == 0)
                    {
                        parametros += "Nome: *" + cli.CLIE_NM_NOME + "*";
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: *" + cli.CLIE_NM_NOME + "*";
                    }
                }
                if (filtro.CLIE_NR_CPF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CPF: " + filtro.CLIE_NR_CPF;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CPF: " + filtro.CLIE_NR_CPF;
                    }
                }
                if (filtro.CLIE_NR_CNPJ != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CNPJ: " + filtro.CLIE_NR_CNPJ;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CNPJ: " + filtro.CLIE_NR_CNPJ;
                    }
                }
                if (filtro.CLIE_NM_EMAIL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "E-Mail: *" + filtro.CLIE_NM_EMAIL + "*";
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e E-Mail: *" + filtro.CLIE_NM_EMAIL + "*";
                    }
                }
                if (filtro.CLIE_NM_CIDADE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Cidade: *" + filtro.CLIE_NM_CIDADE + "*";
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Cidade: *" + filtro.CLIE_NM_CIDADE + "*";
                    }
                }
                if (filtro.UF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "UF: " + filtro.UF.UF_SG_SIGLA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e UF: " + filtro.UF.UF_SG_SIGLA;
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

            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE aten = baseApp.GetItemById((Int32)Session["IdCliente"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Cliente" + aten.CLIE_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            cell.Colspan = 1;
            Image image = null;
            image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente - Detalhes", meuFont2))
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

            try
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                Image imagemCliente = Image.GetInstance(Server.MapPath(aten.CLIE_AQ_FOTO));
                imagemCliente.ScaleAbsolute(50, 50);
                cell.AddElement(imagemCliente);
                table.AddCell(cell);
            }
            catch (Exception ex)
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                Image imagemCliente = Image.GetInstance(Server.MapPath("~/Images/a8.jpg"));
                imagemCliente.ScaleAbsolute(50, 50);
                cell.AddElement(imagemCliente);
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Tipo de Pessoa: " + aten.TIPO_PESSOA.TIPE_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_CLIENTE.CACL_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.CLIE_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Razão Social: " + aten.CLIE_NM_RAZAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CLIE_NR_CPF != null)
            {
                cell = new PdfPCell(new Paragraph("CPF: " + aten.CLIE_NR_CPF, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.CLIE_NR_CNPJ != null)
            {
                cell = new PdfPCell(new Paragraph("CNPJ: " + aten.CLIE_NR_CNPJ, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ins.Estadual: " + aten.CLIE_NR_INSCRICAO_ESTADUAL, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ins.Municipal: " + aten.CLIE_NR_INSCRICAO_MUNICIPAL, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Endereços
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Endereço Principal", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço: " + aten.CLIE_NM_ENDERECO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Número: " + aten.CLIE_NR_NUMERO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Complemento: " + aten.CLIE_NM_COMPLEMENTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Bairro: " + aten.CLIE_NM_BAIRRO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade: " + aten.CLIE_NM_CIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.UF != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("CEP: " + aten.CLIE_NR_CEP, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço de Entrega", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço: " + aten.CLIE_NM_ENDERECO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Número: " + aten.CLIE_NR_NUMERO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Complemento: " + aten.CLIE_NM_COMPLEMENTO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Bairro: " + aten.CLIE_NM_BAIRRO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade: " + aten.CLIE_NM_CIDADE_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.UF1 != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF1.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("CEP: " + aten.CLIE_NR_CEP_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);            
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Contatos
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Contatos", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("E-Mail: " + aten.CLIE_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("E-Mail DANFE: " + aten.CLIE_NM_EMAIL_DANFE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Redes Sociais: " + aten.CLIE_NM_REDES_SOCIAIS, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Website: " + aten.CLIE_NM_WEBSITE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Telefone: " + aten.CLIE_NR_TELEFONE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular: " + aten.CLIE_NR_CELULAR, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tel.Adicional: " + aten.CLIE_NR_TELEFONE_ADICIONAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Lista de Contatos
            if (aten.CLIENTE_CONTATO.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 100f, 120f, 100f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
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
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CLIENTE_CONTATO item in aten.CLIENTE_CONTATO)
                {
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_CARGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_TELEFONE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.CLCO_IN_ATIVO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Inativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados Pessoais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Pessoais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome do Pai: " + aten.CLIE_NM_PAI, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome da Mãe: " + aten.CLIE_NM_MAE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CLIE_DT_NASCIMENTO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Nascimento: " + aten.CLIE_DT_NASCIMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Nascimento: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.SEXO != null)
            {
                cell = new PdfPCell(new Paragraph("Sexo: " + aten.SEXO.SEXO_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Sexo: - ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Naturalidade: " + aten.CLIE_NM_NATURALIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("UF Naturalidade: " + aten.CLIE_SG_NATURALIADE_UF, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nacionalidade: " + aten.CLIE_NM_NACIONALIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.CLIE_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
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

            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult IncluirClienteRapido()
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

            // Prepara listas
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa().OrderBy(p => p.TIPE_NM_NOME), "TIPE_CD_ID", "TIPE_NM_NOME");
            Session["Cliente"] = null;
            ViewBag.Sexo = new SelectList(CarregaSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.TiposCont = new SelectList(CarregaTipoContribuinte(), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Oportunidade", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta", Value = "3" });
            status.Add(new SelectListItem() { Text = "Engajado", Value = "4" });
            status.Add(new SelectListItem() { Text = "Descartado", Value = "5" });
            status.Add(new SelectListItem() { Text = "Suspenso", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            // Trata mensagem
            if (Session["MensCliente"] != null)
            {
                if ((Int32)Session["MensCliente"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0021", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            Session["ClienteNovo"] = 0;
            Session["VoltaCliente"] = 3;
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CACL_CD_ID = 1;
            vm.CLIE_AQ_FOTO = "~/Images/icone_imagem.jpg";
            vm.CLIE_DT_CADASTRO = DateTime.Today.Date;
            vm.CLIE_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.CLIE_IN_STATUS = 1;
            vm.TIPE_CD_ID = 0;
            vm.EMPR_CD_ID = 3;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirClienteRapido(ClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa().OrderBy(p => p.TIPE_NM_NOME), "TIPE_CD_ID", "TIPE_NM_NOME");
            Session["Cliente"] = null;
            ViewBag.Sexo = new SelectList(CarregaSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.TiposCont = new SelectList(CarregaTipoContribuinte(), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Regimes = new SelectList(CarregaRegime(), "RETR_CD_ID", "RETR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Oportunidade", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta", Value = "3" });
            status.Add(new SelectListItem() { Text = "Engajado", Value = "4" });
            status.Add(new SelectListItem() { Text = "Descartado", Value = "5" });
            status.Add(new SelectListItem() { Text = "Suspenso", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCliente"] = 10;
                        return RedirectToAction("IncluirClienteRapido", "Cliente");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Atualiza cache
                    listaMaster = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirCliente"] = 1;
                    Session["ClienteNovo"] = item.CLIE_CD_ID;
                    Session["ClienteAlterada"] = 1;
                    Session["NivelCliente"] = 1;

                    // Atualiza CNPJ
                    if (item.TIPE_CD_ID == 2)
                    {
                        var lstQs = PesquisaCNPJ(item);
                        foreach (var qs in lstQs)
                        {
                            Int32 voltaQs = ccnpjApp.ValidateCreate(qs, usuario);
                        }
                    }

                    // Trata retorno
                    if ((Int32)Session["VoltaCliente"] == 3)
                    {
                        Session["VoltaCliente"] = 0;
                        return RedirectToAction("IncluirClienteRapido", "Cliente");
                    }
                    if ((Int32)Session["VoltaMensagem"] == 30)
                    {
                        return RedirectToAction("MontarTelaCliente", "Cliente");
                    }
                    return RedirectToAction("VoltarBaseCliente", "Cliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                vm.TIPE_CD_ID = 0;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EnviarEMailClienteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMsg"] = 2;
            return RedirectToAction("EnviarEMailCliente", new { id = (Int32)Session["IdCliente"] });
        }

        [HttpGet]
        public ActionResult EnviarSMSClienteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMsg"] = 2;
            return RedirectToAction("EnviarSMSCliente", new { id = (Int32)Session["IdCliente"] });
        }

        [HttpGet]
        public ActionResult EnviarEMailCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CLIENTE cont = baseApp.GetItemById(id);
            Session["Cliente"] = cont;
            ViewBag.Cliente = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CLIE_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CLIE_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailCliente(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioEMailCliente(vm, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["NivelCliente"] = 1;
                    return RedirectToAction("VoltarBaseCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            String erro = null;
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = (CLIENTE)Session["Cliente"];
            String status = "Succeeded";
            String iD = "xyz";

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.CLIE_NM_NOME + "</b>";

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
            mensagem.ASSUNTO = "Contato - " + cont.CLIE_NM_NOME;
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = cont.CLIE_NM_EMAIL;
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
                Tuple<EmailSendStatus, String, Boolean> voltaMail = CrossCutting.CommunicationAzurePackage.SendMail(mensagem, null);
                status = voltaMail.Item1.ToString();
                iD = voltaMail.Item2;
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Cliente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                env.CLIE_CD_ID = cont.CLIE_CD_ID;
                env.MEEN_IN_TIPO = 1;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN_EM_EMAIL_DESTINO = cont.CLIE_NM_EMAIL;
                env.MEEN_NM_ORIGEM = "Mensagem para Cliente";
                env.MEEN_TX_CORPO = vm.MENS_TX_TEXTO;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 2;
                env.MEEN_TX_CORPO_COMPLETO = emailBody;
                if (erro == null)
                {
                    env.MEEN_IN_ENTREGUE = 1;
                }
                else
                {
                    env.MEEN_IN_ENTREGUE = 0;
                    env.MEEN_TX_RETORNO = erro;
                }
                env.MEEN_SG_STATUS = status;
                env.MEEN_GU_ID_MENSAGEM = iD;
                env.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                Int32 volta5 = meApp.ValidateCreate(env);
                Session["MensCliente"] = 100;
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensCliente"] = 110;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CLIENTE item = baseApp.GetItemById(id);
            Session["Cliente"] = item;
            ViewBag.Cliente = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.CLIE_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.CLIE_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSCliente(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioSMSCliente(vm, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["NivelCliente"] = 1;
                    return RedirectToAction("VoltarBaseCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            String erro = null;
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = (CLIENTE)Session["Cliente"];

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). " + cont.CLIE_NM_NOME;

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
            String texto = cab + ". " + vm.MENS_TX_SMS + " " + rod;

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

            // Chama envio
            try
            {
                String listaDest = "55" + Regex.Replace(cont.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"RidolfiWeb\"}]}");
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
                erro = ex.Message;
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Cliente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }

            // Grava envio
            MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
            env.ASSI_CD_ID = idAss;
            env.USUA_CD_ID = usuario.USUA_CD_ID;
            env.CLIE_CD_ID = cont.CLIE_CD_ID;
            env.MEEN_IN_TIPO = 2;
            env.MEEN_DT_DATA_ENVIO = DateTime.Now;
            env.MEEN__CELULAR_DESTINO = cont.CLIE_NR_CELULAR;
            env.MEEN_NM_ORIGEM = "Mensagem para Cliente";
            env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
            env.MEEN_TX_CORPO_COMPLETO = texto;
            env.MEEN_IN_ANEXOS = 0;
            env.MEEN_IN_ATIVO = 1;
            env.MEEN_IN_ESCOPO = 2;
            env.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
            if (erro == null)
            {
                env.MEEN_IN_ENTREGUE = 1;
            }
            else
            {
                env.MEEN_IN_ENTREGUE = 0;
                env.MEEN_TX_RETORNO = erro;
            }
            Int32 volta5 = meApp.ValidateCreate(env);

            return 0;
        }

        public ActionResult MontarTelaIndicadoresCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega Clientes
            List<CLIENTE> forns = CarregaCliente();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                forns = forns.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Int32 fornNum = forns.Count;
            Int32 fornAtivos = forns.Where(p => p.CLIE_IN_ATIVO == 1).Count();

            Int32 fornProposta = 0;
            Int32 fornPendentes= 0;

            Session["FornNum"] = fornNum;
            Session["FornAtivos"] = fornAtivos;

            ViewBag.ClienteNum = fornNum;
            ViewBag.ClienteAtivos = fornAtivos;

            // Recupera dados do CRM 
            List<CLIENTE> props = forns.Where(p => p.CRM_PEDIDO_VENDA.Count > 0).ToList();
            List<CLIENTE> procs = forns.Where(p => p.CRM.Any(m => m.CRM1_IN_ATIVO == 2)).ToList();
            Session["FornProposta"] = props.Count;
            Session["FornPendentes"] = procs.Count;
            ViewBag.ClienteProposta = props.Count;
            ViewBag.ClienteProcesso = procs.Count;

            // Recupera clientes por UF
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            List<UF> ufs = CarregaUF();
            foreach (UF item in ufs)
            {
                Int32 num = forns.Where(p => p.UF_CD_ID == item.UF_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.UF_NM_NOME;
                    mod.Valor = num;
                    lista2.Add(mod);
                }
            }
            ViewBag.ListaClienteUF = lista2;
            Session["ListaClienteUF"] = lista2;

            // Recupera clientes por Cidade
            List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
            List<String> cids = forns.Where(m => m.CLIE_NM_CIDADE != null).Select(p => p.CLIE_NM_CIDADE).Distinct().ToList();
            foreach (String item in cids)
            {
                Int32 num = forns.Where(p => p.CLIE_NM_CIDADE == item).ToList().Count;
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome = item;
                mod.Valor = num;
                lista3.Add(mod);
            }
            ViewBag.ListaClienteCidade = lista3;
            Session["ListaClienteCidade"] = lista3;

            // Recupera Clientes por Categoria
            List<ModeloViewModel> lista4 = new List<ModeloViewModel>();
            List<CATEGORIA_CLIENTE> cats = CarregaCatCliente();
            foreach (CATEGORIA_CLIENTE item in cats)
            {
                Int32 num = forns.Where(p => p.CACL_CD_ID == item.CACL_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.CACL_NM_NOME;
                    mod.Valor = num;
                    lista4.Add(mod);
                }
            }
            ViewBag.ListaClienteCats = lista4;
            Session["ListaClienteCats"] = lista4;

            // Recupera Clientes x Pedidos Ativos
            List<CRM> listaCRM = CarregaCRM();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaCRM = listaCRM.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<Int32> cliPed = listaCRM.Where(m => m.CRM1_IN_ATIVO == 1).Select(p => p.CLIE_CD_ID).Distinct().ToList();
            ViewBag.ClienteProcesso = cliPed.Count;

            // Recupera Clientes x Pedidos cancelados
            cliPed = listaCRM.Where(m => m.CRM1_IN_ATIVO == 3).Select(p => p.CLIE_CD_ID).Distinct().ToList();
            ViewBag.ClienteProcessoCanc = cliPed.Count;

            // Recupera Clientes x Pedidos Encerrados
            cliPed = listaCRM.Where(m => m.CRM1_IN_ATIVO == 5).Select(p => p.CLIE_CD_ID).Distinct().ToList();
            ViewBag.ClienteProcessoEnc = cliPed.Count;

            // Recupera Clientes x Propostas ativas
            List<CRM_PEDIDO_VENDA> listaPedido = CarregaPedidoVenda();
            cliPed = listaPedido.Where(m => m.CRPV_IN_STATUS == 1 || m.CRPV_IN_STATUS == 2).Select(p => p.CLIE_CD_ID.Value).Distinct().ToList();
            ViewBag.ClientePedido = cliPed.Count;

            // Recupera Clientes x Propostas canceladas
            cliPed = listaPedido.Where(m => m.CRPV_IN_STATUS == 3 || m.CRPV_IN_STATUS == 4).Select(p => p.CLIE_CD_ID.Value).Distinct().ToList();
            ViewBag.ClientePedidoCanc = cliPed.Count;

            // Recupera Clientes x Propostas aprovadas
            cliPed = listaPedido.Where(m => m.CRPV_IN_STATUS == 5).Select(p => p.CLIE_CD_ID.Value).Distinct().ToList();
            ViewBag.ClientePedidoAprov = cliPed.Count;

            // Recupera clientes com mais pedidos
            List<CRM_PEDIDO_VENDA> peds = CarregaPedidoVenda();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                peds = peds.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<Int32> clies = peds.Select(p => p.CLIE_CD_ID.Value).Distinct().ToList();
            List<ModeloViewModel> lista9 = new List<ModeloViewModel>();
            foreach (Int32 item in clies)
            {
                Int32 num = peds.Where(p => p.CLIE_CD_ID == item).ToList().Count;
                if (num > 0)
                {
                    CLIENTE cliX = baseApp.GetItemById(item);
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = cliX.CLIE_NM_NOME;
                    mod.Valor = num;
                    mod.Valor1 = item;
                    lista9.Add(mod);
                }
            }

            lista9 = lista9.OrderByDescending(p => p.Valor).ToList();
            lista9 = lista9.Take(5).ToList();
            ViewBag.ListaClientePedido = lista9;
            Session["ListaClientePedido"] = lista9;

            // Clientes incluidos por data
            DateTime limite = DateTime.Today.Date.AddMonths(-12);
            List<DateTime> datas = forns.Select(p => p.CLIE_DT_CADASTRO.Date).Distinct().ToList();
            datas.Sort((i, j) => i.Date.CompareTo(j.Date));
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                if (item.Date > limite)
                {
                    Int32 conta = forns.Where(p => p.CLIE_DT_CADASTRO.Date == item).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.DataEmissao = item;
                    mod.Valor = conta;
                    lista.Add(mod);
                }
            }
            ViewBag.ListaClienteData = lista;
            Session["ListaDatasCliente"] = datas;
            Session["ListaClienteData"] = lista;

            // Clientes incluidos por mes
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
                        Int32 conta = forns.Where(p => p.CLIE_DT_CADASTRO.Date.Month == item.Month & p.CLIE_DT_CADASTRO.Date.Year == item.Year & p.CLIE_DT_CADASTRO > limite).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = mes;
                        mod.Valor = conta;
                        listaMes.Add(mod);
                        mesFeito = item.Month.ToString() + "/" + item.Year.ToString();
                    }
                }
            }
            ViewBag.ListaClienteMes = listaMes;
            Session["ListaClienteMes"] = listaMes;

            // Aniversariantes do dia
            forns = forns.Where(p => p.CLIE_DT_NASCIMENTO != null).ToList();
            List<ModeloViewModel> listaAniv = new List<ModeloViewModel>();
            List<CLIENTE> aniv = forns.Where(p => p.CLIE_DT_NASCIMENTO.Value.Month == DateTime.Today.Month & p.CLIE_DT_NASCIMENTO.Value.Day == DateTime.Today.Day).ToList();
            foreach (CLIENTE item in aniv)
            {
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome = item.CLIE_NM_NOME;
                mod.DataEmissao = item.CLIE_DT_NASCIMENTO.Value;
                mod.Valor1 = item.CLIE_CD_ID;
                listaAniv.Add(mod);
            }
            ViewBag.ListaClienteAnivDia = listaAniv;
            Session["ListaClienteAnivDia"] = listaAniv;
            Session["VoltaMsg"] = 99;
            return View(usuario);
        }

        public JsonResult GetDadosClienteDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaClienteData"];
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

        public JsonResult GetDadosClienteMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaClienteMes"];
            List<String> meses = new List<String>();
            List<Int32> valor = new List<Int32>();
            meses.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                meses.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("meses", meses);
            result.Add("valores", valor);
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

        public ActionResult IncluirCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCatCliente"] = 2;
            return RedirectToAction("IncluirCatCliente", "TabelaAuxiliar");
        }

        public ActionResult IncluirCatCliente1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCatCliente"] = 3;
            return RedirectToAction("IncluirCatCliente", "TabelaAuxiliar");
        }

        public void GerarClientePlanilha()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                // Carrega listas
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CATEGORIA_CLIENTE> lstCat = CarregaCatCliente();
                List<TIPO_PESSOA> lstTp = CarregaTipoPessoa();
                List<UF> lstUF = baseApp.GetAllUF();
                List<SEXO> lstSexo = baseApp.GetAllSexo();

                // Instruções de Preenchimento
                ExcelWorksheet ws1 = package.Workbook.Worksheets.Add("Informações dos Produtos");
                ws1.Cells["A1"].Value = "Informações de Preenchimento:";
                ws1.Cells["A2"].Value = "Preencher colunas obrigatórias marcadas com (*)";
                ws1.Cells["A3"].Value = "As colunas TIPO DE PESSOA, CATEGORIA, UF e GÊNERO devem ser selecionadas da lista em cada célula";
                ws1.Cells["A4"].Value = "As colunas não obrigatórias podem ser deixadas em branco";
                ws1.Cells["A5"].Value = "Se for um novo cliente ele será incluído no cadastro. A verificação é feita pelo código CPF/CNPJ";
                ws1.Cells["A6"].Value = "É obrigatório o preenchimento do CPF ou do CNPJ";
                ws1.Cells["A7"].Value = "Se for um cliente já cadastrado ele será alterado no cadastro";

                ws1.DefaultColWidth = 100;
                ws1.Cells[ws1.Dimension.Address].AutoFitColumns(100);
                using (ExcelRange rng = ws1.Cells["A1:A1"])
                {
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGoldenrod);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.Brown);
                    rng.Style.Font.Bold = true;
                    rng.Style.Locked = true;
                }

                using (ExcelRange rng = ws1.Cells["A2:A7"])
                {
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Beige);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.Green);
                    rng.Style.Locked = true;
                }

                //PREPARA WORKSHEET PARA LISTAS
                ExcelWorksheet HiddenWs = package.Workbook.Worksheets.Add("Hidden");
                HiddenWs.Cells["A9"].LoadFromCollection(lstCat.Where(x => x.CACL_NM_NOME != "").Select(x => x.CACL_NM_NOME));
                HiddenWs.Cells["B9"].LoadFromCollection(lstTp.Where(x => x.TIPE_NM_NOME != "").Select(x => x.TIPE_NM_NOME));
                HiddenWs.Cells["C9"].LoadFromCollection(lstUF.Where(x => x.UF_SG_SIGLA != "").Select(x => x.UF_SG_SIGLA));
                HiddenWs.Cells["D9"].LoadFromCollection(lstSexo.Where(x => x.SEXO_NM_NOME != "").Select(x => x.SEXO_NM_NOME));

                //PREPARA WORKSHEET DADOS GERAIS
                ws1.Cells["A9"].Value = "TIPO DE PESSOA*";
                ws1.Cells["B9"].Value = "CATEGORIA*";
                ws1.Cells["C9"].Value = "CPF";
                ws1.Cells["D9"].Value = "NOME*";
                ws1.Cells["E9"].Value = "RAZAO SOCIAL";
                ws1.Cells["F9"].Value = "CNPJ";
                ws1.Cells["G9"].Value = "INC. ESTADUAL";
                ws1.Cells["H9"].Value = "INC. MUNICIPAL";
                ws1.Cells["I9"].Value = "CEP";
                ws1.Cells["J9"].Value = "ENDEREÇO";
                ws1.Cells["K9"].Value = "NUMERO";
                ws1.Cells["L9"].Value = "COMPLEMENTO";
                ws1.Cells["M9"].Value = "BAIRRO";
                ws1.Cells["N9"].Value = "CIDADE";
                ws1.Cells["O9"].Value = "UF";
                ws1.Cells["P9"].Value = "E-MAIL*";
                ws1.Cells["Q9"].Value = "TELEFONE";
                ws1.Cells["R9"].Value = "CELULAR";
                ws1.Cells["S9"].Value = "DATA NASC.";
                ws1.Cells["T9"].Value = "NATURALIDADE";
                ws1.Cells["U9"].Value = "GÊNERO";

                ws1.DefaultColWidth = 30;
                ws1.Cells[ws1.Dimension.Address].AutoFitColumns(30);
                using (ExcelRange rng = ws1.Cells["A9:U9"])
                {
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSeaGreen);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Locked = true;
                }

                using (ExcelRange rng = ws1.Cells["A10:U210"])
                {
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gainsboro);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Hair;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Hair;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Hair;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                }

                var listTpWs1 = ws1.DataValidations.AddListValidation("A10:A210");
                var listCatWs1 = ws1.DataValidations.AddListValidation("B10:B210");
                var listUFWs1 = ws1.DataValidations.AddListValidation("O10:O210");
                var listSexoWs1 = ws1.DataValidations.AddListValidation("U10:U210");

                listTpWs1.Formula.ExcelFormula = "Hidden!$B$1:$B$" + lstTp.Count.ToString();
                listCatWs1.Formula.ExcelFormula = "Hidden!$A$1:$A$" + lstCat.Count.ToString();
                listUFWs1.Formula.ExcelFormula = "Hidden!$C$1:$C$" + lstUF.Count.ToString();
                listSexoWs1.Formula.ExcelFormula = "Hidden!$D$1:$D$" + lstSexo.Count.ToString();

                HiddenWs.Hidden = eWorkSheetHidden.Hidden;
                Response.Clear();
                Response.ContentType = "application/xlsx";
                Response.AddHeader("content-disposition", "attachment; filename=TemplateCliente.xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
                Response.End();
            }
        }

        [HttpGet]
        public ActionResult ImportarPlanilhaCliente()
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

            // Recupera numero da falha
            Int32? numfalha = 0;
            listaMasterFalha = baseApp.GetAllFalhas(idAss).OrderByDescending(p => p.CLFA_IN_NUMERO).ToList();
            if (listaMasterFalha.Count == 0)
            {
                numfalha = conf.CONF_IN_FALHA_IMPORTACAO;
            }
            else
            {
                numfalha = listaMasterFalha.FirstOrDefault().CLFA_IN_NUMERO + 1;
            }

            // Processa planilha
            using (var pkg = new ExcelPackage(file.InputStream))
            {
                // Inicialização
                ExcelWorksheet wsGeral = pkg.Workbook.Worksheets[1];
                var wsFinalRow = wsGeral.Dimension.End;

                // Listas de pesquisas
                List<TIPO_PESSOA> tp = baseApp.GetAllTiposPessoa();
                List<CATEGORIA_CLIENTE> cats = baseApp.GetAllTipos(idAss);
                List<UF> ufs = baseApp.GetAllUF();

                TIPO_PESSOA tpCheca = new TIPO_PESSOA();
                CATEGORIA_CLIENTE catCheca = new CATEGORIA_CLIENTE();
                UF ufCheca = new UF();
                CLIENTE cliCheca = new CLIENTE();

                // Processa Planilha
                for (int row = 10; row < wsFinalRow.Row; row++)
                {
                    try
                    {
                        // Inicialização
                        Int32 check = 0;
                        Int32 volta = 0;
                        tpCheca = null;
                        catCheca = null;
                        ufCheca = null;
                        cliCheca = null;

                        // Recupera colunas
                        String pessoa = null;
                        if (wsGeral.Cells[row, 1].Value != null)
                        {
                            pessoa = wsGeral.Cells[row, 1].Value.ToString();
                        }
                        String cat = null;
                        if (wsGeral.Cells[row, 2].Value != null)
                        {
                            cat = wsGeral.Cells[row, 2].Value.ToString();
                        }
                        String cpf = null;
                        if (wsGeral.Cells[row, 3].Value != null)
                        {
                            cpf = wsGeral.Cells[row, 3].Value.ToString();
                        }
                        String nome = null;
                        if (wsGeral.Cells[row, 4].Value != null)
                        {
                            nome = wsGeral.Cells[row, 4].Value.ToString();
                        }
                        String razao = null;
                        if (wsGeral.Cells[row, 5].Value != null)
                        {
                            razao = wsGeral.Cells[row, 5].Value.ToString();
                        }
                        String cnpj = null;
                        if (wsGeral.Cells[row, 6].Value != null)
                        {
                            cnpj = wsGeral.Cells[row, 6].Value.ToString();
                        }
                        String est = null;
                        if (wsGeral.Cells[row, 7].Value != null)
                        {
                            est = wsGeral.Cells[row, 7].Value.ToString();
                        }
                        String mun = null;
                        if (wsGeral.Cells[row, 8].Value != null)
                        {
                            mun = wsGeral.Cells[row, 8].Value.ToString();
                        }
                        String cep = null;
                        if (wsGeral.Cells[row, 9].Value != null)
                        {
                            cep = wsGeral.Cells[row, 9].Value.ToString();
                        }
                        String end = null;
                        if (wsGeral.Cells[row, 10].Value != null)
                        {
                            end = wsGeral.Cells[row, 10].Value.ToString();
                        }
                        String num = null;
                        if (wsGeral.Cells[row, 11].Value != null)
                        {
                            num = wsGeral.Cells[row, 11].Value.ToString();
                        }
                        String com = null;
                        if (wsGeral.Cells[row, 12].Value != null)
                        {
                            com = wsGeral.Cells[row, 12].Value.ToString();
                        }
                        String bai = null;
                        if (wsGeral.Cells[row, 13].Value != null)
                        {
                            bai = wsGeral.Cells[row, 13].Value.ToString();
                        }
                        String cid = null;
                        if (wsGeral.Cells[row, 14].Value != null)
                        {
                            cid = wsGeral.Cells[row, 14].Value.ToString();
                        }
                        String uf = null;
                        if (wsGeral.Cells[row, 15].Value != null)
                        {
                            uf = wsGeral.Cells[row, 15].Value.ToString();
                        }
                        String email = null;
                        if (wsGeral.Cells[row, 16].Value != null)
                        {
                            email = wsGeral.Cells[row, 16].Value.ToString();
                        }
                        String tel = null;
                        if (wsGeral.Cells[row, 17].Value != null)
                        {
                            tel = wsGeral.Cells[row, 17].Value.ToString();
                        }
                        String cel = null;
                        if (wsGeral.Cells[row, 18].Value != null)
                        {
                            cel = wsGeral.Cells[row, 18].Value.ToString();
                        }
                        String nasc = null;
                        if (wsGeral.Cells[row, 19].Value != null)
                        {
                            nasc = wsGeral.Cells[row, 19].Value.ToString();
                        }
                        String nat = null;
                        if (wsGeral.Cells[row, 20].Value != null)
                        {
                            nat = wsGeral.Cells[row, 20].Value.ToString();
                        }
                        String sexo = null;
                        if (wsGeral.Cells[row, 21].Value != null)
                        {
                            sexo = wsGeral.Cells[row, 21].Value.ToString();
                        }

                        // Verifica saida
                        if (pessoa == null & cat == null & nome == null)
                        {
                            break;
                        }

                        // Colunas codificadas
                        Int32? tipoPessoa = 0;
                        Int32? catCliente = 0;
                        Int32? ufCliente = 0;
                        Int32? sexoCliente = 0;

                        // Tipo de Pessoa
                        if (pessoa == null)
                        {
                            // Verifica nulo
                            CLIENTE_FALHA fal = new CLIENTE_FALHA();
                            fal.CLFA_IN_NUMERO = numfalha;
                            fal.ASSI_CD_ID = idAss;
                            fal.CLFA_NM_NOME = nome;
                            fal.CLFA_NR_CPF = cpf;
                            fal.CLFA_NR_CNPJ = cnpj;
                            fal.CLFA_DT_DATA = DateTime.Now;
                            fal.CLFA_DS_MOTIVO = "Tipo de pessoa não informado";
                            fal.USUA_CD_ID = user.USUA_CD_ID;
                            volta = baseApp.ValidateCreateFalha(fal);
                            falha++;
                            continue;
                        }
                        else
                        {
                            // Verifica se é PF ou PJ
                            if (pessoa.ToUpper().Contains("FÍSICA") || pessoa.ToUpper().Contains("FISICA") || pessoa == "1")
                            {
                                tipoPessoa = 1;
                            }
                            else if (pessoa.ToUpper().Contains("JURÍDICA") || pessoa.ToUpper().Contains("JURIDICA") || pessoa == "2")
                            {
                                tipoPessoa = 2;
                            }
                            else
                            {
                                CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                fal.CLFA_IN_NUMERO = numfalha;
                                fal.ASSI_CD_ID = idAss;
                                fal.CLFA_NM_NOME = nome;
                                fal.CLFA_NR_CPF = cpf;
                                fal.CLFA_NR_CNPJ = cnpj;
                                fal.CLFA_DT_DATA = DateTime.Now;
                                fal.CLFA_DS_MOTIVO = "Tipo de pessoa inválido";
                                fal.USUA_CD_ID = user.USUA_CD_ID;
                                volta = baseApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }
                        }

                        // Categoria
                        if (cat == null)
                        {
                            catCliente = cats.FirstOrDefault().CACL_CD_ID;
                        }
                        else
                        {
                            // Verifica existencia
                            catCheca = cats.Where(p => p.CACL_NM_NOME.ToUpper().Contains(cat.ToUpper())).FirstOrDefault();
                            if (catCheca == null)
                            {
                                CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                fal.CLFA_IN_NUMERO = numfalha;
                                fal.ASSI_CD_ID = idAss;
                                fal.CLFA_NM_NOME = nome;
                                fal.CLFA_NR_CPF = cpf;
                                fal.CLFA_NR_CNPJ = cnpj;
                                fal.CLFA_DT_DATA = DateTime.Now;
                                fal.CLFA_DS_MOTIVO = "Categoria inválida";
                                fal.USUA_CD_ID = user.USUA_CD_ID;
                                volta = baseApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }
                            catCliente = catCheca.CACL_CD_ID;
                        }

                        // CPF e CNPJ informados
                        if (cpf != null & cnpj != null)
                        {
                            CLIENTE_FALHA fal = new CLIENTE_FALHA();
                            fal.CLFA_IN_NUMERO = numfalha;
                            fal.ASSI_CD_ID = idAss;
                            fal.CLFA_NM_NOME = nome;
                            fal.CLFA_NR_CPF = cpf;
                            fal.CLFA_NR_CNPJ = cnpj;
                            fal.CLFA_DT_DATA = DateTime.Now;
                            fal.CLFA_DS_MOTIVO = "CPF e CNPJ informados simultâneamente";
                            fal.USUA_CD_ID = user.USUA_CD_ID;
                            volta = baseApp.ValidateCreateFalha(fal);
                            falha++;
                            continue;
                        }

                        // Consistência PF
                        if (pessoa.ToUpper().Contains("FÍSICA") || pessoa.ToUpper().Contains("FISICA") || pessoa == "1")
                        {
                            if (cpf != null)
                            {
                                if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(cpf))
                                {
                                    CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                    fal.CLFA_IN_NUMERO = numfalha;
                                    fal.ASSI_CD_ID = idAss;
                                    fal.CLFA_NM_NOME = nome;
                                    fal.CLFA_NR_CPF = cpf;
                                    fal.CLFA_DT_DATA = DateTime.Now;
                                    fal.CLFA_DS_MOTIVO = "Pessoa física com CPF inválido";
                                    fal.USUA_CD_ID = user.USUA_CD_ID;
                                    volta = baseApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;

                                }
                            }
                        }

                        // Consistência PJ
                        if (pessoa.ToUpper().Contains("JURÍDICA") || pessoa.ToUpper().Contains("JURIDICA") || pessoa == "2")
                        {
                            if (cnpj != null)
                            {
                                if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(cnpj))
                                {
                                    CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                    fal.CLFA_IN_NUMERO = numfalha;
                                    fal.ASSI_CD_ID = idAss;
                                    fal.CLFA_NM_NOME = nome;
                                    fal.CLFA_NR_CPF = cpf;
                                    fal.CLFA_DT_DATA = DateTime.Now;
                                    fal.CLFA_DS_MOTIVO = "Pessoa jurídica com CNPJ inválido";
                                    fal.USUA_CD_ID = user.USUA_CD_ID;
                                    volta = baseApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;

                                }
                            }
                        }

                        // Nome
                        if (nome == null)
                        {
                            CLIENTE_FALHA fal = new CLIENTE_FALHA();
                            fal.CLFA_IN_NUMERO = numfalha;
                            fal.ASSI_CD_ID = idAss;
                            fal.CLFA_NM_NOME = nome;
                            fal.CLFA_NR_CPF = cpf;
                            fal.CLFA_NR_CNPJ = cnpj;
                            fal.CLFA_DT_DATA = DateTime.Now;
                            fal.CLFA_DS_MOTIVO = "Nome não informado";
                            fal.USUA_CD_ID = user.USUA_CD_ID;
                            volta = baseApp.ValidateCreateFalha(fal);
                            falha++;
                            continue;
                        }

                        // Razão Social
                        if (razao != null & tipoPessoa == 1 )
                        {
                            razao = null;
                        }
                        if (tipoPessoa == 2 & razao == null)
                        {
                            razao = nome;
                        }

                        // E-Mail
                        if (email == null)
                        {
                            CLIENTE_FALHA fal = new CLIENTE_FALHA();
                            fal.CLFA_IN_NUMERO = numfalha;
                            fal.ASSI_CD_ID = idAss;
                            fal.CLFA_NM_NOME = nome;
                            fal.CLFA_NR_CPF = cpf;
                            fal.CLFA_NR_CNPJ = cnpj;
                            fal.CLFA_DT_DATA = DateTime.Now;
                            fal.CLFA_DS_MOTIVO = "E-Mail não informado";
                            fal.USUA_CD_ID = user.USUA_CD_ID;
                            volta = baseApp.ValidateCreateFalha(fal);
                            falha++;
                            continue;
                        }
                        if (!CrossCutting.ValidarItensDiversos.IsValidEmail(email))
                        {
                            CLIENTE_FALHA fal = new CLIENTE_FALHA();
                            fal.CLFA_IN_NUMERO = numfalha;
                            fal.ASSI_CD_ID = idAss;
                            fal.CLFA_NM_NOME = nome;
                            fal.CLFA_NR_CPF = cpf;
                            fal.CLFA_NR_CNPJ = cnpj;
                            fal.CLFA_DT_DATA = DateTime.Now;
                            fal.CLFA_DS_MOTIVO = "E-Mail inválido";
                            fal.USUA_CD_ID = user.USUA_CD_ID;
                            volta = baseApp.ValidateCreateFalha(fal);
                            falha++;
                            continue;
                        }

                        // Celular
                        if (cel == null)
                        {
                            CLIENTE_FALHA fal = new CLIENTE_FALHA();
                            fal.CLFA_IN_NUMERO = numfalha;
                            fal.ASSI_CD_ID = idAss;
                            fal.CLFA_NM_NOME = nome;
                            fal.CLFA_NR_CPF = cpf;
                            fal.CLFA_NR_CNPJ = cnpj;
                            fal.CLFA_DT_DATA = DateTime.Now;
                            fal.CLFA_DS_MOTIVO = "Celular não informado";
                            fal.USUA_CD_ID = user.USUA_CD_ID;
                            volta = baseApp.ValidateCreateFalha(fal);
                            falha++;
                            continue;
                        }

                        // CEP
                        if (cep != null)
                        {
                            if (!CrossCutting.ValidarItensDiversos.IsValidCep(cep))
                            {
                                CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                fal.CLFA_IN_NUMERO = numfalha;
                                fal.ASSI_CD_ID = idAss;
                                fal.CLFA_NM_NOME = nome;
                                fal.CLFA_NR_CPF = cpf;
                                fal.CLFA_NR_CNPJ = cnpj;
                                fal.CLFA_DT_DATA = DateTime.Now;
                                fal.CLFA_DS_MOTIVO = "CEP inválido";
                                fal.USUA_CD_ID = user.USUA_CD_ID;
                                volta = baseApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }
                        }

                        // UF
                        if (uf != null)
                        {
                            ufCheca = ufs.Where(p => p.UF_SG_SIGLA == uf).FirstOrDefault();
                            if (ufCheca == null)
                            {
                                CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                fal.CLFA_IN_NUMERO = numfalha;
                                fal.ASSI_CD_ID = idAss;
                                fal.CLFA_NM_NOME = nome;
                                fal.CLFA_NR_CPF = cpf;
                                fal.CLFA_NR_CNPJ = cnpj;
                                fal.CLFA_DT_DATA = DateTime.Now;
                                fal.CLFA_DS_MOTIVO = "UF inválida";
                                fal.USUA_CD_ID = user.USUA_CD_ID;
                                volta = baseApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }
                            ufCliente = ufCheca.UF_CD_ID;

                        }
                        else
                        {
                            ufCliente = null;
                        }

                        // Data Nascimento
                        if (nasc != null)
                        {
                            DateTime dateTime;
                            if (!DateTime.TryParse(nasc, out dateTime))
                            {
                                CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                fal.CLFA_IN_NUMERO = numfalha;
                                fal.ASSI_CD_ID = idAss;
                                fal.CLFA_NM_NOME = nome;
                                fal.CLFA_NR_CPF = cpf;
                                fal.CLFA_NR_CNPJ = cnpj;
                                fal.CLFA_DT_DATA = DateTime.Now;
                                fal.CLFA_DS_MOTIVO = "Data de nascimento inválida";
                                fal.USUA_CD_ID = user.USUA_CD_ID;
                                volta = baseApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }
                        }

                        // Sexo
                        if (pessoa == "2")
                        {
                            if (sexo != null)
                            {
                                if (sexo.ToUpper().Contains("MAS") || sexo.ToUpper() == "M" || sexo == "1")
                                {
                                    sexoCliente = 1;
                                }
                                else if (sexo.ToUpper().Contains("FEM") || sexo.ToUpper() == "F" || sexo == "2")
                                {
                                    sexoCliente = 2;
                                }
                                else
                                {
                                    CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                    fal.CLFA_IN_NUMERO = numfalha;
                                    fal.ASSI_CD_ID = idAss;
                                    fal.CLFA_NM_NOME = nome;
                                    fal.CLFA_NR_CPF = cpf;
                                    fal.CLFA_NR_CNPJ = cnpj;
                                    fal.CLFA_DT_DATA = DateTime.Now;
                                    fal.CLFA_DS_MOTIVO = "Sexo inválido";
                                    fal.USUA_CD_ID = user.USUA_CD_ID;
                                    volta = baseApp.ValidateCreateFalha(fal);
                                    falha++;
                                    continue;
                                }
                            }
                            else
                            {
                                sexoCliente = null;
                            }
                        }

                        // Acerta letras
                        if (cid != null)
                        {
                            cid = CommonHelpers.ToPascalCase(cid);
                        }
                        if (end != null)
                        {
                            end = CommonHelpers.ToPascalCase(end);
                        }
                        if (bai != null)
                        {
                            bai = CommonHelpers.ToPascalCase(bai);
                        }
                        if (razao != null)
                        {
                            razao = CommonHelpers.ToPascalCase(razao);
                        }

                        // Verifica existencia
                        if (cpf != null || cnpj != null)
                        {
                            cliCheca = baseApp.CheckExistDoctos(cpf, cnpj, idAss);
                            if (cliCheca != null)
                            {
                                CLIENTE_FALHA fal = new CLIENTE_FALHA();
                                fal.CLFA_IN_NUMERO = numfalha;
                                fal.ASSI_CD_ID = idAss;
                                fal.CLFA_NM_NOME = nome;
                                fal.CLFA_NR_CPF = cpf;
                                fal.CLFA_NR_CNPJ = cnpj;
                                fal.CLFA_DT_DATA = DateTime.Now;
                                fal.CLFA_DS_MOTIVO = "CPF/CNPJ já cadastrado";
                                fal.USUA_CD_ID = user.USUA_CD_ID;
                                volta = baseApp.ValidateCreateFalha(fal);
                                falha++;
                                continue;
                            }
                        }

                        // Monta objeto
                        CLIENTE cliente = new CLIENTE();
                        cliente.ASSI_CD_ID = idAss;
                        cliente.CACL_CD_ID = catCliente;
                        cliente.CLIE_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                        cliente.CLIE_DT_CADASTRO = DateTime.Today.Date;
                        cliente.CLIE_DT_NASCIMENTO = Convert.ToDateTime(nasc);
                        cliente.CLIE_IN_ATIVO = 1;
                        cliente.CLIE_IN_STATUS = 1;
                        cliente.CLIE_IN_SEXO = sexoCliente;
                        cliente.CLIE_NM_BAIRRO = bai;
                        cliente.CLIE_NM_CIDADE = cid;
                        cliente.CLIE_NM_COMPLEMENTO = com;
                        cliente.CLIE_NM_EMAIL = email;
                        cliente.CLIE_NM_ENDERECO = end;
                        cliente.CLIE_NM_NATURALIDADE = nat;
                        cliente.CLIE_NM_NOME = nome;
                        cliente.CLIE_NM_RAZAO = razao;
                        cliente.CLIE_NR_CELULAR = cel;
                        cliente.CLIE_NR_CEP = cep;
                        cliente.CLIE_NR_CNPJ = cnpj;
                        cliente.CLIE_NR_CPF = cpf;
                        cliente.CLIE_NR_INSCRICAO_ESTADUAL = est;
                        cliente.CLIE_NR_INSCRICAO_MUNICIPAL = mun;
                        cliente.CLIE_NR_NUMERO = num;
                        cliente.CLIE_NR_TELEFONE = tel;
                        cliente.TIPE_CD_ID = tipoPessoa;
                        cliente.UF_CD_ID = ufCliente;
                        cliente.USUA_CD_ID = user.USUA_CD_ID;
                        cliente.EMPR_CD_ID = 3;

                        // Grava objeto
                        Int32 volta5 = baseApp.ValidateCreate(cliente, user);
                        conta++;

                        // Cria pastas
                        String caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + cliente.CLIE_CD_ID.ToString() + "/Fotos/";
                        Directory.CreateDirectory(Server.MapPath(caminho));
                        caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + cliente.CLIE_CD_ID.ToString() + "/Anexos/";
                        Directory.CreateDirectory(Server.MapPath(caminho));

                        Session["Conta"] = conta;
                        Session["Falha"] = falha;
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
                        Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    }
                }
                return Task.Delay(5);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ImportarPlanilhaCliente(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            using (var pkg = new ExcelPackage(file.InputStream))
            {
                // Inicialização
                ExcelWorksheet wsGeral = pkg.Workbook.Worksheets[1];
                var wsFinalRow = wsGeral.Dimension.End;
                if (wsFinalRow.Row > 2000)
                {
                    Session["MensCliente"] = 200;
                    return RedirectToAction("MontarTelaCliente");
                }
            }

            await ProcessaOperacaoPlanilha(file);

            // Finaliza
            Session["MensCliente"] = 99;
            Session["ListaCliente"] = null;
            Session["ClienteAlterada"] = 1;
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpGet]
        public ActionResult VerFalhasImportacao()
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
            if ((List<CLIENTE_FALHA>)Session["ListaFalha"] == null)
            {
                listaMasterFalha = baseApp.GetAllFalhas(idAss).Where(p => p.CLFA_DT_DATA == DateTime.Today.Date).ToList();
                if (listaMasterFalha.Count > 0)
                {
                    CLIENTE_FALHA falha = listaMasterFalha.OrderByDescending(p => p.CLFA_IN_NUMERO).ToList().FirstOrDefault();
                    Int32? num = falha.CLFA_IN_NUMERO;
                    listaMasterFalha = listaMasterFalha.Where(p => p.CLFA_IN_NUMERO == num).ToList();
                    Session["ListaFalha"] = listaMasterFalha;
                }
            }

            // Prepara lista
            List<USUARIO> listaTotal = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Listas = (List<CLIENTE_FALHA>)Session["ListaFalha"];

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objetoFalha = new CLIENTE_FALHA();
            objetoFalha.CLFA_DT_DATA = DateTime.Today.Date;
            objetoFalha.CLFA_DT_DUMMY = DateTime.Today.Date;
            return View(objetoFalha);
        }

        public ActionResult RetirarFiltroFalha()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFalha"] = null;
            return RedirectToAction("VerFalhasImportacao");
        }

        public ActionResult MostrarTodasFalha()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFalha = baseApp.GetAllFalhas(idAss).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMasterFalha = listaMasterFalha.Where(p => p.USUARIO.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaFalha"] = listaMasterFalha;
            return RedirectToAction("VerFalhasImportacao");
        }

        public ActionResult MostrarHojeFalha()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFalha = baseApp.GetAllFalhas(idAss).Where(p => p.CLFA_DT_DATA == DateTime.Today.Date).ToList();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                listaMasterFalha = listaMasterFalha.Where(p => p.USUARIO.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            Session["ListaFalha"] = listaMasterFalha;
            return RedirectToAction("VerFalhasImportacao");
        }

        [HttpPost]
        public ActionResult FiltrarFalha(CLIENTE_FALHA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();
                Int32 idAss = (Int32)Session["IdAssinante"];
                IQueryable<CLIENTE_FALHA> query = Db.CLIENTE_FALHA;
                List<CLIENTE_FALHA> lista = new List<CLIENTE_FALHA>();

                // Monta condição
                if (item.USUA_CD_ID > 0)
                {
                    query = query.Where(p => p.USUA_CD_ID == item.USUA_CD_ID);
                }
                if (item.CLFA_DT_DATA != DateTime.MinValue & item.CLFA_DT_DUMMY == DateTime.MinValue)
                {
                    query = query.Where(p => p.CLFA_DT_DATA.Value.Date >= item.CLFA_DT_DATA.Value.Date);
                }
                if (item.CLFA_DT_DATA == DateTime.MinValue & item.CLFA_DT_DUMMY != DateTime.MinValue)
                {
                    query = query.Where(p => p.CLFA_DT_DATA.Value.Date <= item.CLFA_DT_DUMMY.Value.Date);
                }
                if (item.CLFA_DT_DATA != DateTime.MinValue & item.CLFA_DT_DUMMY != DateTime.MinValue)
                {
                    query = query.Where(p => p.CLFA_DT_DATA.Value.Date >= item.CLFA_DT_DATA.Value.Date & p.CLFA_DT_DATA.Value.Date <= item.CLFA_DT_DUMMY.Value.Date);
                }
                if (item.CLFA_DS_MOTIVO != null)
                {
                    query = query.Where(p => p.CLFA_DS_MOTIVO.ToUpper().Contains(item.CLFA_DS_MOTIVO.ToUpper()));
                }

                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.OrderBy(a => a.CLFA_DT_DATA);
                    lista = query.ToList<CLIENTE_FALHA>();
                }

                // Sucesso
                listaMasterFalha = lista;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMasterFalha = listaMasterFalha.Where(p => p.USUARIO.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaFalha"] = listaMasterFalha;
                return RedirectToAction("VerFalhasImportacao");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirAnotacaoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 2;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            CLIENTE item = baseApp.GetItemById((Int32)Session["IdCliente"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            CLIENTE_ANOTACAO coment = new CLIENTE_ANOTACAO();
            ClienteAnotacaoViewModel vm = Mapper.Map<CLIENTE_ANOTACAO, ClienteAnotacaoViewModel>(coment);
            vm.CLAN_DT_ANOTACAO_HORA = DateTime.Now;
            vm.CLAN_DT_ANOTACAO = DateTime.Now;
            vm.CLAN_IN_ATIVO = 1;
            vm.CLIE_CD_ID = item.CLIE_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAnotacaoCliente(ClienteAnotacaoViewModel vm)
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
                    CLIENTE_ANOTACAO item = Mapper.Map<ClienteAnotacaoViewModel, CLIENTE_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CLIENTE not = baseApp.GetItemById((Int32)Session["IdCliente"]);

                    item.USUARIO = null;
                    not.CLIENTE_ANOTACAO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    Session["NivelCliente"] = 5;
                    return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAnotacaoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            Session["VoltaTela"] = 2;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            CLIENTE_ANOTACAO item = baseApp.GetAnotacaoById(id);
            CLIENTE cli = baseApp.GetItemById(item.CLIE_CD_ID);
            ClienteAnotacaoViewModel vm = Mapper.Map<CLIENTE_ANOTACAO, ClienteAnotacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoCliente(ClienteAnotacaoViewModel vm)
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
                    CLIENTE_ANOTACAO item = Mapper.Map<ClienteAnotacaoViewModel, CLIENTE_ANOTACAO>(vm);
                    Int32 volta = baseApp.ValidateEditAnotacao(item);

                    // Verifica retorno
                    Session["ClienteAlterada"] = 1;
                    Session["NivelCliente"] = 5;
                    return RedirectToAction("VoltarAnexoCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                Session["VoltaTela"] = 2;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                CLIENTE_ANOTACAO item = baseApp.GetAnotacaoById(id);
                item.CLAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnotacao(item);
                Session["ClienteAlterada"] = 1;
                Session["NivelCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaFiltroGenericoCliente()
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
            if ((List<CLIENTE>)Session["ListaClienteGeral"] == null || ((List<CLIENTE>)Session["ListaClienteGeral"]).Count == 0)
            {
                listaMaster = CarregaCliente();
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaClienteGeral"] = listaMaster;
            }
            ViewBag.Listas = (List<CLIENTE>)Session["ListaClienteGeral"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(CarregaCatCliente().OrderBy(p => p.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF().OrderBy(p => p.UF_NM_NOME), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.UFNat = new SelectList(CarregaUF().OrderBy(p => p.UF_NM_NOME), "UF_CD_ID", "UF_NM_NOME");
            List<USUARIO> usuarios = CarregaUsuario();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                usuarios = usuarios.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa().OrderBy(p => p.TIPE_NM_NOME), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            Session["Cliente"] = null;

            // Mensagens
            if (Session["MensCliente"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCliente"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Carrega Operadores
            List<SelectListItem> operaCats = new List<SelectListItem>();
            operaCats.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaCats.Add(new SelectListItem() { Text = "Diferente", Value = "2" });
            ViewBag.OperaCats = new SelectList(operaCats, "Value", "Text");
            List<SelectListItem> operaTipe = new List<SelectListItem>();
            operaTipe.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaTipe = new SelectList(operaTipe, "Value", "Text");
            List<SelectListItem> operaSexo = new List<SelectListItem>();
            operaSexo.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaSexo = new SelectList(operaSexo, "Value", "Text");
            List<SelectListItem> operaNome= new List<SelectListItem>();
            operaNome.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaNome.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            operaNome.Add(new SelectListItem() { Text = "Não Contém", Value = "4" });
            ViewBag.OperaNome = new SelectList(operaNome, "Value", "Text");
            List<SelectListItem> operaRazao = new List<SelectListItem>();
            operaRazao.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaRazao.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            operaRazao.Add(new SelectListItem() { Text = "Não Contém", Value = "4" });
            ViewBag.OperaRazao = new SelectList(operaRazao, "Value", "Text");
            List<SelectListItem> operaCPF = new List<SelectListItem>();
            operaCPF.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaCPF.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaCPF = new SelectList(operaCPF, "Value", "Text");
            List<SelectListItem> operaCNPJ = new List<SelectListItem>();
            operaCNPJ.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaCNPJ.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaCNPJ = new SelectList(operaCNPJ, "Value", "Text");
            List<SelectListItem> operaEmail= new List<SelectListItem>();
            operaEmail.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaEmail.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaEmail = new SelectList(operaEmail, "Value", "Text");
            List<SelectListItem> operaTelefone = new List<SelectListItem>();
            operaTelefone.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaTelefone.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaTelefone = new SelectList(operaTelefone, "Value", "Text");
            List<SelectListItem> operaCelular = new List<SelectListItem>();
            operaCelular.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaCelular.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaCelular = new SelectList(operaCelular, "Value", "Text");
            List<SelectListItem> operaCidade = new List<SelectListItem>();
            operaCidade.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaCidade.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            operaCidade.Add(new SelectListItem() { Text = "Não Contém", Value = "4" });
            ViewBag.OperaCidade = new SelectList(operaCidade, "Value", "Text");
            List<SelectListItem> operaCEP = new List<SelectListItem>();
            operaCEP.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaCEP = new SelectList(operaCEP, "Value", "Text");
            List<SelectListItem> operaUF = new List<SelectListItem>();
            operaUF.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaUF.Add(new SelectListItem() { Text = "Diferente", Value = "2" });
            ViewBag.OperaUF = new SelectList(operaUF, "Value", "Text");
            List<SelectListItem> operaIncEst = new List<SelectListItem>();
            operaIncEst.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaIncEst.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaIncEst = new SelectList(operaIncEst, "Value", "Text");
            List<SelectListItem> operaIncMun = new List<SelectListItem>();
            operaIncMun.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaIncMun.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaIncMun = new SelectList(operaIncMun, "Value", "Text");
            List<SelectListItem> operaDataNasc = new List<SelectListItem>();
            operaDataNasc.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaDataNasc.Add(new SelectListItem() { Text = "Entre", Value = "5" });
            ViewBag.OperaDataNasc = new SelectList(operaDataNasc, "Value", "Text");
            List<SelectListItem> operaNatur = new List<SelectListItem>();
            operaNatur.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaNatur.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaNatur = new SelectList(operaNatur, "Value", "Text");
            List<SelectListItem> operaUFNatur = new List<SelectListItem>();
            operaUFNatur.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            ViewBag.OperaUFNatur = new SelectList(operaUFNatur, "Value", "Text");
            List<SelectListItem> operaNacion = new List<SelectListItem>();
            operaNacion.Add(new SelectListItem() { Text = "Igual", Value = "1" });
            operaNacion.Add(new SelectListItem() { Text = "Contém", Value = "3" });
            ViewBag.OperaNacion = new SelectList(operaNacion, "Value", "Text");

            // Abre view
            Session["MensCliente"] = 0;
            Session["VoltaCliente"] = 1;
            Session["VoltaClienteCRM"] = 0;
            Session["VoltaCRM"] = 0;
            ClienteViewModel vm = new ClienteViewModel();
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

        [HttpPost]
        public ActionResult FiltrarClienteGenerico(ClienteViewModel item)
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
                List<CLIENTE> lista = new List<CLIENTE>();
                IQueryable<CLIENTE> query = Db.CLIENTE;

                // Categoria
                if (item.CACL_CD_ID != null & item.CACL_CD_ID > 0)
                {
                    if (item.OperaCategoria != null & item.OperaCategoria > 0)
                    {
                        if (item.OperaCategoria == 1)
                        {
                            query = query.Where(p => p.CACL_CD_ID == item.CACL_CD_ID);
                        }
                        if (item.OperaCategoria == 2)
                        {
                            query = query.Where(p => p.CACL_CD_ID != item.CACL_CD_ID);
                        }
                    }
                }

                // Tipo Pessoa
                if (item.TIPE_CD_ID != null & item.TIPE_CD_ID > 0)
                {
                    if (item.OperaTipe != null & item.OperaTipe > 0)
                    {
                        if (item.OperaTipe == 1)
                        {
                            query = query.Where(p => p.TIPE_CD_ID == item.TIPE_CD_ID);
                        }
                    }
                }

                // Sexo
                if (item.SEXO_CD_ID != null & item.SEXO_CD_ID > 0)
                {
                    if (item.OperaSexo != null & item.OperaSexo > 0)
                    {
                        if (item.OperaSexo == 1)
                        {
                            query = query.Where(p => p.SEXO_CD_ID == item.SEXO_CD_ID);
                        }
                    }
                }

                // Nome
                if (item.CLIE_NM_NOME != null)
                {
                    if (item.OperaNome != null & item.OperaNome > 0)
                    {
                        if (item.OperaNome == 1)
                        {
                            query = query.Where(p => p.CLIE_NM_NOME == item.CLIE_NM_NOME);
                        }
                        if (item.OperaNome == 3)
                        {
                            query = query.Where(p => p.CLIE_NM_NOME.Contains(item.CLIE_NM_NOME));
                        }
                        if (item.OperaNome == 4)
                        {
                            query = query.Where(p => !p.CLIE_NM_NOME.Contains(item.CLIE_NM_NOME));
                        }
                    }
                }

                // razao
                if (item.CLIE_NM_RAZAO != null)
                {
                    if (item.OperaRazao != null & item.OperaRazao > 0)
                    {
                        if (item.OperaRazao == 1)
                        {
                            query = query.Where(p => p.CLIE_NM_RAZAO == item.CLIE_NM_RAZAO);
                        }
                        if (item.OperaRazao == 3)
                        {
                            query = query.Where(p => p.CLIE_NM_RAZAO.Contains(item.CLIE_NM_RAZAO));
                        }
                        if (item.OperaRazao == 4)
                        {
                            query = query.Where(p => !p.CLIE_NM_RAZAO.Contains(item.CLIE_NM_RAZAO));
                        }
                    }
                }

                // CPF
                if (item.CLIE_NR_CPF != null)
                {
                    if (item.OperaCPF != null & item.OperaCPF > 0)
                    {
                        if (item.OperaCPF == 1)
                        {
                            query = query.Where(p => p.CLIE_NR_CPF == item.CLIE_NR_CPF);
                        }
                        if (item.OperaCPF == 3)
                        {
                            query = query.Where(p => p.CLIE_NR_CPF.Contains(item.CLIE_NR_CPF));
                        }
                    }
                }

                // CNPJ
                if (item.CLIE_NR_CNPJ != null)
                {
                    if (item.OperaCNPJ != null & item.OperaCNPJ > 0)
                    {
                        if (item.OperaCNPJ == 1)
                        {
                            query = query.Where(p => p.CLIE_NR_CNPJ == item.CLIE_NR_CNPJ);
                        }
                        if (item.OperaCNPJ == 3)
                        {
                            query = query.Where(p => p.CLIE_NR_CNPJ.Contains(item.CLIE_NR_CNPJ));
                        }
                    }
                }

                // E_Mail
                if (item.CLIE_NM_EMAIL != null)
                {
                    if (item.OperaEMail != null & item.OperaEMail > 0)
                    {
                        if (item.OperaEMail == 1)
                        {
                            query = query.Where(p => p.CLIE_NM_EMAIL == item.CLIE_NM_EMAIL);
                        }
                        if (item.OperaEMail == 3)
                        {
                            query = query.Where(p => p.CLIE_NM_EMAIL.Contains(item.CLIE_NM_EMAIL));
                        }
                    }
                }

                // Telefone
                if (item.CLIE_NR_TELEFONE != null)
                {
                    if (item.OperaTelefone != null & item.OperaTelefone > 0)
                    {
                        if (item.OperaTelefone == 1)
                        {
                            query = query.Where(p => p.CLIE_NR_TELEFONE == item.CLIE_NR_TELEFONE);
                        }
                        if (item.OperaTelefone == 3)
                        {
                            query = query.Where(p => p.CLIE_NR_TELEFONE.Contains(item.CLIE_NR_TELEFONE));
                        }
                    }
                }

                // Celular
                if (item.CLIE_NR_CELULAR != null)
                {
                    if (item.OperaCelular != null & item.OperaCelular > 0)
                    {
                        if (item.OperaCelular == 1)
                        {
                            query = query.Where(p => p.CLIE_NR_CELULAR == item.CLIE_NR_CELULAR);
                        }
                        if (item.OperaCelular == 3)
                        {
                            query = query.Where(p => p.CLIE_NR_CELULAR.Contains(item.CLIE_NR_CELULAR));
                        }
                    }
                }

                // Cidade
                if (item.CLIE_NM_CIDADE != null)
                {
                    if (item.OperaCidade != null & item.OperaCidade > 0)
                    {
                        if (item.OperaCidade == 1)
                        {
                            query = query.Where(p => p.CLIE_NM_CIDADE == item.CLIE_NM_CIDADE);
                        }
                        if (item.OperaCidade == 3)
                        {
                            query = query.Where(p => p.CLIE_NM_CIDADE.Contains(item.CLIE_NM_CIDADE));
                        }
                        if (item.OperaCidade == 4)
                        {
                            query = query.Where(p => !p.CLIE_NM_CIDADE.Contains(item.CLIE_NM_CIDADE));
                        }
                    }
                }

                // UF
                if (item.UF_CD_ID != null & item.UF_CD_ID > 0)
                {
                    if (item.OperaUF != null & item.OperaUF > 0)
                    {
                        if (item.OperaUF == 1)
                        {
                            query = query.Where(p => p.UF_CD_ID == item.UF_CD_ID);
                        }
                        if (item.OperaUF == 2)
                        {
                            query = query.Where(p => p.UF_CD_ID != item.UF_CD_ID);
                        }
                    }
                }

                // CEP
                if (item.CLIE_NR_CEP != null)
                {
                    if (item.OperaCEP != null & item.OperaCEP > 0)
                    {
                        if (item.OperaCEP == 1)
                        {
                            query = query.Where(p => p.CLIE_NR_CEP == item.CLIE_NR_CEP);
                        }
                    }
                }

                // Insc.Estadual
                if (item.CLIE_NR_INSCRICAO_ESTADUAL != null)
                {
                    if (item.OperaIncEst != null & item.OperaIncEst > 0)
                    {
                        if (item.OperaIncEst == 1)
                        {
                            query = query.Where(p => p.CLIE_NR_INSCRICAO_ESTADUAL == item.CLIE_NR_INSCRICAO_ESTADUAL);
                        }
                    }
                }

                // Insc.Municipal
                if (item.CLIE_NR_INSCRICAO_MUNICIPAL != null)
                {
                    if (item.OperaIncMun != null & item.OperaIncMun > 0)
                    {
                        if (item.OperaIncMun == 1)
                        {
                            query = query.Where(p => p.CLIE_NR_INSCRICAO_MUNICIPAL == item.CLIE_NR_INSCRICAO_MUNICIPAL);
                        }
                    }
                }

                // Naturalidade
                if (item.CLIE_NM_NATURALIDADE != null)
                {
                    if (item.OperaNatur != null & item.OperaNatur > 0)
                    {
                        if (item.OperaNatur == 1)
                        {
                            query = query.Where(p => p.CLIE_NM_NATURALIDADE == item.CLIE_NM_NATURALIDADE);
                        }
                        if (item.OperaNatur == 3)
                        {
                            query = query.Where(p => p.CLIE_NM_NATURALIDADE.Contains(item.CLIE_NM_NATURALIDADE));
                        }
                    }
                }

                // Nacionalidade
                if (item.CLIE_NM_NACIONALIDADE != null)
                {
                    if (item.OperaNatur != null & item.OperaNatur > 0)
                    {
                        if (item.OperaNatur == 1)
                        {
                            query = query.Where(p => p.CLIE_NM_NACIONALIDADE == item.CLIE_NM_NACIONALIDADE);
                        }
                        if (item.OperaNatur == 3)
                        {
                            query = query.Where(p => p.CLIE_NM_NACIONALIDADE.Contains(item.CLIE_NM_NACIONALIDADE));
                        }
                    }
                }

                // Data Nascimento
                if (item.CLIE_DT_NASCIMENTO != null & item.DATA_FINAL == null)
                {
                    query = query.Where(p => p.CLIE_DT_NASCIMENTO >= item.CLIE_DT_NASCIMENTO);
                }
                if (item.CLIE_DT_NASCIMENTO == null & item.DATA_FINAL != null)
                {
                    query = query.Where(p => p.CLIE_DT_NASCIMENTO <= item.DATA_FINAL);
                }
                if (item.CLIE_DT_NASCIMENTO != null & item.DATA_FINAL != null)
                {
                    query = query.Where(p => p.CLIE_DT_NASCIMENTO >= item.CLIE_DT_NASCIMENTO & p.CLIE_DT_NASCIMENTO <= item.DATA_FINAL);
                }

                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.OrderBy(a => a.CLIE_NM_NOME);
                    lista = query.ToList<CLIENTE>();
                }

                // Sucesso
                listaMaster = lista;
                if ((String)Session["PerfilUsuario"] != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
                }
                Session["ListaCliente"] = listaMaster;
                return RedirectToAction("MontarTelaCliente");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<CLIENTE> CarregaCliente()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> conf = new List<CLIENTE>();
            if (Session["Clientes"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["ClienteAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
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

        public List<CLIENTE> CarregaClienteUltimos(Int32 num)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> conf = new List<CLIENTE>();
            if (Session["Clientes"] == null)
            {
                conf = baseApp.GetAllItensUltimos(num, idAss);
            }
            else
            {
                if ((Int32)Session["ClienteAlterada"] == 1)
                {
                    conf = baseApp.GetAllItensUltimos(num, idAss);
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

        public List<CLIENTE> CarregaClienteAdm()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> conf = new List<CLIENTE>();
            if (Session["ClientesGeral"] == null)
            {
                conf = baseApp.GetAllItensAdm(idAss);
            }
            else
            {
                if ((Int32)Session["ClienteAlterada"] == 1)
                {
                    conf = baseApp.GetAllItensAdm(idAss);
                }
                else
                {
                    conf = (List<CLIENTE>)Session["ClientesGeral"];
                }
            }
            Session["ClientesGeral"] = conf;
            Session["ClienteAlterada"] = 0;
            return conf;
        }

        public List<CATEGORIA_CLIENTE> CarregaCatCliente()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CATEGORIA_CLIENTE> conf = new List<CATEGORIA_CLIENTE>();
            if (Session["CatClientes"] == null)
            {
                conf = baseApp.GetAllTipos(idAss);
            }
            else
            {
                if ((Int32)Session["CatClienteAlterada"] == 1)
                {
                    conf = baseApp.GetAllTipos(idAss);
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

        public List<TIPO_PESSOA> CarregaTipoPessoa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_PESSOA> conf = new List<TIPO_PESSOA>();
            if (Session["TipoPessoa"] == null)
            {
                conf = baseApp.GetAllTiposPessoa();
            }
            else
            {
                conf = (List<TIPO_PESSOA>)Session["TipoPessoa"];
            }
            Session["TipoPessoa"] = conf;
            return conf;
        }

        public List<TIPO_CONTRIBUINTE> CarregaTipoContribuinte()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_CONTRIBUINTE> conf = new List<TIPO_CONTRIBUINTE>();
            if (Session["TipoContribuintes"] == null)
            {
                conf = baseApp.GetAllContribuinte(idAss);
            }
            else
            {
                conf = (List<TIPO_CONTRIBUINTE>)Session["TipoContribuintes"];
            }
            Session["TipoContribuintes"] = conf;
            return conf;
        }

        public List<REGIME_TRIBUTARIO> CarregaRegime()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<REGIME_TRIBUTARIO> conf = new List<REGIME_TRIBUTARIO>();
            if (Session["RegimeTributarios"] == null)
            {
                conf = baseApp.GetAllRegimes(idAss);
            }
            else
            {
                conf = (List<REGIME_TRIBUTARIO>)Session["RegimeTributarios"];
            }
            Session["RegimeTributarios"] = conf;
            return conf;
        }

        public List<NACIONALIDADE> CarregaNacionalidade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<NACIONALIDADE> conf = new List<NACIONALIDADE>();
            if (Session["Nacionalidades"] == null)
            {
                conf = baseApp.GetAllNacionalidades();
            }
            else
            {
                conf = (List<NACIONALIDADE>)Session["Nacionalidades"];
            }
            Session["Nacionalidades"] = conf;
            return conf;
        }

        public List<MUNICIPIO> CarregaMunicipio()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MUNICIPIO> conf = new List<MUNICIPIO>();
            if (Session["Municipios"] == null)
            {
                conf = baseApp.GetAllMunicipios();
            }
            else
            {
                conf = (List<MUNICIPIO>)Session["Municipios"];
            }
            Session["Municipios"] = conf;
            return conf;
        }

        public List<SEXO> CarregaSexo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SEXO> conf = new List<SEXO>();
            if (Session["Sexos"] == null)
            {
                conf = baseApp.GetAllSexo();
            }
            else
            {
                conf = (List<SEXO>)Session["Sexos"];
            }
            Session["Sexos"] = conf;
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

        public ActionResult VerMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMensagensEnviadas"] = 1;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

        public ActionResult VerMensagensEnviadas1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMensagensEnviadas"] = 2;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

        public ActionResult AtualizarCategoriaCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CLIENTE cli = baseApp.GetItemById((Int32)Session["IdCliente"]);
            Int32 volta = baseApp.AtualizarCategoriaClienteCalculo(idAss, cli);
            return RedirectToAction("VoltarAnexoCliente", "Cliente");
        }

        public List<CRM_PEDIDO_VENDA> CarregaPedidoVenda()
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

        public List<CRM> CarregaCRM()
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

        public ActionResult EnviarMailAniversarioChama()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            Session["VoltaMsg"] = 99;
            return RedirectToAction("EnviarMailAniversario", "Cliente");
        }

        public ActionResult EnviarMailAniversarioChama1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            Session["VoltaMsg"] = 1;
            return RedirectToAction("EnviarMailAniversario", "Cliente");
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult EnviarMailAniversario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            List<CLIENTE> clientes = CarregaCliente();
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                clientes = clientes.Where(p => p.EMPR_CD_ID == (Int32)Session["IdEmpresa"]).ToList();
            }
            List<MensagemViewModel> listaAniv = new List<MensagemViewModel>();
            clientes = clientes.Where(p => p.CLIE_DT_NASCIMENTO != null).ToList();
            List<CLIENTE> aniv = clientes.Where(p => p.CLIE_DT_NASCIMENTO.Value.Month == DateTime.Today.Month & p.CLIE_DT_NASCIMENTO.Value.Day == DateTime.Today.Day).ToList();
            MensagemViewModel mensVolta = new MensagemViewModel();
            foreach (CLIENTE item in aniv)
            {
                if (item.CLIE_NM_EMAIL != null)
                {
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = item.CLIE_NM_NOME;
                    mens.ID = item.CLIE_CD_ID;
                    mens.MODELO = item.CLIE_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    listaAniv.Add(mens);
                }
            }
            ViewBag.ListaClienteAnivDia = listaAniv;
            Session["ListaClienteAnivDia"] = listaAniv;
            mensVolta.MENS_DT_CRIACAO = DateTime.Today.Date;
            mensVolta.MENS_IN_TIPO = 1;
            mensVolta.MENS_NM_CAMPANHA = "Mensagem de Aniversário";
            return View(mensVolta);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarMailAniversario(MensagemViewModel vm)
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
                    List<MensagemViewModel> lista = (List<MensagemViewModel>)Session["ListaClienteAnivDia"];
                    Int32 volta = ProcessaEnvioEMailClienteLista(vm, lista, usuarioLogado);

                    // Sucesso
                    Session["NivelCliente"] = 1;
                    return RedirectToAction("VoltarBaseCliente");
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailClienteLista(MensagemViewModel vm, List<MensagemViewModel> lista, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> listaCli = new List<CLIENTE>();
            String erro = null;
            Int32 volta = 0;
            Int32 totMens = 0;
            RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();
            Session["Erro"] = null;
            List<CLIENTE> nova = new List<CLIENTE>();
            ASSINANTE assi = assApp.GetItemById(idAss);

            // Monta lista de destinatarios
            Session["ListaClienteEMail"] = lista;

            // Monta token
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara texto
            String texto = vm.MENS_TX_TEXTO;
            String link = vm.MENS_NM_LINK;

            // Susbtitui texto
            texto = texto.Replace("{Assinante}", assi.ASSI_NM_NOME);

            // Prepara  link
            StringBuilder str = new StringBuilder();
            str.AppendLine(texto);
            if (!String.IsNullOrEmpty(link))
            {
                if (!link.Contains("www."))
                {
                    link = "www." + link;
                }
                if (!link.Contains("http://"))
                {
                    link = "http://" + link;
                }
                str.AppendLine(link + " Clique aqui para maiores informações");
                texto += "  " + link;
            }

            // Monta corpo
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            body = body.Replace("<p>", "");
            body = body.Replace("</p>", "<br />");
            String emailBody = body;
            Session["BodyEMail"] = body;

            // inicia processo
            String resposta = String.Empty;
            String status = "Succeeded";
            String iD = "xyz";

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            try
            {
                // Envio para grupo
                List<EmailAddress> emails = new List<EmailAddress>();
                String data = String.Empty;
                String json = String.Empty;
                List<AttachmentModel> models = new List<AttachmentModel>();
                models = null;

                // Checa se todos tem e-mail e monta lista
                foreach (MensagemViewModel cli in lista)
                {
                    String bodyA = body.Replace("{Nome}", cli.NOME);
                    totMens++;
                    EmailAddress add = new EmailAddress(
                            address: cli.MODELO,
                            displayName: cli.NOME);
                    emails.Add(add);
                }

                // Envio
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Campanha de Aniversário";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = "www@www.com";
                mensagem.NOME_EMISSOR_AZURE = emissor;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
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
                    Tuple<EmailSendStatus, String, Boolean> voltaMail = CrossCutting.CommunicationAzurePackage.SendMailList(mensagem, models, emails);
                    status = voltaMail.Item1.ToString();
                    iD = voltaMail.Item2;
                    Session["IdMail"] = iD;
                    Session["StatusMail"] = status;
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Cliente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }
                Session["TotMens"] = totMens;

                // Grava mensagem/destino e erros
                Session["Erro"] = erro;
                if (status == "Succeeded")
                {
                    foreach (MensagemViewModel item in lista)
                    {
                        MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                        env.ASSI_CD_ID = idAss;
                        env.USUA_CD_ID = usuario.USUA_CD_ID;
                        env.CLIE_CD_ID = item.ID;
                        env.MEEN_IN_TIPO = 1;
                        env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                        env.MEEN_EM_EMAIL_DESTINO = item.MODELO;
                        env.MEEN_NM_ORIGEM = "Mensagem para Cliente - Aniversário";
                        env.MEEN_TX_CORPO = vm.MENS_TX_TEXTO;
                        env.MEEN_IN_ANEXOS = 0;
                        env.MEEN_IN_ATIVO = 1;
                        env.MEEN_IN_ESCOPO = 2;
                        env.MEEN_TX_CORPO_COMPLETO = emailBody;
                        if (erro == null)
                        {
                            env.MEEN_IN_ENTREGUE = 1;
                        }
                        else
                        {
                            env.MEEN_IN_ENTREGUE = 0;
                            env.MEEN_TX_RETORNO = erro;
                        }
                        env.MEEN_SG_STATUS = status;
                        env.MEEN_GU_ID_MENSAGEM = iD;
                        env.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                        Int32 volta5 = meApp.ValidateCreate(env);
                        Session["MensCliente"] = 100;
                        Session["IdMail"] = iD;
                    }
                }
                else
                {
                    Session["MensCliente"] = 110;
                    Session["IdMail"] = iD;
                    Session["StatusMail"] = status;
                }
                erro = null;
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Cliente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Cliente", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }
            return 0;
        }
    }
}