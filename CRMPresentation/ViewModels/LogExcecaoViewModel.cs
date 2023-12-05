using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LogExcecaoViewModel
    {
        public int LOEX_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public string LOEX_NM_APLICACAO { get; set; }
        public System.DateTime LOEX_DT_DATA { get; set; }
        public string LOEX_NM_GERADOR { get; set; }
        public string LOEX_NM_TIPO_EXCECAO { get; set; }
        public string LOEX_DS_MENSAGEM { get; set; }
        public string LOEX_DS_INNER { get; set; }
        public string LOEX_NM_INNER_TIPO_EXCECAO { get; set; }
        public string LOEX_DS_STACK_TRACE { get; set; }
        public string LOEX_DS_SOURCE { get; set; }
        public Nullable<int> LOEX_IN_TIPO_REGISTRO { get; set; }

        public virtual USUARIO USUARIO { get; set; }

    }
}