﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RidolfiDB_WebEntities : DbContext
    {
        public RidolfiDB_WebEntities()
            : base("name=RidolfiDB_WebEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AGENDA> AGENDA { get; set; }
        public virtual DbSet<AGENDA_ANEXO> AGENDA_ANEXO { get; set; }
        public virtual DbSet<AGENDA_VINCULO> AGENDA_VINCULO { get; set; }
        public virtual DbSet<ASSINANTE> ASSINANTE { get; set; }
        public virtual DbSet<ASSINANTE_ANEXO> ASSINANTE_ANEXO { get; set; }
        public virtual DbSet<ASSINANTE_ANOTACAO> ASSINANTE_ANOTACAO { get; set; }
        public virtual DbSet<ASSINANTE_CONSUMO> ASSINANTE_CONSUMO { get; set; }
        public virtual DbSet<ASSINANTE_PAGAMENTO> ASSINANTE_PAGAMENTO { get; set; }
        public virtual DbSet<ASSINANTE_PLANO> ASSINANTE_PLANO { get; set; }
        public virtual DbSet<ASSINANTE_QUADRO_SOCIETARIO> ASSINANTE_QUADRO_SOCIETARIO { get; set; }
        public virtual DbSet<ATENDIMENTO> ATENDIMENTO { get; set; }
        public virtual DbSet<ATENDIMENTO_ACAO> ATENDIMENTO_ACAO { get; set; }
        public virtual DbSet<ATENDIMENTO_ACOMPANHAMENTO> ATENDIMENTO_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<ATENDIMENTO_AGENDA> ATENDIMENTO_AGENDA { get; set; }
        public virtual DbSet<ATENDIMENTO_ANEXO> ATENDIMENTO_ANEXO { get; set; }
        public virtual DbSet<ATENDIMENTO_CRM> ATENDIMENTO_CRM { get; set; }
        public virtual DbSet<ATENDIMENTO_PROPOSTA> ATENDIMENTO_PROPOSTA { get; set; }
        public virtual DbSet<ATENDIMENTO_PROPOSTA_ACOMPANHAMENTO> ATENDIMENTO_PROPOSTA_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<ATENDIMENTO_PROPOSTA_ANEXO> ATENDIMENTO_PROPOSTA_ANEXO { get; set; }
        public virtual DbSet<ATENDIMENTO_PROPOSTA_PECA> ATENDIMENTO_PROPOSTA_PECA { get; set; }
        public virtual DbSet<ATENDIMENTO_PROPOSTA_SERVICO> ATENDIMENTO_PROPOSTA_SERVICO { get; set; }
        public virtual DbSet<BANCO> BANCO { get; set; }
        public virtual DbSet<BENEFICIARIO> BENEFICIARIO { get; set; }
        public virtual DbSet<BENEFICIARIO_ANEXO> BENEFICIARIO_ANEXO { get; set; }
        public virtual DbSet<BENEFICIARIO_ANOTACOES> BENEFICIARIO_ANOTACOES { get; set; }
        public virtual DbSet<CARGO> CARGO { get; set; }
        public virtual DbSet<CARGO_TESTE> CARGO_TESTE { get; set; }
        public virtual DbSet<CATEGORIA_AGENDA> CATEGORIA_AGENDA { get; set; }
        public virtual DbSet<CATEGORIA_ATENDIMENTO> CATEGORIA_ATENDIMENTO { get; set; }
        public virtual DbSet<CATEGORIA_CLIENTE> CATEGORIA_CLIENTE { get; set; }
        public virtual DbSet<CATEGORIA_CUSTO_FIXO> CATEGORIA_CUSTO_FIXO { get; set; }
        public virtual DbSet<CATEGORIA_FORNECEDOR> CATEGORIA_FORNECEDOR { get; set; }
        public virtual DbSet<CATEGORIA_NOTIFICACAO> CATEGORIA_NOTIFICACAO { get; set; }
        public virtual DbSet<CATEGORIA_ORDEM_SERVICO> CATEGORIA_ORDEM_SERVICO { get; set; }
        public virtual DbSet<CATEGORIA_PRODUTO> CATEGORIA_PRODUTO { get; set; }
        public virtual DbSet<CATEGORIA_SERVICO> CATEGORIA_SERVICO { get; set; }
        public virtual DbSet<CATEGORIA_TELEFONE> CATEGORIA_TELEFONE { get; set; }
        public virtual DbSet<CATEGORIA_USUARIO> CATEGORIA_USUARIO { get; set; }
        public virtual DbSet<CENTRO_CUSTO> CENTRO_CUSTO { get; set; }
        public virtual DbSet<CLIENTE> CLIENTE { get; set; }
        public virtual DbSet<CLIENTE_ANEXO> CLIENTE_ANEXO { get; set; }
        public virtual DbSet<CLIENTE_ANOTACAO> CLIENTE_ANOTACAO { get; set; }
        public virtual DbSet<CLIENTE_CONTATO> CLIENTE_CONTATO { get; set; }
        public virtual DbSet<CLIENTE_FALHA> CLIENTE_FALHA { get; set; }
        public virtual DbSet<CLIENTE_QUADRO_SOCIETARIO> CLIENTE_QUADRO_SOCIETARIO { get; set; }
        public virtual DbSet<CLIENTE_REFERENCIA> CLIENTE_REFERENCIA { get; set; }
        public virtual DbSet<CLIENTE_TAG> CLIENTE_TAG { get; set; }
        public virtual DbSet<COMISSAO> COMISSAO { get; set; }
        public virtual DbSet<COMISSAO_CARGO> COMISSAO_CARGO { get; set; }
        public virtual DbSet<CONFIGURACAO> CONFIGURACAO { get; set; }
        public virtual DbSet<CONFIGURACAO_CHAVES> CONFIGURACAO_CHAVES { get; set; }
        public virtual DbSet<CONFIGURACAO_CRIPT> CONFIGURACAO_CRIPT { get; set; }
        public virtual DbSet<CONTA_BANCO> CONTA_BANCO { get; set; }
        public virtual DbSet<CONTA_BANCO_CONTATO> CONTA_BANCO_CONTATO { get; set; }
        public virtual DbSet<CONTA_BANCO_LANCAMENTO> CONTA_BANCO_LANCAMENTO { get; set; }
        public virtual DbSet<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        public virtual DbSet<CONTA_PAGAR_ANEXO> CONTA_PAGAR_ANEXO { get; set; }
        public virtual DbSet<CONTA_PAGAR_PARCELA> CONTA_PAGAR_PARCELA { get; set; }
        public virtual DbSet<CONTA_PAGAR_RATEIO> CONTA_PAGAR_RATEIO { get; set; }
        public virtual DbSet<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        public virtual DbSet<CONTA_RECEBER_ANEXO> CONTA_RECEBER_ANEXO { get; set; }
        public virtual DbSet<CONTA_RECEBER_PARCELA> CONTA_RECEBER_PARCELA { get; set; }
        public virtual DbSet<CONTA_RECEBER_RATEIO> CONTA_RECEBER_RATEIO { get; set; }
        public virtual DbSet<CONTATO> CONTATO { get; set; }
        public virtual DbSet<CONTATO_FALHA> CONTATO_FALHA { get; set; }
        public virtual DbSet<CONVENIO> CONVENIO { get; set; }
        public virtual DbSet<COR> COR { get; set; }
        public virtual DbSet<CRM> CRM { get; set; }
        public virtual DbSet<CRM_ACAO> CRM_ACAO { get; set; }
        public virtual DbSet<CRM_ANEXO> CRM_ANEXO { get; set; }
        public virtual DbSet<CRM_COMENTARIO> CRM_COMENTARIO { get; set; }
        public virtual DbSet<CRM_CONTATO> CRM_CONTATO { get; set; }
        public virtual DbSet<CRM_FOLLOW> CRM_FOLLOW { get; set; }
        public virtual DbSet<CRM_ORIGEM> CRM_ORIGEM { get; set; }
        public virtual DbSet<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        public virtual DbSet<CRM_PEDIDO_VENDA_ACOMPANHAMENTO> CRM_PEDIDO_VENDA_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<CRM_PEDIDO_VENDA_ANEXO> CRM_PEDIDO_VENDA_ANEXO { get; set; }
        public virtual DbSet<CUSTO_FIXO> CUSTO_FIXO { get; set; }
        public virtual DbSet<CUSTO_FIXO_CALENDARIO> CUSTO_FIXO_CALENDARIO { get; set; }
        public virtual DbSet<CUSTO_HISTORICO> CUSTO_HISTORICO { get; set; }
        public virtual DbSet<CUSTO_VARIAVEL> CUSTO_VARIAVEL { get; set; }
        public virtual DbSet<CUSTO_VARIAVEL_ANTECIPACAO> CUSTO_VARIAVEL_ANTECIPACAO { get; set; }
        public virtual DbSet<CUSTO_VARIAVEL_CALCULO> CUSTO_VARIAVEL_CALCULO { get; set; }
        public virtual DbSet<CUSTO_VARIAVEL_HISTORICO> CUSTO_VARIAVEL_HISTORICO { get; set; }
        public virtual DbSet<DEPARTAMENTO> DEPARTAMENTO { get; set; }
        public virtual DbSet<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }
        public virtual DbSet<EMAIL_AGENDAMENTO> EMAIL_AGENDAMENTO { get; set; }
        public virtual DbSet<EMPRESA> EMPRESA { get; set; }
        public virtual DbSet<EMPRESA_ANEXO> EMPRESA_ANEXO { get; set; }
        public virtual DbSet<EMPRESA_CUSTO_VARIAVEL> EMPRESA_CUSTO_VARIAVEL { get; set; }
        public virtual DbSet<EMPRESA_CUSTO_VARIAVEL_NORMAL> EMPRESA_CUSTO_VARIAVEL_NORMAL { get; set; }
        public virtual DbSet<EMPRESA_MAQUINA> EMPRESA_MAQUINA { get; set; }
        public virtual DbSet<EMPRESA_PLATAFORMA> EMPRESA_PLATAFORMA { get; set; }
        public virtual DbSet<EMPRESA_TICKET> EMPRESA_TICKET { get; set; }
        public virtual DbSet<ENDERECO> ENDERECO { get; set; }
        public virtual DbSet<ESCOLARIDADE> ESCOLARIDADE { get; set; }
        public virtual DbSet<ESTADO_CIVIL> ESTADO_CIVIL { get; set; }
        public virtual DbSet<FICHA_TECNICA> FICHA_TECNICA { get; set; }
        public virtual DbSet<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }
        public virtual DbSet<FILIAL> FILIAL { get; set; }
        public virtual DbSet<FORMA_ENVIO> FORMA_ENVIO { get; set; }
        public virtual DbSet<FORMA_FRETE> FORMA_FRETE { get; set; }
        public virtual DbSet<FORMA_PAGAMENTO> FORMA_PAGAMENTO { get; set; }
        public virtual DbSet<FORNECEDOR> FORNECEDOR { get; set; }
        public virtual DbSet<FORNECEDOR_ANEXO> FORNECEDOR_ANEXO { get; set; }
        public virtual DbSet<FORNECEDOR_ANOTACOES> FORNECEDOR_ANOTACOES { get; set; }
        public virtual DbSet<FORNECEDOR_CONTATO> FORNECEDOR_CONTATO { get; set; }
        public virtual DbSet<FORNECEDOR_QUADRO_SOCIETARIO> FORNECEDOR_QUADRO_SOCIETARIO { get; set; }
        public virtual DbSet<FUNIL> FUNIL { get; set; }
        public virtual DbSet<FUNIL_ETAPA> FUNIL_ETAPA { get; set; }
        public virtual DbSet<GRAU_INSTRUCAO> GRAU_INSTRUCAO { get; set; }
        public virtual DbSet<GRUPO> GRUPO { get; set; }
        public virtual DbSet<GRUPO_CC> GRUPO_CC { get; set; }
        public virtual DbSet<GRUPO_CLIENTE> GRUPO_CLIENTE { get; set; }
        public virtual DbSet<HONORARIO> HONORARIO { get; set; }
        public virtual DbSet<HONORARIO_ANEXO> HONORARIO_ANEXO { get; set; }
        public virtual DbSet<HONORARIO_ANOTACOES> HONORARIO_ANOTACOES { get; set; }
        public virtual DbSet<LINGUA> LINGUA { get; set; }
        public virtual DbSet<LOAS> LOAS { get; set; }
        public virtual DbSet<LOG> LOG { get; set; }
        public virtual DbSet<LOG_EXCECAO> LOG_EXCECAO { get; set; }
        public virtual DbSet<LOG_EXCECAO_NOVO> LOG_EXCECAO_NOVO { get; set; }
        public virtual DbSet<MAQUINA> MAQUINA { get; set; }
        public virtual DbSet<MENSAGEM_ANEXO> MENSAGEM_ANEXO { get; set; }
        public virtual DbSet<MENSAGEM_AUTOMACAO> MENSAGEM_AUTOMACAO { get; set; }
        public virtual DbSet<MENSAGEM_AUTOMACAO_DATAS> MENSAGEM_AUTOMACAO_DATAS { get; set; }
        public virtual DbSet<MENSAGENS> MENSAGENS { get; set; }
        public virtual DbSet<MENSAGENS_DESTINOS> MENSAGENS_DESTINOS { get; set; }
        public virtual DbSet<MENSAGENS_ENVIADAS_SISTEMA> MENSAGENS_ENVIADAS_SISTEMA { get; set; }
        public virtual DbSet<META> META { get; set; }
        public virtual DbSet<META_ANEXO> META_ANEXO { get; set; }
        public virtual DbSet<META_ANOTACAO> META_ANOTACAO { get; set; }
        public virtual DbSet<MOTIVO_CANCELAMENTO> MOTIVO_CANCELAMENTO { get; set; }
        public virtual DbSet<MOTIVO_ENCERRAMENTO> MOTIVO_ENCERRAMENTO { get; set; }
        public virtual DbSet<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        public virtual DbSet<MUNICIPIO> MUNICIPIO { get; set; }
        public virtual DbSet<NACIONALIDADE> NACIONALIDADE { get; set; }
        public virtual DbSet<NATUREZA> NATUREZA { get; set; }
        public virtual DbSet<NOMENCLATURA_BRAS_SERVICOS> NOMENCLATURA_BRAS_SERVICOS { get; set; }
        public virtual DbSet<NOTICIA> NOTICIA { get; set; }
        public virtual DbSet<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
        public virtual DbSet<NOTIFICACAO> NOTIFICACAO { get; set; }
        public virtual DbSet<NOTIFICACAO_ANEXO> NOTIFICACAO_ANEXO { get; set; }
        public virtual DbSet<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        public virtual DbSet<ORDEM_SERVICO_ACOMPANHAMENTO> ORDEM_SERVICO_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<ORDEM_SERVICO_AGENDA> ORDEM_SERVICO_AGENDA { get; set; }
        public virtual DbSet<ORDEM_SERVICO_ANEXO> ORDEM_SERVICO_ANEXO { get; set; }
        public virtual DbSet<ORDEM_SERVICO_COMENTARIOS> ORDEM_SERVICO_COMENTARIOS { get; set; }
        public virtual DbSet<ORDEM_SERVICO_PRODUTO> ORDEM_SERVICO_PRODUTO { get; set; }
        public virtual DbSet<ORDEM_SERVICO_SERVICO> ORDEM_SERVICO_SERVICO { get; set; }
        public virtual DbSet<OUTRO_CUSTO_VARIAVEL> OUTRO_CUSTO_VARIAVEL { get; set; }
        public virtual DbSet<PACIENTE> PACIENTE { get; set; }
        public virtual DbSet<PACIENTE_ANAMNESE> PACIENTE_ANAMNESE { get; set; }
        public virtual DbSet<PACIENTE_ANEXO> PACIENTE_ANEXO { get; set; }
        public virtual DbSet<PACIENTE_ANOTACAO> PACIENTE_ANOTACAO { get; set; }
        public virtual DbSet<PACIENTE_CONSULTA> PACIENTE_CONSULTA { get; set; }
        public virtual DbSet<PACIENTE_PRESCRICAO> PACIENTE_PRESCRICAO { get; set; }
        public virtual DbSet<PARENTESCO> PARENTESCO { get; set; }
        public virtual DbSet<PECA> PECA { get; set; }
        public virtual DbSet<PECA_ANEXO> PECA_ANEXO { get; set; }
        public virtual DbSet<PECA_ANOTACAO> PECA_ANOTACAO { get; set; }
        public virtual DbSet<PEDIDO_VENDA> PEDIDO_VENDA { get; set; }
        public virtual DbSet<PEDIDO_VENDA_ACOMPANHAMENTO> PEDIDO_VENDA_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<PEDIDO_VENDA_ANEXO> PEDIDO_VENDA_ANEXO { get; set; }
        public virtual DbSet<PERFIL> PERFIL { get; set; }
        public virtual DbSet<PERIODICIDADE> PERIODICIDADE { get; set; }
        public virtual DbSet<PERIODICIDADE_TAREFA> PERIODICIDADE_TAREFA { get; set; }
        public virtual DbSet<PESQUISA> PESQUISA { get; set; }
        public virtual DbSet<PESQUISA_ANEXO> PESQUISA_ANEXO { get; set; }
        public virtual DbSet<PESQUISA_ANOTACAO> PESQUISA_ANOTACAO { get; set; }
        public virtual DbSet<PESQUISA_ENVIO> PESQUISA_ENVIO { get; set; }
        public virtual DbSet<PESQUISA_ITEM> PESQUISA_ITEM { get; set; }
        public virtual DbSet<PESQUISA_ITEM_OPCAO> PESQUISA_ITEM_OPCAO { get; set; }
        public virtual DbSet<PESQUISA_RESPOSTA> PESQUISA_RESPOSTA { get; set; }
        public virtual DbSet<PESQUISA_RESPOSTA_ITEM> PESQUISA_RESPOSTA_ITEM { get; set; }
        public virtual DbSet<PLANO> PLANO { get; set; }
        public virtual DbSet<PLANO_PERIODICIDADE> PLANO_PERIODICIDADE { get; set; }
        public virtual DbSet<PLATAFORMA_ENTREGA> PLATAFORMA_ENTREGA { get; set; }
        public virtual DbSet<PRECATORIO> PRECATORIO { get; set; }
        public virtual DbSet<PRECATORIO_ANEXO> PRECATORIO_ANEXO { get; set; }
        public virtual DbSet<PRECATORIO_ANOTACAO> PRECATORIO_ANOTACAO { get; set; }
        public virtual DbSet<PRECATORIO_ESTADO> PRECATORIO_ESTADO { get; set; }
        public virtual DbSet<PRECATORIO_FALHA> PRECATORIO_FALHA { get; set; }
        public virtual DbSet<PRECIFICACAO> PRECIFICACAO { get; set; }
        public virtual DbSet<PRODUTO> PRODUTO { get; set; }
        public virtual DbSet<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        public virtual DbSet<PRODUTO_ANOTACAO> PRODUTO_ANOTACAO { get; set; }
        public virtual DbSet<PRODUTO_CONCORRENTE> PRODUTO_CONCORRENTE { get; set; }
        public virtual DbSet<PRODUTO_CUSTO> PRODUTO_CUSTO { get; set; }
        public virtual DbSet<PRODUTO_EMPRESA> PRODUTO_EMPRESA { get; set; }
        public virtual DbSet<PRODUTO_ESTOQUE_FILIAL> PRODUTO_ESTOQUE_FILIAL { get; set; }
        public virtual DbSet<PRODUTO_FALHA> PRODUTO_FALHA { get; set; }
        public virtual DbSet<PRODUTO_KIT> PRODUTO_KIT { get; set; }
        public virtual DbSet<PRODUTO_ORIGEM> PRODUTO_ORIGEM { get; set; }
        public virtual DbSet<PRODUTO_PRECO_VENDA> PRODUTO_PRECO_VENDA { get; set; }
        public virtual DbSet<QUALIFICACAO> QUALIFICACAO { get; set; }
        public virtual DbSet<QUEM_DESLIGOU> QUEM_DESLIGOU { get; set; }
        public virtual DbSet<RECURSIVIDADE> RECURSIVIDADE { get; set; }
        public virtual DbSet<RECURSIVIDADE_DATA> RECURSIVIDADE_DATA { get; set; }
        public virtual DbSet<RECURSIVIDADE_DESTINO> RECURSIVIDADE_DESTINO { get; set; }
        public virtual DbSet<REGIME_TRIBUTARIO> REGIME_TRIBUTARIO { get; set; }
        public virtual DbSet<RESULTADO_ROBOT> RESULTADO_ROBOT { get; set; }
        public virtual DbSet<SERVICO> SERVICO { get; set; }
        public virtual DbSet<SERVICO_ANEXO> SERVICO_ANEXO { get; set; }
        public virtual DbSet<SERVICO_EMPRESA> SERVICO_EMPRESA { get; set; }
        public virtual DbSet<SERVICO_TABELA_PRECO> SERVICO_TABELA_PRECO { get; set; }
        public virtual DbSet<SEXO> SEXO { get; set; }
        public virtual DbSet<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }
        public virtual DbSet<SUBGRUPO> SUBGRUPO { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TABELA_IMPORTACAO> TABELA_IMPORTACAO { get; set; }
        public virtual DbSet<TABELA_IPCA> TABELA_IPCA { get; set; }
        public virtual DbSet<TABELA_IRRF> TABELA_IRRF { get; set; }
        public virtual DbSet<TAREFA> TAREFA { get; set; }
        public virtual DbSet<TAREFA_ACOMPANHAMENTO> TAREFA_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<TAREFA_ANEXO> TAREFA_ANEXO { get; set; }
        public virtual DbSet<TAREFA_NOTIFICACAO> TAREFA_NOTIFICACAO { get; set; }
        public virtual DbSet<TAREFA_VINCULO> TAREFA_VINCULO { get; set; }
        public virtual DbSet<TELEFONE> TELEFONE { get; set; }
        public virtual DbSet<TEMPLATE> TEMPLATE { get; set; }
        public virtual DbSet<TEMPLATE_EMAIL> TEMPLATE_EMAIL { get; set; }
        public virtual DbSet<TEMPLATE_PROPOSTA> TEMPLATE_PROPOSTA { get; set; }
        public virtual DbSet<TEMPLATE_SMS> TEMPLATE_SMS { get; set; }
        public virtual DbSet<TICKET_ALIMENTACAO> TICKET_ALIMENTACAO { get; set; }
        public virtual DbSet<TIPO_ACAO> TIPO_ACAO { get; set; }
        public virtual DbSet<TIPO_CONTA> TIPO_CONTA { get; set; }
        public virtual DbSet<TIPO_CONTRIBUINTE> TIPO_CONTRIBUINTE { get; set; }
        public virtual DbSet<TIPO_CRM> TIPO_CRM { get; set; }
        public virtual DbSet<TIPO_EMBALAGEM> TIPO_EMBALAGEM { get; set; }
        public virtual DbSet<TIPO_FOLLOW> TIPO_FOLLOW { get; set; }
        public virtual DbSet<TIPO_GRUPO> TIPO_GRUPO { get; set; }
        public virtual DbSet<TIPO_ITEM_PESQUISA> TIPO_ITEM_PESQUISA { get; set; }
        public virtual DbSet<TIPO_PACIENTE> TIPO_PACIENTE { get; set; }
        public virtual DbSet<TIPO_PESQUISA> TIPO_PESQUISA { get; set; }
        public virtual DbSet<TIPO_PESSOA> TIPO_PESSOA { get; set; }
        public virtual DbSet<TIPO_TAREFA> TIPO_TAREFA { get; set; }
        public virtual DbSet<TIPO_TELEFONE> TIPO_TELEFONE { get; set; }
        public virtual DbSet<TIPO_TELEFONE_BASE> TIPO_TELEFONE_BASE { get; set; }
        public virtual DbSet<TITULARIDADE> TITULARIDADE { get; set; }
        public virtual DbSet<TRF> TRF { get; set; }
        public virtual DbSet<UF> UF { get; set; }
        public virtual DbSet<UNIDADE> UNIDADE { get; set; }
        public virtual DbSet<USUARIO> USUARIO { get; set; }
        public virtual DbSet<USUARIO_ANEXO> USUARIO_ANEXO { get; set; }
        public virtual DbSet<USUARIO_ANOTACAO> USUARIO_ANOTACAO { get; set; }
        public virtual DbSet<VARA> VARA { get; set; }
        public virtual DbSet<VENDA_MENSAL> VENDA_MENSAL { get; set; }
        public virtual DbSet<VENDA_MENSAL_ANEXO> VENDA_MENSAL_ANEXO { get; set; }
        public virtual DbSet<VENDA_MENSAL_ANOTACAO> VENDA_MENSAL_ANOTACAO { get; set; }
        public virtual DbSet<VENDA_MENSAL_CALCULO> VENDA_MENSAL_CALCULO { get; set; }
        public virtual DbSet<VENDA_MENSAL_COMISSAO> VENDA_MENSAL_COMISSAO { get; set; }
        public virtual DbSet<VENDA_MENSAL_PARTES> VENDA_MENSAL_PARTES { get; set; }
        public virtual DbSet<VIDEO> VIDEO { get; set; }
        public virtual DbSet<VIDEO_COMENTARIO> VIDEO_COMENTARIO { get; set; }
        public virtual DbSet<VOLTA_PESQUISA> VOLTA_PESQUISA { get; set; }
    }
}
