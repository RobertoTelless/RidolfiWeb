using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ContatoSuporteViewModel
    {
        public Int32 ASSI_CD_ID { get; set; }
        public String ASSI_NM_NOME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COSU_DT_CONTATO { get; set; }
        public String COSU_TX_OBSERVACOES { get; set; }
        [Required(ErrorMessage = "Campo MENSAGEM obrigatorio")]
        [StringLength(1000, ErrorMessage = "A MENSAGEM deve conter no máximo 1000 caracteres.")]
        public String COSU_TX_MENSAGEM { get; set; }
        public String COSU_AQ_ANEXO { get; set; }
        public String COSU_GU_GUID { get; set; }
        public Nullable<Int32> COSU_IN_TIPO { get; set; }
        public Nullable<Int32> COSU_IN_PRIORIDADE { get; set; }
        public Nullable<Int32> COSU_IN_RESPOSTA { get; set; }
        public Nullable<Int32> COSU_IN_HORARIO { get; set; }
        [StringLength(150, ErrorMessage = "O E-MAIL deve conter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public String COSU_EM_MAIL { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public String COSU_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public String COSU_NR_CELULAR { get; set; }
    }
}