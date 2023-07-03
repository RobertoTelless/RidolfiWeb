using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FunilEtapaViewModel
    {
        [Key]
        public int FUET_CD_ID { get; set; }
        public int FUNI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string FUET_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10 caracteres.")]
        public string FUET_SG_SIGLA { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string FUET_DS_DESCRICAO { get; set; }
        public int FUET_IN_ENCERRA { get; set; }
        public int FUET_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo ORDEM NO PROCESSO obrigatorio")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> FUET_IN_ORDEM { get; set; }
        public Nullable<int> FUET_IN_PROPOSTA { get; set; }
        public Nullable<int> FUET_IN_EMAIL { get; set; }
        public Nullable<int> FUET_IN_SMS { get; set; }

        public bool Encerra
        {
            get
            {
                if (FUET_IN_ENCERRA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FUET_IN_ENCERRA = (value == true) ? 1 : 0;
            }
        }
        public bool Proposta
        {
            get
            {
                if (FUET_IN_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FUET_IN_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool EMail
        {
            get
            {
                if (FUET_IN_EMAIL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FUET_IN_EMAIL = (value == true) ? 1 : 0;
            }
        }
        public bool SMS
        {
            get
            {
                if (FUET_IN_SMS == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FUET_IN_SMS = (value == true) ? 1 : 0;
            }
        }

        public virtual FUNIL FUNIL { get; set; }
    }
}