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
    public class RecursividadeAppService : AppServiceBase<RECURSIVIDADE>, IRecursividadeAppService
    {
        private readonly IRecursividadeService _baseService;
        private readonly IConfiguracaoService _confService;      

        public RecursividadeAppService(IRecursividadeService baseService,  IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            
        }

        public List<RECURSIVIDADE> GetAllItens(Int32 idAss)
        {
            List<RECURSIVIDADE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<RECURSIVIDADE> GetAllItensAdm(Int32 idAss)
        {
            List<RECURSIVIDADE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public RECURSIVIDADE GetItemById(Int32 id)
        {
            RECURSIVIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public RECURSIVIDADE CheckExist(RECURSIVIDADE conta, Int32 idAss)
        {
            RECURSIVIDADE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public RECURSIVIDADE_DESTINO GetDestinoById(Int32 id)
        {
            RECURSIVIDADE_DESTINO lista = _baseService.GetDestinoById(id);
            return lista;
        }

        public RECURSIVIDADE_DATA GetDataById(Int32 id)
        {
            RECURSIVIDADE_DATA lista = _baseService.GetDataById(id);
            return lista;
        }

        public Tuple<Int32, List<RECURSIVIDADE>, Boolean> ExecuteFilter(Int32? tipoMensagem, String nome, DateTime? dataInicio, DateTime? dataFim, String texto, Int32 idAss)
        {
            try
            {
                List<RECURSIVIDADE> objeto = new List<RECURSIVIDADE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipoMensagem, nome, dataInicio, dataFim, texto, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(RECURSIVIDADE item, USUARIO usuario)
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
                    item.RECU_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddRECU",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<RECURSIVIDADE>(item)
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item, log);
                }
                else
                {
                    // Completa objeto
                    item.RECU_IN_ATIVO = 1;

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

        public Int32 ValidateEdit(RECURSIVIDADE item, RECURSIVIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditRECU",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<RECURSIVIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<RECURSIVIDADE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(RECURSIVIDADE item, RECURSIVIDADE itemAntes)
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

        public Int32 ValidateDelete(RECURSIVIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.RECURSIVIDADE_DATA.Where(p => p.REDA_IN_PROCESSADA == 0).ToList().Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.RECU_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelRECU",
                    LOG_TX_REGISTRO = "Nome: " + item.RECU_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(RECURSIVIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.RECU_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatRECU",
                    LOG_TX_REGISTRO = "Nome: " + item.RECU_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateDestino(RECURSIVIDADE_DESTINO item)
        {
            try
            {
                item.REDE_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateDestino(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateData(RECURSIVIDADE_DATA item)
        {
            try
            {
                item.REDA_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateData(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
