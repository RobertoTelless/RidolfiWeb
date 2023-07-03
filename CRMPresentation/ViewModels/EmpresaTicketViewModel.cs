using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EmpresaTicketViewModel
    {
        [Key]
        public int EMTI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo EMPRRESA obrigatorio")]
        public int EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TICKET obrigatorio")]
        public int TICK_CD_ID { get; set; }
        public int EMTI_IN_ATIVO { get; set; }

        public virtual EMPRESA EMPRESA { get; set; }
        public virtual TICKET_ALIMENTACAO TICKET_ALIMENTACAO { get; set; }
    }
}