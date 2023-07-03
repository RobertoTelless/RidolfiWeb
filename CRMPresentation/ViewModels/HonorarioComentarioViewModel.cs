using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class HonorarioComentarioViewModel
    {
        [Key]
        public int HOAT_CD_ID { get; set; }
        public int HONO_CD_ID { get; set; }
        public Nullable<System.DateTime> HOAT_DT_DATA { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string HOAT_TX_TEXTO { get; set; }
        public Nullable<int> HOAT_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime HOAT_DT_COMENTARIO { get; set; }

        public virtual HONORARIO HONORARIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}