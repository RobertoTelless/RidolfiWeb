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

    public class FilialAppService : AppServiceBase<FILIAL>, IFilialAppService
    {
        private readonly IFilialService _baseService;

        public FilialAppService(IFilialService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<FILIAL> GetAllItens(Int32 idAss)
        {
            List<FILIAL> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<FILIAL> GetAllItensAdm(Int32 idAss)
        {
            List<FILIAL> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<UF> GetAllUF()
        {
            return _baseService.GetAllUF();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _baseService.GetAllTiposPessoa();
        }

        public FILIAL GetItemById(Int32 id)
        {
            FILIAL item = _baseService.GetItemById(id);
            return item;
        }

        public FILIAL CheckExist(FILIAL filial, Int32 idAss)
        {
            FILIAL item = _baseService.CheckExist(filial, idAss);
            return item;
        }

        public Int32 ValidateCreate(FILIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (usuario != null)
                {
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }
                }

                // Completa objeto
                item.FILI_IN_ATIVO = 1;
                Int32 volta = 0;

                // Monta Log
                if (usuario != null)
                {
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "AddFILI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<FILIAL>(item)
                    };

                    // Persiste
                    volta = _baseService.Create(item, log);
                }
                else
                {
                    // Persiste
                    volta = _baseService.Create(item);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FILIAL item, FILIAL itemAntes, USUARIO usuario)
        {
            try
            {
                //// Monta Log
                ///
                item.CLIENTE = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditFILI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = item.FILI_NM_NOME,
                    LOG_TX_REGISTRO_ANTES = itemAntes.FILI_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(FILIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                //if (item.FORNECEDOR.Count > 0)
                //{
                //    return 1;
                //}
                if (item.CLIENTE.Count > 0)
                {
                    return 1;
                }
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                //if (item.FORNECEDOR != null)
                //{
                //    item.FORNECEDOR = null;
                //}
                // Acerta campos
                item.FILI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelFILI",
                    LOG_TX_REGISTRO = "Filial : " + item.FILI_NM_NOME + "|" + item.FILI_NR_CNPJ
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FILIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FILI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatFILI",
                    LOG_TX_REGISTRO = "Filial : " + item.FILI_NM_NOME + "|" + item.FILI_NR_CNPJ
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
