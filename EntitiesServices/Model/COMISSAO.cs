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
    
    public partial class COMISSAO
    {
        public int COMI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<System.DateTime> COMI_DT_REFERENCIA { get; set; }
        public Nullable<int> COMI_IN_TIPO { get; set; }
        public Nullable<decimal> COMI_VL_VALOR { get; set; }
        public Nullable<decimal> COMI_VL_GERENTE { get; set; }
        public Nullable<decimal> COMI_VL_OUTROS { get; set; }
        public Nullable<decimal> COMI_PC_VENDEDOR { get; set; }
        public Nullable<decimal> COMI_PC_GERENTE { get; set; }
        public Nullable<decimal> COMI_PC_OUTROS { get; set; }
        public int COMI_IN_ATIVO { get; set; }
    }
}
