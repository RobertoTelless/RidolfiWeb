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
    
    public partial class TELEFONE
    {
        public int TELE_CD_ID { get; set; }
        public int BENE_CD_ID { get; set; }
        public int TITE_CD_ID { get; set; }
        public string TELE_NR_NUMERO { get; set; }
        public int TELE_IN_ATIVO { get; set; }
        public string TELE_NR_CELULAR { get; set; }
    
        public virtual BENEFICIARIO BENEFICIARIO { get; set; }
        public virtual TIPO_TELEFONE_BASE TIPO_TELEFONE_BASE { get; set; }
    }
}
