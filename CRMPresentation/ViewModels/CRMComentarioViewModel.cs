using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMComentarioViewModel
    {
        [Key]
        public int CRCM_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public Nullable<System.DateTime> CRCM_DT_COMENTARIO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9]|-|_|\s)+$$", ErrorMessage = "Anotação inválida")]
        public string CRCM_DS_COMENTARIO { get; set; }
        public Nullable<int> CRCM_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }

        public virtual CRM CRM { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }
    }
}