using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PesquisaAnotacaoViewModel
    {
        [Key]
        public int PECO_CD_ID { get; set; }
        public int PESQ_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PECO_DT_ANOTACAO { get; set; }
        [Required(ErrorMessage = "Campo ANOTAÇÃO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 caracteres e no máximo 5000 caracteres.")]
        public string PECO_TX_ANOTACAO { get; set; }
        public int PECO_IN_ATIVO { get; set; }

        public virtual PESQUISA PESQUISA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}