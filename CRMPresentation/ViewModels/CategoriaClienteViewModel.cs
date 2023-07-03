using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CategoriaClienteViewModel
    {
        [Key]
        public int CACL_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string CACL_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo LIMITE MÍNIMO EMITIDAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CACL_IN_LIMITE_MINIMO_EMITIDAS { get; set; }
        [Required(ErrorMessage = "Campo LIMITE MÁXIMO EMITIDAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CACL_IN_LIMITE_MAXIMO_EMITIDAS { get; set; }
        [Required(ErrorMessage = "Campo LIMITE MÍNIMO APROVADAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CACL_IN_LIMITE_MINIMO_APROVADAS { get; set; }
        [Required(ErrorMessage = "Campo LIMITE MÁXIMO APROVADAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CACL_IN_LIMITE__MAXIMO_APROVADAS { get; set; }
        [Required(ErrorMessage = "Campo LIMITE MÍNIMO REPROVADAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CACL_IN_LIMITE_MINIMO_REPROVADAS { get; set; }
        [Required(ErrorMessage = "Campo LIMITE MÁXIMO REPROVADAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CACL_IN_LIMITE__MAXIMO_REPROVADAS { get; set; }
        public Nullable<int> CACL_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo ORDEM obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CACL_IN_ORDEM { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
    }
}