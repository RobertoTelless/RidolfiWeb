using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, USUARIO>();
            CreateMap<UsuarioLoginViewModel, USUARIO>();
            CreateMap<LogViewModel, LOG>();
            CreateMap<ConfiguracaoViewModel, CONFIGURACAO>();
            CreateMap<NotificacaoViewModel, NOTIFICACAO>();
            CreateMap<MensagemViewModel, MENSAGENS>();
            CreateMap<GrupoViewModel, GRUPO>();
            CreateMap<GrupoContatoViewModel, GRUPO_CLIENTE>();
            CreateMap<TemplateViewModel, TEMPLATE>();
            CreateMap<CRMViewModel, CRM>();
            CreateMap<CRMContatoViewModel, CRM_CONTATO>();
            CreateMap<CRMComentarioViewModel, CRM_COMENTARIO>();
            CreateMap<CRMAcaoViewModel, CRM_ACAO>();
            CreateMap<AgendaViewModel, AGENDA>();
            CreateMap<AssinanteViewModel, ASSINANTE>();
            CreateMap<NoticiaViewModel, NOTICIA>();
            CreateMap<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>();
            CreateMap<TarefaViewModel, TAREFA>();
            CreateMap<TarefaAcompanhamentoViewModel, TAREFA_ACOMPANHAMENTO>();
            CreateMap<ClienteViewModel, CLIENTE>();
            CreateMap<ClienteContatoViewModel, CLIENTE_CONTATO>();
            CreateMap<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>();
            CreateMap<TemplateSMSViewModel, TEMPLATE_SMS>();
            CreateMap<TemplateEMailViewModel, TEMPLATE_EMAIL>();
            CreateMap<CategoriaClienteViewModel, CATEGORIA_CLIENTE>();
            CreateMap<CargoViewModel, CARGO>();
            CreateMap<CRMOrigemViewModel, CRM_ORIGEM>();
            CreateMap<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>();
            CreateMap<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>();
            CreateMap<TipoAcaoViewModel, TIPO_ACAO>();
            CreateMap<MensagemAutomacaoViewModel, MENSAGEM_AUTOMACAO>();
            CreateMap<MensagemAutomacaoDatasViewModel, MENSAGEM_AUTOMACAO_DATAS>();
            CreateMap<CRMPedidoViewModel, CRM_PEDIDO_VENDA>();
            CreateMap<CRMPedidoComentarioViewModel, CRM_PEDIDO_VENDA_ACOMPANHAMENTO>();
            CreateMap<TemplatePropostaViewModel, TEMPLATE_PROPOSTA>();
            CreateMap<FunilViewModel, FUNIL>();
            CreateMap<FunilEtapaViewModel, FUNIL_ETAPA>();
            CreateMap<AssinanteAnotacaoViewModel, ASSINANTE_ANOTACAO>();
            CreateMap<AssinantePagamentoViewModel, ASSINANTE_PAGAMENTO>();
            CreateMap<PlanoViewModel, PLANO>();
            CreateMap<AssinantePlanoViewModel, ASSINANTE_PLANO>();
            CreateMap<TipoTarefaViewModel, TIPO_TAREFA>();
            CreateMap<MensagemEmitidaViewModel, MENSAGENS_ENVIADAS_SISTEMA>();
            CreateMap<RecursividadeViewModel, RECURSIVIDADE>();
            CreateMap<TipoFollowViewModel, TIPO_FOLLOW>();
            CreateMap<CRMFollowViewModel, CRM_FOLLOW>();
            CreateMap<BancoViewModel, BANCO>();
            CreateMap<EscolaridadeViewModel, ESCOLARIDADE>();
            CreateMap<ParentescoViewModel, PARENTESCO>();
            CreateMap<BeneficiarioComentarioViewModel, BENEFICIARIO_ANOTACOES>();
            CreateMap<BeneficiarioViewModel, BENEFICIARIO>();
            CreateMap<TabelaIPCAViewModel, TABELA_IPCA>();
            CreateMap<TabelaIRRFViewModel, TABELA_IRRF>();
            CreateMap<HonorarioComentarioViewModel, HONORARIO_ANOTACOES>();
            CreateMap<HonorarioViewModel, HONORARIO>();
            CreateMap<ContatoViewModel, CONTATO>();
            CreateMap<EnderecoViewModel, ENDERECO>();
            CreateMap<TRFViewModel, TRF>();
            CreateMap<VaraViewModel, VARA>();
            CreateMap<PrecatorioComentarioViewModel, PRECATORIO_ANOTACAO>();
            CreateMap<PrecatorioViewModel, PRECATORIO>();
            CreateMap<AgendaContatoViewModel, AGENDA_CONTATO>();


        }
    }
}