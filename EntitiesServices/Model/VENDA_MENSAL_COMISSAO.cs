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
    
    public partial class VENDA_MENSAL_COMISSAO
    {
        public int VECO_CD_ID { get; set; }
        public Nullable<int> VEMA_CD_ID { get; set; }
        public Nullable<decimal> VECO_VL_VENDEDOR { get; set; }
        public Nullable<decimal> VECO_VL_GERENTE { get; set; }
        public Nullable<decimal> VECO_VL_OUTROS { get; set; }
        public Nullable<int> VECO_IN_ATIVO { get; set; }
    
        public virtual VENDA_MENSAL VENDA_MENSAL { get; set; }
    }
}
