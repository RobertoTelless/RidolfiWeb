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
    
    public partial class PRODUTO_ANOTACAO
    {
        public int PRAT_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public System.DateTime PRAT_DT_ANOTACAO { get; set; }
        public string PRAT_DS_ANOTACAO { get; set; }
        public int PRAT_IN_ATIVO { get; set; }
    
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
