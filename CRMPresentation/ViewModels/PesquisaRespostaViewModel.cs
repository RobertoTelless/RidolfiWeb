using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PesquisaRespostaViewModel
    {
        [Key]
        public int PERE_CD_ID { get; set; }
        public int PESQ_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE RESPOSTA deve ser uma data válida")]
        public System.DateTime PERE_DT_RESPOSTA { get; set; }
        [Required(ErrorMessage = "Campo RESPONDENTE obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O RESPONDENTE deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string PERE_NM_RESPONDENTE { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 150 caracteres.")]
        public string PERE_EM_EMAIL_RESPONDENTE { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50 caracteres.")]
        public string PERE_NR_TELEFONE_RESPONDENTE { get; set; }
        [StringLength(500, ErrorMessage = "O COMENTÁRIO deve conter no máximo 500 caracteres.")]
        public string PERE_DS_COMENTARIOS { get; set; }
        public int PERE_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo CLIENTE obrigatorio")]
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> PEIT_CD_ID { get; set; }

        public virtual PESQUISA PESQUISA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_RESPOSTA_ITEM> PESQUISA_RESPOSTA_ITEM { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual PESQUISA_ITEM PESQUISA_ITEM { get; set; }
    }
}