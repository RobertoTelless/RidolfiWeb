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
    public class AssinanteController : Controller
    {
        private readonly IAssinanteAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IAssinanteCnpjAppService ccnpjApp;
        private readonly IClienteAppService cliApp;
        private readonly ICRMAppService crmApp;
        private readonly IMensagemAppService menApp;
        private readonly ITemplateAppService temApp;
        private readonly ICategoriaAgendaAppService caApp;
        private readonly ICategoriaClienteAppService ccApp;
        private readonly ICargoAppService cgApp;
        private readonly ICategoriaTelefoneAppService ctApp;
        private readonly ICRMOrigemAppService coApp;
        private readonly IFunilAppService fuApp;
        private readonly IMotivoCancelamentoAppService mcApp;
        private readonly IMotivoEncerramentoAppService meApp;
        private readonly IPeriodicidadeAppService peApp;
        private readonly ITemplateAppService teApp;
        private readonly INoticiaAppService ntApp;
        private readonly INotificacaoAppService noApp;
        private readonly ITipoAcaoAppService taApp;
        private readonly IVideoAppService viApp;
        private readonly ITipoTarefaAppService ttApp;
        private readonly IPlanoAppService plaApp;
        private readonly IMensagemEnviadaSistemaAppService mevApp;

        private String msg;
        private Exception exception;
        ASSINANTE objeto = new ASSINANTE();
        ASSINANTE objetoAntes = new ASSINANTE();
        List<ASSINANTE> listaMaster = new List<ASSINANTE>();
        String extensao;

        public AssinanteController(IAssinanteAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IAssinanteCnpjAppService ccnpjApps, IClienteAppService cliApps, ICRMAppService crmApps, IMensagemAppService menApps, ITemplateAppService temApps, ICategoriaAgendaAppService caApps, ICategoriaClienteAppService ccApps, ICargoAppService cgApps, ICategoriaTelefoneAppService ctApps, ICRMOrigemAppService coApps, IFunilAppService fuApps, IMotivoCancelamentoAppService mcApps, IMotivoEncerramentoAppService meApps, IPeriodicidadeAppService peApps, ITemplateAppService teApps, INoticiaAppService ntApps, INotificacaoAppService noApps, ITipoAcaoAppService taApps, IVideoAppService viApps, ITipoTarefaAppService ttApps, IPlanoAppService plaApps, IMensagemEnviadaSistemaAppService mevApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            ccnpjApp = ccnpjApps;  
            cliApp = cliApps;  
            crmApp = crmApps;
            menApp = menApps;
            temApp = temApps;
            caApp = caApps;
            ccApp = ccApps;
            cgApp = cgApps;   
            ctApp = ctApps; 
            coApp = coApps;
            fuApp = fuApps;
            mcApp = mcApps;
            meApp = meApps;
            peApp = peApps;
            teApp = teApps;
            ntApp = ntApps;
            noApp = noApps;
            taApp = taApps;
            viApp = viApps;
            ttApp = ttApps; 
            plaApp = plaApps;
            mevApp = mevApps;
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

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAssinantes", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
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

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<ASSINANTE> clientes = baseApp.GetAllItens();
            Session["Assi"] = clientes;

            if (nome != null)
            {
                List<ASSINANTE> lstCliente = clientes.Where(x => x.ASSI_NM_NOME != null && x.ASSI_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<ASSINANTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = clientes.Where(x => x.ASSI_NM_RAZAO_SOCIAL != null).ToList<ASSINANTE>();
                    lstCliente = lstCliente.Where(x => x.ASSI_NM_RAZAO_SOCIAL.ToLower().Contains(nome.ToLower())).ToList<ASSINANTE>();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.ASSI_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.ASSI_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.ASSI_NM_NOME + " (" + item.ASSI_NM_RAZAO_SOCIAL + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }

            return Json(listResult);
        }

        [HttpPost]
        public JsonResult PesquisaCNPJ(string cnpj)
        {
            List<ASSINANTE_QUADRO_SOCIETARIO> lstQs = new List<ASSINANTE_QUADRO_SOCIETARIO>();

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
                    ASSINANTE_QUADRO_SOCIETARIO qs = new ASSINANTE_QUADRO_SOCIETARIO();

                    qs.ASSINANTE = new ASSINANTE();
                    qs.ASSINANTE.ASSI_NM_RAZAO_SOCIAL = jObject["name"] == null ? String.Empty : jObject["name"].ToString();
                    qs.ASSINANTE.ASSI_NM_NOME = jObject["alias"] == null ? jObject["name"].ToString() : jObject["alias"].ToString();
                    qs.ASSINANTE.ASSI_NR_CEP = jObject["address"]["zip"].ToString();
                    qs.ASSINANTE.ASSI_NM_ENDERECO = jObject["address"]["street"].ToString();
                    qs.ASSINANTE.ASSI_NR_NUMERO = jObject["address"]["number"].ToString();
                    qs.ASSINANTE.ASSI_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                    qs.ASSINANTE.ASSI_NM_CIDADE = jObject["address"]["city"].ToString();
                    qs.ASSINANTE.UF_CD_ID = baseApp.GetAllUF().Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                    qs.ASSINANTE.ASSI_NR_TELEFONE = jObject["phone"].ToString();
                    qs.ASSINANTE.ASSI_NM_EMAIL = jObject["email"].ToString();
                    qs.ASQS_IN_ATIVO = 0;

                    lstQs.Add(qs);
                }
                else
                {
                    foreach (var s in jObject["membership"])
                    {
                        ASSINANTE_QUADRO_SOCIETARIO qs = new ASSINANTE_QUADRO_SOCIETARIO();

                        qs.ASSINANTE = new ASSINANTE();
                        qs.ASSINANTE.ASSI_NM_RAZAO_SOCIAL = jObject["name"].ToString() == "" ? String.Empty : jObject["name"].ToString();
                        qs.ASSINANTE.ASSI_NM_NOME = jObject["alias"].ToString() == "" ? jObject["name"].ToString() : jObject["alias"].ToString();
                        qs.ASSINANTE.ASSI_NR_CEP = jObject["address"]["zip"].ToString();
                        qs.ASSINANTE.ASSI_NM_ENDERECO = jObject["address"]["street"].ToString();
                        qs.ASSINANTE.ASSI_NR_NUMERO = jObject["address"]["number"].ToString();
                        qs.ASSINANTE.ASSI_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                        qs.ASSINANTE.ASSI_NM_CIDADE = jObject["address"]["city"].ToString();
                        qs.ASSINANTE.UF_CD_ID = baseApp.GetAllUF().Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                        qs.ASSINANTE.ASSI_NR_TELEFONE = jObject["phone"].ToString();
                        qs.ASSINANTE.ASSI_NM_EMAIL = jObject["email"].ToString();
                        qs.ASQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                        qs.ASQS_NM_NOME = s["name"].ToString();

                        // CNPJá não retorna esses valores
                        qs.ASQS_NM_PAIS_ORIGEM = String.Empty;
                        qs.ASQS_NM_REPRESENTANTE_LEGAL = String.Empty;
                        qs.ASQS_NM_QUALIFICACAO_REP_LEGAL = String.Empty;

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

        public List<ASSINANTE_QUADRO_SOCIETARIO> PesquisaCNPJ(ASSINANTE cliente)
        {
            List<ASSINANTE_QUADRO_SOCIETARIO> lstQs = new List<ASSINANTE_QUADRO_SOCIETARIO>();

            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(cliente.ASSI_NR_CNPJ, "[^0-9]", "");
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
                ASSINANTE_QUADRO_SOCIETARIO qs = new ASSINANTE_QUADRO_SOCIETARIO();

                qs.ASQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                qs.ASQS_NM_NOME = s["name"].ToString();
                qs.ASSI_CD_ID = cliente.ASSI_CD_ID;

                // CNPJá não retorna esses valores
                qs.ASQS_NM_PAIS_ORIGEM = String.Empty;
                qs.ASQS_NM_REPRESENTANTE_LEGAL = String.Empty;
                qs.ASQS_NM_QUALIFICACAO_REP_LEGAL = String.Empty;

                lstQs.Add(qs);
            }

            return lstQs;
        }

        [HttpGet]
        public ActionResult MontarTelaAssinante()
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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
            if ((List<ASSINANTE>)Session["ListaAssinante"] == null)
            {
                listaMaster = CarregaAssinante();
                Session["ListaAssinante"] = listaMaster;
            }
            ViewBag.Listas = (List<ASSINANTE>)Session["ListaAssinante"];
            ViewBag.Title = "Assi";
            ViewBag.Tipos = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Assi"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ativo.Add(new SelectListItem() { Text = "Bloqueado", Value = "2" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            Session["IncluirAssinante"] = 0;

            // Indicadores
            List<ASSINANTE> bloqueado = listaMaster.Where(p => p.ASSI_IN_STATUS == 2).ToList();
            Int32 bloqueados = bloqueado.Count;
            Int32 inativos = listaMaster.Where(p => p.ASSI_IN_STATUS == 0).ToList().Count;
            Int32 ativos = listaMaster.Where(p => p.ASSI_IN_STATUS == 1).ToList().Count;

            List<ASSINANTE_PAGAMENTO> pags = baseApp.GetAllPagamentos();
            pags = pags.Where(p => p.ASPA_IN_PAGO == 0 & p.ASPA_NR_ATRASO > 0).ToList();
            Int32 atraso = pags.Count;

            ViewBag.Bloqueados = bloqueados;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Inativos = inativos;
            ViewBag.NumAtivos = ativos;
            ViewBag.Atraso = atraso;

            // Recupera vencimentos
            List<ASSINANTE_PLANO> planosAss = baseApp.GetAllAssPlanos();
            List<ASSINANTE_PLANO> planosVencidos = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date).ToList();
            List<ASSINANTE_PLANO> planosVencer30 = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30)).ToList();
            Int32 vencidos = planosVencidos.Count;
            Int32 vencer30 = planosVencer30.Count;

            Session["Bloqueados"] = bloqueado;
            Session["PlanosVencidos"] = planosVencidos;
            Session["PlanosVencer30"] = planosVencer30;
            Session["Atrasos"] = pags;

            ViewBag.Vencidos = vencidos;
            ViewBag.Vencer30 = vencer30;
            ViewBag.NumBloqueados = bloqueados;
            ViewBag.NumAtrasos = atraso;

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensAssinante"] = null;
            Session["VoltaAssinante"] = 1;
            Session["VoltaMensAssinante"] = 0;
            objeto = new ASSINANTE();
            return View(objeto);
        }

        public ActionResult RetirarFiltroAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaAssinante"] = null;
            Session["FiltroAssinante"] = null;
            return RedirectToAction("MontarTelaAssinante");
        }

        public ActionResult MostrarTudoAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = CarregaAssinanteAdm();
            Session["FiltroAssinante"] = null;
            Session["ListaAssinante"] = listaMaster;
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpPost]
        public ActionResult FiltrarAssinante(ASSINANTE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ASSINANTE> listaObj = new List<ASSINANTE>();
                Session["FiltroAssinante"] = item;
                Tuple<Int32, List<ASSINANTE>, Boolean> volta = baseApp.ExecuteFilter(item.TIPE_CD_ID, item.ASSI_NM_NOME, item.ASSI_NR_CPF, item.ASSI_NR_CNPJ, item.ASSI_NM_CIDADE, item.UF_CD_ID, item.ASSI_IN_STATUS);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensAssinante"] = 1;
                    return RedirectToAction("MontarTelaAssinante");
                }

                // Sucesso
                Session["MensAssinante"] = 0;
                listaMaster = volta.Item2;
                Session["ListaAssinante"] = volta.Item2;
                return RedirectToAction("MontarTelaAssinante");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaAssinante");
        }

        public ActionResult VoltarMensagemAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaMensAssinante"] == 1)
            {
                return RedirectToAction("VoltarAnexoAssinante", "Assinante");
            }
            return RedirectToAction("MontarTelaAssinante");
        }

        [HttpGet]
        public ActionResult IncluirAssinante()
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");;
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");

            // Prepara view
            ASSINANTE item = new ASSINANTE();
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(item);
            vm.ASSI_DT_INICIO = DateTime.Today.Date;
            vm.ASSI_IN_ATIVO = 1;
            vm.TIPE_CD_ID = 0;
            vm.ASSI_IN_TIPO = 0;
            vm.ASSI_IN_STATUS = 1;
            vm.ASSI_IN_BLOQUEADO = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirAssinante(AssinanteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME"); ;
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Usuarios = new SelectList(CarregaUsuario(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAssinante"] = 3;
                        return RedirectToAction("MontarTelaAssinante", "Assinante");
                    }

                    // Carrega foto e processa alteracao
                    if (item.ASSI_AQ_FOTO == null)
                    {
                        item.ASSI_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                        volta = baseApp.ValidateEdit(item, item, usuario);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/Assinante/" + item.ASSI_CD_ID.ToString() + "/Foto/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/Assinante/" + item.ASSI_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<ASSINANTE>();
                    Session["ListaAssinante"] = null;
                    Session["IncluirAssinante"] = 1;

                    // Trata Anexos e Foto
                    Session["IdVolta"] = item.ASSI_CD_ID;
                    Session["IdAssi"] = item.ASSI_CD_ID;
                    Session["AssinanteAlterada"] = 1;
                    if (Session["FileQueueAssinante"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueAssinante"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueAssinante(file);
                            }
                            else
                            {
                                UploadFotoQueueAssinante(file);
                            }
                        }
                        Session["FileQueueAssinante"] = null;
                    }

                    // Cria ambiente
                    Int32 volta1 = CriaAmbienteNovoAssinante(item);
                    return RedirectToAction("VoltarAnexoAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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
        public ActionResult EditarAssinante(Int32 id)
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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

            // Prepara view
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME"); ;
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ativo.Add(new SelectListItem() { Text = "Bloqueado", Value = "2" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");

            // Recupera consumo
            Int32 numUsu = CarregaUsuario().Count;
            Int32 numCli = cliApp.GetAllItens(idAss).Count;
            Int32 numProc = crmApp.GetAllItens(idAss).Count;
            Int32 numAcoes = crmApp.GetAllAcoes(idAss).Count;
            Int32 numProps = crmApp.GetAllPedidos(idAss).Count;
            Int32 numEmail = menApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1).ToList().Count;
            Int32 numSMS = menApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2).ToList().Count;

            ViewBag.NumUsu = numUsu;
            ViewBag.NumCli = numCli;
            ViewBag.NumProc = numProc;
            ViewBag.NumAcoes = numAcoes;
            ViewBag.NumProps = numProps;
            ViewBag.NumEmail = numEmail;
            ViewBag.NumSMS = numSMS;

            // Recupera
            ASSINANTE item = baseApp.GetItemById(id);
            ViewBag.QuadroSoci = ccnpjApp.GetByCliente(item);

            // Recupera vencimentos
            List<ASSINANTE_PLANO> planos = item.ASSINANTE_PLANO.Where(p => p.ASPL_IN_ATIVO == 1).ToList();
            List<ASSINANTE_PLANO> planosVencidos = planos.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date).ToList();
            List<ASSINANTE_PLANO> planosVencer30 = planos.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30)).ToList();
            Int32 vencidos = planosVencidos.Count;
            Int32 vencer30 = planosVencer30.Count;

            Session["PlanosVencidos"] = planosVencidos;
            Session["PlanosVencer30"] = planosVencer30;
            ViewBag.Vencidos = vencidos;
            ViewBag.Vencer30 = vencer30;
            ViewBag.Planos = planos.Count;
            ViewBag.Usuarios = item.USUARIO.Where(p => p.USUA_IN_ATIVO == 1).Count();

            // Recupera parcelas em atraso
            List<ASSINANTE_PAGAMENTO> pags = baseApp.GetAllPagamentos();
            pags = pags.Where(p => p.ASPA_IN_PAGO == 0 & p.ASPA_NR_ATRASO > 0 & p.ASSI_CD_ID == id).ToList();
            Int32 atraso = pags.Count;
            ViewBag.Atrasos = pags.Count;

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 21)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0204", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0205", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 23)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0206", CultureInfo.CurrentCulture));
                }
            }

            // Indicadores

            // Sessões
            Session["MensAssinante"] = null;
            Session["VoltaAssinante"] = 1;
            objetoAntes = item;
            Session["Assi"] = item;
            Session["IdAssi"] = id;
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarAssinante(AssinanteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME"); ;
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ativo.Add(new SelectListItem() { Text = "Bloqueado", Value = "2" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["AssinanteAlterada"] = 1;
                    listaMaster = new List<ASSINANTE>();
                    Session["ListaAssinante"] = null;
                    return RedirectToAction("MontarTelaAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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
        public ActionResult ExcluirAssinante(Int32 id)
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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
            try
            {
                ASSINANTE item = baseApp.GetItemById(id);
                objetoAntes = (ASSINANTE)Session["Assi"];
                item.ASSI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                listaMaster = new List<ASSINANTE>();
                Session["ListaAssinante"] = null;
                Session["FiltroAssinante"] = null;
                Session["AssinanteAlterada"] = 1;
                return RedirectToAction("MontarTelaAssinante");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarAssinante(Int32 id)
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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
            try
            {
                ASSINANTE item = baseApp.GetItemById(id);
                objetoAntes = (ASSINANTE)Session["Assi"];
                item.ASSI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                listaMaster = new List<ASSINANTE>();
                Session["ListaAssinante"] = null;
                Session["FiltroAssinante"] = null;
                Session["AssinanteAlterada"] = 1;
                return RedirectToAction("MontarTelaAssinante");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoAssinante(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            ASSINANTE_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoAssinanteAudio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            ASSINANTE_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarAssinante", new { id = (Int32)Session["IdAssi"] });
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaAssinante"] == 1)
            {
                return RedirectToAction("MontarTelaAssinante", "Assinante");
            }
            if ((Int32)Session["VoltaAssinante"] == 2)
            {
                return RedirectToAction("MontarTelaDashboardAssinantes", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaDashboardAssinantes", "BaseAdmin");
        }

        public FileResult DownloadAssinante(Int32 id)
        {
            ASSINANTE_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.ASAN_AQ_ARQUIVO;
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
            Session["FileQueueAssinante"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueAssinante(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdAssi"];
            Int32 idAss = (Int32)Session["IdAssi"];

            if (file == null)
            {
                Session["MensAssinante"] = 5;
                return RedirectToAction("VoltarAnexoAssinante");
            }

            ASSINANTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 50)
            {
                Session["MensAssinante"] = 6;
                return RedirectToAction("VoltarAnexoAssinante");
            }
            String caminho = "/Imagens/Assinante/" + item.ASSI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                ASSINANTE_ANEXO foto = new ASSINANTE_ANEXO();
                foto.ASAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.ASAN_DT_ANEXO = DateTime.Today;
                foto.ASAN_IN_ATIVO = 1;
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
                foto.ASAN_IN_TIPO = tipo;
                foto.ASAN_NM_TITULO = fileName;
                foto.ASSI_CD_ID = item.ASSI_CD_ID;

                item.ASSINANTE_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
                return RedirectToAction("VoltarAnexoAssinante");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileAssinante(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdAssi"];
            Int32 idAss = (Int32)Session["IdAssi"];

            if (file == null)
            {
                Session["MensAssinante"] = 5;
                return RedirectToAction("VoltarAnexoAssinante");
            }

            ASSINANTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensAssinante"] = 6;
                return RedirectToAction("VoltarAnexoAssinante");
            }
            String caminho = "/Imagens/Assinante/" + item.ASSI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            try
            {
                ASSINANTE_ANEXO foto = new ASSINANTE_ANEXO();
                foto.ASAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.ASAN_DT_ANEXO = DateTime.Today;
                foto.ASAN_IN_ATIVO = 1;
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
                foto.ASAN_IN_TIPO = tipo;
                foto.ASAN_NM_TITULO = fileName;
                foto.ASSI_CD_ID = item.ASSI_CD_ID;

                item.ASSINANTE_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
                return RedirectToAction("VoltarAnexoAssinante");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoQueueAssinante(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdAssi"];
            Int32 idAss = (Int32)Session["IdAssi"];

            if (file == null)
            {
                Session["MensAssinante"] = 5;
                return RedirectToAction("VoltarAnexoAssinante");
            }
            ASSINANTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensAssinante"] = 6;
                return RedirectToAction("VoltarAnexoAssinante");
            }
            String caminho = "/Imagens/Assinante/" + item.ASSI_CD_ID.ToString() + "/Foto/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.ASSI_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
            listaMaster = new List<ASSINANTE>();
            Session["ListaAssinante"] = null;
            return RedirectToAction("VoltarAnexoAssinante");
        }

        [HttpPost]
        public ActionResult UploadFotoAssinante(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdAssi"];
            Int32 idAss = (Int32)Session["IdAssi"];

            if (file == null)
            {
                Session["MensAssinante"] = 5;
                return RedirectToAction("VoltarAnexoAssinante");
            }
            ASSINANTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensAssinante"] = 6;
                return RedirectToAction("VoltarAnexoAssinante");
            }
            String caminho = "/Imagens/Assinante/" + item.ASSI_CD_ID.ToString() + "/Foto/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.ASSI_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
            listaMaster = new List<ASSINANTE>();
            Session["ListaAssinante"] = null;
            return RedirectToAction("VoltarAnexoAssinante");
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
            hash.Add("ASSI_NM_ENDERECO", end.Address);
            hash.Add("ASSI_NR_NUMERO", end.Complement);
            hash.Add("ASSI_NM_BAIRRO", end.District);
            hash.Add("ASSI_NM_CIDADE", end.City);
            hash.Add("UF_CD_ID", baseApp.GetUFBySigla(end.Uf).UF_CD_ID);
            hash.Add("ASSI_NR_CEP", cep);

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        [HttpGet]
        public ActionResult EditarAssinantePlano(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Planos = new SelectList(CarregaPlano().OrderBy(p => p.PLAN_NM_NOME), "PLAN_CD_ID", "PLAN_NM_NOME");
            List<SelectListItem> preco = new List<SelectListItem>();
            preco.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            preco.Add(new SelectListItem() { Text = "Promoção", Value = "2" });
            ViewBag.Precos = new SelectList(preco, "Value", "Text");
            ASSINANTE_PLANO item = baseApp.GetPlanoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];
            AssinantePlanoViewModel vm = Mapper.Map<ASSINANTE_PLANO, AssinantePlanoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAssinantePlano(AssinantePlanoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Planos = new SelectList(baseApp.GetAllPlanos().OrderBy(p => p.PLAN_NM_NOME), "PLAN_CD_ID", "PLAN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    ASSINANTE_PLANO item = Mapper.Map<AssinantePlanoViewModel, ASSINANTE_PLANO>(vm);
                    Int32 volta = baseApp.ValidateEditPlano(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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
        public ActionResult ExcluirAssinantePlano(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ASSINANTE_PLANO item = baseApp.GetPlanoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];
            item.ASPL_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditPlano(item);

            // Desativa pagamentos
            ASSINANTE assi = baseApp.GetItemById(item.ASSI_CD_ID);
            PLANO plan = baseApp.GetPlanoBaseById(item.PLAN_CD_ID);
            List<ASSINANTE_PAGAMENTO> pags = assi.ASSINANTE_PAGAMENTO.Where(p => p.PLAN_CD_ID == item.PLAN_CD_ID).ToList();
            pags.ForEach(p => p.ASPA_IN_ATIVO = 0);
            Int32 volta1 = baseApp.ValidateEdit(assi, assi, usuario);

            // Envia e-mail ao assinante
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String texto = temApp.GetByCode("ASSPLANEXC").TEMP_TX_CORPO;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = assi.ASSI_NM_NOME;
            mens.ID = assi.ASSI_CD_ID;
            mens.MODELO = assi.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.MENS_NM_LINK = conf.CONF_LK_LINK_SISTEMA;
            mens.LINK = "1";
            texto = texto.Replace("{plano}", plan.PLAN_NM_NOME);
            texto = texto.Replace("{data}", item.ASPL_DT_INICIO.Value.ToShortDateString());
            mens.MENS_TX_TEXTO = texto;
            Int32 volta2 = ProcessaEnvioEMailAssinante(mens, usuario);

            return RedirectToAction("VoltarAnexoAssinante");
        }

        [HttpGet]
        public ActionResult ReativarAssinantePlano(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ASSINANTE_PLANO item = baseApp.GetPlanoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];
            item.ASPL_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditPlano(item);

            // Reativa pagamentos
            ASSINANTE assi = baseApp.GetItemById(item.ASSI_CD_ID);
            PLANO plan = baseApp.GetPlanoBaseById(item.PLAN_CD_ID);
            List<ASSINANTE_PAGAMENTO> pags = assi.ASSINANTE_PAGAMENTO.Where(p => p.PLAN_CD_ID == item.PLAN_CD_ID).ToList();
            pags.ForEach(p => p.ASPA_IN_ATIVO = 1);
            Int32 volta1 = baseApp.ValidateEdit(assi, assi, usuario);

            // Envia e-mail ao assinante
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String texto = temApp.GetByCode("ASSPLANREA").TEMP_TX_CORPO;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = assi.ASSI_NM_NOME;
            mens.ID = assi.ASSI_CD_ID;
            mens.MODELO = assi.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.MENS_NM_LINK = conf.CONF_LK_LINK_SISTEMA;
            mens.LINK = "1";
            texto = texto.Replace("{plano}", plan.PLAN_NM_NOME);
            texto = texto.Replace("{data}", item.ASPL_DT_INICIO.Value.ToShortDateString());
            mens.MENS_TX_TEXTO = texto;
            Int32 volta2 = ProcessaEnvioEMailAssinante(mens, usuario);

            return RedirectToAction("VoltarAnexoAssinante");
        }

        [HttpGet]
        public ActionResult AtivarAssinantePlano(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ASSINANTE_PLANO item = baseApp.GetPlanoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];

            // Cria lançamentos de pagamentos futuros
            ASSINANTE assi = baseApp.GetItemById(item.ASSI_CD_ID);
            PLANO plan = baseApp.GetPlanoBaseById(item.PLAN_CD_ID);
            Int32? dias = plan.PLANO_PERIODICIDADE.PLPE_NR_DIAS;
            DateTime? corrente = item.ASPL_DT_INICIO;
            DateTime? final = item.ASPL_DT_VALIDADE;
            Boolean j = true;
            Decimal? preco1 = 0;

            while (j)
            {
                ASSINANTE_PAGAMENTO pag = new ASSINANTE_PAGAMENTO();
                pag.PLAN_CD_ID = item.PLAN_CD_ID;
                pag.ASSI_CD_ID = item.ASSI_CD_ID;
                pag.ASPA_DT_PAGAMENTO = null;
                pag.ASPA_DT_VENCIMENTO = corrente.Value.AddDays(dias.Value);
                pag.ASPA_IN_ATIVO = 1;
                if (item.ASPL_IN_PRECO == 1)
                {
                    pag.ASPA_VL_VALOR = plan.PLAN_VL_PRECO;
                    preco1 = plan.PLAN_VL_PRECO;
                }
                else
                {
                    pag.ASPA_VL_VALOR = plan.PLAN_VL_PROMOCAO;
                    preco1 = plan.PLAN_VL_PROMOCAO;
                }
                pag.ASPA_VL_VALOR_PAGO = null;
                pag.ASPA_IN_PAGO = 0;
                assi.ASSINANTE_PAGAMENTO.Add(pag);

                corrente = corrente.Value.AddDays(dias.Value);
                if (corrente > final)
                {
                    j = false;
                }
            }
            item.ASPL_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditPlano(item);
            Int32 volta2 = baseApp.ValidateEdit(assi, assi, usuario);

            // Envia e-mail ao assinante
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String texto = temApp.GetByCode("ASSPLANATV").TEMP_TX_CORPO;
            String cab = temApp.GetByCode("ASSPLANATV").TEMP_TX_CABECALHO;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = assi.ASSI_NM_NOME;
            mens.ID = assi.ASSI_CD_ID;
            mens.MODELO = assi.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.MENS_NM_LINK = conf.CONF_LK_LINK_SISTEMA;
            mens.LINK = "1";
            texto = texto.Replace("{plano}", plan.PLAN_NM_NOME);
            texto = texto.Replace("{data}", item.ASPL_DT_INICIO.Value.ToShortDateString());
            mens.MENS_TX_TEXTO = texto;
            Int32 volta5 = ProcessaEnvioEMailAssinante(mens, usuario);

            return RedirectToAction("VoltarAnexoAssinante");
        }

        [HttpGet]
        public ActionResult IncluirAssinantePlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara Listas
            ViewBag.Planos = new SelectList(CarregaPlano().OrderBy(p => p.PLAN_NM_NOME), "PLAN_CD_ID", "PLAN_NM_NOME");
            List<SelectListItem> preco = new List<SelectListItem>();
            preco.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            preco.Add(new SelectListItem() { Text = "Promoção", Value = "2" });
            ViewBag.Precos = new SelectList(preco, "Value", "Text");

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 31)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0207", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 32)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0208", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 33)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0209", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 34)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0210", CultureInfo.CurrentCulture));
                }
            }
                
            // Prepara view
            ASSINANTE_PLANO item = new ASSINANTE_PLANO();
            AssinantePlanoViewModel vm = Mapper.Map<ASSINANTE_PLANO, AssinantePlanoViewModel>(item);
            vm.ASSI_CD_ID = (Int32)Session["IdAssi"];
            vm.ASPL_IN_ATIVO = 1;
            vm.ASPL_DT_INICIO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAssinantePlano(AssinantePlanoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Planos = new SelectList(baseApp.GetAllPlanos().OrderBy(p => p.PLAN_NM_NOME), "PLAN_CD_ID", "PLAN_NM_NOME");
            List<SelectListItem> preco = new List<SelectListItem>();
            preco.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            preco.Add(new SelectListItem() { Text = "Promoção", Value = "2" });
            ViewBag.Precos = new SelectList(preco, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ASSINANTE_PLANO item = Mapper.Map<AssinantePlanoViewModel, ASSINANTE_PLANO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PLANO plano = plaApp.GetItemById(item.PLAN_CD_ID);
                    item.ASPL_DT_VALIDADE = item.ASPL_DT_INICIO.Value.AddMonths(plano.PLAN_IN_DURACAO.Value);
                    Int32 volta = baseApp.ValidateCreatePlano(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAssinante"] = 31;
                        return RedirectToAction("IncluirAssinantePlano", "Assinante");
                    }
                    if (volta == 2)
                    {
                        Session["MensAssinante"] = 32;
                        return RedirectToAction("IncluirAssinantePlano", "Assinante");
                    }
                    if (volta == 3)
                    {
                        Session["MensAssinante"] = 33;
                        return RedirectToAction("IncluirAssinantePlano", "Assinante");
                    }
                    if (volta == 4)
                    {
                        Session["MensAssinante"] = 34;
                        return RedirectToAction("IncluirAssinantePlano", "Assinante");
                    }

                    // Cria lançamentos de pagamentos futuros
                    ASSINANTE assi = baseApp.GetItemById(item.ASSI_CD_ID);
                    PLANO plan = baseApp.GetPlanoBaseById(item.PLAN_CD_ID);
                    Int32? dias = plan.PLANO_PERIODICIDADE.PLPE_NR_DIAS;
                    DateTime? corrente = item.ASPL_DT_INICIO;
                    DateTime? final = item.ASPL_DT_VALIDADE;
                    Boolean j = true;
                    Decimal? preco1 = 0;

                    while (j)
                    {
                        ASSINANTE_PAGAMENTO pag = new ASSINANTE_PAGAMENTO();
                        pag.PLAN_CD_ID = item.PLAN_CD_ID;
                        pag.ASSI_CD_ID = item.ASSI_CD_ID;
                        pag.ASPA_DT_PAGAMENTO = null;
                        pag.ASPA_DT_VENCIMENTO = corrente.Value.AddDays(dias.Value);
                        pag.ASPA_IN_ATIVO = 1;
                        if (item.ASPL_IN_PRECO == 1)
                        {
                            pag.ASPA_VL_VALOR = plan.PLAN_VL_PRECO;
                            preco1 = plan.PLAN_VL_PRECO;
                        }
                        else
                        {
                            pag.ASPA_VL_VALOR = plan.PLAN_VL_PROMOCAO;
                            preco1 = plan.PLAN_VL_PROMOCAO;
                        }
                        pag.ASPA_VL_VALOR_PAGO = null;
                        pag.ASPA_IN_PAGO = 0;
                        assi.ASSINANTE_PAGAMENTO.Add(pag);

                        corrente = corrente.Value.AddDays(dias.Value);
                        if (corrente > final)
                        {
                            j = false;
                        }
                    }
                    Int32 volta2 = baseApp.ValidateEdit(assi, assi, usuarioLogado);

                    // Envia e-mail ao assinante
                    CONFIGURACAO conf = confApp.GetItemById(usuarioLogado.ASSI_CD_ID);
                    String texto = temApp.GetByCode("ASSPLANINC").TEMP_TX_CORPO;
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = assi.ASSI_NM_NOME;
                    mens.ID = assi.ASSI_CD_ID;
                    mens.MODELO = assi.ASSI_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_NM_LINK = conf.CONF_LK_LINK_SISTEMA;
                    mens.LINK = "1";
                    texto = texto.Replace("{plano}", plan.PLAN_NM_NOME);
                    texto = texto.Replace("{data}", item.ASPL_DT_INICIO.Value.ToShortDateString());
                    texto = texto.Replace("{final}", item.ASPL_DT_VALIDADE.Value.ToShortDateString());
                    texto = texto.Replace("{periodo}", plan.PLANO_PERIODICIDADE.PLPE_NM_NOME);
                    texto = texto.Replace("{valor}", preco1.ToString());
                    mens.MENS_TX_TEXTO = texto;
                    Int32 volta1 = ProcessaEnvioEMailAssinante(mens, usuarioLogado);

                    // Finaliza
                    return RedirectToAction("VoltarAnexoAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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
        public ActionResult EnviarEMailAssinante(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ASSINANTE cont = baseApp.GetItemById(id);
            Session["Assi"] = cont;
            ViewBag.Assinante = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.ASSI_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.LINK = "2";
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailAssinante(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioEMailAssinante(vm, usuarioLogado);

                    // Verifica retorno
                    // Sucesso
                    return RedirectToAction("VoltarMensagemAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailAssinante(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];
            ASSINANTE cont = baseApp.GetItemById(vm.ID.Value);
            String erro = null;

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.ASSI_NM_NOME + "</b>";

            // Prepara rodape
            String rod = String.Empty;
            if (vm.MENS_NM_RODAPE == null)
            {
                rod = "<b> Administração CRMSys </b>";
            }

            // Prepara corpo do e-mail
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            
            // Link          
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
                if (vm.LINK == "1")
                {
                    str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para acessar o CRMSys</a>");
                }
                else
                {
                    str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
                }
            }
            String body = str.ToString();
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;
            String status = "Succeeded";
            String iD = "xyz";

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Assinante - Ativação de Plano";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = cont.ASSI_NM_EMAIL;
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
                throw;
            }

            // Grava envio
            MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
            env.ASSI_CD_ID = idAss;
            env.USUA_CD_ID = usuario.USUA_CD_ID;
            env.MEEN_IN_ASSINANTE = cont.ASSI_CD_ID;
            env.MEEN_IN_TIPO = 1;
            env.MEEN_DT_DATA_ENVIO = DateTime.Now;
            env.MEEN_EM_EMAIL_DESTINO = cont.ASSI_NM_EMAIL;
            env.MEEN_NM_ORIGEM = "Mensagem para Assinante";
            env.MEEN_TX_CORPO = vm.MENS_TX_TEXTO;
            env.MEEN_IN_ANEXOS = 0;
            env.MEEN_IN_ATIVO = 1;
            env.MEEN_IN_ESCOPO = 4;
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
            Int32 volta5 = mevApp.ValidateCreate(env);
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSAssinante(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ASSINANTE item = baseApp.GetItemById(id);
            Session["Assi"] = item;
            ViewBag.Assinante = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.ASSI_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.ASSI_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSAssinante(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioSMSAssinante(vm, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VoltarMensagemAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSAssinante(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            ASSINANTE cont = (ASSINANTE)Session["Assi"];

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.ASSI_NM_NOME;

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assi"];
            String rod = assi.ASSI_NM_NOME;

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Decriptografa chaves
            String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
            String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

            // Monta token
            //String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            String text = login + ":" + senha;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = cab + vm.MENS_TX_SMS + rod;

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
            String erro = null;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(cont.ASSI_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                throw;
            }

            // Grava envio
            MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
            env.ASSI_CD_ID = idAss;
            env.USUA_CD_ID = usuario.USUA_CD_ID;
            env.MEEN_IN_ASSINANTE = cont.ASSI_CD_ID;
            env.MEEN_IN_TIPO = 2;
            env.MEEN_DT_DATA_ENVIO = DateTime.Now;
            env.MEEN__CELULAR_DESTINO = cont.ASSI_NR_CELULAR;
            env.MEEN_NM_ORIGEM = "Mensagem para Assinante";
            env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
            env.MEEN_TX_CORPO_COMPLETO = texto;
            env.MEEN_IN_ANEXOS = 0;
            env.MEEN_IN_ATIVO = 1;
            env.MEEN_IN_ESCOPO = 4;
            if (erro == null)
            {
                env.MEEN_IN_ENTREGUE = 1;
            }
            else
            {
                env.MEEN_IN_ENTREGUE = 0;
                env.MEEN_TX_RETORNO = erro;
            }
            Int32 volta5 = mevApp.ValidateCreate(env);
            return 0;
        }

        public ActionResult MontarTelaIndicadoresAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Carrega Assinantes
            List<ASSINANTE> forns = CarregaAssinante();
            Int32 fornNum = forns.Count;
            Int32 fornAtivos = forns.Where(p => p.ASSI_IN_STATUS == 1).Count();
            Int32 fornBloqueados = forns.Where(p => p.ASSI_IN_STATUS == 2).Count();

            Session["AssNum"] = fornNum;
            Session["AssAtivos"] = fornAtivos;
            Session["AssBloqueados"] = fornAtivos;

            ViewBag.AssNum = fornNum;
            ViewBag.AssAtivos = fornAtivos;
            ViewBag.AssBloqueados = fornBloqueados;

            // Recupera assinantes por UF
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
            ViewBag.ListaAssUF = lista2;
            Session["ListaAssUF"] = lista2;

            // Recupera Assinantes por Cidade
            List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
            List<String> cids = forns.Where(m => m.ASSI_NM_CIDADE != null).Select(p => p.ASSI_NM_CIDADE).Distinct().ToList();
            foreach (String item in cids)
            {
                Int32 num = forns.Where(p => p.ASSI_NM_CIDADE == item).ToList().Count;
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome = item;
                mod.Valor = num;
                lista3.Add(mod);
            }
            ViewBag.ListaAssCidade = lista3;
            Session["ListaAssCidade"] = lista3;

            // Recupera Assinantes por Plano
            List<ModeloViewModel> lista4 = new List<ModeloViewModel>();
            List<PLANO> cats = CarregaPlano();
            List<ASSINANTE_PLANO> assPlan = baseApp.GetAllAssPlanos().ToList();
            
            foreach (PLANO item in cats)
            {
                Int32 num = assPlan.Where(p => p.PLAN_CD_ID == item.PLAN_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.PLAN_NM_NOME;
                    mod.Valor = num;
                    lista4.Add(mod);
                }
            }
            ViewBag.ListaAssPlano = lista4;
            Session["ListaAssPlano"] = lista4;

            // Recupera Assinantes com Pagto em atraso
            ViewBag.ListaClienteAtraso = null;

            return View(vm);
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

        public JsonResult GetDadosAssinantePlanoLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssPlano"];
            List<String> plano = new List<String>();
            List<Int32> valor = new List<Int32>();
            plano.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                plano.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", plano);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteUF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssUF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#FFA113");
            cor.Add("#FFB798");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                else if (i == 4)
                {
                    cor.Add("#FFA113");
                }
                else if (i == 5)
                {
                    cor.Add("#FF7C32");
                }
                i++;
                if (i > 5)
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
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#FFA113");
            cor.Add("#FFB798");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                else if (i == 4)
                {
                    cor.Add("#FFA113");
                }
                else if (i == 5)
                {
                    cor.Add("#FF7C32");
                }
                i++;
                if (i > 5)
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

        public JsonResult GetDadosAssinantePlano()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssPlano"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#FFA113");
            cor.Add("#FFB798");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                else if (i == 4)
                {
                    cor.Add("#FFA113");
                }
                else if (i == 5)
                {
                    cor.Add("#FF7C32");
                }
                i++;
                if (i > 5)
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

        public ActionResult IncluirAnotacaoAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ASSINANTE item = baseApp.GetItemById((Int32)Session["IdAssi"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ASSINANTE_ANOTACAO coment = new ASSINANTE_ANOTACAO();
            AssinanteAnotacaoViewModel vm = Mapper.Map<ASSINANTE_ANOTACAO, AssinanteAnotacaoViewModel>(coment);
            vm.ASAT_DT_ANOTACAO = DateTime.Now;
            vm.ASAT_IN_ATIVO = 1;
            vm.ASSI_CD_ID = item.ASSI_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAnotacaoAssinante(AssinanteAnotacaoViewModel vm)
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
                    ASSINANTE_ANOTACAO item = Mapper.Map<AssinanteAnotacaoViewModel, ASSINANTE_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ASSINANTE not = baseApp.GetItemById((Int32)Session["IdAssi"]);

                    item.USUARIO = null;
                    not.ASSINANTE_ANOTACAO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarAssinante", new { id = (Int32)Session["IdAssi"] });
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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
        public ActionResult EditarPagamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara view
            ASSINANTE_PAGAMENTO item = baseApp.GetPagtoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];
            AssinantePagamentoViewModel vm = Mapper.Map<ASSINANTE_PAGAMENTO, AssinantePagamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPagamento(AssinantePagamentoViewModel vm)
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
                    ASSINANTE_PAGAMENTO item = Mapper.Map<AssinantePagamentoViewModel, ASSINANTE_PAGAMENTO>(vm);
                    Int32 volta = baseApp.ValidateEditPagto(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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
        public ActionResult ExcluirPagamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ASSINANTE_PAGAMENTO item = baseApp.GetPagtoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];
            item.ASPA_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditPagto(item);
            return RedirectToAction("VoltarAnexoAssinante");
        }

        [HttpGet]
        public ActionResult ReativarPagamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ASSINANTE_PAGAMENTO item = baseApp.GetPagtoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];
            item.ASPA_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditPagto(item);
            return RedirectToAction("VoltarAnexoAssinante");
        }

        [HttpGet]
        public ActionResult PagamentoPadrao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ASSINANTE_PAGAMENTO item = baseApp.GetPagtoById(id);
            objetoAntes = (ASSINANTE)Session["Assi"];
            item.ASPA_IN_PAGO = 1;
            item.ASPA_DT_PAGAMENTO = DateTime.Today.Date;
            item.ASPA_VL_VALOR_PAGO = item.ASPA_VL_VALOR;
            Int32 volta = baseApp.ValidateEditPagto(item);
            return RedirectToAction("VoltarAnexoAssinante");
        }

        [HttpGet]
        public ActionResult IncluirPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Recupera listas
            ASSINANTE assi = baseApp.GetItemById((Int32)Session["IdAssi"]);
            List<ASSINANTE_PLANO> lista = assi.ASSINANTE_PLANO.ToList();
            List<PLANO> planos = new List<PLANO>();
            foreach (ASSINANTE_PLANO plan in lista)
            {
                planos.Add(plan.PLANO);
            }
            ViewBag.Planos = new SelectList(planos, "PLAN_CD_ID", "PLAN_NM_NOME");

            // Prepara view
            ASSINANTE_PAGAMENTO item = new ASSINANTE_PAGAMENTO();
            AssinantePagamentoViewModel vm = Mapper.Map<ASSINANTE_PAGAMENTO, AssinantePagamentoViewModel>(item);
            vm.ASSI_CD_ID = (Int32)Session["IdAssi"];
            vm.ASPA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirPagamento(AssinantePagamentoViewModel vm)
        {

            ASSINANTE assi = baseApp.GetItemById((Int32)Session["IdAssi"]);
            List<ASSINANTE_PLANO> lista = assi.ASSINANTE_PLANO.ToList();
            List<PLANO> planos = new List<PLANO>();
            foreach (ASSINANTE_PLANO plan in lista)
            {
                planos.Add(plan.PLANO);
            }
            ViewBag.Planos = new SelectList(planos, "PLAN_CD_ID", "PLAN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ASSINANTE_PAGAMENTO item = Mapper.Map<AssinantePagamentoViewModel, ASSINANTE_PAGAMENTO>(vm);
                    Int32 volta = baseApp.ValidateCreatePagto(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAssinante"] = 21;
                        return RedirectToAction("VoltarAnexoAssinante", "Assinante");
                    }
                    if (volta == 2)
                    {
                        Session["MensAssinante"] = 22;
                        return RedirectToAction("VoltarAnexoAssinante", "Assinante");
                    }
                    if (volta == 3)
                    {
                        Session["MensAssinante"] = 23;
                        return RedirectToAction("VoltarAnexoAssinante", "Assinante");
                    }

                    // Finaliza
                    return RedirectToAction("VoltarAnexoAssinante");
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Assinante";
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

        public JsonResult GetPlanos(Int32 id)
        {
            PLANO forn = baseApp.GetPlanoBaseById(id);
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

        [HttpGet]
        public ActionResult EnviarEMailAssinanteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMensAssinante"] = 1;
            return RedirectToAction("EnviarEMailAssinante", new { id = (Int32)Session["IdAssi"] });
        }

        [HttpGet]
        public ActionResult EnviarSMSAssinanteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMensAssinante"] = 1;
            return RedirectToAction("EnviarSMSAssinante", new { id = (Int32)Session["IdAssi"] });
        }

        public JsonResult GetDadosUsuario()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaUsuario"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
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

        public JsonResult GetDadosCliente()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaCliente"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
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

        public JsonResult GetDadosProcesso()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProcesso"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
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

        public JsonResult GetDadosAcao()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAcao"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
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

        public JsonResult GetDadosEMail()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaEMail"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
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

        public JsonResult GetDadosSMS()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaSMS"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
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
        public ActionResult VerConsumoAssinante(Int32 id)
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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

            // Prepara view
            ViewBag.TiposPessoa = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME"); ;
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");

            // Recupera consumo
            Int32 numUsu = usuApp.GetAllItens(idAss).Count;
            Int32 numCli = cliApp.GetAllItens(idAss).Count;
            Int32 numProc = crmApp.GetAllItens(idAss).Count;
            Int32 numAcoes = crmApp.GetAllAcoes(idAss).Count;
            Int32 numProps = crmApp.GetAllPedidos(idAss).Count;
            Int32 numEmail = menApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1).ToList().Count;
            Int32 numSMS = menApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2).ToList().Count;

            ViewBag.NumUsu = numUsu;
            ViewBag.NumCli = numCli;
            ViewBag.NumProc = numProc;
            ViewBag.NumAcoes = numAcoes;
            ViewBag.NumProps = numProps;
            ViewBag.NumEmail = numEmail;
            ViewBag.NumSMS = numSMS;

            // Monta listas para graficos
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            ModeloViewModel mod = new ModeloViewModel();
            mod.Nome = "Usuários";
            mod.Valor = numUsu;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Nome = "Limite";
            mod.Valor = (Int32)Session["NumUsuarios"];
            lista1.Add(mod);
            ViewBag.ListaUsuario = lista1;
            Session["ListaUsuario"] = lista1;

            lista1 = new List<ModeloViewModel>();
            mod = new ModeloViewModel();
            mod.Nome = "Clientes";
            mod.Valor = numCli;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Nome = "Limite";
            mod.Valor = (Int32)Session["NumClientes"];
            lista1.Add(mod);
            ViewBag.ListaCliente = lista1;
            Session["ListaCliente"] = lista1;

            lista1 = new List<ModeloViewModel>();
            mod = new ModeloViewModel();
            mod.Nome = "Processos";
            mod.Valor = numProc;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Nome = "Limite";
            mod.Valor = (Int32)Session["NumProcessos"];
            lista1.Add(mod);
            ViewBag.ListaProcessos = lista1;
            Session["ListaProcesso"] = lista1;

            lista1 = new List<ModeloViewModel>();
            mod = new ModeloViewModel();
            mod.Nome = "Ações";
            mod.Valor = numAcoes;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Nome = "Limite";
            mod.Valor = (Int32)Session["NumAcoes"];
            lista1.Add(mod);
            ViewBag.ListaAcoes = lista1;
            Session["ListaAcao"] = lista1;

            lista1 = new List<ModeloViewModel>();
            mod = new ModeloViewModel();
            mod.Nome = "E-Mails";
            mod.Valor = numAcoes;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Nome = "Limite";
            mod.Valor = (Int32)Session["NumEMail"];
            lista1.Add(mod);
            ViewBag.ListaEMail = lista1;
            Session["ListaEMail"] = lista1;

            lista1 = new List<ModeloViewModel>();
            mod = new ModeloViewModel();
            mod.Nome = "SMS";
            mod.Valor = numAcoes;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Nome = "Limite";
            mod.Valor = (Int32)Session["numSMS"];
            lista1.Add(mod);
            ViewBag.ListaSMS = lista1;
            Session["ListaSMS"] = lista1;

            // Recupera
            ASSINANTE item = baseApp.GetItemById(id);
            ViewBag.QuadroSoci = ccnpjApp.GetByCliente(item);

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 21)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0204", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0205", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 23)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0206", CultureInfo.CurrentCulture));
                }
            }

            // Indicadores

            // Sessões
            Session["MensAssinante"] = null;
            Session["VoltaAssinante"] = 1;
            objetoAntes = item;
            Session["Assi"] = item;
            Session["IdAssi"] = id;
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult VerConsumoAssinanteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerConsumoAssinante", new { id = (Int32)Session["IdAssi"] });
        }

        [HttpGet]
        public ActionResult VerAssinantesAtraso()
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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

            // Carrega lista
            if ((List<ASSINANTE_PAGAMENTO>)Session["Atrasos"] != null)
            {
                ViewBag.Listas = (List<ASSINANTE_PAGAMENTO>)Session["Atrasos"];
            }
            else
            {
                List<ASSINANTE_PAGAMENTO> pags = baseApp.GetAllPagamentos();
                pags = pags.Where(p => p.ASPA_IN_PAGO == 0 & p.ASPA_NR_ATRASO > 0).ToList();
                Session["Atrasos"] = pags;
                ViewBag.Listas = pags;
            }

            // Filtros
            ViewBag.Tipos = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Assi"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ativo.Add(new SelectListItem() { Text = "Bloqueado", Value = "2" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            Session["IncluirAssinante"] = 0;

            // Indicadores
            ViewBag.Atraso = ((List<ASSINANTE_PAGAMENTO>)Session["Atrasos"]).Count;

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensAssinante"] = null;
            ASSINANTE_PAGAMENTO objeto = new ASSINANTE_PAGAMENTO();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult VerAssinantesBloqueados()
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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

            // Carrega lista
            if ((List<ASSINANTE_PLANO>)Session["PlanosVencidos"] != null)
            {
                ViewBag.Listas = (List<ASSINANTE>)Session["Bloqueados"];
            }
            else
            {
                List<ASSINANTE> bloqueado = baseApp.GetAllItens().Where(p => p.ASSI_IN_STATUS == 2).ToList();
                Session["Bloqueados"] = bloqueado;
                ViewBag.Listas = bloqueado;
            }

            // Filtros
            ViewBag.Tipos = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Assi"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ativo.Add(new SelectListItem() { Text = "Bloqueado", Value = "2" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            Session["IncluirAssinante"] = 0;

            // Indicadores
            ViewBag.Vencidas = ((List<ASSINANTE_PLANO>)Session["PlanosVencidos"]).Count;

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensAssinante"] = null;
            ASSINANTE objeto = new ASSINANTE();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult VerAssinaturasVencidas()
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
                if (usuario.USUA_IN_ESPECIAL != 1)
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

            // Carrega lista
            if ((List<ASSINANTE_PLANO>)Session["PlanosVencidos"] != null)
            {
                ViewBag.Listas = (List<ASSINANTE_PLANO>)Session["PlanosVencidos"];
            }
            else
            {
                List<ASSINANTE_PLANO> planosAss = baseApp.GetAllAssPlanos();
                List<ASSINANTE_PLANO> planosVencidos = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date).ToList();
                Session["PlanosVencidos"] = planosVencidos;
                ViewBag.Listas = planosVencidos;
            }

            // Filtros
            ViewBag.Tipos = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Assi"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ativo.Add(new SelectListItem() { Text = "Bloqueado", Value = "2" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            Session["IncluirAssinante"] = 0;

            // Indicadores
            ViewBag.Vencidas = ((List<ASSINANTE_PLANO>)Session["PlanosVencidos"]).Count;

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensAssinante"] = null;
            ASSINANTE_PLANO objeto = new ASSINANTE_PLANO();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult VerAssinaturasVencidas30()
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
                if (usuario.USUA_IN_ESPECIAL != 1)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            //Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega lista
            if ((List<ASSINANTE_PLANO>)Session["PlanosVencer30"] != null)
            {
                ViewBag.Listas = (List<ASSINANTE_PLANO>)Session["PlanosVencer30"];
            }
            else
            {
                List<ASSINANTE_PLANO> planosAss = baseApp.GetAllAssPlanos();
                List<ASSINANTE_PLANO> planosVencer30 = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30)).ToList();
                Session["PlanosVencer30"] = planosVencer30;
                ViewBag.Listas = planosVencer30;
            }

            // Filtros
            ViewBag.Tipos = new SelectList(CarregaTipoPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Assi"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ativo.Add(new SelectListItem() { Text = "Bloqueado", Value = "2" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            Session["IncluirAssinante"] = 0;

            // Indicadores
            ViewBag.Vencer30 = ((List<ASSINANTE_PLANO>)Session["PlanosVencer30"]).Count;

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensAssinante"] = null;
            ASSINANTE_PLANO objeto = new ASSINANTE_PLANO();
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarAssinanteAtraso(ASSINANTE_PAGAMENTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                //Int32 idAss = (Int32)Session["IdAssinante"];
                List<ASSINANTE_PAGAMENTO> listaObj = new List<ASSINANTE_PAGAMENTO>();
                Session["FiltroAssinante"] = item;
                Int32 volta = baseApp.ExecuteFilterAtraso(item.ASSINANTE.ASSI_NM_NOME, item.ASSINANTE.ASSI_NR_CPF, item.ASSINANTE.ASSI_NR_CNPJ, item.ASSINANTE.ASSI_NM_CIDADE, item.ASSINANTE.UF_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAssinante"] = 1;
                    return RedirectToAction("VerAssinantesAtraso");
                }

                // Prepara nova lista
                listaObj = listaObj.Where(p => p.ASPA_IN_PAGO == 0 & p.ASPA_NR_ATRASO > 0).ToList();
                Session["Atrasos"] = listaObj;
                return RedirectToAction("VerAssinantesAtraso");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroAtraso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["Atrasos"] = null;
            return RedirectToAction("VerAssinantesAtraso");
        }

        [HttpPost]
        public ActionResult FiltrarAssinanteVencidos(ASSINANTE_PLANO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                //Int32 idAss = (Int32)Session["IdAssinante"];
                List<ASSINANTE_PLANO> listaObj = new List<ASSINANTE_PLANO>();
                Int32 volta = baseApp.ExecuteFilterVencidos(item.ASSINANTE.ASSI_NM_NOME, item.ASSINANTE.ASSI_NR_CPF, item.ASSINANTE.ASSI_NR_CNPJ, item.ASSINANTE.ASSI_NM_CIDADE, item.ASSINANTE.UF_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAssinante"] = 1;
                    return RedirectToAction("VerAssinaturasVencidas");
                }

                // Prepara nova lista
                listaObj = listaObj.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date).ToList();
                Session["PlanosVencidos"] = listaObj;
                return RedirectToAction("VerAssinaturasVencidas");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroVencidos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["PlanosVencidos"] = null;
            return RedirectToAction("VerAssinaturasVencidas");
        }

        [HttpPost]
        public ActionResult FiltrarAssinanteVencer30(ASSINANTE_PLANO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                //Int32 idAss = (Int32)Session["IdAssinante"];
                List<ASSINANTE_PLANO> listaObj = new List<ASSINANTE_PLANO>();
                Int32 volta = baseApp.ExecuteFilterVencer30(item.ASSINANTE.ASSI_NM_NOME, item.ASSINANTE.ASSI_NR_CPF, item.ASSINANTE.ASSI_NR_CNPJ, item.ASSINANTE.ASSI_NM_CIDADE, item.ASSINANTE.UF_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAssinante"] = 1;
                    return RedirectToAction("VerAssinaturasVencidas30");
                }

                // Prepara nova lista
                listaObj = listaObj.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30)).ToList();
                Session["PlanosVencer30"] = listaObj;
                return RedirectToAction("VerAssinaturasVencidas30");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Assinante";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroVencer30()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["PlanosVencer30"] = null;
            return RedirectToAction("VerAssinaturasVencidas30");
        }

        public ActionResult GerarRelatorioLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AssinanteLista" + "_" + data + ".pdf";
            List<ASSINANTE> lista = (List<ASSINANTE>)Session["ListaAssinante"];
            ASSINANTE filtro = (ASSINANTE)Session["FiltroAssinante"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Assinantes - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] {150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Assinantes selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

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
            cell = new PdfPCell(new Paragraph("Celular", meuFont))
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

            foreach (ASSINANTE item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.ASSI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSI_NR_CPF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSI_NR_CPF, meuFont))
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
                if (item.ASSI_NR_CNPJ != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSI_NR_CNPJ, meuFont))
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
                cell = new PdfPCell(new Paragraph(item.ASSI_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSI_NR_CELULAR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSI_NR_CELULAR, meuFont))
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
                if (item.ASSI_NM_CIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSI_NM_CIDADE, meuFont))
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
                if (filtro.TIPE_CD_ID > 0)
                {
                    parametros += "Tipo de Pessoa: " + filtro.TIPE_CD_ID;
                    ja = 1;
                }
                if (filtro.ASSI_NM_NOME  != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.ASSI_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.ASSI_NM_NOME;
                    }
                }
                if (filtro.ASSI_NR_CPF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CPF: " + filtro.ASSI_NR_CPF;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CPF: " + filtro.ASSI_NR_CPF;
                    }
                }
                if (filtro.ASSI_NR_CNPJ != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CNPJ: " + filtro.ASSI_NR_CNPJ;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CNPJ: " + filtro.ASSI_NR_CNPJ;
                    }
                }
                if (filtro.ASSI_NM_CIDADE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Cidade: " + filtro.ASSI_NM_CIDADE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Cidade: " + filtro.ASSI_NM_CIDADE;
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

            return RedirectToAction("MontarTelaAssinante");
        }

        public ActionResult GerarRelatorioListaAtraso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AssinanteAtrasoLista" + "_" + data + ".pdf";
            List<ASSINANTE_PAGAMENTO> lista = (List<ASSINANTE_PAGAMENTO>)Session["Atrasos"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Assinantes em Atraso - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] {150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Assinantes selecionados pelo atraso nas parcelas", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

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
            cell = new PdfPCell(new Paragraph("Celular", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso (Dias)", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (ASSINANTE_PAGAMENTO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSINANTE.ASSI_NR_CPF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CPF, meuFont))
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
                if (item.ASSINANTE.ASSI_NR_CNPJ != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CNPJ, meuFont))
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
                cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSINANTE.ASSI_NR_CELULAR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CELULAR, meuFont))
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
                if (item.PLANO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO.PLAN_NM_NOME, meuFont))
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
                if (item.ASPA_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.ASPA_NR_ATRASO.ToString(), meuFont))
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

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerAssinantesAtraso");
        }

        public ActionResult GerarRelatorioListaVencidos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AssinanteVencidosLista" + "_" + data + ".pdf";
            List<ASSINANTE_PLANO> lista = (List<ASSINANTE_PLANO>)Session["PlanosVencidos"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Assinantes - Planos Vencidos", meuFont2))
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
            table = new PdfPTable(new float[] {150f, 60f, 60f, 150f, 50f, 50f, 20f, 80f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

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
            cell = new PdfPCell(new Paragraph("Celular", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
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

            foreach (ASSINANTE_PLANO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSINANTE.ASSI_NR_CPF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CPF, meuFont))
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
                if (item.ASSINANTE.ASSI_NR_CNPJ != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CNPJ, meuFont))
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
                cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSINANTE.ASSI_NR_CELULAR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CELULAR, meuFont))
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
                if (item.PLANO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO.PLAN_NM_NOME, meuFont))
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
                if (item.ASPL_DT_VALIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASPL_DT_VALIDADE.Value.ToShortDateString(), meuFont))
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
                if (System.IO.File.Exists(Server.MapPath(item.ASSINANTE.ASSI_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.ASSINANTE.ASSI_AQ_FOTO));
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

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerAssinaturasVencidas");
        }

        public ActionResult GerarRelatorioListaVencer30()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AssinanteVencer30Lista" + "_" + data + ".pdf";
            List<ASSINANTE_PLANO> lista = (List<ASSINANTE_PLANO>)Session["PlanosVencer30"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Assinantes - Planos Vencendo em 30 dias", meuFont2))
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
            table = new PdfPTable(new float[] {150f, 60f, 60f, 150f, 50f, 50f, 20f, 80f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

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
            cell = new PdfPCell(new Paragraph("Celular", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
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

            foreach (ASSINANTE_PLANO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSINANTE.ASSI_NR_CPF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CPF, meuFont))
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
                if (item.ASSINANTE.ASSI_NR_CNPJ != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CNPJ, meuFont))
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
                cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ASSINANTE.ASSI_NR_CELULAR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASSINANTE.ASSI_NR_CELULAR, meuFont))
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
                if (item.PLANO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO.PLAN_NM_NOME, meuFont))
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
                if (item.ASPL_DT_VALIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ASPL_DT_VALIDADE.Value.ToShortDateString(), meuFont))
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
                if (System.IO.File.Exists(Server.MapPath(item.ASSINANTE.ASSI_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.ASSINANTE.ASSI_AQ_FOTO));
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

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerAssinaturasVencidas30");
        }

        [HttpGet]
        public Int32 CriaAmbienteNovoAssinante(ASSINANTE item)
        {
            // Recupera usuario
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 assi = item.ASSI_CD_ID;

            // Cria configuracao
            CONFIGURACAO_CHAVES chaves = baseApp.GetChaves(1);
            CONFIGURACAO a = new CONFIGURACAO();
            a.ASSI_CD_ID = assi;
            a.CONF_NR_FALHAS_DIA = 3;
            a.CONF_NM_HOST_SMTP = "smtp.sendgrid.net";
            a.CONF_NM_PORTA_SMTP = "465";
            a.CONF_NM_EMAIL_EMISSOO = "sistema@systembr.net";
            a.CONF_NM_SENHA_EMISSOR = chaves.CONF_NM_SENDGRID_PWD;
            a.CONF_NR_REFRESH_DASH = 3000;
            a.CONF_NM_ARQUIVO_ALARME = "chimes.wav";
            a.CONF_NR_REFRESH_NOTIFICACAO = 3000;
            a.CONF_SG_LOGIN_SMS = chaves.CONF_SG_LOGIN_SMS;
            a.CONF_SG_SENHA_SMS = chaves.CONF_SG_SENHA_SMS;
            a.CONF_NM_SENDGRID_LOGIN = chaves.CONF_NM_SENDGRID_LOGIN;
            a.CONF_NM_SENDGRID_PWD = chaves.CONF_NM_SENDGRID_PWD;
            a.CONF_NM_SENDGRID_APIKEY = chaves.CONF_NM_SENDGRID_APIKEY;
            a.CONF_IN_RESIDUAL = 0;
            a.CONF_NR_DIAS_ATENDIMENTO = 3;
            a.CONF_NR_DIAS_ACAO = 3;
            a.CONF_NR_DIAS_PROPOSTA = 3;
            a.CONF_NR_MARGEM_ATRASO = 3;
            a.CONF_IN_DIAS_RESERVA_ESTOQUE = 3;
            a.CONF_IN_NUMERO_INICIAL_PROPOSTA = 1;
            a.CONF_IN_NUMERO_INICIAL_PEDIDO = 1;
            a.CONF_IN_CNPJ_DUPLICADO = 1;
            a.CONF_IN_INCLUIR_SEM_ESTOQUE = 1;
            a.CONF_IN_ASSINANTE_FILIAL = 1;
            a.CONF_IN_FALHA_IMPORTACAO = 1;
            a.CONF_IN_ETAPAS_CRM = 9;
            a.CONF_IN_NOTIF_ACAO_ADM = 1;
            a.CONF_IN_NOTIF_ACAO_GER = 1;
            a.CONF_IN_NOTIF_ACAO_VEN = 1;
            a.CONF_IN_NOTIF_ACAO_OPR = 0;
            a.CONF_IN_NOTIF_ACAO_USU = 0;
            a.CONF_LK_LINK_SISTEMA = "https://crmsys.azurewebsites.net";
            a.CONF_EM_CRMSYS = "rti.principal@gmail.com";
            a.CONF_EM_CRMSYS1 = "clayton@systembr.net";
            a.CONF_NR_SUPORTE_ZAP = "(27)99787-1093";
            Int32 volta = confApp.ValidateCreate(a);

            // Cat.Agenda
            CATEGORIA_AGENDA b = new CATEGORIA_AGENDA();
            b.ASSI_CD_ID = assi;
            b.CAAG_NM_NOME = "Normal";
            b.CAAG_IN_ATIVO = 1;
            volta = caApp.ValidateCreate(b, usuario);

            // Cat.Cliente
            CATEGORIA_CLIENTE c = new CATEGORIA_CLIENTE();
            c.ASSI_CD_ID = assi;
            c.CACL_NM_NOME = "Normal";
            c.CACL_IN_ORDEM = 1;
            c.CACL_IN_LIMITE_MINIMO_EMITIDAS = 10;
            c.CACL_IN_LIMITE_MAXIMO_EMITIDAS = 30;
            c.CACL_IN_LIMITE_MINIMO_APROVADAS = 15;
            c.CACL_IN_LIMITE__MAXIMO_APROVADAS = 40;
            c.CACL_IN_LIMITE_MINIMO_REPROVADAS = 5;
            c.CACL_IN_LIMITE__MAXIMO_REPROVADAS = 10;
            c.CACL_IN_ATIVO = 1;
            volta = ccApp.ValidateCreate(c, null);

            // Cargo
            CARGO d = new CARGO();
            d.ASSI_CD_ID = assi;
            d.CARG_NM_NOME = "Operador";
            d.CARG_IN_TIPO = 1;
            d.CARG_IN_ATIVO = 1;
            volta = cgApp.ValidateCreate(d, usuario);
            CARGO cargo = cgApp.GetAllItens(assi).FirstOrDefault();
            Int32 carId = cargo.CARG_CD_ID;

            // Cat.Telefone
            CATEGORIA_TELEFONE e = new CATEGORIA_TELEFONE();
            e.ASSI_CD_ID = assi;
            e.CATE_NM_NOME = "Normal";
            e.CATE_IN_ATIVO = 1;
            volta = ctApp.ValidateCreate(e, usuario);

            // CRM Origem
            CRM_ORIGEM f = new CRM_ORIGEM();
            f.ASSI_CD_ID = assi;
            f.CROR_NM_NOME = "Contato";
            f.CROR_IN_ATIVO = 1;
            volta = coApp.ValidateCreate(f, null);

            // Filial
            //FILIAL g = new FILIAL();
            //g.ASSI_CD_ID = assi;
            //g.FILI_IN_MATRIZ = 1;
            //g.FILI_NM_NOME = item.ASSI_NM_NOME;
            //g.FILI_NM_RAZAO = item.ASSI_NM_RAZAO_SOCIAL;
            //g.FILI_NR_CPF = item.ASSI_NR_CPF;
            //g.FILI_NR_CNPJ = item.ASSI_NR_CNPJ;
            //g.FILI_NR_RG = null;
            //g.FILI_NM_EMAIL = item.ASSI_NM_EMAIL;
            //g.FILI_NM_TELEFONES = item.ASSI_NR_TELEFONE;
            //g.FILI_NR_CELULAR = item.ASSI_NR_CELULAR;
            //g.FILI_NM_ENDERECO = item.ASSI_NM_ENDERECO + " " + item.ASSI_NR_NUMERO + " " + item.ASSI_NM_COMPLEMENTO;
            //g.FILI_NM_BAIRRO = item.ASSI_NM_BAIRRO;
            //g.FILI_NM_CIDADE = item.ASSI_NM_CIDADE;
            //g.UF_CD_ID = item.UF_CD_ID;
            //g.FILI_NR_CEP = item.ASSI_NR_CEP;
            //g.FILI_IN_IE_ISENTO = 1;
            //g.FILI_DT_CADASTRO = DateTime.Today.Date;
            //g.FILI_IN_ATIVO = 1;
            //volta = fiApp.ValidateCreate(g, null);
            //FILIAL filial = fiApp.GetAllItens(assi).FirstOrDefault();
            //Int32 filId = filial.FILI_CD_ID;

            //String caminho = "/Imagens/" + assi.ToString() + "/Filial/" + filId.ToString() + "/Logo/";
            //Directory.CreateDirectory(Server.MapPath(caminho));

            // Funil
            FUNIL h = new FUNIL();
            h.ASSI_CD_ID = assi;
            h.FUNI_NM_NOME = "Funil Padrão";
            h.FUNI_SG_SIGLA = "PAD";
            h.FUNI_DS_DESCRICAO = "Funil Padrão do CRMSys";
            h.FUNI_IN_ACOES = 1;
            h.FUNI_IN_CONTATO = 1;
            h.FUNI_IN_PROPOSTA = 1;
            h.FUNI_IN_AGENDA = 1;
            h.FUNI_DT_CADASTRO = DateTime.Today.Date;
            h.FUNI_IN_FIXO = 1;
            h.FUNI_IN_CLIENTE = 1;
            h.FUNI_IN_RESPONSAVEL = 1;
            h.FUNI_IN_ATIVO = 1;
            volta = fuApp.ValidateCreate(h, null);
            FUNIL funil = fuApp.GetAllItens(assi).FirstOrDefault();
            Int32 funilId = funil.FUNI_CD_ID;

            // Etapas
            FUNIL_ETAPA i = new FUNIL_ETAPA();
            i.FUNI_CD_ID = funilId;
            i.FUET_NM_NOME = "Prospecção";
            i.FUET_SG_SIGLA = "PRO";
            i.FUET_DS_DESCRICAO = "Etapa inicial do processo";
            i.FUET_IN_ORDEM = 1;
            i.FUET_IN_ENCERRA = 0;
            i.FUET_IN_PROPOSTA = 0;
            i.FUET_IN_EMAIL = 0;
            i.FUET_IN_SMS = 0;
            i.FUET_IN_ATIVO = 1;
            volta = fuApp.ValidateCreateEtapa(i);

            i = new FUNIL_ETAPA();
            i.FUNI_CD_ID = funilId;
            i.FUET_NM_NOME = "Contato Realizado";
            i.FUET_SG_SIGLA = "CTR";
            i.FUET_DS_DESCRICAO = "Contato mantido com o cliente";
            i.FUET_IN_ORDEM = 2;
            i.FUET_IN_ENCERRA = 0;
            i.FUET_IN_PROPOSTA = 0;
            i.FUET_IN_EMAIL = 0;
            i.FUET_IN_SMS = 0;
            i.FUET_IN_ATIVO = 1;
            volta = fuApp.ValidateCreateEtapa(i);

            i = new FUNIL_ETAPA();
            i.FUNI_CD_ID = funilId;
            i.FUET_NM_NOME = "Proposta Enviada";
            i.FUET_SG_SIGLA = "PRE";
            i.FUET_DS_DESCRICAO = "Proposta enviada ao cliente";
            i.FUET_IN_ORDEM = 3;
            i.FUET_IN_ENCERRA = 0;
            i.FUET_IN_PROPOSTA = 1;
            i.FUET_IN_EMAIL = 1;
            i.FUET_IN_SMS = 1;
            i.FUET_IN_ATIVO = 1;
            volta = fuApp.ValidateCreateEtapa(i);

            i = new FUNIL_ETAPA();
            i.FUNI_CD_ID = funilId;
            i.FUET_NM_NOME = "Em Negociação";
            i.FUET_SG_SIGLA = "NEG";
            i.FUET_DS_DESCRICAO = "Negociação da proposta";
            i.FUET_IN_ORDEM = 4;
            i.FUET_IN_ENCERRA = 0;
            i.FUET_IN_PROPOSTA = 0;
            i.FUET_IN_EMAIL = 1;
            i.FUET_IN_SMS = 1;
            i.FUET_IN_ATIVO = 1;
            volta = fuApp.ValidateCreateEtapa(i);

            i = new FUNIL_ETAPA();
            i.FUNI_CD_ID = funilId;
            i.FUET_NM_NOME = "Encerrado";
            i.FUET_SG_SIGLA = "ENC";
            i.FUET_DS_DESCRICAO = "Processo encerrado";
            i.FUET_IN_ORDEM = 5;
            i.FUET_IN_ENCERRA = 1;
            i.FUET_IN_PROPOSTA = 0;
            i.FUET_IN_EMAIL = 1;
            i.FUET_IN_SMS = 1;
            i.FUET_IN_ATIVO = 1;
            volta = fuApp.ValidateCreateEtapa(i);

            // Motivo cancelamento
            MOTIVO_CANCELAMENTO j = new MOTIVO_CANCELAMENTO();
            j.ASSI_CD_ID = assi;
            j.MOCA_NM_NOME = "Desistência";
            j.MOCA_IN_ATIVO = 1;
            volta = mcApp.ValidateCreate(j, null);

            // Motivo encerramento
            MOTIVO_ENCERRAMENTO k = new MOTIVO_ENCERRAMENTO();
            k.ASSI_CD_ID = assi;
            k.MOEN_NM_NOME = "Sucesso";
            k.MOEN_IN_ATIVO = 1;
            volta = meApp.ValidateCreate(k, null);

            // Periodicidade
            PERIODICIDADE_TAREFA l = new PERIODICIDADE_TAREFA();
            l.ASSI_CD_ID = assi;
            l.PETA_NM_NOME = "Diária";
            l.PETA_NR_DIAS = 1;
            l.PETA_IN_ATIVO = 1;
            volta = peApp.ValidateCreate(l, usuario);

            l = new PERIODICIDADE_TAREFA();
            l.ASSI_CD_ID = assi;
            l.PETA_NM_NOME = "Semanal";
            l.PETA_NR_DIAS = 7;
            l.PETA_IN_ATIVO = 1;
            volta = peApp.ValidateCreate(l, usuario);

            l = new PERIODICIDADE_TAREFA();
            l.ASSI_CD_ID = assi;
            l.PETA_NM_NOME = "Mensal";
            l.PETA_NR_DIAS = 30;
            l.PETA_IN_ATIVO = 1;
            volta = peApp.ValidateCreate(l, usuario);

            l = new PERIODICIDADE_TAREFA();
            l.ASSI_CD_ID = assi;
            l.PETA_NM_NOME = "Semestral";
            l.PETA_NR_DIAS = 180;
            l.PETA_IN_ATIVO = 1;
            volta = peApp.ValidateCreate(l, usuario);

            l = new PERIODICIDADE_TAREFA();
            l.ASSI_CD_ID = item.ASSI_CD_ID;
            l.PETA_NM_NOME = "Anual";
            l.PETA_NR_DIAS = 365;
            l.PETA_IN_ATIVO = 1;
            volta = peApp.ValidateCreate(l, usuario);

            // Tipo Ação
            TIPO_ACAO m = new TIPO_ACAO();
            m.ASSI_CD_ID = assi;
            m.TIAC_NM_NOME = "Normal";
            m.TIAC_IN_ATIVO = 1;
            volta = taApp.ValidateCreate(m, null);

            // Tipo Tarefa
            TIPO_TAREFA x = new TIPO_TAREFA();
            x.ASSI_CD_ID = assi;
            x.TITR_NM_NOME = "Normal";
            x.TITR_IN_ATIVO = 1;
            volta = ttApp.ValidateCreate(x, null);

            // Usuario
            USUARIO n = new USUARIO();
            n.ASSI_CD_ID = assi;
            n.PERF_CD_ID = 2;
            n.CAUS_CD_ID = 1;
            n.CARG_CD_ID = carId;
            n.USUA_NM_NOME = item.ASSI_NM_NOME;
            n.USUA_NM_LOGIN = "ADM" + assi.ToString();
            n.USUA_NM_EMAIL = item.ASSI_NM_EMAIL;
            n.USUA_NR_TELEFONE = item.ASSI_NR_TELEFONE;
            n.USUA_NR_CELULAR = item.ASSI_NR_CELULAR;
            n.USUA_NM_SENHA = "ADM123";
            n.USUA_NM_SENHA_CONFIRMA = "ADM123";
            n.USUA_IN_BLOQUEADO = 0;
            n.USUA_IN_SISTEMA = 1;
            n.USUA_IN_ATIVO = 1;
            n.USUA_IN_LOGADO = 1;
            n.USUA_DT_ACESSO = DateTime.Today.Date;
            n.USUA_DT_CADASTRO = DateTime.Today.Date;
            n.USUA_NR_ACESSOS = 0;
            n.USUA_NR_FALHAS = 0;
            n.USUA_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
            n.USUA_NR_CPF = item.ASSI_NR_CPF;
            n.USUA_IN_COMPRADOR = 0;
            n.USUA_IN_APROVADOR = 0;
            n.USUA_IN_ESPECIAL = 1;
            n.USUA_IN_CRM = 1;
            n.USUA_IN_ERP = 1;
            n.USUA_IN_TECNICO = 1;
            volta = usuApp.ValidateCreate(n, usuario);
            USUARIO usua = usuApp.GetAllItens(assi).FirstOrDefault();
            Int32 usuId = usua.USUA_CD_ID;

            String caminho1 = "/Imagens/" + assi.ToString() + "/Usuario/" + usuId.ToString() + "/Fotos/";
            Directory.CreateDirectory(Server.MapPath(caminho1));
            caminho1 = "/Imagens/" + assi.ToString() + "/Usuario/" + usuId.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho1));

            // Notificação
            NOTIFICACAO o = new NOTIFICACAO();
            o.ASSI_CD_ID = assi;
            o.USUA_CD_ID = usuId;
            o.CANO_CD_ID = 1;
            o.NOTI_DT_EMISSAO = DateTime.Today;
            o.NOTI_IN_ATIVO = 1;
            o.NOTI_IN_STATUS = 1;
            o.NOTI_TX_TEXTO = "Sua assinatura foi ativada com sucesso";
            o.NOTI_NM_TITULO = "Ativação de Assinatura";
            o.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(180);
            o.NOTI_IN_ORIGEM = 1;
            o.NOTI_IN_NIVEL = 1;
            o.NOTI_IN_VISTA = 0;
            volta = noApp.ValidateCreate(o, usuario);

            // Enviar e-mail com instruções
            String texto = temApp.GetByCode("ASSCONFIRM").TEMP_TX_CORPO;
            List<ASSINANTE_PLANO> planos = item.ASSINANTE_PLANO.ToList();
            texto = texto.Replace("{nome}", item.ASSI_NM_NOME);
            texto = texto.Replace("{data}", DateTime.Today.Date.ToShortDateString());
            texto = texto.Replace("{user}", usua.USUA_NM_LOGIN);
            texto = texto.Replace("{senha}", usua.USUA_NM_SENHA);
            String rodape = String.Empty;
            foreach (ASSINANTE_PLANO plan in planos)
            {
                rodape += "Plano:" + plan.PLANO.PLAN_NM_NOME + ". Validade: " + plan.ASPL_DT_VALIDADE.Value.ToShortDateString() + "</ br>";
            }

            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.ASSI_NM_NOME;
            mens.ID = assi;
            mens.MODELO = item.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.LINK = "2";
            mens.MENS_TX_TEXTO = texto;
            mens.MENS_NM_RODAPE = rodape;
            Session["Assinante"] = item;
            volta = ProcessaEnvioEMailAssinante(mens, usuario);

            return 1;
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            ASSINANTE aten = baseApp.GetItemById((Int32)Session["IdAssinante"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Assinante" + aten.ASSI_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font meuFontRED = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

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
            Image image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Assinante - Detalhes", meuFont2))
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
                Image imagemCliente = Image.GetInstance(Server.MapPath(aten.ASSI_AQ_FOTO));
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
            cell = new PdfPCell(new Paragraph("Data Início: " + aten.ASSI_DT_INICIO.Value.ToShortDateString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.ASSI_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Razão Social: " + aten.ASSI_NM_RAZAO_SOCIAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.ASSI_NR_CPF != null)
            {
                cell = new PdfPCell(new Paragraph("CPF: " + aten.ASSI_NR_CPF, meuFont));
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

            if (aten.ASSI_NR_CNPJ != null)
            {
                cell = new PdfPCell(new Paragraph("CNPJ: " + aten.ASSI_NR_CNPJ, meuFont));
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

            cell = new PdfPCell(new Paragraph("Endereço: " + aten.ASSI_NM_ENDERECO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Número: " + aten.ASSI_NR_NUMERO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Complemento: " + aten.ASSI_NM_COMPLEMENTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Bairro: " + aten.ASSI_NM_BAIRRO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade: " + aten.ASSI_NM_CIDADE, meuFont));
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
            cell = new PdfPCell(new Paragraph("CEP: " + aten.ASSI_NR_CEP, meuFont));
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

            cell = new PdfPCell(new Paragraph("E-Mail: " + aten.ASSI_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Telefone: " + aten.ASSI_NR_TELEFONE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular: " + aten.ASSI_NR_CELULAR, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Lista de Planos
            if (aten.ASSINANTE_PLANO.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 100f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Planos de Assinaturas", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Plano", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ASSINANTE_PLANO item in aten.ASSINANTE_PLANO)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO.PLAN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.ASPL_DT_INICIO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.ASPL_DT_VALIDADE.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Pagamentos
            if (aten.ASSINANTE_PAGAMENTO.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 80f, 80f, 80f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Pagamentos", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 6;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Plano", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Atraso (Dias)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor Pago (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ASSINANTE_PAGAMENTO item in aten.ASSINANTE_PAGAMENTO)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO.PLAN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.ASPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ASPA_VL_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.ASPA_NR_ATRASO > 0)
                    {
                        cell = new PdfPCell(new Paragraph(item.ASPA_NR_ATRASO.Value.ToString(), meuFontRED))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph(item.ASPA_NR_ATRASO.Value.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.ASPA_DT_PAGAMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.ASPA_DT_PAGAMENTO.Value.ToShortDateString(), meuFont))
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
                    if (item.ASPA_VL_VALOR_PAGO != null)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ASPA_VL_VALOR_PAGO.Value), meuFont))
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
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);


            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.ASSI_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
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

            return RedirectToAction("VoltarAnexoAssinante");
        }

        public List<ASSINANTE> CarregaAssinante()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<ASSINANTE> conf = new List<ASSINANTE>();
            if (Session["Assinantes"] == null)
            {
                conf = baseApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["AssinanteAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens();
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

        public List<ASSINANTE> CarregaAssinanteAdm()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<ASSINANTE> conf = new List<ASSINANTE>();
            if (Session["AssinantesGeral"] == null)
            {
                conf = baseApp.GetAllItensAdm();
            }
            else
            {
                if ((Int32)Session["AssinanteAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens();
                }
                else
                {
                    conf = (List<ASSINANTE>)Session["AssinantesGeral"];
                }
            }
            Session["AssinantesGeral"] = conf;
            Session["AssinanteAlterada"] = 0;
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

        public List<PLANO> CarregaPlano()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PLANO> conf = new List<PLANO>();
            if (Session["Planos"] == null)
            {
                conf = baseApp.GetAllPlanos();
            }
            else
            {
                if ((Int32)Session["PlanoAlterada"] == 1)
                {
                    conf = baseApp.GetAllPlanos();
                }
                else
                {
                    conf = (List<PLANO>)Session["Planos"];
                }
            }
            Session["Planos"] = conf;
            Session["PlanoAlterada"] = 0;
            return conf;
        }

        public ActionResult VerMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMensagensEnviadas"] = 5;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

    }
}