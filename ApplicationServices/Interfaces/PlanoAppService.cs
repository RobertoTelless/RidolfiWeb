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
    public class PlanoAppService : AppServiceBase<PLANO>, IPlanoAppService
    {
        private readonly IPlanoService _baseService;
        private readonly IConfiguracaoService _confService;

        public PlanoAppService(IPlanoService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<PLANO> GetAllItens()
        {
            List<PLANO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PLANO> GetAllItensAdm()
        {
            List<PLANO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public List<PLANO> GetAllValidos()
        {
            List<PLANO> lista = _baseService.GetAllValidos();
            return lista;
        }

        public List<PLANO_PERIODICIDADE> GetAllPeriodicidades()
        {
            List<PLANO_PERIODICIDADE> lista = _baseService.GetAllPeriodicidades();
            return lista;
        }

        public PLANO_PERIODICIDADE GetPeriodicidadeById(Int32 id)
        {
            return _baseService.GetPeriodicidadeById(id);
        }

        public PLANO GetItemById(Int32 id)
        {
            PLANO item = _baseService.GetItemById(id);
            return item;
        }

        public PLANO CheckExist(PLANO conta)
        {
            PLANO item = _baseService.CheckExist(conta);
            return item;
        }

        public Tuple<Int32, List<PLANO>, Boolean> ExecuteFilter(String nome, String descricao)
        {
            try
            {
                List<PLANO> objeto = new List<PLANO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, descricao);
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

        public Int32 ValidateCreate(PLANO item, USUARIO usuario)
        {
            try
            {
                // Checa existencia
                var conf = usuario.USUA_CD_ID;
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Valida duracao
                PLANO_PERIODICIDADE per = _baseService.GetPeriodicidadeById(item.PLPE_CD_ID.Value);
                if (per.PLPE_NM_NOME.Contains("Bimestral"))
                {
                    if (item.PLAN_IN_DURACAO < 2)
                    {
                        return 2;
                    }
                    Int32 rem = item.PLAN_IN_DURACAO.Value % 2;
                    if (rem != 0)
                    {
                        return 2;
                    }
                }
                if (per.PLPE_NM_NOME.Contains("Trimestral"))
                {
                    if (item.PLAN_IN_DURACAO < 3)
                    {
                        return 2;
                    }
                    Int32 rem = item.PLAN_IN_DURACAO.Value % 3;
                    if (rem != 0)
                    {
                        return 2;
                    }
                }
                if (per.PLPE_NM_NOME.Contains("Quadrimestral"))
                {
                    if (item.PLAN_IN_DURACAO < 4)
                    {
                        return 2;
                    }
                    Int32 rem = item.PLAN_IN_DURACAO.Value % 4;
                    if (rem != 0)
                    {
                        return 2;
                    }
                }
                if (per.PLPE_NM_NOME.Contains("Semestral"))
                {
                    if (item.PLAN_IN_DURACAO < 6)
                    {
                        return 2;
                    }
                    Int32 rem = item.PLAN_IN_DURACAO.Value % 6;
                    if (rem != 0)
                    {
                        return 2;
                    }
                }
                if (per.PLPE_NM_NOME.Contains("Anual"))
                {
                    if (item.PLAN_IN_DURACAO < 12)
                    {
                        return 2;
                    }
                    Int32 rem = item.PLAN_IN_DURACAO.Value % 12;
                    if (rem != 0)
                    {
                        return 2;
                    }
                }

                // Completa objeto
                item.PLAN_IN_ATIVO = 1;
                item.PLAN_DT_CRIACAO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPLAN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO>(item)
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

        public Int32 ValidateEdit(PLANO item, PLANO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPLAN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PLANO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PLANO item, PLANO itemAntes)
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

        public Int32 ValidateDelete(PLANO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ASSINANTE_PLANO.Count > 0)
                {
                    return 1;
                }
                if (item.ASSINANTE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PLAN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPLAN",
                    LOG_TX_REGISTRO = item.PLAN_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PLANO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PLAN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPLAN",
                    LOG_TX_REGISTRO = item.PLAN_NM_NOME
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
