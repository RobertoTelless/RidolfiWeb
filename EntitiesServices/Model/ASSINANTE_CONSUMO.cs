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
    
    public partial class ASSINANTE_CONSUMO
    {
        public int ASCO_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<System.DateTime> ASCO_DT_REFERENCIA { get; set; }
        public Nullable<int> ASCO_NR_USUARIO { get; set; }
        public Nullable<int> ASCO_NR_CONTATO { get; set; }
        public Nullable<int> ASCO_NR_EMAIL { get; set; }
        public Nullable<int> ASCO_NR_SMS { get; set; }
        public Nullable<int> ASCO_NR_WHATSAPP { get; set; }
        public Nullable<int> ASCO_NR_PROCESSO { get; set; }
        public Nullable<int> ASCO_NR_ACOES { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}