using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PesquisaViewModel
    {
        [Key]
        public int PESQ_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESQUISA obrigatorio")]
        public int TIPS_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string PESQ_NM_NOME { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string PESQ_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÂO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime PESQ_DT_CRIACAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public System.DateTime PESQ_DT_VALIDADE { get; set; }
        [StringLength(150, ErrorMessage = "O LINK deve conter no máximo 150 caracteres.")]
        public string PESQ_LK_LINK { get; set; }
        public byte[] PESQ_TX_OBSERVACAO { get; set; }
        public int PESQ_IN_ATIVO { get; set; }
        [StringLength(1000, ErrorMessage = "AS OBSERVAÇÕES deve conter no máximo 1000 caracteres.")]
        public string PESQ_TX_OBSERVACOES { get; set; }
        [StringLength(50, ErrorMessage = "A CAMPANHA deve conter no máximo 50 caracteres.")]
        public string PESQ_NM_CAMPANHA { get; set; }
        public Nullable<int> GRUP_CD_ID { get; set; }
        public Nullable<int> TEEM_CD_ID { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public string PESQ_TX_CABECALHO { get; set; }
        public string PESQ_TX_CORPO { get; set; }
        public string PESQ_TX_RODAPE { get; set; }
        public Nullable<int> ID { get; set; }

        public String NumPerguntas
        {
            get
            {
                if (PESQUISA_ITEM.Count == 0)
                {
                    return "Nenhuma";
                }
                else
                {
                    return PESQUISA_ITEM.Count.ToString();
                }
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_ANOTACAO> PESQUISA_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_ANEXO> PESQUISA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_ITEM> PESQUISA_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_RESPOSTA> PESQUISA_RESPOSTA { get; set; }
        public virtual TIPO_PESQUISA TIPO_PESQUISA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_ENVIO> PESQUISA_ENVIO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual GRUPO GRUPO { get; set; }
        public virtual TEMPLATE_EMAIL TEMPLATE_EMAIL { get; set; }
    }
}