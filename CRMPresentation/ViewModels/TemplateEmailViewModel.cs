using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TemplateEMailViewModel
    {
        [Key]
        public int TEEM_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        [RegularExpression(@"^[0-9a-záâãéêíóôõú'A-ZÁÂÃÉÊÍÓÕÔÚ']$", ErrorMessage = "Nome inválido")]
        public string TEEM_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10 caracteres.")]
        [RegularExpression(@"^[0-9a-záâãéêíóôõú'A-ZÁÂÃÉÊÍÓÕÔÚ']$", ErrorMessage = "Sigla inválida")]
        public string TEEM_SG_SIGLA { get; set; }
        [StringLength(50, ErrorMessage = "O LINK deve conter máximo 50 caracteres.")]
        [RegularExpression(@"^((http|ftp|https|www)://)?([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$", ErrorMessage = "Link inválido")]
        public string TEEM_LK_LINK { get; set; }
        public Nullable<int> TEEM_IN_ATIVO { get; set; }
        public string TEEM_TX_CABECALHO { get; set; }
        public string TEEM_TX_CORPO { get; set; }
        public string TEEM_TX_DADOS { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO ARQUIVO deve conter no máximo 250 caracteres.")]
        public string TEEM_AQ_ARQUIVO { get; set; }
        [AllowHtml]
        public Nullable<int> TEEM_IN_HTML { get; set; }
        [AllowHtml]
        public Nullable<int> TEEM_IN_FIXO { get; set; }
        [AllowHtml]
        public Nullable<int> TEEM_IN_IMAGEM { get; set; }

        public string file { get; set; }
        public Int32 html { get; set; }

        public String TIPO
        {
            get
            {
                if (TEEM_IN_HTML == 1)
                {
                    return "Texto HTML Digitado";
                }
                return "Arquivo HTML";
            }
        }
        public String IMAGEM
        {
            get
            {
                if (TEEM_IN_IMAGEM == 2)
                {
                    return "Imagens Embutidas";
                }
                if (TEEM_IN_IMAGEM == 1)
                {
                    return "Imagens Externas";
                }
                return "-";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_AUTOMACAO> MENSAGEM_AUTOMACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA> PESQUISA { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }

    }
}