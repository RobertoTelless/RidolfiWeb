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
    
    public partial class HONORARIO_ANOTACOES
    {
        public int HOAT_CD_ID { get; set; }
        public int HONO_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> HOAT_DT_DATA { get; set; }
        public string HOAT_TX_TEXTO { get; set; }
        public int HOAT_IN_ATIVO { get; set; }
        public System.DateTime HOAT_DT_COMENTARIO { get; set; }
    
        public virtual HONORARIO HONORARIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
