using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class GrupoContatoViewModel
    {
        [Key]
        public int GRCL_CD_ID { get; set; }
        public int GRUP_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public int GRCL_IN_ATIVO { get; set; }

        public virtual CLIENTE CLIENTE { get; set; }
        public virtual GRUPO GRUPO { get; set; }
    }
}