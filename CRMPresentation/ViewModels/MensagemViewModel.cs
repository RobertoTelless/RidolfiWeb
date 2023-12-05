    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MensagemViewModel
    {
        [Key]
        public int MENS_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> TEMP_CD_ID { get; set; }
        public Nullable<int> MENS_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public Nullable<System.DateTime> MENS_DT_CRIACAO { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DA CAMPANHA deve conter no máximo 50 caracteres.")]
        public string MENS_NM_CAMPANHA { get; set; }
        public string MENS_TX_TEXTO { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE MENSAGEM obrigatorio")]
        public Nullable<int> MENS_IN_TIPO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE AGENDAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> MENS_DT_AGENDAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENVIO deve ser uma data válida")]
        public Nullable<System.DateTime> MENS_DT_ENVIO { get; set; }
        public string MENS_TX_RETORNO { get; set; }
        public string MENS_NM_CABECALHO { get; set; }
        public string MENS_NM_RODAPE { get; set; }
        [StringLength(500, ErrorMessage = "O LINK deve conter no máximo 500 caracteres.")]
        [RegularExpression(@"^((http|ftp|https|www)://)?([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$", ErrorMessage = "Link inválido")]
        public string MENS_NM_LINK { get; set; }
        [StringLength(250, ErrorMessage = "O TEXTO DO SMS deve conter no máximo 250 caracteres.")]
        public string MENS_TX_SMS { get; set; }
        public Nullable<int> TSMS_CD_ID { get; set; }
        public Nullable<int> TEEM_CD_ID { get; set; }
        public Nullable<int> TEPR_CD_ID { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DA MENSAGEM deve conter no máximo 250 caracteres.")]
        public string MENS_NM_NOME { get; set; }
        public Nullable<int> MENS_IN_AGENDAMENTO { get; set; }
        public Nullable<int> MENS_IN_REPETICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MENS_NR_REPETICOES { get; set; }
        public Nullable<int> PETA_CD_ID { get; set; }
        public Nullable<int> MENS_IN_PERIODICIDADE { get; set; }
        public Nullable<int> MENS_IN_TIPO_SMS { get; set; }
        public Nullable<int> MENS_IN_TIPO_EMAIL { get; set; }
        public string MENS_AQ_ARQUIVO { get; set; }
        public Nullable<int> MENS_IN_STATUS { get; set; }
        public string MENS_NM_ASSINATURA { get; set; }

        public Int32? SEXO { get; set; }
        public string NOME { get; set; }
        public Int32? ID { get; set; }
        public string CIDADE { get; set; }
        public Nullable<System.DateTime> DATA_NASC{ get; set; }
        public Int32? UF { get; set; }
        public Int32? CATEGORIA { get; set; }
        public Int32? STATUS { get; set; }
        public String LINK { get; set; }
        public ExcecaoViewModel EXCECAO { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> FORN_CD_ID { get; set; }
        public Nullable<int> MENS_IN_USUARIO { get; set; }

        public Nullable<int> GRUP_CD_ID { get; set; }
        public String MODELO { get; set; }
        public string MENS_TX_TEXTO_LIMPO { get; set; }
        public string MENS_GU_GUID { get; set; }

        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMAIL_AGENDAMENTO> EMAIL_AGENDAMENTO { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual GRUPO GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_ANEXO> MENSAGEM_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_DESTINOS> MENSAGENS_DESTINOS { get; set; }
        public virtual PERIODICIDADE_TAREFA PERIODICIDADE_TAREFA { get; set; }
        public virtual PESQUISA PESQUISA { get; set; }
        public virtual TEMPLATE TEMPLATE { get; set; }
        public virtual TEMPLATE_EMAIL TEMPLATE_EMAIL { get; set; }
        public virtual TEMPLATE_SMS TEMPLATE_SMS { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECURSIVIDADE> RECURSIVIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESULTADO_ROBOT> RESULTADO_ROBOT { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}