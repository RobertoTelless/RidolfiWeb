using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EmpresaOutorsCustosViewModel
    {
        [Key]
        public int EMCV_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo EMPRRESA obrigatorio")]
        public Nullable<int> EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, ErrorMessage = "O NOME deve conter no máximo 50 caracteres.")]
        public string EMCV_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMCV_VL_VALOR { get; set; }
        public Nullable<int> EMCV_IN_ATIVO { get; set; }

        public virtual EMPRESA EMPRESA { get; set; }
    }
}