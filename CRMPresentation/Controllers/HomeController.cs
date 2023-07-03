using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CRMPresentation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
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
            Session["TipoFollows"] = null;
            Session["TipoFollowAlterada"] = 0;
            Session["Empresas"] = null;
            Session["EmpresaAlterada"] = 0;
            Session["Precatorios"] = null;
            Session["PrecatorioAlterada"] = 0;
            Session["Beneficiarios"] = null;
            Session["BeneficiarioAlterada"] = 0;
            Session["TRFs"] = null;
            Session["TRFAlterada"] = 0;
            return RedirectToAction("Login", "ControleAcesso");
            //return RedirectToAction("CarregarLandingPage", "BaseAdmin");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}