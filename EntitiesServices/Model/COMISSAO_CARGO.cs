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
    
    public partial class COMISSAO_CARGO
    {
        public int COCA_CD_ID { get; set; }
        public int CARG_CD_ID { get; set; }
        public string COCA_NM_NOME { get; set; }
        public decimal COCA_PC_PERCENTUAL { get; set; }
        public int COCA_IN_TIPO_COMISSAO { get; set; }
        public int COCA_IN_ATIVO { get; set; }
    
        public virtual CARGO CARGO { get; set; }
    }
}
