using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AssinanteViewModel
    {
        [Key]
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        public Nullable<int> TIPE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O NOME deve conter no minimo 3 e no máximo 50 caracteres.")]
        public string ASSI_NM_NOME { get; set; }
        public int ASSI_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ASSI_DT_INICIO { get; set; }
        public Nullable<int> ASSI_IN_TIPO { get; set; }
        public Nullable<int> ASSI_IN_STATUS { get; set; }
        [Required(ErrorMessage = "Campo E-Mail obrigatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O E-MAil deve conter no minimo 3 e no máximo 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string ASSI_NM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 50 caracteres.")]
        public string ASSI_NM_RAZAO_SOCIAL { get; set; }
        [StringLength(20, ErrorMessage = "O CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string ASSI_NR_CNPJ { get; set; }
        [StringLength(20, ErrorMessage = "O CPF deve conter no máximo 20 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string ASSI_NR_CPF { get; set; }
        [StringLength(5000, ErrorMessage = "AS OBSERVAÇÔES deve conter no máximo 5000 caracteres.")]
        public string ASSI_TX_OBSERVACOES { get; set; }
        [Required(ErrorMessage = "Campo ENDEREÇO obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O ENDEREÇO deve conter no minimo 3 e no máximo 50 caracteres.")]
        public string ASSI_NM_ENDERECO { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NÚMERO deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string ASSI_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo BAIRRO obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O BAIRRO deve conter no minimo 1 e no máximo 20 caracteres.")]
        public string ASSI_NM_BAIRRO { get; set; }
        [Required(ErrorMessage = "Campo CIDADE obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "A CIDADE deve conter no minimo 3 e no máximo 50 caracteres.")]
        public string ASSI_NM_CIDADE { get; set; }
        [Required(ErrorMessage = "Campo UF obrigatorio")]
        public Nullable<int> UF_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CEP obrigatorio")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "O CEP deve conter no minimo 3 e no máximo 10 caracteres.")]
        public string ASSI_NR_CEP { get; set; }
        public string ASSI_AQ_FOTO { get; set; }
        public Nullable<int> PLAN_CD_ID { get; set; }
        [StringLength(20, MinimumLength = 13, ErrorMessage = "O TELEFONE deve conter no minimo 13 e no máximo 20 caracteres.")]
        public string ASSI_NR_TELEFONE { get; set; }
        [StringLength(20, MinimumLength = 14, ErrorMessage = "O CELULAR deve conter no minimo 14 e no máximo 20 caracteres.")]
        public string ASSI_NR_CELULAR { get; set; }
        public Nullable<int> ASSI_IN_BLOQUEADO { get; set; }
        [StringLength(50, ErrorMessage = "O COMPLEMENTO deve conter no máximo 50 caracteres.")]
        public string ASSI_NM_COMPLEMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_ANEXO> ASSINANTE_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_ANOTACAO> ASSINANTE_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_CONSUMO> ASSINANTE_CONSUMO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_PAGAMENTO> ASSINANTE_PAGAMENTO { get; set; }
        public virtual PLANO PLANO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_PLANO> ASSINANTE_PLANO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_QUADRO_SOCIETARIO> ASSINANTE_QUADRO_SOCIETARIO { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CARGO> CARGO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_AGENDA> CATEGORIA_AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_CLIENTE> CATEGORIA_CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_TELEFONE> CATEGORIA_TELEFONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_FALHA> CLIENTE_FALHA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONFIGURACAO> CONFIGURACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ACAO> CRM_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ORIGEM> CRM_ORIGEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DEPARTAMENTO> DEPARTAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMAIL_AGENDAMENTO> EMAIL_AGENDAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNIL> FUNIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO> GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOG> LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_AUTOMACAO> MENSAGEM_AUTOMACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOTIVO_CANCELAMENTO> MOTIVO_CANCELAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOTIVO_ENCERRAMENTO> MOTIVO_ENCERRAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA> NOTICIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO> NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERIODICIDADE_TAREFA> PERIODICIDADE_TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TELEFONE> TELEFONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE> TEMPLATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_EMAIL> TEMPLATE_EMAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_PROPOSTA> TEMPLATE_PROPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_SMS> TEMPLATE_SMS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_ACAO> TIPO_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_CRM> TIPO_CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_GRUPO> TIPO_GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_TAREFA> TIPO_TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VIDEO> VIDEO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VOLTA_PESQUISA> VOLTA_PESQUISA { get; set; }
    }
}