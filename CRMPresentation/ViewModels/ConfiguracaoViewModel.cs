using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ConfiguracaoViewModel
    {
        [Key]
        public int CONF_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FALHAS/DIA obrigatorio")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public int CONF_NR_FALHAS_DIA { get; set; }
        [Required(ErrorMessage = "Campo HOST SMTP obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O HOST SMTP deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CONF_NM_HOST_SMTP { get; set; }
        [Required(ErrorMessage = "Campo PORTA SMTP obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A PORTA SMTP deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CONF_NM_PORTA_SMTP { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL EMISSOR obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL EMISSOR deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string CONF_NM_EMAIL_EMISSOO { get; set; }
        [Required(ErrorMessage = "Campo CREDENCIAIS obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A CREDENCIAL deve conter no minimo 1 caracteres e no máximo 50.")]
        public string CONF_NM_SENHA_EMISSOR { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_REFRESH_DASH { get; set; }
        [StringLength(50, ErrorMessage = "O ARQUIVO DE ALARME deve conter no máximo 50.")]
        public string CONF_NM_ARQUIVO_ALARME { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_REFRESH_NOTIFICACAO { get; set; }
        [Required(ErrorMessage = "Campo LOGIN SMS PADRÃO obrigatorio")]
        public string CONF_SG_LOGIN_SMS { get; set; }
        [Required(ErrorMessage = "Campo SENHA SMS PADRÃO obrigatorio")]
        public string CONF_SG_SENHA_SMS { get; set; }
        public Nullable<int> CONF_IN_CNPJ_DUPLICADO { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_DIAS_ACAO { get; set; }
        public Nullable<int> CONF_IN_ASSINANTE_FILIAL { get; set; }
        public Nullable<int> CONF_IN_FALHA_IMPORTACAO { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        [Range(1, 9, ErrorMessage = "Não pode haver mais de 9 etapas num processo")]
        public Nullable<int> CONF_IN_ETAPAS_CRM { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_ADM { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_GER { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_VEN { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_OPR { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_USU { get; set; }
        [Display(Name = "Ações em Atraso")]
        public string LABEL1 { get; set; }
        [StringLength(250, ErrorMessage = "O LINK DO SISTEMA deve conter no máximo 250 caracteres.")]
        public string CONF_LK_LINK_SISTEMA { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string CONF_EM_CRMSYS { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string CONF_EM_CRMSYS1 { get; set; }
        [StringLength(50, ErrorMessage = "O WHATSAPP deve conter no máximo 50 caracteres.")]
        public string CONF_NR_SUPORTE_ZAP { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        [Required(ErrorMessage = "Campo VALIDADE DA SENHA obrigatorio")]
        public Nullable<int> CONF_NR_VALIDADE_SENHA { get; set; }
        [Required(ErrorMessage = "Campo TAMANHO MÍNIMO DA SENHA obrigatorio")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_TAMANHO_SENHA { get; set; }
        [Required(ErrorMessage = "Campo VALIDADE DA PESQUISA obrigatorio")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_VALIDADE_PESQUISA { get; set; }
        public string CONF_NM_SENDGRID_LOGIN { get; set; }
        public string CONF_NM_SENDGRID_PWD { get; set; }
        public string CONF_NM_SENDGRID_APIKEY { get; set; }
        public Nullable<int> CONF_IN_LOGO_EMPRESA { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_GRID_CLIENTE { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_GRID_MENSAGEM { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_GRID_PRODUTO { get; set; }
        [Required(ErrorMessage = "Campo VALIDADE DO CÓDIGO obrigatorio")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_IN_VALIDADE_CODIGO { get; set; }
        public string CONF_SG_LOGIN_SMS_CRIP { get; set; }
        public string CONF_SG_SENHA_SMS_CRIP { get; set; }
        public string CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP { get; set; }
        public string CONF_SG_SENHA_SMS_PRIORITARIO_CRIP { get; set; }
        public string CONF_NM_SENDGRID_LOGIN_CRIP { get; set; }
        public string CONF_NM_SENDGRID_PWD_CRIP { get; set; }
        public string CONF_NM_SENDGRID_APIKEY_CRIP { get; set; }
        public string CONF_CS_CONNECTION_STRING_AZURE_CRIP { get; set; }
        public string CONF_NM_KEY_AZURE_CRIP { get; set; }
        public string CONF_NM_ENDPOINT_AZURE_CRIP { get; set; }
        public string CONF_NM_EMISSOR_AZURE_CRIP { get; set; }
        public string CONF_SG_LOGIN_SMS_PRIORITARIO { get; set; }
        public string CONF_SG_SENHA_SMS_PRIORITARIO { get; set; }
        public string CONF_NM_ENDPOINT_AZURE { get; set; }
        public string CONF_NM_EMISSOR_AZURE { get; set; }
        public string CONF_NM_LOCAL_AUDIO { get; set; }

        public bool NotifAcaoAdm
        {
            get
            {
                if (CONF_IN_NOTIF_ACAO_ADM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CONF_IN_NOTIF_ACAO_ADM = (value == true) ? 1 : 0;
            }
        }
        public bool NotifAcaoGer
        {
            get
            {
                if (CONF_IN_NOTIF_ACAO_GER == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CONF_IN_NOTIF_ACAO_GER = (value == true) ? 1 : 0;
            }
        }
        public bool NotifAcaoVen
        {
            get
            {
                if (CONF_IN_NOTIF_ACAO_VEN == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CONF_IN_NOTIF_ACAO_VEN = (value == true) ? 1 : 0;
            }
        }
        public bool NotifAcaoOpr
        {
            get
            {
                if (CONF_IN_NOTIF_ACAO_OPR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CONF_IN_NOTIF_ACAO_OPR = (value == true) ? 1 : 0;
            }
        }
        public bool NotifAcaoUsu
        {
            get
            {
                if (CONF_IN_NOTIF_ACAO_USU == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CONF_IN_NOTIF_ACAO_USU = (value == true) ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}