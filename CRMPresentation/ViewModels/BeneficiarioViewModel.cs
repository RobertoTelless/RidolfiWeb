using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class BeneficiarioViewModel
    {
        [Key]
        public int BENE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        public int TIPE_CD_ID { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> ESCI_CD_ID { get; set; }
        public Nullable<int> ESCO_CD_ID { get; set; }
        public Nullable<int> PARE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 250 caracteres.")]
        public string BENE_NM_NOME { get; set; }
        [StringLength(250, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 250 caracteres.")]
        public string MOME_NM_RAZAO_SOCIAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE NASCIMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> BENE_DT_NASCIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> BENE_VL_RENDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> BENE_VL_RENDA_ESTIMADA { get; set; }
        public int BENE_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> BENE_DT_CADASTRO { get; set; }
        [StringLength(20, ErrorMessage = "O CPF deve conter no máximo 20 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string BENE_NR_CPF { get; set; }
        [StringLength(20, ErrorMessage = "O RG deve conter no máximo 20 caracteres.")]
        public string BENE_NR_RG { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public string BENE_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public string BENE_NR_TELEFONE2 { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public string BENE_NR_TELEFONE3 { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public string BENE_NR_TELEFONE4 { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve conter no máximo 20 caracteres.")]
        public string BENE_NR_CELULAR { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve conter no máximo 20 caracteres.")]
        public string BENE_NR_CELULAR2 { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve conter no máximo 20 caracteres.")]
        public string BENE_NR_CELULAR3 { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve conter no máximo 20 caracteres.")]
        public string BENE_NR_CELULAR4 { get; set; }
        [StringLength(150, ErrorMessage = "O E-MAIL deve conter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string BENE_EM_EMAIL { get; set; }
        [StringLength(20, ErrorMessage = "O CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string BENE_NR_CNPJ { get; set; }
        [StringLength(100, ErrorMessage = "O NOME DO PARENTE deve conter no máximo 100 caracteres.")]
        public string BENE_NM_PARENTESCO { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string BENE_NM_CIDADE { get; set; }
        [StringLength(100, ErrorMessage = "O ENDEREÇO deve conter no máximo 100 caracteres.")]
        public string BENE_NM_ENDERECO { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10 caracteres.")]
        public string BENE_NR_CEP { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string BENE_NM_BAIRRO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BENEFICIARIO_ANEXO> BENEFICIARIO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BENEFICIARIO_ANOTACOES> BENEFICIARIO_ANOTACOES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTATO> CONTATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ENDERECO> ENDERECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_ENVIADAS_SISTEMA> MENSAGENS_ENVIADAS_SISTEMA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TELEFONE> TELEFONE { get; set; }
        public virtual ESTADO_CIVIL ESTADO_CIVIL { get; set; }
        public virtual PARENTESCO PARENTESCO { get; set; }
        public virtual SEXO SEXO { get; set; }
        public virtual ESCOLARIDADE ESCOLARIDADE { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECATORIO> PRECATORIO { get; set; }
        public virtual UF UF { get; set; }
    }
}