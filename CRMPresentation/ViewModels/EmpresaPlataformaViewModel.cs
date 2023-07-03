using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EmpresaPlataformaViewModel
    {
        [Key]
        public int EMPL_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo EMPRRESA obrigatorio")]
        public int EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PLATAFORMA DE ENTREGA obrigatorio")]
        public int PLEN_CD_ID { get; set; }
        public int EMPL_IN_ATIVO { get; set; }

        public virtual EMPRESA EMPRESA { get; set; }
        public virtual PLATAFORMA_ENTREGA PLATAFORMA_ENTREGA { get; set; }
    }
}