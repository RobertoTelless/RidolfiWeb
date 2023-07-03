using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ClienteAnotacaoViewModel
    {
        [Key]
        public int CLAN_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime CLAN_DT_ANOTACAO { get; set; }
        [Required(ErrorMessage = "Campo ANOTAÇÃO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000 caracteres.")]
        public string CLAN_TX_ANOTACAO { get; set; }
        public int CLAN_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CLAN_DT_ANOTACAO_HORA { get; set; }

        public virtual CLIENTE CLIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}