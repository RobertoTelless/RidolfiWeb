using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FaleConoscoViewModel
    {
        public String Telefone { get; set; }
        public String WhatsApp { get; set; }
        public String EMail { get; set; }
        public String Nome { get; set; }
        public String Resposta { get; set; }

        public String Mensagem { get; set; }
        public Nullable<int> TipoMensagem { get; set; }
        public Nullable<int> Assunto { get; set; }

    }
}