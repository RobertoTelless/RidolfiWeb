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
    public class AssinanteAppService : AppServiceBase<ASSINANTE>, IAssinanteAppService
    {
        private readonly IAssinanteService _baseService;

        public AssinanteAppService(IAssinanteService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public ASSINANTE CheckExist(ASSINANTE conta)
        {
            ASSINANTE item = _baseService.CheckExist(conta);
            return item;
        }

        public List<ASSINANTE> GetAllItens()
        {
            List<ASSINANTE> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<ASSINANTE_PAGAMENTO> GetAllPagamentos()
        {
            List<ASSINANTE_PAGAMENTO> lista = _baseService.GetAllPagamentos();
            return lista;
        }

        public List<ASSINANTE_PLANO> GetAllAssPlanos()
        {
            List<ASSINANTE_PLANO> lista = _baseService.GetAllAssPlanos();
            return lista;
        }

        public List<ASSINANTE> GetAllItensAdm()
        {
            List<ASSINANTE> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public ASSINANTE GetItemById(Int32 id)
        {
            ASSINANTE item = _baseService.GetItemById(id);
            return item;
        }

        public UF GetUFBySigla(String sigla)
        {
            UF item = _baseService.GetUFBySigla(sigla);
            return item;
        }

        public CONFIGURACAO_CHAVES GetChaves(Int32 id)
        {
            CONFIGURACAO_CHAVES item = _baseService.GetChaves(id);
            return item;
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _baseService.GetAllTiposPessoa();
        }

        public List<PLANO> GetAllPlanos()
        {
            return _baseService.GetAllPlanos();
        }

        public ASSINANTE_ANEXO GetAnexoById(Int32 id)
        {
            return _baseService.GetAnexoById(id);
        }

        public ASSINANTE_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _baseService.GetAnotacaoById(id);
        }

        public ASSINANTE_PAGAMENTO GetPagtoById(Int32 id)
        {
            return _baseService.GetPagtoById(id);
        }

        public List<UF> GetAllUF()
        {
            return _baseService.GetAllUF();
        }

        public ASSINANTE_PLANO GetPlanoById(Int32 id)
        {
            ASSINANTE_PLANO lista = _baseService.GetPlanoById(id);
            return lista;
        }

        public PLANO GetPlanoBaseById(Int32 id)
        {
            PLANO lista = _baseService.GetPlanoBaseById(id);
            return lista;
        }

        public Tuple<Int32, List<ASSINANTE>, Boolean> ExecuteFilter(Int32? tipo, String nome, String cpf, String cnpj, String cidade, Int32? uf, Int32? status)
        {
            try
            {
                List<ASSINANTE> objeto = new List<ASSINANTE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, nome, cpf, cnpj, cidade, uf, status);
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

        public Int32 ExecuteFilterAtraso(String nome, String cpf, String cnpj, String cidade, Int32? uf, out List<ASSINANTE_PAGAMENTO> objeto)
        {
            try
            {
                objeto = new List<ASSINANTE_PAGAMENTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterAtraso(nome, cpf, cnpj, cidade, uf);
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

        public Int32 ExecuteFilterVencidos(String nome, String cpf, String cnpj, String cidade, Int32? uf, out List<ASSINANTE_PLANO> objeto)
        {
            try
            {
                objeto = new List<ASSINANTE_PLANO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterVencidos(nome, cpf, cnpj, cidade, uf);
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

        public Int32 ExecuteFilterVencer30(String nome, String cpf, String cnpj, String cidade, Int32? uf, out List<ASSINANTE_PLANO> objeto)
        {
            try
            {
                objeto = new List<ASSINANTE_PLANO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterVencer30(nome, cpf, cnpj, cidade, uf);
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

        public Int32 ValidateCreate(ASSINANTE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.ASSI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddASSI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ASSINANTE>(item)
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

        public Int32 ValidateEdit(ASSINANTE item, ASSINANTE itemAntes, USUARIO usuario)
        {
            try
            {
                // Serialização 
                String texto = item.ASSI_NM_NOME + "|" + item.ASSI_NM_RAZAO_SOCIAL == null ? item.ASSI_NM_RAZAO_SOCIAL : " " + "|" + item.ASSI_NR_CPF == null ? item.ASSI_NR_CPF : " " + "|" + item.ASSI_NR_CNPJ == null ? item.ASSI_NR_CNPJ : " " + "|" + item.ASSI_NM_BAIRRO == null ? item.ASSI_NM_BAIRRO : " " + "|" + item.ASSI_NM_CIDADE == null ? item.ASSI_NM_CIDADE : " " + "|" + item.ASSI_NM_COMPLEMENTO == null ? item.ASSI_NM_COMPLEMENTO : " " + "|" + item.ASSI_NM_EMAIL + "|" + item.ASSI_NM_ENDERECO == null ? item.ASSI_NM_ENDERECO : " " + "|" + item.ASSI_NR_CEP == null ? item.ASSI_NR_CEP : " " + "|" + item.ASSI_NR_NUMERO == null ? item.ASSI_NR_NUMERO : " " + "|" + item.ASSI_NR_TELEFONE == null ? item.ASSI_NR_TELEFONE : " " + "|" + item.ASSI_NR_CELULAR == null ? item.ASSI_NR_CELULAR : " " + "|" + item.TIPE_CD_ID.ToString();
                String textoAntes = itemAntes.ASSI_NM_NOME + "|" + itemAntes.ASSI_NM_RAZAO_SOCIAL == null ? itemAntes.ASSI_NM_RAZAO_SOCIAL : " " + "|" + itemAntes.ASSI_NR_CPF == null ? itemAntes.ASSI_NR_CPF : " " + "|" + itemAntes.ASSI_NR_CNPJ == null ? itemAntes.ASSI_NR_CNPJ : " " + "|" + itemAntes.ASSI_NM_BAIRRO == null ? itemAntes.ASSI_NM_BAIRRO : " " + "|" + itemAntes.ASSI_NM_CIDADE == null ? itemAntes.ASSI_NM_CIDADE : " " + "|" + itemAntes.ASSI_NM_COMPLEMENTO == null ? itemAntes.ASSI_NM_COMPLEMENTO : " " + "|" + itemAntes.ASSI_NM_EMAIL + "|" + itemAntes.ASSI_NM_ENDERECO == null ? itemAntes.ASSI_NM_ENDERECO : " " + "|" + itemAntes.ASSI_NR_CEP == null ? itemAntes.ASSI_NR_CEP : " " + "|" + itemAntes.ASSI_NR_NUMERO == null ? itemAntes.ASSI_NR_NUMERO : " " + "|" + itemAntes.ASSI_NR_TELEFONE == null ? itemAntes.ASSI_NR_TELEFONE : " " + "|" + itemAntes.ASSI_NR_CELULAR == null ? itemAntes.ASSI_NR_CELULAR : " " + "|" + itemAntes.TIPE_CD_ID.ToString();

                // Monta Log
                LOG log = new LOG()
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditASSI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = texto,
                    LOG_TX_REGISTRO_ANTES = textoAntes
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ASSINANTE item)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(ASSINANTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.USUARIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.ASSI_IN_ATIVO = 0;
                item.ASSI_IN_STATUS = 2;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelASSI",
                    LOG_TX_REGISTRO = "Assinante: " + item.ASSI_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ASSINANTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ASSI_IN_ATIVO = 1;
                item.ASSI_IN_STATUS = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatASSI",
                    LOG_TX_REGISTRO = "Assinante: " + item.ASSI_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditPagto(ASSINANTE_PAGAMENTO item)
        {
            try
            {
                // Persiste
                return _baseService.EditPagto(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePagto(ASSINANTE_PAGAMENTO item)
        {
            try
            {
                // Criticas
                if (item.ASPA_DT_VENCIMENTO < DateTime.Today.Date)
                {
                    return 1;

                }
                if (item.ASPA_DT_PAGAMENTO > DateTime.Today.Date)
                {
                    return 2;

                }

                // Verifica existencia
                PLANO plano = _baseService.GetPlanoBaseById(item.PLAN_CD_ID.Value);
                Int32? dias = plano.PLANO_PERIODICIDADE.PLPE_NR_DIAS;
                List<ASSINANTE_PAGAMENTO> pags = _baseService.GetAllPagamentos().Where(p => p.ASSI_CD_ID == item.ASSI_CD_ID).ToList();

                if (dias == 30)
                {
                    pags = pags.Where(p => p.ASPA_DT_VENCIMENTO.Value.Month == item.ASPA_DT_VENCIMENTO.Value.Month & p.ASPA_DT_VENCIMENTO.Value.Year == item.ASPA_DT_VENCIMENTO.Value.Year).ToList();
                    if (pags.Count > 0)
                    {
                        return 3;
                    }
                }

                // Completa objeto
                item.ASPA_IN_ATIVO = 1;
                item.ASPA_IN_PAGO = 1;

                // Persiste
                Int32 volta = _baseService.CreatePagto(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditPlano(ASSINANTE_PLANO item)
        {
            try
            {
                // Persiste
                return _baseService.EditPlano(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePlano(ASSINANTE_PLANO item)
        {
            try
            {
                // Critica e validade
                if (item.ASPL_DT_INICIO > DateTime.Today.Date)
                {
                    return 1;
                }
                if (item.ASPL_DT_VALIDADE < DateTime.Today.Date)
                {
                    return 2;
                }
                if (item.ASPL_DT_VALIDADE <= item.ASPL_DT_INICIO)
                {
                    return 3;
                }

                // Verfica existencia
                ASSINANTE assi = _baseService.GetItemById(item.ASSI_CD_ID);
                List<ASSINANTE_PLANO> lista = assi.ASSINANTE_PLANO.Where(p => p.PLAN_CD_ID == item.PLAN_CD_ID & p.ASPL_IN_ATIVO == 1).ToList();
                if (lista.Count > 0)
                {
                    return 4;
                }

                // Persiste
                Int32 volta = _baseService.CreatePlano(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ASSINANTE_PLANO GetByAssPlan(Int32 plan, Int32 assi)
        {
            return _baseService.GetByAssPlan(plan, assi);
        }

    }
}
