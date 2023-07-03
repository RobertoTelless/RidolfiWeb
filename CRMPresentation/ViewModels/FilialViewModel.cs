using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FilialViewModel
    {
        [Key]
        public int FILI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string FILI_NM_NOME { get; set; }
        [StringLength(100, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 100.")]
        public string FILI_NM_RAZAO { get; set; }
        [StringLength(20, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter no minimo 14 caracteres e no máximo 20.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string FILI_NR_CNPJ { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string FILI_NM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "O CONTATO deve conter no máximo 50.")]
        public string FILI_NM_CONTATOS { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50.")]
        public string FILI_NM_TELEFONES { get; set; }
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve conter no máximo 50.")]
        public string FILI_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50.")]
        public string FILI_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50.")]
        public string FILI_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10.")]
        public string FILI_NR_CEP { get; set; }
        public System.DateTime FILI_DT_CADASTRO { get; set; }
        public int FILI_IN_ATIVO { get; set; }
        public Nullable<int> TIPE_CD_ID { get; set; }
        public Nullable<int> CRTR_CD_ID { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 50.")]
        public string FILI_NR_INSCRICAO_ESTADUAL { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO MUNICIAPAL deve conter no máximo 50.")]
        public string FILI_NR_INSCRICAO_MUNICIPAL { get; set; }
        public Nullable<int> FILI_IN_IE_ISENTO { get; set; }
        [StringLength(50, ErrorMessage = "O CNAE deve conter no máximo 50.")]
        public string FILI_NR_CNAE { get; set; }
        [StringLength(100, ErrorMessage = "O WEBSITE deve conter no máximo 100.")]
        public string FILI_NM_WEBSITE { get; set; }
        [StringLength(50, ErrorMessage = "O CELURAR deve conter no máximo 50.")]
        public string FILI_NR_CELULAR { get; set; }
        [StringLength(250, ErrorMessage = "O LOGOTIPO deve conter no máximo 250.")]
        public string FILI_AQ_LOGOTIPO { get; set; }
        [StringLength(20, ErrorMessage = "O CPF deve conter no máximo 20.")]
        public string FILI_NR_CPF { get; set; }
        [StringLength(20, ErrorMessage = "O CNPJ deve conter no máximo 20.")]
        public string FILI_NR_RG { get; set; }
        public Nullable<int> FILI_IN_MATRIZ { get; set; }


        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }

    }
}