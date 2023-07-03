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
    public class TipoItemPesquisaAppService : AppServiceBase<TIPO_ITEM_PESQUISA>, ITipoItemPesquisaAppService
    {
        private readonly ITipoItemPesquisaService _baseService;

        public TipoItemPesquisaAppService(ITipoItemPesquisaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_ITEM_PESQUISA> GetAllItens(Int32 idAss)
        {
            List<TIPO_ITEM_PESQUISA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_ITEM_PESQUISA> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_ITEM_PESQUISA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_ITEM_PESQUISA GetItemById(Int32 id)
        {
            TIPO_ITEM_PESQUISA item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_ITEM_PESQUISA CheckExist(TIPO_ITEM_PESQUISA conta, Int32 idAss)
        {
            TIPO_ITEM_PESQUISA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_ITEM_PESQUISA item, USUARIO usuario)
        {
            try
            {
                if (usuario != null)
                {
                    // Verifica existencia prévia
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }

                    // Completa objeto
                    item.TIIT_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTIIT",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_ITEM_PESQUISA>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TIIT_IN_ATIVO = 1;

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(TIPO_ITEM_PESQUISA item, TIPO_ITEM_PESQUISA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditTIIT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_ITEM_PESQUISA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_ITEM_PESQUISA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_ITEM_PESQUISA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PESQUISA_ITEM.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIIT_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIIT",
                    LOG_TX_REGISTRO = item.TIIT_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_ITEM_PESQUISA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIIT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTIIT",
                    LOG_TX_REGISTRO = item.TIIT_NM_NOME
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
