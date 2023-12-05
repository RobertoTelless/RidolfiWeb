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
    public class VendaMensalAppService : AppServiceBase<VENDA_MENSAL>, IVendaMensalAppService
    {
        private readonly IVendaMensalService _baseService;
        private readonly IEmpresaAppService _filService;

        public VendaMensalAppService(IVendaMensalService baseService, IEmpresaAppService filService) : base(baseService)
        {
            _baseService = baseService;
            _filService = filService;
        }


        public Int32 ValidateEditAnotacao(VENDA_MENSAL_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public VENDA_MENSAL_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _baseService.GetAnotacaoById(id);
        }

        public VENDA_MENSAL_ANEXO GetAnexoById(Int32 id)
        {
            return _baseService.GetAnexoById(id);
        }

        public List<VENDA_MENSAL> GetAllItens(Int32 idAss)
        {
            List<VENDA_MENSAL> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<VENDA_MENSAL> GetAllItensAdm(Int32 idAss)
        {
            List<VENDA_MENSAL> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public VENDA_MENSAL GetItemById(Int32 id)
        {
            VENDA_MENSAL item = _baseService.GetItemById(id);
            return item;
        }

        public VENDA_MENSAL CheckExist(VENDA_MENSAL conta, Int32 idAss)
        {
            VENDA_MENSAL item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Tuple<Int32, List<VENDA_MENSAL>, Boolean> ExecuteFilterTuple(DateTime? dataRef, Int32? tipo, Int32 idAss)
        {
            try
            {
                List<VENDA_MENSAL> objeto = new List<VENDA_MENSAL>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(dataRef, tipo, idAss);
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


        public Int32 ValidateCreate(VENDA_MENSAL item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Critica
                if (item.VEMA_IN_TIPO == 2 || item.VEMA_IN_TIPO == 3)
                {
                    if (item.MAQN_CD_ID == null)
                    {
                        return 4;
                    }
                }
                if (item.VEMA_IN_TIPO == 4)
                {
                    if (item.PLEN_CD_ID == null)
                    {
                        return 5;
                    }
                }
                if (item.VEMA_IN_TIPO == 5)
                {
                    if (item.TICK_CD_ID == null)
                    {
                        return 6;
                    }
                }

                // Completa objeto
                item.VEMA_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Checa total
                if (item.VEMA_VL_TOTAL == 0 || item.VEMA_VL_TOTAL == null)
                {
                    return 3;
                }
                Decimal? com = item.VEMA_VL_GERENTE + item.VEMA_VL_VENDEDOR + item.VEMA_VL_OUTROS;
                if (com > item.VEMA_VL_TOTAL)
                {
                    return 2;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddVEMA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VENDA_MENSAL>(item)
                };

                // Persiste produto
                Int32 volta = _baseService.Create(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(VENDA_MENSAL item, VENDA_MENSAL itemAntes, USUARIO usuario)
        {
            try
            {
                // Critica
                if (item.VEMA_IN_TIPO == 2 || item.VEMA_IN_TIPO == 3)
                {
                    if (item.MAQN_CD_ID == null)
                    {
                        return 4;
                    }
                }
                if (item.VEMA_IN_TIPO == 4)
                {
                    if (item.PLEN_CD_ID == null)
                    {
                        return 5;
                    }
                }
                if (item.VEMA_IN_TIPO == 5)
                {
                    if (item.TICK_CD_ID == null)
                    {
                        return 6;
                    }
                }

                // Checa total
                if (item.VEMA_VL_TOTAL == 0 || item.VEMA_VL_TOTAL == null)
                {
                    return 1;
                }
                Decimal? com = item.VEMA_VL_GERENTE + item.VEMA_VL_VENDEDOR + item.VEMA_VL_OUTROS;
                if (com > item.VEMA_VL_TOTAL)
                {
                    return 2;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditVEMA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VENDA_MENSAL>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<VENDA_MENSAL>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(VENDA_MENSAL item, VENDA_MENSAL itemAntes)
        {
            try
            {
                // Critica
                if (item.VEMA_IN_TIPO == 2 || item.VEMA_IN_TIPO == 3)
                {
                    if (item.MAQN_CD_ID == null)
                    {
                        return 4;
                    }
                }
                if (item.VEMA_IN_TIPO == 4)
                {
                    if (item.PLEN_CD_ID == null)
                    {
                        return 5;
                    }
                }
                if (item.VEMA_IN_TIPO == 5)
                {
                    if (item.TICK_CD_ID == null)
                    {
                        return 6;
                    }
                }

                // Checa total
                if (item.VEMA_VL_TOTAL == 0)
                {
                    return 1;
                }
                Decimal? com = item.VEMA_VL_GERENTE + item.VEMA_VL_VENDEDOR + item.VEMA_VL_OUTROS;
                if (com != item.VEMA_VL_TOTAL)
                {
                    return 2;
                }

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(VENDA_MENSAL item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.VEMA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelVEMA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VENDA_MENSAL>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(VENDA_MENSAL item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.VEMA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatVEMA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VENDA_MENSAL>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public VENDA_MENSAL_COMISSAO GetComissaoById(Int32 id)
        {
            return _baseService.GetComissaoById(id);
        }

        public Int32 ValidateEditComissao(VENDA_MENSAL_COMISSAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditComissao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateComissao(VENDA_MENSAL_COMISSAO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateComissao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
