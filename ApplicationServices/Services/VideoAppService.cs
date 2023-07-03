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
    public class VideoAppService : AppServiceBase<VIDEO>, IVideoAppService
    {
        private readonly IVideoService _baseService;

        public VideoAppService(IVideoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<VIDEO> GetAllItens(Int32 idAss)
        {
            List<VIDEO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<VIDEO> GetAllItensAdm(Int32 idAss)
        {
            List<VIDEO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public VIDEO GetItemById(Int32 id)
        {
            VIDEO item = _baseService.GetItemById(id);
            return item;
        }

        public List<VIDEO> GetAllItensValidos(Int32 idAss)
        {
            List<VIDEO> lista = _baseService.GetAllItensValidos(idAss);
            return lista;
        }

        public VIDEO_COMENTARIO GetComentarioById(Int32 id)
        {
            VIDEO_COMENTARIO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Tuple<Int32, List<VIDEO>, Boolean> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss)
        {
            try
            {
                List<VIDEO> objeto = new List<VIDEO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(titulo, autor, data, texto, link, idAss);
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

        public Int32 ValidateCreate(VIDEO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.VIDE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddVIDE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VIDEO>(item)
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

        public Int32 ValidateEdit(VIDEO item, VIDEO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditVIDE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VIDEO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<VIDEO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(VIDEO item, VIDEO itemAntes)
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

        public Int32 ValidateDelete(VIDEO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.VIDE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelVIDE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VIDEO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(VIDEO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.VIDE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatVIDE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VIDEO>(item)
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
