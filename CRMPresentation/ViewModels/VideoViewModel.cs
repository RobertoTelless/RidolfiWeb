using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class VideoViewModel
    {
        [Key]
        public int VIDE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O TÍTULO deve ter no minimo 1 caractere e no máximo 100.")]
        public string VIDE_NM_TITULO { get; set; }
        [StringLength(1000, ErrorMessage = "A DESCRIÇÃO deve ter máximo 1000 caracteres.")]
        public string VIDE_NM_DESCRICAO { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO ARQUIVO deve ter máximo 250 caracteres.")]
        public string VIDE_AQ_FOTO { get; set; }
        [Required(ErrorMessage = "Campo LINK obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O LINK deve ter no minimo 1 caractere e no máximo 250 caracteres.")]
        public string VIDE_LK_LINK { get; set; }
        [Required(ErrorMessage = "Campo DATA DE EMISSÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE EMISSÂO Deve ser uma data válida")]
        public System.DateTime VIDE_DT_EMISSAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE VALIDADE Deve ser uma data válida")]
        public System.DateTime VIDE_DT_VALIDADE { get; set; }
        public int VIDE_NR_ACESSOS { get; set; }
        [StringLength(100, ErrorMessage = "O AUTOR deve ter no máximo 100 caracteres.")]
        public string VIDE_NM_AUTOR { get; set; }
        public int VIDE_IN_ATIVO { get; set; }

        public virtual ICollection<VIDEO_COMENTARIO> VIDEO_COMENTARIO { get; set; }
    }
}