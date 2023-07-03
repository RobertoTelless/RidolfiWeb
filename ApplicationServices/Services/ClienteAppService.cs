using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class ClienteAppService : AppServiceBase<CLIENTE>, IClienteAppService
    {
        private readonly IClienteService _baseService;
        private readonly IConfiguracaoService _confService;
        private readonly ICRMService _crmService;
        private readonly ICategoriaClienteService _ccService;
        private readonly IPrecatorioService _precService;
        private readonly IBeneficiarioService _beneService;

        public ClienteAppService(IClienteService baseService, IConfiguracaoService confService, ICRMService crmService, ICategoriaClienteService ccService, IPrecatorioService precService, IBeneficiarioService beneService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            _crmService = crmService;   
            _ccService = ccService; 
            _precService = precService;
            _beneService = beneService;
        }

        public List<CLIENTE> GetAllItens(Int32 idAss)
        {
            List<CLIENTE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CLIENTE> GetAllItensUltimos(Int32 num, Int32 idAss)
        {
            List<CLIENTE> lista = _baseService.GetAllItensUltimos(num, idAss);
            return lista;
        }

        public CLIENTE_ANOTACAO GetComentarioById(Int32 id)
        {
            return _baseService.GetComentarioById(id);
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<CLIENTE> GetAllItensAdm(Int32 idAss)
        {
            List<CLIENTE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<TIPO_CONTRIBUINTE> GetAllContribuinte(Int32 idAss)
        {
            List<TIPO_CONTRIBUINTE> lista = _baseService.GetAllContribuinte(idAss);
            return lista;
        }

        public List<REGIME_TRIBUTARIO> GetAllRegimes(Int32 idAss)
        {
            List<REGIME_TRIBUTARIO> lista = _baseService.GetAllRegimes(idAss);
            return lista;
        }

        public List<CLIENTE_FALHA> GetAllFalhas(Int32 idAss)
        {
            List<CLIENTE_FALHA> lista = _baseService.GetAllFalhas(idAss);
            return lista;
        }

        public List<SEXO> GetAllSexo()
        {
            List<SEXO> lista = _baseService.GetAllSexo();
            return lista;
        }

        public List<LINGUA> GetAllLinguas()
        {
            List<LINGUA> lista = _baseService.GetAllLinguas();
            return lista;
        }

        public List<NACIONALIDADE> GetAllNacionalidades()
        {
            List<NACIONALIDADE> lista = _baseService.GetAllNacionalidades();
            return lista;
        }

        public List<MUNICIPIO> GetAllMunicipios()
        {
            List<MUNICIPIO> lista = _baseService.GetAllMunicipios();
            return lista;
        }

        public List<MUNICIPIO> GetMunicipioByUF(Int32 uf)
        {
            List<MUNICIPIO> lista = _baseService.GetMunicipioByUF(uf);
            return lista;
        }

        public CLIENTE GetItemById(Int32 id)
        {
            CLIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public CLIENTE GetByEmail(String email)
        {
            CLIENTE item = _baseService.GetByEmail(email);
            return item;
        }

        public GRUPO_CLIENTE GetGrupoById(Int32 id)
        {
            GRUPO_CLIENTE lista = _baseService.GetGrupoById(id);
            return lista;
        }

        public CLIENTE CheckExist(CLIENTE conta, Int32 idAss)
        {
            CLIENTE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public CLIENTE CheckExistDoctos(String cpf, String cnpj, Int32 idAss)
        {
            CLIENTE item = _baseService.CheckExistDoctos(cpf, cnpj, idAss);
            return item;
        }

        public MUNICIPIO CheckExistMunicipio(MUNICIPIO conta)
        {
            MUNICIPIO item = _baseService.CheckExistMunicipio(conta);
            return item;
        }

        public List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_CLIENTE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllTiposPessoa();
            return lista;
        }

        public CLIENTE_ANEXO GetAnexoById(Int32 id)
        {
            CLIENTE_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CLIENTE_CONTATO GetContatoById(Int32 id)
        {
            CLIENTE_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public CLIENTE_ANOTACAO GetAnotacaoById(Int32 id)
        {
            CLIENTE_ANOTACAO lista = _baseService.GetAnotacaoById(id);
            return lista;
        }

        public CLIENTE_REFERENCIA GetReferenciaById(Int32 id)
        {
            CLIENTE_REFERENCIA lista = _baseService.GetReferenciaById(id);
            return lista;
        }

        public List<VOLTA_PESQUISA> PesquisarTudo(String parm, Int32[] permissoes, String perfil, Int32? empresa, Int32 idAss)
        {
            // Critica
            if (parm == null)
            {
                return null;
            }

            // Busca em Clientes
            List<CLIENTE> listaClientes = new List<CLIENTE>();
            listaClientes = _baseService.GetAllItens(idAss).Where(p => p.CLIE_NM_NOME.Contains(parm) || p.CLIE_NM_EMAIL.Contains(parm)).ToList();

            // Busca em Precatorios
            List<PRECATORIO> listaPrec = new List<PRECATORIO>();
            listaPrec = _precService.GetAllItens().Where(p => p.PREC_NM_PRECATORIO.Contains(parm) || p.PREC_NM_ASSUNTO.Contains(parm)  || p.PREC_NM_REQUERENTE.Contains(parm)  || p.PREC_NM_REQUERIDO.Contains(parm)  || p.PREC_NM_DEPRECANTE.Contains(parm)  || p.PREC_NM_PROCESSO_ORIGEM.Contains(parm)).ToList();

            // Busca em Beneficiarios
            List<BENEFICIARIO> listaBene = new List<BENEFICIARIO>();
            listaBene = _beneService.GetAllItens().Where(p => p.BENE_NM_NOME.Contains(parm)).ToList();

            // Busca em Processos
            List<CRM> listaCRM = new List<CRM>();
            List<CRM_ACAO> listaAcao = new List<CRM_ACAO>();
            List<CRM_PEDIDO_VENDA> listaProp = new List<CRM_PEDIDO_VENDA>();
            listaCRM = _crmService.GetAllItens(idAss).Where(p => p.CRM1_NM_NOME.Contains(parm) || p.CLIENTE.CLIE_NM_NOME.Contains(parm) || p.CRM1_DS_DESCRICAO.Contains(parm)).ToList();
            if (ValidarItensDiversos.IsDateTime(parm))
            {
                listaCRM = listaCRM.Where(p => p.CRM1_DT_CRIACAO.Value.Date == Convert.ToDateTime(parm)).ToList();
            }

            // Busca em Ações
            listaAcao = _crmService.GetAllAcoes(idAss).Where(p => p.CRAC_NM_TITULO.Contains(parm)).ToList();
            if (ValidarItensDiversos.IsDateTime(parm))
            {
                listaAcao = listaAcao.Where(p => p.CRAC_DT_CRIACAO.Value.Date == Convert.ToDateTime(parm) || p.CRAC_DT_PREVISTA.Value.Date == Convert.ToDateTime(parm)).ToList();
            }

            // Busca em Propostas
            listaProp = _crmService.GetAllPedidos(idAss).Where(p => p.CRPV_NM_NOME.Contains(parm) || p.CRPV_IN_NUMERO_GERADO.ToString() == parm || p.CRPV_TX_CONDICOES_COMERCIAIS.Contains(parm)).ToList();
            if (ValidarItensDiversos.IsDateTime(parm))
            {
                listaProp = listaProp.Where(p => p.CRPV_DT_PEDIDO.Date.ToString() == parm || p.CRPV_DT_VALIDADE.Date.ToString() == parm).ToList();
            }

            // Prepara lista de retorno
            List<VOLTA_PESQUISA> listaVolta = new List<VOLTA_PESQUISA>();
            foreach (CLIENTE item in listaClientes)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 1;
                volta.PEGR_CD_ITEM = item.CLIE_CD_ID;
                volta.PEGR_NM_NOME1 = item.CLIE_NM_NOME;
                if (item.TIPE_CD_ID == 1)
                {
                    volta.PEGR_NM_NOME2 = item.CLIE_NM_CIDADE;
                    volta.PEGR_NM_NOME3 = item.CLIE_NR_CPF;
                }
                if (item.TIPE_CD_ID == 2)
                {
                    volta.PEGR_NM_NOME2 = item.CLIE_NM_RAZAO;
                    volta.PEGR_NM_NOME3 = item.CLIE_NR_CNPJ;
                }
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);  
            }
            foreach (PRECATORIO item in listaPrec)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 5;
                volta.PEGR_CD_ITEM = item.PREC_CD_ID;
                volta.PEGR_NM_NOME1 = item.PREC_NM_PRECATORIO;
                volta.PEGR_NM_NOME2 = item.TRF.TRF1_NM_NOME;
                volta.PEGR_NM_NOME3 = item.PREC_NM_REQUERENTE;
                volta.PEGR_NM_NOME4 = item.PREC_NM_REQUERIDO;
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (BENEFICIARIO item in listaBene)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 6;
                volta.PEGR_CD_ITEM = item.BENE_CD_ID;
                volta.PEGR_NM_NOME1 = item.BENE_NM_NOME;
                volta.PEGR_NM_NOME2 = item.BENE_EM_EMAIL;
                volta.PEGR_NM_NOME3 = item.BENE_NR_TELEFONE;
                volta.PEGR_NM_NOME4 = item.BENE_NR_CELULAR;
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }

            foreach (CRM item in listaCRM)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 2;
                volta.PEGR_CD_ITEM = item.CRM1_CD_ID;
                volta.PEGR_NM_NOME1 = item.CRM1_NM_NOME;
                volta.PEGR_NM_NOME2 = item.CRM1_DT_CRIACAO.Value.ToShortDateString();
                volta.PEGR_NM_NOME3 = item.CRM1_IN_STATUS.ToString();
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (CRM_ACAO item in listaAcao)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 3;
                volta.PEGR_CD_ITEM = item.CRAC_CD_ID;
                volta.PEGR_NM_NOME1 = item.CRAC_NM_TITULO;
                volta.PEGR_NM_NOME2 = item.CRAC_DT_CRIACAO.Value.ToShortDateString();
                volta.PEGR_NM_NOME3 = item.CRAC_IN_STATUS.ToString();
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (CRM_PEDIDO_VENDA item in listaProp)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 4;
                volta.PEGR_CD_ITEM = item.CRPV_CD_ID;
                volta.PEGR_NM_NOME1 = item.CRPV_NM_NOME;
                volta.PEGR_NM_NOME2 = item.CRPV_DT_PEDIDO.ToShortDateString();
                volta.PEGR_NM_NOME3 = item.CRPV_IN_STATUS.ToString();
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            return listaVolta;
        }

        public Int32 ExecuteFilter(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32? filial, Int32 idAss, out List<CLIENTE> objeto)
        {
            try
            {
                objeto = new List<CLIENTE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(id, catId, razao, nome, cpf, cnpj, email, cidade, uf, ativo, filial, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple < Int32, List<CLIENTE>, Boolean > ExecuteFilterTuple(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32? filial, Int32 idAss)
        {
            try
            {
                List<CLIENTE> objeto = new List<CLIENTE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(id, catId, razao, nome, cpf, cnpj, email, cidade, uf, ativo, filial, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Recupera flag de duplicidade
                CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);
                Int32? dup = conf.CONF_IN_CNPJ_DUPLICADO;

                // Verifica Existencia
                if (item.TIPE_CD_ID == 1)
                {
                    if (item.CLIE_NR_CPF != null)
                    {
                        if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                        {
                            return 1;
                        }
                    }
                }
                if (item.TIPE_CD_ID == 2)
                {
                    if (dup == 0)
                    {
                        if (item.CLIE_NR_CNPJ != null)
                        {
                            if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                            {
                                return 1;
                            }
                        }
                    }
                }

                // Completa objeto
                item.CLIE_IN_ATIVO = 1;

                // Acerta letras
                if (item.CLIE_NM_NOME != null)
                {
                    item.CLIE_NM_NOME = CommonHelpers.ToPascalCase(item.CLIE_NM_NOME);
                }
                if (item.CLIE_NM_RAZAO != null)
                {
                    item.CLIE_NM_RAZAO = CommonHelpers.ToPascalCase(item.CLIE_NM_RAZAO);
                }
                if (item.CLIE_NM_ENDERECO != null)
                {
                    item.CLIE_NM_ENDERECO = CommonHelpers.ToPascalCase(item.CLIE_NM_ENDERECO);
                }
                if (item.CLIE_NM_ENDERECO_ENTREGA != null)
                {
                    item.CLIE_NM_ENDERECO_ENTREGA = CommonHelpers.ToPascalCase(item.CLIE_NM_ENDERECO_ENTREGA);
                }
                if (item.CLIE_NM_BAIRRO != null)
                {
                    item.CLIE_NM_BAIRRO = CommonHelpers.ToPascalCase(item.CLIE_NM_BAIRRO);
                }
                if (item.CLIE_NM_BAIRRO_ENTREGA != null)
                {
                    item.CLIE_NM_BAIRRO_ENTREGA = CommonHelpers.ToPascalCase(item.CLIE_NM_BAIRRO_ENTREGA);
                }
                if (item.CLIE_NM_CIDADE != null)
                {
                    item.CLIE_NM_CIDADE = CommonHelpers.ToPascalCase(item.CLIE_NM_CIDADE);
                }
                if (item.CLIE_NM_CIDADE_ENTREGA != null)
                {
                    item.CLIE_NM_CIDADE_ENTREGA = CommonHelpers.ToPascalCase(item.CLIE_NM_CIDADE_ENTREGA);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCLIE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CLIENTE>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CLIENTE item, CLIENTE itemAntes, USUARIO usuario)
        {
            try
            {
                if (itemAntes.CATEGORIA_CLIENTE != null)
                {
                    itemAntes.CATEGORIA_CLIENTE = null;
                }
                //if (itemAntes.FILIAL != null)
                //{
                //    itemAntes.FILIAL = null;
                //}
                if (itemAntes.SEXO != null)
                {
                    itemAntes.SEXO = null;
                }
                if (itemAntes.TIPO_CONTRIBUINTE != null)
                {
                    itemAntes.TIPO_CONTRIBUINTE = null;
                }
                if (itemAntes.TIPO_PESSOA != null)
                {
                    itemAntes.TIPO_PESSOA = null;
                }
                if (itemAntes.UF != null)
                {
                    itemAntes.UF = null;
                }
                if (itemAntes.USUARIO != null)
                {
                    itemAntes.USUARIO = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCLIE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CLIENTE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CLIENTE>(itemAntes)
                };

                // Persiste
                item.TIPO_PESSOA = null;
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CLIENTE item, CLIENTE itemAntes)
        {
            try
            {
                item.UF = null;
                item.TIPO_PESSOA = null;
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CRM.Count > 0)
                {
                    if (item.CRM.Where(p => p.CRM1_IN_ATIVO == 1).ToList().Count > 0)
                    {
                        return 1;
                    }
                }
                if (item.CRM_PEDIDO_VENDA.Count > 0)
                {
                    if (item.CRM_PEDIDO_VENDA.Where(p => p.CRPV_IN_ATIVO == 1 || p.CRPV_IN_ATIVO == 2).ToList().Count > 0)
                    {
                        return 1;
                    }
                }

                // Acerta campos
                item.CLIE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCLIE",
                    LOG_TX_REGISTRO = "Nome: " + item.CLIE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CLIE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCLIE",
                    LOG_TX_REGISTRO = "Nome: " + item.CLIE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditContato(CLIENTE_CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnotacao(CLIENTE_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateMunicipio(MUNICIPIO item)
        {
            try
            {
                item.MUNI_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateMunicipio(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(CLIENTE_CONTATO item)
        {
            try
            {
                item.CLCO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditReferencia(CLIENTE_REFERENCIA item)
        {
            try
            {
                // Persiste
                return _baseService.EditReferencia(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateReferencia(CLIENTE_REFERENCIA item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateReferencia(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditFalha(CLIENTE_FALHA item)
        {
            try
            {
                // Persiste
                return _baseService.EditFalha(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateFalha(CLIENTE_FALHA item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateFalha(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateGrupo(GRUPO_CLIENTE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateGrupo(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditGrupo(GRUPO_CLIENTE item)
        {
            try
            {
                // Persiste
                return _baseService.EditGrupo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 AtualizarCategoriaClienteCalculo(Int32 idAss, CLIENTE cli)
        {
            // Recupera cliente
            CATEGORIA_CLIENTE cat = _ccService.GetItemById(cli.CACL_CD_ID.Value);
            List<CATEGORIA_CLIENTE> cats = _ccService.GetAllItens(idAss);
            Int32? maiorOrdem = cats.Max(p => p.CACL_IN_ORDEM.Value);
            Int32? menorOrdem = cats.Min(p => p.CACL_IN_ORDEM.Value);
            Int32? novaOrdem = 0;

            // Recupera propostas
            List<CRM_PEDIDO_VENDA> listaProp = _crmService.GetAllPedidos(idAss).Where(p => p.CLIE_CD_ID == cli.CLIE_CD_ID).ToList();
            listaProp = listaProp.Where(p => p.CRPV_DT_PEDIDO > DateTime.Today.Date.AddDays(-30)).ToList();

            Int32 props = listaProp.Count;
            Int32 aprovs = listaProp.Where(p => p.CRPV_IN_STATUS == 5).ToList().Count;
            Int32 reprovs = listaProp.Where(p => p.CRPV_IN_STATUS == 4).ToList().Count;

            // Verifica limites
            Int32 emiMax = 0;
            Int32 emiMin = 0;
            Int32 aprMax = 0;
            Int32 aprMin = 0;
            Int32 repMax = 0;
            Int32 repMin = 0;
            Int32 sobe = 0;
            Int32 desce = 0;

            if (props >= cat.CACL_IN_LIMITE_MAXIMO_EMITIDAS)
            {
                emiMax = 1;
            }
            if (props < cat.CACL_IN_LIMITE_MINIMO_EMITIDAS)
            {
                emiMin = 1;
            }
            if (aprovs >= cat.CACL_IN_LIMITE__MAXIMO_APROVADAS)
            {
                aprMax = 1;
            }
            if (aprovs < cat.CACL_IN_LIMITE_MINIMO_APROVADAS)
            {
                aprMin = 1;
            }
            if (reprovs >= cat.CACL_IN_LIMITE__MAXIMO_REPROVADAS)
            {
                repMax = 1;
            }
            if (reprovs < cat.CACL_IN_LIMITE_MINIMO_REPROVADAS)
            {
                repMin = 1;
            }

            // Calcula faixa
            if (emiMax == 1 & aprMax == 1 & repMin == 1)
            {
                sobe = 1;
            }
            if (emiMin == 1 || aprMin == 1 || repMax == 1)
            {
                desce = 1;
            }

            // Altera faixa
            if (sobe == 1)
            {
                novaOrdem = cat.CACL_IN_ORDEM + 1;
                if (novaOrdem > maiorOrdem)
                {
                    novaOrdem = cat.CACL_IN_ORDEM;
                }
                else
                {
                    Int32 k = 0;
                    while (k == 0)
                    {
                        CATEGORIA_CLIENTE catOrdem = cats.Where(p => p.CACL_IN_ORDEM == novaOrdem).FirstOrDefault();
                        if (catOrdem == null)
                        {
                            novaOrdem++;
                            if (novaOrdem > maiorOrdem)
                            {
                                novaOrdem = cat.CACL_IN_ORDEM;
                                k = 1;
                            }
                        }
                        else
                        {
                            cli.CACL_CD_ID = catOrdem.CACL_CD_ID;
                            Int32 volta = _baseService.Edit(cli);
                        }
                    }
                }
            }
            if (desce == 1)
            {
                novaOrdem = cat.CACL_IN_ORDEM - 1;
                if (novaOrdem < menorOrdem)
                {
                    novaOrdem = cat.CACL_IN_ORDEM;
                }
                else
                {
                    Int32 k = 0;
                    while (k == 0)
                    {
                        CATEGORIA_CLIENTE catOrdem = cats.Where(p => p.CACL_IN_ORDEM == novaOrdem).FirstOrDefault();
                        if (catOrdem == null)
                        {
                            novaOrdem--;
                            if (novaOrdem > menorOrdem)
                            {
                                novaOrdem = cat.CACL_IN_ORDEM;
                                k = 1;
                            }
                        }
                        else
                        {
                            cli.CACL_CD_ID = catOrdem.CACL_CD_ID;
                            Int32 volta = _baseService.Edit(cli);
                            k = 1;
                        }
                    }
                }
            }
            return 0;
        }

    }
}
