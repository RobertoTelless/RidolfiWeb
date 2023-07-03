using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PesquisaItemViewModel
    {
        [Key]
        public int PEIT_CD_ID { get; set; }
        public int PESQ_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE ITEM obrigatorio")]
        public int TIIT_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public int PEIT_IN_ORDEM { get; set; }
        [Required(ErrorMessage = "Campo PERGUNTA obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A PERGUNTA deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string PEIT_NM_PERGUNTA { get; set; }
        [Required(ErrorMessage = "Campo TEXTO DA PERGUNTA obrigatorio")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "O TEXTO DA PERGUNTA deve conter no minimo 1 caracteres e no máximo 1000 caracteres.")]
        public string PEIT_DS_PERGUNTA { get; set; }
        public int PEIT_IN_OBRIGATORIA { get; set; }
        public int PEIT_IN_ATIVO { get; set; }
        public string PEIT_DS_RESPOSTA_TEXTO { get; set; }
        public int USUA_CD_ID { get; set; }

        public virtual PESQUISA PESQUISA { get; set; }
        public virtual TIPO_ITEM_PESQUISA TIPO_ITEM_PESQUISA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_ITEM_OPCAO> PESQUISA_ITEM_OPCAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_RESPOSTA_ITEM> PESQUISA_RESPOSTA_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_RESPOSTA> PESQUISA_RESPOSTA { get; set; }
    }
}