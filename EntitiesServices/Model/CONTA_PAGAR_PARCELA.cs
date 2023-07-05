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
    
    public partial class CONTA_PAGAR_PARCELA
    {
        public int CPPA_CD_ID { get; set; }
        public int CAPA_CD_ID { get; set; }
        public Nullable<int> COBA_CD_ID { get; set; }
        public Nullable<int> CPPA_IN_PARCELA { get; set; }
        public Nullable<decimal> CPPA_VL_VALOR { get; set; }
        public Nullable<System.DateTime> CPPA_DT_VENCIMENTO { get; set; }
        public Nullable<int> CPPA_IN_ATIVO { get; set; }
        public string CPPA_DS_DESCRICAO { get; set; }
        public Nullable<System.DateTime> CPPA_DT_QUITACAO { get; set; }
        public Nullable<decimal> CPPA_VL_VALOR_PAGO { get; set; }
        public Nullable<decimal> CPPA_VL_DESCONTO { get; set; }
        public Nullable<decimal> CPPA_VL_JUROS { get; set; }
        public Nullable<decimal> CPPA_VL_TAXAS { get; set; }
        public Nullable<int> CPPA_IN_QUITADA { get; set; }
        public string CPPA_NR_PARCELA { get; set; }
        public Nullable<int> CPPA_IN_CHEQUE { get; set; }
        public string CPPA_NR_CHEQUE { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<int> CPPA_NR_ATRASO { get; set; }
    
        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        public virtual CONTA_PAGAR CONTA_PAGAR { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
    }
}
