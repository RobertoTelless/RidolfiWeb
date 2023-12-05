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
    public class CategoriaAgendaAppService : AppServiceBase<CATEGORIA_AGENDA>, ICategoriaAgendaAppService
    {
        private readonly ICategoriaAgendaService _baseService;

        public CategoriaAgendaAppService(ICategoriaAgendaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public CATEGORIA_AGENDA CheckExist(CATEGORIA_AGENDA conta, Int32 idAss)
        {
            CATEGORIA_AGENDA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_AGENDA> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_AGENDA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_AGENDA> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_AGENDA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_AGENDA GetItemById(Int32 id)
        {
            CATEGORIA_AGENDA item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_AGENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CAAG_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAAG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_AGENDA>(item),
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

        public Int32 ValidateEdit(CATEGORIA_AGENDA item, CATEGORIA_AGENDA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditCAAG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_AGENDA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_AGENDA>(itemAntes),

                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_AGENDA item, CATEGORIA_AGENDA itemAntes)
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

        public Int32 ValidateDelete(CATEGORIA_AGENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.AGENDA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CAAG_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleCAAG",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAAG_NM_NOME,
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

        public Int32 ValidateReativar(CATEGORIA_AGENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAAG_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCAAG",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAAG_NM_NOME,
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
