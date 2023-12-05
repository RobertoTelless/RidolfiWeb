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
    
    public partial class NOTIFICACAO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NOTIFICACAO()
        {
            this.NOTIFICACAO_ANEXO = new HashSet<NOTIFICACAO_ANEXO>();
        }
    
        public int NOTI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int CANO_CD_ID { get; set; }
        public Nullable<int> ATEN_CD_ID { get; set; }
        public Nullable<System.DateTime> NOTI_DT_EMISSAO { get; set; }
        public int NOTI_IN_ATIVO { get; set; }
        public Nullable<int> NOTI_IN_STATUS { get; set; }
        public string NOTI_TX_TEXTO { get; set; }
        public string NOTI_NM_TITULO { get; set; }
        public Nullable<int> NOTI_IN_VISTA { get; set; }
        public Nullable<System.DateTime> NOTI_DT_VALIDADE { get; set; }
        public Nullable<System.DateTime> NOTI_DT_VISTA { get; set; }
        public Nullable<int> NOTI_IN_ORIGEM { get; set; }
        public Nullable<int> NOTI_IN_NIVEL { get; set; }
        public Nullable<int> NOTI_IN_ANEXOS { get; set; }
    
        public virtual CATEGORIA_NOTIFICACAO CATEGORIA_NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO_ANEXO> NOTIFICACAO_ANEXO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
