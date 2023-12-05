using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class InformePagamentoViewModel
    {
        public Int32 ASSI_CD_ID { get; set; }
        public String ASSI_NM_NOME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Required(ErrorMessage = "Campo DATA DE PAGAMENTO obrigatorio")]
        public Nullable<System.DateTime> INPA_DT_PAGAMENTO { get; set; }
        public String INPA_NR_COMPROVANTE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> INPA_VL_VALOR { get; set; }
        public String INPA_TX_OBSERVACOES { get; set; }
        [Required(ErrorMessage = "Campo MENSAGEM obrigatorio")]
        [StringLength(1000, ErrorMessage = "A MENSAGEM deve conter no máximo 1000 caracteres.")]
        public String INPA_TX_MENSAGEM { get; set; }
        public String INPA_AQ_ANEXO { get; set; }
        public String INPA_GU_GUID { get; set; }
        public Int32 ASPA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> INPA_DT_VENCIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Required(ErrorMessage = "Campo VALOR PAGO obrigatorio")]
        public Nullable<decimal> INPA_VL_VALOR_PAGO { get; set; }
    }
}