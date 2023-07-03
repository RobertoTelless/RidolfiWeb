using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TabelaIRRFViewModel
    {
        [Key]
        public int IRRF_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FAIXA obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int IRRF_IN_FAIXA { get; set; }
        [Required(ErrorMessage = "Campo AlÍQUOTA obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal IRRF_VL_ALIQUOTA { get; set; }
        [Required(ErrorMessage = "Campo PORCENTAGEM obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal IRRF_VL_PORCENTAGEM { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal IRRF_VL_VALOR { get; set; }
        public int IRRF_IN_ATIVO { get; set; }
    }
}