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
    
    public partial class CONTATO_FALHA
    {
        public int COFA_CD_ID { get; set; }
        public string COFA_NR_PROTOCOLO { get; set; }
        public string COFA_NR_PRECATORIO { get; set; }
        public string COFA_NM_CONTATO { get; set; }
        public string COFA_NM_OPERADOR { get; set; }
        public Nullable<System.DateTime> COFA_DT_DATA { get; set; }
        public Nullable<System.DateTime> COFA_DT_DATA_FALHA { get; set; }
        public string COFA_DS_MOTIVO { get; set; }
    }
}
