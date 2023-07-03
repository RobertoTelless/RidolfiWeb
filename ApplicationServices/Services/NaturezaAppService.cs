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
    public class NaturezaAppService : AppServiceBase<NATUREZA>, INaturezaAppService
    {
        private readonly INaturezaService _baseService;

        public NaturezaAppService(INaturezaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<NATUREZA> GetAllItens()
        {
            List<NATUREZA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<NATUREZA> GetAllItensAdm()
        {
            List<NATUREZA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public NATUREZA GetItemById(Int32 id)
        {
            NATUREZA item = _baseService.GetItemById(id);
            return item;
        }

        public NATUREZA CheckExist(NATUREZA conta)
        {
            NATUREZA item = _baseService.CheckExist(conta);
            return item;
        }

        public Int32 ValidateCreate(NATUREZA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.NATU_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddNATU",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NATUREZA>(item)
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


        public Int32 ValidateEdit(NATUREZA item, NATUREZA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditNATU",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NATUREZA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<NATUREZA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(NATUREZA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PRECATORIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.NATU_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelNATU",
                    LOG_TX_REGISTRO = item.NATU_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(NATUREZA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.NATU_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatNATU",
                    LOG_TX_REGISTRO = item.NATU_NM_NOME
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
