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
    
    public partial class TEMPLATE_SMS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEMPLATE_SMS()
        {
            this.MENSAGEM_AUTOMACAO = new HashSet<MENSAGEM_AUTOMACAO>();
            this.MENSAGENS = new HashSet<MENSAGENS>();
        }
    
        public int TSMS_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public string TSMS_NM_NOME { get; set; }
        public string TSMS_SG_SIGLA { get; set; }
        public string TSMS_TX_CORPO { get; set; }
        public string TSMS_LK_LINK { get; set; }
        public int TSMS_IN_ATIVO { get; set; }
        public Nullable<int> TSMS_IN_FIXO { get; set; }
    
        public virtual EMPRESA EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_AUTOMACAO> MENSAGEM_AUTOMACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
    }
}
