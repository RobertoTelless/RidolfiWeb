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
    
    public partial class PRODUTO_CONCORRENTE
    {
        public int PRPF_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public string PRPF_NM_CONCORRENTE { get; set; }
        public decimal PRPF_VL_PRECO_CONCORRENTE { get; set; }
        public int PRPF_IN_ATIVO { get; set; }
        public System.DateTime PRPF_DT_CADASTRO { get; set; }
    
        public virtual PRODUTO PRODUTO { get; set; }
    }
}