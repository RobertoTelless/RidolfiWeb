using System;
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
    public class MensagemAutomacaoAppService : AppServiceBase<MENSAGEM_AUTOMACAO>, IMensagemAutomacaoAppService
    {
        private readonly IMensagemAutomacaoService _baseService;
        private readonly IConfiguracaoService _confService;

        public MensagemAutomacaoAppService(IMensagemAutomacaoService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }


        public List<MENSAGEM_AUTOMACAO> GetAllItens(Int32 idAss)
        {
            List<MENSAGEM_AUTOMACAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public MENSAGEM_AUTOMACAO_DATAS GetDatasById(Int32 id)
        {
            return _baseService.GetDatasById(id);
        }

        public List<MENSAGEM_AUTOMACAO> GetAllItensAdm(Int32 idAss)
        {
            List<MENSAGEM_AUTOMACAO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MENSAGEM_AUTOMACAO GetItemById(Int32 id)
        {
            MENSAGEM_AUTOMACAO item = _baseService.GetItemById(id);
            return item;
        }

        public MENSAGEM_AUTOMACAO CheckExist(MENSAGEM_AUTOMACAO conta, Int32 idAss)
        {
            MENSAGEM_AUTOMACAO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<GRUPO> GetAllGrupos(Int32 idAss)
        {
            List<GRUPO> lista = _baseService.GetAllGrupos(idAss);
            return lista;
        }

        public List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = _baseService.GetAllTemplatesSMS(idAss);
            return lista;
        }

        public List<TEMPLATE_EMAIL> GetAllTemplatesEMail(Int32 idAss)
        {
            List<TEMPLATE_EMAIL> lista = _baseService.GetAllTemplatesEMail(idAss);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? tipo, Int32? grupo, String nome, Int32 idAss, out List<MENSAGEM_AUTOMACAO> objeto)
        {
            try
            {
                objeto = new List<MENSAGEM_AUTOMACAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, grupo, nome, idAss);
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

       public Int32 ValidateCreate(MENSAGEM_AUTOMACAO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.MEAU_IN_ATIVO = 1;
                item.MEAU_DT_CADASTRO = DateTime.Now.Date;
                item.USUA_CD_ID = usuario.USUA_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddMEAU",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MENSAGEM_AUTOMACAO>(item)
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

        public Int32 ValidateEdit(MENSAGEM_AUTOMACAO item, MENSAGEM_AUTOMACAO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditMEAU",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MENSAGEM_AUTOMACAO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<MENSAGEM_AUTOMACAO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(MENSAGEM_AUTOMACAO item, MENSAGEM_AUTOMACAO itemAntes)
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

        public Int32 ValidateDelete(MENSAGEM_AUTOMACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MEAU_IN_ATIVO = 0;
                item.USUARIO = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelMEAU",
                    LOG_TX_REGISTRO = item.MEAU_DS_DESCRICAO + "|" + item.MEAU_DT_CADASTRO.Value.ToShortDateString() + "|" + item.GRUPO.GRUP_NM_NOME.ToString()
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(MENSAGEM_AUTOMACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MEAU_IN_ATIVO = 1;
                item.USUARIO = null;                

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatMEAU",
                    LOG_TX_REGISTRO = item.MEAU_DS_DESCRICAO + "|" + item.MEAU_DT_CADASTRO.Value.ToShortDateString() + "|" + item.GRUPO.GRUP_NM_NOME.ToString()
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditDatas(MENSAGEM_AUTOMACAO_DATAS item)
        {
            try
            {
                // Persiste
                return _baseService.EditDatas(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateDatas(MENSAGEM_AUTOMACAO_DATAS item)
        {
            try
            {
                // Persiste
                return _baseService.CreateDatas(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
