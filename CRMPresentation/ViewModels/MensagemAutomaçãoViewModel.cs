    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MensagemAutomacaoViewModel
    {
        [Key]
        public int MEAU_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINANTE obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public Nullable<System.DateTime> MEAU_DT_CADASTRO { get; set; }
        [Required(ErrorMessage = "Campo USUÀRIO obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> MEAU_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo GRUPO obrigatorio")]
        public Nullable<int> GRUP_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE MENSAGEM obrigatorio")]
        public Nullable<int> MEAU_IN_TIPO { get; set; }
        public Nullable<int> TEEM_CD_ID { get; set; }
        public Nullable<int> TSMS_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE ENVIO obrigatorio")]
        public Nullable<int> MEAU_IN_TIPO_ENVIO { get; set; }
        [Required(ErrorMessage = "Campo DATA INICIAL obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE INÍCIO deve ser uma data válida")]
        public Nullable<System.DateTime> MEAU_DT_INICIO { get; set; }
        [Required(ErrorMessage = "Campo DATA FINAL obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE TÉRMINO deve ser uma data válida")]
        public Nullable<System.DateTime> MEAU_DT_FINAL { get; set; }
        public Nullable<System.TimeSpan> MEAU_HR_DISPARO { get; set; }
        public Nullable<int> MEAU_IN_DIA_SEMANA { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(250, ErrorMessage = "A DESCRIÇÂO deve conter no máximo 250 caracteres.")]
        public string MEAU_DS_DESCRICAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE DISPARO deve ser uma data válida")]
        public Nullable<System.DateTime> MEAU_DT_DATA_FIXA { get; set; }
        public Nullable<int> PERI_CD_ID { get; set; }
        public Nullable<int> MEAU_IN_ANIVERSARIO { get; set; }
        public string MEAU_ME_DATA_FIXA { get; set; }

        public string Tipo { get; set; }
        public string DiaSemana { get; set; }
        public string DataDisparo { get; set; }
        public string Aniversario { get; set; }
        public string TipoSel { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual GRUPO GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_AUTOMACAO_DATAS> MENSAGEM_AUTOMACAO_DATAS { get; set; }
        public virtual TEMPLATE_EMAIL TEMPLATE_EMAIL { get; set; }
        public virtual TEMPLATE_SMS TEMPLATE_SMS { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PERIODICIDADE PERIODICIDADE { get; set; }
    }
}