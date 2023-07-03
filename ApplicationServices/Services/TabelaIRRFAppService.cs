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
    public class TabelaIRRFAppService : AppServiceBase<TABELA_IRRF>, ITabelaIRRFAppService
    {
        private readonly ITabelaIRRFService _baseService;
        private readonly IConfiguracaoService _confService;

        public TabelaIRRFAppService(ITabelaIRRFService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<TABELA_IRRF> GetAllItens()
        {
            List<TABELA_IRRF> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<TABELA_IRRF> GetAllItensAdm()
        {
            List<TABELA_IRRF> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public TABELA_IRRF GetItemById(Int32 id)
        {
            TABELA_IRRF item = _baseService.GetItemById(id);
            return item;
        }

        public TABELA_IRRF CheckExist(TABELA_IRRF conta)
        {
            TABELA_IRRF item = _baseService.CheckExist(conta);
            return item;
        }

        public Int32 ValidateCreate(TABELA_IRRF item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.IRRF_IN_ATIVO = 1;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddIRRF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IRRF>(item)
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

        public Int32 ValidateEdit(TABELA_IRRF item, TABELA_IRRF itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditIRRF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IRRF>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TABELA_IRRF>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TABELA_IRRF item, TABELA_IRRF itemAntes)
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

        public Int32 ValidateDelete(TABELA_IRRF item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.IRRF_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelIRRF",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IRRF>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TABELA_IRRF item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.IRRF_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatIRRF",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TABELA_IRRF>(item)
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
