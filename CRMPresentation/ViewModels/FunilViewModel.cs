using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FunilViewModel
    {
        [Key]
        public int FUNI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string FUNI_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10 caracteres.")]
        public string FUNI_SG_SIGLA { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string FUNI_DS_DESCRICAO { get; set; }
        public System.DateTime FUNI_DT_CADASTRO { get; set; }
        public int FUNI_IN_ATIVO { get; set; }
        public Nullable<int> FUNI_IN_ACOES { get; set; }
        public Nullable<int> FUNI_IN_CONTATO { get; set; }
        public Nullable<int> FUNI_IN_PROPOSTA { get; set; }
        public Nullable<int> FUNI_IN_AGENDA { get; set; }
        public Nullable<int> FUNI_IN_FIXO { get; set; }
        public Nullable<int> FUNI_IN_CLIENTE { get; set; }
        public Nullable<int> FUNI_IN_RESPONSAVEL { get; set; }
        public Nullable<int> FUNI_IN_TIPO { get; set; }

        public bool Proposta
        {
            get
            {
                if (FUNI_IN_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FUNI_IN_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool Cliente
        {
            get
            {
                if (FUNI_IN_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FUNI_IN_CLIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool Responsavel
        {
            get
            {
                if (FUNI_IN_RESPONSAVEL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FUNI_IN_RESPONSAVEL = (value == true) ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNIL_ETAPA> FUNIL_ETAPA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
    }
}