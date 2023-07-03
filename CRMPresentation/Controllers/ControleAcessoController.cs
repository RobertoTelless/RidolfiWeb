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
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using CrossCutting;
using System.Text;
using System.Net;
using System.Windows.Input;
using Azure.Communication.Email;
using CaptchaMvc.HtmlHelpers;
using ERP_Condominios_Solution.Classes;

namespace ERP_Condominios_Solution.Controllers
{
    public class ControleAcessoController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateAppService temApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public ControleAcessoController(IUsuarioAppService baseApps, IConfiguracaoAppService confApps, ITemplateAppService temApps)
        {
            baseApp = baseApps;
            confApp = confApps;
            temApp = temApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            USUARIO item = new USUARIO();
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        private void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Log4Net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }

        public ActionResult AtualizarCaptcha()
        {

            String texto = string.Empty;
            Random randNum = new Random();

            Session["Captcha"] = randNum.Next(10000, 99999).ToString();
            ViewBag.Captcha = "~/Handlers/ghCaptcha.ashx?" + Session["Captcha"];

            return RedirectToAction("Login", "ControleAcesso");
        }

        public void MontaSessao()
        {
            Session["Close"] = false;
            Session["MensEnvioLogin"] = 0;
            Session["Ativa"] = "1";
            Session["Configuracao"] = null;
            Session["ConfAlterada"] = 0;
            Session["Usuarios"] = null;
            Session["UsuariosAdm"] = null;
            Session["UsuarioAlterada"] = 0;
            Session["MensLog"] = 0;
            Session["Agendas"] = null;
            Session["AgendaAlterada"] = 0;
            Session["Tarefas"] = null;
            Session["TarefaAlterada"] = 0;
            Session["Noticias"] = null;
            Session["NoticiaGeral"] = null;
            Session["NoticiaAlterada"] = 0;
            Session["Notificacoes"] = null;
            Session["NotificacaoAlterada"] = 0;
            Session["Videos"] = null;
            Session["VideosGeral"] = null;
            Session["VideoAlterada"] = 0;
            Session["Planos"] = null;
            Session["PlanosGeral"] = null;
            Session["PlanosCarga"] = null;
            Session["PlanoAlterada"] = 0;
            Session["Clientes"] = null;
            Session["ClientesGeral"] = null;
            Session["ClienteAlterada"] = 0;
            Session["Grupos"] = null;
            Session["GrupoAlterada"] = 0;
            Session["Filiais"] = null;
            Session["FilialAlterada"] = 0;
            Session["TemplatesProposta"] = null;
            Session["TemplatesPropostaAlterada"] = 0;
            Session["UF"] = null;
            Session["TipoPessoa"] = null;
            Session["Sexos"] = null;
            Session["CatClientes"] = null;
            Session["CatClienteAlterada"] = 0;
            Session["Assinantes"] = null;
            Session["AssinantesGeral"] = null;
            Session["AssinanteAlterada"] = 0;
            Session["CatNotificacoes"] = null;
            Session["CatNotificacaoAlterada"] = 0;
            Session["CatAgendas"] = null;
            Session["CatAgendaAlterada"] = 0;
            Session["TipoTarefas"] = null;
            Session["TipoTarefaAlterada"] = 0;
            Session["Periodicidades"] = null;
            Session["PeriodicidadeAlterada"] = 0;
            Session["Perfis"] = null;
            Session["Cargos"] = null;
            Session["CargoAlterada"] = 0;
            Session["Funis"] = null;
            Session["FunilAlterada"] = 0;
            Session["PeriodicidadePlanos"] = null;
            Session["PeriodicidadePlanoAlterada"] = 0;
            Session["TipoContribuintes"] = null;
            Session["RegimeTributarios"] = null;
            Session["CRMs"] = null;
            Session["CRMsGeral"] = null;
            Session["CRMAlterada"] = 0;
            Session["Origens"] = null;
            Session["OrigemAlterada"] = 0;
            Session["MotCancelamentos"] = null;
            Session["MotCancelamentoAlterada"] = 0;
            Session["MotEncerramentos"] = null;
            Session["MotEncerramentoAlterada"] = 0;
            Session["PedidoVendas"] = null;
            Session["PedidoVendaAlterada"] = 0;
            Session["PedidoVendasGeral"] = null;
            Session["TipoAcoes"] = null;
            Session["TipoAcaoAlterada"] = 0;
            Session["TemplatePropostas"] = null;
            Session["TemplatePropostaAlterada"] = 0;
            Session["FlagAlteraCliente"] = 0;
            Session["Pesquisas"] = null;
            Session["PesquisaAlterada"] = 0;
            Session["TipoPesquisas"] = null;
            Session["TipoPesquisaAlterada"] = 0;
            Session["NumPesquisas"] = 0;
            Session["NumPesquisas"] = 0;
            Session["MensagensEnviadas"] = null;
            Session["MensagemEnviadaAlterada"] = 0;
            Session["FlagMensagensEnviadas"] = 0;
            Session["TipoItemPesquisas"] = null;
            Session["TipoItemPesquisaAlterada"] = 0;
            Session["Respostas"] = null;
            Session["RespostasAlterada"] = 0;
            Session["SMSPadrao"] = null;
            Session["SMSPadraoAlterada"] = 0;
            Session["SMSPrior"] = null;
            Session["SMSPriorAlterada"] = 0;
            Session["EMails"] = null;
            Session["EMailAlterada"] = 0;
            Session["UsuarioAlterada"] = 0;
            Session["TemplatesEMail"] = null;
            Session["TemplatesEMailAlterada"] = 0;
            Session["TemplatesSMS"] = null;
            Session["TemplatesSMSAlterada"] = 0;
            Session["Recursividades"] = null;
            Session["RecursividadeAlterada"] = 0;
            Session["Mensagens"] = null;
            Session["MensagemAlterada"] = 0;
            Session["Nacionalidades"] = null;
            Session["NacionalidadeAlterada"] = 0;
            Session["Municipios"] = null;
            Session["MunicipioAlterada"] = 0;
            Session["Precatorios"] = null;
            Session["PrecatorioAlterada"] = 0;
            Session["Beneficiarios"] = null;
            Session["BeneficiarioAlterada"] = 0;
            Session["TRFs"] = null;
            Session["TRFAlterada"] = 0;
            Session["Varas"] = null;
            Session["VaraAlterada"] = 0;
            Session["Honorarios"] = null;
            Session["HonorarioAlterada"] = 0;
        }

        [HttpGet]
        public ActionResult Login()
        {
            // Mensagens
            if (Session["MensagemLogin"] != null)
            {
                if ((Int32)Session["MensagemLogin"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0211", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 13)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 15)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 16)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 17)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 18)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 19)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 20)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0012", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 21)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0228", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 24)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                }
            }

            // Exibe tela
            Session["MensSenha"] = null;
            Session["UserCredentials"] = null;
            MontaSessao();
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            vm.CapImage = "data:image/png;base64," + Convert.ToBase64String(new CaptchaUtility().VerificationTextGenerator());
            vm.CapImageText = Convert.ToString(Session["Captcha"]);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario;
                Session["UserCredentials"] = null;
                ViewBag.Usuario = null;
                Session["MensSenha"] = 0;
                Session["MensagemLogin"] = 0;

                if (vm.CaptchaCodeText != (String)Session["Captcha"])
                {
                    Session["MensagemLogin"] = 10;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }

                USUARIO login = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateLogin(login.USUA_NM_LOGIN, login.USUA_NM_SENHA, out usuario);
                Session["UserCredentials"] = usuario;
                if (volta == 1)
                {
                    Session["MensagemLogin"] = 11;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 2)
                {
                    Session["MensagemLogin"] = 12;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 3)
                {
                    Session["MensagemLogin"] = 13;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 5)
                {
                    Session["MensagemLogin"] = 14;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 4)
                {
                    Session["MensagemLogin"] = 15;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 6)
                {
                    Session["MensagemLogin"] = 16;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 7)
                {
                    Session["MensagemLogin"] = 17;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 9)
                {
                    Session["MensagemLogin"] = 18;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 10)
                {
                    Session["MensagemLogin"] = 19;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 11)
                {
                    Session["MensagemLogin"] = 20;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0012", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 20)
                {
                    Session["MensagemLogin"] = 21;
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 22)
                {
                    Session["MensagemLogin"] = 22;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0228", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 40)
                {
                    Session["MensagemLogin"] = 24;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Armazena credenciais para autorização
                Session["UserCredentials"] = usuario;
                Session["Usuario"] = usuario;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["Assinante"] = usuario.ASSINANTE;
                Session["PlanosVencidos"] = null;
                Session["IdEmpresa"] = 3;

                // Reseta flags de permissao e totais
                Session["PermMens"] = 0;
                Session["PermCRM"] = 0;
                Session["PermPesquisa"] = 0;
                Session["PermAtendimentos"] = 0;
                Session["NumSMS"] = 0;
                Session["NumSMSPrior"] = 0;
                Session["NumEMail"] = 0;
                Session["NumZap"] = 0;
                Session["NumClientes"] = 0;
                Session["NumProcessos"] = 0;
                Session["NumPesquisas"] = 0;
                Session["NumProcessosBase"] = 0;
                Session["NumUsuarios"] = 0;
                Session["MensagemLogin"] = 0;
                Session["NumPesquisas"] = 0;
                Session["NumAtendimentos"] = 0;
                Session["MensPermissao"] = 0;
                Session["PermComercial"] = 0;

                // Recupera Planos do assinante
                List<PlanoVencidoViewModel> vencidos = new List<PlanoVencidoViewModel>();
                List<ASSINANTE_PLANO> plAss = usuario.ASSINANTE.ASSINANTE_PLANO.ToList();
                plAss = plAss.Where(p => p.ASPL_IN_ATIVO == 1).ToList();
                
                List<PLANO> planos = new List<PLANO>();
                if (plAss.Count == 0)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0215", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                foreach (ASSINANTE_PLANO item in plAss)
                {
                    // Verifica validade
                    if (item.ASPL_DT_VALIDADE < DateTime.Today.Date)
                    {
                        PlanoVencidoViewModel plan = new PlanoVencidoViewModel();
                        plan.Plano = item.PLAN_CD_ID;
                        plan.Assinante = item.ASSI_CD_ID;
                        plan.Vencimento = item.ASPL_DT_VALIDADE;
                        plan.Tipo = 1;
                        vencidos.Add(plan);                      
                        continue;
                    }
                    if (item.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30))
                    {
                        PlanoVencidoViewModel plan = new PlanoVencidoViewModel();
                        plan.Plano = item.PLAN_CD_ID;
                        plan.Assinante = item.ASSI_CD_ID;
                        plan.Vencimento = item.ASPL_DT_VALIDADE;
                        plan.Tipo = 2;
                        vencidos.Add(plan);
                    }

                    // Processa planos
                    planos.Add(item.PLANO);
                    Session["NumeroEmpresas"] = item.PLANO.PLAN_NR_EMPRESA;
                    Session["NumUsuarios"] = item.PLANO.PLAN_NR_USUARIOS;
                    Session["PermMens"] = 1;
                    Session["NumSMS"] = 100000;
                    Session["NumSMSPrior"] = 100000;
                    Session["NumEMail"] = 100000;
                    Session["NumZap"] = 100000;
                    Session["NumClientes"] = 1000000;
                    Session["PermCRM"] = 1;
                    Session["NumProcessos"] = 100000;
                    Session["NumProcessosBase"] = 100000;
                    Session["PermPesquisa"] = 1;
                    Session["PermAtendimentos"] = 1;
                    Session["PermComercial"] = 1;
                }
                Int32[] permissoes = new Int32[] { (Int32)Session["PermMens"], (Int32)Session["PermCRM"], (Int32)Session["PermPesquisa"], (Int32)Session["PermAtendimentos"], (Int32)Session["PermComercial"] };
                Session["Permissoes"] = permissoes;

                // Verifica Acesso
                Session["PlanosVencidosModel"] = vencidos;
                if (planos.Count == 0)
                {
                    Session["MensagemLogin"] = 1;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Session["Planos"] = planos;
                Session["ListaMensagem"] = null;

                // Atualiza view
                String frase = String.Empty;
                String nome = usuario.USUA_NM_NOME;
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

                ViewBag.Greeting = frase;
                ViewBag.Nome = usuario.USUA_NM_NOME;
                if (usuario.CARGO != null)
                {
                    ViewBag.Cargo = usuario.CARGO.CARG_NM_NOME;
                    Session["Cargo"] = usuario.CARGO.CARG_NM_NOME;
                }
                else
                {
                    ViewBag.Cargo = "-";
                    Session["Cargo"] = "-";
                }
                ViewBag.Foto = usuario.USUA_AQ_FOTO;

                // Trata Nome
                String nomeMax = String.Empty;
                if (usuario.USUA_NM_NOME.Contains(" "))
                {
                    nomeMax = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
                }
                else
                {
                    nomeMax = usuario.USUA_NM_NOME;
                }

                // Carrega Sessions
                Session["NomeMax"] = nomeMax;
                Session["Greeting"] = frase;
                Session["Nome"] = usuario.USUA_NM_NOME;
                Session["Foto"] = usuario.USUA_AQ_FOTO;
                Session["Perfil"] = usuario.PERFIL;
                Session["PerfilSigla"] = usuario.PERFIL.PERF_SG_SIGLA;
                Session["FlagInicial"] = 0;
                Session["FiltroData"] = 1;
                Session["FiltroStatus"] = 1;
                Session["Ativa"] = "1";
                Session["Login"] = 1;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["IdUsuario"] = usuario.USUA_CD_ID;

                // Grava flag de logado
                usuario.USUA_IN_LOGADO = 1;
                Int32 volta5 = baseApp.ValidateEdit(usuario, usuario);

                // Route
                Session["MensSenha"] = 0;
                if (volta == 30)
                {
                    Session["MensagemLogin"] = 23;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0262", CultureInfo.CurrentCulture));
                    return RedirectToAction("TrocarSenhaCodigo", "ControleAcesso");
                }
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    Session["MensSenha"] = 22;
                    return RedirectToAction("TrocarSenhaInicio", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult Logout()
        {
            // Grava flag de saida
            Session["UserCredentials"] = null;
            Session.Clear();
            Session["MensEnvioLogin"] = 0;
            return RedirectToAction("Login", "ControleAcesso");
        }

        public ActionResult Cancelar()
        {
            return RedirectToAction("Login", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult TrocarSenha()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenha(USUARIO item)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(item);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(item);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(item);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(item);
                }

                // Grava dados no usuario
                Session["UserCredentials"] = item;
                Session["CodigoSenha"] = item.USUA_SG_CODIGO;
                Session["Senha"] = item.USUA_NM_SENHA;

                // Envia mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = item.USUA_NM_NOME;
                mens.ID = item.USUA_CD_ID;
                mens.MODELO = item.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MODELO = item.USUA_SG_CODIGO;
                mens.MENS_TX_TEXTO = temApp.GetByCode("TROCASENHA").TEMP_TX_CORPO; 
                Int32 xx = ProcessaEnvioEMailSenha(mens, item);


                // Retorno
                Session["MensSenha"] = 10;
                return RedirectToAction("TrocarSenhaCodigo", "ControleAcesso");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailSenha(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + usuario.USUA_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>RidolfiWeb</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            corpo = corpo.Replace("{Codigo}", vm.MODELO);
            corpo = corpo.Replace("{duracao}", conf.CONF_IN_VALIDADE_CODIGO.ToString());

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
            String status = "Succeeded";
            String iD = "xyz";
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Troca de Senha";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "CRMSys";
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
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }

            return 0;
        }

        [HttpGet]
        public ActionResult TrocarSenhaCodigo()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();

            // Mensagens
            if (Session["MensSenha"] != null)
            {
                if ((Int32)Session["MensSenha"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0233", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSenha"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0234", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSenha"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0235", CultureInfo.CurrentCulture) + usuario.USUA_NM_EMAIL);
                }
                if ((Int32)Session["MensSenha"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0261", CultureInfo.CurrentCulture));
                    return RedirectToAction("Logout", "ControleAcesso");
                }
            }

            // Monta view
            Session["MensSenha"] = 0;
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            usu.USUA_NR_MATRICULA = String.Empty;
            return View(usu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenhaCodigo(USUARIO vm)
        {
            try
            {
                // Valida codigo
                CONFIGURACAO conf = confApp.GetItemById(vm.ASSI_CD_ID);
                String codigo = vm.USUA_SG_CODIGO;
                if (vm.USUA_NR_MATRICULA == null)
                {
                    Session["MensSenha"] = 2;
                    return RedirectToAction("TrocarSenhaCodigo", "ControleAcesso");
                }
                if (vm.USUA_NR_MATRICULA != codigo.Trim())
                {
                    Session["MensSenha"] = 1;
                    return RedirectToAction("TrocarSenhaCodigo", "ControleAcesso");
                }
                DateTime limite = vm.USUA_DT_CODIGO.Value.AddMinutes(conf.CONF_IN_VALIDADE_CODIGO.Value);
                if (limite < DateTime.Now)
                {
                    Session["MensSenha"] = 3;
                    return RedirectToAction("TrocarSenhaCodigo", "ControleAcesso");
                }

                // Processa
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateChangePasswordFinal(vm);

                // Gera mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = vm.USUA_NM_NOME;
                mens.ID = vm.USUA_CD_ID;
                mens.MODELO = vm.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MODELO = codigo;
                mens.MENS_TX_TEXTO = temApp.GetByCode("CONFSENHA").TEMP_TX_CORPO;
                Int32 xx = ProcessaEnvioEMailSenha(mens, vm);

                // Retorno
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult GerarSenha()
        {
            // Mensagens
            if (Session["MensSenha"] != null)
            {
                // Mensagens
                if ((Int32)Session["MensSenha"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSenha"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0096", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSenha"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSenha"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                }
            }

            // Abre tela
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                // Processa
                Session["UserCredentials"] = null;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.GenerateNewPassword(item.USUA_NM_EMAIL);
                if (volta != 0)
                {
                    Session["MensSenha"] = volta;
                    return RedirectToAction("GerarSenha", "ControleAcesso");
                }
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaInicio()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usuario);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenhaInicio(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", CRMSys_Import.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                Session["UserCredentials"] = item;
                Session["CodigoSenha"] = item.USUA_SG_CODIGO;
                Session["Senha"] = item.USUA_NM_SENHA;

                // Envia mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = item.USUA_NM_NOME;
                mens.ID = item.USUA_CD_ID;
                mens.MODELO = item.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MODELO = item.USUA_SG_CODIGO;
                mens.MENS_TX_TEXTO = temApp.GetByCode("TROCASENHA").TEMP_TX_CORPO;
                Int32 xx = ProcessaEnvioEMailSenha(mens, item);

                // Retorno
                Session["MensSenha"] = 10;
                return RedirectToAction("TrocarSenhaCodigoInicio", "ControleAcesso");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaCodigoInicio()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Mensagens
            if ((Int32)Session["MensSenha"] == 1)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0233", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensSenha"] == 2)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0234", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensSenha"] == 10)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0235", CultureInfo.CurrentCulture) + usuario.USUA_NM_EMAIL);
            }

            // Monta view
            Session["MensSenha"] = 0;
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            usu.USUA_NR_MATRICULA = String.Empty;
            return View(usu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenhaCodigoInicio(USUARIO vm)
        {
            try
            {
                // Valida codigo
                CONFIGURACAO conf = confApp.GetItemById(vm.ASSI_CD_ID);
                String codigo = vm.USUA_SG_CODIGO;
                if (vm.USUA_NR_MATRICULA == null)
                {
                    Session["MensSenha"] = 2;
                    return RedirectToAction("TrocarSenhaCodigoInicio", "ControleAcesso");
                }
                if (vm.USUA_NR_MATRICULA != codigo.Trim())
                {
                    Session["MensSenha"] = 1;
                    return RedirectToAction("TrocarSenhaCodigoInicio", "ControleAcesso");
                }
                DateTime limite = vm.USUA_DT_CODIGO.Value.AddMinutes(conf.CONF_IN_VALIDADE_CODIGO.Value);
                if (limite < DateTime.Now)
                {
                    Session["MensSenha"] = 3;
                    return RedirectToAction("TrocarSenhaCodigoInicio", "ControleAcesso");
                }

                // Processa
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateChangePasswordFinal(vm);

                // Gera mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = vm.USUA_NM_NOME;
                mens.ID = vm.USUA_CD_ID;
                mens.MODELO = vm.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MODELO = codigo;
                mens.MENS_TX_TEXTO = temApp.GetByCode("CONFSENHA").TEMP_TX_CORPO;
                Int32 xx = ProcessaEnvioEMailSenha(mens, vm);

                // Retorno
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "RidolfiWeb", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }
    }
}