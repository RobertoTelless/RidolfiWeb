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
    public class EscolaridadeAppService : AppServiceBase<ESCOLARIDADE>, IEscolaridadeAppService
    {
        private readonly IEscolaridadeService _baseService;

        public EscolaridadeAppService(IEscolaridadeService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<ESCOLARIDADE> GetAllItens()
        {
            List<ESCOLARIDADE> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<ESCOLARIDADE> GetAllItensAdm()
        {
            List<ESCOLARIDADE> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public ESCOLARIDADE GetItemById(Int32 id)
        {
            ESCOLARIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public ESCOLARIDADE CheckExist(ESCOLARIDADE conta)
        {
            ESCOLARIDADE item = _baseService.CheckExist(conta);
            return item;
        }

        public Int32 ValidateCreate(ESCOLARIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.ESCO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddESCO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ESCOLARIDADE>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(ESCOLARIDADE item, ESCOLARIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditESCO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ESCOLARIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<ESCOLARIDADE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(ESCOLARIDADE item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.BENEFICIARIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.ESCO_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelESCO",
                    LOG_TX_REGISTRO = item.ESCO_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ESCOLARIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ESCO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatESCO",
                    LOG_TX_REGISTRO = item.ESCO_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
