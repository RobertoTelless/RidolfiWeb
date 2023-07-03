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
    public class MotivoEncerramentoAppService : AppServiceBase<MOTIVO_ENCERRAMENTO>, IMotivoEncerramentoAppService
    {
        private readonly IMotivoEncerramentoService _baseService;

        public MotivoEncerramentoAppService(IMotivoEncerramentoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllItens(Int32 idAss)
        {
            List<MOTIVO_ENCERRAMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<MOTIVO_ENCERRAMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MOTIVO_ENCERRAMENTO GetItemById(Int32 id)
        {
            MOTIVO_ENCERRAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public MOTIVO_ENCERRAMENTO CheckExist(MOTIVO_ENCERRAMENTO conta, Int32 idAss)
        {
            MOTIVO_ENCERRAMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(MOTIVO_ENCERRAMENTO item, USUARIO usuario)
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
                    item.MOEN_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddMOEN",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<MOTIVO_ENCERRAMENTO>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.MOEN_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(MOTIVO_ENCERRAMENTO item, MOTIVO_ENCERRAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditMOEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOTIVO_ENCERRAMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<MOTIVO_ENCERRAMENTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(MOTIVO_ENCERRAMENTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CRM.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.MOEN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelMOEN",
                    LOG_TX_REGISTRO = item.MOEN_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(MOTIVO_ENCERRAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MOEN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatMOEN",
                    LOG_TX_REGISTRO = item.MOEN_NM_NOME
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
