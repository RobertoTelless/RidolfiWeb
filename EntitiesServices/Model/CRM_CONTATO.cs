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
    
    public partial class CRM_CONTATO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CRM_CONTATO()
        {
            this.MENSAGENS_ENVIADAS_SISTEMA = new HashSet<MENSAGENS_ENVIADAS_SISTEMA>();
        }
    
        public int CRCO_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        public string CRCO_NM_NOME { get; set; }
        public string CRCO_NR_TELEFONE { get; set; }
        public string CRCO_NR_CELULAR { get; set; }
        public string CRCO_NM_EMAIL { get; set; }
        public string CRCO_NM_CARGO { get; set; }
        public Nullable<int> CRCO_IN_PRINCIPAL { get; set; }
        public int CRCO_IN_ATIVO { get; set; }
    
        public virtual CRM CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_ENVIADAS_SISTEMA> MENSAGENS_ENVIADAS_SISTEMA { get; set; }
    }
}
