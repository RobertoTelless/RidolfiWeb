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
    
    public partial class TEMPLATE_PROPOSTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEMPLATE_PROPOSTA()
        {
            this.CRM_PEDIDO_VENDA = new HashSet<CRM_PEDIDO_VENDA>();
            this.CRM_PEDIDO_VENDA1 = new HashSet<CRM_PEDIDO_VENDA>();
        }
    
        public int TEPR_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public string TEPR_SG_SIGLA { get; set; }
        public string TEPR_NM_NOME { get; set; }
        public string TEPR_TX_TEXTO { get; set; }
        public string TEPR_TX_CABECALHO { get; set; }
        public string TEPR_TX_RODAPE { get; set; }
        public string TEPR_TX_COMPLETO { get; set; }
        public int TEPR_IN_ATIVO { get; set; }
        public Nullable<int> TEPR_IN_FIXO { get; set; }
        public Nullable<int> TEPR_IN_TIPO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA1 { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
    }
}
