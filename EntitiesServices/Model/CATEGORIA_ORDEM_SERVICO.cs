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
    
    public partial class CATEGORIA_ORDEM_SERVICO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CATEGORIA_ORDEM_SERVICO()
        {
            this.ORDEM_SERVICO = new HashSet<ORDEM_SERVICO>();
        }
    
        public int CAOS_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public string CAOS_NM_NOME { get; set; }
        public Nullable<int> CAOS_IN_ATIVO { get; set; }
        public Nullable<int> CAOS_IN_SLA { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
    }
}
