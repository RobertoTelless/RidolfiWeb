using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Web.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using ModelServices.Interfaces.Repositories;
using ApplicationServices.Services;
using ModelServices.EntitiesServices;
using DataServices.Repositories;
using Ninject.Web.Common.WebHost;
using DateTimeExtensions.NaturalText;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.Start.NinjectWebCommons), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Presentation.Start.NinjectWebCommons), "Stop")]

namespace Presentation.Start
{
    public class NinjectWebCommons
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IAppServiceBase<>)).To(typeof(AppServiceBase<>));
            kernel.Bind<IUsuarioAppService>().To<UsuarioAppService>();
            kernel.Bind<ILogAppService>().To<LogAppService>();
            kernel.Bind<IConfiguracaoAppService>().To<ConfiguracaoAppService>();
            kernel.Bind<INotificacaoAppService>().To<NotificacaoAppService>();
            kernel.Bind<ITemplateAppService>().To<TemplateAppService>();
            kernel.Bind<IAssinanteAppService>().To<AssinanteAppService>();
            kernel.Bind<IClienteAppService>().To<ClienteAppService>();
            kernel.Bind<IClienteCnpjAppService>().To<ClienteCnpjAppService>();
            kernel.Bind<IMensagemAppService>().To<MensagemAppService>();
            kernel.Bind<IGrupoAppService>().To<GrupoAppService>();
            kernel.Bind<ICategoriaClienteAppService>().To<CategoriaClienteAppService>();
            kernel.Bind<ICRMAppService>().To<CRMAppService>();
            kernel.Bind<IAgendaAppService>().To<AgendaAppService>();
            kernel.Bind<ITemplateSMSAppService>().To<TemplateSMSAppService>();
            kernel.Bind<ITemplateEMailAppService>().To<TemplateEMailAppService>();
            kernel.Bind<INoticiaAppService>().To<NoticiaAppService>();
            kernel.Bind<ITarefaAppService>().To<TarefaAppService>();
            kernel.Bind<ITipoPessoaAppService>().To<TipoPessoaAppService>();
            kernel.Bind<ICategoriaTelefoneAppService>().To<CategoriaTelefoneAppService>();
            kernel.Bind<ICargoAppService>().To<CargoAppService>();
            kernel.Bind<ICRMOrigemAppService>().To<CRMOrigemAppService>();
            kernel.Bind<IMotivoCancelamentoAppService>().To<MotivoCancelamentoAppService>();
            kernel.Bind<IMotivoEncerramentoAppService>().To<MotivoEncerramentoAppService>();
            kernel.Bind<ITipoAcaoAppService>().To<TipoAcaoAppService>();
            kernel.Bind<IPeriodicidadeAppService>().To<PeriodicidadeAppService>();
            kernel.Bind<IMensagemAutomacaoAppService>().To<MensagemAutomacaoAppService>();
            kernel.Bind<IEMailAgendaAppService>().To<EMailAgendaAppService>();
            kernel.Bind<IVideoAppService>().To<VideoAppService>();
            kernel.Bind<ITemplatePropostaAppService>().To<TemplatePropostaAppService>();
            kernel.Bind<ICRMDiarioAppService>().To<CRMDiarioAppService>();
            kernel.Bind<IFunilAppService>().To<FunilAppService>();
            kernel.Bind<IAssinanteCnpjAppService>().To<AssinanteCnpjAppService>();
            kernel.Bind<IPlanoAppService>().To<PlanoAppService>();
            kernel.Bind<ICategoriaAgendaAppService>().To<CategoriaAgendaAppService>();
            kernel.Bind<ITipoTarefaAppService>().To<TipoTarefaAppService>();
            kernel.Bind<IMensagemEnviadaSistemaAppService>().To<MensagemEnviadaSistemaAppService>();
            kernel.Bind<IRecursividadeAppService>().To<RecursividadeAppService>();
            kernel.Bind<ITipoFollowAppService>().To<TipoFollowAppService>();
            kernel.Bind<IBancoAppService>().To<BancoAppService>();
            kernel.Bind<IEscolaridadeAppService>().To<EscolaridadeAppService>();
            kernel.Bind<IParentescoAppService>().To<ParentescoAppService>();
            kernel.Bind<IBeneficiarioAppService>().To<BeneficiarioAppService>();
            kernel.Bind<ITabelaIPCAAppService>().To<TabelaIPCAAppService>();
            kernel.Bind<ITabelaIRRFAppService>().To<TabelaIRRFAppService>();
            kernel.Bind<IHonorarioAppService>().To<HonorarioAppService>();
            kernel.Bind<ITRFAppService>().To<TRFAppService>();
            kernel.Bind<IVaraAppService>().To<VaraAppService>();
            kernel.Bind<IPrecatorioAppService>().To<PrecatorioAppService>();
            kernel.Bind<IContatoAppService>().To<ContatoAppService>();
            kernel.Bind<INaturezaAppService>().To<NaturezaAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IConfiguracaoService>().To<ConfiguracaoService>();
            kernel.Bind<INotificacaoService>().To<NotificacaoService>();
            kernel.Bind<ITemplateService>().To<TemplateService>();
            kernel.Bind<IAssinanteService>().To<AssinanteService>();
            kernel.Bind<IClienteService>().To<ClienteService>();
            kernel.Bind<IClienteCnpjService>().To<ClienteCnpjService>();
            kernel.Bind<IMensagemService>().To<MensagemService>();
            kernel.Bind<IGrupoService>().To<GrupoService>();
            kernel.Bind<ICategoriaClienteService>().To<CategoriaClienteService>();
            kernel.Bind<ICRMService>().To<CRMService>();
            kernel.Bind<IAgendaService>().To<AgendaService>();
            kernel.Bind<ITemplateSMSService>().To<TemplateSMSService>();
            kernel.Bind<ITemplateEMailService>().To<TemplateEMailService>();
            kernel.Bind<INoticiaService>().To<NoticiaService>();
            kernel.Bind<ITarefaService>().To<TarefaService>();
            kernel.Bind<ITipoPessoaService>().To<TipoPessoaService>();
            kernel.Bind<ICategoriaTelefoneService>().To<CategoriaTelefoneService>();
            kernel.Bind<ICargoService>().To<CargoService>();
            kernel.Bind<ICRMOrigemService>().To<CRMOrigemService>();
            kernel.Bind<IMotivoCancelamentoService>().To<MotivoCancelamentoService>();
            kernel.Bind<IMotivoEncerramentoService>().To<MotivoEncerramentoService>();
            kernel.Bind<ITipoAcaoService>().To<TipoAcaoService>();
            kernel.Bind<IPeriodicidadeService>().To<PeriodicidadeService>();
            kernel.Bind<IMensagemAutomacaoService>().To<MensagemAutomacaoService>();
            kernel.Bind<IEMailAgendaService>().To<EmailAgendaService>();
            kernel.Bind<IVideoService>().To<VideoService>();
            kernel.Bind<ITemplatePropostaService>().To<TemplatePropostaService>();
            kernel.Bind<ICRMDiarioService>().To<CRMDiarioService>();
            kernel.Bind<IFunilService>().To<FunilService>();
            kernel.Bind<IAssinanteCnpjService>().To<AssinanteCnpjService>();
            kernel.Bind<IPlanoService>().To<PlanoService>();
            kernel.Bind<ICategoriaAgendaService>().To<CategoriaAgendaService>();
            kernel.Bind<ITipoTarefaService>().To<TipoTarefaService>();
            kernel.Bind<IMensagemEnviadaSistemaService>().To<MensagemEnviadaSistemaService>();
            kernel.Bind<IRecursividadeService>().To<RecursividadeService>();
            kernel.Bind<ITipoFollowService>().To<TipoFollowService>();
            kernel.Bind<IBancoService>().To<BancoService>();
            kernel.Bind<IEscolaridadeService>().To<EscolaridadeService>();
            kernel.Bind<IParentescoService>().To<ParentescoService>();
            kernel.Bind<IBeneficiarioService>().To<BeneficiarioService>();
            kernel.Bind<ITabelaIPCAService>().To<TabelaIPCAService>();
            kernel.Bind<ITabelaIRRFService>().To<TabelaIRRFService>();
            kernel.Bind<IHonorarioService>().To<HonorarioService>();
            kernel.Bind<ITRFService>().To<TRFService>();
            kernel.Bind<IVaraService>().To<VaraService>();
            kernel.Bind<IPrecatorioService>().To<PrecatorioService>();
            kernel.Bind<IContatoService>().To<ContatoService>();
            kernel.Bind<INaturezaService>().To<NaturezaService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IConfiguracaoRepository>().To<ConfiguracaoRepository>();
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IPerfilRepository>().To<PerfilRepository>();
            kernel.Bind<ITemplateRepository>().To<TemplateRepository>();
            kernel.Bind<ITipoPessoaRepository>().To<TipoPessoaRepository>();
            kernel.Bind<ICategoriaNotificacaoRepository>().To<CategoriaNotificacaoRepository>();
            kernel.Bind<INotificacaoRepository>().To<NotificacaoRepository>();
            kernel.Bind<INotificacaoAnexoRepository>().To<NotificacaoAnexoRepository>();
            kernel.Bind<IUsuarioAnexoRepository>().To<UsuarioAnexoRepository>();
            kernel.Bind<IUFRepository>().To<UFRepository>();
            kernel.Bind<IAssinanteRepository>().To<AssinanteRepository>();
            kernel.Bind<IAssinanteAnexoRepository>().To<AssinanteAnexoRepository>();
            kernel.Bind<ICategoriaClienteRepository>().To<CategoriaClienteRepository>();
            kernel.Bind<IClienteRepository>().To<ClienteRepository>();
            kernel.Bind<IClienteAnexoRepository>().To<ClienteAnexoRepository>();
            kernel.Bind<IClienteContatoRepository>().To<ClienteContatoRepository>();
            kernel.Bind<IClienteCnpjRepository>().To<ClienteCnpjRepository>();
            kernel.Bind<ICargoRepository>().To<CargoRepository>();
            kernel.Bind<IMensagemRepository>().To<MensagemRepository>();
            kernel.Bind<IMensagemDestinoRepository>().To<MensagemDestinoRepository>();
            kernel.Bind<IMensagemAnexoRepository>().To<MensagemAnexoRepository>();
            kernel.Bind<IGrupoRepository>().To<GrupoRepository>();
            kernel.Bind<IGrupoContatoRepository>().To<GrupoContatoRepository>();
            kernel.Bind<ICRMRepository>().To<CRMRepository>();
            kernel.Bind<ICRMAnexoRepository>().To<CRMAnexoRepository>();
            kernel.Bind<ICRMComentarioRepository>().To<CRMComentarioRepository>();
            kernel.Bind<ITipoCRMRepository>().To<TipoCRMRepository>();
            kernel.Bind<ITipoAcaoRepository>().To<TipoAcaoRepository>();
            kernel.Bind<IMotivoCancelamentoRepository>().To<MotivoCancelamentoRepository>();
            kernel.Bind<IMotivoEncerramentoRepository>().To<MotivoEncerramentoRepository>();
            kernel.Bind<ICRMOrigemRepository>().To<CRMOrigemRepository>();
            kernel.Bind<ICRMContatoRepository>().To<CRMContatoRepository>();
            kernel.Bind<ICRMAcaoRepository>().To<CRMAcaoRepository>();
            kernel.Bind<ICategoriaAgendaRepository>().To<CategoriaAgendaRepository>();
            kernel.Bind<IAgendaAnexoRepository>().To<AgendaAnexoRepository>();
            kernel.Bind<IAgendaRepository>().To<AgendaRepository>();
            kernel.Bind<ITipoContribuinteRepository>().To<TipoContribuinteRepository>();
            kernel.Bind<IRegimeTributarioRepository>().To<RegimeTributarioRepository>();
            kernel.Bind<ISexoRepository>().To<SexoRepository>();
            kernel.Bind<ITemplateSMSRepository>().To<TemplateSMSRepository>();
            kernel.Bind<ITemplateEMailRepository>().To<TemplateEMailRepository>();
            kernel.Bind<INoticiaRepository>().To<NoticiaRepository>();
            kernel.Bind<INoticiaComentarioRepository>().To<NoticiaComentarioRepository>();
            kernel.Bind<ITarefaRepository>().To<TarefaRepository>();
            kernel.Bind<ITarefaAnexoRepository>().To<TarefaAnexoRepository>();
            kernel.Bind<ITipoTarefaRepository>().To<TipoTarefaRepository>();
            kernel.Bind<ICategoriaTelefoneRepository>().To<CategoriaTelefoneRepository>();
            kernel.Bind<IClienteReferenciaRepository>().To<ClienteReferenciaRepository>();
            kernel.Bind<IPeriodicidadeRepository>().To<PeriodicidadeRepository>();
            kernel.Bind<IMensagemAutomacaoRepository>().To<MensagemAutomacaoRepository>();
            kernel.Bind<IMensagemAutomacaoDatasRepository>().To<MensagemAutomacaoDatasRepository>();
            kernel.Bind<ITemplatePropostaRepository>().To<TemplatePropostaRepository>();
            kernel.Bind<ICRMPedidoRepository>().To<CRMPedidoRepository>();
            kernel.Bind<ICRMPedidoAnexoRepository>().To<CRMPedidoAnexoRepository>();
            kernel.Bind<ICRMPedidoComentarioRepository>().To<CRMPedidoComentarioRepository>();
            kernel.Bind<IEmailAgendaRepository>().To<EMailAgendaRepository>();
            kernel.Bind<IUsuarioAnotacaoRepository>().To<UsuarioAnotacaoRepository>();
            kernel.Bind<IVideoRepository>().To<VideoRepository>();
            kernel.Bind<IVideoComentarioRepository>().To<VideoComentarioRepository>();
            kernel.Bind<IClienteAnotacaoRepository>().To<ClienteAnotacaoRepository>();
            kernel.Bind<ICRMDiarioRepository>().To<CRMDiarioRepository>();
            kernel.Bind<IClienteFalhaRepository>().To<ClienteFalhaRepository>();
            kernel.Bind<IFunilRepository>().To<FunilRepository>();
            kernel.Bind<IFunilEtapaRepository>().To<FunilEtapaRepository>();
            kernel.Bind<IAssinanteAnotacaoRepository>().To<AssinanteAnotacaoRepository>();
            kernel.Bind<IAssinanteCnpjRepository>().To<AssinanteCnpjRepository>();
            kernel.Bind<IAssinantePagamentoRepository>().To<AssinantePagamentoRepository>();
            kernel.Bind<IPeriodicidadePlanoRepository>().To<PeriodicidadePlanoRepository>();
            kernel.Bind<IPlanoRepository>().To<PlanoRepository>();
            kernel.Bind<IAssinantePlanoRepository>().To<AssinantePlanoRepository>();
            kernel.Bind<IMensagemEnviadaSistemaRepository>().To<MensagemEnviadaSistemaRepository>();
            kernel.Bind<IConfiguracaoChavesRepository>().To<ConfiguracaoChavesRepository>();
            kernel.Bind<IRecursividadeRepository>().To<RecursividadeRepository>();
            kernel.Bind<IRecursividadeDestinoRepository>().To<RecursividadeDestinoRepository>();
            kernel.Bind<IRecursividadeDataRepository>().To<RecursividadeDataRepository>();
            kernel.Bind<IResultadoRobotRepository>().To<ResultadoRobotRepository>();
            kernel.Bind<ILinguaRepository>().To<LinguaRepository>();
            kernel.Bind<INacionalidadeRepository>().To<NacionalidadeRepository>();
            kernel.Bind<IMunicipioRepository>().To<MunicipioRepository>();
            kernel.Bind<ILogExcecaoRepository>().To<LogExcecaoRepository>();
            kernel.Bind<ITipoFollowRepository>().To<TipoFollowRepository>();
            kernel.Bind<ICRMFollowRepository>().To<CRMFollowRepository>();
            kernel.Bind<IBancoRepository>().To<BancoRepository>();
            kernel.Bind<IEscolaridadeRepository>().To<EscolaridadeRepository>();
            kernel.Bind<IParentescoRepository>().To<ParentescoRepository>();
            kernel.Bind<IBeneficiarioRepository>().To<BeneficiarioRepository>();
            kernel.Bind<IBeneficiarioAnexoRepository>().To<BeneficiarioAnexoRepository>();
            kernel.Bind<IBeneficiarioComentarioRepository>().To<BeneficiarioComentarioRepository>();
            kernel.Bind<IEstadoCivilRepository>().To<EstadoCivilRepository>();
            kernel.Bind<ITabelaIPCARepository>().To<TabelaIPCARepository>();
            kernel.Bind<ITabelaIRRFRepository>().To<TabelaIRRFRepository>();
            kernel.Bind<IHonorarioAnexoRepository>().To<HonorarioAnexoRepository>();
            kernel.Bind<IHonorarioComentarioRepository>().To<HonorarioComentarioRepository>();
            kernel.Bind<IHonorarioRepository>().To<HonorarioRepository>();
            kernel.Bind<IContatoRepository>().To<ContatoRepository>();
            kernel.Bind<IEnderecoRepository>().To<EnderecoRepository>();
            kernel.Bind<ITipoTelefoneBaseRepository>().To<TipoTelefoneBaseRepository>();
            kernel.Bind<ITRFRepository>().To<TRFRepository>();
            kernel.Bind<IVaraRepository>().To<VaraRepository>();
            kernel.Bind<ITitularidadeRepository>().To<TitularidadeRepository>();
            kernel.Bind<IPrecatorioRepository>().To<PrecatorioRepository>();
            kernel.Bind<IPrecatorioAnexoRepository>().To<PrecatorioAnexoRepository>();
            kernel.Bind<IPrecatorioAnotacaoRepository>().To<PrecatorioAnotacaoRepository>();
            kernel.Bind<IPrecatorioEstadoRepository>().To<PrecatorioEstadoRepository>();
            kernel.Bind<INaturezaRepository>().To<NaturezaRepository>();
            kernel.Bind<IPrecatorioFalhaRepository>().To<PrecatorioFalhaRepository>();
            kernel.Bind<IContatoFalhaRepository>().To<ContatoFalhaRepository>();
            kernel.Bind<IQualificacaoRepository>().To<QualificacaoRepository>();
            kernel.Bind<IQuemDesligouRepository>().To<QuemDesligouRepository>();
        }
    }
}