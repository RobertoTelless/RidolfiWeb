using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class VideoComentarioViewModel
    {
        [Key]
        public int VICO_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int VIDE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime VICO_DT_COMENTARIO { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000.")]
        public string VICO_TX_COMENTARIO { get; set; }
        public int VICO_IN_ATIVO { get; set; }

        public virtual VIDEO VIDEO { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}