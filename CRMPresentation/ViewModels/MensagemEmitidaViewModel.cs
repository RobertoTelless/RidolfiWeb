    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MensagemEmitidaViewModel
    {
        [Key]
        public int MEEN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public Nullable<int> CRM1_CD_ID { get; set; }
        public Nullable<int> CRCO_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> MEEN_IN_USUARIO { get; set; }
        public Nullable<int> MEEN_IN_ASSINANTE { get; set; }
        public int MEEN_IN_TIPO { get; set; }
        public Nullable<System.DateTime> MEEN_DT_DATA_ENVIO { get; set; }
        public string MEEN_EM_EMAIL_DESTINO { get; set; }
        public string MEEN__CELULAR_DESTINO { get; set; }
        public string MEEN_NM_ORIGEM { get; set; }
        public string MEEN_NM_TITULO { get; set; }
        public string MEEN_TX_CORPO { get; set; }
        public string MEEN_TX_CORPO_COMPLETO { get; set; }
        public Nullable<int> MEEN_IN_ESCOPO { get; set; }
        public Nullable<int> MEEN_IN_ANEXOS { get; set; }
        public Nullable<int> MEEN_IN_ENTREGUE { get; set; }
        public string MEEN_LK_LINK { get; set; }
        public string MEEN_TX_RETORNO { get; set; }
        public int MEEN_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> MEEN_DT_DUMMY { get; set; }
        public string MEEN_SG_STATUS { get; set; }
        public string MEEN_GU_ID_MENSAGEM { get; set; }

        public String Tipo
        {
            get
            {
                if (MEEN_IN_TIPO == 1)
                {
                    return "E-Mail";
                }
                if (MEEN_IN_TIPO == 2)
                {
                    return "SMS";
                }
                if (MEEN_IN_TIPO == 3)
                {
                    return "WhatsApp";
                }
                return String.Empty;
            }
        }

        public String Escopo
        {
            get
            {
                if (MEEN_IN_ESCOPO == 1)
                {
                    return "Emoresa";
                }
                if (MEEN_IN_ESCOPO == 2)
                {
                    return "Cliente";
                }
                if (MEEN_IN_ESCOPO == 3)
                {
                    return "Usuário";
                }
                if (MEEN_IN_ESCOPO == 4)
                {
                    return "Usuário";
                }
                if (MEEN_IN_ESCOPO == 5)
                {
                    return "Processo CRM";
                }
                return String.Empty;
            }
        }

        public String Anexo
        {
            get
            {
                if (MEEN_IN_ANEXOS == 0)
                {
                    return "Nenhum";
                }
                else
                {
                    return MEEN_IN_ANEXOS.ToString();
                }
                return String.Empty;
            }
        }

        public String Entregue
        {
            get
            {
                if (MEEN_SG_STATUS == "Succeeded")
                {
                    return "Sim";
                }
                else
                {
                    return "Não";
                }
                return String.Empty;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual ASSINANTE ASSINANTE1 { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual CRM CRM { get; set; }
        public virtual CRM_CONTATO CRM_CONTATO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
    }
}