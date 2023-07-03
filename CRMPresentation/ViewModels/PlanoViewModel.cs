using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PlanoViewModel
    {
        [Key]
        public int PLAN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O NOME deve conter no minimo 2 e no máximo 50 caracteres.")]
        public string PLAN_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> PLAN_DT_CRIACAO { get; set; }
        public Nullable<int> PLAN_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 2 e no máximo 500 caracteres.")]
        public string PLAN_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE USUÁRIOS ATIVOS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_USUARIOS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE CONTATOS ATIVOS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_CONTATOS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE E-MAILS/MÊS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_EMAIL { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE SMS/MÊS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_SMS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE SMS/MÊS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_SMS_PRIORITARIO { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE WHATSAPP/MÊS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_WHATSAPP { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE PROCESSOS ATIVOS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_PROCESSOS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE AÇÕES ATIVAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_ACOES { get; set; }
        [Required(ErrorMessage = "Campo PREÇO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PLAN_VL_PRECO { get; set; }
        [Required(ErrorMessage = "Campo PERIODICIDADE obrigatorio")]
        public Nullable<int> PLPE_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public Nullable<System.DateTime> PLAN_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo PREÇO PROMOÇÃO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PLAN_VL_PROMOCAO { get; set; }
        public string PLAN_TX_SITE { get; set; }
        [Required(ErrorMessage = "Campo MENSAGENS obrigatorio")]
        public Nullable<int> PLAN_IN_MENSAGENS { get; set; }
        [Required(ErrorMessage = "Campo CRM obrigatorio")]
        public Nullable<int> PLAN_IN_CRM { get; set; }
        public Nullable<int> PLAN_IN_FINANCEIRO { get; set; }
        public Nullable<int> PLAN_IN_FATURA { get; set; }
        public Nullable<int> PLAN_IN_ESTOQUE { get; set; }
        public Nullable<int> PLAN_IN_PATRIMONIO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_PATRIMONIO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_PRODUTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_FORNECEDOR { get; set; }
        public Nullable<int> PLAN_IN_VENDAS { get; set; }
        public Nullable<int> PLAN_IN_COMPRA { get; set; }
        public Nullable<int> PLAN_IN_SERVICOS { get; set; }
        public Nullable<int> PLAN_IN_ATENDIMENTOS { get; set; }
        public Nullable<int> PLAN_IN_OS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_COMPRA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_VENDA { get; set; }
        public Nullable<int> PLAN_NR_SERVICE { get; set; }
        [Required(ErrorMessage = "Campo DURAÇÃO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_IN_DURACAO { get; set; }
        [Required(ErrorMessage = "Campo NOME DE EXIBIÇÃO obrigatorio")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "O NOME DE EXIBIÇÃO deve conter no minimo 2 e no máximo 10 caracteres.")]
        public string PLAN_NM_EXIBE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_IN_PESQUISAS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_PESQUISAS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_ATENDIMENTOS { get; set; }

        public string ValorNormal
        {
            get
            {
                return PLAN_VL_PRECO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PLAN_VL_PRECO.Value) : "0,0";
            }
            set
            {
                PLAN_VL_PRECO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public string ValorPromocao
        {
            get
            {
                return PLAN_VL_PROMOCAO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PLAN_VL_PROMOCAO.Value) : "0,0";
            }
            set
            {
                PLAN_VL_PROMOCAO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public bool Mensagem
        {
            get
            {
                if (PLAN_IN_MENSAGENS == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_MENSAGENS = (value == true) ? 1 : 0;
            }
        }
        public bool CRM
        {
            get
            {
                if (PLAN_IN_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool Financeiro
        {
            get
            {
                if (PLAN_IN_FINANCEIRO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_FINANCEIRO = (value == true) ? 1 : 0;
            }
        }
        public bool Patrimonio
        {
            get
            {
                if (PLAN_IN_PATRIMONIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_PATRIMONIO = (value == true) ? 1 : 0;
            }
        }
        public bool Compra
        {
            get
            {
                if (PLAN_IN_COMPRA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_COMPRA = (value == true) ? 1 : 0;
            }
        }
        public bool Estoque
        {
            get
            {
                if (PLAN_IN_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool Vendas
        {
            get
            {
                if (PLAN_IN_VENDAS == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_VENDAS = (value == true) ? 1 : 0;
            }
        }
        public bool Atendimentos
        {
            get
            {
                if (PLAN_IN_ATENDIMENTOS == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_ATENDIMENTOS = (value == true) ? 1 : 0;
            }
        }

        public bool Pesquisas
        {
            get
            {
                if (PLAN_IN_PESQUISAS == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PLAN_IN_PESQUISAS = (value == true) ? 1 : 0;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE> ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_PAGAMENTO> ASSINANTE_PAGAMENTO { get; set; }
        public virtual PLANO_PERIODICIDADE PLANO_PERIODICIDADE { get; set; }

    }
}