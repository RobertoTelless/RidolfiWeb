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
    public class TicketAppService : AppServiceBase<TICKET_ALIMENTACAO>, ITicketAppService
    {
        private readonly ITicketService _baseService;

        public TicketAppService(ITicketService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public TICKET_ALIMENTACAO CheckExist(TICKET_ALIMENTACAO conta, Int32 idAss)
        {
            TICKET_ALIMENTACAO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TICKET_ALIMENTACAO> GetAllItens(Int32 idAss)
        {
            List<TICKET_ALIMENTACAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TICKET_ALIMENTACAO> GetAllItensAdm(Int32 idAss)
        {
            List<TICKET_ALIMENTACAO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TICKET_ALIMENTACAO GetItemById(Int32 id)
        {
            TICKET_ALIMENTACAO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(TICKET_ALIMENTACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TICK_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddTICK",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TICKET_ALIMENTACAO>(item)
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

        public Int32 ValidateEdit(TICKET_ALIMENTACAO item, TICKET_ALIMENTACAO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTICK",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TICKET_ALIMENTACAO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TICKET_ALIMENTACAO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TICKET_ALIMENTACAO item, TICKET_ALIMENTACAO itemAntes)
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

        public Int32 ValidateDelete(TICKET_ALIMENTACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TICK_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleTICK",
                    LOG_TX_REGISTRO = "Ticket: " + item.TICK_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TICKET_ALIMENTACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TICK_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTICK",
                    LOG_TX_REGISTRO = "Ticket: " + item.TICK_NM_NOME
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
