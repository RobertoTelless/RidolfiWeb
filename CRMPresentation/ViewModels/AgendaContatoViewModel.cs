using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AgendaContatoViewModel
    {
        [Key]
        public int AGCO_CD_ID { get; set; }
        public int AGEN_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public int AGCO_IN_TIPO { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 100 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "NOME com caracteres inválidos")]
        public string AGCO_NM_NOME { get; set; }
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 e no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string AGCO_NM_EMAIL { get; set; }
        public int AGCO_IN_ATIVO { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "A ANOTAÇÃO deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string AGCO_DS_ANOTACAO { get; set; }
        public Nullable<int> AGCO_IN_ENVIO { get; set; }

        public String Tipo
        {
            get
            {
                if (AGCO_IN_TIPO == 1)
                {
                    return "Interno";
                }
                if (AGCO_IN_TIPO == 2)
                {
                    return "Externo";
                }
                return "-";
            }
        }
        public String Envio
        {
            get
            {
                if (AGCO_IN_ENVIO == 0)
                {
                    return "Não";
                }
                if (AGCO_IN_ENVIO == 1)
                {
                    return "Sim";
                }
                return "-";
            }
        }

        public virtual AGENDA AGENDA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}