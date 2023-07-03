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
    public class HonorarioAppService : AppServiceBase<HONORARIO>, IHonorarioAppService
    {
        private readonly IHonorarioService _baseService;
        private readonly IConfiguracaoService _confService;

        public HonorarioAppService(IHonorarioService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<HONORARIO> GetAllItens()
        {
            List<HONORARIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<HONORARIO> GetAllItensAdm()
        {
            List<HONORARIO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public HONORARIO GetItemById(Int32 id)
        {
            HONORARIO item = _baseService.GetItemById(id);
            return item;
        }

        public HONORARIO CheckExist(HONORARIO conta)
        {
            HONORARIO item = _baseService.CheckExist(conta);
            return item;
        }

        public HONORARIO CheckExist(String nome, String cpf)
        {
            HONORARIO item = _baseService.CheckExist(nome, cpf);
            return item;
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllTiposPessoa();
            return lista;
        }

        public HONORARIO_ANEXO GetAnexoById(Int32 id)
        {
            HONORARIO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public HONORARIO_ANOTACOES GetComentarioById(Int32 id)
        {
            HONORARIO_ANOTACOES lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? tipo, String cpf, String cnpj, String razao, String nome, out List<HONORARIO> objeto)
        {
            try
            {
                objeto = new List<HONORARIO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, cpf, cnpj, razao, nome);
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

        public Int32 ValidateCreate(HONORARIO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.HONO_IN_ATIVO = 1;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddHONO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<HONORARIO>(item)
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

        public Int32 ValidateEdit(HONORARIO item, HONORARIO itemAntes, USUARIO usuario)
        {
            try
            {
                if (itemAntes.TIPO_PESSOA != null)
                {
                    itemAntes.TIPO_PESSOA = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditHONO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<HONORARIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<HONORARIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(HONORARIO item, HONORARIO itemAntes)
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

        public Int32 ValidateDelete(HONORARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PRECATORIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.HONO_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelHONO",
                    LOG_TX_REGISTRO = item.HONO_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(HONORARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.HONO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatHONO",
                    LOG_TX_REGISTRO = item.HONO_NM_NOME
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
