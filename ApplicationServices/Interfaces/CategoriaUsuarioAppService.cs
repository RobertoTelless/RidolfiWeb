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
    public class CategoriaUsuarioAppService : AppServiceBase<CATEGORIA_USUARIO>, ICategoriaUsuarioAppService
    {
        private readonly ICategoriaUsuarioService _baseService;

        public CategoriaUsuarioAppService(ICategoriaUsuarioService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public CATEGORIA_USUARIO CheckExist(CATEGORIA_USUARIO conta, Int32 idAss)
        {
            CATEGORIA_USUARIO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_USUARIO> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_USUARIO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_USUARIO> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_USUARIO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_USUARIO GetItemById(Int32 id)
        {
            CATEGORIA_USUARIO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_USUARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CAUS_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAUS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_USUARIO>(item),
                    LOG_IN_SISTEMA = 2

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

        public Int32 ValidateEdit(CATEGORIA_USUARIO item, CATEGORIA_USUARIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditCAUS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_USUARIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_USUARIO>(itemAntes),
                    LOG_IN_SISTEMA = 2

                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_USUARIO item, CATEGORIA_USUARIO itemAntes)
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

        public Int32 ValidateDelete(CATEGORIA_USUARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.USUARIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CAUS_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleCAUS",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAUS_NM_NOME,
                    LOG_IN_SISTEMA = 2

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_USUARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAUS_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCAUS",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAUS_NM_NOME,
                    LOG_IN_SISTEMA = 2

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
