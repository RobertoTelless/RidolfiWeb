using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EscolaridadeViewModel
    {
        [Key]
        public int ESCO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string ESCO_NM_NOME { get; set; }
        public Nullable<int> ESCO_IN_ATIVO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ESCOLARIDADE> ESCOLARIDADE { get; set; }

    }
}