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
    
    public partial class AGENDA_ANEXO
    {
        public int AGAN_CD_ID { get; set; }
        public int AGEN_CD_ID { get; set; }
        public string AGAN_NM_TITULO { get; set; }
        public System.DateTime AGAN_DT_ANEXO { get; set; }
        public int AGAN_IN_TIPO { get; set; }
        public string AGAN_AQ_ARQUIVO { get; set; }
        public int AGAN_IN_ATIVO { get; set; }
    
        public virtual AGENDA AGENDA { get; set; }
    }
}