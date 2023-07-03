using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_CRM_Solution.ViewModels
{
    public class TelefoneBenefViewModel
    {
        [Key]
        public int TELE_CD_ID { get; set; }
        public int BENE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE TELEFONE obrigatorio")]
        public int TITE_CD_ID { get; set; }
        [StringLength(30,  ErrorMessage = "O TELEFONE deve conter no máximo 30 caracteres.")]
        public string TELE_NR_NUMERO { get; set; }
        public int TELE_IN_ATIVO { get; set; }
        [StringLength(30, ErrorMessage = "O CELULAR deve conter no máximo 30 caracteres.")]
        public string TELE_NR_CELULAR { get; set; }

        public virtual BENEFICIARIO BENEFICIARIO { get; set; }
        public virtual TIPO_TELEFONE TIPO_TELEFONE { get; set; }

    }
}