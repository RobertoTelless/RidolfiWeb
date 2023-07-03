using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TRFViewModel
    {
        [Key]
        public int TRF1_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UF obrigatorio")]
        public int UF_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O NOME deve conter no minimo 3 caracteres e no máximo 50 caracteres.")]
        public string TRF1_NM_NOME { get; set; }
        public byte[] TRF1_TX_OBSERVACOES { get; set; }
        public int TRF1_IN_ATIVO { get; set; }
        public string TRF1_TX_OBSERVACAO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOAS> LOAS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECATORIO> PRECATORIO { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VARA> VARA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }

    }
}