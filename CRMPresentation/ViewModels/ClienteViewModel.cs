using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ClienteViewModel
    {
        [Key]
        public int CLIE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Display(Name = "Filial")]
        public Nullable<int> FILI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        [Display(Name = "Categoria do Cliente")]
        public Nullable<int> CACL_CD_ID { get; set; }
        [Display(Name = "Genero")]
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> TICO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        [Display(Name = "Tipo de Pessoa")]
        public Nullable<int> TIPE_CD_ID { get; set; }
        public Nullable<int> RETR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        [Display(Name = "Nome")]
        public string CLIE_NM_NOME { get; set; }
        [StringLength(100, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 100.")]
        [Display(Name = "Razão Social")]
        public string CLIE_NM_RAZAO { get; set; }
        [StringLength(20, MinimumLength = 11, ErrorMessage = "O CPF deve conter no minimo 11 caracteres e no máximo 20.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        [Display(Name = "CPF")]
        public string CLIE_NR_CPF { get; set; }
        [StringLength(20, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter no minimo 14 caracteres e no máximo 20.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        [Display(Name = "CNPJ")]
        public string CLIE_NR_CNPJ { get; set; }
        [StringLength(20, MinimumLength = 1, ErrorMessage = "O RG deve conter no minimo 1 caracteres e no máximo 20.")]
        [RegularExpression(@"^([0-9]|-|_|\s)+$$", ErrorMessage = "RG inválido")]
        [Display(Name = "RG")]
        public string CLIE_NR_RG { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        [Display(Name = "E-Mail")]
        public string CLIE_NM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50.")]
        [Display(Name = "Telefone")]
        public string CLIE_NR_TELEFONE { get; set; }
        [StringLength(50, ErrorMessage = "AS REDES SOCIAIS deve conter no máximo 50.")]
        public string CLIE_NM_REDES_SOCIAIS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CADASTRO deve ser uma data válida")]
        public System.DateTime CLIE_DT_CADASTRO { get; set; }
        public Nullable<int> CLIE_IN_ATIVO { get; set; }
        public string CLIE_AQ_FOTO { get; set; }
        [StringLength(50, ErrorMessage = "A PROFISSÃO deve conter no máximo 50.")]
        public string CLIE_NM_PROFISSAO { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 50.")]
        [Display(Name = "Inscrição Estadual")]
        public string CLIE_NR_INSCRICAO_ESTADUAL { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO MUNICIPAL deve conter no máximo 50.")]
        [Display(Name = "Inscrição Municipal")]
        public string CLIE_NR_INSCRICAO_MUNICIPAL { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR deve conter no máximo 50.")]
        [Display(Name = "Celular")]
        public string CLIE_NR_CELULAR { get; set; }
        [StringLength(50, ErrorMessage = "O WEBSITE deve conter no máximo 50.")]
        [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$", ErrorMessage = "Website inválido")]
        public string CLIE_NM_WEBSITE { get; set; }
        [StringLength(100, ErrorMessage = "O E_MAIL para DANFE deve conter no máximo 100.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string CLIE_NM_EMAIL_DANFE { get; set; }
        [StringLength(50, ErrorMessage = "O FAX deve conter no máximo 50.")]
        public string CLIE_NR_FAX { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO CONJUGE deve conter no máximo 50.")]
        public string CLIE_NM_NOME_CONJUGE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE NASCIMENTO deve ser uma data válida")]
        [Display(Name = "Data de Nascimento")]
        public Nullable<System.DateTime> CLIE_DT_NASCIMENTO { get; set; }
        public string CLIE_TX_OBSERVACOES { get; set; }
        [StringLength(100, ErrorMessage = "O ENDEREÇO deve conter no máximo 100.")]
        public string CLIE_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50.")]
        public string CLIE_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50.")]
        [Display(Name = "Cidade")]
        public string CLIE_NM_CIDADE { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10.")]
        [RegularExpression(@"^([0-9]|-|_|\s)+$$", ErrorMessage = "CEP inválido")]
        [Display(Name = "CEP")]
        public string CLIE_NR_CEP { get; set; }
        [Display(Name = "UF")]
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(100, ErrorMessage = "O ENDEREÇO deve conter no máximo 100.")]
        public string CLIE_NM_ENDERECO_ENTREGA { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50.")]
        public string CLIE_NM_BAIRRO_ENTREGA { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50.")]
        public string CLIE_NM_CIDADE_ENTREGA { get; set; }
        public string CLIE_SG_UF { get; set; }
        public string CLIE_SG_UF_ENTREGA { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 50.")]
        [RegularExpression(@"^([0-9]|-|_|\s)+$$", ErrorMessage = "CEP inválido")]
        public string CLIE_NR_CEP_ENTREGA { get; set; }
        public Nullable<int> CLIE_UF_CD_ENTREGA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O LIMITE DE CRÉDITO deve ser um valor numérico positivo")]
        public Nullable<decimal> CLIE_VL_LIMITE_CREDITO { get; set; }
        public string LimiteText
        {
            get
            {
                return CLIE_VL_LIMITE_CREDITO.HasValue ? CrossCutting.Formatters.DecimalFormatter(CLIE_VL_LIMITE_CREDITO.Value) : string.Empty;
            }
            set
            {
                CLIE_VL_LIMITE_CREDITO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50.")]
        public string CLIE_NR_TELEFONE_ADICIONAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O SALDO deve ser um valor numérico positivo")]
        public Nullable<decimal> CLIE_VL_SALDO { get; set; }
        [StringLength(10, ErrorMessage = "O NÚMERO SUFRAMA deve conter no máximo 10.")]
        public string CLIE_NR_SUFRAMA { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO PAI deve conter no máximo 50.")]
        public string CLIE_NM_PAI { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DA MÃE deve conter no máximo 50.")]
        public string CLIE_NM_MAE { get; set; }
        [StringLength(50, ErrorMessage = "A NATURALIDADE deve conter no máximo 50.")]
        [Display(Name = "Naturalidade")]
        public string CLIE_NM_NATURALIDADE { get; set; }
        [Display(Name = "Naturalidade - UF")]
        public string CLIE_SG_NATURALIADE_UF { get; set; }
        [StringLength(50, ErrorMessage = "A NACIONALIDADE deve conter no máximo 50.")]
        [Display(Name = "Nacionalidade")]
        public string CLIE_NM_NACIONALIDADE { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10.")]
        public string CLIE_NR_CEP_BUSCA { get; set; }
        [StringLength(50, ErrorMessage = "O COMPLEMENTO deve conter no máximo 50.")]
        public string CLIE_NM_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "O COMPLEMENTO deve conter no máximo 50.")]
        public string CLIE_NM_COMPLEMENTO_ENTREGA { get; set; }
        public string CLIE_NM_SITUACAO { get; set; }
        public string CLIE_NR_NUMERO { get; set; }
        public string CLIE_NR_NUMERO_ENTREGA { get; set; }
        public Nullable<int> CLIE_IN_STATUS { get; set; }
        public Nullable<System.DateTime> DATA_FINAL { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DO ÚLTIMO CONTATO deve ser uma data válida")]
        public Nullable<System.DateTime> CLIE_DT_ULTIMO_USO { get; set; }
        public Nullable<int> NACI_CD_ID { get; set; }
        public Nullable<int> MUNI_CD_ID { get; set; }
        public Nullable<int> MUNI_SG_UF { get; set; }

        // Filtros
        public Nullable<int> OperaCategoria { get; set; }
        public Nullable<int> OperaTipe{ get; set; }
        public Nullable<int> OperaFilal { get; set; }
        public Nullable<int> OperaSexo { get; set; }
        public Nullable<int> OperaNome { get; set; }
        public Nullable<int> OperaRazao { get; set; }
        public Nullable<int> OperaCPF{ get; set; }
        public Nullable<int> OperaCNPJ{ get; set; }
        public Nullable<int> OperaEMail{ get; set; }
        public Nullable<int> OperaTelefone { get; set; }
        public Nullable<int> OperaCelular { get; set; }
        public Nullable<int> OperaCidade { get; set; }
        public Nullable<int> OperaUF { get; set; }
        public Nullable<int> OperaCEP { get; set; }
        public Nullable<int> OperaIncEst { get; set; }
        public Nullable<int> OperaIncMun { get; set; }
        public Nullable<int> OperaDataNasc { get; set; }
        public Nullable<int> OperaNatur { get; set; }
        public Nullable<int> OperaUFNatur { get; set; }
        public Nullable<int> OperaNacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO> ATENDIMENTO { get; set; }
        public virtual CATEGORIA_CLIENTE CATEGORIA_CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_ANEXO> CLIENTE_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_ANOTACAO> CLIENTE_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_CONTATO> CLIENTE_CONTATO { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_QUADRO_SOCIETARIO> CLIENTE_QUADRO_SOCIETARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_REFERENCIA> CLIENTE_REFERENCIA { get; set; }
        public virtual REGIME_TRIBUTARIO REGIME_TRIBUTARIO { get; set; }
        public virtual SEXO SEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_TAG> CLIENTE_TAG { get; set; }
        public virtual TIPO_CONTRIBUINTE TIPO_CONTRIBUINTE { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
        public virtual UF UF1 { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMAIL_AGENDAMENTO> EMAIL_AGENDAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO_CLIENTE> GRUPO_CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_DESTINOS> MENSAGENS_DESTINOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_ENVIADAS_SISTEMA> MENSAGENS_ENVIADAS_SISTEMA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA> PESQUISA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_ENVIO> PESQUISA_ENVIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA_RESPOSTA> PESQUISA_RESPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECURSIVIDADE_DESTINO> RECURSIVIDADE_DESTINO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESULTADO_ROBOT> RESULTADO_ROBOT { get; set; }
        public virtual MUNICIPIO MUNICIPIO { get; set; }
        public virtual NACIONALIDADE NACIONALIDADE { get; set; }
        public virtual UF UF2 { get; set; }
    }
}