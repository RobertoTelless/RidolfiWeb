//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class DIARIO_PROCESSO
    {
        public int DIPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        public Nullable<int> CRPV_CD_ID { get; set; }
        public Nullable<int> CRAC_CD_ID { get; set; }
        public Nullable<int> CRCM_CD_ID { get; set; }
        public Nullable<int> CRFL_CD_ID { get; set; }
        public Nullable<int> AGEN_CD_ID { get; set; }
        public System.DateTime DIPR_DT_DATA { get; set; }
        public string DIPR_NM_OPERACAO { get; set; }
        public string DIPR_DS_DESCRICAO { get; set; }
        public Nullable<System.DateTime> DIPR_DT_DUMMY { get; set; }
        public Nullable<System.DateTime> DIPR_DT_DUMMY_1 { get; set; }
    
        public virtual AGENDA AGENDA { get; set; }
        public virtual CRM CRM { get; set; }
        public virtual CRM_ACAO CRM_ACAO { get; set; }
        public virtual CRM_COMENTARIO CRM_COMENTARIO { get; set; }
        public virtual CRM_FOLLOW CRM_FOLLOW { get; set; }
        public virtual CRM_PEDIDO_VENDA CRM_PEDIDO_VENDA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
