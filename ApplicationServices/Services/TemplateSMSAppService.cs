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
    public class TemplateSMSAppService : AppServiceBase<TEMPLATE_SMS>, ITemplateSMSAppService
    {
        private readonly ITemplateSMSService _baseService;

        public TemplateSMSAppService(ITemplateSMSService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TEMPLATE_SMS> GetAllItens(Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public TEMPLATE_SMS CheckExist(TEMPLATE_SMS conta, Int32 idAss)
        {
            TEMPLATE_SMS item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TEMPLATE_SMS> GetAllItensAdm(Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TEMPLATE_SMS GetItemById(Int32 id)
        {
            TEMPLATE_SMS item = _baseService.GetItemById(id);
            return item;
        }

        public TEMPLATE_SMS GetByCode(String sigla, Int32 idAss)
        {
            TEMPLATE_SMS item = _baseService.GetByCode(sigla, idAss);
            return item;
        }

        public Int32 ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss, out List<TEMPLATE_SMS> objeto)
        {
            try
            {
                objeto = new List<TEMPLATE_SMS>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(sigla, nome, conteudo, idAss);
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

        public Int32 ValidateCreate(TEMPLATE_SMS item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TSMS_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTSMS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_SMS>(item)
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

        public Int32 ValidateEdit(TEMPLATE_SMS item, TEMPLATE_SMS itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTSMS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_SMS>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TEMPLATE_SMS>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TEMPLATE_SMS item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.MENSAGENS.Count > 0)
                {
                    if (item.MENSAGENS.Where(p => p.MENS_IN_ATIVO == 1).ToList().Count > 0)
                    {
                        return 1;
                    }
                }

                // Acerta campos
                item.TSMS_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTSMS",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_SMS>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TEMPLATE_SMS item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TSMS_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTSMS",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_SMS>(item)
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
