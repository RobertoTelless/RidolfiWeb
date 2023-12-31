using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ModeloViewModel
    {
        public DateTime DataEmissao { get; set; }
        public String Data { get; set; }
        public Int32 Valor { get; set; }
        public Int32 Valor1 { get; set; }
        public Decimal ValorDec { get; set; }
        public Decimal ValorDec1 { get; set; }
        public Decimal ValorDec2 { get; set; }
        public Decimal ValorDec3 { get; set; }
        public Decimal ValorDec4 { get; set; }
        public Decimal ValorDec5 { get; set; }
        public String Nome { get; set; }
        public Double ValorDouble { get; set; }
        public Double ValorDouble2 { get; set; }
        public String Nome1 { get; set; }

    }
}