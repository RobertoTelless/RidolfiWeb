using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PrecatorioComentarioViewModel
    {
        [Key]
        public int PRAT_CD_ID { get; set; }
        public int PREC_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PRAT_DT_ANOTACAO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string PRAT_TX_ANOTACAO { get; set; }
        public int PRAT_IN_ATIVO { get; set; }

        public virtual PRECATORIO PRECATORIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}