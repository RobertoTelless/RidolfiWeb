using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AntecipacaoViewModel
    {
        [Key]
        public int ANTE_CD_ID { get; set; }
        public int EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo MÁQUINA obrigatorio")]
        public Nullable<int> MAQN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA INICIAL obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ANTE_DT_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ANTE_DT_FIM { get; set; }
        public int ANTE_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> ANTE_VL_VALOR { get; set; }
        public int ASSI_CD_ID { get; set; }

        public virtual EMPRESA EMPRESA { get; set; }
        public virtual MAQUINA MAQUINA { get; set; }
    }
}