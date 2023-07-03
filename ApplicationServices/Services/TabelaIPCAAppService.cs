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
    public class TabelaIPCAAppService : AppServiceBase<TABELA_IPCA>, ITabelaIPCAAppService
    {
        private readonly ITabelaIPCAService _baseService;
        private readonly IConfiguracaoService _confService;

        public TabelaIPCAAppService(ITabelaIPCAService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<TABELA_IPCA> GetAllItens()
        {
            List<TABELA_IPCA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<TABELA_IPCA> GetAllItensAdm()
        {
            List<TABELA_IPCA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public TABELA_IPCA GetItemById(Int32 id)
        {
            TABELA_IPCA item = _baseService.GetItemById(id);
            return item;
        }

        public TABELA_IPCA CheckExist(TABELA_IPCA conta)
        {
            TABELA_IPCA item = _baseService.CheckExist(conta);
            return item;
        }

        public Int32 ExecuteFilter(DateTime? data, out List<TABELA_IPCA> objeto)
        {
            try
            {
                objeto = new List<TABELA_IPCA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(data);
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

        public Int32 ValidateCreate(TABELA_IPCA item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.IPCA_IN_ATIVO = 1;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddIPCA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IPCA>(item)
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

        public Int32 ValidateEdit(TABELA_IPCA item, TABELA_IPCA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditIPCA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IPCA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TABELA_IPCA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TABELA_IPCA item, TABELA_IPCA itemAntes)
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

        public Int32 ValidateDelete(TABELA_IPCA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.IPCA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelIPCA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IPCA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TABELA_IPCA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.IPCA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatIPCA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IPCA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
