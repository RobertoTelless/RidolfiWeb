using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EmpresaCustoVariavelViewModel
    {
        [Key]
        public int EMCV_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatório")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string EMCV_NM_NOME { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMCV_VL_VALOR { get; set; }
        public Nullable<int> EMCV_IN_ATIVO { get; set; }
        public Nullable<int> PLEN_CD_ID { get; set; }
        public Nullable<int> MAQN_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> EMCV_PC_PERCENTUAL_VENDA { get; set; }
        [Required(ErrorMessage = "Campo TAXA obrigatório")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMCV_PC_TAXA { get; set; }
        public Nullable<int> EMCV_IN_TIPO { get; set; }
        public Nullable<int> EMCV_IN_VENDA { get; set; }
        public Nullable<int> TICK_CD_ID { get; set; }

        public String Tipo
        {
            get
            {
                if (EMCV_IN_TIPO == 1)
                {
                    return "Dinheiro";
                }
                if (EMCV_IN_TIPO == 2)
                {
                    return "Cartão";
                }
                if (EMCV_IN_TIPO == 3)
                {
                    return "Plataforma de Entrega";
                }
                if (EMCV_IN_TIPO == 4)
                {
                    return "Ticket Alimentação";
                }
                return "Dinheiro";
            }
        }

        public String TipoVenda
        {
            get
            {
                if (EMCV_IN_VENDA == 1)
                {
                    return "Custos Referentes a Vendas";
                }
                if (EMCV_IN_VENDA == 0)
                {
                    return "Custos Diversos";
                }
                return "Custos Diversos";
            }
        }

        public virtual EMPRESA EMPRESA { get; set; }
        public virtual MAQUINA MAQUINA { get; set; }
        public virtual PLATAFORMA_ENTREGA PLATAFORMA_ENTREGA { get; set; }
        public virtual TICKET_ALIMENTACAO TICKET_ALIMENTACAO { get; set; }
    }
}