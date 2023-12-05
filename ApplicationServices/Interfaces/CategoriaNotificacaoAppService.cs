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
    public class CategoriaNotificacaoAppService : AppServiceBase<CATEGORIA_NOTIFICACAO>, ICategoriaNotificacaoAppService
    {
        private readonly ICategoriaNotificacaoService _baseService;

        public CategoriaNotificacaoAppService(ICategoriaNotificacaoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public CATEGORIA_NOTIFICACAO CheckExist(CATEGORIA_NOTIFICACAO conta, Int32 idAss)
        {
            CATEGORIA_NOTIFICACAO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_NOTIFICACAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_NOTIFICACAO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_NOTIFICACAO GetItemById(Int32 id)
        {
            CATEGORIA_NOTIFICACAO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_NOTIFICACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CANO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCANO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_NOTIFICACAO>(item),
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

        public Int32 ValidateEdit(CATEGORIA_NOTIFICACAO item, CATEGORIA_NOTIFICACAO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditCANO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_NOTIFICACAO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_NOTIFICACAO>(itemAntes),
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

        public Int32 ValidateEdit(CATEGORIA_NOTIFICACAO item, CATEGORIA_NOTIFICACAO itemAntes)
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

        public Int32 ValidateDelete(CATEGORIA_NOTIFICACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.NOTIFICACAO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CANO_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleCANO",
                    LOG_TX_REGISTRO = "Categoria: " + item.CANO_NM_NOME,
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

        public Int32 ValidateReativar(CATEGORIA_NOTIFICACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CANO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCANO",
                    LOG_TX_REGISTRO = "Categoria: " + item.CANO_NM_NOME,
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
