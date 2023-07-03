using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ContatoViewModel
    {
        [Key]
        public int CONT_CD_ID { get; set; }
        public int BENE_CD_ID { get; set; }
        public int QUAL_CD_ID { get; set; }
        public int QUDE_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE NASCIMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> CONT_DT_CONTATO { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO AGENTE deve conter no máximo 50 caracteres.")]
        public string CONT_NM_AGENTE { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO REQYERENTE deve conter no máximo 50 caracteres.")]
        public string CONT_NM_REQUERENTE { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public string CONT_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O CPF deve conter no máximo 20 caracteres.")]
        public string CONT_NR_CPF { get; set; }
        [StringLength(50, ErrorMessage = "O PROTOCOLO deve conter no máximo 50 caracteres.")]
        public string CONT_NR_PROTOCOLO { get; set; }
        public Nullable<int> CONT_IN_ATIVO { get; set; }

        public virtual BENEFICIARIO BENEFICIARIO { get; set; }
        public virtual QUEM_DESLIGOU QUEM_DESLIGOU { get; set; }
        public virtual QUALIFICACAO QUALIFICACAO { get; set; }
    }
}