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
    public class MaquinaAppService : AppServiceBase<MAQUINA>, IMaquinaAppService
    {
        private readonly IMaquinaService _baseService;

        public MaquinaAppService(IMaquinaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public MAQUINA CheckExist(MAQUINA conta, Int32 idAss)
        {
            MAQUINA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<MAQUINA> GetAllItens(Int32 idAss)
        {
            List<MAQUINA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<MAQUINA> GetAllItensAdm(Int32 idAss)
        {
            List<MAQUINA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MAQUINA GetItemById(Int32 id)
        {
            MAQUINA item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<MAQUINA>, Boolean> ExecuteFilterTuple(String provedor, String nome, Int32? idAss)
        {
            try
            {
                List<MAQUINA> objeto = new List<MAQUINA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(provedor, nome, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(MAQUINA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.MAQN_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.MAQN_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddMAQN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MAQUINA>(item)
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

        public Int32 ValidateEdit(MAQUINA item, MAQUINA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditMAQN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MAQUINA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<MAQUINA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(MAQUINA item, MAQUINA itemAntes)
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

        public Int32 ValidateDelete(MAQUINA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.EMPRESA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.MAQN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleMAQN",
                    LOG_TX_REGISTRO = "Máquina: " + item.MAQN_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(MAQUINA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MAQN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatMAQN",
                    LOG_TX_REGISTRO = "Máquina: " + item.MAQN_NM_NOME
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
