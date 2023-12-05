//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class EMPRESA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPRESA()
        {
            this.ATENDIMENTO = new HashSet<ATENDIMENTO>();
            this.ATENDIMENTO_PROPOSTA = new HashSet<ATENDIMENTO_PROPOSTA>();
            this.CLIENTE = new HashSet<CLIENTE>();
            this.CRM = new HashSet<CRM>();
            this.CRM_ACAO = new HashSet<CRM_ACAO>();
            this.CRM_FOLLOW = new HashSet<CRM_FOLLOW>();
            this.CRM_PEDIDO_VENDA = new HashSet<CRM_PEDIDO_VENDA>();
            this.CUSTO_FIXO = new HashSet<CUSTO_FIXO>();
            this.EMPRESA_ANEXO = new HashSet<EMPRESA_ANEXO>();
            this.EMPRESA_CUSTO_VARIAVEL = new HashSet<EMPRESA_CUSTO_VARIAVEL>();
            this.EMPRESA_CUSTO_VARIAVEL_NORMAL = new HashSet<EMPRESA_CUSTO_VARIAVEL_NORMAL>();
            this.EMPRESA_MAQUINA = new HashSet<EMPRESA_MAQUINA>();
            this.EMPRESA_PLATAFORMA = new HashSet<EMPRESA_PLATAFORMA>();
            this.EMPRESA_TICKET = new HashSet<EMPRESA_TICKET>();
            this.GRUPO = new HashSet<GRUPO>();
            this.MENSAGENS = new HashSet<MENSAGENS>();
            this.META = new HashSet<META>();
            this.ORDEM_SERVICO = new HashSet<ORDEM_SERVICO>();
            this.PEDIDO_VENDA = new HashSet<PEDIDO_VENDA>();
            this.TEMPLATE_EMAIL = new HashSet<TEMPLATE_EMAIL>();
            this.TEMPLATE_PROPOSTA = new HashSet<TEMPLATE_PROPOSTA>();
            this.TEMPLATE_SMS = new HashSet<TEMPLATE_SMS>();
            this.USUARIO = new HashSet<USUARIO>();
            this.VENDA_MENSAL = new HashSet<VENDA_MENSAL>();
        }
    
        public int EMPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> TIPE_CD_ID { get; set; }
        public int RETR_CD_ID { get; set; }
        public Nullable<int> MAQN_CD_ID { get; set; }
        public Nullable<int> EMPR_IN_MATRIZ { get; set; }
        public string EMPR_NM_NOME { get; set; }
        public Nullable<decimal> EMPR_VL_PATRIMONIO_LIQUIDO { get; set; }
        public Nullable<decimal> EMPR_PC_VENDA_DEBITO { get; set; }
        public Nullable<decimal> EMPR_PC_VENDA_CREDITO { get; set; }
        public Nullable<decimal> EMPR_PC_VENDA_DINHEIRO { get; set; }
        public int EMPR_IN_OPERA_CARTAO { get; set; }
        public string EMPR_NM_OUTRA_MAQUINA { get; set; }
        public Nullable<decimal> EMPR_PC_ANTECIPACAO { get; set; }
        public int EMPR_IN_PAGA_COMISSAO { get; set; }
        public Nullable<decimal> EMPR_VL_IMPOSTO_MEI { get; set; }
        public Nullable<decimal> EMPR_VL_IMPOSTO_OUTROS { get; set; }
        public string EMPR_NM_RAZAO { get; set; }
        public string EMPR_NR_CNPJ { get; set; }
        public string EMPR_NR_CPF { get; set; }
        public string EMPR_NR_INSCRICAO_MUNICIPAL { get; set; }
        public string EMPR_NR_INSCRICAO_ESTADUAL { get; set; }
        public string EMPR_NM_ENDERECO { get; set; }
        public string EMPR_NM_NUMERO { get; set; }
        public string EMPR_NM_COMPLEMENTO { get; set; }
        public string EMPR_NM_BAIRRO { get; set; }
        public string EMPR_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string EMPR_NR_CEP { get; set; }
        public System.DateTime EMPR_DT_CADASTRO { get; set; }
        public Nullable<decimal> EMPR_VL_TAXA_MEDIA { get; set; }
        public Nullable<decimal> EMPR_VL_COMISSAO_VENDEDOR { get; set; }
        public Nullable<decimal> EMPR_VL_COMISSAO_OUTROS { get; set; }
        public Nullable<decimal> EMPR_VL_COMISSAO_GERENTE { get; set; }
        public Nullable<decimal> EMPR_VL_TAXA_MEDIA_DEBITO { get; set; }
        public Nullable<decimal> EMPR_PC_CUSTO_VARIAVEL_VENDA { get; set; }
        public Nullable<decimal> EMPR_PC_CUSTO_VARIAVEL_TOTAL { get; set; }
        public Nullable<decimal> EMPR_PC_CUSTO_ANTECIPACOES { get; set; }
        public Nullable<decimal> EMPR_VL_ROYALTIES { get; set; }
        public Nullable<decimal> EMPR_VL_FUNDO_PROPAGANDA { get; set; }
        public Nullable<decimal> EMPR_VL_FUNDO_SEGURANCA { get; set; }
        public int EMPR_IN_ATIVO { get; set; }
        public Nullable<int> EMPR_IN_CALCULADO { get; set; }
        public string EMPR_AQ_LOGO { get; set; }
        public string EMPR_NM_GERENTE { get; set; }
        public string EMPR_NR_TELEFONE { get; set; }
        public string EMPR_NR_CELULAR { get; set; }
        public string EMPR_NM_EMAIL { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO> ATENDIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO_PROPOSTA> ATENDIMENTO_PROPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ACAO> CRM_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_FOLLOW> CRM_FOLLOW { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUSTO_FIXO> CUSTO_FIXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_ANEXO> EMPRESA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_CUSTO_VARIAVEL> EMPRESA_CUSTO_VARIAVEL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_CUSTO_VARIAVEL_NORMAL> EMPRESA_CUSTO_VARIAVEL_NORMAL { get; set; }
        public virtual MAQUINA MAQUINA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_MAQUINA> EMPRESA_MAQUINA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_PLATAFORMA> EMPRESA_PLATAFORMA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_TICKET> EMPRESA_TICKET { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO> GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<META> META { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA> PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_EMAIL> TEMPLATE_EMAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_PROPOSTA> TEMPLATE_PROPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_SMS> TEMPLATE_SMS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VENDA_MENSAL> VENDA_MENSAL { get; set; }
        public virtual REGIME_TRIBUTARIO REGIME_TRIBUTARIO { get; set; }
    }
}
