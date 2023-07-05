using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PrecatorioViewModel
    {
        [Key]
        public int PREC_CD_ID { get; set; }
        [Display(Name = "Tribunal")]
        public int TRF1_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo BENEFICIÁRIO obrigatorio")]
        [Display(Name = "Beneficiário")]
        public Nullable<int> BENE_CD_ID { get; set; }
        [Display(Name = "Advogado")]
        public Nullable<int> HONO_CD_ID { get; set; }
        [Display(Name = "Banco")]
        public Nullable<int> BANC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NATUREZA obrigatorio")]
        [Display(Name = "Natureza")]
        public Nullable<int> NATU_CD_ID { get; set; }
        [Display(Name = "Estado do Precatório")]
        public Nullable<int> PRES_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRECATÓRIO obrigatorio")]
        [StringLength(50, ErrorMessage = "O PRECATÓRIO deve conter no máximo 50 caracteres.")]
        [Display(Name = "Precatório")]
        public string PREC_NM_PRECATORIO { get; set; }
        [Required(ErrorMessage = "Campo ANO obrigatorio")]
        [StringLength(4, ErrorMessage = "O ANO deve conter no máximo 4 caracteres.")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Ano")]
        public string PREC_NR_ANO { get; set; }
        [Required(ErrorMessage = "Campo PROCESSO DE ORIGEM obrigatorio")]
        [StringLength(20, ErrorMessage = "O PROCESSO DE ORIGEM deve conter no máximo 20 caracteres.")]
        [Display(Name = "Processo de Origem")]
        public string PREC_NM_PROCESSO_ORIGEM { get; set; }
        [Required(ErrorMessage = "Campo REQUERENTE obrigatorio")]
        [StringLength(50, ErrorMessage = "O REQUERENTE deve conter no máximo 50 caracteres.")]
        [Display(Name = "Requerente")]
        public string PREC_NM_REQUERENTE { get; set; }
        public string PREC_NM_ADVOGADO { get; set; }
        [Required(ErrorMessage = "Campo DEPRECANTE obrigatorio")]
        [StringLength(50, ErrorMessage = "O DEPRECANTE deve conter no máximo 50 caracteres.")]
        [Display(Name = "Deprecante")]
        public string PREC_NM_DEPRECANTE { get; set; }
        [StringLength(500, ErrorMessage = "O ASSUNTO deve conter no máximo 500 caracteres.")]
        [Display(Name = "Assunto")]
        public string PREC_NM_ASSUNTO { get; set; }
        [Required(ErrorMessage = "Campo REQUERIDO obrigatorio")]
        [StringLength(50, ErrorMessage = "O REQUERIDO deve conter no máximo 50 caracteres.")]
        [Display(Name = "Requerido")]
        public string PREC_NM_REQUERIDO { get; set; }
        [Display(Name = "Procedimento")]
        public string PREC_SG_PROCEDIMENTO { get; set; }
        [StringLength(50, ErrorMessage = "A SITUAÇÂO DA REQUISIÇÂO deve conter no máximo 50 caracteres.")]
        [Display(Name = "Situação Requisição")]
        public string PREC_NM_SITUACAO_REQUISICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Valor Inscrito Proposta (R$)")]
        public Nullable<decimal> PREC_VL_VALOR_INSCRITO_PROPOSTA { get; set; }
        [StringLength(50, ErrorMessage = "O PROCESSO EXECUÇÃO deve conter no máximo 50 caracteres.")]
        [Display(Name = "Processo de Execução")]
        public string PREC_NM_PROC_EXECUCAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CÁLCULO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Display(Name = "Data de Cálculo")]
        public Nullable<System.DateTime> PREC_DT_BEN_DATABASE { get; set; }
        [Required(ErrorMessage = "Campo VALOR RRINCIPAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Valor Principal (R$)")]
        public Nullable<decimal> PREC_VL_BEN_VALOR_PRINCIPAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Juros (R$)")]
        public Nullable<decimal> PREC_VL_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Valor Inicial PSS (R$)")]
        public Nullable<decimal> PREC_VL_VALOR_INICIAL_PSS { get; set; }
        [Required(ErrorMessage = "Campo VALOR REQUISITADO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Valor Requsitado (R$)")]
        public Nullable<decimal> PREC_VL_BEN_VALOR_REQUISITADO { get; set; }
        [Display(Name = "IR RRA)")]
        public string PREC_SG_BEN_IR_RRA { get; set; }
        public Nullable<int> PREC_BEN_MESES_EXE_ANTERIOR { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PREC_DT_HON_DATABASE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PREC_VL_HON_VALOR_PRINCIPAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PREC_VL_HON_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PREC_VL_HON_VALOR_INICIAL_PSS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PREC_VL_HON_VALOR_REQUISITADO { get; set; }
        public string PREC_SG_HON_IR_RRA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PREC_IN_HON_MESES_EXE_ANTERIOR { get; set; }
        [Required(ErrorMessage = "Campo FOI PESQUISADO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Pesquisado?")]
        public Nullable<int> PREC_IN_FOI_PESQUISADO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE INSERÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Display(Name = "Data de Inserção")]
        public Nullable<System.DateTime> PREC_DT_INSERT_BD { get; set; }
        [Required(ErrorMessage = "Campo IMPORTADO CRM obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Display(Name = "Importado CRM?")]
        public Nullable<int> PREC_IN_FOI_IMPORTADO_PIPE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Display(Name = "Data Protocolo TRF")]
        public Nullable<System.DateTime> PREC_DT_PROTOCOLO_TRF { get; set; }
        [StringLength(50, ErrorMessage = "O OFÍCIO REQUISITÓRIO deve conter no máximo 50 caracteres.")]
        [Display(Name = "Ofício Requisitório")]
        public string PREC_NM_OFICIO_REQUISITORIO { get; set; }
        [StringLength(50, ErrorMessage = "A REQUISIÇÂO BLOQUEADA deve conter no máximo 50 caracteres.")]
        [Display(Name = "Requisição Bloqueada)")]
        public string PREC_NM_REQUISICAO_BLOQUEADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Display(Name = "Data Expedição")]
        public Nullable<System.DateTime> PREC_DT_EXPEDICAO { get; set; }
        [Required(ErrorMessage = "Campo PRC LOA obrigatorio")]
        [StringLength(50, ErrorMessage = "O PRC LOA deve conter no máximo 50 caracteres.")]
        [Display(Name = "PRC LOA")]
        public string PREC_NM_PRC_LOA { get; set; }
        [StringLength(5000, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 5000 caracteres.")]
        [Display(Name = "Observações")]
        public string PREC_TX_OBSERVACAO { get; set; }
        public Nullable<int> PREC_IN_ATIVO { get; set; }
        [Display(Name = "Situação Atual")]
        public Nullable<int> PREC_IN_SITUACAO_ATUAL { get; set; }
        public Nullable<System.DateTime> PREC_DT_CADASTRO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PREC_VL_RRA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PREC_PC_RRA { get; set; }

        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public string ANO_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VALOR_INSC_PROP_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<System.DateTime> BEN_DATA_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VALOR__PRINCIPAL_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> JUROS_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VALOR_PSS_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VALOR_REQ_FINAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> DATA_EXP_FINAL { get; set; }
        [Required(ErrorMessage = "Campo RESPONSÁVEL obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }
        public string PREC_NM_NOME { get; set; }
        [StringLength(500, ErrorMessage = "PREFERÊNCIA deve conter no máximo 500 caracteres.")]
        public string PREC_NM_PREFERENCIA { get; set; }
        [StringLength(4, ErrorMessage = "O ANO DA PROPOSTA deve conter no máximo 4 caracteres.")]
        public string PREC_NR_ANO_PROPOSTA { get; set; }
        [StringLength(150, ErrorMessage = "O TIPO DE DESPESA deve conter no máximo 150 caracteres.")]
        public string PREC_NM_TIPO_DESPESA { get; set; }

        // Filtros
        public Nullable<int> OperaTRF { get; set; }
        public Nullable<int> OperaBene { get; set; }
        public Nullable<int> OperaAdv { get; set; }
        public Nullable<int> OperaNat { get; set; }
        public Nullable<int> OperaPres { get; set; }
        public Nullable<int> OperaPrec { get; set; }
        public Nullable<int> OperaAno { get; set; }
        public Nullable<int> OperaProcOrig { get; set; }
        public Nullable<int> OperaReq { get; set; }
        public Nullable<int> OperaDepr { get; set; }
        public Nullable<int> OperaAss { get; set; }
        public Nullable<int> OperaReqd { get; set; }
        public Nullable<int> OperaSgProc { get; set; }
        public Nullable<int> OperaSitReq { get; set; }
        public Nullable<int> OperaValorInsProp { get; set; }
        public Nullable<int> OperaProcExec { get; set; }
        public Nullable<int> OperaBenData { get; set; }
        public Nullable<int> OperaValorPrin { get; set; }
        public Nullable<int> OperaJuros { get; set; }
        public Nullable<int> OperaValorIniPSS { get; set; }
        public Nullable<int> OperaBenValorReq { get; set; }
        public Nullable<int> OperaPesq { get; set; }
        public Nullable<int> OperaSitAtual { get; set; }
        public Nullable<int> OperaImpCRM { get; set; }
        public Nullable<int> OperaDataProtTRF { get; set; }
        public Nullable<int> OperaOfiReq { get; set; }
        public Nullable<int> OperaReqBloq { get; set; }
        public Nullable<int> OperaDataExp { get; set; }
        public Nullable<int> OperaNomLOA { get; set; }

        // Totais
        public Nullable<decimal> TOTAL_VALOR_INSC_PROP { get; set; }
        public Nullable<decimal> TOTAL_VALOR_PRINCIPAL { get; set; }
        public Nullable<decimal> TOTAL_JUROS { get; set; }
        public Nullable<decimal> TOTAL_VALOR_PSS { get; set; }
        public Nullable<decimal> TOTAL_VALOR_REQ { get; set; }

        // Contagem
        public Nullable<int> ContagemFiltro { get; set; }

        public virtual BANCO BANCO { get; set; }
        public virtual BENEFICIARIO BENEFICIARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTATO> CONTATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        public virtual HONORARIO HONORARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOAS> LOAS { get; set; }
        public virtual NATUREZA NATUREZA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECATORIO_ANEXO> PRECATORIO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECATORIO_ANOTACAO> PRECATORIO_ANOTACAO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PRECATORIO_ESTADO PRECATORIO_ESTADO { get; set; }
        public virtual TRF TRF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }

    }
}