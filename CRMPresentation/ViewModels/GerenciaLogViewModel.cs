using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class GerenciaLogViewModel
    {
        public System.DateTime DATA_INICIO { get; set; }
        public System.DateTime DATA_FINAL { get; set; }
        public string OPERACAO { get; set; }
        public Int32 USUARIO { get; set; }
        public Int32 CONTA { get; set; }
        public List<LOG> LISTA { get; set; }


    }
}