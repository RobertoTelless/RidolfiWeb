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
    public class CategoriaClienteAppService : AppServiceBase<CATEGORIA_CLIENTE>, ICategoriaClienteAppService
    {
        private readonly ICategoriaClienteService _baseService;

        public CategoriaClienteAppService(ICategoriaClienteService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<CATEGORIA_CLIENTE> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_CLIENTE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_CLIENTE> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_CLIENTE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_CLIENTE GetItemById(Int32 id)
        {
            CATEGORIA_CLIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public CATEGORIA_CLIENTE CheckExist(CATEGORIA_CLIENTE conta, Int32 idAss)
        {
            CATEGORIA_CLIENTE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (usuario != null)
                {
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }

                    // Completa objeto
                    item.CACL_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddCACL",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_CLIENTE>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.CACL_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(CATEGORIA_CLIENTE item, CATEGORIA_CLIENTE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtCACL",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_CLIENTE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_CLIENTE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CATEGORIA_CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CLIENTE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CACL_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCACL",
                    LOG_TX_REGISTRO = item.CACL_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CACL_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaCACL",
                    LOG_TX_REGISTRO = item.CACL_NM_NOME
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
