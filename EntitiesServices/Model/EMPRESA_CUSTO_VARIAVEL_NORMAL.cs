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
    
    public partial class EMPRESA_CUSTO_VARIAVEL_NORMAL
    {
        public int EMCF_CD_ID { get; set; }
        public int EMPR_CD_ID { get; set; }
        public string EMCF_NM_NOME { get; set; }
        public decimal EMCF_PC_TAXA { get; set; }
        public int EMCF_IN_ATIVO { get; set; }
    
        public virtual EMPRESA EMPRESA { get; set; }
    }
}
