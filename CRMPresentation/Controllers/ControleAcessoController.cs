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
using System.Diagnostics;
using System.Threading.Tasks;

namespace ERP_Condominios_Solution.Controllers
{
    public class ControleAcessoController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateAppService temApp;
        private readonly IAssinanteAppService assApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public ControleAcessoController(IUsuarioAppService baseApps, IConfiguracaoAppService confApps, ITemplateAppService temApps, IAssinanteAppService assApps)
        {
            baseApp = baseApps;
            confApp = confApps;
            temApp = temApps;
            assApp = assApps;
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
            Session["Planos"] = null;
            Session["PlanosGeral"] = null;
            Session["PlanosCarga"] = null;
            Session["PlanoAlterada"] = 0;
            Session["UF"] = null;
            Session["TipoPessoa"] = null;
            Session["Assinantes"] = null;
            Session["AssinantesGeral"] = null;
            Session["AssinanteAlterada"] = 0;
            Session["Periodicidades"] = null;
            Session["PeriodicidadeAlterada"] = 0;
            Session["Perfis"] = null;
            Session["Cargos"] = null;
            Session["CargoAlterada"] = 0;
            Session["PeriodicidadePlanos"] = null;
            Session["PeriodicidadePlanoAlterada"] = 0;
            Session["RegimeTributarios"] = null;
            Session["Empresas"] = null;
            Session["EmpresaAlterada"] = 0;
            Session["Regimes"] = null;
            Session["RegimeAlterada"] = 0;
            Session["Agendas"] = null;
            Session["AgendaAlterada"] = 0;
            Session["Tarefas"] = null;
            Session["TarefaAlterada"] = 0;
            Session["Noticias"] = null;
            Session["NoticiaGeral"] = null;
            Session["NoticiaAlterada"] = 0;
            Session["Notificacoes"] = null;
            Session["NotificacaoAlterada"] = 0;
            Session["CatNotificacoes"] = null;
            Session["CatNotificacaoAlterada"] = 0;
            Session["CatAgendas"] = null;
            Session["CatAgendaAlterada"] = 0;
            Session["TipoTarefas"] = null;
            Session["TipoTarefaAlterada"] = 0;
            Session["MensEnvioLogin"] = 0;
            Session["UsuarioAlterada"] = 0;
            Session["Calendarios"] = null;
            Session["CalendarioAlterada"] = 0;
            Session["NotificacoesUsuario"] = null;
            Session["NotificacaoUsuarioAlterada"] = 0;
            Session["MensagensEnviadas"] = null;
            Session["MensagensEnviadaAlterada"] = 0;
            Session["Clientes"] = null;
            Session["ClienteAlterada"] = 0;
            Session["CRMPedidos"] = null;
            Session["CRMPedidoAlterada"] = 0;
            Session["FlagNotificacoes"] = 1;
            Session["FlagTarefas"] = 1;
            Session["FlagAgendas"] = 1;
            Session["FlagCliente"] = 1;
            Session["Clientes"] = null;
            Session["ClienteAlterada"] = 0;
            Session["FlagCRM"] = 1;
            Session["FlagCRMPedido"] = 1;
            Session["CRMs"] = null;
            Session["CRMAlterada"] = 0;
            Session["FlagCRMPedido"] = 1;
            Session["CRMPedidos"] = null;
            Session["CRMPedidoAlterada"] = 0;
            Session["MensPermissao"] = 0;
            Session["ModuloPermissao"] = 0;
            Session["ModeloEMails"] = null;
            Session["ModeloEMailAlterada"] = 0;
            Session["ModeloPropostas"] = null;
            Session["ModeloPropostaAlterada"] = 0;
            Session["Vence30"] = "Não";
            Session["CRMAcoes"] = null;
            Session["CRMAcaoAlterada"] = 0;
            Session["CRMPedidoAlterada"] = 1;
            Session["PlanosLista"] = null;
            Session["CRMAcoesAss"] = null;
            Session["PedidoVendasAss"] = null;
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
            Session["Contatos"] = null;
            Session["ContatoAlterada"] = 0;
            Session["Qualificacoes"] = null;
            Session["QualificacaoAlterada"] = 0;
            Session["Desligas"] = null;
            Session["DesligaAlterada"] = 0;
            Session["Escolaridades"] = null;
            Session["Naturezas"] = null;
            Session["EstadosCivil"] = null;
            Session["Parentescos"] = null;
            Session["Sexos"] = null;
            Session["FlagBeneficiario"] = 1;
            Session["FlagPrecatorio"] = 1;

            Session["ListaCliente"] = null;
            Session["ListaClienteUF"] = null;
            Session["ListaClienteCidade"] = null;
            Session["ListaClienteCats"] = null;
            Session["ListaCRMMesResumo"] = null;
            Session["ListaCRMMeses"] = null;
            Session["ListaPedidosMesResumo"] = null;
            Session["ListaPedidosMeses"] = null;
            Session["ListaCRMSituacao"] = null;
            Session["ListaCRMStatus"] = null;
            Session["ListaCRMAcao"] = null;
            Session["ListaFunilResumo"] = null;
            Session["ListaCRMPed"] = null;
            Session["ListaEtapaResumo"] = null;
            Session["ListaPendencia"] = null;
            Session["ListaAtraso"] = null;
            Session["ListaLemb"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaClientePedido"] = null;
            Session["ListaClienteData"] = null;
            Session["ListaClienteMes"] = null;
            Session["ListaClienteAnivDia"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaAlerta"] = null;
            Session["ListaBeneficiario"] = null;
            Session["ListaBeneficiarioUF"] = null;
            Session["ListaBeneficiarioCidade"] = null;
            Session["ListaPrecatorio"] = null;
            Session["ListaPrecatorioTRF"] = null;



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
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0322", CultureInfo.CurrentCulture));
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
                if ((Int32)Session["MensagemLogin"] == 55)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0330", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 99)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0332", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 90)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0341", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 91)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0342", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 92)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0344", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 93)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0345", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 94)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0343", CultureInfo.CurrentCulture));
                }
            }

            // Exibe tela
            Session.Clear();
            Session["MensSenha"] = null;
            Session["MensagemLogin"] = 0;
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
                // Inicialização
                USUARIO usuario;
                Session["UserCredentials"] = null;
                ViewBag.Usuario = null;
                Session["MensSenha"] = 0;
                Session["MensagemLogin"] = 0;
                String senha = vm.USUA_NM_SENHA;

                // Verifica captcha
                if (vm.CaptchaCodeText != (String)Session["Captcha"])
                {
                    Session["MensagemLogin"] = 10;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Sanitização
                vm.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_LOGIN);
                vm.USUA_NM_SENHA = CrossCutting.UtilitariosGeral.CleanStringSenha(vm.USUA_NM_SENHA);

                // Valida credenciais
                Int32 volta = baseApp.ValidateLogin(vm.USUA_NM_LOGIN, vm.USUA_NM_SENHA, out usuario);
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
                Session["IdEmpresa"] = usuario.EMPR_CD_ID;
                Session["Empresa"] = usuario.EMPRESA;
                Session["NomeEmpresa"] = usuario.EMPRESA.EMPR_NM_NOME;
                Session["PerfilUsuario"] = usuario.PERFIL;
                PERFIL perfil = usuario.PERFIL;
                Session["Perfil"] = usuario.PERFIL;
                Session["PerfilSigla"] = usuario.PERFIL.PERF_SG_SIGLA;
                Session["NomeEmpresaAssina"] = usuario.EMPRESA.EMPR_NM_NOME;

                // Reseta flags de permissao e totais
                Session["FlagNotificacoes"] = 1;
                Session["FlagTarefas"] = 1;
                Session["FlagAgendas"] = 1;

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

                // Prepara ambiente
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
                Session["MensPermisao"] = 0;
                Session["FlagCustoFixo"] = 1;
                Session["ListaMensagem"] = null;

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
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult Logout()
        {
            // Grava flag de saida
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Session["MensagemLogin"] = 99;

            Session["UserCredentials"] = null;
            Session["MensEnvioLogin"] = 0;
            return RedirectToAction("Login", "ControleAcesso");
        }

        public ActionResult Cancelar()
        {
            return RedirectToAction("Logout", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult TrocarSenha()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenha(USUARIO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

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
                Int32 xx = await ProcessaEnvioEMailSenha(mens, item);

                // Retorno
                Session["MensSenha"] = 10;
                return RedirectToAction("TrocarSenhaCodigo", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailSenha(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return 0;
                }

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
                mensagem.NOME_EMISSOR = "RidolfiWeb";
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
                    LogError(ex.Message);
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Acesso";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaCodigo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenhaCodigo(USUARIO vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                vm.USUA_SG_CODIGO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_SG_CODIGO);

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
                Int32 xx = await ProcessaEnvioEMailSenha(mens, vm);

                // Retorno
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult GerarSenha()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GerarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                vm.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_NM_EMAIL);

                // Processa
                Session["UserCredentials"] = null;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = await baseApp.GenerateNewPassword(item.USUA_NM_EMAIL);
                if (volta != 0)
                {
                    Session["MensSenha"] = volta;
                    return RedirectToAction("GerarSenha", "ControleAcesso");
                }
                Session["MensagemLogin"] = 55;

                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaInicio()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenhaInicio(UsuarioLoginViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

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
                Int32 xx = await ProcessaEnvioEMailSenha(mens, item);

                // Retorno
                Session["MensSenha"] = 10;
                return RedirectToAction("TrocarSenhaCodigoInicio", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaCodigoInicio()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenhaCodigoInicio(USUARIO vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Sanitização
                vm.USUA_SG_CODIGO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_SG_CODIGO);

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
                Int32 xx = await ProcessaEnvioEMailSenha(mens, vm);

                // Retorno
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }
    }
}