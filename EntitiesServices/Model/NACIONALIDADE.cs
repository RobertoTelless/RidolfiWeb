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
    
    public partial class NACIONALIDADE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NACIONALIDADE()
        {
            this.CLIENTE = new HashSet<CLIENTE>();
        }
    
        public int NACI_CD_ID { get; set; }
        public Nullable<int> LING_CD_ID { get; set; }
        public string NACI_NM_PAIS { get; set; }
        public string NACI_SG_SIGLA { get; set; }
        public int NACI_IN_ATIVO { get; set; }
        public int NACI_IN_FIXO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        public virtual LINGUA LINGUA { get; set; }
    }
}
