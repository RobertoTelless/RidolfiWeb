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
    public class ParentescoAppService : AppServiceBase<PARENTESCO>, IParentescoAppService
    {
        private readonly IParentescoService _baseService;

        public ParentescoAppService(IParentescoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<PARENTESCO> GetAllItens()
        {
            List<PARENTESCO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PARENTESCO> GetAllItensAdm()
        {
            List<PARENTESCO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public PARENTESCO GetItemById(Int32 id)
        {
            PARENTESCO item = _baseService.GetItemById(id);
            return item;
        }

        public PARENTESCO CheckExist(PARENTESCO conta)
        {
            PARENTESCO item = _baseService.CheckExist(conta);
            return item;
        }

        public Int32 ValidateCreate(PARENTESCO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PARE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PARENTESCO>(item)
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


        public Int32 ValidateEdit(PARENTESCO item, PARENTESCO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PARENTESCO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PARENTESCO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PARENTESCO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.BENEFICIARIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PARE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPARE",
                    LOG_TX_REGISTRO = item.PARE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PARENTESCO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PARE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPARE",
                    LOG_TX_REGISTRO = item.PARE_NM_NOME
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
