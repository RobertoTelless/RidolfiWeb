using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;

namespace ERP_Condominios_Solution.ViewModels
{
    public class VaraViewModel
    {
        [Key]
        public int VARA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TRF obrigatorio")]
        public int TRF1_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O NOME deve conter no minimo 3 caracteres e no máximo 50 caracteres.")]
        public string VARA_NM_NOME { get; set; }
        public int VARA_IN_ATIVO { get; set; }
        public string VARA_TX_OBSERVACOES { get; set; }

        public virtual TRF TRF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }

    }
}