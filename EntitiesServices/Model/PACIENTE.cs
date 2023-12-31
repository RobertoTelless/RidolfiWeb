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
    
    public partial class PACIENTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PACIENTE()
        {
            this.AGENDA = new HashSet<AGENDA>();
            this.MENSAGENS_ENVIADAS_SISTEMA = new HashSet<MENSAGENS_ENVIADAS_SISTEMA>();
            this.PACIENTE_ANAMNESE = new HashSet<PACIENTE_ANAMNESE>();
            this.PACIENTE_ANEXO = new HashSet<PACIENTE_ANEXO>();
            this.PACIENTE_ANOTACAO = new HashSet<PACIENTE_ANOTACAO>();
            this.PACIENTE_CONSULTA = new HashSet<PACIENTE_CONSULTA>();
            this.PACIENTE_PRESCRICAO = new HashSet<PACIENTE_PRESCRICAO>();
        }
    
        public int PACI__CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int TIPA_CD_ID { get; set; }
        public string PACI_NM_NOME { get; set; }
        public string PACI_NM_SOCIAL { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> COR1_CD_ID { get; set; }
        public Nullable<int> ESCI_CD_ID { get; set; }
        public Nullable<int> CONV_CD_ID { get; set; }
        public string PACI_NM_PAI { get; set; }
        public string PACI_NM_MAE { get; set; }
        public string PACI_NR_CEP { get; set; }
        public string PACI_NM_ENDERECO { get; set; }
        public string PACI_NR_NUMERO { get; set; }
        public string PACI_NR_COMPLEMENTO { get; set; }
        public string PACI_NM_BAIRRO { get; set; }
        public string PACI_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string PACI_NM_PROFISSAO { get; set; }
        public Nullable<System.DateTime> PACI_DT_NASCIMENTO { get; set; }
        public string PACI_NM_NATURALIDADE { get; set; }
        public string PACI_NM_NACIONALIDADE { get; set; }
        public string PACI_NR_CPF { get; set; }
        public string PACI_NR_RG { get; set; }
        public string PACI_NR_TELEFONE { get; set; }
        public string PACI_NR_CELULAR { get; set; }
        public string PACI_NM_EMAIL { get; set; }
        public Nullable<int> PACI_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> PACI_DT_CADASTRO { get; set; }
        public string PACI_AQ_FOTO { get; set; }
        public string PACI_NR_MATRICULA { get; set; }
        public Nullable<int> GRAU_CD_ID { get; set; }
        public Nullable<System.DateTime> PACI_DT_PREVISAO_RETORNO { get; set; }
        public string PACI_NM_INDICACAO { get; set; }
        public string PACI_TX_OBSERVACOES { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CONVENIO CONVENIO { get; set; }
        public virtual COR COR { get; set; }
        public virtual ESTADO_CIVIL ESTADO_CIVIL { get; set; }
        public virtual GRAU_INSTRUCAO GRAU_INSTRUCAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_ENVIADAS_SISTEMA> MENSAGENS_ENVIADAS_SISTEMA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_ANAMNESE> PACIENTE_ANAMNESE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_ANEXO> PACIENTE_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_ANOTACAO> PACIENTE_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_CONSULTA> PACIENTE_CONSULTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_PRESCRICAO> PACIENTE_PRESCRICAO { get; set; }
        public virtual SEXO SEXO { get; set; }
        public virtual TIPO_PACIENTE TIPO_PACIENTE { get; set; }
        public virtual UF UF { get; set; }
    }
}
