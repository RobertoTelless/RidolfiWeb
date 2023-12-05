using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMPedidoComentarioViewModel
    {
        [Key]
        public int CRPC_CD_ID { get; set; }
        public int CRPV_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime CRPC_DT_ACOMPANHAMENTO { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9]|-|_|\s)+$$", ErrorMessage = "Anotação inválida")]
        public string CRPC_TX_ACOMPANHAMENTO { get; set; }
        public int CRPC_IN_ATIVO { get; set; }

        public virtual CRM_PEDIDO_VENDA CRM_PEDIDO_VENDA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}