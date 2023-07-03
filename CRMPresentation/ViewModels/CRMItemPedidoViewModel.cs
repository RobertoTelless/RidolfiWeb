using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMItemPedidoViewModel
    {
        [Key]
        public int CRPI_CD_ID { get; set; }
        public int CRPV_CD_ID { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int CRPI_IN_QUANTIDADE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPI_VL_PRECO_AJUSTADO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPI_VL_PRECO_TOTAL { get; set; }
        [StringLength(1000, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 1000 caracteres.")]
        public string CRPI_TX_OBSERVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CRPI_DT_JUSTIFICATIVA { get; set; }
        [StringLength(1000, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 1000 caracteres.")]
        public string CRPI_DS_JUSTIFICATIVA { get; set; }
        public int CRPI_IN_ATIVO { get; set; }
        public Nullable<int> CRPI_IN_QUANTIDADE_REVISADA { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<decimal> CRPI_VL_MARKUP { get; set; }
        public Nullable<decimal> CRPI_VL_DESCONTO { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE ITEM obrigatorio")]
        public Nullable<int> CRPI_IN_TIPO_ITEM { get; set; }
        public Nullable<int> SERV_CD_ID { get; set; }
        public Nullable<int> CRPI_IN_DIFERENCA { get; set; }

        public virtual CRM_PEDIDO_VENDA CRM_PEDIDO_VENDA { get; set; }
    }
}