using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CRMViewModel
    {
        [Key]
        public int CRM1_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> FUNI_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public Nullable<int> PREC_CD_ID { get; set; }
        public Nullable<int> TICR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRM1_DT_CRIACAO { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string CRM1_DS_DESCRICAO { get; set; }
        [StringLength(5000, ErrorMessage = "AS INFORMAÇÕES GERAIS devem conter no máximo 5000 caracteres.")]
        public string CRM1_TX_INFORMACOES_GERAIS { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int CRM1_IN_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CANCELAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRM1_DT_CANCELAMENTO { get; set; }
        [StringLength(500, ErrorMessage = "O MOTIVO DE CANCELAMENTO deve conter no máximo 500 caracteres.")]
        public string CRM1_DS_MOTIVO_CANCELAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENCERRAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRM1_DT_ENCERRAMENTO { get; set; }
        [StringLength(5000, ErrorMessage = "AS INFORMAÇÕES DE ENCERRAMENTO devem conter no máximo 5000 caracteres.")]
        public string CRM1_DS_INFORMACOES_ENCERRAMENTO { get; set; }
        public Nullable<int> CRM1_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "O NOME deve conter no minimo 2 e no máximo 150 caracteres.")]
        public string CRM1_NM_NOME { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> MENS_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ORIGEM obrigatorio")]
        public Nullable<int> ORIG_CD_ID { get; set; }
        public Nullable<int> MOCA_CD_ID { get; set; }
        public Nullable<int> MOEN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FAVORITO obrigatorio")]
        public Nullable<int> CRM1_IN_ESTRELA { get; set; }
        public Nullable<int> PEVE_CD_ID1 { get; set; }
        public Nullable<int> PEVE_CD_ID2 { get; set; }
        public Nullable<int> CRM1_IN_DUMMY { get; set; }
        public string CRM1_AQ_IMAGEM { get; set; }
        public Nullable<int> CRM1_NR_TEMPERATURA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRM1_VL_VALOR_INICIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRM1_VL_VALOR_FINAL { get; set; }
        public Nullable<int> CRM1_NR_ATRASO { get; set; }
        public Nullable<int> TRAN_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE PREVISÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRM1_DT_PREVISAO_ENTREGA { get; set; }
        public Nullable<int> CRM1_IN_AVISO_ENTREGA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE SAÍDA deve ser uma data válida")]
        public Nullable<System.DateTime> CRM1_DT_DATA_SAIDA { get; set; }
        public Nullable<int> CRM1_IN_ENTREGA_CONFIRMADA { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE SAÍDA devem conter no máximo 1000 caracteres.")]
        public string CRM_DS_INFORMACOES_SAIDA { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        [StringLength(500, ErrorMessage = "A CAMPANHA deve conter no máximo 50 caracteres.")]
        public string CRM1_NM_CAMPANHA { get; set; }

        public string NumeroProposta { get; set; }
        public string NomeProposta { get; set; }
        public Nullable<System.DateTime> DataProposta { get; set; }
        public Nullable<System.DateTime> DataAprovacao { get; set; }
        public Nullable<decimal> ValorTotal { get; set; }

        public string NumeroPedido{ get; set; }
        public string NomePedido{ get; set; }
        public Nullable<System.DateTime> DataPedido { get; set; }
        public Nullable<System.DateTime> DataAprovacaoPedido { get; set; }
        public Nullable<decimal> ValorTotalpedido { get; set; }

        public bool Entrega
        {
            get
            {
                if (CRM1_IN_ENTREGA_CONFIRMADA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CRM1_IN_ENTREGA_CONFIRMADA = (value == true) ? 1 : 0;
            }
        }
        public string ValorInicial
        {
            get
            {
                return CRM1_VL_VALOR_INICIAL.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRM1_VL_VALOR_INICIAL.Value) : "0,0";
            }
            set
            {
                CRM1_VL_VALOR_INICIAL = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string ValorFinal
        {
            get
            {
                return CRM1_VL_VALOR_FINAL.HasValue ? CrossCutting.Formatters.DecimalFormatter(CRM1_VL_VALOR_FINAL.Value) : "0,0";
            }
            set
            {
                CRM1_VL_VALOR_FINAL = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        //public virtual ASSINANTE ASSINANTE { get; set; }
        //public virtual CLIENTE CLIENTE { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CRM_ACAO> CRM_ACAO { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CRM_ANEXO> CRM_ANEXO { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CRM_COMENTARIO> CRM_COMENTARIO { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CRM_CONTATO> CRM_CONTATO { get; set; }
        //public virtual CRM_ORIGEM CRM_ORIGEM { get; set; }
        //public virtual MENSAGENS MENSAGENS { get; set; }
        //public virtual MOTIVO_CANCELAMENTO MOTIVO_CANCELAMENTO { get; set; }
        //public virtual MOTIVO_ENCERRAMENTO MOTIVO_ENCERRAMENTO { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        //public virtual TIPO_CRM TIPO_CRM { get; set; }
        //public virtual USUARIO USUARIO { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<AGENDA> AGENDA { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }
        //public virtual FUNIL FUNIL { get; set; }
        //public virtual EMPRESA EMPRESA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ACAO> CRM_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ANEXO> CRM_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMENTARIO> CRM_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_CONTATO> CRM_CONTATO { get; set; }
        public virtual CRM_ORIGEM CRM_ORIGEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_FOLLOW> CRM_FOLLOW { get; set; }
        public virtual FUNIL FUNIL { get; set; }
        public virtual MENSAGENS MENSAGENS { get; set; }
        public virtual MOTIVO_CANCELAMENTO MOTIVO_CANCELAMENTO { get; set; }
        public virtual MOTIVO_ENCERRAMENTO MOTIVO_ENCERRAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        public virtual PRECATORIO PRECATORIO { get; set; }
        public virtual TIPO_CRM TIPO_CRM { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_ENVIADAS_SISTEMA> MENSAGENS_ENVIADAS_SISTEMA { get; set; }

    }
}