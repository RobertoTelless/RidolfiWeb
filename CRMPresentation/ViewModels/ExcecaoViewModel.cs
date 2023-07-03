using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ExcecaoViewModel
    {
        public DateTime DataExcecao { get; set; }
        public String Gerador { get; set; }
        public Int32 tipoVolta { get; set; }
        public String tipoExcecao { get; set; }
        public String Message { get; set; }
        public String Inner { get; set; }
        public String StackTrace { get; set; }
        public String Source { get; set; }
        public String SuporteZap { get; set; }
        public String SuporteMail { get; set; }
    }
}