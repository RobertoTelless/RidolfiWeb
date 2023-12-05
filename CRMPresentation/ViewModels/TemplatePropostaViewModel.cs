using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TemplatePropostaViewModel
    {
        [Key]
        public int TEPR_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9]|-|_|\s)+$$", ErrorMessage = "Nome inválido")]
        public string TEPR_SG_SIGLA { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9]|-|_|\s)+$$", ErrorMessage = "Sigla inválida")]
        public string TEPR_NM_NOME { get; set; }
        [AllowHtml]
        public string TEPR_TX_TEXTO { get; set; }
        [AllowHtml]
        public string TEPR_TX_CABECALHO { get; set; }
        [AllowHtml]
        public string TEPR_TX_RODAPE { get; set; }
        public int TEPR_IN_ATIVO { get; set; }
        public string TEPR_TX_COMPLETO { get; set; }
        public Nullable<int> TEPR_IN_FIXO { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE MODELO obrigatorio")]
        public Nullable<int> TEPR_IN_TIPO { get; set; }

        public String TIPO
        {
            get
            {
                if (TEPR_IN_TIPO == 1)
                {
                    return "Proposta";
                }
                return "E-Mail de Envio";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA1 { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
    }
}