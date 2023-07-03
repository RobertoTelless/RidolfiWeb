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
    public class TRFAppService : AppServiceBase<TRF>, ITRFAppService
    {
        private readonly ITRFService _baseService;

        public TRFAppService(ITRFService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public TRF CheckExist(TRF conta)
        {
            TRF item = _baseService.CheckExist(conta);
            return item;
        }

        public List<TRF> GetAllItens()
        {
            List<TRF> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<TRF> GetAllItensAdm()
        {
            List<TRF> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public TRF GetItemById(Int32 id)
        {
            TRF item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ExecuteFilter(String nome, Int32? uf, out List<TRF> objeto)
        {
            try
            {
                objeto = new List<TRF>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, uf);
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

        public Int32 ValidateCreate(TRF item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TRF1_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddTRF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TRF>(item)
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

        public Int32 ValidateEdit(TRF item, TRF itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditTRF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TRF>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TRF>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TRF item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PRECATORIO.Count > 0)
                {
                    return 1;
                }
                if (item.CLIENTE.Count > 0)
                {
                    return 1;
                }
                if (item.VARA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TRF1_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTRF",
                    LOG_TX_REGISTRO = item.TRF1_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TRF item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TRF1_IN_ATIVO = 1;
                item.UF = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTRF",
                    LOG_TX_REGISTRO = item.TRF1_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<TRF>, Boolean> ExecuteFilterTuple(String nome, Int32? uf, Int32 idAss)
        {
            try
            {
                List<TRF> objeto = new List<TRF>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, uf);
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

    }
}
