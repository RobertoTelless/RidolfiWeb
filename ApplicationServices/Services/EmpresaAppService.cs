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
    public class EmpresaAppService : AppServiceBase<EMPRESA>, IEmpresaAppService
    {
        private readonly IEmpresaService _baseService;

        public EmpresaAppService(IEmpresaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public EMPRESA CheckExist(EMPRESA conta, Int32 idAss)
        {
            EMPRESA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<EMPRESA> GetAllItens(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<EMPRESA> GetAllItensAdm(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public EMPRESA GetItemById(Int32 id)
        {
            EMPRESA item = _baseService.GetItemById(id);
            return item;
        }

        public EMPRESA GetItemByAssinante(Int32 id)
        {
            EMPRESA item = _baseService.GetItemByAssinante(id);
            return item;
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

        public EMPRESA_ANEXO GetAnexoById(Int32 id)
        {
            EMPRESA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public REGIME_TRIBUTARIO GetRegimeById(Int32 id)
        {
            REGIME_TRIBUTARIO lista = _baseService.GetRegimeById(id);
            return lista;
        }

        public Tuple<Int32, List<EMPRESA>, Boolean> ExecuteFilterTuple(String nome, Int32 idAss)
        {
            try
            {
                List<EMPRESA> objeto = new List<EMPRESA>();
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

        public List<REGIME_TRIBUTARIO> GetAllRegimes()
        {
            return _baseService.GetAllRegimes();
        }

        public Int32 ValidateCreate(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.EMPR_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.EMPR_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
                    LOG_IN_SISTEMA = 1

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

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log                
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EdtEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<EMPRESA>(itemAntes),
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes)
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

        public Int32 ValidateDelete(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.EMPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelEMPR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.EMPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaEMPR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Int32 ValidateEditAnexo(EMPRESA_ANEXO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
