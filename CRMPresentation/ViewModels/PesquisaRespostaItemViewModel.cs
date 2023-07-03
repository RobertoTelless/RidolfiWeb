using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PesquisaRespostaItemViewModel
    {
        [Key]
        public int PERT_CD_ID { get; set; }
        public int PERE_CD_ID { get; set; }
        public int PEIT_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo RESPOSTA obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A RESPOSTA deve conter no minimo 1 caracteres e no máximo 5000 caracteres.")]
        public string PERT_NM_RESPOSTA { get; set; }

        public virtual PESQUISA_ITEM PESQUISA_ITEM { get; set; }
        public virtual PESQUISA_RESPOSTA PESQUISA_RESPOSTA { get; set; }
    }
}