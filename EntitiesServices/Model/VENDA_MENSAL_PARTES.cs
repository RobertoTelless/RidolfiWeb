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
    
    public partial class VENDA_MENSAL_PARTES
    {
        public int VMPA_CD_ID { get; set; }
        public int VEMA_CD_ID { get; set; }
        public Nullable<int> VMPA_IN_TIPO { get; set; }
        public Nullable<int> MAQN_CD_ID { get; set; }
        public Nullable<int> TICK_CD_ID { get; set; }
        public Nullable<int> PLEN_CD_ID { get; set; }
        public Nullable<decimal> VMPA_VL_TOTAL { get; set; }
    
        public virtual MAQUINA MAQUINA { get; set; }
        public virtual PLATAFORMA_ENTREGA PLATAFORMA_ENTREGA { get; set; }
        public virtual TICKET_ALIMENTACAO TICKET_ALIMENTACAO { get; set; }
        public virtual VENDA_MENSAL VENDA_MENSAL { get; set; }
    }
}