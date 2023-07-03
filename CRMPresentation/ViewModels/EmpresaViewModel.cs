using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EmpresaViewModel
    {
        [Key]
        public int EMPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int RETR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME DA EMPRESA obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME DA EMPRESA deve conter no minimo 1 caracteres e no máximo 100 caracteres.")]
        public string EMPR_NM_NOME { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_PATRIMONIO_LIQUIDO { get; set; }
        public string PatrimonioText
        {
            get
            {
                return EMPR_VL_PATRIMONIO_LIQUIDO.HasValue ? CrossCutting.Formatters.DecimalFormatter(EMPR_VL_PATRIMONIO_LIQUIDO.Value) : string.Empty;
            }
            set
            {
                EMPR_VL_PATRIMONIO_LIQUIDO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CADASTRO Deve ser uma data válida")]
        public System.DateTime EMPR_DT_CADASTRO { get; set; }
        public int EMPR_IN_ATIVO { get; set; }
        public Nullable<int> MAQN_CD_ID { get; set; }
        public int EMPR_IN_OPERA_CARTAO { get; set; }
        public string EMPR_NM_OUTRA_MAQUINA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_ANTECIPACAO { get; set; }
        public int EMPR_IN_PAGA_COMISSAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_IMPOSTO_MEI { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_VENDA_DEBITO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_VENDA_CREDITO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_VENDA_DINHEIRO { get; set; }
        [StringLength(100, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 100 caracteres.")]
        public string EMPR_NM_RAZAO { get; set; }
        [StringLength(20, ErrorMessage = "O CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        [Required(ErrorMessage = "Campo CNPJ obrigatorio")]
        public string EMPR_NR_CNPJ { get; set; }
        [StringLength(20, ErrorMessage = "A INSCRIÇÃO MUNICIPAL deve conter no máximo 20 caracteres.")]
        public string EMPR_NR_INSCRICAO_MUNICIPAL { get; set; }
        [StringLength(20, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 20 caracteres.")]
        public string EMPR_NR_INSCRICAO_ESTADUAL { get; set; }
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_ENDERECO { get; set; }
        [StringLength(20, ErrorMessage = "O NÚMERO deve conter no máximo 20 caracteres.")]
        public string EMPR_NM_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "O COMPLEMENTO deve conter no máximo 20 caracteres.")]
        public string EMPR_NM_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10 caracteres.")]
        public string EMPR_NR_CEP { get; set; }
        public Nullable<int> PLEN_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_TAXA_MEDIA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_COMISSAO_VENDEDOR { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_COMISSAO_OUTROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_TAXA_MEDIA_DEBITO { get; set; }
        public Nullable<decimal> EMPR_PC_CUSTO_VARIAVEL_VENDA { get; set; }
        public Nullable<decimal> EMPR_PC_CUSTO_VARIAVEL_TOTAL { get; set; }
        public Nullable<decimal> EMPR_PC_CUSTO_ANTECIPACOES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_IMPOSTO_OUTROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_COMISSAO_GERENTE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_ROYALTIES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_FUNDO_PROPAGANDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_FUNDO_SEGURANCA { get; set; }
        public Nullable<int> EMPR_IN_CALCULADO { get; set; }
        public string EMPR_AQ_LOGO { get; set; }
        public Nullable<int> EMPR_IN_MATRIZ { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO GERENTE deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_GERENTE { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50 caracteres.")]
        public string EMPR_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve conter no máximo 20 caracteres.")]
        public string EMPR_NR_CELULAR { get; set; }
        [StringLength(100, ErrorMessage = "O E-MAIL deve conter no máximo 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string EMPR_NM_EMAIL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTECIPACAO> ANTECIPACAO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO> ATENDIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO_PROPOSTA> ATENDIMENTO_PROPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUSTO_FIXO> CUSTO_FIXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_ANEXO> EMPRESA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_CUSTO_VARIAVEL> EMPRESA_CUSTO_VARIAVEL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_CUSTO_VARIAVEL_NORMAL> EMPRESA_CUSTO_VARIAVEL_NORMAL { get; set; }
        public virtual MAQUINA MAQUINA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_MAQUINA> EMPRESA_MAQUINA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_PLATAFORMA> EMPRESA_PLATAFORMA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_TICKET> EMPRESA_TICKET { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO> GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<META> META { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA> PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VENDA_MENSAL> VENDA_MENSAL { get; set; }
        public virtual REGIME_TRIBUTARIO REGIME_TRIBUTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_EMAIL> TEMPLATE_EMAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_PROPOSTA> TEMPLATE_PROPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_SMS> TEMPLATE_SMS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ACAO> CRM_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_FOLLOW> CRM_FOLLOW { get; set; }
    }
}