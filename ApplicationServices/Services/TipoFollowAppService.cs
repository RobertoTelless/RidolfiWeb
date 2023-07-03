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
    public class TipoFollowAppService : AppServiceBase<TIPO_FOLLOW>, ITipoFollowAppService
    {
        private readonly ITipoFollowService _baseService;

        public TipoFollowAppService(ITipoFollowService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_FOLLOW> GetAllItens(Int32 idAss)
        {
            List<TIPO_FOLLOW> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_FOLLOW> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_FOLLOW> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_FOLLOW GetItemById(Int32 id)
        {
            TIPO_FOLLOW item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_FOLLOW CheckExist(TIPO_FOLLOW conta, Int32 idAss)
        {
            TIPO_FOLLOW item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_FOLLOW item, USUARIO usuario)
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
                    item.TIFL_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTIFL",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_FOLLOW>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TIFL_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(TIPO_FOLLOW item, TIPO_FOLLOW itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditTIFL",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_FOLLOW>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_FOLLOW>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_FOLLOW item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CRM_FOLLOW.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIFL_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIFL",
                    LOG_TX_REGISTRO = item.TIFL_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_FOLLOW item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIFL_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTIFL",
                    LOG_TX_REGISTRO = item.TIFL_NM_NOME
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
