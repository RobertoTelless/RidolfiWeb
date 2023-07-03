using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AssinanteAnotacaoViewModel
    {
        [Key]
        public int ASAT_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime ASAT_DT_ANOTACAO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string ASAT_TX_ANOTACAO { get; set; }
        public int ASAT_IN_ATIVO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}