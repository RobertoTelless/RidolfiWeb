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
    
    public partial class EMPRESA_TICKET
    {
        public int EMTI_CD_ID { get; set; }
        public int EMPR_CD_ID { get; set; }
        public int TICK_CD_ID { get; set; }
        public int EMTI_IN_ATIVO { get; set; }
    
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual TICKET_ALIMENTACAO TICKET_ALIMENTACAO { get; set; }
    }
}