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
    
    public partial class VARA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VARA()
        {
            this.CLIENTE = new HashSet<CLIENTE>();
        }
    
        public int VARA_CD_ID { get; set; }
        public int TRF1_CD_ID { get; set; }
        public string VARA_NM_NOME { get; set; }
        public int VARA_IN_ATIVO { get; set; }
        public string VARA_TX_OBSERVACOES { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        public virtual TRF TRF { get; set; }
    }
}