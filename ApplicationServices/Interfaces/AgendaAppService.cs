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
using System.Web.Mvc;

namespace ApplicationServices.Services
{
    public class AgendaAppService : AppServiceBase<AGENDA>, IAgendaAppService
    {
        private readonly IAgendaService _baseService;

        public AgendaAppService(IAgendaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<AGENDA> GetAllItens(Int32 idAss)
        {
            List<AGENDA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<AGENDA> GetAllItensAdm(Int32 idAss)
        {
            List<AGENDA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<AGENDA> GetByDate(DateTime data, Int32 idAss)
        {
            List<AGENDA> lista = _baseService.GetByDate(data, idAss);
            return lista;
        }

        public List<AGENDA> GetByUser(Int32 id, Int32 idAss)
        {
            List<AGENDA> lista = _baseService.GetByUser(id, idAss);
            return lista;
        }

        public AGENDA GetItemById(Int32 id)
        {
            AGENDA item = _baseService.GetItemById(id);
            return item;
        }

        public List<CATEGORIA_AGENDA> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_AGENDA> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public AGENDA_ANEXO GetAnexoById(Int32 id)
        {
            AGENDA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public async Task<IEnumerable<CATEGORIA_AGENDA>> GetAllItensAsync(Int32 idAss)
        {
            return await _baseService.GetAllItensAsync(idAss);
        }

        public Int32 ExecuteFilter(DateTime? data, Int32 ? cat, String titulo, String descricao, Int32 idAss, Int32 idUser, Int32 corp, out List<AGENDA> objeto)
        {
            try
            {
                objeto = new List<AGENDA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(data, cat, titulo, descricao, idAss, idUser, corp);
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

        public Tuple<Int32, List<AGENDA>, Boolean> ExecuteFilterTuple(DateTime? data, Int32 ? cat, String titulo, String descricao, Int32 idAss, Int32 idUser, Int32 corp)
        {
            try
            {
                List<AGENDA> objeto = new List<AGENDA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(data, cat, titulo, descricao, idAss, idUser, corp);
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

        public Int32 ValidateCreate(AGENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica data e hora
                if (item.AGEN_DT_DATA < DateTime.Today.Date)
                {
                    return 1;
                }
                if (item.AGEN_DT_DATA == DateTime.Today.Date)
                {
                    if (item.AGEN_HR_HORA < DateTime.Now.TimeOfDay)
                    {
                        return 1;
                    }
                    if (item.AGEN_HR_FINAL < DateTime.Now.TimeOfDay)
                    {
                        return 1;
                    }
                }
                if (item.AGEN_HR_HORA > item.AGEN_HR_FINAL)
                {
                    return 3;
                }

                List<AGENDA> lista = _baseService.GetByUser(usuario.USUA_CD_ID, usuario.ASSI_CD_ID).Where(p => p.AGEN_DT_DATA == item.AGEN_DT_DATA).ToList();
                List<AGENDA> lista1 = lista.Where(p => p.AGEN_HR_HORA < item.AGEN_HR_HORA & p.AGEN_HR_FINAL > item.AGEN_HR_FINAL).ToList();
                List<AGENDA> lista2 = lista.Where(p => p.AGEN_HR_HORA < item.AGEN_HR_HORA & p.AGEN_HR_FINAL < item.AGEN_HR_FINAL & p.AGEN_HR_FINAL > item.AGEN_HR_HORA).ToList();
                List<AGENDA> lista3 = lista.Where(p => p.AGEN_HR_HORA > item.AGEN_HR_HORA & p.AGEN_HR_FINAL > item.AGEN_HR_FINAL & p.AGEN_HR_HORA < item.AGEN_HR_FINAL).ToList();
                List<AGENDA> lista4 = lista.Where(p => p.AGEN_HR_HORA > item.AGEN_HR_HORA & p.AGEN_HR_FINAL < item.AGEN_HR_FINAL).ToList();

                if (lista1.Count > 0 || lista2.Count > 0 || lista3.Count > 0 || lista4.Count > 0)
                {
                    return 2;
                }

                // Completa objeto
                item.AGEN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddAGEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA>(item),
                    LOG_IN_SISTEMA = 2
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(AGENDA item, AGENDA itemAntes, USUARIO usuario)
        {
            try
            {
                // Ajustes
                itemAntes.CATEGORIA_AGENDA = null;
                itemAntes.USUARIO = null;
                itemAntes.USUARIO1 = null;

                // Verifica data e hora
                if (item.AGEN_DT_DATA < DateTime.Today.Date)
                {
                    return 1;
                }
                if (item.AGEN_DT_DATA == DateTime.Today.Date)
                {
                    if (item.AGEN_HR_HORA < DateTime.Now.TimeOfDay)
                    {
                        return 1;
                    }
                    if (item.AGEN_HR_FINAL < DateTime.Now.TimeOfDay)
                    {
                        return 1;
                    }
                }
                if (item.AGEN_HR_HORA > item.AGEN_HR_FINAL)
                {
                    return 3;
                }

                List<AGENDA> lista = _baseService.GetByUser(usuario.USUA_CD_ID, usuario.ASSI_CD_ID).Where(p => p.AGEN_DT_DATA == item.AGEN_DT_DATA).ToList();
                List<AGENDA> lista1 = lista.Where(p => p.AGEN_HR_HORA < item.AGEN_HR_HORA & p.AGEN_HR_FINAL > item.AGEN_HR_FINAL).ToList();
                List<AGENDA> lista2 = lista.Where(p => p.AGEN_HR_HORA < item.AGEN_HR_HORA & p.AGEN_HR_FINAL < item.AGEN_HR_FINAL & p.AGEN_HR_FINAL > item.AGEN_HR_HORA).ToList();
                List<AGENDA> lista3 = lista.Where(p => p.AGEN_HR_HORA > item.AGEN_HR_HORA & p.AGEN_HR_FINAL > item.AGEN_HR_FINAL & p.AGEN_HR_HORA < item.AGEN_HR_FINAL).ToList();
                List<AGENDA> lista4 = lista.Where(p => p.AGEN_HR_HORA > item.AGEN_HR_HORA & p.AGEN_HR_FINAL < item.AGEN_HR_FINAL).ToList();

                if (lista1.Count > 0 || lista2.Count > 0 || lista3.Count > 0 || lista4.Count > 0)
                {
                    return 2;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtAGEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<AGENDA>(item),
                    LOG_IN_SISTEMA = 2
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

        public Int32 ValidateEdit(AGENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica data e hora
                if (item.AGEN_DT_DATA < DateTime.Today.Date)
                {
                    return 1;
                }
                if (item.AGEN_DT_DATA == DateTime.Today.Date)
                {
                    if (item.AGEN_HR_HORA < DateTime.Now.TimeOfDay)
                    {
                        return 1;
                    }
                    if (item.AGEN_HR_FINAL < DateTime.Now.TimeOfDay)
                    {
                        return 1;
                    }
                }
                if (item.AGEN_HR_HORA > item.AGEN_HR_FINAL)
                {
                    return 3;
                }

                List<AGENDA> lista = _baseService.GetByUser(usuario.USUA_CD_ID, usuario.ASSI_CD_ID).Where(p => p.AGEN_DT_DATA == item.AGEN_DT_DATA).ToList();
                List<AGENDA> lista1 = lista.Where(p => p.AGEN_HR_HORA < item.AGEN_HR_HORA & p.AGEN_HR_FINAL > item.AGEN_HR_FINAL).ToList();
                List<AGENDA> lista2 = lista.Where(p => p.AGEN_HR_HORA < item.AGEN_HR_HORA & p.AGEN_HR_FINAL < item.AGEN_HR_FINAL & p.AGEN_HR_FINAL > item.AGEN_HR_HORA).ToList();
                List<AGENDA> lista3 = lista.Where(p => p.AGEN_HR_HORA > item.AGEN_HR_HORA & p.AGEN_HR_FINAL > item.AGEN_HR_FINAL & p.AGEN_HR_HORA < item.AGEN_HR_FINAL).ToList();
                List<AGENDA> lista4 = lista.Where(p => p.AGEN_HR_HORA > item.AGEN_HR_HORA & p.AGEN_HR_FINAL < item.AGEN_HR_FINAL).ToList();

                if (lista1.Count > 0 || lista2.Count > 0 || lista3.Count > 0 || lista4.Count > 0)
                {
                    return 2;
                }

                // Persiste
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(AGENDA item, USUARIO usuario)
        {
            try
            {
                // Ajustes
                if (item.CATEGORIA_AGENDA != null)
                {
                    item.CATEGORIA_AGENDA = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.USUARIO1 != null)
                {
                    item.USUARIO1 = null;
                }

                // Acerta campos
                item.AGEN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelAGEN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA>(item),
                    LOG_IN_SISTEMA = 2
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

        public Int32 ValidateReativar(AGENDA item, USUARIO usuario)
        {
            try
            {
                // Ajustes
                if (item.CATEGORIA_AGENDA != null)
                {
                    item.CATEGORIA_AGENDA = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.USUARIO1 != null)
                {
                    item.USUARIO1 = null;
                }

                // Verifica integridade referencial

                // Acerta campos
                item.AGEN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaAGEN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AGENDA>(item),
                    LOG_IN_SISTEMA = 2
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

        public Int32 ValidateEditAnexo(AGENDA_ANEXO item)
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

        public AGENDA_CONTATO GetContatoById(Int32 id)
        {
            AGENDA_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public Int32 ValidateEditContato(AGENDA_CONTATO item)
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

        public Int32 ValidateCreateContato(AGENDA_CONTATO item)
        {
            try
            {
                // Persiste
                item.AGCO_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
