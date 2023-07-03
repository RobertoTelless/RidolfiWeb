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
    public class PlataformaEntregaAppService : AppServiceBase<PLATAFORMA_ENTREGA>, IPlataformaEntregaAppService
    {
        private readonly IPlataformaEntregaService _baseService;

        public PlataformaEntregaAppService(IPlataformaEntregaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public PLATAFORMA_ENTREGA CheckExist(PLATAFORMA_ENTREGA conta, Int32 idAss)
        {
            PLATAFORMA_ENTREGA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<PLATAFORMA_ENTREGA> GetAllItens(Int32 idAss)
        {
            List<PLATAFORMA_ENTREGA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<PLATAFORMA_ENTREGA> GetAllItensAdm(Int32 idAss)
        {
            List<PLATAFORMA_ENTREGA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public PLATAFORMA_ENTREGA GetItemById(Int32 id)
        {
            PLATAFORMA_ENTREGA item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<PLATAFORMA_ENTREGA>, Boolean> ExecuteFilterTuple(String nome, Int32 idAss)
        {
            try
            {
                List<PLATAFORMA_ENTREGA> objeto = new List<PLATAFORMA_ENTREGA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, idAss);
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

        public Int32 ValidateCreate(PLATAFORMA_ENTREGA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PLEN_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.PLEN_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPLEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLATAFORMA_ENTREGA>(item)
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

        public Int32 ValidateEdit(PLATAFORMA_ENTREGA item, PLATAFORMA_ENTREGA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditPLEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLATAFORMA_ENTREGA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PLATAFORMA_ENTREGA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PLATAFORMA_ENTREGA item, PLATAFORMA_ENTREGA itemAntes)
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

        public Int32 ValidateDelete(PLATAFORMA_ENTREGA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PLEN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelePLEN",
                    LOG_TX_REGISTRO = "Plataforma: " + item.PLEN_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PLATAFORMA_ENTREGA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PLEN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPLEN",
                    LOG_TX_REGISTRO = "Plataforma: " + item.PLEN_NM_NOME
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
