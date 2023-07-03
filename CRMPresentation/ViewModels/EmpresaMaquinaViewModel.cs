using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EmpresaMaquinaViewModel
    {
        [Key]
        public int EMMA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo EMPRRESA obrigatorio")]
        public int EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo MAQUINA obrigatorio")]
        public int MAQN_CD_ID { get; set; }
        public int EMMA_IN_ATIVO { get; set; }

        public virtual EMPRESA EMPRESA { get; set; }
        public virtual MAQUINA MAQUINA { get; set; }

    }
}