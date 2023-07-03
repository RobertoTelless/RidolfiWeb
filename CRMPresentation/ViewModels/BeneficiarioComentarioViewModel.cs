using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class BeneficiarioComentarioViewModel
    {
        [Key]
        public int BECO_CD_ID { get; set; }
        public int BENE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> BECO_DT_COMENTARIO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string BECO_TX_TEXTO { get; set; }
        public Nullable<int> BECO_IN_ATIVO { get; set; }

        public virtual BENEFICIARIO BENEFICIARIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}