using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PesquisaItemOpcaoViewModel
    {
        [Key]
        public int PEIO_CD_ID { get; set; }
        public int PEIT_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo OPÇÃO obrigatorio")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "A OPÇÃO deve conter no minimo 1 caracteres e no máximo 500 caracteres.")]
        public string PEIO_NM_OPCAO { get; set; }
        public int PEIO_IN_ATIVO { get; set; }
        public Nullable<int> PEIO_IN_ORDEM { get; set; }

        public virtual PESQUISA_ITEM PESQUISA_ITEM { get; set; }
    }
}