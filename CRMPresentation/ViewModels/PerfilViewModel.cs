using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class PerfilViewModel
    {
        [Key]
        public int PERF_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve ter no minimo 1 caractere e no máximo 10.")]
        public string PERF_SG_SIGLA { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve ter no minimo 1 caractere e no máximo 50.")]
        public string PERF_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo ATIVO obrigatorio")]
        public Nullable<int> PERF_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo FIXO obrigatorio")]
        public Nullable<int> PERF_IN_FIXO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }

    }
}