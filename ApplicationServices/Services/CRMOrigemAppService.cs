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
    public class CRMOrigemAppService : AppServiceBase<CRM_ORIGEM>, ICRMOrigemAppService
    {
        private readonly ICRMOrigemService _baseService;

        public CRMOrigemAppService(ICRMOrigemService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<CRM_ORIGEM> GetAllItens(Int32 idAss)
        {
            List<CRM_ORIGEM> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CRM_ORIGEM> GetAllItensAdm(Int32 idAss)
        {
            List<CRM_ORIGEM> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CRM_ORIGEM GetItemById(Int32 id)
        {
            CRM_ORIGEM item = _baseService.GetItemById(id);
            return item;
        }

        public CRM_ORIGEM CheckExist(CRM_ORIGEM conta, Int32 idAss)
        {
            CRM_ORIGEM item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(CRM_ORIGEM item, USUARIO usuario)
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
                    item.CROR_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddCROR",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_ORIGEM>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.CROR_IN_ATIVO = 1;


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


        public Int32 ValidateEdit(CRM_ORIGEM item, CRM_ORIGEM itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCROR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_ORIGEM>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CRM_ORIGEM>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CRM_ORIGEM item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CRM.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CROR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCROR",
                    LOG_TX_REGISTRO = item.CROR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CRM_ORIGEM item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CROR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCROR",
                    LOG_TX_REGISTRO = item.CROR_NM_NOME
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
