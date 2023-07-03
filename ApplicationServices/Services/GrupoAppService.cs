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
    public class GrupoAppService : AppServiceBase<GRUPO>, IGrupoAppService
    {
        private readonly IGrupoService _baseService;
        private readonly IConfiguracaoService _confService;

        public GrupoAppService(IGrupoService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<GRUPO> GetAllItens(Int32 idAss)
        {
            List<GRUPO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<GRUPO> GetAllItensAdm(Int32 idAss)
        {
            List<GRUPO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public GRUPO GetItemById(Int32 id)
        {
            GRUPO item = _baseService.GetItemById(id);
            return item;
        }

        public GRUPO_CLIENTE GetContatoById(Int32 id)
        {
            return _baseService.GetContatoById(id);
        }

        public GRUPO CheckExist(GRUPO conta, Int32 idAss)
        {
            GRUPO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public GRUPO_CLIENTE CheckExistContato(GRUPO_CLIENTE conta)
        {
            GRUPO_CLIENTE item = _baseService.CheckExistContato(conta);
            return item;
        }

        public Int32 ValidateCreate(GRUPO item, MontagemGrupo grupo, USUARIO usuario)
        {
            try
            {
                // Checa existencia
                var conf = usuario.USUA_CD_ID;
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 3456;
                }

                // Checa existencia de contatos
                List<CLIENTE> lista = _baseService.FiltrarContatos(item, usuario.ASSI_CD_ID);
                if (lista.Count == 0)
                {
                    return 6543;
                }

                // Completa objeto
                item.GRUP_IN_ATIVO = 1;
                item.GRUP_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddGRUP",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<GRUPO>(item)
                };

                // Persiste grupo
                Int32 volta = _baseService.Create(item, log);

                // Processa contatos
                GRUPO grupoCriado = _baseService.GetItemById(item.GRUP_CD_ID);
                GRUPO_CLIENTE gru = new GRUPO_CLIENTE();
                foreach (CLIENTE cli in lista)
                {
                    gru.CLIE_CD_ID = cli.CLIE_CD_ID;
                    gru.GRCL_IN_ATIVO = 1;
                    gru.GRUP_CD_ID = item.GRUP_CD_ID;
                    grupoCriado.GRUPO_CLIENTE.Add(gru);
                    gru = new GRUPO_CLIENTE();
                }
                Int32 volta1 = _baseService.Edit(grupoCriado, log);
                return lista.Count;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateRemontar(GRUPO item, MontagemGrupo grupo, USUARIO usuario)
        {
            try
            {
                // Checa existencia de contatos
                List<CLIENTE> lista = _baseService.FiltrarContatos(item, usuario.ASSI_CD_ID);
                if (lista.Count == 0)
                {
                    return 2;
                }

                // Remonta contatos
                GRUPO grupoCriado = _baseService.GetItemById(item.GRUP_CD_ID);
                GRUPO_CLIENTE gru = new GRUPO_CLIENTE();
                foreach (CLIENTE cli in lista)
                {
                    gru.CLIE_CD_ID = cli.CLIE_CD_ID;
                    gru.GRCL_IN_ATIVO = 1;
                    gru.GRUP_CD_ID = item.GRUP_CD_ID;
                    grupoCriado.GRUPO_CLIENTE.Add(gru);
                    gru = new GRUPO_CLIENTE();
                }
                Int32 volta = _baseService.Edit(grupoCriado);
                return lista.Count;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(GRUPO item, GRUPO itemAntes, USUARIO usuario)
        {
            try
            {
                if (itemAntes.USUARIO != null)
                {
                    itemAntes.USUARIO = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditGRUP",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<GRUPO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<GRUPO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(GRUPO item, GRUPO itemAntes)
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

        public Int32 ValidateDelete(GRUPO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.MENSAGENS.Count > 0)
                {
                    if (item.MENSAGENS.Where(p => p.MENS_IN_ATIVO == 1).ToList().Count > 0)
                    {
                        return 1;
                    }
                }

                // Acerta campos
                item.GRUP_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelGRUP",
                    LOG_TX_REGISTRO = item.GRUP_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(GRUPO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.GRUP_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatGRUP",
                    LOG_TX_REGISTRO = item.GRUP_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(GRUPO_CLIENTE item)
        {
            try
            {
                // Checa existencia
                if (_baseService.CheckExistContato(item) != null)
                {
                    return 1;
                }

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditContato(GRUPO_CLIENTE item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
