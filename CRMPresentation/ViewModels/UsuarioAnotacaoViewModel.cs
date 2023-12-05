using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class UsuarioAnotacaoViewModel
    {
        [Key]
        public int USAN_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> USAN_DT_ANOTACAO { get; set; }
        public int USAN_IN_AUTOR { get; set; }
        public string USAN_TX_ANOTACAO { get; set; }
        public Nullable<int> USAN_IN_ATIVO { get; set; }

        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}