using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AntecipacaoDataViewModel
    {
        [Key]
        public int ANTD_CD_ID { get; set; }
        public int ANTE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ANTD_DT_DATA { get; set; }
        [Required(ErrorMessage = "Campo PERCENTUAL DAS VENDAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> ANTD_PC_PERCENTUAL { get; set; }
        public int ANTD_IN_ATIVO { get; set; }

        public virtual ANTECIPACAO ANTECIPACAO { get; set; }
    }
}