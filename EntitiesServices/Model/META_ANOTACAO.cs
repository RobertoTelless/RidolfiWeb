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
    
    public partial class META_ANOTACAO
    {
        public int MEAT_CD_ID { get; set; }
        public int META_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public System.DateTime MEAT_DT_ANOTACAO { get; set; }
        public string MEAT_TX_TEXTO { get; set; }
        public int MEAT_IN_ATIVO { get; set; }
    
        public virtual META META { get; set; }
    }
}
