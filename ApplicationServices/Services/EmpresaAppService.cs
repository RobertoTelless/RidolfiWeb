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
    public class EmpresaAppService : AppServiceBase<EMPRESA>, IEmpresaAppService
    {
        private readonly IEmpresaService _baseService;
        private readonly IMaquinaService _maqService;
        private readonly IPlataformaEntregaService _plaService;
        private readonly ITicketService _TktService;

        public EmpresaAppService(IEmpresaService baseService, IMaquinaService maqService, IPlataformaEntregaService plaService, ITicketService tktService) : base(baseService)
        {
            _baseService = baseService;
            _maqService = maqService;
            _plaService = plaService;
            _TktService = tktService;
        }

        public EMPRESA CheckExist(EMPRESA conta, Int32 idAss)
        {
            EMPRESA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<EMPRESA> GetAllItens(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<EMPRESA> GetAllItensAdm(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public EMPRESA GetItemById(Int32 id)
        {
            EMPRESA item = _baseService.GetItemById(id);
            return item;
        }

        public EMPRESA GetItemByAssinante(Int32 id)
        {
            EMPRESA item = _baseService.GetItemByAssinante(id);
            return item;
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

        public EMPRESA_ANEXO GetAnexoById(Int32 id)
        {
            EMPRESA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public EMPRESA_MAQUINA GetMaquinaById(Int32 id)
        {
            EMPRESA_MAQUINA lista = _baseService.GetMaquinaById(id);
            return lista;
        }

        public EMPRESA_PLATAFORMA GetPlataformaById(Int32 id)
        {
            EMPRESA_PLATAFORMA lista = _baseService.GetPlataformaById(id);
            return lista;
        }

        public REGIME_TRIBUTARIO GetRegimeById(Int32 id)
        {
            REGIME_TRIBUTARIO lista = _baseService.GetRegimeById(id);
            return lista;
        }

        public Tuple<Int32, List<EMPRESA>, Boolean> ExecuteFilterTuple(String nome, Int32 idAss)
        {
            try
            {
                List<EMPRESA> objeto = new List<EMPRESA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, idAss);
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

        public List<MAQUINA> GetAllMaquinas(Int32 idAss)
        {
            return _baseService.GetAllMaquinas(idAss);
        }

        public List<REGIME_TRIBUTARIO> GetAllRegimes()
        {
            return _baseService.GetAllRegimes();
        }

        public Int32 ValidateCreate(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.EMPR_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.EMPR_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item)
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

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes, USUARIO usuario)
        {
            try
            {
                // Serialização
                String atual = item.ASSI_CD_ID.ToString() + "|" + item.EMPR_NM_NOME + "|" + item.EMPR_VL_PATRIMONIO_LIQUIDO.ToString() + "|" + item.EMPR_PC_ANTECIPACAO.ToString() + "|" + item.EMPR_IN_PAGA_COMISSAO.ToString() + "|" + item.EMPR_VL_IMPOSTO_MEI.ToString() + "|" + item.EMPR_NM_RAZAO + "|" + item.EMPR_NR_CNPJ;
                String antes = itemAntes.ASSI_CD_ID.ToString() + "|" + itemAntes.EMPR_NM_NOME + "|" + itemAntes.EMPR_VL_PATRIMONIO_LIQUIDO.ToString() + "|" + itemAntes.EMPR_PC_ANTECIPACAO.ToString() + "|" + itemAntes.EMPR_IN_PAGA_COMISSAO.ToString() + "|" + itemAntes.EMPR_VL_IMPOSTO_MEI.ToString() + "|" + itemAntes.EMPR_NM_RAZAO + "|" + itemAntes.EMPR_NR_CNPJ;

                // Monta Log                
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = atual,
                    LOG_TX_REGISTRO_ANTES = antes
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes)
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

        public Int32 ValidateDelete(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.EMPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleEMPR",
                    LOG_TX_REGISTRO = "Empresa: " + item.EMPR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.EMPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatEMPR",
                    LOG_TX_REGISTRO = "Empresa: " + item.EMPR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditMaquina(EMPRESA_MAQUINA item)
        {
            try
            {
                // Persiste
                return _baseService.EditMaquina(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateMaquina(EMPRESA_MAQUINA item, Int32 idAss)
        {
            try
            {
                // Verifica existencia
                EMPRESA_MAQUINA volta1 = _baseService.CheckExistMaquina(item, idAss);
                if (volta1 != null)
                {
                    return 1;
                }

                // Persiste
                Int32 volta = _baseService.CreateMaquina(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina)
        {
            return _baseService.GetByEmpresaMaquina(empresa, maquina);
        }

        public Int32 ValidateEditPlataforma(EMPRESA_PLATAFORMA item)
        {
            try
            {
                // Persiste
                return _baseService.EditPlataforma(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePlataforma(EMPRESA_PLATAFORMA item, Int32 idAss)
        {
            try
            {
                // Verifica existencia
                EMPRESA_PLATAFORMA volta1 = _baseService.CheckExistPlataforma(item, idAss);
                if (volta1 != null)
                {
                    return 1;
                }

                // Persiste
                Int32 volta = _baseService.CreatePlataforma(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EMPRESA_PLATAFORMA GetByEmpresaPlataforma(Int32 empresa, Int32 plataforma)
        {
            return _baseService.GetByEmpresaPlataforma(empresa, plataforma);
        }

        public EMPRESA_CUSTO_VARIAVEL CheckExistCustoVariavel(EMPRESA_CUSTO_VARIAVEL conta, Int32 idAss)
        {
            EMPRESA_CUSTO_VARIAVEL item = _baseService.CheckExistCustoVariavel(conta, idAss);
            return item;
        }

        public EMPRESA_CUSTO_VARIAVEL GetCustoVariavelById(Int32 id)
        {
            EMPRESA_CUSTO_VARIAVEL lista = _baseService.GetCustoVariavelById(id);
            return lista;
        }

        public Int32 ValidateEditCustoVariavel(EMPRESA_CUSTO_VARIAVEL item)
        {
            try
            {
                // Verifica percentuais
                Int32? flagMenos = 0;
                EMPRESA emp = _baseService.GetItemById(item.EMPR_CD_ID.Value);
                List<EMPRESA_CUSTO_VARIAVEL> custos = emp.EMPRESA_CUSTO_VARIAVEL.ToList();
                Int32? perc = custos.Sum(p => p.EMCV_PC_PERCENTUAL_VENDA);
                if (perc > 100)
                {
                    return 1;
                }
                if (perc < 100)
                {
                    flagMenos = 1;
                }

                // Persiste
                Int32 volta =_baseService.EditCustoVariavel(item);
                if (flagMenos == 1)
                {
                    return 2;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateCustoVariavel(EMPRESA_CUSTO_VARIAVEL item, Int32 idAss)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateCustoVariavel(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EMPRESA_TICKET CheckExistTicket(EMPRESA_TICKET conta, Int32 idAss)
        {
            EMPRESA_TICKET item = _baseService.CheckExistTicket(conta, idAss);
            return item;
        }

        public EMPRESA_TICKET GetTicketById(Int32 id)
        {
            EMPRESA_TICKET lista = _baseService.GetTicketById(id);
            return lista;
        }

        public Int32 ValidateEditTicket(EMPRESA_TICKET item)
        {
            try
            {
                // Persiste
                return _baseService.EditTicket(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateTicket(EMPRESA_TICKET item, Int32 idAss)
        {
            try
            {
                // Verifica existencia
                EMPRESA_TICKET volta1 = _baseService.CheckExistTicket(item, idAss);
                if (volta1 != null)
                {
                    return 1;
                }

                // Persiste
                Int32 volta = _baseService.CreateTicket(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EMPRESA_TICKET GetByEmpresaTicket(Int32 empresa, Int32 ticket)
        {
            return _baseService.GetByEmpresaTicket(empresa, ticket);
        }

        public CUSTO_VARIAVEL_CALCULO GetCustoVariavelCalculoById(Int32 id)
        {
            CUSTO_VARIAVEL_CALCULO lista = _baseService.GetCustoVariavelCalculoById(id);
            return lista;
        }

        public Int32 ValidateEditCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item)
        {
            try
            {
                // Persiste
                return _baseService.EditCustoVariavelCalculo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateCustoVariavelCalculo(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<CUSTO_VARIAVEL_CALCULO>, Boolean> ExecuteFilterCalculo(DateTime? data, Int32? idAss)
        {
            try
            {
                List<CUSTO_VARIAVEL_CALCULO> objeto = new List<CUSTO_VARIAVEL_CALCULO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterCalculo(data, idAss);
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

        public List<CUSTO_VARIAVEL_CALCULO> GetAllCalculo(Int32 id)
        {
            return _baseService.GetAllCalculo(id);
        }

        public CUSTO_VARIAVEL_HISTORICO GetCustoVariavelHistoricoById(Int32 id)
        {
            CUSTO_VARIAVEL_HISTORICO lista = _baseService.GetCustoVariavelHistoricoById(id);
            return lista;
        }

        public Int32 ValidateEditCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item)
        {
            try
            {
                // Persiste
                return _baseService.EditCustoVariavelHistorico(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateCustoVariavelHistorico(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<CUSTO_VARIAVEL_HISTORICO>, Boolean> ExecuteFilterHistorico(DateTime? data, Int32? idAss)
        {
            try
            {
                List<CUSTO_VARIAVEL_HISTORICO> objeto = new List<CUSTO_VARIAVEL_HISTORICO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterHistorico(data, idAss);
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

        public List<CUSTO_VARIAVEL_HISTORICO> GetAllHistorico(Int32 id)
        {
            return _baseService.GetAllHistorico(id);
        }

        public List<EMPRESA_CUSTO_VARIAVEL> GetAllCustoVariavel(Int32 id)
        {
            return _baseService.GetAllCustoVariavel(id);
        }

        public CUSTO_HISTORICO GetCustoHistoricoById(Int32 id)
        {
            CUSTO_HISTORICO lista = _baseService.GetCustoHistoricoById(id);
            return lista;
        }

        public Int32 ValidateCreateCustoHistorico(CUSTO_HISTORICO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateCustoHistorico(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditCustoHistorico(CUSTO_HISTORICO item)
        {
            try
            {
                // Persiste
                return _baseService.EditCustoHistorico(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<CUSTO_HISTORICO>, Boolean> ExecuteFilterCustoHistorico(DateTime? data, Int32? idAss)
        {
            try
            {
                List<CUSTO_HISTORICO> objeto = new List<CUSTO_HISTORICO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterCustoHistorico(data, idAss);
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

        public List<CUSTO_HISTORICO> GetAllCustoHistorico(Int32 id)
        {
            return _baseService.GetAllCustoHistorico(id);
        }

        public COMISSAO GetComissaoById(Int32 id)
        {
            COMISSAO lista = _baseService.GetComissaoById(id);
            return lista;
        }

        public Int32 ValidateCreateComissao(COMISSAO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateComissao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditComissao(COMISSAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditComissao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<COMISSAO>, Boolean> ExecuteFilterComissao(DateTime? data, Int32? idAss)
        {
            try
            {
                List<COMISSAO> objeto = new List<COMISSAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterComissao(data, idAss);
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

        public List<COMISSAO> GetAllComissao(Int32 id)
        {
            return _baseService.GetAllComissao(id);
        }

    }
}
