using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TabelaIPCAViewModel
    {
        [Key]
        public int IPCA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE REFERÊNCIA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE REFERÊNCIA Deve ser uma data válida")]
        public System.DateTime IPCA_DT_REFERENCIA { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public decimal IPCA_VL_VALOR_DECIMAL { get; set; }
        public int IPCA_IN_ATIVO { get; set; }
    }
}