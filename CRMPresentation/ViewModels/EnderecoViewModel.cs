using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EnderecoViewModel
    {
        [Key]
        public int ENDE_CD_ID { get; set; }
        public int BENE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ENDEREÇO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 250 caracteres.")]
        public string ENDE_NM_ENDERECO { get; set; }
        [Required(ErrorMessage = "Campo BAIRRO obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O BAIRRO deve conter no minimo 1 caracteres e no máximo 100 caracteres.")]
        public string ENDE_NM_BAIRRO { get; set; }
        [Required(ErrorMessage = "Campo CIDADE obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "A CIDADE deve conter no minimo 1 caracteres e no máximo 100 caracteres.")]
        public string ENDE_NM_CIDADE { get; set; }
        [Required(ErrorMessage = "Campo UF obrigatorio")]
        public int UF_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CEP obrigatorio")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "O CEP deve conter no minimo 8 caracteres e no máximo 10 caracteres.")]
        public string ENDE_NR_CEP { get; set; }
        public int ENDE_IN_ATIVO { get; set; }

        public virtual BENEFICIARIO BENEFICIARIO { get; set; }
        public virtual UF UF { get; set; }
    }
}