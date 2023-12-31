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
    
    public partial class PEDIDO_VENDA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PEDIDO_VENDA()
        {
            this.CRM = new HashSet<CRM>();
            this.PEDIDO_VENDA_ACOMPANHAMENTO = new HashSet<PEDIDO_VENDA_ACOMPANHAMENTO>();
            this.PEDIDO_VENDA_ANEXO = new HashSet<PEDIDO_VENDA_ANEXO>();
        }
    
        public int PEVE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<int> CECU_CD_ID { get; set; }
        public Nullable<int> PERI_CD_ID { get; set; }
        public Nullable<int> FOEN_CD_ID { get; set; }
        public Nullable<int> FOFR_CD_ID { get; set; }
        public string PEVE_NM_NOME { get; set; }
        public System.DateTime PEVE_DT_DATA { get; set; }
        public int PEVE_IN_STATUS { get; set; }
        public Nullable<System.DateTime> PEVE_DT_MUDANCA_STATUS { get; set; }
        public Nullable<System.DateTime> PEVE_DT_APROVACAO { get; set; }
        public Nullable<System.DateTime> PEVE_DT_CANCELAMENTO { get; set; }
        public string PEVE_DS_DESCRICAO { get; set; }
        public string PEVE_DS_JUSTIFICATIVA { get; set; }
        public Nullable<System.DateTime> PEVE_DT_VALIDADE { get; set; }
        public string PEVE_NM_FORMA_PAGAMENTO { get; set; }
        public string PEVE_TX_OBSERVACOES { get; set; }
        public int PEVE_IN_ATIVO { get; set; }
        public string PEVE_NR_NUMERO { get; set; }
        public Nullable<int> PEVE_NR_NUMERO_GERADO { get; set; }
        public Nullable<System.DateTime> PEVE_DT_PREVISTA { get; set; }
        public Nullable<System.DateTime> PEVE_DT_ALTERACAO { get; set; }
        public Nullable<System.DateTime> PEVE_DT_FINAL { get; set; }
        public Nullable<decimal> PEVE_VL_VALOR { get; set; }
        public Nullable<int> PEVE_IN_VENDEDOR { get; set; }
        public string PEVE_DS_JUSTIFICATIVA_OPORTUNIDADE { get; set; }
        public Nullable<System.DateTime> PEVE_DT_CRIACAO_OPORTUNIDADE { get; set; }
        public Nullable<System.DateTime> PEVE_DT_PREVISTA_OPORTUNIDADE { get; set; }
        public Nullable<System.DateTime> PEVE_DT_CRIACAO { get; set; }
        public Nullable<System.DateTime> PEVE_DT_CRIACAO_PROPOSTA { get; set; }
        public Nullable<System.DateTime> PEVE_DT_PREVISTA_PROPOSTA { get; set; }
        public string PEVE_NR_NUMERO_PROPOSTA { get; set; }
        public string PEVE_NR_OPORTUNIDADE { get; set; }
        public Nullable<decimal> PEVE_VL_DESCONTO { get; set; }
        public Nullable<System.DateTime> PV_DT_CANCELAMENTO_OPORTUNIDADE { get; set; }
        public Nullable<decimal> PEVE_VL_FRETE { get; set; }
        public Nullable<System.DateTime> PV_DT_CANCELAMENTO_PROPOSTA { get; set; }
        public string PEVE_DS_JUSTIFICATIVA_PROPOSTA { get; set; }
        public Nullable<System.DateTime> PEVE_DT_FATURAMENTO { get; set; }
        public Nullable<System.DateTime> PEVE_DT_VENCIMENTO { get; set; }
        public Nullable<int> PEVE_IN_PARCELAS { get; set; }
        public Nullable<System.DateTime> PEVE_DT_INICIO_PARCELAS { get; set; }
        public Nullable<int> PEVE_IN_NUMERO_PARCELAS { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CENTRO_CUSTO CENTRO_CUSTO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual FORMA_ENVIO FORMA_ENVIO { get; set; }
        public virtual FORMA_FRETE FORMA_FRETE { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA_ACOMPANHAMENTO> PEDIDO_VENDA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA_ANEXO> PEDIDO_VENDA_ANEXO { get; set; }
        public virtual PERIODICIDADE PERIODICIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}
