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
    
    public partial class SUBCATEGORIA_PRODUTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SUBCATEGORIA_PRODUTO()
        {
            this.PECA = new HashSet<PECA>();
            this.PRODUTO = new HashSet<PRODUTO>();
        }
    
        public int SCPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> CAPR_CD_ID { get; set; }
        public string SCPR_NM_NOME { get; set; }
        public Nullable<int> SCPR_IN_ATIVO { get; set; }
        public Nullable<int> SCPR_IN_TIPO { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PECA> PECA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
    }
}
