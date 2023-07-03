using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class RecursividadeViewModel
    {
        [Key]
        public int RECU_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int RECU_IN_TIPO_MENSAGEM { get; set; }
        public System.DateTime RECU_DT_CRIACAO { get; set; }
        public int RECU_IN_TIPO_SMS { get; set; }
        public string RECU_NM_NOME { get; set; }
        public string RECU_LK_LINK { get; set; }
        public string RECU_TX_TEXTO { get; set; }
        public int RECU_IN_ATIVO { get; set; }
        public Nullable<int> MENS_CD_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECURSIVIDADE_DATA> RECURSIVIDADE_DATA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECURSIVIDADE_DESTINO> RECURSIVIDADE_DESTINO { get; set; }
        public virtual MENSAGENS MENSAGENS { get; set; }
    }
}