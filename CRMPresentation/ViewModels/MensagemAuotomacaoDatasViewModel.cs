    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MensagemAutomacaoDatasViewModel
    {
        [Key]
        public int MEAD_CD_ID { get; set; }
        public int MEAU_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA deve ser uma data válida")]
        public Nullable<System.DateTime> MEAD_DT_DATA { get; set; }
        public Nullable<int> MEAD_IN_ATIVO { get; set; }

        public virtual MENSAGEM_AUTOMACAO MENSAGEM_AUTOMACAO { get; set; }
    }
}