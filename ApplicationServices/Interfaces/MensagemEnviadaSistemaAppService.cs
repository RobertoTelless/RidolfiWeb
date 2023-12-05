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
    public class MensagemEnviadaSistemaAppService : AppServiceBase<MENSAGENS_ENVIADAS_SISTEMA>, IMensagemEnviadaSistemaAppService
    {
        private readonly IMensagemEnviadaSistemaService _baseService;
        private readonly IConfiguracaoService _confService;

        public MensagemEnviadaSistemaAppService(IMensagemEnviadaSistemaService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> GetAllItens(Int32 idAss)
        {
            List<MENSAGENS_ENVIADAS_SISTEMA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public MENSAGENS_ENVIADAS_SISTEMA GetItemById(Int32 id)
        {
            MENSAGENS_ENVIADAS_SISTEMA item = _baseService.GetItemById(id);
            return item;
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> GetByDate(DateTime data, Int32 idAss)
        {
            List<MENSAGENS_ENVIADAS_SISTEMA> item = _baseService.GetByDate(data, idAss);
            return item;
        }

        public Tuple<Int32, List<MENSAGENS_ENVIADAS_SISTEMA>, Boolean> ExecuteFilterTuple(Int32? escopo, Int32? tipo, DateTime? inicio, DateTime? final, Int32? usuario, String titulo, String origem, Int32 idAss)
        {
            try
            {
                List<MENSAGENS_ENVIADAS_SISTEMA> objeto = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(escopo, tipo, inicio, final, usuario, titulo, origem, idAss);
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

        public Int32 ValidateCreate(MENSAGENS_ENVIADAS_SISTEMA item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
