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
    public class TipoTarefaAppService : AppServiceBase<TIPO_TAREFA>, ITipoTarefaAppService
    {
        private readonly ITipoTarefaService _baseService;

        public TipoTarefaAppService(ITipoTarefaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_TAREFA> GetAllItens(Int32 idAss)
        {
            List<TIPO_TAREFA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_TAREFA> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_TAREFA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_TAREFA GetItemById(Int32 id)
        {
            TIPO_TAREFA item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_TAREFA CheckExist(TIPO_TAREFA conta, Int32 idAss)
        {
            TIPO_TAREFA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_TAREFA item, USUARIO usuario)
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
                    item.TITR_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTITR",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_TAREFA>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TITR_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(TIPO_TAREFA item, TIPO_TAREFA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditTITR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_TAREFA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_TAREFA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_TAREFA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.TAREFA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TITR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTITR",
                    LOG_TX_REGISTRO = item.TITR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_TAREFA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TITR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTITR",
                    LOG_TX_REGISTRO = item.TITR_NM_NOME
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
