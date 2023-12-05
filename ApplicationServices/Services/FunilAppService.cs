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
using ModelServices.Interfaces.Repositories;

namespace ApplicationServices.Services
{
    public class FunilAppService : AppServiceBase<FUNIL>, IFunilAppService
    {
        private readonly IFunilService _baseService;
        private readonly IConfiguracaoService _confService;      

        public FunilAppService(IFunilService baseService,  IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            
        }

        public List<FUNIL> GetAllItens(Int32 idAss)
        {
            List<FUNIL> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<FUNIL> GetAllItensAdm(Int32 idAss)
        {
            List<FUNIL> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public FUNIL GetItemById(Int32 id)
        {
            FUNIL item = _baseService.GetItemById(id);
            return item;
        }


        public FUNIL GetItemBySigla(String sigla, Int32 idAss)
        {
            FUNIL item = _baseService.GetItemBySigla(sigla, idAss);
            return item;
        }

        public FUNIL CheckExist(FUNIL conta, Int32 idAss)
        {
            FUNIL item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public FUNIL_ETAPA GetEtapaById(Int32 id)
        {
            FUNIL_ETAPA lista = _baseService.GetEtapaById(id);
            return lista;
        }

        public Int32 ValidateCreate(FUNIL item, USUARIO usuario)
        {
            try
            {
                if (usuario != null)
                {
                    // Verifica Existencia
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }

                    // Completa objeto
                    item.FUNI_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddFUNI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<FUNIL>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item, log);
                }
                else
                {
                    // Completa objeto
                    item.FUNI_IN_ATIVO = 1;

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

        public Int32 ValidateEdit(FUNIL item, FUNIL itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditFUNI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FUNIL>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<FUNIL>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FUNIL item, FUNIL itemAntes)
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

        public Int32 ValidateDelete(FUNIL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CRM.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.FUNI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelFUNI",
                    LOG_TX_REGISTRO = "Nome: " + item.FUNI_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FUNIL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FUNI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatFUNI",
                    LOG_TX_REGISTRO = "Nome: " + item.FUNI_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditEtapa(FUNIL_ETAPA item)
        {
            try
            {
                // Persiste
                return _baseService.EditEtapa(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateEtapa(FUNIL_ETAPA item)
        {
            try
            {
                item.FUET_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateEtapa(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
