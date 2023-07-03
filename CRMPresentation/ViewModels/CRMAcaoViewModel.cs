using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMAcaoViewModel
    {
        [Key]
        public int CRAC_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public Nullable<System.DateTime> CRAC_DT_CRIACAO { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public Nullable<int> USUA_CD_ID1 { get; set; }
        public Nullable<int> USUA_CD_ID2 { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O TÍTULO deve conter no minimo 1 e no máximo 150 caracteres.")]
        public string CRAC_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÂO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string CRAC_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA PREVISTA deve ser uma data válida")]
        public Nullable<System.DateTime> CRAC_DT_PREVISTA { get; set; }
        public Nullable<int> TIAC_CD_ID { get; set; }
        public Nullable<int> CRAC_IN_STATUS { get; set; }
        public Nullable<int> CRAC_IN_ATIVO { get; set; }

        public Nullable<int> CRIA_AGENDA { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }

        public virtual CRM CRM { get; set; }
        public virtual TIPO_ACAO TIPO_ACAO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }

    }
}