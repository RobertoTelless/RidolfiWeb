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
    
    public partial class CATEGORIA_FORNECEDOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CATEGORIA_FORNECEDOR()
        {
            this.FORNECEDOR = new HashSet<FORNECEDOR>();
        }
    
        public int CAFO_CD_ID { get; set; }
        public string CAFO_NM_NOME { get; set; }
        public Nullable<int> CAFO_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR> FORNECEDOR { get; set; }
    }
}
