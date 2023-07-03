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
    public class VaraAppService : AppServiceBase<VARA>, IVaraAppService
    {
        private readonly IVaraService _baseService;

        public VaraAppService(IVaraService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public VARA CheckExist(VARA conta)
        {
            VARA item = _baseService.CheckExist(conta);
            return item;
        }

        public List<VARA> GetAllItens()
        {
            List<VARA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<VARA> GetAllItensAdm()
        {
            List<VARA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public VARA GetItemById(Int32 id)
        {
            VARA item = _baseService.GetItemById(id);
            return item;
        }

        public List<TRF> GetAllTRF()
        {
            List<TRF> lista = _baseService.GetAllTRF();
            return lista;
        }

        public Int32 ExecuteFilter(String nome, Int32? trf, out List<VARA> objeto)
        {
            try
            {
                objeto = new List<VARA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, trf);
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

        public Int32 ValidateCreate(VARA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.VARA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddVARA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VARA>(item)
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

        public Int32 ValidateEdit(VARA item, VARA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditVARA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VARA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<VARA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(VARA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CLIENTE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.VARA_IN_ATIVO = 0;
                item.TRF = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelVARA",
                    LOG_TX_REGISTRO = item.VARA_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(VARA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.VARA_IN_ATIVO = 1;
                item.TRF = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatVARA",
                    LOG_TX_REGISTRO = item.VARA_NM_NOME
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
