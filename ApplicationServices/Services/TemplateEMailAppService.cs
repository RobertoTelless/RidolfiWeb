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
    public class TemplateEMailAppService : AppServiceBase<TEMPLATE_EMAIL>, ITemplateEMailAppService
    {
        private readonly ITemplateEMailService _baseService;

        public TemplateEMailAppService(ITemplateEMailService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TEMPLATE_EMAIL> GetAllItens(Int32 idAss)
        {
            List<TEMPLATE_EMAIL> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public TEMPLATE_EMAIL CheckExist(TEMPLATE_EMAIL conta, Int32 idAss)
        {
            TEMPLATE_EMAIL item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TEMPLATE_EMAIL> GetAllItensAdm(Int32 idAss)
        {
            List<TEMPLATE_EMAIL> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TEMPLATE_EMAIL GetItemById(Int32 id)
        {
            TEMPLATE_EMAIL item = _baseService.GetItemById(id);
            return item;
        }

        public TEMPLATE_EMAIL GetByCode(String sigla, Int32 idAss)
        {
            TEMPLATE_EMAIL item = _baseService.GetByCode(sigla, idAss);
            return item;
        }

        public Int32 ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss, out List<TEMPLATE_EMAIL> objeto)
        {
            try
            {
                objeto = new List<TEMPLATE_EMAIL>();
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

        public Int32 ValidateCreate(TEMPLATE_EMAIL item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TEEM_IN_ATIVO = 1;
                item.TEEM_TX_COMPLETO = item.TEEM_TX_CABECALHO +  item.TEEM_TX_CORPO + item.TEEM_TX_DADOS;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTEEM",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_EMAIL>(item)
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

        public Int32 ValidateEdit(TEMPLATE_EMAIL item, TEMPLATE_EMAIL itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTEEM",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = item.TEEM_SG_SIGLA + "|" + item.TEEM_NM_NOME,
                    LOG_TX_REGISTRO_ANTES = String.Empty
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TEMPLATE_EMAIL item, USUARIO usuario)
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
                item.TEEM_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTEEM",
                    LOG_TX_REGISTRO = "Template: " + item.TEEM_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TEMPLATE_EMAIL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TEEM_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTEEM",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_EMAIL>(item)
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
