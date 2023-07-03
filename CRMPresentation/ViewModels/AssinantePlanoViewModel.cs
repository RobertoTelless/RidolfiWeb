using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AssinantePlanoViewModel
    {
        [Key]
        public int ASPL_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINANTE obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PLANO obrigatorio")]
        public int PLAN_CD_ID { get; set; }
        public int ASPL_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE INÍCIO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE INÍCIO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPL_DT_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENCERRAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPL_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PREÇO obrigatorio")]
        public int ASPL_IN_PRECO { get; set; }
        public int ASPL_IN_PAGTO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual PLANO PLANO { get; set; }

    }
}