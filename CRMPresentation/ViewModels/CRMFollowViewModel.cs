using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMFollowViewModel
    {
        [Key]
        public int CRFL_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE ACOMPANHAMENTO obrigatorio")]
        public Nullable<int> TIFL_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime CRFL_DT_FOLLOW { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string CRFL_DS_FOLLOW { get; set; }
        public int CRFL_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O TÍTULO deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string CRFL_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA PREVISTA deve ser uma data válida")]
        public Nullable<System.DateTime> CRFL_DT_PREVISTA { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }

        public virtual CRM CRM { get; set; }
        public virtual TIPO_FOLLOW TIPO_FOLLOW { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }

    }
}