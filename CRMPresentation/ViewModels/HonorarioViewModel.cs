using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class HonorarioViewModel
    {
        [Key]
        public int HONO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        public int TIPE_CD_ID { get; set; }
        [StringLength(250, ErrorMessage = "O CPF deve conter no máximo 20 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string HONO_NR_CPF { get; set; }
        [StringLength(250, ErrorMessage = "O CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string HONO_NR_CNPJ { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 250 caracteres.")]
        public string HONO_NM_NOME { get; set; }
        [StringLength(250, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 250 caracteres.")]
        public string HONO_NM_RAZAO_SOCIAL { get; set; }
        public int HONO_IN_ATIVO { get; set; }
        public System.DateTime HONO_DT_CADASTRO { get; set; }
        public string HONO_NR_OAB { get; set; }

        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECATORIO> PRECATORIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HONORARIO_ANEXO> HONORARIO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HONORARIO_ANOTACOES> HONORARIO_ANOTACOES { get; set; }
    }
}