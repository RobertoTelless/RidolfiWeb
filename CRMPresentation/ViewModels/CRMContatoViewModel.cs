using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMContatoViewModel
    {
        [Key]
        public int CRCO_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string CRCO_NM_NOME { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50 caracteres.")]
        public string CRCO_NR_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR deve conter no máximo 50 caracteres.")]
        public string CRCO_NR_CELULAR { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 e no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string CRCO_NM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "O CARGO deve conter no máximo 50 caracteres.")]
        public string CRCO_NM_CARGO { get; set; }
        [Required(ErrorMessage = "Campo PRINCIPAL obrigatorio")]
        public Nullable<int> CRCO_IN_PRINCIPAL { get; set; }
        public int CRCO_IN_ATIVO { get; set; }

        public virtual CRM CRM { get; set; }
    }
}