using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<USUARIO, UsuarioViewModel>();
            CreateMap<USUARIO, UsuarioLoginViewModel>();
            CreateMap<LOG, LogViewModel>();
            CreateMap<CONFIGURACAO, ConfiguracaoViewModel>();
            CreateMap<NOTIFICACAO, NotificacaoViewModel>();
            CreateMap<MENSAGENS, MensagemViewModel>();
            CreateMap<GRUPO, GrupoViewModel>();
            CreateMap<GRUPO_CLIENTE, GrupoContatoViewModel>();
            CreateMap<TEMPLATE, TemplateViewModel>();
            CreateMap<CRM, CRMViewModel>();
            CreateMap<CRM_CONTATO, CRMContatoViewModel>();
            CreateMap<CRM_COMENTARIO, CRMComentarioViewModel>();
            CreateMap<CRM_ACAO, CRMAcaoViewModel>();
            CreateMap<AGENDA, AgendaViewModel>();
            CreateMap<PLANO, PlanoViewModel>();
            CreateMap<ASSINANTE, AssinanteViewModel>();
            CreateMap<ASSINANTE_PAGAMENTO, AssinantePagamentoViewModel>();
            CreateMap<ASSINANTE_ANOTACAO, AssinanteAnotacaoViewModel>();
            CreateMap<NOTICIA, NoticiaViewModel>();
            CreateMap<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>();
            CreateMap<TAREFA, TarefaViewModel>();
            CreateMap<TAREFA_ACOMPANHAMENTO, TarefaAcompanhamentoViewModel>();
            CreateMap<CLIENTE, ClienteViewModel>();
            CreateMap<CLIENTE_CONTATO, ClienteContatoViewModel>();
            CreateMap<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>();
            CreateMap<TEMPLATE_SMS, TemplateSMSViewModel>();
            CreateMap<TEMPLATE_EMAIL, TemplateEMailViewModel>();
            CreateMap<CATEGORIA_CLIENTE, CategoriaClienteViewModel>();
            CreateMap<CARGO, CargoViewModel>();
            CreateMap<CRM_ORIGEM, CRMOrigemViewModel>();
            CreateMap<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>();
            CreateMap<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>();
            CreateMap<TIPO_ACAO, TipoAcaoViewModel>();
            CreateMap<MENSAGEM_AUTOMACAO, MensagemAutomacaoViewModel>();
            CreateMap<MENSAGEM_AUTOMACAO_DATAS, MensagemAutomacaoDatasViewModel>();
            CreateMap<CRM_PEDIDO_VENDA, CRMPedidoViewModel>();
            CreateMap<CRM_PEDIDO_VENDA_ACOMPANHAMENTO, CRMPedidoComentarioViewModel>();
            CreateMap<TEMPLATE_PROPOSTA, TemplatePropostaViewModel>();
            CreateMap<FUNIL, FunilViewModel>();
            CreateMap<FUNIL_ETAPA, FunilEtapaViewModel>();
            CreateMap<ASSINANTE_PLANO, AssinantePlanoViewModel>();
            CreateMap<TIPO_TAREFA, TipoTarefaViewModel>();
            CreateMap<MENSAGENS_ENVIADAS_SISTEMA, MensagemEmitidaViewModel>();
            CreateMap<RECURSIVIDADE, RecursividadeViewModel>();
            CreateMap<TIPO_FOLLOW, TipoFollowViewModel>();
            CreateMap<CRM_FOLLOW, CRMFollowViewModel>();
            CreateMap<BANCO, BancoViewModel>();
            CreateMap<ESCOLARIDADE, EscolaridadeViewModel>();
            CreateMap<PARENTESCO, ParentescoViewModel>();
            CreateMap<BENEFICIARIO, BeneficiarioViewModel>();
            CreateMap<BENEFICIARIO_ANOTACOES, BeneficiarioComentarioViewModel>();
            CreateMap<TABELA_IPCA, TabelaIPCAViewModel>();
            CreateMap<TABELA_IRRF, TabelaIRRFViewModel>();
            CreateMap<HONORARIO_ANOTACOES, HonorarioComentarioViewModel>();
            CreateMap<HONORARIO, HonorarioViewModel>();
            CreateMap<CONTATO, ContatoViewModel>();
            CreateMap<ENDERECO, EnderecoViewModel>();
            CreateMap<TRF, TRFViewModel>();
            CreateMap<VARA, VaraViewModel>();
            CreateMap<PRECATORIO, PrecatorioViewModel>();
            CreateMap<PRECATORIO_ANOTACAO, PrecatorioComentarioViewModel>();
            CreateMap<AGENDA_CONTATO, AgendaContatoViewModel>();

        }
    }
}
