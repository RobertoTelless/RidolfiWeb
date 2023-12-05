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
        public Nullable<int> CAPR_CD_ID { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }
        public int RETR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME DA EMPRESA obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME DA EMPRESA deve conter no minimo 1 caracteres e no máximo 100 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "NOME com caracteres inválidos")]
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
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "RAZÃO SOCIAL com caracteres inválidos")]
        public string EMPR_NM_RAZAO { get; set; }
        [StringLength(20, ErrorMessage = "O CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        [Required(ErrorMessage = "Campo CNPJ obrigatorio")]
        public string EMPR_NR_CNPJ { get; set; }
        [StringLength(20, ErrorMessage = "A INSCRIÇÃO MUNICIPAL deve conter no máximo 20 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9]|-|_|\s)+$$", ErrorMessage = "Inscrição Municipal inválido")]
        public string EMPR_NR_INSCRICAO_MUNICIPAL { get; set; }
        [StringLength(20, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 20 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9]|-|_|\s)+$$", ErrorMessage = "Inscrição Municipal inválido")]
        public string EMPR_NR_INSCRICAO_ESTADUAL { get; set; }
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "ENDEREÇO com caracteres inválidos")]
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_ENDERECO { get; set; }
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9]|-|_|\s)+$$", ErrorMessage = "Número inválido")]
        [StringLength(50, ErrorMessage = "O NÚMERO deve conter no máximo 20 caracteres.")]
        public string EMPR_NM_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "O COMPLEMENTO deve conter no máximo 20 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "COMPLEMENTO com caracteres inválidos")]
        public string EMPR_NM_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "BAIROO com caracteres inválidos")]
        public string EMPR_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "CIDADE com caracteres inválidos")]
        public string EMPR_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10 caracteres.")]
        [RegularExpression(@"^([0-9]|-|_|\s)+$$", ErrorMessage = "CEP inválido")]
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
        public Nullable<int> CALC_IN_ESCOPO { get; set; }
        public Nullable<int> CALC_IN_TIPO { get; set; }
        public Nullable<int> CALC_IN_CALCULO { get; set; }
        public Nullable<decimal> CALC_VL_ESTOQUE_MAXIMO { get; set; }
        public Nullable<decimal> CALC_VL_DESCONTO { get; set; }
        public Nullable<decimal> CALC_VL_PRECO { get; set; }
        public Nullable<decimal> CALC_VL_CMV { get; set; }
        public Nullable<decimal> CALC_VL_MARGEM_PERC { get; set; }
        public Nullable<decimal> CALC_VL_MARGEM_REAL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTECIPACAO> ANTECIPACAO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
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
        public virtual REGIME_TRIBUTARIO REGIME_TRIBUTARIO { get; set; }
    }
}