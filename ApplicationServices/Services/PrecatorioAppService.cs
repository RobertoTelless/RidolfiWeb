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
    public class PrecatorioAppService : AppServiceBase<PRECATORIO>, IPrecatorioAppService
    {
        private readonly IPrecatorioService _baseService;
        private readonly IConfiguracaoService _confService;
        private readonly ICRMAppService _crmService;

        public PrecatorioAppService(IPrecatorioService baseService, IConfiguracaoService confService, ICRMAppService crmService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            _crmService = crmService;   
        }

        public List<PRECATORIO> GetAllItens()
        {
            List<PRECATORIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PRECATORIO> GetAllItensAdm()
        {
            List<PRECATORIO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public PRECATORIO GetItemById(Int32 id)
        {
            PRECATORIO item = _baseService.GetItemById(id);
            return item;
        }

        public PRECATORIO CheckExist(PRECATORIO conta)
        {
            PRECATORIO item = _baseService.CheckExist(conta);
            return item;
        }

        public NATUREZA CheckExistNatureza(String conta)
        {
            NATUREZA item = _baseService.CheckExistNatureza(conta);
            return item;
        }

        public PRECATORIO CheckExist(String conta)
        {
            PRECATORIO item = _baseService.CheckExist(conta);
            return item;
        }

        public CONTATO CheckExistContato(String prot)
        {
            CONTATO item = _baseService.CheckExistContato(prot);
            return item;
        }

        public List<BENEFICIARIO> GetAllBeneficiarios()
        {
            List<BENEFICIARIO> lista = _baseService.GetAllBeneficiarios();
            return lista;
        }

        public List<TRF> GetAllTRF()
        {
            List<TRF> lista = _baseService.GetAllTRF();
            return lista;
        }

        public List<HONORARIO> GetAllAdvogados()
        {
            List<HONORARIO> lista = _baseService.GetAllAdvogados();
            return lista;
        }

        public List<BANCO> GetAllBancos()
        {
            List<BANCO> lista = _baseService.GetAllBancos();
            return lista;
        }

        public List<NATUREZA> GetAllNaturezas()
        {
            List<NATUREZA> lista = _baseService.GetAllNaturezas();
            return lista;
        }

        public List<PRECATORIO_ESTADO> GetAllEstados()
        {
            List<PRECATORIO_ESTADO> lista = _baseService.GetAllEstados();
            return lista;
        }

        public PRECATORIO_ANEXO GetAnexoById(Int32 id)
        {
            PRECATORIO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public PRECATORIO_ANOTACAO GetComentarioById(Int32 id)
        {
            PRECATORIO_ANOTACAO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? trf, Int32? beneficiario, Int32? advogado, Int32? natureza, Int32? estado, String nome, String ano, Int32? crm, Int32? pesquisa, Decimal? valor1, Decimal? valor2, Int32? situacao, out List<PRECATORIO> objeto)
        {
            try
            {
                objeto = new List<PRECATORIO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(trf, beneficiario, advogado, natureza, estado, nome, ano, crm, pesquisa, valor1, valor2, situacao);
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

        public Int32 ValidateCreate(PRECATORIO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.PREC_IN_ATIVO = 1;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPREC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRECATORIO>(item)
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

        public Int32 ValidateEdit(PRECATORIO item, PRECATORIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPREC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRECATORIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PRECATORIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PRECATORIO item, PRECATORIO itemAntes)
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

        public Int32 ValidateDelete(PRECATORIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CRM.Count > 0)
                {
                    return 1;
                }
                if (item.CLIENTE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PREC_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPREC",
                    LOG_TX_REGISTRO = item.PREC_NM_PRECATORIO
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PRECATORIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PREC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPREC",
                    LOG_TX_REGISTRO = item.PREC_NM_PRECATORIO
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateFalha(PRECATORIO_FALHA item)
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

        public Int32 ValidateCreateFalhaContato(CONTATO_FALHA item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateFalhaContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<VOLTA_PESQUISA> PesquisarTudo(String parm, Int32 idAss)
        {
            // Critica
            if (parm == null)
            {
                return null;
            }

            // Busca em Beneficiarios
            List<BENEFICIARIO> listaBenef = _baseService.GetAllBeneficiarios().ToList();
            listaBenef = listaBenef.Where(p => p.BENE_NM_NOME.Contains(parm) || p.MOME_NM_RAZAO_SOCIAL.Contains(parm) || p.BENE_NR_CPF == parm || p.BENE_NR_CNPJ == parm).ToList();

            // Busca em Precatorios 
            List<PRECATORIO> listaPrec = _baseService.GetAllItens();
            listaPrec = listaPrec.Where(p => p.PREC_NM_REQUERENTE.Contains(parm) || p.PREC_NM_DEPRECANTE.Contains(parm) || p.PREC_NM_PRECATORIO.Contains(parm)).ToList();

            // Busca em Advogados
            List<HONORARIO> listaHon= _baseService.GetAllAdvogados();
            listaHon = listaHon.Where(p => p.HONO_NM_NOME.Contains(parm) || p.HONO_NM_RAZAO_SOCIAL.Contains(parm) || p.HONO_NR_CPF == parm || p.HONO_NR_OAB == parm).ToList();

            // Busca em Processos
            List<CRM> listaCRM = _crmService.GetAllItens(idAss);
            listaCRM = listaCRM.Where(p => p.CRM1_NM_NOME.Contains(parm) || p.PRECATORIO.PREC_NM_PRECATORIO.Contains(parm)).ToList();

            // Busca em Ações
            List<CRM_ACAO> listaAcao = _crmService.GetAllAcoes(idAss);
            listaAcao = listaAcao.Where(p => p.CRAC_NM_TITULO.Contains(parm)).ToList();

            // Busca em Propostas
            List<CRM_PEDIDO_VENDA> listaProp = _crmService.GetAllPedidos(idAss);
            listaProp = listaProp.Where(p => p.CRPV_NM_NOME.Contains(parm) || p.CRPV_IN_NUMERO_GERADO.ToString() == parm || p.CRPV_TX_CONDICOES_COMERCIAIS.Contains(parm)).ToList();

            // Prepara lista de retorno
            List<VOLTA_PESQUISA> listaVolta = new List<VOLTA_PESQUISA>();
            foreach (BENEFICIARIO item in listaBenef)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 1;
                volta.PEGR_CD_ITEM = item.BENE_CD_ID;
                volta.PEGR_NM_NOME1 = item.BENE_NM_NOME;
                if (item.TIPE_CD_ID == 1)
                {
                    volta.PEGR_NM_NOME2 = item.BENE_NR_RG;
                    volta.PEGR_NM_NOME3 = item.BENE_NR_CPF;
                }
                if (item.TIPE_CD_ID == 2)
                {
                    volta.PEGR_NM_NOME2 = item.MOME_NM_RAZAO_SOCIAL;
                    volta.PEGR_NM_NOME3 = item.BENE_NR_CNPJ;
                }
                volta.PEGR_NM_NOME4 = null;
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (PRECATORIO item in listaPrec)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 2;
                volta.PEGR_CD_ITEM = item.PREC_CD_ID;
                volta.PEGR_NM_NOME1 = item.PREC_NM_PRECATORIO;
                volta.PEGR_NM_NOME2 = item.PREC_DT_BEN_DATABASE.Value.ToShortTimeString() ;
                volta.PEGR_NM_NOME3 = item.PREC_NM_DEPRECANTE;
                volta.PEGR_NM_NOME4 = item.PREC_IN_SITUACAO_ATUAL.ToString();
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (HONORARIO item in listaHon)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 3;
                volta.PEGR_CD_ITEM = item.HONO_CD_ID;
                volta.PEGR_NM_NOME1 = item.HONO_NM_NOME;
                if (item.TIPE_CD_ID == 1)
                {
                    volta.PEGR_NM_NOME2 = item.HONO_NR_OAB;
                    volta.PEGR_NM_NOME3 = item.HONO_NR_CPF;
                }
                if (item.TIPE_CD_ID == 2)
                {
                    volta.PEGR_NM_NOME2 = item.HONO_NR_OAB;
                    volta.PEGR_NM_NOME3 = item.HONO_NR_CNPJ;
                }
                volta.PEGR_NM_NOME4 = null;
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (CRM item in listaCRM)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 4;
                volta.PEGR_CD_ITEM = item.CRM1_CD_ID;
                volta.PEGR_NM_NOME1 = item.CRM1_NM_NOME;
                volta.PEGR_NM_NOME2 = item.CRM1_DT_CRIACAO.Value.ToShortDateString();
                volta.PEGR_NM_NOME4 = item.CRM1_IN_STATUS.ToString();
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (CRM_ACAO item in listaAcao)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 5;
                volta.PEGR_CD_ITEM = item.CRAC_CD_ID;
                volta.PEGR_NM_NOME1 = item.CRAC_NM_TITULO;
                volta.PEGR_NM_NOME2 = item.CRAC_DT_CRIACAO.Value.ToShortDateString();
                volta.PEGR_NM_NOME4 = item.CRAC_IN_STATUS.ToString();
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            foreach (CRM_PEDIDO_VENDA item in listaProp)
            {
                VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                volta.PEGR_IN_TIPO = 6;
                volta.PEGR_CD_ITEM = item.CRPV_CD_ID;
                volta.PEGR_NM_NOME1 = item.CRPV_NM_NOME;
                volta.PEGR_NM_NOME2 = item.CRPV_DT_PEDIDO.ToShortDateString();
                volta.PEGR_NM_NOME4 = item.CRPV_IN_STATUS.ToString();
                volta.ASSI_CD_ID = idAss;
                listaVolta.Add(volta);
            }
            return listaVolta;
        }

    }
}
