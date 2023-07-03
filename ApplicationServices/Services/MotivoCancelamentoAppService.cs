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
    public class MotivoCancelamentoAppService : AppServiceBase<MOTIVO_CANCELAMENTO>, IMotivoCancelamentoAppService
    {
        private readonly IMotivoCancelamentoService _baseService;

        public MotivoCancelamentoAppService(IMotivoCancelamentoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<MOTIVO_CANCELAMENTO> GetAllItens(Int32 idAss)
        {
            List<MOTIVO_CANCELAMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<MOTIVO_CANCELAMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<MOTIVO_CANCELAMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MOTIVO_CANCELAMENTO GetItemById(Int32 id)
        {
            MOTIVO_CANCELAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public MOTIVO_CANCELAMENTO CheckExist(MOTIVO_CANCELAMENTO conta, Int32 idAss)
        {
            MOTIVO_CANCELAMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(MOTIVO_CANCELAMENTO item, USUARIO usuario)
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
                    item.MOCA_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddMOCA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<MOTIVO_CANCELAMENTO>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.MOCA_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(MOTIVO_CANCELAMENTO item, MOTIVO_CANCELAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditMOCA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOTIVO_CANCELAMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<MOTIVO_CANCELAMENTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(MOTIVO_CANCELAMENTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CRM.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.MOCA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelMOCA",
                    LOG_TX_REGISTRO = item.MOCA_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(MOTIVO_CANCELAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MOCA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatMOCA",
                    LOG_TX_REGISTRO = item.MOCA_NM_NOME
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
